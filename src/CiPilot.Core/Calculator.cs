namespace CiPilot.Core;

public class Calculator
{
    public int Add(int a, int b) => a + b;

    // CiAgent Faz 1 uçtan uca doğrulaması için KASITLI olarak eklenmiş derleme
    // hatası (eksik kapanış parantezi). Bu dal ve PR, doğrulama tamamlanınca
    // silinecek - agent üzerinde çalışan bir kod değişikliği DEĞİL.
    public int Subtract(int a, int b) => a - b
}
