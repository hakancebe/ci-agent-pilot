using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// TEK SEFERDE SADECE 1 SENARYOYU AKTİF BIRAKIP DİĞERLERİNİ YORUM SATIRI YAP
// Hepsi aynı anda açıksa çok sayıda hata birden basar (istersen öyle de test edebilirsin)

// // 1) Syntax hatası (CS1002)
// public class SyntaxHatasiTest
// {
//     public void Calistir()
//     {
//         Console.WriteLine("Merhaba")
//     }
// }

// 2) Tip uyuşmazlığı (CS0029)
public class TipUyusmazligiTest
{
    public void Calistir()
    {
        int sayi = 0;
    }
}

// // 3) Tanımsız değişken (CS0103)
// public class TanimsizDegiskenTest
// {
//     public void Calistir()
//     {
//         Console.WriteLine(tanimsizDegisken);
//     }
// }

// // 4) Eksik using / bilinmeyen tip (CS0246)
// public class EksikUsingTest
// {
//     public void Calistir()
//     {
//         Liste<string> liste = new Liste<string>();
//     }
// }

// // 5) Erişim belirleyici hatası (CS0122)
// public class GizliMetodSahibi
// {
//     private void GizliMetod() { }
// }

// public class ErisimHatasiTest
// {
//     public void Calistir()
//     {
//         new GizliMetodSahibi().GizliMetod();
//     }
// }

// // 6) Eksik parametre (CS7036)
// public class EksikParametreTest
// {
//     public void Topla(int a, int b) { }

//     public void Calistir()
//     {
//         Topla(5);
//     }
// }

// // 7) Interface implement edilmemiş (CS0535)
// public interface IHesapla
// {
//     int Hesapla(int x);
// }

// public class InterfaceHatasiTest : IHesapla
// {
// }

// // 8) Nullable reference hatası (CS8602)
// #nullable enable
// public class NullRefTest
// {
//     public void Calistir()
//     {
//         string? isim = null;
//         int uzunluk = isim.Length;
//     }
// }
// #nullable disable

// // 9) Duplicate tip tanımı (CS0101)
// public class DuplicateTest
// {
//     public class DuplicateTest { }
// }

// // 10) Async/await yanlış kullanımı (CS4033)
// public class AsyncHatasiTest
// {
//     public void MetodAsenkronDegil()
//     {
//         var sonuc = await Task.Delay(1000);
//     }
// }