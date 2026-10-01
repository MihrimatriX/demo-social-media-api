## Demo Social Media API

Katmanlı bir .NET 10 sosyal medya API örneği: kimlik doğrulama, arkadaşlık, gönderi/yorum/beğeni/kayıt, dosya yükleme (MinIO) ve gerçek zamanlı sohbet (SignalR). CQRS + MediatR ile yapılandırılmıştır.

### Mimari
- **DemoSocialMedia.Api**: Controller'lar, `ApiExceptionHandler`, Swagger, SignalR hub.
- **DemoSocialMedia.Application**: Komut/sorgu handler'ları (MediatR), DTO'lar, FluentValidation kuralları, `AppException`.
- **DemoSocialMedia.Domain**: Saf entity'ler.
- **DemoSocialMedia.Infrastructure**: EF Core `AppDbContext`, entity konfigürasyonları, migration'lar, MinIO servisi.

### Hızlı Başlangıç
Gereksinimler: .NET 10 SDK, PostgreSQL, MinIO (ya da sadece Docker).

```bash
docker compose up --build
```
- API: `http://localhost:8080` (Swagger: `/swagger`, health: `/health`)
- MinIO konsol: `http://localhost:9001`
- Değerler `.env` dosyasından okunur (`copy .env.example .env`); dosya yoksa compose'daki dev varsayılanları kullanılır.
- API başlarken EF Core migration'ları uygulanır.

Yerel çalıştırma (API açılışta migration uyguladığı için Postgres ayakta olmalı, yoksa `Failed to connect to 127.0.0.1:5432` ile kapanır):
```bash
docker compose up -d --wait postgres minio
dotnet tool restore
dotnet run --project DemoSocialMedia.Api --launch-profile https
```
- Swagger: `https://localhost:7186/swagger` (ilk seferde `dotnet dev-certs https --trust`)
- Bağlantı ve MinIO ayarları `DemoSocialMedia.Api/appsettings.json`; JWT anahtarı `appsettings.Development.json` (en az 32 bayt, yoksa uygulama açılmaz).
- CORS'a izin verilen origin'ler: `Cors:Origins`.
- Yeni migration: `dotnet ef migrations add <Ad> -p DemoSocialMedia.Infrastructure -s DemoSocialMedia.Api -o Persistence/Migrations`

Testler SQLite in-memory ile çalışır (`dotnet test`); `TestAppFactory`, PostgreSQL migration'ını atlamak için `SKIP_DATABASE_MIGRATION` ayarlar ve MinIO'yu sahte servisle değiştirir.

### Hata Formatı
İş kuralı hataları `AppException` ile fırlatılır ve `ApiExceptionHandler` tarafından [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) olarak döner (`400/401/403/404/409`). Doğrulama hataları FluentValidation üzerinden `400 ValidationProblemDetails` döner.

### Kimlik Doğrulama
- `POST /api/auth/register` → kayıt (e-posta küçük harfe normalize edilir)
- `POST /api/auth/login` → HttpOnly `token` cookie'si set eder
- `POST /api/auth/logout` → cookie'yi siler
- `GET /api/auth/me` → oturum bilgisi
- JWT, `Authorization: Bearer` header'ından ya da (header yoksa) `token` cookie'sinden okunur (`JwtBearerEvents.OnMessageReceived`).

### Kullanıcı ve Arkadaşlık
- `GET /api/users/search?query=` → nickname'de arama, e-postada sadece tam eşleşme; kendin ve arkadaşların hariç
- `POST /api/friends/requests` → istek gönder (`{ receiverId }`); kendine veya zaten istek/arkadaşlık olan kişiye gönderilemez
- `PUT /api/friends/requests/{requestId}/accept` → istek kabul
- `GET /api/friends` → arkadaş listesi (`id, nickname, profilePictureUrl`)
- `GET /api/friends/requests?incoming=true|false` → bekleyen istekler

### Gönderiler
- `GET /api/posts` (anonim) → son 50 gönderi; oturum varsa `isLiked/isSaved` dolu
- `GET /api/posts/{id}` (anonim) → detay
- `POST /api/posts` → gönderi oluştur (`{ content, imageUrl? }` — `imageUrl` = upload'ın döndürdüğü `objectName`)
- `POST /api/posts/{id}/comments` → yorum (`{ content }`)
- `POST /api/posts/{id}/like` / `POST /api/posts/{id}/save` → toggle

### Dosya Yükleme (MinIO)
- `POST /api/files/upload` (form-data: `file`) → `{ url, objectName }`
- Sadece JPEG/PNG/GIF/WebP, en fazla 5 MB.
- Development'ta düz public URL döner (bucket ilk oluşturulurken anonim okumaya açılır); diğer ortamlarda 1 saatlik presigned URL.

### SignalR Sohbet
- `POST /api/chat/rooms` → oda oluştur (`{ name?, isGroupChat, memberIds }`); aynı iki kişi için birebir oda idempotenttir
- `GET /api/chat/rooms/{roomId}/messages` → geçmiş (sadece üyeler)
- `POST /api/chat/rooms/{roomId}/messages` → mesaj kaydet + odaya `ReceiveMessage` yayını (sadece üyeler)
- Hub: `/chathub` (oturum gerekli). İstemci metodları: `JoinRoom(roomId)` (üyelik kontrolü yapılır) / `LeaveRoom(roomId)`. Hub üzerinden mesaj gönderilmez; gönderen kimliği taklit edilemesin diye mesajlar REST ile gider.

### Güvenlik Notları
- JWT anahtarını prod'da gizli yönetimine taşıyın (Key Vault, AWS SM vb.).
- `ASPNETCORE_ENVIRONMENT=Development|Production`
