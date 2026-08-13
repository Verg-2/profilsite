# Aşama 1: Build (Derleme)
FROM node:20-alpine AS build-stage
WORKDIR /app

# Önce sadece package.json'ları kopyalıyoruz (Docker önbelleğinden faydalanmak için)
COPY package*.json ./
# npm ci, package-lock.json'a tam sadık kalarak güvenli kurulum yapar
RUN npm ci

# Tüm kodları kopyalıyoruz
COPY . .
# Vite ile projeyi derleyip statik (dist) klasörünü oluşturuyoruz
RUN npm run build

# Aşama 2: Prod (Yayınlama) - Sadece Nginx (Çok hafif bir web sunucusu)
FROM nginx:alpine AS production-stage

# Varsayılan Nginx ayarını ezip, kendi SPA dostu ayarımızı koyuyoruz
COPY nginx.conf /etc/nginx/conf.d/default.conf

# 1. Aşamada üretilen 'dist' klasörünü Nginx'in sunacağı yere kopyalıyoruz
COPY --from=build-stage /app/dist /usr/share/nginx/html

EXPOSE 80
# Arkaplanda değil, önyüzde çalışmasını sağlıyoruz ki container kapanmasın
CMD ["nginx", "-g", "daemon off;"]
