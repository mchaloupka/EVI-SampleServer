# Stage 1: Build image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
WORKDIR /app

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet publish --output /out/ --configuration Release

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT [ "dotnet", "Slp.Evi.Endpoint.dll" ]