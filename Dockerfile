# Build stage (.NET 10 SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Kopiera ALLA projektfiler först och kör sedan restore för optimal Docker-cache
COPY ["src/NetAdmin.Core/NetAdmin.Core.csproj", "src/NetAdmin.Core/"]
COPY ["src/NetAdmin.Infrastructure/NetAdmin.Infrastructure.csproj", "src/NetAdmin.Infrastructure/"]
COPY ["src/NetAdmin.Providers.Apple/NetAdmin.Providers.Apple.csproj", "src/NetAdmin.Providers.Apple/"]
COPY ["src/NetAdmin.Providers.Asus/NetAdmin.Providers.Asus.csproj", "src/NetAdmin.Providers.Asus/"]
COPY ["src/NetAdmin.Providers.Dell/NetAdmin.Providers.Dell.csproj", "src/NetAdmin.Providers.Dell/"]
COPY ["src/NetAdmin.Providers.Epson/NetAdmin.Providers.Epson.csproj", "src/NetAdmin.Providers.Epson/"]
COPY ["src/NetAdmin.Providers.Google/NetAdmin.Providers.Google.csproj", "src/NetAdmin.Providers.Google/"]
COPY ["src/NetAdmin.Providers.HomeWizard/NetAdmin.Providers.HomeWizard.csproj", "src/NetAdmin.Providers.HomeWizard/"]
COPY ["src/NetAdmin.Providers.Ikea/NetAdmin.Providers.Ikea.csproj", "src/NetAdmin.Providers.Ikea/"]
COPY ["src/NetAdmin.Providers.OpenWrt/NetAdmin.Providers.OpenWrt.csproj", "src/NetAdmin.Providers.OpenWrt/"]
COPY ["src/NetAdmin.Providers.OpnSense/NetAdmin.Providers.OpnSense.csproj", "src/NetAdmin.Providers.OpnSense/"]
COPY ["src/NetAdmin.Providers.PfSense/NetAdmin.Providers.PfSense.csproj", "src/NetAdmin.Providers.PfSense/"]
COPY ["src/NetAdmin.Providers.Samsung/NetAdmin.Providers.Samsung.csproj", "src/NetAdmin.Providers.Samsung/"]
COPY ["src/NetAdmin.Providers.Shelly/NetAdmin.Providers.Shelly.csproj", "src/NetAdmin.Providers.Shelly/"]
COPY ["src/NetAdmin.Providers.TpLink/NetAdmin.Providers.TpLink.csproj", "src/NetAdmin.Providers.TpLink/"]
COPY ["src/NetAdmin.Providers.UniFi/NetAdmin.Providers.UniFi.csproj", "src/NetAdmin.Providers.UniFi/"]
COPY ["src/NetAdmin.Providers.Xiaomi/NetAdmin.Providers.Xiaomi.csproj", "src/NetAdmin.Providers.Xiaomi/"]
COPY ["src/NetAdmin.Providers.Yamaha/NetAdmin.Providers.Yamaha.csproj", "src/NetAdmin.Providers.Yamaha/"]
COPY ["src/NetAdmin.Web/NetAdmin.Web.csproj", "src/NetAdmin.Web/"]

RUN dotnet restore "src/NetAdmin.Web/NetAdmin.Web.csproj"

# Kopiera resten av källkoden och publicera
COPY . .
WORKDIR "/src/src/NetAdmin.Web"
RUN dotnet publish "NetAdmin.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage (.NET 10 ASP.NET Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
EXPOSE 8080

# Skapa katalog för SQLite-volymen
RUN mkdir -p /app/data

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NetAdmin.Web.dll"]