# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Samples/Containerized/ToolHosting_AspNetCore/ToolHosting_AspNetCore/ToolHosting_AspNetCore.csproj", "Samples/Containerized/ToolHosting_AspNetCore/ToolHosting_AspNetCore/"]
COPY ["Orpius.Platform.ClientSdk.ProtobufNet/Orpius.Platform.ClientSdk.ProtobufNet.csproj", "Orpius.Platform.ClientSdk.ProtobufNet/"]
COPY ["Orpius.Platform.ClientSdk.ProtobufNet.Generators/Orpius.Platform.ClientSdk.ProtobufNet.Generators.csproj", "Orpius.Platform.ClientSdk.ProtobufNet.Generators/"]

RUN dotnet restore "Samples/Containerized/ToolHosting_AspNetCore/ToolHosting_AspNetCore/ToolHosting_AspNetCore.csproj"
COPY . .

WORKDIR "/src/Samples/Containerized/ToolHosting_AspNetCore/ToolHosting_AspNetCore"
RUN dotnet build "ToolHosting_AspNetCore.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Samples/Containerized/ToolHosting_AspNetCore/ToolHosting_AspNetCore"
RUN dotnet publish "./ToolHosting_AspNetCore.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToolHosting_AspNetCore.dll"]