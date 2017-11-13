using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public abstract class TestBase
	{
		protected  const string Dir_StyleSalt = "D:\\Temp\\FindAndReplaceTest\\StyleSalt";
		protected string _tempDir;
		protected string _speedDir;

		//1000 chars
		const string Chars1000 =
				@"The GNU General Public License is a free, copyleft license for
software and other kinds of works.

  The licenses for most software and other practical works are designed
to take away your freedom to share and change the works.  By contrast,
the GNU General Public License is intended to guarantee your freedom to
share and change all versions of a program--to make sure it remains free
software for all its users.  We, the Free Software Foundation, use the
GNU General Public License for most of our software; it applies also to
any other work released this way by its authors.  You can apply it to
your programs, too.

  When we speak of free software, we are referring to freedom, not
price.  Our General Public Licenses are designed to make sure that you
have the freedom to distribute copies of free software (and charge for
them if you wish), that you receive source code or can get it if you
want it, that you can change the software or use pieces of it in new
free program...";


		[SetUp] 
		public virtual void SetUp()
		{
			CreateTestDir();

			FileStream fs = new FileStream(_tempDir + "\\test1.txt", FileMode.Create);
			StreamWriter sr = new StreamWriter(fs);

			sr.Write("The licenses for most software and other practical");
			sr.Write(" works are designed to take away your freedom to share");
			sr.Write(" and change the works. By contrast, the GNU General Public");
			sr.Write(" License is intended to guarantee your freedom to share and ");
			sr.Write("change all versions of a program--to make sure it remains free");
			sr.Write(" software for all its users. We, the Free Software Foundation, use");
			sr.Write(" the GNU General Public License for most of our software; it applies");
			sr.Write(" also to any other work released this way by its authors. You can apply it to your programs, too.");

			sr.Close();
			fs.Close();

			fs = new FileStream(_tempDir + "\\test2.txt", FileMode.Create);
			sr = new StreamWriter(fs);

			sr.WriteLine("1234567890");
			sr.WriteLine("abcdefghijk");
			sr.WriteLine("aabbccddee");
			sr.WriteLine("mail@mail.com");

			sr.Close();
			fs.Close();

			fs = new FileStream(_tempDir + "\\test3.txt", FileMode.Create);
			sr = new StreamWriter(fs);

			sr.WriteLine("This is a readonly file");

			sr.Close();
			fs.Close();

			File.SetAttributes(_tempDir + "\\test3.txt", FileAttributes.ReadOnly);

			fs = new FileStream(_tempDir + "\\test4.txt", FileMode.Create);
			sr = new StreamWriter(fs);

			sr.WriteLine(
				"2012-12-17T00:05:28+00:00 info: Thank you for contacting Norton One Member Support, your support agent will be with you within the next 2 minutes.  </b><br><br>    If you need help to download and install a Norton product, we have an online tutorial with step by step instructions available from <a href=\" http://one.norton.com/support\" target=\"_blank\"> one.norton.com/support </a></b><br> Please feel free to check it out at your convenience.");
			sr.WriteLine("2012-12-17T00:05:32+00:00 info: You are now chatting with Jayakaran Y E.");
			sr.WriteLine("2012-12-17T00:05:46+00:00 Jayakaran Y E: <span dir=\"ltr\">Hi Mary, Welcome to Norton One Support, My name is Jayakaran, How can I help you today ? </span>");
			sr.WriteLine("2012-12-17T00:05:59+00:00 Mary Starnes: Hello, I spoke with you yesterday about this computer, Jayakaran... and");
			sr.WriteLine("2012-12-17T00:06:58+00:00 Mary Starnes: I have decided to return it to amazon.  However, I want to discontinue Norton 360 and Norton One from this computer but not my other one... can you help me?");
			sr.WriteLine("2012-12-17T00:07:36+00:00 Jayakaran Y E: <span dir=\"ltr\">Sure I will</span>");
			sr.WriteLine("2012-12-17T00:07:37+00:00 Mary Starnes: This computer is WAY out of date and not worth the money I paid.  There are even drivers that are totally out of date.");
			sr.WriteLine("2012-12-17T00:08:17+00:00 Jayakaran Y E: <span dir=\"ltr\">On the Norton One web page Please go to my account</span>");
			sr.WriteLine("2012-12-17T00:08:38+00:00 Mary Starnes: I do not understand what you mean");
			sr.WriteLine("2012-12-17T00:08:51+00:00 Jayakaran Y E: <span dir=\"ltr\">Your Norton one website</span>");
			sr.WriteLine("2012-12-17T00:09:10+00:00 Jayakaran Y E: <span dir=\"ltr\">You should already be logged in to it.</span>");
			sr.WriteLine("2012-12-17T00:09:36+00:00 Mary Starnes: I see, I am logged in!");
			sr.WriteLine("2012-12-17T00:09:38+00:00 Mary Starnes: sorry");
			sr.WriteLine("2012-12-17T00:09:47+00:00 Jayakaran Y E: <span dir=\"ltr\">Thats Okay</span>");
			sr.WriteLine("2012-12-17T00:09:58+00:00 Jayakaran Y E: <span dir=\"ltr\">you will find my account, on the top</span>");
			sr.WriteLine("2012-12-17T00:10:02+00:00 Jayakaran Y E: <span dir=\"ltr\">Please click on it</span>");
			sr.WriteLine("2012-12-17T00:10:13+00:00 Mary Starnes: I see it ... and am clicking");
			sr.WriteLine("2012-12-17T00:10:23+00:00 Jayakaran Y E: <span dir=\"ltr\">Under My account you will see products and Norton One Highlighted</span>");
			sr.WriteLine("2012-12-17T00:10:56+00:00 Mary Starnes: Yes, I see that");
			sr.WriteLine("2012-12-17T00:11:01+00:00 Jayakaran Y E: <span dir=\"ltr\">You will see the licenses used.  You will find the name of the computer that you wish to remove</span>");
			sr.WriteLine("2012-12-17T00:11:16+00:00 Jayakaran Y E: <span dir=\"ltr\">Please click on the trash can next to it and remove it</span>");
			sr.WriteLine("2012-12-17T00:11:17+00:00 Mary Starnes: the product key?");
			sr.WriteLine("2012-12-17T00:11:29+00:00 Mary Starnes: serial number?");
			sr.WriteLine("2012-12-17T00:11:38+00:00 Jayakaran Y E: <span dir=\"ltr\">Below that</span>");
			sr.WriteLine("2012-12-17T00:12:03+00:00 Mary Starnes: just a second");

			sr.Close();
			fs.Close();


			//And one binary file
			
			fs = new FileStream(_tempDir + "\\test5.dll", FileMode.Create);
			sr = new StreamWriter(fs);

			sr.WriteLine("This file is binary because it has char: \0\0\0\0");
			
			sr.Close();
			fs.Close();

			fs = new FileStream(_tempDir + "\\test6.txt", FileMode.Create);
			sr = new StreamWriter(fs);

			sr.WriteLine("This file has special chars");
			sr.WriteLine("This \t file has special \v chars");

			sr.Close();
			fs.Close();
			

			Directory.CreateDirectory(_tempDir + "\\subDir");
			File.Copy(_tempDir + "\\test1.txt", _tempDir + "\\subDir\\test1.txt", true);
			File.Copy(_tempDir + "\\test2.txt", _tempDir + "\\subDir\\test2.txt", true);
		}

		protected void CreateTestDir()
		{
			_tempDir = Path.GetTempPath() + "\\FindAndReplaceTests";
			Directory.CreateDirectory(_tempDir);
		}


		protected void DeleteTestDir()
		{
			var tempDir = Path.GetTempPath() + "\\FindAndReplaceTests";
			
			Directory.Delete(tempDir, true);
		}


		protected void CreateSpeedDir()
		{
			_speedDir = _tempDir + "\\Speed";

			if (Directory.Exists(_speedDir))
				throw new InvalidOperationException("Dir '" + _speedDir + "' already exists.");

			Directory.CreateDirectory(_speedDir);
		}

		protected void DeleteSpeedDir()
		{
			_speedDir = _tempDir + "\\Speed";

			if (Directory.Exists(_speedDir))
				Directory.Delete(_speedDir, true);
		}

		//Rounds up to 1000 chars
		protected string GetFileContent(int fileSize)
		{
			var sbFileContent = new StringBuilder();
			for (int i = 0; i < System.Math.Ceiling((decimal)fileSize / 1000); i++)
				sbFileContent.Append(Chars1000);

			return sbFileContent.ToString();
		}


		[TearDown]
		public virtual void TearDown()
		{
			var tempDir = Path.GetTempPath() + "\\FindAndReplaceTests";
			File.SetAttributes(tempDir + "\\test3.txt", FileAttributes.Normal);

			DeleteTestDir();
		}

	}
}
