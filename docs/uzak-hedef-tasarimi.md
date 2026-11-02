# Uzak hedefe yedek: tasarım ön çalışması

Durum: taslak, henüz kod yazılmadı. Açık sorular yanıtlanınca uygulamaya geçilecek.

## Amaç
Yedeğin tek kopyası, veritabanıyla aynı makinede (çoğu zaman aynı diskte) duruyor. Disk arızası,
fidye yazılımı ya da makinenin kaybı durumunda yedek de gider. Amaç, başarıyla alınıp doğrulanan
her yedeğin bir ya da birkaç ikinci hedefe de kopyalanması.

## Kapsam önerisi

| Sürüm | Hedef türü | Not |
|---|---|---|
| 1 | Ağ paylaşımı (`\\sunucu\paylasim\klasor`) | En sık ihtiyaç; dosya yolu olarak çalışır |
| 1 | Yerel / harici disk klasörü (`F:\Yedekler`) | Ağ paylaşımıyla aynı kod; ek iş yok |
| 2 | SFTP | Ek kütüphane gerekir (ör. SSH.NET) |
| 2+ | S3 uyumlu depolama, Azure Blob, OneDrive | Her biri ayrı SDK ve kimlik yönetimi demek |

1. sürümde iki tür de "dosya yolu" olduğu için tek bir `FolderTarget` uygulaması yeterli olur.
Arayüz, ileride yeni türler eklenebilecek şekilde kurulur:

```csharp
internal interface IRemoteTarget
{
    string describe();                                   // log ve e-posta için
    void connect();                                      // gerekirse ağ oturumu açar
    void upload(string localFile, string remoteName, IProgress<int> progress, CancellationToken token);
    void verify(string remoteName, long size, byte[] sha256, CancellationToken token);
    IEnumerable<string> list();                          // saklama kuralı için
    void delete(string remoteName);
    void disconnect();
}
```

## Akış
Yerel yedek **başarıyla alınıp doğrulandıktan sonra**, her etkin uzak hedef için sırayla:

1. **Bağlan.** Gerekirse ağ paylaşımına kayıtlı kullanıcıyla oturum açılır (aşağıya bakın).
2. **Gönder.** Dosya önce `<ad>.yedekal_parca` adıyla yazılır. Gönderim sırasında yerel dosyanın
   SHA-256 değeri de hesaplanır.
3. **Doğrula.** Uzak dosya baştan okunup boyutu ve SHA-256 değeri karşılaştırılır. Ancak tutarsa
   dosya asıl adına çevrilir. Böylece yarım kalmış bir kopya hiçbir zaman yedek gibi görünmez.
4. **Kaydet.** Sonuç ayrı bir uzak geçmiş dosyasına yazılır (`uzak_gecmis.txt`: yedek, profil,
   hedef, uzak yol, durum, SHA-256).
5. **Eskileri sil.** Hedefin kendi saklama kuralı (son N kopya ve/veya N gün) uygulanır. Yalnızca
   uzak geçmişte kayıtlı dosyalar silinir; yereldeki kural gibi.
6. **Bağlantıyı kapat.**

Bir hedefe gönderim başarısız olursa yerel yedek **geçerli kalır**. Hata log'a ve e-posta
bildirimine düşer, diğer hedefler denenmeye devam eder. Geçici ağ hataları için 3 deneme yapılır
(30 sn, 2 dk, 5 dk arayla).

Klasör kopyası olarak alınan, yani sıkıştırılmamış yedekler 1. sürümde uzak hedefe gönderilmez.
Bu tür bir profilde uzak hedef tanımlanmak istenirse kullanıcı uyarılır.

## En zor kısım: ağ paylaşımında kimlik
Zamanlanmış görev **SYSTEM** hesabıyla çalışıyor. SYSTEM ağa bilgisayar hesabı olarak çıkar:

- **Etki alanında (domain):** Paylaşıma `ETKIALANI\BILGISAYAR$` hesabı için yazma izni vermek
  yeterli. Parola saklamaya gerek kalmaz; önerilen yol budur.
- **Çalışma grubunda (workgroup):** Bilgisayar hesabı uzak makinede tanınmaz. Hedef için bir
  kullanıcı adı ve parola gerekir:
  - Parola, diğer parolalar gibi DPAPI (makine kapsamı) ile saklanır.
  - Gönderimden önce `WNetAddConnection2` ile paylaşıma bu kullanıcıyla bağlanılır, iş bitince
    `WNetCancelConnection2` ile bağlantı kapatılır.
  - Bilinen bir tuzak var: Aynı sunucuya o oturumda **başka bir kullanıcıyla** zaten bağlantı
    varsa Windows 1219 hatası verir. Bu hata yakalanıp anlaşılır bir mesaja çevrilmeli.

