Senaryo 3 — Notlar (Deploy hatası, ham log)

1. Zip yapısı — bu senaryoya özel fark

0_deploy.txt         → deploy job'unun logu (kırmızı)
1_build-test.txt      → build-test job'unun logu (yeşil)

Önceki senaryolardan farklı olarak iki dosya birden var — çünkü ilk kez build-test yeşil kaldı. Dosya numaraları job sırasını yansıtmıyor (0_deploy önce geliyor, jobs.json'da ise build-test önce listeleniyordu). Eşleştirme dosya adındaki job ismine göre yapılmalı, numaraya göre değil.

2. Step grup başlığı, YAML'daki step adıyla eşleşmiyor

##[group]Run echo "Deploying..."

ci.yml'de step adı Fake deploy ama grup başlığı komutun ilk satırı. Senaryo 2'de de aynı durumu görmüştük (Run dotnet build ...). Genel kural: step eşleştirmesi asla isme göre yapılmamalı, started_at zaman damgasına göre yapılmalı.

3. Hata sinyali neredeyse yok — en zayıf senaryo

Deploying...
Deploying...
##[error]Process completed with exit code 1.

echo "Deploying..." çıktısı iki kez basılıyor. Ondan sonra tek bilgi: exit code 1. Hiçbir dosya adı, satır no, ya da anlamlı mesaj yok.

4. Sonuç: bu tip hatalar için log parse'ın tavanı burada

Ajan bu loga ne kadar iyi regex yazarsa yazsın, çıkaracağı tek şey "exit code 1" — annotation'daki çöp kayıtla aynı bilgi. Gerçek sebep (deploy.sh içinde ne olduğu) loga hiç yansımıyor çünkü script'in kendi echo'ları dışında bir çıktısı yok.

5. Gürültü satırları (atılacak) — aynı liste

##[error]Process completed with exit code 1.
Node.js deprecation uyarısı (iki farklı yerde çıkabiliyor — dikkat)
Post job cleanup. ve sonrasındaki tüm git komutları
Senaryo 3 — Ek Notlar: Ajanın Gerçek Dünya Deploy Hatalarındaki Sınırı

1. Ajanın çalışma mantığı

Log parse mekanizması anahtar kelime + regex tabanlı. Script'in içeriğini "anlamıyor", sadece tanıdığı kalıpları arıyor. Bu yüzden gerçek deploy hatalarındaki başarısı, script'in ne kadar bilgi ürettiğine doğrudan bağlı.

2. Yakalayabileceği durumlar — "konuşkan" script hataları

Permission denied (publickey)
scp: /var/www/app: No such file or directory
Error: connection to 10.0.1.5 refused
kubectl: error validating data: ValidationError(Deployment.spec)

Bu tarz mesajlar düz metin olduğundan, özel bir ayrıştırma kalıbına gerek kalmadan ##[error] sonrası satırlar olarak yakalanıp LLM'e gönderilebilir.

3. Zorlanacağı durumlar

Yapılandırılmamış, çok satırlı, gürültülü çıktılar — net bir "hata bloğu" çıpası olmadığından asıl sebep satırı ayırt edilemez.
Script hiçbir şey yazmazsa (bizim deploy.sh senaryomuzdaki gibi) — elde gerçekten hiçbir bilgi yok. Bu ajanın değil, script'in eksikliği.

4. Adım 2'ye eklenmesi gereken üçüncü yol

Eğer annotation yoksa VE xUnit kalıbı eşleşmiyorsa
  → ##[error] satırını ve öncesindeki 5-10 satırı ham olarak al
  → LLM'e "bu ham log parçası, kendi çıkarımını yap" şeklinde gönder

Ajan her zaman temiz bir yapıya ayrıştıramayacak; bazı durumlarda ham metni olduğu gibi LLM'e devretmek en gerçekçi yaklaşım.

5. Doğrulama raporuna (Adım 7) not: Ajanın "konuşkan" script hatalarında makul başarı göstermesi, "sessiz" (sadece exit code veren) script hatalarında ise yapısal olarak başarısız olması beklenmeli — bu bir kusur değil, verinin doğal sınırı. Rapor bu ayrımı açıkça belirtmeli; "ajan her deploy hatasını çözer" gibi abartılı bir iddiadan kaçınılmalı.