using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace IfClauseDiagnostic
{
    [ExportCodeFixProvider("IfClauseDiagnosticCodeFixProvider", LanguageNames.CSharp), Shared]
    public class IfClauseDiagnosticCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(IfClauseDiagnosticAnalyzer.DiagnosticId);
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task ComputeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var statement = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterFix(
                CodeAction.Create("Make Block", c => MakeBlockAsync(context.Document, statement, c)),
                diagnostic);
        }

        private async Task<Document> MakeBlockAsync(Document document, ExpressionStatementSyntax trueStatement, 
            CancellationToken cancellationToken)
        {
            var statementLeadingTrivia = trueStatement.GetLeadingTrivia();
            var statementLeadingWhiteSpace = trueStatement.Parent.GetLeadingTrivia()
                .Where(t => t.CSharpKind() == SyntaxKind.WhitespaceTrivia).Single();
            var endOfLineTrivia = SyntaxFactory.EndOfLine("\r\n");
            var blockLeadingTrivia = statementLeadingTrivia.Insert(0, endOfLineTrivia);

            var statements = new SyntaxList<StatementSyntax>();
            statements = statements.Add(trueStatement.WithLeadingTrivia(blockLeadingTrivia));

            var openingTokenWithTrivia = SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(statementLeadingWhiteSpace);
            var closingTokenWithTrivia = SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(statementLeadingWhiteSpace);

            var block = SyntaxFactory.Block(openingTokenWithTrivia, statements, closingTokenWithTrivia);

            var root = await document.GetSyntaxRootAsync();

            var newRoot = root.ReplaceNode((SyntaxNode)trueStatement, block);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}