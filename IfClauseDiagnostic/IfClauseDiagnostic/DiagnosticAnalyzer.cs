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
        internal const string Title = "Type name contains lowercase letters";
        internal const string MessageFormat = "Type name '{0}' contains lowercase letters";
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfBlock, SyntaxKind.IfStatement);
        }

        private static void AnalyzeIfBlock(SyntaxNodeAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var statement = context.Node;
        }
    }
}
