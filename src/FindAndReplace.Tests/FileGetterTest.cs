using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
    [TestFixture]
    //Class FileGetter is not used in workflows for now, since speed improvement causes
    //resource contention between opening files and iterating
    public class FileGetterTest : TestBase
    {

        [SetUp]
        public override void SetUp()
        {
            CreateTestDir();
            CreateSpeedDir();

            WriteFiles(100000);
        }


        [TearDown]
        public override void TearDown()
        {
            DeleteTestDir();
        }


        private void WriteFiles(int fileSize)
        {
            string fileContent = GetFileContent(fileSize);

            CreateTestFile(fileContent, Encoding.ASCII);
            CreateTestFile(fileContent, Encoding.Unicode);
            CreateTestFile(fileContent, Encoding.BigEndianUnicode);
            CreateTestFile(fileContent, Encoding.UTF32);
            CreateTestFile(fileContent, Encoding.UTF7);
            CreateTestFile(fileContent, Encoding.UTF8);

        } 

        private void CreateTestFile(string fileContent, Encoding encoding)
        {
            string filePath = _speedDir + "\\" + encoding.EncodingName + ".txt";
            File.WriteAllText(filePath, fileContent, encoding);
        }


        protected void CreateTestDir()
        {
           
            _tempDir = Path.GetTempPath() + "FindAndReplaceTests";
            Directory.CreateDirectory(_tempDir);
        }


        protected void DeleteTestDir()
        {
            var tempDir = Path.GetTempPath() + "FindAndReplaceTests";

            Directory.Delete(tempDir, true);
        }


        protected void CreateSpeedDir()
        {
            _speedDir = _tempDir + "\\Speed";

            if (Directory.Exists(_speedDir))
                throw new InvalidOperationException("Dir '" + _speedDir + "' already exists.");

            Directory.CreateDirectory(_speedDir);
        }  

        [Test]
        public void RunAsync_UsingBlockingCollection_TryTake_Works()
        {
            var fileGetter = new FileGetter
                {
                    DirPath = _tempDir,
                    FileMasks = new List<string>{"*.*"},
                    SearchOption = SearchOption.AllDirectories
                };

            fileGetter.RunAsync();


            while (true)
            {
                string filePath;
                bool success = fileGetter.FileCollection.TryTake(out filePath);

                if (success)
                {
                    Console.WriteLine(filePath);
                }
                else
                {
                    if (fileGetter.FileCollection.IsCompleted)
                        break;

                    Console.WriteLine("Blocking Collection empty");
                }
            }
        }


        [Test]
        public void RunAsync_UsingBlockingCollection_Take_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "*.*" },
                SearchOption = SearchOption.AllDirectories
            };

            fileGetter.RunAsync();

            // Consume bc
            while (true)
            {
                string filePath;

                try
                {
                    filePath = fileGetter.FileCollection.Take();
                    Console.WriteLine(filePath);
                }
                catch (InvalidOperationException)
                {
                    if (fileGetter.FileCollection.IsCompleted)
                        break;

                    throw;
                }
            }
        }


        [Test]
        public void RunSync_WhenFileMaskIsTest1_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "Unicode*.*" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(5, files.Count);
        }

        [Test]
        public void Run_WhenFileMaskIsTest1AndTest2_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "Unicode (UTF-7).txt", "US-ASCII.*" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(2, files.Count);
        }


        [Test]
        public void Run_WhenExcludeFileMaskIsTxt_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "*.*" },
                ExcludeFileMasks = new List<string> { "*.txt" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(0, files.Count);
        }

        [Test]
        public void Run_WhenExcludeFileMaskIsTxtAndDll_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "*.*" },
                ExcludeFileMasks = new List<string> { "*.txt", "*.dll" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(0, files.Count);
        }


        [Test]
        public void Speed_CompareAll()
        {
            CompareGetFilesSpeed("*.*");
            CompareGetFilesSpeed("*.*");

            //CompareGetFilesSpeed("*.txt");
            //CompareGetFilesSpeed("*.txt");
        }

        private  string _getFilesSpeedDir = Path.GetTempPath() + "\\FindAndReplaceTests";//"C:\\Temp\\FindAndReplaceTest\\Stable";
        //private const string _getFilesSpeedDir = "C:\\Code\\SpaBooker\\9_4";


        private void CompareGetFilesSpeed(string fileMask)
        {
            TestFileGetterSpeed(fileMask, false);

            TestDirEnumerateFilesSpeed(fileMask);

            TestDirGetFilesSpeed(fileMask);

            TestFileGetterSpeed(fileMask, true);
        }


        public void TestDirEnumerateFilesSpeed(string fileMask)
        {
            var stopWatch = new StopWatch();

            stopWatch.Start();
            var files = Directory.EnumerateFiles(_getFilesSpeedDir, fileMask, SearchOption.AllDirectories).ToList();
            stopWatch.Stop();

            Console.WriteLine("EnumerateFiles  FileMask = " + fileMask + ", Count=" + files.Count() + ", Duration=" + stopWatch.Milliseconds + "ms");
        }

        public void TestDirGetFilesSpeed(string fileMask)
        {
            var stopWatch = new StopWatch();

            stopWatch.Start();
            var files = Directory.GetFiles(_getFilesSpeedDir, fileMask, SearchOption.AllDirectories).ToList();
            stopWatch.Stop();

            Console.WriteLine("GetFiles FileMask = " + fileMask + ", Count=" + files.Count() + ", Duration=" + stopWatch.Milliseconds + "ms");
        }

        public void TestFileGetterSpeed(string fileMask, bool useBlockingCollection = true)
        {
            var stopWatch = new StopWatch();
            stopWatch.Start();

            var fileGetter = new FileGetter
            {
                DirPath = _getFilesSpeedDir,
                FileMasks = new List<string> { fileMask },
                SearchOption = SearchOption.AllDirectories,
                UseBlockingCollection = useBlockingCollection
            };

            var files = fileGetter.RunSync();
            stopWatch.Stop();
            Console.WriteLine("FileGetter.RunSync  FileMask = " + fileMask + ", UseBlockingCollection=" + useBlockingCollection +
                                                        ", Count=" + files.Count() + ", Duration=" + stopWatch.Milliseconds + "ms");

        }


        [Test]
        public void Speed_FileGetter_RunSync_UseBlockingCollection_Check()
        {
            //var stopWatch = new StopWatch();
            //stopWatch.Start();

            TestFileGetterSpeed("*.*");
            //stopWatch.Stop();

            //StopWatch.PrintCollection(stopWatch.Milliseconds);
            //StopWatch.Collection.Clear();
        }


        [Test]
        public void Speed_FileGetter_RunSync_UseConcurrentQueue_Check()
        {
            //var stopWatch = new StopWatch();
            //stopWatch.Start();

            TestFileGetterSpeed("*.*", false);
            //stopWatch.Stop();

            //StopWatch.PrintCollection(stopWatch.Milliseconds);
            //StopWatch.Collection.Clear();
        }


        [Test]
        public void Speed_DirEnumerateFiles_Check()
        {
            TestDirEnumerateFilesSpeed("*.*");
        }

        [Test]
        public void Speed_DirGetFiles_Check()
        {
            TestDirGetFilesSpeed("*.*");
        }
    }
}
