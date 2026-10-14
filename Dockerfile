# --- build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Directory.Build.props VisionLab.sln ./
COPY src/VisionLab.Domain/VisionLab.Domain.csproj src/VisionLab.Domain/
COPY src/VisionLab.Application/VisionLab.Application.csproj src/VisionLab.Application/
COPY src/VisionLab.Infrastructure/VisionLab.Infrastructure.csproj src/VisionLab.Infrastructure/
COPY src/VisionLab.Api/VisionLab.Api.csproj src/VisionLab.Api/
RUN dotnet restore src/VisionLab.Api/VisionLab.Api.csproj
COPY src/ src/
RUN dotnet publish src/VisionLab.Api/VisionLab.Api.csproj -c Release -o /app --no-restore

# --- runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "VisionLab.Api.dll"]
