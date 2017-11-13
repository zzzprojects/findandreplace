using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace FindAndReplace.Tests
{


	[TestFixture]
	public class ReplacerTest : TestBase
	{

		[Test]
		public void Replace_WhenSearchTextIsLicenseNoRegExpr_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "license";
			replacer.ReplaceText = "agreement";

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(2, resultItems.Count);
			Assert.AreEqual("test1.txt", resultItems[0].FileName);
			Assert.AreEqual(3, resultItems[0].NumMatches);
			Assert.IsTrue(resultItems[0].IsSuccess);

			resultItems = replacer.Replace().ResultItems;

			resultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();
			Assert.AreEqual(0, resultItems.Count);
		}
		
		[Test]
		public void Replace_WhenSearchTextIsEENoRegExpr_ReplacesTextInBoth()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";


			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();
			Assert.AreEqual(3, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.txt").ToList();

			Assert.AreEqual(1, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.IsTrue(firstFile[0].IsSuccess);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.txt").ToList();

			Assert.AreEqual(1, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.IsTrue(secondFile[0].IsSuccess);
		}
		
		[Test]
		public void Replace_WhenSearchTextIsNewYork_NoReplacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "New York";
			replacer.ReplaceText = "Moscow";

			var resultItems = replacer.Replace().ResultItems;

			Assert.AreEqual(0, resultItems.Count);
		}
		
		[Test]
		public void Replace_WhenFileMaskIsTxt1Only_NoRepacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.txt1";
			replacer.FindText = "a";
			replacer.ReplaceText = "b";

			var resultItems = replacer.Replace().ResultItems;

			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Replace_WhenFileMaskIsTest1_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "test1.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);
			Assert.AreEqual(5, matchedResultItems[0].NumMatches);
			Assert.AreEqual("test1.txt", matchedResultItems[0].FileName);
			Assert.IsTrue(matchedResultItems[0].IsSuccess);

			resultItems = replacer.Replace().ResultItems;

			matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, matchedResultItems.Count);
		}

		[Test]
		public void Replace_WhenSearchTextIsSoAndCaseSensitiveNoRegExpr_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "So";
			replacer.IsCaseSensitive = true;
			replacer.ReplaceText = "Su";

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test1.txt", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
			Assert.IsTrue(resultItems[0].IsSuccess);

		
			resultItems = replacer.Replace().ResultItems;
			Assert.AreEqual(0, resultItems.Count);
		}
		
		[Test]
		public void Replace_WhenSearchTextIsEEAndUseSubDirNoRegExpr_ReplacesTextInFour()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";
			replacer.IncludeSubDirectories = true;

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(5, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.txt").ToList();

			Assert.AreEqual(2, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.AreEqual(5, firstFile[1].NumMatches);
			Assert.IsTrue(firstFile[0].IsSuccess);
			Assert.IsTrue(firstFile[1].IsSuccess);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.txt").ToList();

			Assert.AreEqual(2, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.IsTrue(secondFile[0].IsSuccess);
			Assert.AreEqual(1, secondFile[1].NumMatches);
			Assert.IsTrue(secondFile[1].IsSuccess);
		}

		[Test]
		public void Replace_WhenSearchTextIsRegularExpression_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"; //email pattern
			replacer.ReplaceText = "Email was here..";
			replacer.FindTextHasRegEx = true;

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test2.txt", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
		}

		[Test]
		public void Replace_WhenFileIsReadonly_CanNotRepacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "readonly";
			replacer.ReplaceText = "I can replace";

			var resultItems = replacer.Replace().ResultItems.Where(r=>!r.IsSuccess).ToList();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test3.txt", resultItems[0].FileName);
			Assert.IsTrue(resultItems[0].FailedToWrite);
			Assert.IsNotEmpty(resultItems[0].ErrorMessage);
		}

		[Test]
		public void Replace_WhenSpanish_KeepsEncoding()
		{
			string filePath = _tempDir + "\\test_spanish.txt";
			string fileContent = @"Line1
								Line2
								Hammam Cinili,Hydrotherapy,Baths,sauna, hidro, esfoliação211112, hidratação (1 1/2 créditos)";
			WriteFile(filePath, fileContent);

			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "test_spanish.txt";
			replacer.FindText = "Hammam Cinili";
			replacer.ReplaceText = "Hammam Cinili1";

			var resultItems = replacer.Replace().ResultItems.Where(r => r.IsSuccess).ToList();
			Assert.AreEqual(1, resultItems.Count);

			string expectedFileContent = @"Line1
								Line2
								Hammam Cinili1,Hydrotherapy,Baths,sauna, hidro, esfoliação211112, hidratação (1 1/2 créditos)";
			string actualFileContent = ReadFile(filePath);
			
			Assert.AreEqual(expectedFileContent, actualFileContent);
		}

		[Test]
		public void Replace_WhenNonAsciiSymbols_KeepsEncoding()
		{
			string filePath = _tempDir + "\\test_symbols.txt";
			string fileContent = @"Line1
								Line2
								©[assembly: AssemblyCopyright(Copyright © 2009-2011 My Company)]";
			WriteFile(filePath, fileContent);

			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "test_symbols.txt";
			replacer.FindText = "©[assembly:";
			replacer.ReplaceText = "©[assembly:123";

			var resultItems = replacer.Replace().ResultItems.Where(r => r.IsSuccess).ToList();
			Assert.AreEqual(1, resultItems.Count);

			string expectedFileContent = @"Line1
								Line2
								©[assembly:123 AssemblyCopyright(Copyright © 2009-2011 My Company)]";
			string actualFileContent = ReadFile(filePath, Encoding.Default);

			Assert.AreEqual(expectedFileContent, actualFileContent);
		}

		[Test]
		public void Replace_WhenUseEscapeCahrs_RepacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "test6.txt";
			replacer.FindText = @"\t";
			replacer.ReplaceText = @"\r\n";
			replacer.UseEscapeChars = true;

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Can't find test files");

			Assert.AreEqual(1, resultItems.Count);
			
			Finder finder = new Finder();
			finder.Dir = _tempDir;
			finder.FileMask = "test6.txt";
			finder.FindText = @"\t";
			finder.UseEscapeChars = true;
			finder.IncludeFilesWithoutMatches = true;

			var findResultItems = finder.Find().Items;
			Assert.AreEqual(1, findResultItems.Count);
			Assert.AreEqual(0, findResultItems[0].Matches.Count);

			finder.FindText = @"\r\n";
			findResultItems = finder.Find().Items;
			Assert.AreEqual(1, findResultItems.Count);
			Assert.AreEqual(3, findResultItems[0].Matches.Count);
		}


		private string ReadFile(string filePath, Encoding encoding = null)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			using (var sr = new StreamReader(filePath, encoding))
			{
				return sr.ReadToEnd();
			}
		}

		private void WriteFile(string filePath, string fileContent, Encoding encoding = null)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			using (var fs = new FileStream(filePath, FileMode.Create))
			{
				using (var sr = new StreamWriter(fs, encoding))
					sr.Write(fileContent);
			}			
		}

	}
}
