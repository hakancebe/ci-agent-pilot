namespace CiPilot.Core;

// --- ÖLÇÜM (v0.8.3): fixable=false, gövde seviyesi iki hata ---
//   CS7036 - eksik argüman   -> bir değer koymak zorunlu (sınırda)
//   CS8602 - null dereference -> birden fazla meşru düzeltme -> fixable=false beklenir
// (CS0535 kaldırıldı: bildirim hatası varken Roslyn gövde analizini durduruyor,
//  diğer iki hata hiç raporlanmıyordu.)

public class EksikParametreTest
{
    public void Topla(int a, int b) { }

    public void Calistir()
    {
        Topla(5);
    }
}

public class NullRefTest
{
    public void Calistir()
    {
        string? isim = null;
        int uzunluk = isim.Length;
    }
}
