---
permalink: special-characters
---

## Escape Characters

**Find and Replace (FNR)** supports escape characters such as "\n", "\t", etc. To use [escape characters](https://en.wikipedia.org/wiki/Escape_character) in your find/replace text, you need just to check the **Use escape chars** checkbox if you are using the UI. 

<img src="images/escape-chars.png" alt="Escape Chars"/>

But if you are using the command line in batch mode then use **--useEscapeChars** flag.

{% include template-example.html%} 
{% highlight csharp %}

"D:\fnr.exe" --cl --dir "D:\Project\ZZZ\FNR Testing\Temp" --fileMask "*.*" --includeSubDirectories 
    --useEscapeChars --find "\t" --replace "\r\n"

{% endhighlight %}

## Accented Characters

By default, when you run DOS commands, the values in arguments are limited to [code page 437](https://en.wikipedia.org/wiki/Code_page_437). 

To pass chars that are not supported by code page 437 in find/replace text or other arguments, you can use the following techniques:

 - How to write a batch file that has accented chars: [http://stackoverflow.com/questions/1427796/batch-file-encoding/1427817](http://stackoverflow.com/questions/1427796/batch-file-encoding/1427817).
 - View UTF-16 or other encodings in DOS prompt [http://stackoverflow.com/questions/10764920/utf-16-on-cmd-exe](http://stackoverflow.com/questions/10764920/utf-16-on-cmd-exe). 
 - One more tip is to make sure you save your BAT file in UTF-8 or another format that displays the chars correctly. Don't use ASCII.