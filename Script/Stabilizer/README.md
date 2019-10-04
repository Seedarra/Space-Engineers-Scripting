## Space Engineers - Stabilizer
1.0.0

This class can be used to level out a ships pitch and roll while keeping rotation around the yaw axis possible (optional). It will utilize all gyroscopes found and takes their respective block orientation into account.<br>
The Stabilizer can be activated from different cockpits on the same grid no matter their orientation.

#### Public methods
Default constructor. You might need to experiment with the `yaw_speed` value to make yaw rotation for your ship pleasant. This value is influenced by the number of gyroscopes, the weight of the ship, and your controller sensitivity settings. If set to `0.0f` the Stabilizer is fully locked.
```C#
Stabilizer(Program parent, float yaw_speed = 0.5f)
```
Turn the Stabilizer on or off. Returns `true` if activated, and `false` if deactivated.
```C#
bool Switch()
```
Does the stabilization (if activated).
```C#
void Update()
```
---
*See Darra* - 10/2019
