FROM microsoft/dotnet:latest
COPY . /app
WORKDIR /app
 
RUN ["dotnet", "restore", "--source", "https://api.nuget.org/v3/index.json", "--source", "https://www.myget.org/F/collectively-dev/api/v3/index.json", "--no-cache"]
RUN ["dotnet", "build"]
 
EXPOSE 10002/tcp
ENV ASPNETCORE_URLS http://*:10002
ENV ASPNETCORE_ENVIRONMENT docker
 
ENTRYPOINT ["dotnet", "run"]
