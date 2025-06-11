using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Orpius.Platform.Generators;

/// <summary>
/// Produces a concrete <c>IToolRegistryItem</c> for assemblies with
/// <c>[assembly: GenerateToolRegistryItemAttribute("Namespace.ClassName")]</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ToolRegistryItemGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		/* Detect [GenerateToolRegistryItem] */
		static AssemblyInfo SelectAssemblies(Compilation compilation, CancellationToken _)
		{
			INamedTypeSymbol? attribute = compilation.GetTypeByMetadataName(
				"Orpius.Platform.Tooling.GenerateToolRegistryItemAttribute");

			if (attribute is null)
			{
				return AssemblyInfo.None;
			}

			foreach (AttributeData ad in compilation.Assembly.GetAttributes())
			{
				if (!SymbolEqualityComparer.Default.Equals(ad.AttributeClass, attribute))
				{
					continue;
				}

				if (ad.ConstructorArguments.Length != 1 ||
					ad.ConstructorArguments[0].Value is not string fullClassName)
				{
					continue; // malformed usage
				}

				ImmutableHashSet<IAssemblySymbol>.Builder keep 
					= ImmutableHashSet.CreateBuilder<IAssemblySymbol>(SymbolEqualityComparer.Default);

				/* Include the current assembly by default.
				   We eject this if ScanAssembliesContaining is set. */
				keep.Add(compilation.Assembly);

				// See if the named argument ScanAssembliesContaining was supplied.
				KeyValuePair<string, TypedConstant> pair 
					= ad.NamedArguments.FirstOrDefault(kv => kv.Key == "ScanAssembliesContaining");

				if (!string.IsNullOrEmpty(pair.Key))
				{
					TypedConstant typedConstant = pair.Value;

					if (typedConstant.Kind == TypedConstantKind.Array && typedConstant.Values.Length > 0)
					{
						/* The user explicitly named assemblies,
						   therefore do not keep the default one. */
						keep.Clear();

						foreach (TypedConstant tc in typedConstant.Values)
						{
							if (tc.Value is INamedTypeSymbol marker)
							{
								keep.Add(marker.ContainingAssembly);
							}
						}
					}
				}

				return new AssemblyInfo(true, fullClassName, keep.ToImmutable());
			}

			return AssemblyInfo.None;
		}

		IncrementalValueProvider<AssemblyInfo> assemblyInfo 
			= context.CompilationProvider.Select(SelectAssemblies);

		/* Collect every [Tool] type */
		IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> toolTypes
			= context.SyntaxProvider
					 .ForAttributeWithMetadataName(
						 "Orpius.Platform.Tooling.ToolAttribute",
						 static (_, _) => true,
						 static (context, _) => (INamedTypeSymbol)context.TargetSymbol)
					 .Collect();

		/* Combine and generate */
		context.RegisterSourceOutput(
			assemblyInfo.Combine(toolTypes),
			static (spc, pair) =>
			{
				AssemblyInfo assemblyInfo = pair.Left;
				ImmutableArray<INamedTypeSymbol> tools = pair.Right;

				if (!assemblyInfo.Enabled)
				{
					return; // nothing to generate
				}

				try
				{
					GeneratorModel model = GeneratorModel.Create(assemblyInfo.FullClassName!, tools);
					string source = SourceBuilder.Build(model);

					spc.AddSource($"{model.ClassName}.g.cs",
								  SourceText.From(source, Encoding.UTF8));
				}
				catch (Exception ex)
				{
					spc.ReportDiagnostic(Diagnostic.Create(
						new DiagnosticDescriptor(
							"OPG070",
							"ToolRegistryItemGenerator error",
							"Generator threw an exception: {0}",
							"ToolRegistry",
							DiagnosticSeverity.Error,
							true),
						Location.None,
						ex.Message));
				}
			});
	}

	sealed record AssemblyInfo(bool Enabled, 
							   string? FullClassName, 
							   ImmutableHashSet<IAssemblySymbol> AllowedAssemblies)
	{
		public static readonly AssemblyInfo None 
			= new(false, null, ImmutableHashSet<IAssemblySymbol>.Empty);
	}

	#region Tool / Method model
	sealed record ToolMethodModel(
		string ExposedName,
		string MemberName,
		string? Description,
		ITypeSymbol ParameterType,
		ITypeSymbol ReturnType);

	sealed record ToolModel(
		string ExposedName,
		INamedTypeSymbol Symbol,
		IReadOnlyList<ToolMethodModel> Methods);
	#endregion

	#region Contract model hierarchy
	abstract record ContractModel(ITypeSymbol Type)
	{
		protected static readonly SymbolDisplayFormat Fqn = SymbolDisplayFormat.FullyQualifiedFormat;
		protected string TypeOf => $$"""typeof({{Type.ToDisplayString(Fqn)}}).FullName!""";
		internal abstract string Expression { get; }
	}

	sealed record SimpleContractModel(ITypeSymbol T) : ContractModel(T)
	{
		internal override string Expression =>
			$$"""new ContractMessage { SimpleContract = new SimpleContractMessage({{TypeOf}}) }""";
	}

	sealed record ListContractModel(ITypeSymbol T) : ContractModel(T)
	{
		internal override string Expression =>
			$$"""new ContractMessage { ListContract = new ListContractMessage({{TypeOf}}) }""";
	}

	sealed record EnumContractModel(INamedTypeSymbol Enum) : ContractModel(Enum)
	{
		internal override string Expression =>
			$$"""new ContractMessage { EnumContract = new EnumContractMessage({{TypeOf}}, enumConverter.GetEnumDictionary<{{Enum.ToDisplayString(Fqn)}}>() ) }""";
	}

	sealed record ComplexContractModel(
		INamedTypeSymbol Complex,
		IReadOnlyList<IPropertySymbol> PropertySymbols) : ContractModel(Complex)
	{
		internal override string Expression
		{
			get
			{
				var sb = new StringBuilder();
				sb.Append($$"""
					
								new ContractMessage
								{
									ComplexContract = new ComplexContractMessage({{TypeOf}})
									{
										Properties = new List<ContractPropertyMessage>
										{
					""");

				foreach (IPropertySymbol p in PropertySymbols)
				{
					AttributeData ad = p.GetAttributes().First(
						a => a.AttributeClass?.Name is "ToolPropertyAttribute" or "ToolStringPropertyAttribute");

					string displayName = ad.NamedArguments.FirstOrDefault(pair => pair.Key == "Name").Value.Value as string
										 ?? p.Name;

					bool required = (bool?)ad.NamedArguments.FirstOrDefault(k => k.Key == "Required").Value.Value
									?? false;

					string? description = ad.NamedArguments.FirstOrDefault(pair => pair.Key == "Description").Value.Value as string;

					string? format = ad.NamedArguments.FirstOrDefault(pair => pair.Key == "OpenApiFormat").Value.Value as string;

					INamedTypeSymbol? repAs = ad.NamedArguments.FirstOrDefault(pair => pair.Key == "RepresentAs").Value.Value
												  as INamedTypeSymbol;

					sb.Append($$"""
									
											new ContractPropertyMessage("{{displayName}}", typeof({{p.Type.ToDisplayString(Fqn)}}).FullName!)
											{
					
					""");

					if (required)
					{
						sb.Append("""
													Required = true,
						
						""");
					}
					if (description is not null)
					{
						sb.Append($$"""
													Description = "{{description.Replace("\"", "\\\"")}}", 
						
						""");
					}
					if (format is not null)
					{
						sb.Append($$"""
													OpenApiFormat = "{{format}}", 
						
						""");
					}
					if (repAs is not null)
					{
						sb.Append($$"""
													RepresentAs = typeof({{repAs.ToDisplayString(Fqn)}}).FullName!, 
						
						""");
					}

					sb.Append(
						"""
						
												},
						""");
				}

				sb.Append("""
					
										}
									}
								}
					""");
				return sb.ToString();
			}
		}
	}
	#endregion

	#region GeneratorModel + collectors
	sealed record GeneratorModel(string Namespace,
								 string ClassName,
								 IReadOnlyList<ToolModel> Tools,
								 IReadOnlyList<ContractModel> Contracts)
	{
		internal static GeneratorModel Create(
			string fullClassName,
			ImmutableArray<INamedTypeSymbol> toolSymbols)
		{
			int dotIndex = fullClassName.LastIndexOf('.');

			string namespacePart = dotIndex > 0 
									   ? fullClassName.Substring(0, dotIndex)
									   : string.Empty;

			string classNamePart = dotIndex > 0 
									   ? fullClassName.Substring(dotIndex + 1)
									   : fullClassName;

			ContractCollector collector = new();

			List<ToolModel> tools = new();
			foreach (INamedTypeSymbol t in toolSymbols)
			{
				tools.Add(BuildTool(t, collector));
			}

			return new GeneratorModel(namespacePart, classNamePart, tools, collector.All());
		}

		static ToolModel BuildTool(INamedTypeSymbol tool,
										   ContractCollector collector)
		{
			string exposedName 
				= tool.GetAttributes()
					 .First(attributeData => attributeData.AttributeClass?.Name == "ToolAttribute")
					 .NamedArguments
					 .FirstOrDefault(kv => kv.Key == "Name").Value.Value as string ?? tool.Name;

			List<ToolMethodModel> methods = new();

			foreach (IMethodSymbol m in tool.GetMembers().OfType<IMethodSymbol>())
			{
				if (!m.GetAttributes().Any(a => a.AttributeClass?.Name == "ToolMethodAttribute"))
				{
					continue;
				}

				AttributeData attr = m.GetAttributes()
									  .First(a => a.AttributeClass?.Name == "ToolMethodAttribute");

				string methodName =
					attr.NamedArguments.FirstOrDefault(kv => kv.Key == "Name").Value.Value
					as string ?? m.Name;

				string? description =
					attr.NamedArguments.FirstOrDefault(kv => kv.Key == "Description").Value.Value
					as string;

				// must have exactly 2 parameters
				if (m.Parameters.Length != 2)
				{
					throw new InvalidOperationException(
						$"Tool method '{m.Name}' in '{tool.Name}' must have exactly two parameters " +
						"(request, ICombinedContext).");
				}

				// second parameter must be ICombinedContext  (interface name comparison is enough
				// because the generator runs in the same compilation)
				if (m.Parameters[1].Type.Name != "ICombinedContext")
				{
					throw new InvalidOperationException(
						$"The second parameter of '{m.Name}' in '{tool.Name}' must be ICombinedContext.");
				}

				ITypeSymbol paramType = m.Parameters[0].Type;

				// return type must be Task<T>
				if (m.ReturnType is not INamedTypeSymbol taskType
					|| taskType.Name != "Task" 
					|| !taskType.IsGenericType 
					|| taskType.TypeArguments.Length != 1)
				{
					throw new InvalidOperationException(
						$"Tool method '{m.Name}' in '{tool.Name}' must return Task<T> (not Task).");
				}

				ITypeSymbol returnType = taskType.TypeArguments[0];  // the "T"

				collector.Add(paramType);
				collector.Add(returnType);

				methods.Add(new ToolMethodModel(methodName, m.Name, description, paramType, returnType));
			}

			return new ToolModel(exposedName, tool, methods);
		}
	}

	sealed class ContractCollector
	{
		readonly Dictionary<ITypeSymbol, ContractModel> map =
			new(SymbolEqualityComparer.Default);

		public void Add(ITypeSymbol t)
		{
			Visit(t);
		}

		public IReadOnlyList<ContractModel> All() => map.Values.ToList();

		void Visit(ITypeSymbol t)
		{
			if (map.ContainsKey(t))
			{
				return;
			}

			/* enum */
			if (t is INamedTypeSymbol en && en.TypeKind == TypeKind.Enum)
			{
				map[t] = new EnumContractModel(en);
				return;
			}

			/* primitive */
			if (t.SpecialType != SpecialType.None)
			{
				map[t] = new SimpleContractModel(t);
				return;
			}

			/* array */
			if (t is IArrayTypeSymbol arr)
			{
				Visit(arr.ElementType);
				map[t] = new ListContractModel(arr);
				return;
			}

			/* generic list */
			if (t is INamedTypeSymbol nt &&
				nt.IsGenericType &&
				nt.ConstructedFrom?.ToDisplayString() is
					"System.Collections.Generic.IList<T>" or
					"System.Collections.Generic.List<T>" or
					"System.Collections.Generic.IEnumerable<T>")
			{
				Visit(nt.TypeArguments[0]);
				map[t] = new ListContractModel(nt);
				return;
			}

			/* complex */
			if (t is INamedTypeSymbol complex)
			{
				List<IPropertySymbol> propertySymbols 
					= complex.GetMembers()
							 .OfType<IPropertySymbol>()
							 .Where(symbol => symbol.GetAttributes()
										  .Any(data => data.AttributeClass?.Name
													 is "ToolPropertyAttribute"
													 or "ToolStringPropertyAttribute"))
							 .ToList();

				foreach (IPropertySymbol p in propertySymbols)
				{
					Visit(p.Type);
				}

				map[t] = new ComplexContractModel(complex, propertySymbols);
				return;
			}

			/* fallback */
			map[t] = new SimpleContractModel(t);
		}
	}
	#endregion

	#region SourceBuilder
	static class SourceBuilder
	{
		static readonly SymbolDisplayFormat fqn = SymbolDisplayFormat.FullyQualifiedFormat;

		internal static string Build(GeneratorModel m)
		{
			var sb = new StringBuilder();

			sb.Append("""
				// <auto-generated />
				
				using System;
				using System.Collections.Generic;
				using System.Threading.Tasks;
				
				using Orpius.Platform.Text.Json;
				using Orpius.Platform.Tooling;
				using Orpius.Platform.Tooling.RpcToolProviderService;
				using Orpius.Platform.Tooling.RpcToolsRegistrationService;
				using Orpius.Platform.Tooling.ToolRegistration;
				using Orpius.Platform.Tooling.Utilities;
				
				""");

			if (m.Namespace.Length > 0)
			{
				sb.Append($$"""

				namespace {{m.Namespace}}
				{
				""");
			}
			
			/* class header & ctor */
			sb.Append($$"""

				partial class {{m.ClassName}} : IToolRegistryItem
				{
					public {{m.ClassName}}()
					{
						toolsMetadata = new Lazy<IToolsMetadata>(GetMetadata);
						toolInvoker   = new Lazy<GeneratedToolInvoker>(() => new GeneratedToolInvoker());
					}

					public {{m.ClassName}}(IToolRegistry registry) : this()
					{
						registry.AddItem(this);
					}

					readonly Lazy<GeneratedToolInvoker> toolInvoker;
					public  IToolInvoker ToolInvoker => toolInvoker.Value;

					readonly Lazy<IToolsMetadata> toolsMetadata;
					public  IToolsMetadata ToolsMetadata => toolsMetadata.Value;
				""");

			/* GetMetadata body */
			sb.Append("""

					IToolsMetadata GetMetadata()
					{
				""");

			foreach (ToolModel t in m.Tools)
			{
				string var = SafeVar(t.Symbol);

				sb.Append($$"""
				
						ToolMessage {{var}} = new ToolMessage(
							toolName: "{{t.ExposedName}}",
							typeName: typeof({{t.Symbol.ToDisplayString(fqn)}}).FullName!)
						{
							Methods = new List<ToolMethodMessage>
							{
				""");

				foreach (ToolMethodModel mm in t.Methods)
				{
					string descArg = mm.Description is null
						? "description: null"
						: $"description: \"{mm.Description.Replace("\"", "\\\"")}\"";

					sb.Append($$"""
								new ToolMethodMessage(
									methodName: "{{mm.ExposedName}}",
									parameterContractTypeName: typeof({{mm.ParameterType.ToDisplayString(fqn)}}).FullName!,
									returnsContractTypeName:   typeof({{mm.ReturnType.ToDisplayString(fqn)}}).FullName!,
									{{descArg}}),
				""");
				}

				sb.Append("""
				
							}
						};
						
				""");
			}

			sb.Append("""

						var enumConverter = new EnumToDictionaryConverter();

						var contracts = new List<ContractMessage>
						{
				""");

			foreach (ContractModel c in m.Contracts)
			{
				sb.Append($$"""
				
							{{c.Expression}},
				""");
			}

			sb.Append($$"""

						};

						return new ToolsMetadata(
							new ToolMessage[] { {{string.Join(", ", m.Tools.Select(t => SafeVar(t.Symbol)))}} },
							contracts);
					}
				""");

			/* ToolInvoker */
			sb.Append("""

					public sealed class GeneratedToolInvoker : IToolInvoker
					{
						readonly HashSet<string> toolNames = new HashSet<string>
						{
				""");

			foreach (ToolModel t in m.Tools)
			{
				sb.Append($$"""
							"{{t.ExposedName}}",
				""");
			}

			sb.Append($$"""

						};

						public bool IsInvokerForTool(string toolName)
						{
							return toolNames.Contains(toolName);
						}

						public bool RemoveTool(string toolName)
						{
							return toolNames.Remove(toolName);
						}

						public IReadOnlyCollection<string> ToolNames => toolNames;

						public async Task<UseToolResponse> InvokeToolAsync(
							UseToolRequest request,
							ICombinedContext context,
							IJsonSerializer serializer,
							IToolResolver resolver)
						{
							if (string.IsNullOrWhiteSpace(request.ParameterAsJson))
							{
								throw new ArgumentException("ParameterAsJson must not be null or whitespace.");
							}

							switch (request.ToolName)
							{
							
				""");

			foreach (ToolModel t in m.Tools)
			{
				sb.Append($$"""
				
								case "{{t.ExposedName}}":
								{
									if (!resolver.TryGetTool<{{t.Symbol.ToDisplayString(fqn)}}>(out var tool))
									{
										throw new ArgumentOutOfRangeException($"No tool registered with name '{{t.ExposedName}}'.");
									}

									switch (request.ToolMember)
									{
				""");

				foreach (ToolMethodModel mm in t.Methods)
				{
					sb.Append($$"""
				
										case "{{mm.ExposedName}}":
										{
											var parameter  = serializer.Deserialize<{{mm.ParameterType.ToDisplayString(fqn)}}>(request.ParameterAsJson!);
											var result = await tool.{{mm.MemberName}}(parameter, context);
											return new UseToolResponse { ResultAsJson = serializer.Serialize(result) };
										}
				""");
				}

				sb.Append("""
				
										default:
										{
											throw new ArgumentException($"Member '{request.ToolMember}' not found for tool '{request.ToolName}'.");
										}
									}
								}
				""");
			}

			sb.Append("""

								default:
								{
									throw new ArgumentException($"Tool '{request.ToolName}' not found.");
								}
							}
						}
					}
				""");

			/* close class & namespace */
			sb.Append("""

				}
				""");

			if (m.Namespace.Length > 0)
			{
				sb.Append("""
				
				}
				""");
			}

			return sb.ToString();

			static string SafeVar(INamedTypeSymbol s) =>
				s.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
				 .Replace('.', '_')
				 .Replace('<', '_')
				 .Replace('>', '_');
		}
	}
	#endregion
}
