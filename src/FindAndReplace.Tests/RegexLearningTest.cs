using System.Text.RegularExpressions;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class RegexLearningTest
	{

		//This is for reasearch for https://findandreplace.codeplex.com/discussions/470706
		//Testing fix: http://stackoverflow.com/questions/1191014/how-to-regex-replace-named-groups
		[Test]
		public void Replace_WhenReplacingStaticPartOfPattern_ReplacesText()
		{
			string text = "A x@x.com letter --- alphabetical ---- missing ---- lack release penchant slack acryllic laundry A x@x.com hh --- cease";
			string findPattern = @"(A x@x.com .*? )---";
			string replaceText = "$1-B-";

			var result = Regex.Replace(text, findPattern, replaceText);

			Assert.AreEqual("A x@x.com letter -B- alphabetical ---- missing ---- lack release penchant slack acryllic laundry A x@x.com hh -B- cease", result);
		}



		[Test]
		public void Replace_WhenReplacingDynamicPartOfPattern_ReplacesText()
		{
			string text = "A x@x.com letter --- alphabetical ---- missing ---- lack release penchant slack acryllic laundry A x@x.com hh --- cease";
			string findPattern = @"(A x@x.com )(.*?)( ---)";
			string replaceText = "$1-B-$3";

			var result = Regex.Replace(text, findPattern, replaceText);

			Assert.AreEqual("A x@x.com -B- --- alphabetical ---- missing ---- lack release penchant slack acryllic laundry A x@x.com -B- --- cease", result);
		}
	}
}
