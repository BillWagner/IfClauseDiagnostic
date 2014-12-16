using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using IfClauseDiagnostic;

namespace IfClauseDiagnostic.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void IfStatementWithoutBraces()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{
    class Program
    {
        static void Main(string[] args)
        {
            bool b1 = true;

            if (b1)
                Console.WriteLine(""b1"");

        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = IfClauseDiagnosticAnalyzer.DiagnosticId,
                Message = "The true clause is not surrounded by braces.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 17, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{
    class Program
    {
        static void Main(string[] args)
        {
            bool b1 = true;

            if (b1)
            {
                Console.WriteLine(""b1"");
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        //Diagnostic finds already corrected code.
        [TestMethod]
        public void IfStatementWithBraces()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{
    class Program
    {
        static void Main(string[] args)
        {
            bool b2 = true;

            if (b2)
            {
                Console.WriteLine(""b2"");
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void IfElseStatementWithoutBraces()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{
    class Program
    {
        static void Main(string[] args)
        {
            bool b3 = true;

            if (b3)
                Console.WriteLine(""b3"");
            else
                Console.WriteLine(""not b3"");
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = IfClauseDiagnosticAnalyzer.DiagnosticId,
                Message = "The true clause is not surrounded by braces.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 17, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{
    class Program
    {
        static void Main(string[] args)
        {
            bool b3 = true;

            if (b3)
            {
                Console.WriteLine(""b3"");
            }
            else
            {
                Console.WriteLine(""not b3"");
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IfClauseDiagnosticCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new IfClauseDiagnosticAnalyzer();
        }
    }
}