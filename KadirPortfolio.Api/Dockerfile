# Aşama 1: Build (Derleme) Aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece proje dosyasını kopyalayıp bağımlılıkları indiriyoruz (Docker layer cache için)
COPY ["KadirPortfolio.Api.csproj", "./"]
RUN dotnet restore "KadirPortfolio.Api.csproj"

# Tüm kodları kopyalayıp uygulamayı derliyoruz
COPY . .
RUN dotnet build "KadirPortfolio.Api.csproj" -c Release -o /app/build

# Yayınlama (Publish) işlemi
FROM build AS publish
RUN dotnet publish "KadirPortfolio.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Aşama 2: Run (Çalıştırma) Aşaması - Sadece Runtime (Daha küçük imaj boyutu)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Klasörleri önceden oluşturup yetkilendirelim (Uploads gibi klasörler için)
USER root
RUN mkdir -p /app/wwwroot/uploads && chown -R app:app /app/wwwroot

# Güvenlik için kısıtlı 'app' kullanıcısına geçiş
USER app

# Varsayılan portu 8080 olarak belirliyoruz
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Publish edilmiş kodları final imajına alıyoruz
COPY --from=publish /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "KadirPortfolio.Api.dll"]
