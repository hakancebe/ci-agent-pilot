Senaryo 7 — Deploy hata sınıfları, çoklu job ve zaman aşımı

Senaryo 6 tek bir CD hatasını ölçmüştü. Bu not, geriye kalan
doğrulanmamış maddeleri kapatıyor. Üç job kasıtlı olarak farklı
şekillerde patlatıldı ve tek koşuda ölçüldü.

Ölçülen run'lar: CD 34127272978 (v0.6.8), CD 34128641202 (v0.6.9),
CD 34128... (yalnızca zaman aşımı).

1. Kurulan üç hata sınıfı

olcum-docker-build : Dockerfile olmayan bir klasörü kopyalıyor.
                     Container HİÇ oluşmuyor, dolayısıyla docker logs
                     diye bir kanıt da yok.
olcum-sessiz-hata  : çıplak `exit 1`. Hiçbir açıklayıcı çıktı yok.
olcum-takilan-adim : timeout-minutes: 1 + sleep 120.

2. Job sonuçları — ilk sürpriz

olcum-takilan-adim   cancelled     <-- failure DEĞİL
olcum-sessiz-hata    failure
olcum-docker-build   failure
deploy               success

Zaman aşımına uğrayan job "cancelled" dönüyor. Adım seviyesinde de aynı:

  adım "Zaman aşımına kadar bekle" -> cancelled

Agent hem job hem adım seçerken "failure" arıyordu, yani takılan bir
deploy iki ayrı yerde birden görünmezdi.

3. Docker build hatası: iyi analiz edildi

Container hiç oluşmadığı için docker logs kanıtı yoktu. Model yine de
ham logdan gerçek sebebi çıkardı:

  "Dockerfile.bozuk 6. satır, 'boyle-bir-klasor-yok' klasörü bulunamadı"
  affectedFile: Dockerfile.bozuk:6

Bu sınıf için ek bir kurala gerek yok.

4. ASIL BULGU — jenerik mesaj farklı hataları birleştiriyordu

İki job da aynı ayrıştırılmış mesajı üretti:

  Process completed with exit code 1.

FailureGrouper (Kind, FilePath, LineNumber, Mesaj) ile gruplayınca
ikisini TEK hata + 2 tekrar saydı. Model de ikisine ORTAK bir kök neden
uydurdu ve bu rapora yazıldı:

  "Dockerfile... Bu, hem olcum-docker-build hem olcum-sessiz-hata
   job'larının başarısız olmasının temel nedenidir."

Sessiz job'ın Dockerfile ile hiçbir ilgisi yok.

En dikkat çekici kısım: HAM LOG İKİSİNİ DE İÇERİYORDU. Çoklu-job
birleştirmesi her job'ı "### Job / Adım" başlığıyla ekliyor. Model doğru
kanıta sahipti ama yapılandırılmış özet "tek hata" dediği için ona uydu.

Ders: yanlış bir gruplama, doğru ham veriden güçlü çıkıyor. "Modele daha
çok bağlam ver" sezgisinin ters çalıştığı yer burası — verdiğin bağlam
yanlışsa ham veriyi bastırıyor.

5. Düzeltme ve öncesi/sonrası (v0.6.8 -> v0.6.9)

