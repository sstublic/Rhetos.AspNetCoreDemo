dotnet publish -c Release
docker build -t rhetos-webapp-image -f Dockerfile .
