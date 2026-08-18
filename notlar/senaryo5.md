1. Zip yapısı ve step sınırları

Senaryo 1 ile birebir aynı — 5 step grubu, Test step'i satır 142'de başlıyor. Fark yalnızca hata bloğunun içeriğinde.

2. Hata bloğu formatı — senaryo 1'den farkı

Failed CiPilot.Core.Tests.CalculatorTests.ThrowsUnexpectedException [< 1 ms]
  Error Message:
   System.InvalidOperationException : Beklenmeyen hata
  Stack Trace:
     at CiPilot.Core.Tests.CalculatorTests.ThrowsUnexpectedException() in /home/runner/work/ci-agent-pilot/ci-agent-pilot/tests/CiPilot.Core.Tests/CalculatorTests.cs:line 25
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(...)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(...)

Fark, tahmin ettiğimiz gibi çıktı: Expected: / Actual: satırları yok. Bunların yerine tek satırlık bir exception özeti var: <ExceptionTipi> : <mesaj>.

3. İki alt format, tek genel kalıba indirgenebilir

	Senaryo 1 (Assert)	Senaryo 5 (throw)
Başlangıç çıpası	  Failed <TestAdı> [	Aynı
Error Message içeriği	Assert.Equal() Failure: Values differ + ayrı Expected:/Actual: satırları	<ExceptionTipi> : <mesaj> tek satırda
Stack Trace	Aynı format	Aynı format
Bitiş çıpası	Failed!  - Failed: özet satırı	Aynı

Sonuç: regex'in "Error Message:" ile "Stack Trace:" arasındaki her şeyi olduğu gibi (ayrıştırmadan) LLM'e vermesi yeterli. Expected/Actual'ı ayrı alanlara ayırmaya çalışmak gereksiz karmaşıklık — LLM zaten "Assert.Equal() Failure: Values differ / Expected: 5 / Actual: 4" metnini okuyup anlayabilir, aynı şekilde "System.InvalidOperationException : Beklenmeyen hata" metnini de anlar. Ayrıştırma sadece blok sınırlarını (başlangıç/bitiş) bulmak için gerekli, blok içeriğini alt alanlara bölmeye gerek yok.

4. Stack trace formatı birebir aynı

at CiPilot.Core.Tests.CalculatorTests.ThrowsUnexpectedException() in /home/runner/.../CalculatorTests.cs:line 25

Senaryo 1'deki dosya yolu kırpma ve framework satırı filtreleme kuralları (System.Reflection... satırlarının atılması) burada da aynen geçerli — ayrı bir kural gerekmiyor.

5. Sonuç: xUnit hata bloğu için TEK regex ailesi yeterli

Assert hatası ile runtime exception, aynı zarfın (başlangıç/bitiş çıpaları) içinde farklı içerik taşıyor ama zarfın kendisi değişmiyor. Adım 2'de:

"  Failed " ile başlayan satırdan
"Failed!  - Failed:" özet satırına (veya sonraki ##[group]/##[error]'a) kadar
→ olduğu gibi al, alt alanlara ayırma, LLM'e ham blok olarak gönder

Bu, planı basitleştiriyor — iki ayrı xUnit alt-parser yazmana gerek yok.