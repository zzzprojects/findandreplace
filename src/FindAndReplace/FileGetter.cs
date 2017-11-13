using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FindAndReplace
{
    public class FileGetter
    {
        public bool UseBlockingCollection { get; set; }
        public string DirPath { get; set; }
        public List<string> FileMasks { get; set; }
        public List<string> ExcludeFileMasks { get; set; }
        public List<string> ExcludeFileMasksRegEx { get; set; }

        public SearchOption SearchOption { get; set; }

		public bool IsCancelRequested { get; set; }
        public bool IsCancelled { get; set; }

        public Task _task;

        public BlockingCollection<string> FileCollection = new BlockingCollection<string>();
        public ConcurrentQueue<string> FileQueue = new ConcurrentQueue<string>();

        private int _fileCount;

        public int FileCount
        {
            get { return _fileCount; }
        }

        public bool IsFileCountFinal { get; set; }

        public FileGetter()
        {
            UseBlockingCollection = true;
        }

        public void RunAsync()
        {
            _task = Task.Factory.StartNew(Run);
        }

        private void Run()
        {
            IsCancelled = false;
            _fileCount = 0;

            StopWatch.Start("FileGetter.Run");
                   
            foreach (var fileMask in FileMasks)
            {
                StopWatch.Start("FileGetter.Run Directory.EnumerateFiles");
                          
                var files = Directory.EnumerateFiles(DirPath, fileMask, SearchOption);

                StopWatch.Stop("FileGetter.Run Directory.EnumerateFiles");
                
                foreach (string filePath in files)
                {

                    StopWatch.Start("FileGetter.Run IsMatchWithExcludeFileMasks");
                    bool isMatchWithExcludeFileMasks = IsMatchWithExcludeFileMasks(filePath);
                    StopWatch.Stop("FileGetter.Run IsMatchWithExcludeFileMasks");
           
                    if (!isMatchWithExcludeFileMasks)
                    {
	                    _fileCount++;
                       
                        if (UseBlockingCollection)
                        {
                            //StopWatch.Start("FileGetter.FileCollection.Add");
                            FileCollection.Add(filePath);
                            //StopWatch.Stop("FileGetter.FileCollection.Add");
                        }
                        else
                        {
                            StopWatch.Start("FileGetter.Run FileQueue.Enqueue");
                            FileQueue.Enqueue(filePath);
                            StopWatch.Stop("FileGetter.Run FileQueue.Enqueue");
                        }
                    }

  
	                if (IsCancelRequested)
	                     break;
                }

               if (IsCancelRequested)
					break;
			}


            
            if (IsCancelRequested)
                IsCancelled = true;
            else
                IsFileCountFinal = true;

            if (UseBlockingCollection)
                FileCollection.CompleteAdding();

            StopWatch.Stop("FileGetter.Run");

            StopWatch.PrintCollection(StopWatch.Collection["FileGetter.Run"].Milliseconds);
       }


        public List<string> RunSync()
        {

            //StopWatch.Start("RunSync RunAsync");
           
            RunAsync();

            //StopWatch.Stop("RunSync RunAsync");
           
            var filePathes = new List<string>();

            //StopWatch.Start("RunSync While loop");
                       
            while (true)
            {
                string filePath;

                if (UseBlockingCollection)
                {
                    //StopWatch.Start("FileGetter.RunSync FileCollection.Take");
                       
                    try
                    {
                       
                        filePath = FileCollection.Take();
                    }
                    catch (InvalidOperationException)
                    {
                        if (FileCollection.IsCompleted)
                            break;

                        throw;
                    }

                    //StopWatch.Stop("FileGetter.RunSync FileCollection.Take");

                    filePathes.Add(filePath);
                }
                else
                {
                    bool isDequeued = false;

                    //StopWatch.Start("FileGetter.RunSync FileQueue.TryDequeue");
                   
                    if (FileQueue.TryDequeue(out filePath))
                    {
                        filePathes.Add(filePath);
                        isDequeued = true;
                    }

                    //StopWatch.Stop("FileGetter.RunSync FileQueue.TryDequeue");
                   

                    if (FileQueue.IsEmpty && IsFileCountFinal)
                        break;

                    //If nothing was in queue sleep
                    if (!isDequeued)
                    {
                        //Console.WriteLine("FileGetter.RunSync Nothing to dequeue. Sleeping 100ms.");
                        //StopWatch.Start("FileGetter.FileQueue Sleep 100");
                        Thread.Sleep(100);
                        //StopWatch.Stop("FileGetter.FileQueue Sleep 100");
                    }
                }
            }

            //StopWatch.Stop("RunSync While loop");
           
            return filePathes;
        }


        public List<string> RunSync2()
        {
            RunAsync();

            var filePathes = new List<string>();

            while (!IsFileCountFinal)
            {
                string filePath;
                if (FileQueue.TryDequeue(out filePath))
                {
                    filePathes.Add(filePath);
                }
            }

            return filePathes;
        }


        private bool IsMatchWithExcludeFileMasks(string filePath)
        {
            if (ExcludeFileMasks == null || ExcludeFileMasks.Count == 0)
                return false;

            if (ExcludeFileMasksRegEx == null)
                ExcludeFileMasksRegEx = ExcludeFileMasks.Select(Utils.WildcardToRegex).ToList();


            foreach (string pattern in ExcludeFileMasksRegEx)
            {
                if (Regex.IsMatch(filePath, pattern))
                    return true;
            }

            return false;
        }


        public void Cancel()
        {
            IsCancelRequested = true;
        }
    }
}
