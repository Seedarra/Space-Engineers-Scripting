## Space Engineers - Payload
1.0.0

Get the amount of cargo in percent, kilogram or liters in the inventories of a given set of blocks.

#### Public methods
Default constructor where `container` can be the name of a group, a single block, or `""` for all available blocks on the grid.
```C#
Payload(Program parent, string container = "")
```
Get the maximum amount of cargo available in liters.
```C#
float Capacity { get; }
```
Get the amount of cargo in kilograms.
```C#
float Mass { get; protected set; }
```
Get the amount of cargo in liters.
```C#
float Volume { get; protected set; }
```
Get the amount of cargo in percent.
```C#
float Percentage { get; protected set; }
```
Update all values.
```C#
void Update()
```
---
*See Darra* - 10/2019
