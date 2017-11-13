using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace FindAndReplace.Tests
{

	//Various command line issues are described here
	//http://weblogs.asp.net/jgalloway/archive/2006/10/05/_5B002E00_NET-Gotcha_5D00_-Commandline-args-ending-in-_5C002200_-are-subject-to-CommandLineToArgvW-whackiness.aspx
	
	[TestFixture]
	public class CommandLineLearningTests
	{
		private string _applicationExePath = @"FindAndReplace.Tests.CommandLine.exe";

		[Test]
		public void When_pass_unescaped_slash_Then_get_quote_and_following_flag()
		{
			string input = @"\";
			string expectedOutput =@""" --someFlag";

			TestCmdArg(input, expectedOutput);
		}

		[Test]
		public void When_pass_escaped_slash_Then_get_single_slash()
		{
			string input = @"\\";
			string expectedOutput = @"\";

			TestCmdArg(input, expectedOutput);
		}


		[Test]
		public void When_pass_escaped_slash_and_text_Then_get_single_slash()
		{
			string input = @"\\ Test \\";
			string expectedOutput = @"\\ Test \";

			TestCmdArg(input, expectedOutput);
		}


		[Test]
		public void When_pass_three_slashes_Then_get_slash_and_quote_and_following_flag()
		{
			string input = @"\\\";
			string expectedOutput = @"\"" --someFlag";

			TestCmdArg(input, expectedOutput);
		}



		[Test]
		public void When_pass_unescaped_quote_Then_get_quote_and_following_flag()
		{
			string input = "\"";
			string expectedOutput = "\" --someFlag";

			TestCmdArg(input, expectedOutput);
		}


		[Test]
		public void When_pass_unecaped_quoted_string_Then_get_partial_back()
		{
			string input = @"Shakespere said: ""To be or not to be"" long time ago.";
			string expectedOutput = @"Shakespere said: To";
			
			TestCmdArg(input, expectedOutput);
		}

		[Test]
		public void When_pass_quoted_string_and_quotes_escaped_with_quotes_Then_get_same()
		{
			string input = @"Shakespere said: """"To be or not to be"""" long time ago.";
			string expectedOutput =@"Shakespere said: ""To be or not to be"" long time ago.";

			TestCmdArg(input, expectedOutput);
		}


		[Test]
		public void When_pass_quoted_string_and_quotes_escaped_with_slashes_Then_get_same()
		{
			string input = @"Shakespere said: \""To be or not to be\"" long time ago.";
			string expectedOutput = @"Shakespere said: ""To be or not to be"" long time ago.";

			TestCmdArg(input, expectedOutput);
		}


		[Test]
		public void When_pass_various_escapable_chars_Then_get_same()   //from http://www.robvanderwoude.com/escapechars.php
		{
			string input = @"%^&<>|'`,;=()![]";
			string expectedOutput = input;

			TestCmdArg(input, expectedOutput);
		}

		private void TestCmdArg(string input, string expectedOutput)
		{
			var cmdText = GenCommandLine(input);

			System.Diagnostics.Process.Start(_applicationExePath, cmdText);

			//wait for FindAndReplace.Tests.CommandLine.exe finish
			Thread.Sleep(1000);

			var outputValue = GetValueFromOutput();

			Assert.AreEqual(expectedOutput, outputValue);
		}

		private string GenCommandLine(string value)
		{
			return String.Format("--skipDecoding --testVal \"{0}\" --someFlag", value);
		}


		private string GetValueFromOutput()
		{
			string result = String.Empty;

			var filename = "output.log";

			if (File.Exists(filename))
			{
				using (var outfile = new StreamReader(filename))
				{
					result = outfile.ReadLine();
				}
			}

			return result;
		}
	}
}
