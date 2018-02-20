<h2><a href="http://findandreplace.io/download">DOWNLOAD LATEST VERSION HERE</a></h2>
<br />

<h1>Project Description</h1>
<p>An open source tool to find and replace text in multiple files.<br /><br /></p>

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
<p>For screenshots I used a common scenario of replacing a ConnectionString in all ConnectionStrings.config in the solution when rolling from staging to production.<br /><br />Screenshot 1: Finding all occurrences of connection string.<br /><br /><img title="FnR_Screenshot1_Find.png" src="http://findandreplace.io/images/358a59e5-8789-4779-b131-0cf89b5a0286.png" alt="FnR_Screenshot1_Find.png" /><br /><br /><br />Screenshot 2: Replacing all occurrences of connection string.<br /><br /><img title="FnR_Screenshot2_Replace.png" src="http://findandreplace.io/images/bab753cd-5f59-44f1-b62a-0a24b99345d5.png" alt="FnR_Screenshot2_Replace.png" /><br /><br />Screenshot 3: Generating command line text to run "Replace" using batch file<br /><br /><img title="FnR_Screenshot3_GenerateCommandLine.png" src="http://findandreplace.io/images/2196c765-9b2a-4035-984c-01bdca0f8755.png" alt="FnR_Screenshot3_GenerateCommandLine.png" /><br /><br /><br />Screenshot 4: Viewing command line options<br /><br /><img title="FnR_Screenshot4_CommandLineHelp.png" src="http://findandreplace.io/images/246341ae-bf89-4318-856d-5d308b13d79a.png" alt="FnR_Screenshot4_CommandLineHelp.png" /><br /><br /><br />Screenshot 5: Finding occurrences using command line. <br /><br /><img title="FnR_Screenshot5_Find_CommandLine.png" src="http://findandreplace.io/images/3d5ad2c7-0122-4e90-b985-f81a4b7641b5.png" alt="FnR_Screenshot5_Find_CommandLine.png" /><br /><br /><br />Screenshot 6: Replacing occurrences using command line.<br /><br /><img title="FnR_Screenshot6_Replace_CommandLine.png" src="http://findandreplace.io/images/e0652876-1428-4d98-8e47-646df4cefa84.png" alt="FnR_Screenshot6_Replace_CommandLine.png" /></p>
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
