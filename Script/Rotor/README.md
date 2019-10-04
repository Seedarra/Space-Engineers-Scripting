## Space Engineers - Rotor
1.0.0

This class can be used to control one or more rotors. You can rotate them step-by-step or you can set them to a certain angle.<br>
The Velocity setting of the rotor is used to determine rotation speed. Rotors with a Velocity of 0 will not rotate.

#### Public methods
Default constructor where `name` can be the name of a group, a single block, or `""` for all available blocks on the grid.
```C#
Rotor(Program parent, string name = "")
```
Rotate to an angle.
```C#
void SetAngle(float angle)
```
Change the current angle.
```C#
void AddAngle(float angle)
```
Get the current angle.
```C#
float CurrentAngle { get; }
```
---
*See Darra* - 10/2019
