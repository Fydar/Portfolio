#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-alpine3.19-arm64v8 AS base
USER app
WORKDIR /app
EXPOSE 8060
EXPOSE 8061

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
SHELL ["/bin/bash", "-c"]
ENV TERM=xterm-256color
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fydar.Dev.WebApp/Fydar.Dev.WebApp.csproj", "Fydar.Dev.WebApp/"]
COPY ["Fydar.Dev.Services.EmailTickets/Fydar.Dev.Services.EmailTickets.csproj", "Fydar.Dev.Services.EmailTickets/"]
COPY ["Fydar.Dev.WebApp.Client/Fydar.Dev.WebApp.Client.csproj", "Fydar.Dev.WebApp.Client/"]
COPY ["Fydar.Dev.WebApp.Toolkit.Icons/Fydar.Dev.WebApp.Toolkit.Icons.csproj", "Fydar.Dev.WebApp.Toolkit.Icons/"]
RUN dotnet restore "./Fydar.Dev.WebApp/Fydar.Dev.WebApp.csproj"
COPY . .
WORKDIR "/src/Fydar.Dev.WebApp"
RUN dotnet build "./Fydar.Dev.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fydar.Dev.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fydar.Dev.WebApp.dll"]
