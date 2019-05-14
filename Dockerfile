# Using microsoft dotnet software development kit as 
# the build envionment.
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY GirafRest/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./GirafRest/ ./

# Build the app for production
RUN dotnet publish -c Release -o out

#------------------------------------------#

# Using microsoft aps net core 2.2 as hosting envionment
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime-env
WORKDIR /srv

# COPY from build envionment into local container.
COPY --from=build-env /app .

# Remove the appsettings files from the container 
# so no passwords are pushed to docker hub
RUN rm appsettings*

# Expose the port intented for communications
EXPOSE 5000

# Start running the app.
ENTRYPOINT ["dotnet", "out/GirafRest.dll", "--list"]
