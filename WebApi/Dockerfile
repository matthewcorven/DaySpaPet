#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy each csproj file into respective folder
COPY ["./DaySpaPet.WebApi.Core/DaySpaPet.WebApi.Core.csproj", "src/DaySpaPet.WebApi.Core/"]
COPY ["./DaySpaPet.WebApi.Infrastructure/DaySpaPet.WebApi.Infrastructure.csproj", "src/DaySpaPet.WebApi.Infrastructure/"]
COPY ["./DaySpaPet.WebApi.SharedKernel/DaySpaPet.WebApi.SharedKernel.csproj", "src/DaySpaPet.WebApi.SharedKernel/"]
COPY ["./DaySpaPet.WebApi.UseCases/DaySpaPet.WebApi.UseCases.csproj", "src/DaySpaPet.WebApi.UseCases/"]
COPY ["./DaySpaPet.WebApi.Api/DaySpaPet.WebApi.Api.csproj", "src/DaySpaPet.WebApi.Api/"]

# Restore all projects, starting with the API application project
RUN dotnet restore "src/DaySpaPet.WebApi.Api/DaySpaPet.WebApi.Api.csproj"

COPY . .

# Run the build
WORKDIR "/src/DaySpaPet.WebApi.Api/"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish the build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
RUN ls -la
ENTRYPOINT ["dotnet", "DaySpaPet.WebApi.Api.dll"]