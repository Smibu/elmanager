# Elmanager
Elmanager is a tool for managing [Elasto Mania](http://www.elastomania.com) replays and levels. Most important features include replay/level searching and viewing and a level editor.

Elmanager is written in C#.

## System requirements
Windows 7/8/10.

## Installation
Unzip `Elmanager.zip` to any directory, such as `C:\Elma\Elmanager`. Program settings are also saved in this directory.

## Features
Elmanager has basically three components: replay manager, level manager and a level editor (named SLE, "Smibu's level editor").

### Replay manager

Features:

  -   Replay searching with many parameters
      -   Internal/external replays
      -   Finished/unfinished replays
      -   Singleplayer/multiplayer replays
      -   Replays driven in Across/Elma levels
      -   Replays whose time matches the specified range
      -   Replays driven in specified levels
      -   Replays with wrong level version
      -   Replays with a missing level file
      -   Replays whose filename matches the specified regular expression
      -   Replays whose appletakes/left volts/right volts/supervolts/turns/groundtouches match the specified ranges
  -   Duplicate replay search
  -   Duplicate filename search
  -   Best replay search
  -   Replay renaming
  -   Replay list that displays properties of replays
      -   Filename
      -   Filename of level
      -   Time
      -   Whether the replay is finished/unfinished
      -   Multiplayer/singleplayer
      -   Whether the level exists for the replay
      -   Wrong version of level
      -   Hover mouse cursor above a replay to show more information (appletakes, left volts, ...)
  -   Replay viewer
      -   Watch as many players in the same level as you want simultaneously
      -   Choose LGR file in configuration window to play the replay with same graphics as in game
      -   Customizable colors for apples, killers, active/inactive player, flower, start object, driving lines, ground and sky
      -   Smooth zooming
      -   Event list displaying the selected events for selected player
      -   Fullscreen mode
      -   Locked camera -mode (for fun)
  -   Replay comparison
      -   Compare replays by apples or groundtouches
  -   Move or copy selected replays to anywhere
  -   Open level file of a replay with the default program
  -   Replay renaming with pattern
  -   Save properties of selected replays to a text file
  -   Merge two singleplayer replays to a multiplayer replay

#### Screenshots
![Main window](pictures/RM.png)

![Replay viewer](pictures/RMviewer.png)

### Level manager

Features:

- Search levels
  - File name
  - Best time (single/multi)
  - Number of replays
  - Number of grass/ground polygons/vertices
  - Nicknames in best times
  - Number of (gravity up/down/left/right) apples/killers/flowers
- Delete selected times from top 10
- View replays in a level

#### Screenshots
![Main window](pictures/LM.png)

### Level editor (SLE)

Features:

  -   Create pipe
  -   Draw ground with pencil
  -   Dynamic topology checking
  -   Frame polygons
  -   Cut/connect polygons
  -   Smoothen/unsmoothen polygons
  -   Lock mouse horizontally or vertically
  -   Transform selected objects
  -   AutoGrass
  -   QuickGrass
  -   Undo/redo
  -   Grid
  -   Mirror level
  -   Edge bending
  -   Polygon operations (merge, difference)
  -   Type text
  -   Customize various colors

#### Screenshots
![Main window](pictures/leveleditor.png)
