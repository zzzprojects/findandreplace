using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace FindAndReplace
{

	public class FinderEventArgs : EventArgs
	{
		public Finder.FindResultItem ResultItem { get; set; }

		public Stats Stats { get; set; }

		public Status Status { get; set; }

		public bool IsSilent { get; set; }

		public FinderEventArgs(Finder.FindResultItem resultItem, Stats stats, Status status, bool isSilent = false)
		{
			ResultItem = resultItem;
			Stats = stats;
			Status = status;
			IsSilent = isSilent;
		}
	}

	public delegate void FileProcessedEventHandler(object sender, FinderEventArgs e);

	public class Finder
	{
		public string Dir { get; set; }
		public bool IncludeSubDirectories { get; set; }
		public string FileMask { get; set; }
		public string ExcludeFileMask { get; set; }
	    public string ExcludeDir { get; set; }
        public string FindText { get; set; }
		public bool IsCaseSensitive { get; set; }
	    public bool IsKeepModifiedDate { get; set; }
        public bool FindTextHasRegEx { get; set; }
		public bool SkipBinaryFileDetection { get; set; }

		public bool UseEscapeChars { get; set; }

		public Encoding AlwaysUseEncoding { get; set; }
		public Encoding DefaultEncodingIfNotDetected { get; set; }

		public bool IncludeFilesWithoutMatches { get; set; }
		public bool IsSilent { get; set; }

		public bool IsCancelRequested { get; set; }

		public class FindResultItem : ResultItem
		{
		}

		public class FindResult
		{
			public List<FindResultItem> Items { get; set; }
			public Stats Stats { get; set; }

			public List<FindResultItem> ItemsWithMatches
			{
				get { return Items.Where(r => r.NumMatches > 0).ToList(); }
			}

		}


		public Finder()
		{
		}


		public FindResult Find()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");

			Status status = Status.Processing;

			StopWatch.Start("GetFilesInDirectory");

			//time
			var startTime = DateTime.Now;


			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories, ExcludeFileMask, ExcludeDir);

			var resultItems = new List<FindResultItem>();
			var stats = new Stats();
			stats.Files.Total = filesInDirectory.Length;

			StopWatch.Stop("GetFilesInDirectory");


			var startTimeProcessingFiles = DateTime.Now;

			foreach (string filePath in filesInDirectory)
			{

				stats.Files.Processed++;


				var resultItem = FindInFile(filePath);

				if (resultItem.IsSuccess)
				{
					stats.Matches.Found += resultItem.Matches.Count;

					if (resultItem.Matches.Count > 0)
						stats.Files.WithMatches++;
					else
						stats.Files.WithoutMatches++;
				}
				else
				{
					if (resultItem.FailedToOpen)
						stats.Files.FailedToRead++;

					if (resultItem.IsBinaryFile)
						stats.Files.Binary++;
				}


				stats.UpdateTime(startTime, startTimeProcessingFiles);


				//Update status
				if (IsCancelRequested)
					status = Status.Cancelled;

				if (stats.Files.Total == stats.Files.Processed)
					status = Status.Completed;


				//Skip files that don't have matches
				if (resultItem.IncludeInResultsList)
					resultItems.Add(resultItem);

				//Handle event
				OnFileProcessed(new FinderEventArgs(resultItem, stats, status, IsSilent));


				if (status == Status.Cancelled)
					break;
			}



			if (filesInDirectory.Length == 0)
			{
				status = Status.Completed;
				OnFileProcessed(new FinderEventArgs(new FindResultItem(), stats, status, IsSilent));
			}

			return new FindResult {Items = resultItems, Stats = stats};
		}

		private FindResultItem FindInFile(string filePath)
		{
			var resultItem = new FindResultItem();
			resultItem.IsSuccess = true;
			resultItem.IncludeFilesWithoutMatches = IncludeFilesWithoutMatches;

			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

			byte[] sampleBytes;

			StopWatch.Start("ReadSampleFileContent");

			//Check if can read first
			try
			{
				sampleBytes = Utils.ReadFileContentSample(filePath);
			}
			catch (Exception exception)
			{
				StopWatch.Stop("ReadSampleFileContent");

				resultItem.IsSuccess = false;
				resultItem.FailedToOpen = true;
				resultItem.ErrorMessage = exception.Message;
				return resultItem;
			}

			StopWatch.Stop("ReadSampleFileContent");

			Encoding encoding = Encoding.UTF8; 
			RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);
			if (Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
			{
				using (var pdf = PdfDocument.Open(filePath))
				{
					List<LiteMatch> results = new List<LiteMatch>();

					foreach (var page in pdf.GetPages())
					{
						// Rebuild text with \r\n line breaks by detecting Y changes
						var words = page.GetWords()
							.OrderByDescending(w => w.BoundingBox.Bottom)
							.ThenBy(w => w.BoundingBox.Left)
							.ToList();

						var sb = new StringBuilder();
						double? currentY = null;
						bool firstInLine = true;
						const double yTolerance = 2.0; // tweak if needed

						foreach (var w in words)
						{
							var y = w.BoundingBox.Bottom;

							if (currentY == null)
							{
								currentY = y;
								firstInLine = true;
							}
							else if (Math.Abs(y - currentY.Value) > yTolerance)
							{
								sb.Append("\r\n");
								currentY = y;
								firstInLine = true;
							}

							if (!firstInLine) sb.Append(' ');
							sb.Append(w.Text);
							firstInLine = false;
						}

						string text = sb.ToString();
						string[] lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);

						// Compute absolute start index of each line in the rebuilt 'text'
						int[] lineStarts = new int[lines.Length];
						int cursor = 0;
						for (int i = 0; i < lines.Length; i++)
						{
							lineStarts[i] = cursor;
							cursor += lines[i].Length + 2; // +2 for \r\n that Split removed
						}

						for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
						{
							string line = lines[lineIndex];

							if (FindTextHasRegEx)
							{
								foreach (Match m in Regex.Matches(line, FindText, regexOptions))
								{
									int start = lineStarts[lineIndex] + m.Index;

									results.Add(new LiteMatch
									{
										Index = start,
										Length = m.Length,
										LineNumber = lineIndex + 1,
										PreviewText = string.Join("\r\n", new[]
										{
											lineIndex > 1 ? lines[lineIndex - 2] : string.Empty,
											lineIndex > 0 ? lines[lineIndex - 1] : string.Empty,
											line,
											lineIndex < lines.Length - 1 ? lines[lineIndex + 1] : string.Empty,
											lineIndex < lines.Length - 2 ? lines[lineIndex + 2] : string.Empty
										})
									});
								}
							}
							else
							{
								int idx = 0;
								while ((idx = line.IndexOf(FindText, idx, StringComparison.OrdinalIgnoreCase)) >= 0)
								{
									int start = lineStarts[lineIndex] + idx;

									results.Add(new LiteMatch
									{
										Index = start,
										Length = FindText.Length,
										LineNumber = lineIndex + 1,
										PreviewText = string.Join("\r\n", new[]
										{
											lineIndex > 1 ? lines[lineIndex - 2] : string.Empty,
											lineIndex > 0 ? lines[lineIndex - 1] : string.Empty,
											line,
											lineIndex < lines.Length - 1 ? lines[lineIndex + 1] : string.Empty,
											lineIndex < lines.Length - 2 ? lines[lineIndex + 2] : string.Empty
										})
									});

									idx += FindText.Length;
								}
							}
						}
					}

					resultItem.FileEncoding = encoding;
					resultItem.Matches = results;
					resultItem.NumMatches = resultItem.Matches.Count;
				}
			}
			else
			{
				if (!SkipBinaryFileDetection)
				{
					StopWatch.Start("IsBinaryFile");

					if (resultItem.IsSuccess)
					{
						// check for /0/0/0/0
						if (Utils.IsBinaryFile(sampleBytes))
						{
							StopWatch.Stop("IsBinaryFile");

							resultItem.IsSuccess = false;
							resultItem.IsBinaryFile = true;
							return resultItem;
						}
					}

					StopWatch.Stop("IsBinaryFile");
				}

				encoding = DetectEncoding(sampleBytes);
				if (encoding == null)
				{
					resultItem.IsSuccess = false;
					resultItem.FailedToOpen = true;
					resultItem.ErrorMessage = "Could not detect file encoding.";
					return resultItem;
				}

				resultItem.FileEncoding = encoding;

				StopWatch.Start("ReadFullFileContent");

				string fileContent;
				using (var sr = new StreamReader(filePath, encoding))
				{
					fileContent = sr.ReadToEnd();
				}

				StopWatch.Stop("ReadFullFileContent");

				StopWatch.Start("FindMatches");

				resultItem.Matches = Utils.FindMatches(fileContent, FindText, FindTextHasRegEx, UseEscapeChars, regexOptions);

				StopWatch.Stop("FindMatches");

				resultItem.NumMatches = resultItem.Matches.Count;
			}
			return resultItem;
		}

		private Encoding DetectEncoding(byte[] sampleBytes)
		{
			if (AlwaysUseEncoding != null)
				return AlwaysUseEncoding;

			return EncodingDetector.Detect(sampleBytes, defaultEncoding: DefaultEncodingIfNotDetected);
		}

		public void CancelFind()
		{
			IsCancelRequested = true;
		}



		public event FileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(FinderEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}

		public string GenCommandLine(bool showEncoding)
		{
			return CommandLineUtils.GenerateCommandLine(Dir, FileMask, ExcludeFileMask, ExcludeDir, IncludeSubDirectories, IsCaseSensitive,
			                                            FindTextHasRegEx, SkipBinaryFileDetection, showEncoding,
			                                            IncludeFilesWithoutMatches, UseEscapeChars, AlwaysUseEncoding, FindText,
			                                            null, IsKeepModifiedDate);
		}
	}
}
