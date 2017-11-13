using System;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using System.Linq;
using href.Utils;

namespace FindAndReplace.Tests
{
	[TestFixture]
	[Ignore]
	public class DetectEncodingAccuracyTest : TestBase
	{

		[SetUp]
		public override void SetUp()
		{
			CreateTestDir();
			
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
			string filePath = _tempDir + "\\" + encoding.EncodingName + ".txt";
			File.WriteAllText(filePath, fileContent, encoding);
		}



		[Test]
		public void MLang()
		{
			RunTest(EncodingDetector.Options.MLang);
		}

		[Test]
		public void KlerkSoftBom()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom);
		}


		private void RunTest(EncodingDetector.Options encDetectorOptions)
		{
			int totalFiles =0;
			int numFoundEncodings = 0;

			foreach (string filePath in Directory.EnumerateFiles(_tempDir, "*.*", SearchOption.AllDirectories))
			{
				var sampleBytes = Utils.ReadFileContentSample(filePath);

				Encoding encoding = EncodingDetector.Detect(sampleBytes, encDetectorOptions);
				totalFiles++;

				if (encoding != null)
					numFoundEncodings++;
				
				WriteToConsole(Path.GetFileName(filePath) + ": " + encoding);
				
				if (totalFiles > 10)
					break;
			}

			Console.WriteLine("Found Encoding in:" + numFoundEncodings + " out of " + totalFiles);
		}




		private void WriteToConsole(string line)
		{
			Console.WriteLine(DateTime.Now.ToString("hh:mm:ssss") + " ThreadId: " + Thread.CurrentThread.ManagedThreadId + " : " + line);
		}
	}


	public class UniversalEncodingDetector
	{

		[Flags]
		public enum Options
		{
			KlerkSoftBom = 1,
			KlerkSoftHeuristics = 2,
			MLang = 4
		}

		public static Encoding Detect(byte[] bytes, UniversalEncodingDetector.Options opts, Encoding defaultEncoding = null)
		{
			Encoding encoding = null;

			if ((opts & Options.KlerkSoftBom) == Options.KlerkSoftBom)
			{
				StopWatch.Start("DetectEncoding: UsingKlerksSoftBom");

				encoding = DetectEncodingUsingKlerksSoftBom(bytes);

				StopWatch.Stop("DetectEncoding: UsingKlerksSoftBom");
			}

			if (encoding != null)
				return encoding;

			if ((opts & Options.KlerkSoftHeuristics) == Options.KlerkSoftHeuristics)
			{
				StopWatch.Start("DetectEncoding: UsingKlerksSoftHeuristics");
				encoding = DetectEncodingUsingKlerksSoftHeuristics(bytes);
				StopWatch.Stop("DetectEncoding: UsingKlerksSoftHeuristics");
			}

			if (encoding != null)
				return encoding;

			if ((opts & Options.MLang) == Options.MLang)
			{
				StopWatch.Start("DetectEncoding: UsingMLang");
				encoding = DetectEncodingUsingMLang(bytes);
				StopWatch.Stop("DetectEncoding: UsingMLang");
			}

			if (encoding == null)
				encoding = defaultEncoding;


			return encoding;
		}

		private static Encoding DetectEncodingUsingKlerksSoftBom(byte[] bytes)
		{
			Encoding encoding = null;
			if (bytes.Count() >= 4)
				encoding = KlerksSoftEncodingDetector.DetectBOMBytes(bytes);

			return encoding;
		}


		private static Encoding DetectEncodingUsingKlerksSoftHeuristics(byte[] bytes)
		{
			Encoding encoding = KlerksSoftEncodingDetector.DetectUnicodeInByteSampleByHeuristics(bytes);

			return encoding;
		}

		private static Encoding DetectEncodingUsingMLang(Byte[] bytes)
		{
			try
			{
				Encoding[] detected = EncodingTools.DetectInputCodepages(bytes, 1);
				if (detected.Length > 0)
				{
					return detected[0];
				}
			}
			catch //(COMException ex)
			{
				// return default codepage on error
			}

			return null;
		}

	}
}
