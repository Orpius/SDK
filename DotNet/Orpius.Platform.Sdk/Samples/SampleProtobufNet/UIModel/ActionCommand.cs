#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-21 15:34:42Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
// ReSharper disable ExplicitCallerInfoArgument

namespace SampleProtobufNet
{
	/// <summary>
	/// A command that uses delegates to perform its action 
	/// and determine its enabled state. 
	/// </summary>
	public class ActionCommand : ActionCommand<object>
	{
		/// <summary>
		/// Creates a synchronous command. 
		/// </summary>
		/// <param name="executeAction">
		/// The action to invoke when the command is executed.</param>
		/// <param name="canExecuteFunc">
		/// A func that determines if the command 
		/// may be performed. Can be <c>null</c>.</param>
		/// <param name="filePath">
		/// The path to the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		/// <param name="lineNumber">The line number of the file 
		/// that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public ActionCommand(
			Action<object?> executeAction,
			Func<object?, bool>? canExecuteFunc = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
			: base(executeAction, canExecuteFunc, filePath, lineNumber)
		{
		}
	}

	/// <summary>
	/// A command that uses delegates to perform its action 
	/// and determine its enabled state. 
	/// </summary>
	/// <typeparam name="TParameter">The parameter type,
	/// which may be used when executing the command or
	/// evaluating if the command can execute.</typeparam>
	public class ActionCommand<TParameter> : ViewModelBase, ICommand
	{
		readonly Action<TParameter?> executeAction;
		readonly Func<TParameter?, bool>? canExecuteFunc;

		public event EventHandler? CanExecuteChanged;

		/// <summary>
		/// Creates a synchronous command. 
		/// </summary>
		/// <param name="executeAction">The action to invoke 
		/// when the command is executed.</param>
		/// <param name="canExecuteFunc">A func that determines if the command 
		/// may be executed. Can be <c>null</c>.</param>
		/// <param name="filePath">
		/// The path to the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		/// <param name="lineNumber">The line number of the file 
		/// that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public ActionCommand(
			Action<TParameter?> executeAction,
			Func<TParameter?, bool>? canExecuteFunc = null,
			[CallerFilePath] string? filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			this.executeAction  = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
			this.canExecuteFunc = canExecuteFunc;

			if (canExecuteFunc != null)
			{
				Refresh();
			}
		}

		#region ICommand Members

		public bool CanExecute(object? parameter = null)
		{
			TParameter? coercedParameter = (TParameter?)parameter;
			
			bool result = CanExecute(coercedParameter);

			return result;
		}

		public void Execute(object? parameter = null)
		{
			executeAction((TParameter?)parameter);
		}

		/// <summary>
		/// Causes the command's execute action to be invoked.
		/// </summary>
		/// <param name="parameter">A parameter that is passed to the execute action.</param>
		public virtual void Execute(TParameter? parameter = default)
		{
			executeAction(parameter);
		}

		#endregion

		public virtual bool CanExecute(TParameter? parameter = default)
		{
			bool result;

			if (canExecuteFunc == null)
			{
				result = true;
			}
			else
			{
				result = canExecuteFunc(parameter);
			}

			Enabled = result;

			return result;
		}

		bool enabled = true;

		/// <summary>
		/// Indicates whether the command can execute.
		/// </summary>
		public bool Enabled
		{
			get => enabled;
			protected internal set => Set(ref enabled, value);
		}

		/// <summary>
		/// Refreshes the command's properties.
		/// </summary>
		/// <param name="parameter"></param>
		protected virtual void RefreshCore(TParameter? parameter)
		{
			bool canCurrentlyExecute = Enabled;
			bool valueAfterUpdate = CanExecute(parameter);

			if (canCurrentlyExecute != valueAfterUpdate)
			{
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Refreshes the command's properties.
		/// </summary>
		/// <param name="commandParameter"></param>
		public void Refresh(object? commandParameter = null)
		{
			TParameter? parameter = (TParameter?)commandParameter;
			RefreshCore(parameter);
		}
	}
}
