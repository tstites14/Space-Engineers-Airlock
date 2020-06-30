# Space-Engineers-Airlock
An MDK-SE project to be used in Space Engineers to simulate the cycling of an airlock.

Installation:
- Copy Program.cs to C:\Users\USERNAME\AppData\Roaming\SpaceEngineers\IngameScripts\local.
- Open a Programmble Block in Space Engineers.
- Click Edit in the Control Panel.
- Click Browse Scripts.
- Select SE-Airlock from the list of available scripts on the left.

Ingame Requirements:
- Two sensors in a group called "Airlock Sensors".
  - NOTE: The actual names of the blocks does not matter as long as they are in a correctly titled group.
  - The sensor by the outside door must have its "Custom Data" field say 1.
  - The sensor by the inside door must have its "Custom Data" field say 0.
- Two doors in a group called "Airlock Doors".
  - NOTE: The actual names of the blocks does not matter as long as they are in a correctly titled group.
  - NOTE: These doors do not need anything in their "Custom Data" fields (but if you want to put something in there it won't hurt).
- A group of lights called "Airlock Lights".
  - NOTE: The type of light does not matter as long as they all use under the IMyInteriorLight interface (typically Corner Lights and Interior Lights).
- One Air Vent called "Airlock Vent".
- One Rotating Light (IMyReflectorLight) called "Airlock Rotating Light".
