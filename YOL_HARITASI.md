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
- [x] Hata bildirimi (e-posta; SMTP + STARTTLS, istenirse başarılı yedeklerde de)
- [x] Birden fazla profil (tek zamanlanmış görev dahil profilleri sırayla yedekler, tek özet e-posta)
- [x] Geri yükleme (önce doğrula ve yanına aç, sonra hizmeti durdurup yer değiştir; hata olursa eski veritabanı geri gelir)

## İleride: uzak hedefe yedek
Yedeğin tek kopyasının aynı makinede durmaması için arşivin ikinci bir hedefe de gönderilmesi.

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
- [ ] Testler şimdilik geçici betiklerle çalıştırılıyor; kalıcı bir test projesi eklenmeli.
