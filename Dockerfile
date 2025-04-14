# Stage 1: Build the application
# Use the official .NET SDK image. Adjust the version (e.g., 8.0) if needed.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the solution file
COPY *.sln .

# Copy ALL necessary project files listed in the Solution (.sln) file BEFORE restore.
COPY SmartHome.API/*.csproj ./SmartHome.API/
COPY SmartHome.Application/*.csproj ./SmartHome.Application/
COPY SmartHome.Domain/*.csproj ./SmartHome.Domain/
COPY SmartHome.Infrastructure/*.csproj ./SmartHome.Infrastructure/
COPY SmartHome.Dto/*.csproj ./SmartHome.Dto/
COPY SmartHome.Enum/*.csproj ./SmartHome.Enum/
COPY SmartHome.Web/*.csproj ./SmartHome.Web/
COPY SmartHome.App/*.csproj ./SmartHome.App/
COPY SmartHome.Shared/*.csproj ./SmartHome.Shared/
# Copy NuGet.config if you have one
# COPY NuGet.config .

# Restore step is removed as we let publish handle dependencies per project

# Copy the rest of the source code
COPY . .

# --- Publish SmartHome.API ---
WORKDIR /source
RUN dotnet publish "./SmartHome.API/SmartHome.API.csproj" -c Release -o /app/publish-api

# --- Publish SmartHome.Web ---
WORKDIR /source
RUN dotnet publish "./SmartHome.Web/SmartHome.Web.csproj" -c Release -o /app/publish-web

# --- Stage 2: Runtime image for SmartHome.API ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api-final
WORKDIR /app
COPY --from=build /app/publish-api .
EXPOSE 62062
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:62062
# Optional Data Protection Volume for API
# VOLUME /root/.aspnet/DataProtection-Keys
ENTRYPOINT ["dotnet", "SmartHome.API.dll"]


# --- Stage 3: Runtime image for SmartHome.Web ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS web-final
WORKDIR /app

# Copy the CA certificate into the container
# Make sure ca.crt is in the build context (e.g., next to Dockerfile)
COPY ca.crt /usr/local/share/ca-certificates/SmartHomeCA.crt
# Update the OS certificate store to include the new CA cert
RUN update-ca-certificates

COPY --from=build /app/publish-web .
EXPOSE 5042
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5042
# Optional Data Protection Volume for Web (if needed)
# VOLUME /root/.aspnet/DataProtection-Keys
ENTRYPOINT ["dotnet", "SmartHome.Web.dll"]