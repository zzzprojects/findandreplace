using System;
using System.Linq;
using System.Text;

using href.Utils;

namespace FindAndReplace
{

	public class EncodingDetector
	{

		[Flags]
		public enum Options
		{
			KlerkSoftBom = 1,
			KlerkSoftHeuristics = 2,
			MLang = 4
		}

		public static Encoding Detect(byte[] bytes, EncodingDetector.Options opts = Options.KlerkSoftBom | Options.MLang | Options.KlerkSoftHeuristics, Encoding defaultEncoding = null, bool collectStopwatchData = true)
		{
			Encoding encoding = null;

			if ((opts & Options.KlerkSoftBom) == Options.KlerkSoftBom)
			{
				if (collectStopwatchData)
					StopWatch.Start("DetectEncoding: UsingKlerksSoftBom");

				encoding = DetectEncodingUsingKlerksSoftBom(bytes);

				if (collectStopwatchData)
					StopWatch.Stop("DetectEncoding: UsingKlerksSoftBom");
			}

			if (encoding != null)
				return encoding;

			if ((opts & Options.KlerkSoftHeuristics) == Options.KlerkSoftHeuristics)
			{
				if (collectStopwatchData)
					StopWatch.Start("DetectEncoding: UsingKlerksSoftHeuristics");
				encoding = DetectEncodingUsingKlerksSoftHeuristics(bytes);
				if (collectStopwatchData)
					StopWatch.Stop("DetectEncoding: UsingKlerksSoftHeuristics");
			}

			if (encoding != null)
				return encoding;

			if ((opts & Options.MLang) == Options.MLang)
			{
				if (collectStopwatchData)
					StopWatch.Start("DetectEncoding: UsingMLang");
				encoding = DetectEncodingUsingMLang(bytes);
				if (collectStopwatchData)
					StopWatch.Stop("DetectEncoding: UsingMLang");
			}

            if (encoding == null)
				encoding = defaultEncoding;

			return encoding;
		}

		private static Encoding DetectEncodingUsingKlerksSoftBom(byte[] bytes)
		{
			Encoding encoding = null;
			if (bytes.Count() >= 4)
				 encoding = KlerksSoftEncodingDetector.DetectBOMBytes(bytes);

			return encoding;
		}


		private static Encoding DetectEncodingUsingKlerksSoftHeuristics(byte[] bytes)
		{
			Encoding encoding = KlerksSoftEncodingDetector.DetectUnicodeInByteSampleByHeuristics(bytes);

			return encoding;
		}

		private static Encoding DetectEncodingUsingMLang(Byte[] bytes)
		{
			try
			{
				Encoding[] detected = EncodingTools.DetectInputCodepages(bytes, 1);
				if (detected.Length > 0)
				{
					return detected[0];
				}
			}
			catch //(COMException ex)
			{
				// return default codepage on error
			}

			return null;
		}

	}
}
