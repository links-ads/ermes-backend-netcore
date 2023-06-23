##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update && apt-get install -y libgdiplus  #See https://github.com/dotnet/dotnet-docker/issues/618
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Ermes.Web/Ermes.Web.csproj", "src/Ermes.Web/"]
COPY ["src/Abp.BusConsumer/Abp.BusConsumer.csproj", "src/Abp.BusConsumer/"]
COPY ["src/Ermes.Core/Ermes.Core.csproj", "src/Ermes.Core/"]
COPY ["src/Abp.Azure/Abp.Azure.csproj", "src/Abp.Azure/"]
COPY ["src/Abp.BusProducer/Abp.BusProducer.csproj", "src/Abp.BusProducer/"]
COPY ["src/Abp.Chatbot/Abp.Chatbot.csproj", "src/Abp.Chatbot/"]
COPY ["src/Abp.Firebase/Abp.Firebase.csproj", "src/Abp.Firebase/"]
COPY ["src/Abp.AzureCognitiveServices/Abp.AzureCognitiveServices.csproj", "src/Abp.AzureCognitiveServices/"]
COPY ["src/Ermes.Application/Ermes.Application.csproj", "src/Ermes.Application/"]
COPY ["src/Abp.Importer/Abp.Importer.csproj", "src/Abp.Importer/"]
COPY ["src/Abp.SensorService/Abp.SensorService.csproj", "src/Abp.SensorService/"]
COPY ["src/Abp.SocialMedia/Abp.SocialMedia.csproj", "src/Abp.SocialMedia/"]
COPY ["src/Ermes.EntityFrameworkCore/Ermes.EntityFrameworkCore.csproj", "src/Ermes.EntityFrameworkCore/"]
COPY ["src/FusionAuthNetCore/FusionAuthNetCore.csproj", "src/FusionAuthNetCore/"]
RUN dotnet restore "src/Ermes.Web/Ermes.Web.csproj"
COPY . .
WORKDIR "/src/src/Ermes.Web"
RUN dotnet build "Ermes.Web.csproj" -c Release -o /app/build

########################
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS migration
WORKDIR /src
COPY ["src/Migrator/Migrator.csproj", "src/Migrator/"]
RUN dotnet restore "src/Migrator/Migrator.csproj"
COPY . .
WORKDIR "/src/src/Migrator"
RUN dotnet build "Migrator.csproj" -c Release -o /app/migration
#######################

FROM build AS publish
RUN dotnet publish "Ermes.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /migration
COPY --from=migration /app/migration .

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ermes.Web.dll"]