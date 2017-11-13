using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	public class MatchPreviewLineNumber
	{
		public int LineNumber { get; set; }

		public bool HasMatch { get; set; }
	}

	public class LineNumberComparer : IEqualityComparer<MatchPreviewLineNumber>
	{
		public bool Equals(MatchPreviewLineNumber x, MatchPreviewLineNumber y)
		{
			return x.LineNumber == y.LineNumber;
		}

		public int GetHashCode(MatchPreviewLineNumber obj)
		{
			return obj.LineNumber.GetHashCode();
		}
	}
}
