FROM node:22-alpine AS frontend-build
WORKDIR /frontend
COPY src/Frontend/package*.json ./
RUN npm ci
COPY src/Frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Domain/Todo.Domain.csproj src/Domain/
COPY src/Application/Todo.Application.csproj src/Application/
COPY src/Infrastructure/Todo.Infrastructure.csproj src/Infrastructure/
COPY src/ServiceDefaults/Todo.ServiceDefaults.csproj src/ServiceDefaults/
COPY src/Gateway/Todo.Gateway.csproj src/Gateway/
RUN dotnet restore src/Gateway/Todo.Gateway.csproj

COPY src/Domain/ src/Domain/
COPY src/Application/ src/Application/
COPY src/Infrastructure/ src/Infrastructure/
COPY src/ServiceDefaults/ src/ServiceDefaults/
COPY src/Gateway/ src/Gateway/
RUN dotnet publish src/Gateway/Todo.Gateway.csproj -c Release -o /app/publish -p:SkipFrontendCopy=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=frontend-build /frontend/dist/Frontend/browser ./wwwroot/
# LettuceEncrypt stores certificates here (mounted as Docker volume)
RUN mkdir -p /app/certs
ENTRYPOINT ["dotnet", "Todo.Gateway.dll"]
