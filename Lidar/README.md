# Lidar - contains C# library using Slamtec lidar 

The C# connector for Slamtec RpLidar uses at the base the native lidar driver from sdk and between a C library to communicate with the device.

- the RpLidarConnector class starts an seprate task for receiving data from lidar and raises an event when a scan is received.   
- the result is a list of PolarVectors. 
- the project is tested with Slamtec RpLidar A1 only, but should work with the others too. 

- Uses parts of Slamtec RpLidar Sdk, with some changes that the sdk driver project build, also x64 build is added.
- If needed, the origin SDK C++ source from Slamtec you will find here: https://github.com/Slamtec/rplidar_sdk, this fork is optimized for using Windows 10 runtime. You find that forked SDK in the RPLIDAR_SDK folder of this project.

I mentioned with lidar A1:
- the motor stops only short time when calling stop. The motor stops when you pull the cable.

Give it a try, feel free to use, change what you need. 
 

