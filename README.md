# 🚀 Kadir Portfolio & CMS Projesi

![Vue.js](https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vue.js&logoColor=4FC08D)
![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

Kadir Portfolio, modern web teknolojileri kullanılarak geliştirilmiş, tamamen dinamik yönetilebilen bir kişisel portfolyo ve CMS (İçerik Yönetim Sistemi) projesidir. Sistem, baştan sona Dockerize edilmiş mimarisi sayesinde tek bir komutla ayağa kaldırılıp çalıştırılabilir.

## ✨ Öne Çıkan Özellikler

- **Dinamik Yönetim Paneli**: Tüm içerikleri (Hakkında, Blog, Projeler, İletişim vb.) kullanıcı dostu admin paneli üzerinden yönetebilirsiniz.
- **Tamamen Dockerize Mimari**: Frontend, Backend ve Veritabanı izole konteynerlar (container) içerisinde çalışır.
- **Modern ve Şık Tasarım**: Vue 3 ile geliştirilmiş, harika animasyonlara sahip (Glassmorphism) kullanıcı arayüzü.
- **Gelişmiş SEO ve GEO Ayarları**: Sitenizin Google ve sosyal medya (Open Graph, Twitter Cards) görünümlerini tamamen dinamik olarak admin panelinden ayarlayabilirsiniz.
- **Dinamik Sözlük (Çeviri Kalkanı)**: Sitenizdeki teknik terimlerin çeviri motorları tarafından yanlış çevrilmesini önlemek için özel kurallar ekleyebilirsiniz.
- **Sistem Sağlığı ve Analitik**: Ziyaretçi istatistiklerini görebilir, API sağlığını ve hata kayıtlarını admin panelinden izleyebilirsiniz.

---

## 🛠 Kullanılan Teknolojiler

### Önyüz (Frontend)
- **Vue.js 3** & **Vite**
- **Vue Router** (Yönlendirmeler)
- **Vanilla CSS** (Özel modern tasarımlar)
- **SweetAlert2** (Gelişmiş bildirim pencereleri)

### Arkayüz (Backend)
- **.NET 8 (C#)** & **ASP.NET Core Web API**
- **Entity Framework Core** (ORM)
- **JWT (JSON Web Token)** (Güvenli kimlik doğrulama)

### Veritabanı ve Altyapı
- **PostgreSQL** (Güçlü ve açık kaynak ilişkisel veritabanı)
- **Docker & Docker Compose** (Konteyner mimarisi)
- **Nginx** (Frontend SPA sunumu için ters vekil sunucu)

---

## 🚀 Kurulum ve Çalıştırma (A'dan Z'ye)

Bu proje **Docker** kullanılarak tasarlandığı için sisteminize Node.js, .NET SDK veya PostgreSQL kurmanıza gerek **yoktur**. Sadece Docker'ın bilgisayarınızda yüklü olması yeterlidir.

### Ön Gereksinimler
- Bilgisayarınızda [Docker Desktop](https://www.docker.com/products/docker-desktop) kurulu ve arka planda çalışıyor olmalıdır.

### 1. Adım: Projeyi İndirin
Projeyi bilgisayarınıza indirin (veya git clone ile çekin) ve terminalden (veya komut istemcisi/powershell) projenin ana klasörüne gidin:
```bash
git clone https://github.com/Verg-2/profilsite.git
cd profilsite
```

### 2. Adım: Çevresel Değişkenleri (Env) Ayarlayın
Sistemin çalışması için bazı şifrelere ihtiyacı vardır. 
- `KadirPortfolio.Api` klasörünün içindeki `appsettings.example.json` veya `.env.example` dosyasını örnek alarak, aynı klasörde bir `.env` dosyası oluşturun ve içerisine şifrelerinizi yazın.

Örnek `.env` içeriği (`KadirPortfolio.Api/.env`):
```env
JWT_SECRET_KEY=BURAYA_UZUN_VE_GIZLI_BIR_SIFRE_YAZIN
DB_CONNECTION_STRING=Host=db;Port=5432;Database=kadirportfolio;Username=postgres;Password=KadirPortfolio2026!
```

### 3. Adım: Tek Komutla Sistemi Başlatın
Ana klasörde (`docker-compose.yml` dosyasının bulunduğu yer) terminali açın ve şu sihirli komutu çalıştırın:

```bash
docker-compose up -d --build
```
*Not: Bu işlem, ilk çalıştırmada imajları (image) indirip derleyeceği için bilgisayarınızın ve internetinizin hızına göre birkaç dakika sürebilir.*

### 4. Adım: Siteye Giriş Yapın
Kurulum bittikten sonra sisteminiz kullanıma hazırdır! Tarayıcınızı açın ve aşağıdaki adreslere gidin:

- **Ana Web Sitesi:** [http://localhost](http://localhost)
- **Admin Paneli:** [http://localhost/admin/login](http://localhost/admin/login)

---

## 🛑 Sistemi Durdurmak veya Kapatmak

Çalışan projeyi durdurmak isterseniz, terminalde projenin ana dizininde şu komutu çalıştırabilirsiniz:
```bash
docker-compose down
```
Bu komut sistemi durdurur. Merak etmeyin, veritabanına eklediğiniz bilgiler (yazılar, ayarlar vb.) silinmez, koruma altındadır. Sistemi tekrar başlatmak için `docker-compose up -d` yazmanız yeterlidir.

---

## 🗂 Klasör Yapısı

- `/KadirPortfolio.Api` ➔ .NET 8 Backend API kodları, Modeller, Controller'lar ve API Servisleri.
- `/vue-proje` ➔ Vue 3 Frontend kodları, Vue sayfaları (`src/views`), bileşenler (`src/components`) ve CSS stilleri.
- `docker-compose.yml` ➔ Tüm mimarinin (Frontend, Backend, Veritabanı) iletişimini kuran ve yöneten ana yapılandırma dosyası.

---
**Geliştirici:** Kadir 
*Eğer bir hata ile karşılaşırsanız veya katkıda bulunmak isterseniz Issue veya Pull Request açabilirsiniz!*