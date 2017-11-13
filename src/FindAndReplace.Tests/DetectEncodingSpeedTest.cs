using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	[Ignore]
	public class DetectEncodingSpeedTest : TestBase
	{

		[SetUp]
		public override void SetUp()
		{
			CreateTestDir();
			CreateSpeedDir();

			WriteFiles(100000);
		}


		[TearDown]
		public override void TearDown()
		{
			DeleteTestDir();
		}


		private void WriteFiles(int fileSize)
		{
			string fileContent = GetFileContent(fileSize);

			CreateTestFile(fileContent, Encoding.ASCII);
			CreateTestFile(fileContent, Encoding.Unicode);
			CreateTestFile(fileContent, Encoding.BigEndianUnicode);
			CreateTestFile(fileContent, Encoding.UTF32);
			CreateTestFile(fileContent, Encoding.UTF7);
			CreateTestFile(fileContent, Encoding.UTF8);

		}

		private void CreateTestFile(string fileContent, Encoding encoding)
		{
			string filePath = _speedDir + "\\" + encoding.EncodingName + ".txt";
			File.WriteAllText(filePath, fileContent, encoding);
		}

		[Test]
		public void KlerkSoft_Bom_NewFiles()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom, _speedDir);
		}


		[Test]
		public void KlerkSoft_Heuristics_NewFiles()
		{
			RunTest(EncodingDetector.Options.KlerkSoftHeuristics, _speedDir);
		}


		[Test]
		public void MLang_NewFiles()
		{
			RunTest(EncodingDetector.Options.MLang, _speedDir);
		}

		[Test]
		public void KlerkSoft_Bom_Real_Dir()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom, Dir_StyleSalt);
		}

		[Test]
		public void KlerkSoft_Bom_And_Heuristics_Real_Dir()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom | EncodingDetector.Options.KlerkSoftHeuristics, Dir_StyleSalt);
		}


		[Test]
		public void MLang_Real_Dir()
		{
			RunTest(EncodingDetector.Options.MLang, Dir_StyleSalt);
		}

		[Test]
		public void KlerkSoft_Bom_And_MLang_Real_Dir()
		{
			RunTest(EncodingDetector.Options.MLang, Dir_StyleSalt);
		}



		[Test]
		public void MLang_Real_Dir_MultiThreaded()
		{
			//http://msdn.microsoft.com/en-us/library/aa767865(v=vs.85).aspx
			//http://www.dotnetframework.org/default.aspx/Dotnetfx_Vista_SP2/Dotnetfx_Vista_SP2/8@0@50727@4016/DEVDIV/depot/DevDiv/releases/whidbey/NetFxQFE/ndp/clr/src/BCL/System/Text/MLangCodePageEncoding@cs/1/MLangCodePageEncoding@cs
			//http://tech.nitoyon.com/ja/blog/2012/03/07/detectinputcodepage-in-cs/
			//http://code.logos.com/blog/c/

			//http://www.mobzystems.com/code/detecting-text-encoding
			//Use Preamble instead of Klerksoft?

			//http://www.codeproject.com/Articles/17201/Detect-Encoding-for-In-and-Outgoing-Text
			//still the best

			//First time seems a lot slower then second time with single thread.  Like 30%
			RunTestMLangMultiThreaded(Dir_StyleSalt, 1);

			//http://bytes.com/topic/net/answers/49348-multithreading-concurrency-interop
			//Interop junk
			for (int numThreads = 1; numThreads < 3; numThreads++) //Environment.ProcessorCount
				RunTestMLangMultiThreaded(Dir_StyleSalt, numThreads);
		}

		private void RunTest(EncodingDetector.Options encDetectorOptions, string dir)
		{
			int totalFiles =0;
			int numFoundEncodings = 0;

			StopWatch stopWatch = new StopWatch();
			stopWatch.Start();
			
			string detectorName = encDetectorOptions.ToString();
			foreach (string filePath in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
			{
				StopWatch.Start("ReadFileSample");

				var sampleBytes = Utils.ReadFileContentSample(filePath);

				StopWatch.Stop("ReadFileSample");
				
				StopWatch.Start("IsBinaryFile");
				
				if (Utils.IsBinaryFile(sampleBytes))
				{
					StopWatch.Stop("IsBinaryFile");
					continue;
				}
				
				StopWatch.Stop("IsBinaryFile");


				StopWatch.Start(detectorName);

			
				//First try BOM detection and Unicode detection using Klerks Soft encoder
				//stream.Seek(0, SeekOrigin.Begin);

				Encoding encoding = EncodingDetector.Detect(sampleBytes, encDetectorOptions);
				totalFiles++;

				if (encoding != null)
					numFoundEncodings++;
				
				StopWatch.Stop(detectorName);

				WriteToConsole(filePath + ": " + encoding);
				
				if (totalFiles > 10)
					break;
			}

			Console.WriteLine("Found Encoding in:" + numFoundEncodings + " out of " + totalFiles);

			stopWatch.Stop();

			StopWatch.PrintCollection(stopWatch.Milliseconds);
			StopWatch.Collection.Clear();
		}



		BlockingCollection<DetectEncodingFileDto> _filesToDetectEncoding;

		private int _numFoundEncodings = 0;
		private int _totalFiles = 0;


		private void RunTestMLangMultiThreaded(string dir, int numThreads)
		{
			_numFoundEncodings = 0;
			_totalFiles = 0;
			_filesToDetectEncoding = new BlockingCollection<DetectEncodingFileDto>();

			StopWatch stopWatch = new StopWatch();
			stopWatch.Start();

			Console.WriteLine("");
			Console.WriteLine("=====================================================");
			Console.WriteLine("Number of threads = " + numThreads);

			//EncodingTools.PreDetectInputCodepages2();

			var tasks = new List<Task>();
			for (int i = 0; i < numThreads; i++)
			{
				int i1 = i;
				tasks.Add((Task.Factory.StartNew(() => DetectEncodingAsyncAction(i1))));
			}

			int fileCount = 0;
			foreach (string filePath in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
			{
				StopWatch.Start("ReadFileSample");

				var sampleBytes = Utils.ReadFileContentSample(filePath);

				StopWatch.Stop("ReadFileSample");
				
				StopWatch.Start("IsBinaryFile");

				if (Utils.IsBinaryFile(sampleBytes))
				{
					StopWatch.Stop("IsBinaryFile");
					continue;
				}

				StopWatch.Stop("IsBinaryFile");

				fileCount++;

				if (fileCount > 1000)
					break;

				_filesToDetectEncoding.Add(new DetectEncodingFileDto {FilePath = filePath, SampleBytes = sampleBytes});
			}

			_filesToDetectEncoding.CompleteAdding();

			Task.WaitAll(tasks.ToArray());


			//EncodingTools.PostDetectInputCodepages2();

			Console.WriteLine("Found Encoding in:" + _numFoundEncodings + " out of " + _totalFiles);

			stopWatch.Stop();

			StopWatch.PrintCollection(stopWatch.Milliseconds);
			StopWatch.Collection.Clear();

			_filesToDetectEncoding.Dispose();
		}

		
		private class DetectEncodingFileDto
		{
			public string FilePath { get; set; }
			public byte[] SampleBytes { get; set; }
		}

		private void DetectEncodingAsyncAction(int taskIndex)
		{
			WriteToConsole("Started wait for DetectEncodingAsyncAction Task: " + taskIndex);

			DetectEncodingFileDto dto;
			while (_filesToDetectEncoding.Count > 0 || !_filesToDetectEncoding.IsAddingCompleted)
			{
				if (_filesToDetectEncoding.TryTake(out dto, 1))
					DetectEncodingSyncAction(dto);
			}

			WriteToConsole("Ended wait for DetectEncodingAsyncAction Task: " + taskIndex);
		}

		private Object lockObj = new Object();

		private void DetectEncodingSyncAction(DetectEncodingFileDto dto)
		{
			string stopWatchKey = "DetectEncodingSyncAction_" + Thread.CurrentThread.ManagedThreadId;
			StopWatch.Start(stopWatchKey);
			
			//First try BOM detection and Unicode detection using Klerks Soft encoder
			//stream.Seek(0, SeekOrigin.Begin);

			Encoding encoding = EncodingDetector.Detect(dto.SampleBytes, EncodingDetector.Options.MLang);

			//Encoding encoding = null;
			//Encoding[] detected;

			//try
			//{
			//	detected = EncodingTools.DetectInputCodepages(dto.SampleBytes, 1);

			//	if (detected.Length > 0)
			//		encoding = detected[0];
			//}
			//catch (COMException ex)
			//{
			//	// return default codepage on error
			//}

			

			lock (lockObj)
			{
				_totalFiles++;

				if (encoding != null)
					_numFoundEncodings++;
			}

			//WriteToConsole(dto.FilePath + ": " + encoding);

			StopWatch.Stop(stopWatchKey);
		}


		private void WriteToConsole(string line)
		{
			Console.WriteLine(DateTime.Now.ToString("hh:mm:ssss") + " ThreadId: " + Thread.CurrentThread.ManagedThreadId + " : " + line);
		}
	}
}
