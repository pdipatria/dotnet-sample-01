# Use an SDK image as your build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /App

# Copy specific files from the src directory into the container directory
COPY ./src/MyProject.Api ./

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out

# Use a runtime image as your final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Set working directory to the correct location within the final container
WORKDIR /App

# Copy all necessary files from the build stage into the final container
COPY --from=build /App/out .

ENV ASPNETCORE_URLS="https://+;http://+" \
 ASPNETCORE_HTTPS_PORTS=443 \
 ASPNETCORE_Kestrel__Certificates__Default__Password="cab247b3-2e96-45e0-8d11-82a494a4b75a" \
 ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

ENTRYPOINT ["dotnet", "MyProject.Api.dll"]
