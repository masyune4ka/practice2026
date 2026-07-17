using System;
using task14;
using Xunit;

namespace task14tests;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_IntegralOfX_FromMinus1To1_ReturnsZero()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, precision: 4);
    }

    [Fact]
    public void Solve_IntegralOfSin_FromMinus1To1_ReturnsApproximatelyZero()
    {
        var SIN = (double x) => Math.Sin(x);
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, precision: 4);
    }

    [Fact]
    public void Solve_IntegralOfX_From0To5_ReturnsApproximately12_5()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, precision: 5);
    }

    [Fact]
    public void Solve_WithMoreThreads_ReturnsCorrectResult()
    {
        var X = (double x) => x * x;
        double result = DefiniteIntegral.Solve(0, 3, X, 1e-4, 4);
        Assert.Equal(9.0, result, precision: 3);
    }
    [Fact]
    public void Solve_ConstantFunction_ReturnsCorrectResult()
    {
        var CONSTANT = (double x) => 5.0;
        double result = DefiniteIntegral.Solve(0, 10, CONSTANT, 1e-3, 4);
        Assert.Equal(50.0, result, precision: 2);
    }
    [Fact]
    public void Solve_NegativeInterval_ReturnsCorrectResult()
    {
        var X = (double x) => x;
        double result = DefiniteIntegral.Solve(-5, 0, X, 1e-4, 4);
        Assert.Equal(-12.5, result, precision: 3);
    }
    [Fact]
    public void Solve_ExponentialFunction_ReturnsCorrectResult()
    {
        var EXP = (double x) => Math.Exp(x);
        double result = DefiniteIntegral.Solve(0, 1, EXP, 1e-4, 4);
        Assert.Equal(1.718, result, precision: 2);
    }
}
