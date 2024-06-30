FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY Directory.Build.props ./
COPY Directory.Packages.props ./

COPY src/WebAPI/WebAPI.csproj ./src/WebAPI/
COPY src/Application/Application.csproj ./src/Application/
COPY src/Domain/Domain.csproj ./src/Domain/
COPY src/Infrastructure/Infrastructure.csproj ./src/Infrastructure/

RUN dotnet new sln --name SmartFridgeManagerAPI && \
    dotnet sln add src/WebAPI/WebAPI.csproj && \
    dotnet sln add src/Application/Application.csproj && \
    dotnet sln add src/Domain/Domain.csproj && \
    dotnet sln add src/Infrastructure/Infrastructure.csproj

RUN dotnet restore

COPY src/WebAPI ./src/WebAPI
COPY src/Application ./src/Application
COPY src/Domain ./src/Domain
COPY src/Infrastructure ./src/Infrastructure

RUN dotnet publish src/WebAPI/WebAPI.csproj -c Release -o out

RUN ls -la /app/out

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y wget && \
    wget -q https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh && \
    chmod +x wait-for-it.sh

COPY --from=build-env /app/out .

RUN ls -la /app

ENV ASPNETCORE_URLS=http://+:80
ENV WAIT_TIMEOUT=60

EXPOSE 80

ENTRYPOINT ["sh", "-c", "\
    ./wait-for-it.sh ${RABBITMQ_HOSTNAME}:${RABBITMQ_PORT} --timeout=${WAIT_TIMEOUT} --strict && \
    ./wait-for-it.sh ${DB_HOSTNAME}:${DB_PORT} --timeout=${WAIT_TIMEOUT} --strict && \
    ./wait-for-it.sh ${REDIS_HOSTNAME}:${REDIS_PORT} --timeout=${WAIT_TIMEOUT} --strict && \
    dotnet SmartFridgeManagerAPI.WebAPI.dll"]