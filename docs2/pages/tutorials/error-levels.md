---
permalink: error-levels
---

## Error Levels

**Find and Replace (FNR)** supports the following error levels:

Level | Description|
:-----|:-----------|
0 | No errors|
1 | Major error like directory is invalid|
2 | Read/Write error for any processed file|

By default, only 0 and 1 are returned by the command. To enable ErrorLevel 2, use new flag --setErrorLevelIfAnyFileErrors. See details for flags [here](/flags).