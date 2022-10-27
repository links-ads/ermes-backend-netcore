#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
LABEL stage=builder
RUN apt-get update && apt-get install -y libgdiplus  #See https://github.com/dotnet/dotnet-docker/issues/618
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Ermes.Web/Ermes.Web.csproj", "src/Ermes.Web/"]
COPY ["src/Ermes.EntityFrameworkCore/Ermes.EntityFrameworkCore.csproj", "src/Ermes.EntityFrameworkCore/"]
COPY ["src/Ermes.Core/Ermes.Core.csproj", "src/Ermes.Core/"]
COPY ["src/Ermes.Application/Ermes.Application.csproj", "src/Ermes.Application/"]
COPY ["src/FusionAuthNetCore/FusionAuthNetCore.csproj", "src/FusionAuthNetCore/"]
RUN dotnet restore "src/Ermes.Web/Ermes.Web.csproj"
COPY . .
WORKDIR "/src/src/Ermes.Web"
RUN dotnet build "Ermes.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ermes.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ermes.Web.dll"]
