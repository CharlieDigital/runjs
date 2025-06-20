# See the ./build-server.sh script for the instructions to build this
# I am building on a Mac M1 so this requires buildx in most cases
# See: https://devblogs.microsoft.com/dotnet/improving-multiplatform-container-support/

# The build image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# See: https://docs.docker.com/engine/reference/builder/#automatic-platform-args-in-the-global-scope
ARG TARGETARCH
ARG BUILDPLATFORM

COPY ./server ./
RUN dotnet publish ./runjs-server.csproj \
  --output /app/published-app \
  --configuration Release \
  -a $TARGETARCH

# The runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/published-app /app

# TODO: Add options here for configuring Jint
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the MCP server
ENTRYPOINT [ "dotnet", "/app/runjs-server.dll" ]
