# ─── Stage 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for layer caching
COPY TechMoveData/TechMoveData.csproj         TechMoveData/
COPY TechMoveRepo/TechMoveRepo.csproj         TechMoveRepo/
COPY TechMoveServices/TechMoveServices.csproj TechMoveServices/
COPY TechMove.API/TechMove.API.csproj         TechMove.API/

# Restore dependencies (cached layer if .csproj unchanged)
RUN dotnet restore TechMove.API/TechMove.API.csproj

# Copy the rest of the source code
COPY TechMoveData/     TechMoveData/
COPY TechMoveRepo/     TechMoveRepo/
COPY TechMoveServices/ TechMoveServices/
COPY TechMove.API/     TechMove.API/

# Publish in Release mode to /app/publish
RUN dotnet publish TechMove.API/TechMove.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create uploads directory
RUN mkdir -p /app/wwwroot/uploads

# Copy only the published output from the build stage
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TechMove.API.dll"]
