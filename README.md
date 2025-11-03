## Demo Social Media API

Modern, katmanlı bir .NET 9 sosyal medya API örneği. Kimlik doğrulama, arkadaşlık, gönderi/yorum/beğeni/kayıt, dosya yükleme (MinIO) ve gerçek zamanlı sohbet (SignalR) özellikleri içerir. Geliştirilebilir, test edilebilir ve buluta hazır olacak şekilde Domain-Driven ve CQRS + MediatR desenleriyle yapılandırılmıştır.

### Mimarinin Genel Yapısı
- **DemoSocialMedia.Api**: HTTP katmanı, controller’lar, middleware’ler, Swagger, SignalR hub.
- **DemoSocialMedia.Application**: Use case’ler (CQRS komut/sorgu + MediatR), DTO’lar, servis sözleşmeleri ve iş kuralları.
- **DemoSocialMedia.Domain**: Saf domain varlıkları ve ilişkiler.
- **DemoSocialMedia.Infrastructure**: EF Core `AppDbContext`, entity konfigurasyonları, MinIO servis entegrasyonu ve kalıcı katman.

```
DemoSocialMedia.sln
├─ DemoSocialMedia.Api
│  ├─ Controllers (Auth, Users, Posts, Friends, Chat, Files, Map)
│  ├─ Middleware (JwtCookieToHeader, UserId)
│  ├─ Swagger (FileUploadOperationFilter)
│  └─ ChatHub (SignalR)
├─ DemoSocialMedia.Application
│  ├─ Auth (Commands, DTOs, Queries, Services, Validators)
│  ├─ Posts (Commands, DTOs, Queries)
│  └─ Map (DTOs)
├─ DemoSocialMedia.Domain (Entities)
└─ DemoSocialMedia.Infrastructure
   ├─ Persistence (AppDbContext)
   ├─ Configurations (EF Core)
   └─ Services (MinioService)
```

### Öne Çıkanlar
- JWT kimlik doğrulama ve cookie-to-header köprüleme
- Health Check (PostgreSQL)
- Swagger ile API dokümantasyonu ve form-data dosya yükleme desteği
- MinIO ile dosya yükleme ve geliştirme ortamında public URL dönüşü
- SignalR ile oda bazlı gerçek zamanlı mesajlaşma

### Hızlı Başlangıç
1) Bağımlılıklar
- .NET 9 SDK
- PostgreSQL (ör: `indievalley` veritabanı)
- MinIO (dev için `minio/minio`)

2) Konfigürasyon
- `DemoSocialMedia.Api/appsettings.json`
  - `ConnectionStrings:DefaultConnection` değerini kendi PostgreSQL bağlantınıza göre güncelleyin.
- MinIO için environment veya `appsettings.*` ile şu anahtarlar desteklenir:
  - `Minio:Endpoint` (varsayılan `localhost:9000`)
  - `Minio:AccessKey` (varsayılan `minioadmin`)
  - `Minio:SecretKey` (varsayılan `minioadmin123`)
  - `Minio:Bucket` (varsayılan `media`)

3) Çalıştırma
```bash
dotnet build
dotnet run --project DemoSocialMedia.Api
```
- Geliştirme profili varsayılan URL: `https://localhost:7186` (Swagger) ve `http://localhost:5216`
- Swagger UI: `https://localhost:7186/swagger`

### Swagger'ı Açma
1) HTTPS geliştirme sertifikasını güvenilir kılın (tek seferlik):
```bash
dotnet dev-certs https --trust
```
2) API'yi https profiliyle başlatın:
```bash
dotnet run --project DemoSocialMedia.Api --launch-profile https
```
3) Swagger'ı açın (Windows PowerShell):
```bash
Start-Process https://localhost:7186/swagger
```
- Portlar farklıysa `DemoSocialMedia.Api/Properties/launchSettings.json` içindeki `applicationUrl` değerini kontrol edin.

4) MinIO Hızlı Kurulum (opsiyonel)
```bash
docker run -p 9000:9000 -p 9001:9001 \
  -e MINIO_ROOT_USER=minioadmin \
  -e MINIO_ROOT_PASSWORD=minioadmin123 \
  -v ./minio-data:/data \
  quay.io/minio/minio server /data --console-address ":9001"
```

### Kimlik Doğrulama
- Login sonrasında backend, HttpOnly bir `token` cookie’si ayarlar.
- `JwtCookieToHeaderMiddleware`, cookie’deki JWT’yi `Authorization: Bearer <token>` header’ına köprüler.
- `UserIdMiddleware`, doğrulanmış isteklerde `HttpContext.Items["UserId"]` değerini ayarlar.

### Sağlık Durumu
- `GET /health` → PostgreSQL bağlantısı için health check.

### SignalR Sohbet
- Hub: `/chathub`
- İstemci metodları:
  - `JoinRoom(roomId)` / `LeaveRoom(roomId)`
  - `SendMessage(roomId, user, message)` → `ReceiveMessage` olayıyla yayınlanır.

### Dosya Yükleme (MinIO)
- Endpoint: `POST /api/files/upload` (form-data: `file`)
- Geliştirme ortamında public URL döner, prod’da presigned URL üretilir.

### Kullanıcı ve Arkadaşlık
- `GET /api/users/search?query=` → oturum sahibine göre filtrelenmiş arama
- `POST /api/friends/requests` → istek gönder (body: `{ receiverId }`)
- `PUT /api/friends/requests/{requestId}/accept` → istek kabul
- `GET /api/friends` → arkadaş listesi
- `GET /api/friends/requests?incoming=true|false` → istekler

### Gönderiler
- `GET /api/posts` (Anonim erişim) → feed
- `GET /api/posts/{id}` → detay (oturum varsa beğeni/kayıt durumu işaretleri ile)
- `POST /api/posts` → gönderi oluştur (body: `{ content, imageUrl? }`)
- `POST /api/posts/{id}/comments` → yorum ekle (body: `{ content }`)
- `POST /api/posts/{id}/like` → beğeni toggle
- `POST /api/posts/{id}/save` → kaydetme toggle

### Harita
- `GET /api/map/grid` → Demo 2D grid içerik (dummy hücreler)

### Auth
- `POST /api/auth/register` → kullanıcı kaydı
- `POST /api/auth/login` → giriş (HttpOnly `token` cookie set eder)
- `GET /api/auth/me` → oturum bilgisi

### Veri Modeli (Seçmece)
- `User, Post, Comment, Like, Save, FriendRequest, Friendship, ChatRoom, ChatRoomMember, Message`
- İlişkiler EF Core `Configurations` altında tanımlı ve `AppDbContext` içinde uygulanır.

### Geliştirme İpuçları
- Komut/Sorgu akışı için MediatR kullanın; controller’lar ince kalsın.
- Validation için FluentValidation otomatik entegrasyon hazır.
- CORS profili `AllowAll` ile `http://localhost:3000` vb. kökenler açık.

### Ortam Değişkenleri ve Güvenlik
- JWT imza anahtarınızı prod’da giz yönetime taşıyın (ör: Azure Key Vault, AWS SM).
- `ASPNETCORE_ENVIRONMENT=Development|Production`
- Veritabanı ve MinIO bilgilerini ortam değişkenleri ile geçebilirsiniz.