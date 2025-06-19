# Builds the server container for deployment
docker buildx build -t runjs/server -f ./Dockerfile .
