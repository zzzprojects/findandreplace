using System.Windows.Forms;
using FindAndReplace.App.Properties;
using Microsoft.Win32;

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

        private static readonly string _versionIndependentRegKey;

		static FormData()
		{
			_versionIndependentRegKey = GetVersionIndependentRegKey();
		}

		//Fix for Registry key changing for each new version
		//http://stackoverflow.com/questions/1515943/application-userappdataregistry-and-version-number

		private static string GetVersionIndependentRegKey()
		{
			//Keep using 1.0.0.0 for everything
			string versionDependent = Application.UserAppDataRegistry.Name;
			string versionIndependent = versionDependent.Substring(0, versionDependent.LastIndexOf("\\"));
			versionIndependent = versionIndependent + "\\1.0.0.0";
			return versionIndependent;
		}

		public void SaveSetting()
		{
		    Settings.Default["Dir"] = Dir; 
		    Settings.Default["IncludeSubDirectories"] = IncludeSubDirectories.ToString();
		    Settings.Default["FileMask"] = FileMask;
		    Settings.Default["ExcludeFileMask"] = ExcludeFileMask;
		    Settings.Default["ExcludeDir"] = ExcludeDir;
		    Settings.Default["FindText"] = FindText;
		    Settings.Default["IsCaseSensitive"] = IsCaseSensitive.ToString();
		    Settings.Default["IsRegEx"] = IsRegEx.ToString();
		    Settings.Default["SkipBinaryFileDetection"] = SkipBinaryFileDetection.ToString();
		    Settings.Default["ShowEncoding"] = ShowEncoding.ToString();
		    Settings.Default["IncludeFilesWithoutMatches"] = IncludeFilesWithoutMatches.ToString();
		    Settings.Default["ReplaceText"] = ReplaceText;
		    Settings.Default["UseEscapeChars"] = UseEscapeChars.ToString();
            Settings.Default["Encoding"] = Encoding;
		    Settings.Default["IsKeepModifiedDate"] = IsKeepModifiedDate.ToString();

		    Settings.Default.Save();
		}

		public bool IsEmpty()
		{
			//When saved even once dir will have a non null volue
			string dir = GetValueFromRegistry("Dir");
			return dir == null;
		}

		public void LoadSetting()
		{   
            Dir = Settings.Default["Dir"].ToString();
			IncludeSubDirectories = Settings.Default["IncludeSubDirectories"].ToString() == "True";
			FileMask = Settings.Default["FileMask"].ToString();
			ExcludeFileMask = Settings.Default["ExcludeFileMask"].ToString();
		    ExcludeDir = Settings.Default["ExcludeDir"].ToString();
            FindText = Settings.Default["FindText"].ToString();
			IsCaseSensitive = Settings.Default["IsCaseSensitive"].ToString() == "True";
			IsRegEx = Settings.Default["IsRegEx"].ToString() == "True";
			SkipBinaryFileDetection = Settings.Default["SkipBinaryFileDetection"].ToString() == "True";
			IncludeFilesWithoutMatches = Settings.Default["IncludeFilesWithoutMatches"].ToString() == "True";
			ShowEncoding = Settings.Default["ShowEncoding"].ToString() == "True";
			ReplaceText = Settings.Default["ReplaceText"].ToString();
			UseEscapeChars = Settings.Default["UseEscapeChars"].ToString() == "True";
			Encoding = Settings.Default["Encoding"].ToString();
		    IsKeepModifiedDate= Settings.Default["IsKeepModifiedDate"].ToString() == "True";
        }


		private void SaveValueToRegistry(string name, string value)
		{
			Registry.SetValue(_versionIndependentRegKey, name, value, RegistryValueKind.String);
		}

		private string GetValueFromRegistry(string name)
		{
			var value = Registry.GetValue(_versionIndependentRegKey, name, null);

			if (value != null)
				return value.ToString();

			return null;
		}


	}

}