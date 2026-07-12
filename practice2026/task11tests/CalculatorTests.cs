using System;
using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorGeneratorTests
    {
        private readonly ICalculator _calculator;

        public CalculatorGeneratorTests()
        {
            _calculator = CalculatorGenerator.CreateCalculator();
        }

        [Fact]
        public void CreateCalculator_ReturnsNonNull()
        {
            Assert.NotNull(_calculator);
        }

        [Fact]
        public void CreateCalculator_ReturnsICalculator()
        {
            Assert.IsAssignableFrom<ICalculator>(_calculator);
        }

        [Fact]
        public void Add_TwoPositiveNumbers_ReturnsSum()
        {
            Assert.Equal(5.0, _calculator.Add(2.0, 3.0));
        }

        [Fact]
        public void Minus_TwoNumbers_ReturnsDifference()
        {
            Assert.Equal(1.0, _calculator.Minus(3.0, 2.0));
        }

        [Fact]
        public void Mul_TwoNumbers_ReturnsProduct()
        {
            Assert.Equal(6.0, _calculator.Mul(2.0, 3.0));
        }

        [Fact]
        public void Div_TwoNumbers_ReturnsQuotient()
        {
            Assert.Equal(2.5, _calculator.Div(5.0, 2.0));
        }

        [Fact]
        public void Div_ByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => _calculator.Div(5.0, 0.0));
        }
    }
}
