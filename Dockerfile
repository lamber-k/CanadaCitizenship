# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY CanadaCitizenship.Blazor/*.csproj /source/CanadaCitizenship.Blazor/
COPY CanadaCitizenship.Algorithm/*.csproj /source/CanadaCitizenship.Algorithm/
WORKDIR /source/CanadaCitizenship.Blazor
RUN dotnet restore

# copy everything else and build app
COPY CanadaCitizenship.Blazor/. /source/CanadaCitizenship.Blazor/
COPY CanadaCitizenship.Algorithm/. /source/CanadaCitizenship.Algorithm/
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM nginx
COPY --from=build /app/wwwroot /usr/share/nginx/html
