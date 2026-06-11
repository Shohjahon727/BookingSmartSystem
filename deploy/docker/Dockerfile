FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BookingApi.Domain/BookingApi.Domain.csproj BookingApi.Domain/
COPY BookingApi.Application/BookingApi.Application.csproj BookingApi.Application/
COPY BookingApi.Infrastructure/BookingApi.Infrastructure.csproj BookingApi.Infrastructure/
COPY BookingSystem.API/BookingSystem.API.csproj BookingSystem.API/

RUN dotnet restore BookingSystem.API/BookingSystem.API.csproj

COPY . .
RUN dotnet publish BookingSystem.API/BookingSystem.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookingSystem.API.dll"]
