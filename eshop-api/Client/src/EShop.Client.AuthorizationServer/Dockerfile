#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.3 AS base
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.203 AS build
WORKDIR /src
COPY ["src/EShop.Client.AuthorizationServer/EShop.Client.AuthorizationServer.csproj", "src/EShop.Client.AuthorizationServer/"]
RUN dotnet restore "src/EShop.Client.AuthorizationServer/EShop.Client.AuthorizationServer.csproj"
COPY . .
WORKDIR "/src/src/EShop.Client.AuthorizationServer"
RUN dotnet build "EShop.Client.AuthorizationServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShop.Client.AuthorizationServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.Client.AuthorizationServer.dll"]