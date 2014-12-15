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

        private async Task<Document> MakeBlockAsync(Document document, ExpressionStatementSyntax trueStatement, CancellationToken cancellationToken)
        {
            var leading = trueStatement.GetLeadingTrivia();
            var leadingEol = SyntaxFactory.EndOfLine("\r\n");
            var newLeading = leading.Insert(0, leadingEol);

            var block = SyntaxFactory.Block(trueStatement.WithLeadingTrivia(newLeading));

            var ifLeadingWhiteSpace = trueStatement.Parent.GetLeadingTrivia().Where(t => t.CSharpKind() == SyntaxKind.WhitespaceTrivia).First();
            block = block.WithLeadingTrivia(ifLeadingWhiteSpace);
            var newClosing = block.CloseBraceToken.WithLeadingTrivia(ifLeadingWhiteSpace);
            var statements = new SyntaxList<StatementSyntax>();
            statements = statements.Add(trueStatement.WithLeadingTrivia(newLeading));
            block = SyntaxFactory.Block(block.OpenBraceToken, statements, newClosing);

            var root = await document.GetSyntaxRootAsync();

            var newRoot = root.ReplaceNode((SyntaxNode)trueStatement, block);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}