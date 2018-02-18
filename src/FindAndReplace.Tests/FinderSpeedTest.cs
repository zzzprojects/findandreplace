using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class FinderSpeedTest : TestBase
	{
		//1000 chars
		const string Chars1000 =
				@"The GNU General Public License is a free, copyleft license for
software and other kinds of works.

  The licenses for most software and other practical works are designed
to take away your freedom to share and change the works.  By contrast,
the GNU General Public License is intended to guarantee your freedom to
share and change all versions of a program--to make sure it remains free
software for all its users.  We, the Free Software Foundation, use the
GNU General Public License for most of our software; it applies also to
any other work released this way by its authors.  You can apply it to
your programs, too.

  When we speak of free software, we are referring to freedom, not
price.  Our General Public Licenses are designed to make sure that you
have the freedom to distribute copies of free software (and charge for
them if you wish), that you receive source code or can get it if you
want it, that you can change the software or use pieces of it in new
free program...";


		[SetUp]
		public override void SetUp()
		{
			CreateTestDir();
		}


		[TearDown]
		public override void TearDown()
		{
			DeleteTestDir();
		}

		private bool _addToAllTestResults;
		private List<FinderTestResult> _allTestResults;
	


		private void WriteFiles(int numFiles, int fileSize)
		{
			string fileContent = GetFileContent(fileSize);

			for (int i = 1; i <= numFiles; i++)
			{
				CreateTestFile(i, fileContent);
			}	
		}

	

		private void CreateTestFile(int index, string fileContent)
		{
			string filePath = _speedDir + "\\" + index + ".txt";
			FileStream fs = new FileStream(filePath, FileMode.Create);
			StreamWriter sr = new StreamWriter(fs);

			sr.Write(fileContent);

			sr.Close();
			fs.Close();

		}

		[Test]
		public void By_FileSize()
		{
			for (int i = 1; i < 101; i=i*10)
				TestFinder("By_FileSize", 100, 1000 * i);
		}


		[Test]
		public void By_NumFiles()
		{
			//Use same files 10K - 10, 100, 1000
			for (int i = 10; i < 1001; i = i * 10)
				TestFinder("By_NumFiles", i, 10000);
		}



		[Test]
		public void In_Real_Directory()
		{
			string realDir = "..//..//bin";
			 
			StopWatch stopWatch = new StopWatch();
			stopWatch.Start();
			
			Finder finder = new Finder();

			finder.Dir = realDir;
			finder.FileMask = "*.*";
			finder.ExcludeFileMask = "*.dll, *.exe";
			finder.FindText = "page";
			finder.IncludeSubDirectories = true;

			var findResult = finder.Find();

			//Assert.AreEqual(numFiles, findResult.Stats.Files.Total);
			
			stopWatch.Stop();

			if (_addToAllTestResults)
				AddToAllTestResults("In_Real_Directory", findResult.Stats.Files.Total, -1);
			else
				StopWatch.PrintCollection(stopWatch.Milliseconds);		

			StopWatch.Collection.Clear();
		}



		[Test]
		public void Use_RegEx()
		{
			const string regExExpression = "\\b(F|f)ree\\b";
			TestFinder("Use_RegEx", 100, 10000, regExExpression);
		}


		private void TestFinder(string testName, int numFiles, int fileSizeInChars, string regExExpression = null)
		{
			CreateSpeedDir();

			WriteFiles(numFiles, fileSizeInChars);


			StopWatch stopWatch = new StopWatch();
			stopWatch.Start();

			Finder finder = new Finder();

			finder.Dir = _speedDir;
			finder.FileMask = "*.*";

			if (regExExpression == null)
			{
				finder.FindText = "it";
			}
			else
			{
				finder.FindText = regExExpression;
				finder.FindTextHasRegEx = true;
			}

			var findResult = finder.Find();

			Assert.AreEqual(numFiles, findResult.Stats.Files.Total);
			//Assert.AreEqual(5000, findResult.Stats.Matches.Found);


			stopWatch.Stop();

			if (_addToAllTestResults)
			{
				AddToAllTestResults(testName, numFiles, fileSizeInChars);
			}
			else
			{
				Console.WriteLine("=================================================");
				Console.WriteLine("Num Files = " + numFiles + ", File Size=" + fileSizeInChars + " chars ");
				Console.WriteLine("==================================================");

				StopWatch.PrintCollection(stopWatch.Milliseconds);
				Console.WriteLine("");
			}

			StopWatch.Collection.Clear();

			DeleteSpeedDir();
		}

		private void AddToAllTestResults(string testName, int numFiles, int fileSizeInChars)
		{
			string fullTestName = testName + " (file size: " + (fileSizeInChars == -1 ? "N/A" : fileSizeInChars.ToString()) + " chars, num files: " + numFiles + ")";
			var finderTestResult = new FinderTestResult
			{
				TestName = fullTestName,
				StopWatches = new Dictionary<string, StopWatch>(StopWatch.Collection)
			};
			_allTestResults.Add(finderTestResult);
		}



		[Test]
		public void Run_All_Tests()
		{
			_addToAllTestResults = true;
			_allTestResults = new List<FinderTestResult>();

			By_FileSize();
			By_NumFiles();
			In_Real_Directory();
			Use_RegEx();
		
			
			WriteHeadingsRow();
			
			WriteRow("GetFilesInDirectory");
			WriteRow("ReadSampleFileContent");
			WriteRow("IsBinaryFile");
			WriteRow("DetectEncoding: UsingKlerksSoftBom");
			WriteRow("DetectEncoding: UsingMLang");
			WriteRow("ReadFullFileContent");
			WriteRow("FindMatches");
			
			WriteTotalsRow();
			

			_addToAllTestResults = false;
		}

		


		private void WriteHeadingsRow()
		{
			for (int i = 0; i < _allTestResults.Count; i++)
			{
				Console.WriteLine("Test " + (i + 1) + ": " + _allTestResults[i].TestName);
			}
			Console.WriteLine("");
			Console.WriteLine("");

			var rowVals = new List<string> {"Action"};

			for (int i = 0; i < _allTestResults.Count; i++)
			{
				rowVals.Add("Test " + (i + 1));
			}

			WriteRow(rowVals);
		}


		private void WriteTotalsRow()
		{
			var rowVals = new List<string>();
			rowVals.Add("Total Test Time:");
			

			for (int i = 0; i < _allTestResults.Count; i++)
			{
				int totalMilliseconds = 0;
				foreach (StopWatch sw in _allTestResults[i].StopWatches.Values)
				{
					totalMilliseconds += sw.Milliseconds;
				}
			
				rowVals.Add(totalMilliseconds + "ms");
			}

			WriteRow(rowVals);
		}

		

		private void WriteRow(string actionName)
		{
			var rowVals = new List<string>();
			rowVals.Add(actionName);
			
			foreach (var testResult in _allTestResults)
			{
				//Total millisecs for single test
				int totalMilliseconds = testResult.StopWatches.Sum(sw => sw.Value.Milliseconds);

				var stopWatch = testResult.StopWatches[actionName];
				
				//decimal avgDuration = Math.Round((((decimal)stopWatch.Milliseconds) / stopWatch.StopCount), 1);
				decimal percent = Math.Round((stopWatch.Milliseconds / (decimal)totalMilliseconds) * 100, 1);

				//rowVals.Add(percent + "% | avg " + avgDuration + " | tl " + stopWatch.Milliseconds);
				rowVals.Add(percent.ToString() + "%");

			}

			WriteRow(rowVals);
		}


		private void WriteRow(List<string> rowVals)
		{
			for (int i = 0; i < rowVals.Count; i++)
			{
				if (i==0)
					rowVals[i] = rowVals[i].PadLeft(35);
				else
					rowVals[i] = rowVals[i].PadLeft(10);
			
			}
				
			Console.WriteLine(string.Join("  ", rowVals));
		}

	}

	internal class FinderTestResult
	{
		public string TestName { get; set; }
		public Dictionary<string, StopWatch> StopWatches { get; set; } 
	}
}
