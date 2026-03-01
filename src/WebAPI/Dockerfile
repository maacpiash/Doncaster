FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Domain/Todo.Domain.csproj src/Domain/
COPY src/Application/Todo.Application.csproj src/Application/
COPY src/Infrastructure/Todo.Infrastructure.csproj src/Infrastructure/
COPY src/ServiceDefaults/Todo.ServiceDefaults.csproj src/ServiceDefaults/
COPY src/WebAPI/Todo.WebAPI.csproj src/WebAPI/
RUN dotnet restore src/WebAPI/Todo.WebAPI.csproj

COPY src/Domain/ src/Domain/
COPY src/Application/ src/Application/
COPY src/Infrastructure/ src/Infrastructure/
COPY src/ServiceDefaults/ src/ServiceDefaults/
COPY src/WebAPI/ src/WebAPI/
RUN dotnet publish src/WebAPI/Todo.WebAPI.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Todo.WebAPI.dll"]
