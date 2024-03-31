#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.3 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.203 AS build
WORKDIR /src
COPY ["src/EShop.Catalog.Api/EShop.Catalog.Api.csproj", "src/EShop.Catalog.Api/"]
COPY ["src/EShop.Catalog.Core/EShop.Catalog.Core.csproj", "src/EShop.Catalog.Core/"]
COPY ["src/EShop.Catalog.Infrastructure/EShop.Catalog.Infrastructure.csproj", "src/EShop.Catalog.Infrastructure/"]
RUN dotnet restore "src/EShop.Catalog.Api/EShop.Catalog.Api.csproj"
COPY . .
WORKDIR "/src/src/EShop.Catalog.Api"
# CMD ["-f","/dev/null"]
RUN dotnet build "EShop.Catalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShop.Catalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.Catalog.Api.dll"]