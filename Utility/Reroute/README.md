# Space Engineers - Reroute
1.0.0

This class mimics a console window and is purely aesthetic. It reroutes text written with `MyGridProgram.Echo()` from the programmable block's detail info area to the programmable block's display.<br>
Have a look at [Program.cs](Program.cs) for an example on how to use this class.

## Public methods
Default constructor.
```C#
Reroute(Program parent)
```
Start a new reroute. Everything send via `Echo` will be written to a internal text buffer.
```C#
void Compile(string user)
```
Stop rerouting. The text buffer is written to the programmable block's display and `Echo` will behave as usual.
```C#
void Report()
```
Method to catch exceptions. To be used in a `catch` block.
```C#
void Cancel(string message)
```
---
*See Darra* - 10/2019
