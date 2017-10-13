We should put something in this readme. -Jake


Before running, you should configure the server.json file in the root folder to use the ip address of the computer running Python and the port you want to use.
If running on the same computer, you can set the ip to 127.0.0.1.
Both Python and C# read this file, so you only need to set it once. 
If you move the file you will need to reset the path in the Python scripts and in Unity. To do this in unity, simply click the "..." button in the TcpConnection script on the Cycle game object and select the new file.
You can do something similar if you move the settings file (but on the Settings game object).

How to run: https://www.youtube.com/watch?v=7ZNFCuZZ3nc
Configure settings file at runtime: https://www.youtube.com/watch?v=YDZcDlgR4ME


Jake Thorn (12/ 10/ 2017) (jakethorn1.1@gmail.com)
================================================================================

RL Robot Assets folder>

HoloLens
: Assets specific to the HoloLens build.

Materials
: Self-explanatory.

Plugins
: Third-party libraries.

Prefabs
: Self-explanatory.

Prefabs> Reinforcement Learning.prefab
: Template that can be used in your own reinforcement learning scenario.

Robot
: Robot model and assets. Downloaded from https://github.com/jthom330/Unity3D-Robotic-Arm-Simulator.

Scenes
: The main scene (RL Robot.unity).

Scripts
: Self-explanatory. Most are general use scripts.

Scripts> RL
: Scripts used for reinforcement learning.

Scripts> RL> Robot
: Scripts used specifically for the robot scenario.


RL Robot.unity>

UI> Canvas> Theta Sliders
: Sliders to control robots axis rotations.

UI> Canvas> Start
: Start reinforcement learning cycle.

UI> Canvas> Camera Preview
: Previews what the robot can see and is sending to TensorFlow (Python).

Reinforcement Learning> Cycle
: Controls flow of data between C# and TensorFlow (Python).
: Data is provided by the SimpleDataHandler and sent and recieved by the TcpConnection, controlled by Cycle.

Reinforcement Learning> Cycle> Cycle
: Will send data the Outgoing Data from the SimpleDataHandler to TensorFlow (Python) and assign the data
: received by TensorFlow to the values representated by Incoming Data in the SimpleDataHandler. It will do
: this as fast possible, only stopping if Cycle.Wait has been set to true.

Reinforcement Learning> Cycle> Simple Data Handler
: Outgoing data goes to Python (TensorFlow) and Incoming data is set to the values provided by TensorFlow.
: Data can be assigned through fields, properties, methods or directly from the inspector.

/*
For example, in the RL Robot.unity scene, the robot's camera, target's position, robot's three rotations and 
the robot's camera's position are sent to TensorFlow. A time, three rotations, a method call and a whether or
not we should reset are passed back from TensorFlow.
*/

Reinforcement Learning> Robot> Robot Controller
: Can rotate the robot based on values set by the SimpleDataHandler.

Reinforcement Learning> Robot> Target
: Robot's red target. It is the only thing seen by the robot's camera. It is randomly positioned on reset, but will only appear within the limits set.

Settings
: Several settings that can be edited at runtime (Even when built).

Dispatcher
: Needs to be in the scene due to Cycle being multi-threaded. 

================================================================================
