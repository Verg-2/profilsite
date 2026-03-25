# 🚀 Profil ve Vue Projesi - Git Kullanım Rehberi

Bu repo, hem statik HTML sayfalarından oluşan bir portföyü hem de **Vue.js** ile geliştirilmiş ayrı bir projeyi içerir.

## 📂 Proje Yapısı

*   **Ana Dizin (`/`)**: Statik HTML, CSS ve JS dosyalarını içerir (`index.html`, `hakkinda.html`, `blog.html`).
*   **Vue Projesi (`/vue-proje`)**: Vue.js ile geliştirilmiş modern web uygulaması burada bulunur.

---

## 🛠️ Günlük Git Komutları (Nasıl Kullanılır?)

Projende bir değişiklik yaptıktan sonra GitHub'a göndermek için sırasıyla şu adımları izle:

### 1. Durumu Kontrol Et
Hangi dosyaların değiştiğini görmek için:
```bash
git status
```

### 2. Değişiklikleri Ekle
Tüm yeni ve değişen dosyaları paketlemek için:
```bash
git add .
```

### 3. Kayıt Oluştur (Commit)
Yaptığın değişikliği anlatan kısa bir mesajla kaydet:
```bash
git commit -m "Buraya yapılan değişikliği yaz (örn: İletişim sayfası eklendi)"
```

### 4. GitHub'a Gönder (Push)
Değişiklikleri uzak sunucuya yükle:
```bash
git push
```

### 5. Güncellemeleri Çek (Pull)
Eğer GitHub üzerinden doğrudan bir değişiklik yaptıysan veya başka bir bilgisayardan yükleme yaptıysan, güncellemeleri almak için:
```bash
git pull
```

---

## 💻 Vue Projesini Çalıştırma

Vue projesini yerel bilgisayarında çalıştırmak istiyorsan şu adımları izlemelisin:

1.  Vue klasörüne gir:
    ```bash
    cd vue-proje
    ```

2.  Gerekli paketleri yükle (Sadece ilk kez veya `package.json` değiştiğinde):
    ```bash
    npm install
    ```

3.  Geliştirme sunucusunu başlat:
    ```bash
    npm run dev
    ```

---

## 🔒 Güvenlik Notları (.gitignore)

Aşağıdaki dosyalar **.gitignore** dosyası ile engellenmiştir ve GitHub'a yüklenmez:
*   `node_modules/`: Çok büyük ve gereksiz kütüphane dosyaları.
*   `.env`: Şifreler ve özel anahtarlar (varsa).
*   `dist/`: Build alındığında oluşan çıktılar.
*   `.idea/`, `.vscode/`: Kişisel editör ayarları.git