Ayarlar penceresinde bir **Bağlantıyı test et** düğmesi olmalı. Ancak bu düğme, arayüzü
çalıştıran yönetici kullanıcısıyla dener, SYSTEM ile değil. Bu yüzden bir de "zamanlanmış görevle
test et" seçeneği düşünülmeli: tek seferlik bir görev çalıştırılıp sonucu log'dan okunur.

## Veri modeli taslağı
`Profile` içine bir liste eklenir:

```csharp
public class RemoteTarget
{
    public string id { get; set; } = Guid.NewGuid().ToString("N");
    public bool enabled { get; set; } = true;
    public string path { get; set; } = "";               // \\sunucu\paylasim\klasor veya F:\Yedekler
    public string user { get; set; } = "";               // boşsa SYSTEM / bilgisayar hesabı
    public string protectedPassword { get; set; } = "";  // DPAPI, amaç: "RemotePassword"
    public int keepLastCount { get; set; }
    public int keepDays { get; set; }
    public bool fullVerify { get; set; } = true;         // false: yalnızca boyut karşılaştırılır
}
```

Mevcut ayar dosyaları bu alanı içermediği için ek bir geçiş adımı gerekmez; liste boş gelir.

## Doğrulamanın maliyeti
Tam doğrulama, uzak dosyayı ağ üzerinden bir kez daha okumak demek; yavaş bağlantılarda
gönderim süresi kabaca iki katına çıkar. Bu yüzden hedef başına "yalnızca boyut karşılaştır"
seçeneği önerilir. Varsayılan tam doğrulama olmalı.

## Harici disk notu
Harici diskin sürücü harfi değişebilir ya da disk takılı olmayabilir. 1. sürümde hedef
bulunamazsa yedek başarısız sayılmaz; uyarı verilir ve bildirim gönderilir. İleride hedef,
sürücü harfi yerine birim etiketi ya da seri numarasıyla tanımlanabilir.

## Etkilenecek yerler
- `Profile`: uzak hedef listesi ve doğrulama kuralları
- `BackupEngine`: yerel yedek ve saklama kuralından sonra uzak gönderim adımı
- Yeni: `Core/RemoteTargets/` (arayüz, `FolderTarget`, ağ oturumu için WNet P/Invoke)
- Yeni: uzak geçmiş dosyası ve uzak saklama kuralı
- `MailNotifier`: rapora hedef başına bir satır
- `FrmProfile`: "Uzak hedefler" listesi (ekle / düzenle / sil / test et) ve yeni `FrmRemoteTarget` penceresi
- `RestoreEngine` (2. adım): yerel kopya yoksa uzak kopyadan geri yükleme

## Test planı
- **Birim testleri:** `FolderTarget` yerel geçici klasörlerle test edilir. Ağ paylaşımı da bir
  dosya yolu olduğu için gönderim, `.yedekal_parca` → asıl ad dönüşümü, SHA-256 doğrulaması,
  bozulmuş uzak dosyanın yakalanması ve saklama kuralı aynı testlerle kapsanır.
- **Elle denenecekler:** Çalışma grubunda kullanıcı adı ve parolayla `WNetAddConnection2`,
  etki alanında bilgisayar hesabıyla erişim, 1219 hatası ve ağ kopması sırasında yeniden deneme.

## Açık sorular (sizin kararınız)
1. 1. sürüm için ağ paylaşımı ve harici disk yeterli mi, yoksa SFTP ya da bulut da hemen gerekli mi?
2. Makineler bir etki alanında mı, yoksa çalışma grubunda mı? İlkinde parola saklamaya gerek kalmaz.
3. Uzak hedefe gönderilemediğinde yedek "başarısız" mı sayılsın, yoksa yalnızca uyarı mı verilsin?
   (Öneri: uyarı; ama bilgisayarı kapatma kararını etkilemesi istenebilir.)
4. Uzak kopyalar için de yerelden farklı bir saklama kuralı gerekli mi? (Öneri: evet, hedef başına.)
5. Yerel yedek şifresizse, uzak hedefe giden kopya şifrelensin mi? Bu, yerelde ikinci bir arşiv
   oluşturmak demek; öneri yerel profilde şifrelemeyi açmak.

## Tahmini iş
1. sürüm (ağ paylaşımı + harici disk, arayüz ve testlerle): profillerdeki iş kadar, orta büyüklükte
bir değişiklik.
