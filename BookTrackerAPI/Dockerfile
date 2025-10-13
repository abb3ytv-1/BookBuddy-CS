# ---------- Base runtime image ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["BookTrackerAPI.csproj", "."]
RUN dotnet restore "BookTrackerAPI.csproj"

# Copy all files and build the project
COPY . .
WORKDIR "/src"
RUN dotnet build "BookTrackerAPI.csproj" -c Release -o /app/build

# ---------- Publish stage ----------
FROM build AS publish
RUN dotnet publish "BookTrackerAPI.csproj" -c Release -o /app/publish --no-restore

# ---------- Final runtime image ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Optional: set env vars if needed
# ENV ASPNETCORE_ENVIRONMENT=Production
# ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "BookTrackerAPI.dll"]
