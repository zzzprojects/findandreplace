using FindAndReplace.App.Properties;

namespace FindAndReplace.App
{
    public class FormData
    {
        public bool IsFindOnly { get; set; }
        public string Dir { get; set; }
        public bool IncludeSubDirectories { get; set; }
        public string FileMask { get; set; }
        public string ExcludeFileMask { get; set; }
        public string ExcludeDir { get; set; }
        public string FindText { get; set; }
        public bool IsCaseSensitive { get; set; }
        public bool IsRegEx { get; set; }
        public bool SkipBinaryFileDetection { get; set; }
        public bool ShowEncoding { get; set; }
        public bool IncludeFilesWithoutMatches { get; set; }
        public string ReplaceText { get; set; }
        public bool UseEscapeChars { get; set; }
        public string Encoding { get; set; }
        public bool IsKeepModifiedDate { get; set; }
        public bool IsFirstTime { get; set; }

        public void SaveSetting()
        {
            Settings.Default.Dir = Dir;
            Settings.Default.IncludeSubDirectories = IncludeSubDirectories;
            Settings.Default.FileMask = FileMask;
            Settings.Default.ExcludeFileMask = ExcludeFileMask;
            Settings.Default.ExcludeDir = ExcludeDir;
            Settings.Default.FindText = FindText;
            Settings.Default.IsCaseSensitive = IsCaseSensitive;
            Settings.Default.IsRegEx = IsRegEx;
            Settings.Default.SkipBinaryFileDetection = SkipBinaryFileDetection;
            Settings.Default.IncludeFilesWithoutMatches = IncludeFilesWithoutMatches;
            Settings.Default.ShowEncoding = ShowEncoding;
            Settings.Default.ReplaceText = ReplaceText;
            Settings.Default.UseEscapeChars = UseEscapeChars;
            Settings.Default.Encoding = Encoding;
            Settings.Default.IsKeepModifiedDate = IsKeepModifiedDate;
            Settings.Default.IsFirstTime = IsFirstTime;

            Settings.Default.Save();
        }

        public void LoadSetting()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            Dir = Settings.Default.Dir;
            IncludeSubDirectories = Settings.Default.IncludeSubDirectories;
            FileMask = Settings.Default.FileMask;
            ExcludeFileMask = Settings.Default.ExcludeFileMask;
            ExcludeDir = Settings.Default.ExcludeDir;
            FindText = Settings.Default.FindText;
            IsCaseSensitive = Settings.Default.IsCaseSensitive;
            IsRegEx = Settings.Default.IsRegEx;
            SkipBinaryFileDetection = Settings.Default.SkipBinaryFileDetection;
            IncludeFilesWithoutMatches = Settings.Default.IncludeFilesWithoutMatches;
            ShowEncoding = Settings.Default.ShowEncoding;
            ReplaceText = Settings.Default.ReplaceText;
            UseEscapeChars = Settings.Default.UseEscapeChars;
            Encoding = Settings.Default.Encoding;
            IsKeepModifiedDate = Settings.Default.IsKeepModifiedDate;
            IsFirstTime = Settings.Default.IsFirstTime;
        }
    }
}