# YedekAl Yol Haritası

## Tamamlanan (1. aşama)
- Servis yönetimi `ServiceController` ile; hata olsa da hizmet yeniden başlatılır
- Kod içinde kopyalama; ZIP / RAR / 7z seçimi ve sıkıştırma seviyesi
- Ön kontroller (yetki, hizmet, kaynak, yazma izni, boş alan)
- Arşiv doğrulama, eski yedekleri saklama kuralına göre silme
- Log dosyaları ve yedek geçmişi
- `/auto` modu ve SYSTEM hesabıyla çalışan günlük zamanlanmış görev

## 2. aşama
- [x] Arşiv şifreleme (7z / RAR; dosya adları da gizlenir, parola DPAPI ile saklanır)
- [x] Hata bildirimi (e-posta; SMTP + STARTTLS, istenirse başarılı yedeklerde de; güvenilmeyen
  sunucu sertifikası isteğe bağlı kabul edilebilir)
- [x] Birden fazla profil (tek zamanlanmış görev dahil profilleri sırayla yedekler, tek özet e-posta)
- [x] Geri yükleme (önce doğrula ve yanına aç, sonra hizmeti durdurup yer değiştir; hata olursa eski veritabanı geri gelir)

## İleride: uzak hedefe yedek
Yedeğin tek kopyasının aynı makinede durmaması için arşivin ikinci bir hedefe de gönderilmesi.
Tasarım ön çalışması: [docs/uzak-hedef-tasarimi.md](docs/uzak-hedef-tasarimi.md) (açık sorular yanıt bekliyor).

- Hedef türleri: ağ paylaşımı (UNC), harici disk, ileride SFTP / bulut depolama
- Zamanlanmış görev SYSTEM hesabıyla çalıştığı için ağ paylaşımında ya bilgisayar hesabına
  yazma izni verilmeli ya da hedef için ayrı kullanıcı bilgisi DPAPI ile saklanmalı
- Gönderim sonrası boyut / hash karşılaştırmasıyla doğrulama
- Uzak hedefte de ayrı saklama kuralı (son N yedek / N gün)
- Uzak hedefe gönderilemezse yerel yedek geçerli kalmalı; hata uyarı olarak loglanmalı
  ve bildirim özelliğiyle haber verilmeli

## Teknik borç
- [x] ZIP doğrulaması CRC'leri artık arşivin merkezi dizininden okuyor (ZIP64 dahil); sonradan
  açılan her ZIP yedeği tam olarak doğrulanabiliyor.
- [x] Kalıcı test projesi: `YedekAl.Tests` (MSTest, 38 test). Visual Studio'da Test Gezgini'nden
  ya da komut satırından çalıştırılır:
  `MSBuild YedekAl.sln -restore` ardından
  `vstest.console.exe YedekAl.Tests\bin\Debug\net481\YedekAl.Tests.dll`.
  Hizmet durdurma, gerçek geri yükleme ve zamanlanmış görevin kaydı yönetici yetkisi istediği için
  testlerin dışında; bunlar elle denenmeli.
