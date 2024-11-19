FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble AS base
EXPOSE 80
RUN apt-get update && apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish --configuration Release --output /app/publish /p:UseAppHost=false src/Server/Server.csproj

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV Refrase__DataDirectory=/data
ENV AspNetCore_Urls=http://+:80
ENTRYPOINT exec setpriv --reuid=${PUID} --regid=${PGID} --clear-groups dotnet Refrase.Server.dll
