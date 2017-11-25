---
permalink: detect-encoding
---

## Detect Encoding

Find and Replace (FNR) uses two approaches two detect file encoding:

 - First KlerkSoft Bom Detector which is streamlined version of code from [here](http://www.architectshack.com/TextFileEncodingDetector.ashx)

 - If Bom Detection fails it uses Microsoft's MLang library using approach described [here](http://www.codeproject.com/Articles/17201/Detect-Encoding-for-In-and-Outgoing-Text)

### Why Detection Fail?

In some very rare cases the detection may fail or may report a false positive. 

- The reason for it failing is that detecting correct file encoding 100% of the time is impossible. 
- MLang uses heuristics to go through file content and give its best guess on encoding. 
- Usually its guess is correct 99%+ of the time. 
- The only sure way to load a file with correct encoding is to know it upfront and pass it in.

### Workaorund 

If you have a case where encoding detection fails for a specific file, you will get error message "Could not detect file encoding." for that file. To get around this error there are several approaches:

#### Save as Unicode Encoding.

 - You can open the problematic file in Notepad and save it using Unicode encoding. 
 - This will add a BOM header to the file so it will be easily detected by KlerkSoft Bom Detector


#### Use Enconding Flags

 - Use **--defaultEncodingIfNotDetected** or **--alwaysUseEncoding** to bypass automatic detection. See details for these flags in the [Flags](/flags).

 - For these flags you can use any encoding listed [here](http://msdn.microsoft.com/en-us/library/system.text.encoding.getencodings(v=vs.110).aspx). Just scroll down to example results and look for values in column 'identifier and name':