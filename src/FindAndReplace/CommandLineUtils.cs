using System;
using System.Text;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	//Various command line issues are described here
	//http://weblogs.asp.net/jgalloway/archive/2006/10/05/_5B002E00_NET-Gotcha_5D00_-Commandline-args-ending-in-_5C002200_-are-subject-to-CommandLineToArgvW-whackiness.aspx
	//Rules are: Backslash is the escape character; always escape quotes; only escape backslashes if they precede a quote.

	public static class CommandLineUtils
	{
		private const string EscapedSlashN = "[SlashN]"; 

		public static string EncodeText(string original, bool hasRegEx = false, bool useEscapeChars = false)
		{
			var result = original;

			//new line
			if (!hasRegEx && !useEscapeChars)
				result = TryEncodeNewLine(result);

			result = TryEncodeSlashesFollowedByQuotes(result);

			result = TryEncodeQuotes(result);
			
			result = TryEncodeLastSlash(result);

			return result;
		}

		private static string TryEncodeNewLine(string original)
		{
			var result = original.Replace("\\n", EscapedSlashN);
			
			result = result.Replace(Environment.NewLine, "\\n");
			return result;
		}
		
		private static string TryEncodeSlashesFollowedByQuotes(string original)
		{
			var regexPattern = @"\\+""";

			string result = Regex.Replace(original, regexPattern, 
				delegate(Match match)
				{
					string matchText = match.ToString();
					string justSlashes = matchText.Remove(matchText.Length - 1);
					return justSlashes + justSlashes + "\"";  //double up the slashes
				});
				
			return result;
		}

		private static string TryEncodeQuotes(string original)
		{
			var result = original.Replace("\"", "\"\"");
			return result;
		}

		private static string TryEncodeLastSlash(string original)
		{
			var regexPattern = @"\\+$";

			string result = Regex.Replace(original, regexPattern,
				delegate(Match match)
				{
					string matchText = match.ToString();
					return matchText + matchText;  //double up the slashes
				});

			return result;
		}

		public static string DecodeText(string original, bool isReplace, bool hasRegEx = false, bool useEscapeChars = false)
		{
			string decoded = original;

			//See case https://findandreplace.codeplex.com/workitem/17
			if (!hasRegEx && !useEscapeChars)
				decoded = TryDecodeNewLine(decoded);

			return decoded;
		}


		private static string TryDecodeNewLine(string original)
		{
			var result = original.Replace("\\n", Environment.NewLine);

			result = result.Replace(EscapedSlashN, "\\n");

			return result;
		}


		public static string GenerateCommandLine(
												string dir,
												string fileMask,
												string excludeFileMask,
                                                string excludeDir,
                                                bool includeSubDirectories,
												bool isCaseSensitive,
												bool isRegEx,
												bool skipBinaryFileDetection,
												bool showEncoding,
												bool includeFilesWithoutMatches,
												bool useEscapeChars,
												Encoding encoding,
												string find,
												string replace,
												bool IsKeepModifiedDate)
		{
			return
				String.Format(
                    "--cl --dir \"{0}\" --fileMask \"{1}\"{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12} --find \"{13}\" {14}",
					dir.TrimEnd('\\'),
					fileMask,
					String.IsNullOrEmpty(excludeFileMask)
						? ""
						: String.Format(" --excludeFileMask \"{0}\"", CommandLineUtils.EncodeText(excludeFileMask)),
                    String.IsNullOrEmpty(excludeDir)
					    ? ""
					    : String.Format(" --ExcludeDir \"{0}\"", CommandLineUtils.EncodeText(excludeDir)),
                    includeSubDirectories ? " --includeSubDirectories" : "",
					isCaseSensitive ? " --caseSensitive" : "",
					isRegEx ? " --useRegEx" : "",
                    IsKeepModifiedDate ? " --KeepModifiedDate" : "", 
                    skipBinaryFileDetection ? " --skipBinaryFileDetection" : "",
					showEncoding ? " --showEncoding" : "",
					includeFilesWithoutMatches ? " --includeFilesWithoutMatches" : "",
					useEscapeChars ? " --useEscapeChars" : "",
					(encoding != null) ? String.Format(" --alwaysUseEncoding \"{0}\"", encoding.HeaderName) : "",
					CommandLineUtils.EncodeText(find, isRegEx, useEscapeChars),
					(replace != null) ? String.Format("--replace \"{0}\"", CommandLineUtils.EncodeText(replace, false, useEscapeChars)) : ""
				);
		}

		//windows arg can't end with odd count of '\'
		//args can ends with even count of '\'
		//if arg ends with odd count of '\' we add one more to the end
		public static bool IsValidCommandLineArg(string text)
		{
			

			return true;
		}

		public static string EscapeBackSlashes(string text)
		{
			var regexPattern = "\\\\";

			var result = text;

			var regex = new Regex(regexPattern);

			var matches = regex.Matches(text);

			for (int i = matches.Count - 1; i>= 0; i--)
			{
				var match = matches[i];
				
				var index = match.Index + match.Length;

				if (index >= text.Length || text[index] == '\\')
					result = result.Insert(match.Index, "\\");
			}

			return result;
		}
	}
}


