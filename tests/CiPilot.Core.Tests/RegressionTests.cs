using CiPilot.Core;
using Xunit;

namespace CiPilot.Core.Tests;

// Senaryo 6: Toplu regresyon (MaxLogChars=8000 tetikleme testi)
//
// Amaç: LlmService.TrimLog fonksiyonu pilot-data'daki 4 senaryonun hiçbirinde
// tetiklenmedi (en büyük RawStepLog 1.779 char, sınırın %22'si). Gerçek bir
// projede ortak bir regresyon (ör. bozulan bir shared fixture/base class) aynı
// anda onlarca testi kırabilir; LogParser.CombineTestFailures hepsini TEK
// RawStepLog'da birleştirdiği için bu, karakter sayısını doğrusala yakın
// büyütüyor. Bu dosya, o durumu kasıtlı olarak simüle ediyor.
//
// Bu geçici bir test senaryosudur — ölçüm sonrası bu commit revert edilecek.
public class RegressionTests
{
    [Fact] public void Add_Regression01() => Assert.Equal(101, new Calculator().Add(1, 1));
    [Fact] public void Add_Regression02() => Assert.Equal(102, new Calculator().Add(2, 2));
    [Fact] public void Add_Regression03() => Assert.Equal(103, new Calculator().Add(3, 3));
    [Fact] public void Add_Regression04() => Assert.Equal(104, new Calculator().Add(4, 4));
    [Fact] public void Add_Regression05() => Assert.Equal(105, new Calculator().Add(5, 5));
    [Fact] public void Add_Regression06() => Assert.Equal(106, new Calculator().Add(6, 6));
    [Fact] public void Add_Regression07() => Assert.Equal(107, new Calculator().Add(7, 7));
    [Fact] public void Add_Regression08() => Assert.Equal(108, new Calculator().Add(8, 8));

    [Fact]
    public void Add_Regression09()
        => throw new InvalidOperationException("Regresyon 09: shared fixture bozuldu");

    [Fact]
    public void Add_Regression10()
        => throw new InvalidOperationException("Regresyon 10: shared fixture bozuldu");

    [Fact]
    public void Add_Regression11()
        => throw new InvalidOperationException("Regresyon 11: shared fixture bozuldu");

    [Fact]
    public void Add_Regression12()
        => throw new InvalidOperationException("Regresyon 12: shared fixture bozuldu");

    [Fact]
    public void Add_Regression13()
        => throw new InvalidOperationException("Regresyon 13: shared fixture bozuldu");

    [Fact]
    public void Add_Regression14()
        => throw new InvalidOperationException("Regresyon 14: shared fixture bozuldu");
}
