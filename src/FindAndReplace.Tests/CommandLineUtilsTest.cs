using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class CommandLineUtilsTest
	{
		private string _applicationExePath = @"FindAndReplace.Tests.CommandLine.exe";

		//From https://findandreplace.codeplex.com/workitem/17
		[Test]
		public void Encode_Decode_FromWorkItem17_ReturnsSameValue()
		{
			TestEncodeDecode("\\r(?!\\n)", true);
		}

		public void TestEncodeDecode(string text, bool hasRegEx = false, bool useEscapeChars = false)
		{
			var cmdText = GenCommandLine(text, hasRegEx, useEscapeChars);

			System.Diagnostics.Process.Start(_applicationExePath, cmdText);
			Thread.Sleep(1000);

			var decodedValue = GetValueFromOutput();

			Assert.AreEqual(text, decodedValue);
		}

		[Test]
		public void Encode_Decode_FromDiscussions541024_ReturnsSameValue()
		{
			TestEncodeDecode("<TargetFrameworkVersion>v[24].0</TargetFrameworkVersion>", true);
		}


		[Test]
		public void Encode_Decode_FromWorkItem35_ReturnsSameValue()
		{
			TestEncodeDecode("\\n line1\r\n    line2");
		}


		[Test]
		public void Encode_Decode_When_backslash_followed_by_quote_Then_returns_same()
		{
			TestEncodeDecode("\\\"");

		}


		[Test]
		public void Encode_Decode_When_two_backslashes_followed_by_quote_Then_returns_same()
		{
			TestEncodeDecode(@"\\""");
		}

		[Test]
		public void Encode_Decode_When_backslash_followed_by_quote_several_times_Then_returns_same()
		{
			TestEncodeDecode(@"\"" hello \\""  ""\");
		}


		[Test]
		public void Encode_Decode_When_one_backslash_Then_returns_same()
		{
			TestEncodeDecode(@"\");
		}

		[Test]
		public void Encode_Decode_When_two_backslashes_Then_return_same()
		{
			TestEncodeDecode(@"\\");
		}

		[Test]
		public void Encode_Decode_When_two_backslashes_and_useEscapeChars_Then_returns_same()
		{
			TestEncodeDecode(@"\\", false, true);
		}


		private string GetValueFromOutput()
		{
			string result = String.Empty;

			var filename = "output.log";

			if (File.Exists(filename))
			{
				using (var outfile = new StreamReader(filename))
				{
					result = outfile.ReadToEnd();
				}
			}

			return result;
		}

		private string GenCommandLine(string value, bool hasRegEx = false, bool useEscapeChars = false)
		{
			value = CommandLineUtils.EncodeText(value, hasRegEx, useEscapeChars);

			return String.Format(" {0}{1}--testVal \"{2}\" --someFlag",
								 hasRegEx ? "--hasRegEx " : "",
								 useEscapeChars ? "--useEscapeChars " : "",
								 value
				);
		}

	}
}
