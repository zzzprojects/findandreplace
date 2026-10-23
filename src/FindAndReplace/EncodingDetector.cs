using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	//public class StopWatchObject
	//{
	//	public int Milliseconds;

	//	public StopWatchObject(int milliseconds)
	//	{
	//		Milliseconds = milliseconds;
	//	}
	//}

	//public class StopWatch
	//{
	//	public static Dictionary<string, StopWatchObject> Collection { get; } =
	//		new Dictionary<string, StopWatchObject>()
	//		{
	//			{ "FileGetter.Run", new StopWatchObject(100) }
	//		};

	//	public static void Start(string key)
	//	{
	//	}

	//	public static void Stop(string key)
	//	{
	//	}

	//	public static void PrintCollection(object milliseconds)
	//	{
	//	}
	//}
	public class EncodingDetector
	{

		[Flags]
		public enum Options
		{
			KlerkSoftBom = 1,
			KlerkSoftHeuristics = 2,
			MLang = 4
		}

		public static Encoding Detect(byte[] bytes, EncodingDetector.Options opts = Options.KlerkSoftBom | Options.MLang, Encoding defaultEncoding = null)
		{
			Encoding encoding = null;

			if ((opts & Options.KlerkSoftBom) == Options.KlerkSoftBom)
			{
				StopWatch.Start("DetectEncoding: UsingKlerksSoftBom");

				encoding = DetectEncodingUsingKlerksSoftBom(bytes);

				StopWatch.Stop("DetectEncoding: UsingKlerksSoftBom");
			}

			if (encoding != null)
				return encoding;

			if ((opts & Options.KlerkSoftHeuristics) == Options.KlerkSoftHeuristics)
			{
				StopWatch.Start("DetectEncoding: UsingKlerksSoftHeuristics");
				encoding = DetectEncodingUsingKlerksSoftHeuristics(bytes);
				StopWatch.Stop("DetectEncoding: UsingKlerksSoftHeuristics");
			}

			if (encoding != null)
				return encoding;

			if ((opts & Options.MLang) == Options.MLang)
			{
				StopWatch.Start("DetectEncoding: UsingMLang");
				encoding = DetectEncodingUsingMLang(bytes);
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
