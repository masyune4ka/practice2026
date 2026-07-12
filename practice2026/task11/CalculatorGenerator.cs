using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public static class CalculatorGenerator
    {
        private const string CalculatorSourceCode = @"
using System;
using System.Runtime.CompilerServices;
using task11;

namespace DynamicAssembly
{
    public class Calculator : ICalculator
    {
        public double Add(double a, double b) => a + b;
        public double Minus(double a, double b) => a - b;
        public double Mul(double a, double b) => a * b;
        public double Div(double a, double b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException();
            }
            return a / b;
        }
    }

    internal static class ModuleInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            CalculatorRegistry.Instance = new Calculator();
        }
    }
}
";

        public static ICalculator CreateCalculator()
        {
            CalculatorRegistry.Instance = null;

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(CalculatorSourceCode);

            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>();

            var task11Assembly = typeof(ICalculator).Assembly;
            references = references.Append(
                MetadataReference.CreateFromFile(task11Assembly.Location));

            CSharpCompilation compilation = CSharpCompilation.Create(
                "DynamicCalculator",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = string.Join(Environment.NewLine,
                    result.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d => d.GetMessage()));
                throw new InvalidOperationException(
                    $"Компиляция динамического класса не удалась:{Environment.NewLine}{errors}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            var loadContext = new AssemblyLoadContext("DynamicCalculatorContext", isCollectible: true);
            var assembly = loadContext.LoadFromStream(ms);

            RuntimeHelpers.RunModuleConstructor(assembly.ManifestModule.ModuleHandle);

            if (CalculatorRegistry.Instance == null)
            {
                throw new InvalidOperationException(
                    "Module initializer не установил экземпляр калькулятора в CalculatorRegistry.Instance.");
            }
            return CalculatorRegistry.Instance;
        }
    }
}
