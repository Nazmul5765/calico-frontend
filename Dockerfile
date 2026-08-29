FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "lofi-frontend/lofi-frontend/lofi-frontend.csproj"

RUN dotnet publish "lofi-frontend/lofi-frontend/lofi-frontend.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

CMD ["sh", "-c", "dotnet lofi-frontend.dll --urls http://0.0.0.0:${PORT:-8080}"]
