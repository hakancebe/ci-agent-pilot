namespace CiPilot.Core;

// --- ÖLÇÜM (v0.8.3): fixable=false sinyalleri ---
// Üç ayrı derleyici hatası, üçü de farklı "belirsizlik" türü:
//   CS7036 - eksik argüman   -> bir değer koymak zorunlu (sınırda)
//   CS0535 - arayüz metodu uygulanmamis -> govde belirsiz  -> fixable=false beklenir
//   CS8602 - null dereference -> birden fazla mesru duzeltme -> fixable=false beklenir

public class EksikParametreTest
{
    public void Topla(int a, int b) { }

    public void Calistir()
    {
        Topla(5);
    }
}

public interface IHesapla
{
    int Hesapla(int x);
}

public class InterfaceHatasiTest : IHesapla
{
}

public class NullRefTest
{
    public void Calistir()
    {
        string? isim = null;
        int uzunluk = isim.Length;
    }
}
