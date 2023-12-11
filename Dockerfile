# Using Microsoft .NET 8.0 SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the solution to the container
COPY . .

# Restore dependencies for the solution
RUN dotnet restore

# Build the GirafAPI project for production
RUN dotnet publish GirafAPI/GirafAPI.csproj -c Release -o /app/out

#------------------------------------------#

# Use Microsoft ASP.NET 8.0 as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env
WORKDIR /app

# Copy the built files from the build environment into a local container
COPY --from=build-env /app/out .

#Copy files for sample data to common directory
COPY GirafRepositories/Persistence/ /GirafRepositories/Persistence/


# Expose the port intended for communications
EXPOSE 5000

# Start running the GirafAPI
ENTRYPOINT ["dotnet", "GirafAPI.dll", "--sample-data"]
