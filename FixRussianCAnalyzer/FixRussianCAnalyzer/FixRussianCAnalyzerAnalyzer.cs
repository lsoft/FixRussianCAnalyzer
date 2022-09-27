using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace FixRussianCAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FixRussianCAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Texting";
        public const string RussianCDiagnosticId = "RussianC";
        public const string EnglishCDiagnosticId = "EnglishC";

        private static readonly LocalizableString RussianCTitle = new LocalizableResourceString(nameof(Resources.RussianCAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString RussianCMessageFormat = new LocalizableResourceString(nameof(Resources.RussianCAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString RussianCDescription = new LocalizableResourceString(nameof(Resources.RussianCAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        public static readonly DiagnosticDescriptor RussianCRule = new DiagnosticDescriptor(RussianCDiagnosticId, RussianCTitle, RussianCMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: RussianCDescription);

        private static readonly LocalizableString EnglishCTitle = new LocalizableResourceString(nameof(Resources.EnglishCAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString EnglishCMessageFormat = new LocalizableResourceString(nameof(Resources.EnglishCAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString EnglishCDescription = new LocalizableResourceString(nameof(Resources.EnglishCAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        public static readonly DiagnosticDescriptor EnglishCRule = new DiagnosticDescriptor(EnglishCDiagnosticId, EnglishCTitle, EnglishCMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: EnglishCDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    RussianCRule,
                    EnglishCRule
                    );
            }
        }

        public static readonly Regex RussianCInEnglishWord = new Regex(@"(?<!\\)[A-Za-z][Сс]|[Сс][A-Za-z]", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        public static readonly Regex EnglishCInRussianWord = new Regex(@"[А-Яа-яЁё][Cc]|[Cc][А-Яа-яЁё]", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static readonly Regex[] Regexes = new[]
        {
            RussianCInEnglishWord,
            EnglishCInRussianWord
        };


        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            context.RegisterSyntaxTreeAction(AnalyzeDocument);
        }

        private void AnalyzeDocument(SyntaxTreeAnalysisContext context)
        {
            if (!context.Tree.HasCompilationUnitRoot)
            {
                return;
            }

            var text = context.Tree.ToString();
            foreach (var regex in Regexes)
            {
                foreach (var match in regex.Matches(text).Cast<Match>())
                {
                    if (ReferenceEquals(RussianCInEnglishWord, regex))
                    {
                        var diagnostic = Diagnostic.Create(
                            RussianCRule,
                            Location.Create(
                                context.Tree,
                                new Microsoft.CodeAnalysis.Text.TextSpan(match.Index, match.Length)
                                ),
                            match.Value
                            );
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if (ReferenceEquals(EnglishCInRussianWord, regex))
                    {
                        var diagnostic = Diagnostic.Create(
                            EnglishCRule,
                            Location.Create(
                                context.Tree,
                                new Microsoft.CodeAnalysis.Text.TextSpan(match.Index, match.Length)
                                ),
                            match.Value
                            );
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }


        }
    }
}