Kimliksiz mesajlarda (runner'ın "exit code N" ve "operation was
canceled" satırları) job+adım da gruplama anahtarına girdi.

                        v0.6.8                  v0.6.9
tespit                  1 farklı hata (2 tekrar) 2 hata
sessiz job'ın sebebi    Dockerfile (YANLIŞ)      "belirlenemedi"
sessiz job güven        yüksek                   DÜŞÜK
sessiz job fixable      true                     false

v0.6.9 çıktısı, sessiz job için aynen şunu diyor:
"Log yetersiz olduğu için hatanın kesin nedeni belirlenemedi."

Yani model artık bilmediği yerde bildiğini iddia etmiyor.

Neden az gruplamak seçildi: fazla gruplama modele olmayan bir ortak
sebep UYDURTUYOR ve bu rapora yazılıp insana gösteriliyor. Az gruplama
ise telafi edilebilir — SystemPrompt zaten modelden kök nedene göre
birleştirmesini istiyor. Bilgi taşıyan mesajlarda matrix birleştirmesi
aynen korundu (testle sabitlendi).

6. Çoklu başarısız job: sorunsuz

Agent iki job'ı tek ErrorContext'te birleştirdi, job ve adım adlarını
virgülle birleştirip raporladı. LogParser'ın çoklu-job kolu canlıda ilk
kez koştu, sorun çıkmadı.

7. Zaman aşımı: daha derin bir katman

Job ve adım seviyesindeki düzeltmelerden sonra izole bir ölçüm yapıldı:
deploy yeşil, tek başarısızlık takılma. Sonuç:

  CD = completed/cancelled

RUN'ın kendi conclusion'ı da "cancelled". WebhookParser yalnızca
"failure" run'larda uyandığı için AGENT HİÇ ÇAĞRILMADI — job ve adım
düzeltmelerine sıra bile gelmedi.

Ayırt edici sinyal arandı, YOK:
- job logu her iki durumda da sadece "##[error]The operation was
  canceled." içeriyor; zaman aşımına özel bir satır yok
- run objesinde iptal edeni/sebebini söyleyen alan yok
- kullanıcının elle iptal ettiği run da aynı conclusion'ı veriyor

"Takıldı" ile "kullanıcı iptal etti" teknik olarak ayrılamıyor.

8. Sonuç: bu bir ürün kararı, teknik karar değil

Her elle iptalde yorum düşmek gürültü olur; gürültü de agent'ı
kapattırır. Bu, takılan bir deploy'u kaçırmaktan daha kötü bir sonuç.

Karar (v0.6.10): davranış opsiyonel, VARSAYILANI KAPALI.
CI_AGENT_ANALYZE_CANCELLED=true diyen repo takılmaları da izler.

Ama tercih edilen yol ayarı açmak değil, pipeline'ı düzeltmek:

  timeout 300 ./deploy.sh

Takılma gerçek bir step failure'a (exit 124) dönüşür, run "failure"
olur, ayrım sorunu hiç doğmaz.

9. Genel ders

Senaryo 6'da "agent'ın kalitesi hata mesajının kalitesine bağlı"
demiştik. Bu senaryo onu bir adım ileri taşıyor: agent'ın kalitesi
PIPELINE'IN YAZILIŞ BİÇİMİNE bağlı. `docker logs` basmayan bir adım
kanıt bırakmıyor; `::error::` yazmayan bir adım anlamsız bir mesaj
bırakıyor; `timeout` kullanmayan bir adım hiç görülmüyor.

Agent bunları telafi edemez, yalnızca dürüstçe "bilmiyorum" diyebilir —
ve v0.6.9'dan sonra diyor.

10. Canlı doğrulama — CI_AGENT_ANALYZE_CANCELLED=true (CD run 34131451957)

Ayar açıldıktan sonra aynı senaryo tekrarlandı: deploy yeşil, tek
başarısızlık zaman aşımı, run conclusion=cancelled.

Agent loglarında zincirin her katmanı göründü:

  "Başarısız job yok ama 1 job iptal/zaman aşımı ile bitmiş; takılma
   ihtimaline karşı bunlar analiz edilecek (olcum-takilan-adim)."
  ErrorContext: job=olcum-takilan-adim, adım=Zaman aşımına kadar bekle

Rapor çıktısı:

  Tespit edilen tüm hatalar (1)
  - Timeout
    `The operation was canceled.`

  Kök Neden (güven: yüksek): "1 dakikalık maksimum çalışma süresini
  aşması nedeniyle GitHub Actions tarafından iptal edildi."

Hata tipi "Generic" değil "Timeout" görünüyor; öneri de doğru dosyayı
ve iki somut seçeneği veriyor (timeout süresini artır ya da bekleme
süresini düşür).

Yani üç katman da doğrulandı: run seviyesi (WebhookParser ayarı), job
seviyesi (cancelled fallback), adım seviyesi (FindCancelledStep).
