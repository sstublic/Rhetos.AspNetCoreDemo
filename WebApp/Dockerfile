FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0/publish/ App/
COPY bin/Release/net5.0/publish/appsettings.Development.json App/appsettings.json
COPY bin/Release/net5.0/publish/rhetos-app.docker.settings.json App/rhetos-app.local.settings.json

WORKDIR /App
EXPOSE 80
ENTRYPOINT ["dotnet", "WebApp.dll"]
