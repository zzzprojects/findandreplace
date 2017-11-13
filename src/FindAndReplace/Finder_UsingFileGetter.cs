using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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
		public string FindText { get; set; }
		public bool IsCaseSensitive { get; set; }
		public bool FindTextHasRegEx { get; set; }
		public bool SkipBinaryFileDetection { get; set; }
		public bool IncludeFilesWithoutMatches { get; set; }
		public string ExcludeFileMask { get; set; }
		public bool IsCancelRequested { get; set; }
		public bool IsSilent { get; set; }
		public int NumThreads { get; set; }

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
			NumThreads = 1;
		}


		public FindResult Find()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");

			Status status = Status.Processing;

			var startTime = DateTime.Now;
			
            var resultItems = new List<FindResultItem>();
            var stats = new Stats();

            var startTimeProcessingFiles = DateTime.Now;

		    var fileGetter = Utils.CreateFileGetter(Dir, FileMask, IncludeSubDirectories, ExcludeFileMask);
            fileGetter.RunAsync();


            string filePath;
                
            while (true)
            {
                if (fileGetter.FileQueue.IsEmpty && fileGetter.IsFileCountFinal)
                    break;

                bool isDequeued = fileGetter.FileQueue.TryDequeue(out filePath);

                //If nothing was in queue sleep a little, not to waste CPU cycles
                if (!isDequeued)
                {
                    Thread.Sleep(100);
                    continue;
                }

                stats.Files.Total = fileGetter.FileCount;
                stats.Files.IsTotalFinal = fileGetter.IsFileCountFinal;

                stats.Files.Processed++;

                var resultItem = FindInFile(filePath);

                //Update stats
                if (resultItem.IsBinaryFile)
                    stats.Files.Binary++;

                if (resultItem.IsSuccess)
                {
                    stats.Matches.Found += resultItem.Matches.Count;

                    if (resultItem.Matches.Count > 0)
                        stats.Files.WithMatches++;
                    else
                        stats.Files.WithoutMatches++;
                }

                stats.UpdateTime(startTime, startTimeProcessingFiles);

                //Update status
                if (IsCancelRequested)
                {
                    fileGetter.Cancel();
                    status = Status.Cancelled;
                }

                if (resultItem.IncludeInResultsList)
                    resultItems.Add(resultItem);

                //Handle event
                OnFileProcessed(new FinderEventArgs(resultItem, stats, status, IsSilent));

                if (status == Status.Cancelled)
                    break;
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

			Encoding encoding = EncodingDetector.Detect(sampleBytes, defaultEncoding: Encoding.UTF8);
			resultItem.FileEncoding = encoding;
			
			StopWatch.Start("ReadFullFileContent");

			string fileContent;
			using (var sr = new StreamReader(filePath, encoding))
			{
				fileContent = sr.ReadToEnd();
			}

			StopWatch.Stop("ReadFullFileContent");

			StopWatch.Start("FindMatches");
			RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);

			resultItem.Matches = Utils.FindMatches(fileContent, FindText, FindTextHasRegEx, regexOptions);

			StopWatch.Stop("FindMatches");

			resultItem.NumMatches = resultItem.Matches.Count;
			return resultItem;
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

		
	}
}
