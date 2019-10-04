# Space Engineers - Battery
1.0.0

Get power status of batteries: the amount of power stored (in MWh or percent), and the total capacity (in MWh).

## Public methods
Default constructor where `batteries` can be the name of a group, a single block, or `""` for all available blocks on the grid.
```C#
Battery(Program parent, string batteries = "")
```
Update all values.
```C#
void Update()
```
Get the current power in MWh.
```C#
float CurrentPower { get; protected set; }
```
Get the maximum power in MWh.
```C#
float MaxPower { get; }
```
Get the remaining charge in percent.
```C#
float Percentage { get; protected set; }
```
---
*See Darra* - 10/2019
