# Space Engineers - Worker
1.0.0

This class was made for atmospheric worker ships with either the Standard or the Industrial cockpit. It displays current amount of payload, the remaining charge of the batteries, and provides a stabilizer. It includes:
+ [__Battery__](../Battery/)
+ [__Payload__](../Payload/)
+ [__Reroute__](../../Utility/Reroute/)
+ [__Stabilizer__](../Stabilizer/)

## Public methods
Default constructor.
```C#
Worker(Program parent, string cockpit = "", string container = "", float yaw_speed = 0.5f)
```
Update the displays and the Stabilizer.
```C#
void Update(UpdateType updateType)
```
Toggle the Stabilizer on/off.
```C#
void SwitchStabilizer()
```
---
*See Darra* - 10/2019
