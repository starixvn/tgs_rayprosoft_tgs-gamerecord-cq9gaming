#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tgs.GameRecord.CQ9Gaming/Tgs.GameRecord.CQ9Gaming.csproj", "Tgs.GameRecord.CQ9Gaming/"]
COPY ["Tgs.GameRecord.CQ9Gaming.Common/Tgs.GameRecord.CQ9Gaming.Common.csproj", "Tgs.GameRecord.CQ9Gaming.Common/"]
COPY ["Tgs.GameRecord.CQ9Gaming.Core/Tgs.GameRecord.CQ9Gaming.Core.csproj", "Tgs.GameRecord.CQ9Gaming.Core/"]
COPY ["Tgs.GameRecord.CQ9Gaming.Repositories/Tgs.GameRecord.CQ9Gaming.Repositories.csproj", "Tgs.GameRecord.CQ9Gaming.Repositories/"]
RUN dotnet restore "./Tgs.GameRecord.CQ9Gaming/Tgs.GameRecord.CQ9Gaming.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.CQ9Gaming"
RUN dotnet build "./Tgs.GameRecord.CQ9Gaming.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.CQ9Gaming.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.CQ9Gaming.dll"]