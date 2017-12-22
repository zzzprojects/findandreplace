using System.Windows.Forms;
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

		public void SaveToRegistry()
		{
			SaveValueToRegistry("Dir", Dir);
			SaveValueToRegistry("IncludeSubDirectories", IncludeSubDirectories.ToString());
			SaveValueToRegistry("FileMask", FileMask);
			SaveValueToRegistry("ExcludeFileMask", ExcludeFileMask);
		    SaveValueToRegistry("ExcludeDir", ExcludeDir);
            SaveValueToRegistry("FindText", FindText);
			SaveValueToRegistry("IsCaseSensitive", IsCaseSensitive.ToString());
			SaveValueToRegistry("IsRegEx", IsRegEx.ToString());
			SaveValueToRegistry("SkipBinaryFileDetection", SkipBinaryFileDetection.ToString());
			SaveValueToRegistry("ShowEncoding", ShowEncoding.ToString());
			SaveValueToRegistry("IncludeFilesWithoutMatches", IncludeFilesWithoutMatches.ToString());
			SaveValueToRegistry("ReplaceText", ReplaceText);
			SaveValueToRegistry("UseEscapeChars", UseEscapeChars.ToString());
			SaveValueToRegistry("Encoding", Encoding);
		}

		public bool IsEmpty()
		{
			//When saved even once dir will have a non null volue
			string dir = GetValueFromRegistry("Dir");
			return dir == null;
		}

		public void LoadFromRegistry()
		{
			Dir = GetValueFromRegistry("Dir");
			IncludeSubDirectories = GetValueFromRegistry("IncludeSubDirectories") == "True";
			FileMask = GetValueFromRegistry("Filemask");
			ExcludeFileMask = GetValueFromRegistry("ExcludeFileMask");
		    ExcludeDir = GetValueFromRegistry("ExcludeDir");
            FindText = GetValueFromRegistry("FindText");
			IsCaseSensitive = GetValueFromRegistry("IsCaseSensitive") == "True";
			IsRegEx = GetValueFromRegistry("IsRegEx") == "True";
			SkipBinaryFileDetection = GetValueFromRegistry("SkipBinaryFileDetection") == "True";
			IncludeFilesWithoutMatches = GetValueFromRegistry("IncludeFilesWithoutMatches") == "True";
			ShowEncoding = GetValueFromRegistry("ShowEncoding") == "True";
			ReplaceText = GetValueFromRegistry("ReplaceText");
			UseEscapeChars = GetValueFromRegistry("UseEscapeChars") == "True";
			Encoding = GetValueFromRegistry("Encoding");
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