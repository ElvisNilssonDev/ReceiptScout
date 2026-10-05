# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1) Kopiera bara projektfilerna och kör restore (cachas tills beroenden ändras)
COPY src/ReceiptScout.Domain/*.csproj         src/ReceiptScout.Domain/
COPY src/ReceiptScout.Application/*.csproj    src/ReceiptScout.Application/
COPY src/ReceiptScout.Infrastructure/*.csproj src/ReceiptScout.Infrastructure/
COPY src/ReceiptScout.Api/*.csproj            src/ReceiptScout.Api/
RUN dotnet restore src/ReceiptScout.Api/ReceiptScout.Api.csproj

# 2) Kopiera källkoden och publicera
COPY src/ src/
RUN dotnet publish src/ReceiptScout.Api/ReceiptScout.Api.csproj -c Release -o /app --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Kör aldrig som root: byt till den inbyggda användaren "app"
USER $APP_UID

ENTRYPOINT ["dotnet", "ReceiptScout.Api.dll"]