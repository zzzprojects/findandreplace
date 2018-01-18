using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FindAndReplace.App
{
	public class ValidationResult
	{
		public bool IsSuccess { get; set; }

		public string ErrorMessage { get; set; }

		public string FieldName { get; set; }
	}

	public static class ValidationUtils
	{
		public static ValidationResult IsDirValid(string dir, string itemName)
		{
			var result = new ValidationResult() {IsSuccess = true, FieldName = itemName};

			if (dir.Trim() == "")
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Dir is required";
				return result;
			}

		Regex dirRegex  = new Regex(@"^(([a-zA-Z]:)|(\\{2}[^\/\\:*?<>|]+)) (\\([^\/\\:*?<>|]*))*(\\)?$");
		    Regex dirRegexNotRooted = new Regex(@"(\\([^\/\\:*?<>|]*))*(\\)?$");
		    if (Path.IsPathRooted(dir))
		    {
			if (!dirRegex.IsMatch(dir))
			{
			    result.IsSuccess = false;
			    result.ErrorMessage = "Dir is invalid";
			    return result;
			}
		    }
		    else
		    {
			if (!dirRegexNotRooted.IsMatch(dir))
			{
			    result.IsSuccess = false;
			    result.ErrorMessage = "Dir is invalid";
			    return result;
			}
		    }

			if (!Directory.Exists(dir))
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Dir does not exist";
				return result;
			}

			return result;
		}

		public static ValidationResult IsNotEmpty(string text, string itemName)
		{
			var result = new ValidationResult() {IsSuccess = true, FieldName = itemName};

			if (text.Trim() == "")
			{
				result.IsSuccess = false;
				result.ErrorMessage = String.Format("{0} is required", itemName);
				return result;
			}

			return result;
		}

		public static ValidationResult IsValidRegExp(string text, string itemName)
		{
			var result = new ValidationResult() {IsSuccess = true, FieldName = itemName};

			try
			{
				Regex.Match("", text);
			}
			catch (ArgumentException)
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Invalid regular expression";
			}

			return result;
		}

		public static ValidationResult IsValidEncoding(string encodingName, string itemName)
		{
			var result = new ValidationResult() {IsSuccess = true, FieldName = itemName};

			try
			{
				Encoding.GetEncoding(encodingName);

			}
			catch (ArgumentException)
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Invalid encoding name";
			}

			return result;
		}

		public static ValidationResult IsValidEscapeSequence(string text, string itemName)
		{
			var result = new ValidationResult() { IsSuccess = true, FieldName = itemName };

			try
			{
				Regex.Unescape(text);
			}
			catch (ArgumentException e)
			{
				string msgPart = "Unrecognized escape sequence";
				int pos = e.Message.IndexOf(msgPart);
				if (pos == -1)
					throw;

				result.IsSuccess = false;
				result.ErrorMessage = e.Message.Substring(pos);
			}

			if (result.IsSuccess)
			{
				if (EndsWithSingleSlash(text))
				{
					result.IsSuccess = false;
					result.ErrorMessage = @"Illegal \ at end of pattern";
				}
			}

			return result;
		}

		private static bool EndsWithSingleSlash(string text)
		{
			var regexPattern = @"\\+$";

			var regex = new Regex(regexPattern);

			var matches = regex.Matches(text);

			if (matches.Count > 0)
			{
				var match = matches[0];

				if ((match.Length % 2) != 0)
					return true;
			}

			return false;
		}
	}
}
