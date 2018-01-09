<h2><a href="http://findandreplace.io/download">DOWNLOAD LATEST VERSION HERE</a></h2>
<br />

<h1>Project Description</h1>
<p>An open source tool to find and replace text in multiple files.<br /><br /></p>

<h1>Latest Releases</h1>
<p>
<strong>Version 1.7 has been released on May 29th, 2014!</strong><br />
<a href="https://findandreplace.codeplex.com/releases/view/122813">Read all about it here</a>
<br /><br /><strong>IMPORTANT</strong>: If you use quotes or backslashes in command line find/replace text, please regenerate your commands before using fnr.exe in batch file. The logic for encoding quotes and slashes has changed in 1.7 to cover all common cases where users had issues.<br /><br /><strong>Version 1.6 has been released on Mar 24th, 2014</strong><br /><a href="https://findandreplace.codeplex.com/releases/view/119228">Read all about it here</a>
<br /><br /><strong>Version 1.5 has been released on Nov 6th, 2013</strong><br /><a href="https://findandreplace.codeplex.com/releases/view/113464">Read all about it here</a><br /><br />
</p>

<h1>Features</h1>
<ul>
<li>Single file download - fnr.exe (181kb)</li>
<li>Replace text in multiple files using windows application or through command line</li>
<li>Find Only to see where matches are found</li>
<li>Case-sensitive searching</li>
<li>Searching for files in one directory or recursing sub-directories</li>
<li>Regular expressions</li>
<li>Find and replace multi-line text</li>
<li>Generate command line button to create command line text to put in batch file</li>
<li>Command line help</li>
<li>Unit tests of Find/Replace engine</li>
</ul>
<p>&nbsp;</p>
<h1>Screenshots</h1>
<p>For screenshots I used a common scenario of replacing a ConnectionString in all ConnectionStrings.config in the solution when rolling from staging to production.<br /><br />Screenshot 1: Finding all occurrences of connection string.<br /><br /><img title="FnR_Screenshot1_Find.png" src="http://download-codeplex.sec.s-msft.com/Download?ProjectName=findandreplace&amp;DownloadId=699486" alt="FnR_Screenshot1_Find.png" /><br /><br /><br />Screenshot 2: Replacing all occurrences of connection string.<br /><br /><img title="FnR_Screenshot2_Replace.png" src="http://download-codeplex.sec.s-msft.com/Download?ProjectName=findandreplace&amp;DownloadId=699487" alt="FnR_Screenshot2_Replace.png" /><br /><br />Screenshot 3: Generating command line text to run "Replace" using batch file<br /><br /><img title="FnR_Screenshot3_GenerateCommandLine.png" src="http://download-codeplex.sec.s-msft.com/Download?ProjectName=findandreplace&amp;DownloadId=699492" alt="FnR_Screenshot3_GenerateCommandLine.png" /><br /><br /><br />Screenshot 4: Viewing command line options<br /><br /><img title="FnR_Screenshot4_CommandLineHelp.png" src="http://download-codeplex.sec.s-msft.com/Download?ProjectName=findandreplace&amp;DownloadId=699491" alt="FnR_Screenshot4_CommandLineHelp.png" /><br /><br /><br />Screenshot 5: Finding occurrences using command line. <br /><br /><img title="FnR_Screenshot5_Find_CommandLine.png" src="http://download-codeplex.sec.s-msft.com/Download?ProjectName=findandreplace&amp;DownloadId=699493" alt="FnR_Screenshot5_Find_CommandLine.png" /><br /><br /><br />Screenshot 6: Replacing occurrences using command line.<br /><br /><img title="FnR_Screenshot6_Replace_CommandLine.png" src="http://download-codeplex.sec.s-msft.com/Download?ProjectName=findandreplace&amp;DownloadId=699494" alt="FnR_Screenshot6_Replace_CommandLine.png" /></p>
<h1>Background</h1>
<p>A couple of months ago I set out to find a simple find and replace tool that I could use when rolling out my projects. For ex. to change various settings in config files to support dev/qa/prod environment.<br /><br />I was looking for something that would have the following features:</p>
<ol>
<li>Simple UI to make sure that find/replace does what I need it to</li>
<li>Command line to run find/replace using batch file.</li>
</ol>
<p><br /><br />There are many tools available, but they all have the following major issues:</p>
<ul>
<li>Out of date, no updates support for at least a year</li>
</ul>
<p>for ex. <a href="http://www.divlocsoft.com/">http://www.divlocsoft.com/</a></p>
<ul>
<li>Bad documentation if any. Many apps mentions that they have command line support, but to find out how it works takes too much effort.</li>
</ul>
<ul>
<li>Too many features. I just need to type in a couple of fields and click on Run, not to learn a new language.</li>
</ul>
<p>for ex.<br /><a href="http://www.powergrep.com/">http://www.powergrep.com/</a><br />grep/sed variants in windows</p>
<ul>
<li>No access to code. Many tools have 90% of what I need, but if I need just a bit more (support for multi-line) - there is not much I can do.</li>
</ul>
<p><br />So I wrote a simple tool to use in my own projects and wanted to share it with others.<br /><br /><br />Shameless plug for my company: <a href="http://www.entechsolutions.com">http://www.entechsolutions.com</a></p>
