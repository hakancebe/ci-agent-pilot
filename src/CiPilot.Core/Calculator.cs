namespace CiPilot.Core;

public class Calculator
{
    public int Add(int a, int b) => a + b;

    // v0.3.4 + App Insights uçtan uca son doğrulama için KASITLI hata.
    public int Modulo(int a, int b) => a % b
}
