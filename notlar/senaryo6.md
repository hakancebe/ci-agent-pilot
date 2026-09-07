Senaryo 6 — Deploy hatası (container ayağa kalkmıyor, health check düşüyor)

Önceki beş senaryo CI tarafındaydı: test, derleme, restore. Bu ilk CD
senaryosu. Ölçüm: CD run 34114468245, deploy job 101717959486.

1. Job yapısı — artık ayrı bir workflow

CD, CI'dan bağımsız bir workflow. workflow_run ile zincirli:
CI success → CD başlar. Yani agent'ın izlediği workflow listesinde CD de
olmak zorunda, yoksa bu hata hiç görülmüyor.

Ham log: 292 satır, 25.421 karakter, 6 adet ##[group]Run bloğu.
Bloklar: checkout, setup-dotnet, Publish, Build image, Start container,
Smoke test.

2. Step sınırı çıpası — değişmiyor

##[group]Run <komut>

Senaryo 1'deki kural CD'de de aynen geçerli. Ayrı bir kural gerekmiyor.

3. Hata formatı — dosya:satır YOK

Derleme ve restore hatalarının aksine burada yapılandırılmış bir hata
satırı yok. Ne "Dosya.cs(12,5): error CS####" var, ne "csproj : error NU####".

Elimizde yalnızca ##[error] satırları var. İki tane:

satır 268: ##[error]Health check 20 saniye sonunda basarisiz oldu
satır 274: ##[error]Process completed with exit code 1.

ExtractGenericError'ın son çare dalı (##[error] regex'i) devreye giriyor.
Regex.Match ilk eşleşmeyi döndürdüğü için 268. satır kazanıyor.

Bu şanslı bir sonuç. 274 kazansaydı mesaj "Process completed with exit
code 1" olurdu — hiçbir şey anlatmayan bir cümle. Ders: adımın kendi
::error:: mesajını yazması, kazanan satırın anlamlı olmasını sağlıyor.

4. Asıl kanıt ##[error] satırında değil, altındaki container loglarında

--- container durumu ---
cipilot-api	Exited (139) 19 seconds ago
--- container loglari ---
Unhandled exception. System.InvalidOperationException: CIPILOT_CONNECTION_STRING ortam değişkeni tanımlı değil. Deploy adımında bu değerin container'a geçirilmesi gerekiyor.
   at Program.<Main>$(String[] args) in /home/runner/work/ci-agent-pilot/ci-agent-pilot/src/CiPilot.Api/Program.cs:line 12

Bu satırlar log'a kendiliğinden gelmiyor. Smoke test adımının failure
dalında docker logs çağrıldığı için var. Yani deploy adımını yazan kişi
"patlarsa logları bas" demeseydi, agent'ın elinde hiçbir kanıt olmazdı.

Bu, agent'ın kalitesinin pipeline'ı yazan kişiye bağlı olduğu ilk net
örnek.

5. Parser çıktısı — ölçüldü

LogParser.BuildErrorContext bu logda şunu üretiyor:

Kind              : Generic
FilePath          : null
LineNumber        : null
Message           : Health check 20 saniye sonunda basarisiz oldu
RawEvidence       : 4.682 karakter
RawStepLog        : 3.003 karakter
AllFailuresLocated: false

AllFailuresLocated false olduğu için ham log prompt'a giriyor. Doğru
davranış: konum yoksa modelin kendi çıkarımını yapması gerekiyor ve tek
dayanağı ham log.

3.003 karakter, 50.000'lik prompt limitinin çok altında. Deploy hataları
bu açıdan rahat.

6. Kritik boşluk — kusurlu komut prompt'a girmiyor

RawStepLog yalnızca patlayan adımın bloğu, yani Smoke test.

Kusurlu komut ise bir önceki blokta:

  docker run -d --name cipilot-api -p 8080:8080 cipilot-api:${{ github.sha }}

Bu satır prompt'ta YOK. Model "ortam değişkeni verilmemiş" sonucuna
semptomdan ulaşıyor, kusurlu satırı görerek değil.

İlk koşuda doğru cevabı vermesinin sebebi kısmen istisna metniydi: mesaj
zaten "Deploy adımında bu değerin container'a geçirilmesi gerekiyor"
diyordu. Mesaj jenerik olsaydı ("Value cannot be null") teşhis kayıyor —
denendi, kaydı.

7. Dosya yolu tuzağı

Ham logda tek bir dosya yolu var: Program.cs:line 12.

Ama düzeltilecek yer orası DEĞİL. Program.cs doğru davranıyor: ayar
eksikse açılmayı reddediyor. Düzeltilecek yer cd.yml.

Yani deploy senaryolarında "logda geçen dosya" ile "düzeltilecek dosya"
birbirinden farklı olabiliyor. Test ve derleme senaryolarında bu ikisi
hep aynıydı.

8. Modelin uydurma davranışı ve düzeltmesi

v0.6.6 ile çalıştırıldığında model affectedFile olarak şunu döndürdü:

  .github/workflows/deploy.yml

Repoda öyle bir dosya yok. Gerçeği cd.yml. Ad logun hiçbir yerinde
geçmiyor, yani model bunu çıkaramazdı — boşluğu doldurdu.

Doğru ad zaten iki yerde duruyordu: workflow_run webhook payload'ında ve
Actions API'sinde, "path" alanında. Prompt'a hiç girmiyordu.

v0.6.7 bu bilgiyi prompt'a taşıdı. Aynı log, aynı model:

  öncesi: .github/workflows/deploy.yml   (olmayan dosya)
  sonrası: .github/workflows/cd.yml      (doğru, üstelik komut örneğiyle)

9. Sonuç — deploy senaryosu için parser'a yeni kural GEREKMİYOR

Mevcut zincir bu hatayı doğru işliyor:
- ##[group] bloklama çalışıyor
- ##[error] son çare dalı doğru mesajı seçiyor
- konum bulunamayınca ham log prompt'a giriyor
- FixPolicy .github/ altını koruduğu için /fix doğru şekilde reddediyor

Eksik olan tek şey parser değil, BAĞLAM'dı: hangi workflow dosyasından
geldiği. O da v0.6.7 ile eklendi.

Genel ders: konumsuz hata sınıflarında (deploy, ortam, yapılandırma)
modele verilecek en değerli şey daha çok log değil, elimizde zaten olan
metadata.
