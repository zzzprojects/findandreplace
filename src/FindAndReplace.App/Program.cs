using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CommandLine;



namespace FindAndReplace.App
{
	//from http://www.rootsilver.com/2007/08/how-to-create-a-consolewindow
	internal static class Program
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool AttachConsole(int dwProcessId);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static int Main(string[] args)
		{
			// from http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx
			AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;

			if (args.Length != 0 && args.Contains("--cl"))
			{
				//Get a pointer to the forground window.  The idea here is that
				//IF the user is starting our application from an existing console
				//shell, that shell will be the uppermost window.  We'll get it
				//and attach to it
				IntPtr ptr = GetForegroundWindow();

				int u;

				GetWindowThreadProcessId(ptr, out u);

				Process process = Process.GetProcessById(u);

				if (process.ProcessName == "cmd") //Is the uppermost window a cmd process?
				{
					AttachConsole(process.Id);
				}
				else
				{
					//no console AND we're in console mode ... create a new console.

					AllocConsole();
				}

				var clRunner = new CommandLineRunner();
				int dosErrorLevel = clRunner.Run(args);

				FreeConsole();
				return dosErrorLevel;
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());

				return 0;
			}
		}

		private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
		{
			String dllName = new AssemblyName(args.Name).Name + ".dll";

			var assem = Assembly.GetExecutingAssembly();

			String resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));

			if (resourceName == null) return null; // Not found, maybe another handler will find it

			using (var stream = assem.GetManifestResourceStream(resourceName))
			{
				Byte[] assemblyData = new Byte[stream.Length];

				stream.Read(assemblyData, 0, assemblyData.Length);

				return Assembly.Load(assemblyData);

			}
		}
	}

	public class CommandLineRunner
	{
		public enum DosErrorLevel
		{
			Success = 0,
			FatalError = 1,
			ErrorsInSomeFiles = 2
		};


		private CommandLineOptions _options;

		public int Run(string[] args)
		{
			DosErrorLevel dosErrorLevel;

			_options = new CommandLineOptions();
			if (!CommandLine.Parser.Default.ParseArguments(args, _options))
			{
				Console.ReadKey();
				Environment.Exit(1);
			}

			var validationResultList = new List<ValidationResult>();

			validationResultList.Add(ValidationUtils.IsDirValid(_options.Dir, "dir"));
			validationResultList.Add(ValidationUtils.IsNotEmpty(_options.FileMask, "fileMask"));
			validationResultList.Add(ValidationUtils.IsNotEmpty(_options.FindText, "find"));
			validationResultList.Add(ValidationUtils.IsNotEmpty(_options.FindText, "find"));

			if (_options.IsFindTextHasRegEx)
				validationResultList.Add(ValidationUtils.IsValidRegExp(_options.FindText, "find"));

			if (!(String.IsNullOrEmpty(_options.AlwaysUseEncoding)))
				validationResultList.Add(ValidationUtils.IsValidEncoding(_options.AlwaysUseEncoding, "alwaysUseEncoding"));

			if (!(String.IsNullOrEmpty(_options.DefaultEncodingIfNotDetected)))
				validationResultList.Add(ValidationUtils.IsValidEncoding(_options.DefaultEncodingIfNotDetected, "alwaysUseEncoding"));

			if (_options.UseEscapeChars)
			{
				validationResultList.Add(ValidationUtils.IsValidEscapeSequence(_options.FindText, "find"));
				
				if (!String.IsNullOrEmpty(_options.ReplaceText))
					validationResultList.Add(ValidationUtils.IsValidEscapeSequence(_options.ReplaceText, "replace"));
			}


			if (!String.IsNullOrEmpty(_options.LogFile))
			{
				var fs1 = new FileStream(_options.LogFile, FileMode.Create);
				var sw1 = new StreamWriter(fs1);
				Console.SetOut(sw1);
			}

			Console.WriteLine("");

			if (validationResultList.Any(vr => !vr.IsSuccess))
			{
				foreach (var validationResult in validationResultList)
				{
					if (!validationResult.IsSuccess)
						Console.WriteLine(String.Format("{0}: {1}", validationResult.FieldName, validationResult.ErrorMessage));
				}

				Console.WriteLine("");

				dosErrorLevel = DosErrorLevel.FatalError;
			}
			else
			{
				dosErrorLevel = DosErrorLevel.Success;

				bool hasRegEx = _options.IsFindTextHasRegEx;

				if (_options.ReplaceText == null)
				{
					var finder = new Finder();
					finder.Dir = _options.Dir;
					finder.IncludeSubDirectories = _options.IncludeSubDirectories;
					finder.FileMask = _options.FileMask;
					finder.ExcludeFileMask = _options.ExcludeFileMask;
				    finder.ExcludeDir = _options.ExcludeDir;

                    finder.FindText = CommandLineUtils.DecodeText(_options.FindText, false, hasRegEx, _options.UseEscapeChars);
					finder.IsCaseSensitive = _options.IsCaseSensitive;
					finder.FindTextHasRegEx = hasRegEx;
					finder.SkipBinaryFileDetection = _options.SkipBinaryFileDetection;
					finder.IncludeFilesWithoutMatches = _options.IncludeFilesWithoutMatches;

					finder.AlwaysUseEncoding = Utils.GetEncodingByName(_options.AlwaysUseEncoding);
					finder.DefaultEncodingIfNotDetected = Utils.GetEncodingByName(_options.DefaultEncodingIfNotDetected);
					finder.UseEscapeChars = _options.UseEscapeChars;

					finder.IsSilent = _options.Silent;

					finder.FileProcessed += OnFinderFileProcessed;

					var findResult = finder.Find();


					if (_options.SetErrorLevelIfAnyFileErrors)
						if (findResult.Stats.Files.FailedToRead > 0)
							dosErrorLevel = DosErrorLevel.ErrorsInSomeFiles;
				}
				else
				{
					var replacer = new Replacer();
					replacer.Dir = _options.Dir;

					replacer.IncludeSubDirectories = _options.IncludeSubDirectories;

					replacer.FileMask = _options.FileMask;
					replacer.ExcludeFileMask = _options.ExcludeFileMask;
				    replacer.ExcludeDir = _options.ExcludeDir;

                    replacer.FindText = CommandLineUtils.DecodeText(_options.FindText, false, hasRegEx, _options.UseEscapeChars);
					replacer.IsCaseSensitive = _options.IsCaseSensitive;
					replacer.FindTextHasRegEx = _options.IsFindTextHasRegEx;
					replacer.SkipBinaryFileDetection = _options.SkipBinaryFileDetection;
					replacer.IncludeFilesWithoutMatches = _options.IncludeFilesWithoutMatches;

					replacer.ReplaceText = CommandLineUtils.DecodeText(_options.ReplaceText, true, _options.IsFindTextHasRegEx, _options.UseEscapeChars);

					replacer.AlwaysUseEncoding = Utils.GetEncodingByName(_options.AlwaysUseEncoding);
					replacer.DefaultEncodingIfNotDetected = Utils.GetEncodingByName(_options.DefaultEncodingIfNotDetected);
					replacer.UseEscapeChars = _options.UseEscapeChars;

					replacer.IsSilent = _options.Silent;

					replacer.FileProcessed += OnReplacerFileProcessed;

					var replaceResult = replacer.Replace();

					if (_options.SetErrorLevelIfAnyFileErrors)
						if (replaceResult.Stats.Files.FailedToRead > 0 || replaceResult.Stats.Files.FailedToWrite > 0)
							dosErrorLevel = DosErrorLevel.ErrorsInSomeFiles;
				}
			}

			if (!String.IsNullOrEmpty(_options.LogFile))
			{
				Console.Out.Close();
			}

#if (DEBUG)
				Console.ReadLine();
			#endif

			return (int) dosErrorLevel;
		}

		private void OnFinderFileProcessed(object sender, FinderEventArgs e)
		{
			if (e.ResultItem.IncludeInResultsList && !e.IsSilent)
				PrintFinderResultRow(e.ResultItem, e.Stats);

			if (e.Stats.Files.Processed == e.Stats.Files.Total && !e.IsSilent)
				PrintStatistics(e.Stats);

		}

		private void OnReplacerFileProcessed(object sender, ReplacerEventArgs e)
		{
			if (e.ResultItem.IncludeInResultsList && !e.IsSilent)
				PrintReplacerResultRow(e.ResultItem, e.Stats);

			if (e.Stats.Files.Processed == e.Stats.Files.Total && !e.IsSilent)
				PrintStatistics(e.Stats, true);
		}


		public void PrintFinderResultRow(Finder.FindResultItem item, Stats stats)
		{
			PrintFileAndEncoding(item);

			if (item.IsSuccess)
				PrintNameValuePair("Matches", item.NumMatches.ToString());
			else
				PrintNameValuePair("Error", item.ErrorMessage);

			Console.WriteLine();
		}

		private void PrintFileAndEncoding(ResultItem item)
		{
			PrintNameValuePair("File", item.FileRelativePath);

			if (_options.ShowEncoding && item.FileEncoding != null)
				PrintNameValuePair("Encoding", item.FileEncoding.WebName);
		}

		public void PrintReplacerResultRow(Replacer.ReplaceResultItem item, Stats stats)
		{
			PrintFileAndEncoding(item);

			if (!item.FailedToOpen)
				PrintNameValuePair("Matches", item.NumMatches.ToString());

			PrintNameValuePair("Replaced", item.IsReplaced ? "Yes" : "No");

			if (!item.IsSuccess)
				PrintNameValuePair("Error", item.ErrorMessage);

			Console.WriteLine();
		}

		private static void PrintNameValuePair(string name, string value)
		{
			string label = name + ":";
			label = label.PadRight(10);
			Console.WriteLine(label + value);
		}

		public void PrintStatistics(Stats stats, bool isReplacerStats = false)
		{
			Console.WriteLine("");

			Console.WriteLine("====================================");
			Console.WriteLine("Stats");
			Console.WriteLine("");
			Console.WriteLine("Files:");
			Console.WriteLine("- Total: " + stats.Files.Total);
			Console.WriteLine("- Binary: " + stats.Files.Binary + " (skipped)");
			Console.WriteLine("- With Matches: " + stats.Files.WithMatches);
			Console.WriteLine("- Without Matches: " + stats.Files.WithoutMatches);
			Console.WriteLine("- Failed to Open: " + stats.Files.FailedToRead);

			if (isReplacerStats)
				Console.WriteLine("- Failed to Write: " + stats.Files.FailedToWrite);

			Console.WriteLine("");
			Console.WriteLine("Matches:");
			Console.WriteLine("- Found: " + stats.Matches.Found);

			if (isReplacerStats)
				Console.WriteLine("- Replaced: " + stats.Matches.Replaced);

			Console.WriteLine("");
			double secs = Math.Round(stats.Time.Passed.TotalSeconds, 3);
			Console.WriteLine("Duration: " + secs.ToString() + " secs");

			Console.WriteLine("====================================");
		}

	}
}
