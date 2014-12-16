using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IfClauseDiagnostic
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfClauseDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IfClauseDiagnostic";
        internal const string Title = "If clauses must be surrounded by braces";
        internal const string MessageFormat = "The {0} is not surrounded by braces.";
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfBlock, SyntaxKind.IfStatement);
        }

        private static void AnalyzeIfBlock(SyntaxNodeAnalysisContext context)
        {
            var statement = context.Node as IfStatementSyntax;

            var thenClause = statement.Statement;

            if (thenClause is ExpressionStatementSyntax)
            {
                // create the diagnostic:
                var diagnostic = Diagnostic.Create(Rule, thenClause.GetLocation(), "true clause");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
