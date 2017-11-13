using System.Linq;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class FinderTest : TestBase
	{
		[Test]
		public void Find_WhenSearchTextIsLicense_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "license";

			var resultItems = finder.Find().ItemsWithMatches;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(2, resultItems.Count);
			Assert.AreEqual("test1.txt", resultItems[0].FileName);
			Assert.AreEqual(3, resultItems[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsEENoRegExpr_FindsTextInBoth()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "ee";

			var resultItems = finder.Find().Items;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(3, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.txt").ToList();

			Assert.AreEqual(1, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.txt").ToList();

			Assert.AreEqual(1, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsNewYorkNoRegExpr_FindsNoMatches()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "New York";

			var resultItems = finder.Find().ItemsWithMatches.ToList();
			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Find_WhenSearchMaskIsTxt1_NoFindsText()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.txt1";
			finder.FindText = "a";

			var resultItems = finder.Find().Items;

			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Find_WhenSearchMaskIsTest1NoRegExpr_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "test1.*";
			finder.FindText = "ee";

			var resultItems = finder.Find().Items;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);

			Assert.AreEqual(5, matchedResultItems[0].NumMatches);

			Assert.AreEqual("test1.txt", matchedResultItems[0].FileName);
		}

		[Test]
		public void Find_WhenSearchTextIsSoAndCaseSensitiveNoRegExpr_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "So";
			finder.IsCaseSensitive = true;

			var resultItems = finder.Find().ItemsWithMatches;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test1.txt", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsEEAndUseSubDirNoRegExpr_FindsTextInFourFiles()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "ee";
			finder.IncludeSubDirectories = true;

			var resultItems = finder.Find().ItemsWithMatches;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");


			Assert.AreEqual(5, resultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.txt").ToList();

			Assert.AreEqual(2, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.AreEqual(5, firstFile[1].NumMatches);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.txt").ToList();

			Assert.AreEqual(2, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.AreEqual(1, secondFile[1].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsEmailPatternRegularExpression_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"; //email pattern
			finder.FindTextHasRegEx = true;

			var resultItems = finder.Find().ItemsWithMatches;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test2.txt", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchStartManyTimes_FindsTextInFourFiles()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "a+";
			finder.IncludeSubDirectories = true;
			finder.FindTextHasRegEx = true;

			Finder.FindResult result = null;
			
			for(int i = 0; i<10; i++)
			{
				result = finder.Find();
			}

			Assert.IsNotNull(result);
		}

		[Test]
		public void Find_WhenFindTextIsBinaryNull_FindsNoMatches()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "\0\0\0\0";

			Finder.FindResult result = finder.Find();

			Assert.AreEqual(1, result.Stats.Files.Binary);
			Assert.AreEqual(0, result.ItemsWithMatches.Count);
		}

		[Test]
		public void Find_WhenFindTextIsBinaryNull_And_SkipBinaryFileDetection_FindsMatch()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "\0\0\0\0";
			finder.SkipBinaryFileDetection = true;


			Finder.FindResult result = finder.Find();

			Assert.AreEqual(0, result.Stats.Files.Binary);
			Assert.AreEqual(1, result.ItemsWithMatches.Count);
		}

		[Test]
		public void Find_WhenSearchTextIsLicense12345_ReturnsNoResults()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "license12345";

			var resultItems = finder.Find().Items;

			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Find_WhenSearchTextIsLicense12345_And_IncludeFilesWithoutMatches_Returns4Results()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "license12345";
			finder.IncludeFilesWithoutMatches = true;

			var resultItems = finder.Find().Items;

			Assert.AreEqual(5, resultItems.Count);
		}

		[Test]
		public void Find_WheUseEscapeChars_ReturnsResults()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "test2.txt";
			finder.FindText = @"\r\n";
			finder.UseEscapeChars = true;

			var resultItems = finder.Find().Items;

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual(4, resultItems[0].Matches.Count);

			finder.FileMask = "test6.txt";
			finder.FindText = @"\t";
			finder.UseEscapeChars = true;

			resultItems = finder.Find().Items;

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual(1, resultItems[0].Matches.Count);
		}

		[Test]
		public void Find_WheNotUseEscapeChars_ReturnsEmptyResult()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "test2.txt";
			finder.FindText = @"\r\n";
			finder.UseEscapeChars = false;

			var resultItems = finder.Find().Items;

			Assert.AreEqual(0, resultItems.Count);
		}

	}
}
