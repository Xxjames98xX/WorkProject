# -------------------------------------------------------------------------------
# NOTE: If using MSSQL server on host machine (NOT CONTAINERIZED) then use below connection string. Within appsettings.json
# "DefaultConnection": "Server=host.docker.internal;Database=MyWebAppDb;TrustServerCertificate=True;User Id=myuser;Password=password;"

# NOTE: if using containerized MSSQL server then use below connection string. Within appsettings.json
# Using System Admin (Sa) user for simplicity. In production use a different user with limited permissions.
# "DefaultConnection": Server=sqlserver;Database=MyWebAppDb;User Id=sa;Password=Password@123;TrustServerCertificate=True;
# -------------------------------------------------------------------------------

# This Dockerfile is used to build and run the ASP.NET Web API application.

# The application is built using the same .Net SDK version as the base development environment.
# In which I am curretly using .Net 8.0 SDK.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire directory project file and restore any dependencies (via nuget)
# In this case refer to the ASPWebAPI.csproj
# Docker file is in the parent directory of ASPWebAPI project folder hence ASPWebAPI/ASPWebAPI.csproj
COPY ASPWebAPI.csproj ./
RUN dotnet restore "ASPWebAPI.csproj"
# Install additional dependencies for Kerberos authentication -> Linux docker image
RUN apt-get update && apt-get install -y libgssapi-krb5-2

# Copy the rest of the code
COPY . ./
# Build the project and publish the output to the /app/publish directory
RUN dotnet publish "ASPWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
# The --from=build is from the earlier FROM stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish . 
ENTRYPOINT ["dotnet", "ASPWebAPI.dll"]

# Apply Migrations on startup also
CMD ["sh", "-c", "dotnet ef database update && dotnet YourWebApi.dll"]