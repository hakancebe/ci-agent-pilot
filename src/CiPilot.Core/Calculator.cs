namespace CiPilot.Core;

public class Calculator
{
    public int Add(int a, int b) => a + b;

    // CiAgent Faz 2 (/fix) uçtan uca doğrulaması için KASITLI derleme hatası.
    // Bu dal ve PR doğrulama bitince silinecek.
    public int Multiply(int a, int b) => a * b;
}
