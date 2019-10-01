## Space Engineers - Blockfinder
1.0.0

This class provides an easy way to find blocks by name, group, type or search term.<br>Each method of this class returns `true` if a block (or a list of blocks) on the same grid as the caller was found, and `false` otherwise.

#### Public methods
Default constructor.
```C#
Blockfinder(Program parent)
```
Get a single block by name.
```C#
bool ByName<T>(string name, out T block) where T : class
```
Get a list of blocks by group name.
```C#
bool ByGroupName<T>(string name, out List<T> block_list) where T : class
```
Get the first block of a type.
```C#
bool ByType<T>(out T block) where T : class
```
Get all blocks of type.
```C#
bool ByType<T>(out List<T> block_list) where T : class
```
Get a single block that matches a search term.
```C#
bool BySearch<T>(string term, out T block) where T : class
```
Get a list of blocks where each block matches a search term.
```C#
bool BySearch<T>(string term, out List<T> block_list) where T : class
```
Get a list by group name. If it fails try to get a single block by name.
```C#
bool ByCombination<T>(string name, out List<T> block_list) where T : class
```
Same as `ByCombination` but fetches all blocks if `name` is empty.
```C#
bool ByDefault<T>(string name, out List<T> block_list) where T : class
```
---
*See Darra* - 09/2019
