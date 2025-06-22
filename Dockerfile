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

ENV ASPNETCORE_ENVIRONMENT=Production

# 👇 Override as you need!
ENV RunJSConfig__Jint__LimitMemory=5000000
ENV RunJSConfig__Jint__TimeoutIntervalSeconds=10
ENV RunJSConfig__Jint__MaxStatements=1000
ENV RunJSConfig__Secrets__UseDatabase=false
ENV RunJSConfig__Db__ConnectionString=YOUR_CONNECTION_STRING

# Start the MCP server
ENTRYPOINT [ "dotnet", "/app/runjs-server.dll" ]
