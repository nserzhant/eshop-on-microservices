#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.3 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.203 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EShop.Ordering.Api/EShop.Ordering.Api.csproj", "src/EShop.Ordering.Api/"]
RUN dotnet restore "./src/EShop.Ordering.Api/EShop.Ordering.Api.csproj"
COPY . .
WORKDIR "/src/src/EShop.Ordering.Api"
RUN dotnet build "./EShop.Ordering.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EShop.Ordering.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.Ordering.Api.dll"]