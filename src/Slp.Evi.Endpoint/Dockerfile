# Stage 1: Build image
FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /app

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet publish --output /out/ --configuration Release

# Stage 2: Runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT [ "dotnet", "Slp.Evi.Endpoint.dll" ]