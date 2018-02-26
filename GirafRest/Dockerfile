# base image from dockerhub 
FROM microsoft/dotnet:1.1.1-sdk

# execute command to setup environment
RUN apt-get update && apt-get -y install sqlite3 && rm -rf /var/lib/apt/lists/*

# copy local files to container
COPY . /srv/

WORKDIR /srv/GirafRest
RUN dotnet restore
RUN dotnet build

# -------- #

EXPOSE 5000
ENTRYPOINT ["dotnet", "run", "--list"]