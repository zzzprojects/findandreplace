using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public class ReplacerEventArgs : EventArgs
	{
		public Replacer.ReplaceResultItem ResultItem { get; set; }
		public Stats Stats { get; set; }
		public Status Status { get; set; }
		public bool IsSilent { get; set; }

		public ReplacerEventArgs(Replacer.ReplaceResultItem resultItem, Stats stats, Status status, bool isSilent = false)
		{
			ResultItem = resultItem;
			Stats = stats;
			Status = status;
			IsSilent = isSilent;
		}
	}

	public delegate void ReplaceFileProcessedEventHandler(object sender, ReplacerEventArgs e);

	public class Replacer
	{
		public string Dir { get; set; }
		public bool IncludeSubDirectories { get; set; }
		public string FileMask { get; set; }
		public string ExcludeFileMask { get; set; }


		public string FindText { get; set; }
		public bool IsCaseSensitive { get; set; }
		public bool FindTextHasRegEx { get; set; }
		public bool SkipBinaryFileDetection { get; set; }
		public bool IncludeFilesWithoutMatches { get; set; }

		public string ReplaceText { get; set; }

		public bool UseEscapeChars { get; set; }

		public Encoding AlwaysUseEncoding { get; set; }
		public Encoding DefaultEncodingIfNotDetected { get; set; }

		public bool IsCancelRequested { get; set; }
		public bool IsSupressOutput { get; set; }
		public bool IsSilent { get; set; }

		
		public class ReplaceResultItem : ResultItem
		{
			public bool FailedToWrite { get; set; }
		}

		public class ReplaceResult
		{
			public List<ReplaceResultItem> ResultItems { get; set; }

			public Stats Stats { get; set; }
		}

		public ReplaceResult Replace()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");
			Verify.Argument.IsNotNull(ReplaceText, "ReplaceText");

			Status status = Status.Processing;

			var startTime = DateTime.Now;
			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories, ExcludeFileMask);

			var resultItems = new List<ReplaceResultItem>();
			var stats = new Stats();
			stats.Files.Total = filesInDirectory.Length;

			var startTimeProcessingFiles = DateTime.Now;

			foreach (string filePath in filesInDirectory)
			{
				var resultItem = ReplaceTextInFile(filePath);
				stats.Files.Processed++;
				stats.Matches.Found += resultItem.NumMatches;

				if (resultItem.IsSuccess)
				{
					if (resultItem.NumMatches > 0)
					{
						stats.Files.WithMatches++;
						stats.Matches.Replaced += resultItem.NumMatches;
					}
					else
					{
						stats.Files.WithoutMatches++;
					}
				}
				else
				{
					if (resultItem.FailedToOpen)
						stats.Files.FailedToRead++;

					if (resultItem.IsBinaryFile)
						stats.Files.Binary++;

					if (resultItem.FailedToWrite)
						stats.Files.FailedToWrite++;
				}

				if (resultItem.IncludeInResultsList)
					resultItems.Add(resultItem);

				stats.UpdateTime(startTime, startTimeProcessingFiles);

				if (IsCancelRequested)
					status = Status.Cancelled;

				if (stats.Files.Total == stats.Files.Processed)
					status = Status.Completed;

				OnFileProcessed(new ReplacerEventArgs(resultItem, stats, status, IsSilent));

				if (status == Status.Cancelled)
					break;
			}

			if (filesInDirectory.Length == 0)
			{
				status = Status.Completed;
				OnFileProcessed(new ReplacerEventArgs(new ReplaceResultItem(), stats, status, IsSilent));
			}

			return new ReplaceResult {ResultItems = resultItems, Stats = stats};
		}


		private ReplaceResultItem ReplaceTextInFile(string filePath)
		{
			string fileContent = string.Empty;

			var resultItem = new ReplaceResultItem();
			resultItem.IsSuccess = true;
			resultItem.IncludeFilesWithoutMatches = IncludeFilesWithoutMatches; //only used internally

			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

			byte[] sampleBytes;

			//Check if can read first
			try
			{
				sampleBytes = Utils.ReadFileContentSample(filePath);
			}
			catch (Exception exception)
			{
				resultItem.IsSuccess = false;
				resultItem.FailedToOpen = true;
				resultItem.ErrorMessage = exception.Message;
				return resultItem;
			}

			if (!SkipBinaryFileDetection)
			{
				if (resultItem.IsSuccess)
				{
					// check for /0/0/0/0
					if (Utils.IsBinaryFile(sampleBytes))
					{
						resultItem.IsSuccess = false;
						resultItem.IsBinaryFile = true;
						return resultItem;
					}
				}
			}

			if (!resultItem.IsSuccess)
				return resultItem;

			Encoding encoding = DetectEncoding(sampleBytes);
			if (encoding == null)
			{
				resultItem.IsSuccess = false;
				resultItem.FailedToOpen = true;
				resultItem.ErrorMessage = "Could not detect file encoding.";
				return resultItem;
			}

			resultItem.FileEncoding = encoding;

			using (var sr = new StreamReader(filePath, encoding))
			{
				fileContent = sr.ReadToEnd();
			}

			RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);

			var matches = Utils.FindMatches(fileContent, FindText, FindTextHasRegEx, UseEscapeChars, regexOptions);

			resultItem.NumMatches = matches.Count;
			resultItem.Matches = matches;

			if (matches.Count > 0)
			{
				string escapedFindText = FindText;
				if (!FindTextHasRegEx && !UseEscapeChars)
					escapedFindText = Regex.Escape(FindText);

				string newContent = Regex.Replace(fileContent, escapedFindText, UseEscapeChars ? Regex.Unescape(ReplaceText) : ReplaceText, regexOptions);

				try
				{
					using (var sw = new StreamWriter(filePath, false, encoding))
					{
						sw.Write(newContent);
					}
				}
				catch (Exception ex)
				{
					resultItem.IsSuccess = false;
					resultItem.FailedToWrite = true;
					resultItem.ErrorMessage = ex.Message;
				}
			}

			return resultItem;
		}


		private Encoding DetectEncoding(byte[] sampleBytes)
		{
			if (AlwaysUseEncoding != null)
				return AlwaysUseEncoding;

			return EncodingDetector.Detect(sampleBytes, defaultEncoding: DefaultEncodingIfNotDetected);
		}

		public void CancelReplace()
		{
			IsCancelRequested = true;
		}

		public event ReplaceFileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(ReplacerEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}

		public string GenCommandLine(bool showEncoding)
		{
			return CommandLineUtils.GenerateCommandLine(Dir, FileMask, ExcludeFileMask, IncludeSubDirectories, IsCaseSensitive,
														FindTextHasRegEx, SkipBinaryFileDetection, showEncoding,
														IncludeFilesWithoutMatches, UseEscapeChars, AlwaysUseEncoding, FindText,
														ReplaceText);
		}
	}
}
