#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/EShop.EmployeeManagement.Api/EShop.EmployeeManagement.Api.csproj", "src/EShop.EmployeeManagement.Api/"]
COPY ["src/EShop.EmployeeManagement.Core/EShop.EmployeeManagement.Core.csproj", "src/EShop.EmployeeManagement.Core/"]
RUN dotnet restore "src/EShop.EmployeeManagement.Api/EShop.EmployeeManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/EShop.EmployeeManagement.Api"
RUN dotnet build "EShop.EmployeeManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShop.EmployeeManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.EmployeeManagement.Api.dll"]