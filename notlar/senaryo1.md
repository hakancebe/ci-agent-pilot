1. Zip yapısı — sabit kural
1_build-test.txt   → job'un TÜM logu, tek dosyada (step başına ayrı dosya YOK)
build-test/system.txt → önemsiz, kullanılmayacak

2. Step sınırı çıpası
##[group]Run <komut>

3. Hata bloğunun gerçek sınırları
Başlangıç çıpası: "  Failed <TestAdı> ["
Bitiş çıpası:     "Failed!  - Failed:" (özet satırı) VEYA sonraki ##[group]/##[error]

4. Gürültü satırları (atılacak)
##[error]Process completed with exit code 1.
Node.js deprecation uyarısı
Post job cleanup. ve sonrasındaki tüm git komutları
Stack trace içinde System.Reflection... gibi framework satırları (sadece senin kodunla ilgili in ...cs:line N içerenler kalsın)

5. Dosya yolu formatı
/home/runner/work/{repo}/{repo}/tests/.../Dosya.cs:line 12
Repo adı iki kez tekrarlanıyor, sabit önek olarak kırpılacak
Satır formatı annotation'daki gibi line: 5 değil, :line 12 — ayrı regex gerekiyor

6. Timestamp formatı
2026-08-03T08:30:39.2285010Z