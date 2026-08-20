using CiPilot.Core;
using Xunit;

namespace CiPilot.Core.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsSum()
    {
        var calc = new Calculator();
        Assert.Equal(300, calc.Add(2, 2));
    }

    [Fact]
    public void Add_WithNegativeNumber_ReturnsCorrectSum()
    {
        var calc = new Calculator();
        Assert.Equal(1, calc.Add(3, -2));
    }
}
