# Using microsoft .NET 6.0 software development kit as 
# the build envionment.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy the project to the container
COPY . /app

# Build the app for production
RUN dotnet publish -c Release -o out

#------------------------------------------#

# Use microsoft ASP.NET 6.0 as the runtime envionment
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime-env
WORKDIR /srv

# COPY the built files from the build environment into a local container
COPY --from=build-env /app/out .

# TODO: ALSO GET THE APPSETTINGS FILE FROM THE BUILD CONTAINER

# Remove the appsettings files from the container
# so no passwords are pushed to docker hub
#RUN rm GirafRest/appsettings*

# Expose the port intented for communications
EXPOSE 5000

# Start running the app
ENTRYPOINT ["dotnet", "GirafRest.dll", "--list"]
