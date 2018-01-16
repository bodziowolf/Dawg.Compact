# Dawg.Compact
Fast prefix matcher allowing to search completions and if a given word or prefix exists in the set of words. 
Based on Direct Acyclic Word Graph stored internally as a binary array. 
Written in C# .NET as an experiment of how data locality impacts data structure performance.

## Usage
```csharp
IPrefixMatcher matcher = new DawgBuilder()
                    .WithOrderedWordsFromFile("file-with-words-new-line-separated.txt")
                    .BuildCompactDawg();

IEnumerable<string> completions = matcher.GetWordsByPrefix("ca");
bool hasWord = matcher.HasWord("cat");
bool hasPrefix = matcher.HasPrefix("ca");
```

## Benchmarks 
### Dictionary Build Time

Implementation|Time
--------------|----
Dawg.Compact|00:00:01.8421006
Dawg.Compact (before compacting)|00:00:01.4801577
DawgSharp|00:00:01.7217895
DAWG (AdamSpeight2008)|00:00:00.6072873

### Prefix Completion Suggestions (10 000 000 prefixes, tested 10 times, averaged run time)

Implementation|Time
--------------|----
Dawg.Compact|00:00:06.0257105
Dawg.Compact (before compacting)|00:00:21.3418086
DawgSharp|00:00:14.5927630
DAWG (AdamSpeight2008)|not available

### Check If Word Exists (10 000 000 prefixes, tested 10 times, averaged run time)

Implementation|Time
--------------|----
Dawg.Compact|00:00:01.0424479
Dawg.Compact (before compacting)|00:00:02.4014963
DawgSharp|00:00:01.5317197
DAWG (AdamSpeight2008)|00:00:05.0766425

### Check If Prefix Exists (10 000 000 prefixes, tested 10 times, averaged run time)

Implementation|Time
--------------|----
Dawg.Compact|00:00:01.0546408
Dawg.Compact (before compacting)|00:00:02.3811045
DawgSharp|not available
DAWG (AdamSpeight2008)|not available

Where:
* Dawg.Compact is CompactDawg class you would normally use from this library
* Dawg.Compact (before compacting) is a reference implementation as tree (NodeDawg class)
* DawgSharp is https://github.com/bzaar/DawgSharp
* DAWG (AdamSpeight2008) is https://www.nuget.org/packages/DAWG

Keep in mind that DawgSharp and DAWG (AdamSpeight2008) can also store playloads under keys.