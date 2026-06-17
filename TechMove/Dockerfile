# ─── Stage 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for layer caching
COPY TechMoveData/TechMoveData.csproj         TechMoveData/
COPY TechMoveRepo/TechMoveRepo.csproj         TechMoveRepo/
COPY TechMoveServices/TechMoveServices.csproj TechMoveServices/
COPY TechMove/TechMove.csproj                 TechMove/

# Restore dependencies
RUN dotnet restore TechMove/TechMove.csproj

# Copy remaining source
COPY TechMoveData/     TechMoveData/
COPY TechMoveRepo/     TechMoveRepo/
COPY TechMoveServices/ TechMoveServices/
COPY TechMove/         TechMove/

# Publish in Release mode
RUN dotnet publish TechMove/TechMove.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TechMove.dll"]
