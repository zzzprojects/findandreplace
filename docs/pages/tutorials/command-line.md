---
permalink: command-line
---

## Command Line

**Find and Replace (FNR)** can also be used as a command line utility. The intersting thing about FNR is that it can create command line text to put in batch file by simply clinking on the **Gen Replace Command Line** button.

<img src="images/command-line.png" alt="Command Line" width="99%"/> 

### Example

Let's try to replace "license" with "agreement" by using the command line, so simply copy the command line text created by FNR.

{% include template-example.html%} 
{% highlight csharp %}

"D:\fnr.exe" --cl --dir "D:\Project\ZZZ\FNR Testing\Temp" --fileMask "*.*" --excludeFileMask 
    "*.dll; *.exe" --includeSubDirectories --find "license" --replace "agreement"

{% endhighlight %}

<img src="images/command-line-1.png" alt="Command Line" width="99%"/> 

