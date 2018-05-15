# Regular Expressions

## Regular Expressions

Regular expressions provide a robust, flexible, and efficient method for processing text. The [.NET RegEx engine](http://msdn.microsoft.com/en-us/library/hs600312(v=vs.110).aspx) is used for regular expressions.

In the .NET Framework, regular expression patterns are defined by a [unique syntax or language](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference), which is compatible with Perl 5 regular expressions and adds some additional features such as right-to-left matching.

To use regular expressions in your find/replace text, you need just to check the "Use regular expressions" checkbox if you are using the UI. For example, to search for emails then you can use the following regular expression.


```csharp

\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b

```

<img src="images/regular-expressions.png" alt="Regular Expressions"/>

But if you are using the command line in batch mode then use **--useRegEx** flag.


```csharp

"D:\fnr.exe" --cl --dir "D:\Project\ZZZ\FNR Testing\Temp" --fileMask "*.*" --includeSubDirectories 
    --useRegEx --find "\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b" --replace ""

```




