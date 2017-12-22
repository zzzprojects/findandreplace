using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace FindAndReplace.App
{
	public class CommandLineOptions
	{
		#region Standard Option Attribute

		[ParserState]
		public IParserState LastParserState { get; set; }

		[Option("cl", HelpText = "Required to run on command line.")]
		public bool UseCommandLine { get; set; }

		[Option("find", Required = true, HelpText = "Text to find.")]
		public string FindText { get; set; }

		[Option("replace", HelpText = "Replacement text.")]
		public string ReplaceText { get; set; }

		[Option("caseSensitive", HelpText = "Case-sensitive.")]
		public bool IsCaseSensitive { get; set; }

		[Option("useRegEx", HelpText = "Find text has Regular Expression.")]
		public bool IsFindTextHasRegEx { get; set; }

		[Option("dir", Required = true, HelpText = "Directory path.")]
		public string Dir { get; set; }

		[Option("includeSubDirectories", HelpText = "Include files in SubDirectories.")]
		public bool IncludeSubDirectories { get; set; }

		[Option("fileMask", Required = true, HelpText = "File mask.")]
		public string FileMask { get; set; }

		[Option("excludeFileMask", HelpText = "Exclude file mask.")]
		public string ExcludeFileMask { get; set; }

	    [Option("excludeDir", HelpText = "Exclude file mask.")]
	    public string ExcludeDir { get; set; }

        [Option("skipBinaryFileDetection", HelpText = "Ignore detection of binary files.")]
		public bool SkipBinaryFileDetection { get; set; }

		[Option("showEncoding", HelpText = "Display detected encoding information for each fle.")]
		public bool ShowEncoding { get; set; }

		[Option("alwaysUseEncoding", HelpText = "Skip encoding detection and always use specified encoding.")]
		public string AlwaysUseEncoding { get; set; }

		[Option("defaultEncodingIfNotDetected",
			HelpText = "If encoding is not detected in some very rare cases, use this one.")]
		public string DefaultEncodingIfNotDetected { get; set; }

		[Option("includeFilesWithoutMatches", HelpText = "Include files without matches in results.")]
		public bool IncludeFilesWithoutMatches { get; set; }

		[Option("setErrorLevelIfAnyFileErrors", HelpText = "Return ErrorLevel 2 if any files have read/write errors.")]
		public bool SetErrorLevelIfAnyFileErrors { get; set; }

		[Option("silent", HelpText = "Supress the command window output.")]
		public bool Silent { get; set; }

		[Option("logFile", HelpText = "Path to log file where to save command output.")]
		public string LogFile { get; set; }

		[Option("useEscapeChars", HelpText = "Escape special chars.")]
		public bool UseEscapeChars { get; set; }


		#endregion

		#region Specialized Option Attribute

		[HelpOption("help", HelpText = "Display this help screen.")]
		public string GetUsage()
		{
			var help = new HelpText("Find And Replace");

			help.Copyright = new CopyrightInfo("ENTech Solutions", DateTime.Now.Year);

			if (this.LastParserState != null && this.LastParserState.Errors.Count > 0)
			{
				HandleParsingErrorsInHelp(help);
			}
			else
			{
				help.AddPreOptionsLine(
					"Usage: \n\nfnr.exe --cl --find \"Text To Find\" --replace \"Text To Replace\"  --caseSensitive  --dir \"Directory Path\" --fileMask \"*.*\"  --includeSubDirectories --useRegEx");
				help.AddPreOptionsLine("\n");
				help.AddPreOptionsLine("Mask new line and quote characters using \\n and \\\".");

				help.AddOptions(this);
			}

			return help;
		}


		private void HandleParsingErrorsInHelp(HelpText help)
		{
			var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
			if (!string.IsNullOrEmpty(errors))
			{
				help.MaximumDisplayWidth = 160;
				help.AddPreOptionsLine(string.Concat("\n", "ERROR(S):"));
				help.AddPreOptionsLine(errors);
				help.AddPreOptionsLine("Use 'fnr.exe --cl --help' to see help for this command.");
			}
		}

		#endregion
	}
}
