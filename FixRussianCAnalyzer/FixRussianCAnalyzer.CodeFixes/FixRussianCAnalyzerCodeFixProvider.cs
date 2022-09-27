using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FixRussianCAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixRussianCAnalyzerCodeFixProvider)), Shared]
    public class FixRussianCAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    FixRussianCAnalyzerAnalyzer.RussianCDiagnosticId,
                    FixRussianCAnalyzerAnalyzer.EnglishCDiagnosticId
                    );
            }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            if (diagnostic.Id != FixRussianCAnalyzerAnalyzer.RussianCDiagnosticId
                && diagnostic.Id != FixRussianCAnalyzerAnalyzer.EnglishCDiagnosticId
                )
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedSolution: c => FixSolution(diagnostic, context.Document, root),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private static Task<Solution> FixSolution(
            Diagnostic diagnostic,
            Document document,
            SyntaxNode root)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var text = root.GetText();
            var incorrectText = text.GetSubText(diagnosticSpan).ToString();

            string correctText;
            if (diagnostic.Id == FixRussianCAnalyzerAnalyzer.RussianCDiagnosticId)
            {
                correctText = incorrectText.Replace('С', 'C').Replace('с', 'c');
            }
            else if (diagnostic.Id == FixRussianCAnalyzerAnalyzer.EnglishCDiagnosticId)
            {
                correctText = incorrectText.Replace('C', 'С').Replace('c', 'с');
            }
            else
            {
                correctText = incorrectText;
            }

            var fixedText = text.Replace(diagnosticSpan, correctText);

            return Task.FromResult(document.WithText(fixedText).Project.Solution);
        }
    }
}
