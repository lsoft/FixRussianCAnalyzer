using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = FixRussianCAnalyzer.Test.CSharpCodeFixVerifier<
    FixRussianCAnalyzer.FixRussianCAnalyzerAnalyzer,
    FixRussianCAnalyzer.FixRussianCAnalyzerCodeFixProvider>;

namespace FixRussianCAnalyzer.Test
{
    [TestClass]
    public class Fixture
    {
        [TestMethod]
        public async Task Empty()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task RussianC_Small_NoMute()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //blac blacbla cbla
        //bl{|#0:aс|} bl{|#1:aс|}bla {|#2:сb|}la
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //blac blacbla cbla
        //blac blacbla cbla
    }";

            var expected0 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.RussianCRule)
                .WithLocation(0).WithArguments("aс")
                ;
            var expected1 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.RussianCRule)
                .WithLocation(1).WithArguments("aс")
                ;
            var expected2 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.RussianCRule)
                .WithLocation(2).WithArguments("сb")
                ;
            await VerifyCS.VerifyCodeFixAsync(test, new[] { expected0, expected1, expected2 }, fixtest);
        }

        [TestMethod]
        public async Task RussianC_Big_NoMute()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //blac blacbla cbla
        //bl{|#0:aС|} bl{|#1:aС|}bla {|#2:Сb|}la
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //blac blacbla cbla
        //blaC blaCbla Cbla
    }";

            var expected0 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.RussianCRule)
                .WithLocation(0).WithArguments("aС")
                ;
            var expected1 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.RussianCRule)
                .WithLocation(1).WithArguments("aС")
                ;
            var expected2 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.RussianCRule)
                .WithLocation(2).WithArguments("Сb")
                ;
            await VerifyCS.VerifyCodeFixAsync(test, new[] { expected0, expected1, expected2 }, fixtest);
        }

        [TestMethod]
        public async Task EnglishC_Small_NoMute()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //бла бласбла бла
        //бл{|#0:аc|} бл{|#1:аc|}бла {|#2:cб|}ла
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //бла бласбла бла
        //блас бласбла сбла
    }";

            var expected0 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.EnglishCRule)
                .WithLocation(0).WithArguments("аc")
                ;
            var expected1 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.EnglishCRule)
                .WithLocation(1).WithArguments("аc")
                ;
            var expected2 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.EnglishCRule)
                .WithLocation(2).WithArguments("cб")
                ;
            await VerifyCS.VerifyCodeFixAsync(test, new[] { expected0, expected1, expected2 }, fixtest);
        }

        [TestMethod]
        public async Task EnglishC_Big_NoMute()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //бла бласбла бла
        //бл{|#0:аC|} бл{|#1:аC|}бла {|#2:Cб|}ла
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        //бла бласбла бла
        //блаС блаСбла Сбла
    }";

            var expected0 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.EnglishCRule)
                .WithLocation(0).WithArguments("аC")
                ;
            var expected1 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.EnglishCRule)
                .WithLocation(1).WithArguments("аC")
                ;
            var expected2 = VerifyCS.Diagnostic(FixRussianCAnalyzerAnalyzer.EnglishCRule)
                .WithLocation(2).WithArguments("Cб")
                ;
            await VerifyCS.VerifyCodeFixAsync(test, new[] { expected0, expected1, expected2 }, fixtest);
        }
    }
}
