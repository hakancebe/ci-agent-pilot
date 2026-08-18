1. Zip yapısı

1_build-test.txt   → job'un TÜM logu, tek dosyada (step başına ayrı dosya YOK)
build-test/system.txt → önemsiz, kullanılmayacak

Senaryo 1'deki yapıyla birebir aynı — bu artık sabit kural.

2. Step sınırı çıpası

##[group]Run <komut>

Bu senaryoda sadece 4 tane var (checkout, setup-dotnet, restore, build). Run dotnet test hiç yok — çünkü Build patladığı için Test'e hiç sıra gelmedi. Job metadata'sındaki Test: skipped bilgisinin log tarafındaki karşılığı bu: step çalışmadıysa logda hiçbir iz bırakmıyor.

3. Hata bloğunun gerçek sınırları

Başlangıç çıpası: "##[error]<yol>(satır,sütun): error CS####:"
Bitiş çıpası:     aynı satırın sonu (tek satırlık hata, blok değil)

4. Format, senaryo 1'den (xUnit) tamamen farklı

##[error]/home/runner/work/ci-agent-pilot/ci-agent-pilot/src/CiPilot.Core/Calculator.cs(5,42): error CS1002: ; expected [/home/.../CiPilot.Core.csproj]
Tek satırda her şey var: dosya yolu, (satır,sütun), hata kodu, mesaj — xUnit'teki gibi çok satırlı blok değil.
Satır/sütun formatı (5,42) — parantez içinde, virgülle ayrılmış. Annotation'daki start_line: 5 formatından farklı, ayrı bir regex ister.
Aynı hata iki kez basılıyor (ilk hata özeti, sonra "Build FAILED" sonrası tekrar). Tekilleştirme burada da gerekiyor.

5. Gürültü satırları (atılacak)

##[error]Process completed with exit code 1.
Node.js deprecation uyarısı
Post job cleanup. ve sonrasındaki tüm git komutları

6. En önemli sonuç: bu format için log parse etmeye gerek YOK

Bu satır, annotation API'sinin zaten ayrıştırıp verdiği bilgiyle birebir aynı kaynak. Yani derleme hatalarında log'a hiç inmene gerek yok — annotation yeterli ve kesin. Log parse detaylı yazılacak asıl format, senaryo 1 ve 5'teki xUnit çok satırlı bloğu.

7. Dosya yolu formatı, senaryo 1 ile aynı kalıp

/home/runner/work/{repo}/{repo}/src/CiPilot.Core/Calculator.cs

Repo adı yine iki kez tekrarlanıyor, aynı kırpma kuralı geçerli.