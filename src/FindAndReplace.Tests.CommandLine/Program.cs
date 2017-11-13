using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using FindAndReplace;

namespace FindAndReplace.Tests.CommandLine
{
	class Program
	{
		static void Main(string[] args)
		{
			var options = new CommandLineOptions();

			string result;
			if (Parser.Default.ParseArguments(args, options))
			{
				if (options.SkipDecoding)
					result = options.TestValue;
				else
					result = CommandLineUtils.DecodeText(options.TestValue, false, options.HasRegEx, options.UseEscapeChars);
			}
			else
			{
				result = "Errors in ParseArguments: " + Environment.NewLine;
				if (options.LastParserState.Errors.Count > 0)
				{
					foreach (var error in options.LastParserState.Errors)
						result += error.BadOption.ShortName + ": " + error.ToString() + Environment.NewLine;
				}	
			}

			using (var outfile = new StreamWriter("output.log"))
			{
				outfile.Write(result);
			} 
		}
	}
}
