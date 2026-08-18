1. Zip yapısı

1_build-test.txt      → job'un TÜM logu, tek dosyada
build-test/system.txt → önemsiz

Sabit kural yine geçerli. deploy job'unun logu yok (skipped).

2. Step sınırı — sadece 3 grup

##[group]Run actions/checkout@v4
##[group]Run actions/setup-dotnet@v4
##[group]Run dotnet restore

Run dotnet build ve Run dotnet test hiç yok — Restore patladığı için ikisine de sıra gelmedi. Fail-fast davranışının dördüncü kanıtı.

3. Hata formatı — derleme hatasına benziyor ama annotation'a düşmüyor

/home/runner/work/ci-agent-pilot/ci-agent-pilot/src/CiPilot.Core/CiPilot.Core.csproj : error NU1101: Unable to find package Bu.Paket.Kesinlikle.Yok. No packages exist with this id in source(s): nuget.org [/home/runner/work/ci-agent-pilot/ci-agent-pilot/CiPilot.slnx]

Yapısal olarak Roslyn hatasına çok benziyor: dosya yolu + error NU1101: + mesaj + köşeli parantez içinde proje dosyası. Ama annotation üretmiyor — çünkü setup-dotnet'in kaydettiği problem matcher yalnızca Roslyn'in (satır,sütun): error CS#### formatını tanıyor, NuGet'in error NU####: formatını tanımıyor. Bu, "hata mesajı yapılandırılmış görünüyor" ile "problem matcher tarafından tanınıyor" ikisinin aynı şey olmadığını gösteriyor.

4. Aynı hata iki kez basılıyor — ama iki farklı proje için

CiPilot.Core.csproj      : error NU1101: ... Bu.Paket.Kesinlikle.Yok ...
CiPilot.Core.Tests.csproj : error NU1101: ... Bu.Paket.Kesinlikle.Yok ...

Senaryo 2'deki tekrar aynı hatanın kopyasıydı; burada ise aynı hata, iki farklı .csproj için ayrı ayrı raporlanıyor (Tests projesi Core'a referans verdiği için o da etkileniyor). Tekilleştirme mesaj+paket adına göre yapılmalı, sadece path+line'a göre değil — çünkü path burada iki farklı proje dosyası.

5. Satır/sütun bilgisi hiç yok

Roslyn hatasında (5,42) vardı; NuGet hatasında hiçbir satır numarası yok — sadece hangi .csproj'da sorun olduğu belli. Adım 6'daki "koda bakma" özelliği bu tip hatalarda satır bazlı kesit çekemez, sadece dosya düzeyinde (.csproj'un tamamı) bağlam verebilir.

6. Gürültü satırları — aynı liste

##[error]Process completed with exit code 1.
Node.js deprecation uyarısı
Post job cleanup. ve sonrası git komutları

7. Sonuç: üçüncü bir format ailesi gerekiyor

Şu ana kadar iki kalıp vardı: annotation (derleme hatası) ve xUnit bloğu (test hatası). Restore hatası annotation'a düşmeyen ama yapılandırılmış görünen üçüncü bir kategori: error NU####: ile başlayan satırları arayan ayrı bir regex gerekiyor. Deploy hatasındaki gibi tamamen ham bırakılmamalı — çünkü burada gerçek bir hata kodu ve paket adı var, LLM'e ham değil, biraz ayrıştırılmış geçirilirse daha isabetli olur.