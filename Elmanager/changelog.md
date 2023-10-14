# Elmanager changelog

## Unreleased
* SLE: Texture names are sorted in ground and sky dropdowns.
* SLE: Bugfix: Sky texture was not rendered correctly if zoom textures was disabled.

## 4.6.2023
* SLE & Replay viewer: Added grass rendering support.
* SLE: Internal levels can be directly opened from the menu.
* SLE: A warning is shown if LGR doesn't have ground/sky texture of the level.
* Replay viewer: Players can be selected from the viewing area.
* In rendering settings, LGRFile is now LGROverride. If set, this LGR is always used instead of `<lgrdir>/<levellgr>.lgr`. The earlier setting was messy and not clear what it meant.
* SLE: Bugfix: Frame tool sometimes added the source polygon back when switching to another tool.

## 24.2.2023
* SLE: Selection is no longer updated when changing selection filter. Separate menu items have been added to unselect all elements of a specific type.
* SLE: Added option to disable "Save/restore start position" feature.
* SLE: Added option to switch between constant and non-constant physics FPS. Default is non-constant.
* SLE: Added brake alias key for playing.
* SLE: Added option to change picture placement anchor.
* SLE: Added fullscreen option (F11 and optionally automatic toggle on play/stop).
* SLE: Zoom level while playing is now separate from normal zoom.
* SLE: Adjusted tool help texts to be more complete and consistent.
* SLE: If LGR is missing pictures, picture names are shown in the warning message.
* SLE: Improved performance of level saving and topology checking.
* SLE: Bugfix: After playing stopped, the player was still hoverable with mouse.
* SLE: Bugfix: qup_18 and qdown_18 appeared in picture list although they are not usable as pictures.
* SLE: Bugfix: SVG export was upside down.
* SLE: Bugfix: Some Across levels failed to open because of different magic number in level file.

## 11.2.2022
* SLE: Added basic saveload support when playing.
* SLE: Physics FPS can be changed.
* SLE: Shortcuts can be disabled when playing.
* SLE: Bugfix: Tab key was not working well if used as turn key.
* SLE: Bugfix: Raster image import result was being vertically mirrored.
* SLE: Bugfix: Apples were not always faded when grabbed.
* SLE: Bugfix: Grid color sometimes changed after grabbing the first apple.

## 24.10.2021
* SLE: Updated ground vertex limit to match newest EOL version (5130 -> 20000).
* SLE: Bugfix: Topology check was very slow if the level had thousands of vertices.

## 12.2.2021
* SLE: Added support for in-editor testing.
* Replay viewer: Added support for showing approximate gas on/off times. (@pskrip)
* Replay manager: Fixed wrong tabulation order. (@pskrip)
* Replay viewer: Fixed an UI layout problem. (@pskrip)
* Fixed a crash when trying to open replay/level manager without configuring lev/rec paths.
* Changed settings file format to JSON. The reason was that the old format (binary) is considered insecure (according to official .NET docs), and as a bonus, you can now edit the settings with a text editor.

## 18.7.2020
* Level manager has been rewritten from scratch in C#, so it is now similar to replay manager.
* Elmanager is now a single-file program and no longer requires .NET Framework.
* SLE: Grass and object visibilities are respected in SVG export.
* SLE: Use the path of the currently opened level (if available) as the initial directory in file dialogs
  (which is usually the same as the level directory in settings but not always).
* Bugfix: Textures were sometimes rendered upside down.
* SLE: Bugfix: Saving as picture always saved as SVG even if PNG format was chosen.
* SLE: Bugfix: Transform tool crashed in a certain situation. (Fixed by tej)

## 18.8.2019
* SLE: Added support for SVG import and export.
* SLE: Added support for drag & drop import.
* Elmanager DLLs now reside in "bin" directory, making EOL folder less messy if Elmanager.exe is in the same folder. (Contributed by tej!)
* Bugfix: Elmanager failed to start in Wine.
* SLE: Bugfix: Polygon operations would sometimes leave duplicate vertices in polygons.
* SLE: Bugfix: When a level was saved under a different filename, an undo operation would restore the previous filename.
* SLE: Bugfix: Font selection in text tool did not work in Wine.

## 20.4.2019
* Replay upload functions (to zworqy and Jappe2) have been removed because the upload functions no longer exist.
* LM: Default lower bound for flowers is now 0 (instead of 1).
* SLE: Added "Move start here" context menu option.
* Bugfix: New version download was not working because of [GitLab removing support of older TLS](https://about.gitlab.com/2018/10/15/gitlab-to-deprecate-older-tls/).
* SLE: Bugfix: Texturization tool crashed on custom masks. It now gives a warning about them not being totally supported but still allows them to be used.
* SLE: Bugfix: Status bar texts were not being updated during mouse movement. (They were updated only after mouse stopped moving.)

## 10.11.2018
* SLE: Bugfix: Topology check did not detect too short edges.

## 1.8.2018
* SLE: Levels without flowers are now allowed but they must have at least one object to pass topology check.
* SLE: Bugfix: Topology error message about too many vertices was sometimes inaccurate.
* SLE: Bugfix: Select tool didn't always prefer selected elements.
* SLE: Bugfix: Saving level after changing filename reverted back to old filename.
* SLE: Bugfix: Crosshair did not work with some tools.

## 5.1.2018
* SLE: Improved Picture tool usability.
* SLE: Added support for high DPI monitors.
* SLE: Inactive grass edges are now shown dashed.
* SLE: Bugfix: Crash if LGR had no images.
* SLE: Bugfix: Too many pictures were excluded from the list of available pictures.

## 28.5.2017
* SLE: QGRASS is available as a ground/sky texture.
* SLE: Added option to mirror selection vertically.
* SLE: Bugfix: LGRs with invalid transparency values did not work.
* SLE: Bugfix: Transparency type for certain pictures was not ignored like in Elma.

## 9.4.2017
* SLE: QGRASS is available as a texture.
* SLE: Numbering direction for filename suggestion is deduced automatically.
* SLE: Title and LGR changes are included in undo history.
* SLE: Improved accuracy of rendering overlapping pictures with same distances.
* SLE: Improved accuracy of texture rendering.
* SLE: Bugfix: Some pictures were rendered incorrectly because LGR transparency fields were ignored.
* SLE: Bugfix: The character '#' in level title was incorrectly reported as invalid.
* SLE: Bugfix: Font dialog would sometimes crash when selecting certain fonts. (You still cannot use such fonts, but now a proper error message is shown.)
* SLE: Bugfix: Wrong "editing in progress" notification was shown with PolyOp tool.
* SLE: Bugfix: PolyOp crashed if inner and then outer polygon was selected.
* Bugfix: Main window tab order was incorrect.

## 31.12.2016
* Added separate files for launching replay manager and SLE.
* SLE: Start position is automatically saved when opening a level.
* SLE: Bugfix: Level was not marked as modified when restoring start position.
* Replay viewer: Added an option to force driver-centered zoom when not playing.
* Replay viewer: Pressing arrow keys to move screen works better.
* Replay viewer: Bugfix: Moving with arrow keys when playing a replay didn't update screen until releasing the key.

## 28.12.2016
* Integrated Level manager.
* SLE: Added possibility to save start position and restore it later.
* SLE: Selection is updated when selection filter is restricted.
* SLE: Picture tool dialog uses numeric text box for distance.
* SLE: Bugfix: Bogus "cannot check topology..." was sometimes shown when connecting polygons.
* SLE: Bugfix: Connect tool did not always connect polygons if the level had a specific topology error (two vertices at the same spot).
* SLE: Bugfix: Pictures/textures were not ignored when checking if the level is too big.
* Replay manager: Added an option to disable replay list tooltip.
* Replay manager: Bugfix: Replay list messed up selected item when something was removed from the list.
* Replay viewer: Last zoom level is remembered.
* Replay viewer: Ground edges no longer "jitter" when playing a replay.
* Replay viewer: Bugfix: Gravity apple arrows were not removed for collected apples.
* Bugfix: SLE or replay viewer could crash if sdl2.dll was found in system path.
* Updated external libraries.

## 16.10.2016
* SLE: Improved handling of default values in picture tool.
* SLE: Bugfix: Picture tool close button click was sometimes being interpreted as OK button click.

## 13.10.2016
* SLE: Title box warns if title contains non-Elma-printable characters.
* SLE: Added "Textures out of bounds" topology check.
* SLE: Added "Wheel exactly on a line" topology check.
* SLE: Clipping and distance are no longer changed when changing picture/texture image.
* SLE: Added support for opening LEB files.
* SLE: Bugfix: Changing apple animation number didn't cause level to be modified.
* SLE: Bugfix: Crash would happen when level folder was moved.
* SLE: Bugfix: It was not possible to convert multiple ground polygons to grass.

## 24.4.2016
* SLE: Bugfix: Texturize function would crash if there was no LGR file selected.
* Downgraded to .NET Framework 4 to keep Windows XP supported.

## 22.4.2016
* SLE: Added a function to automatically texturize the selected polygons.
* SLE: Improved support for polygon operations.
* SLE: Vertex tool: The length of the current edge is shown.
* SLE: Improved the display of error message about non-existent level.
* SLE: Bugfix: Polygon operation tool could cause SLE to freeze.
* SLE: Bugfix: Cut tool could cause SLE to freeze.
* Updated to .NET Framework 4.5 (from 4). If you have Windows 8 or newer, you don't have to install it. People with Vista or 7 can download it [here](https://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe).

## 7.3.2016
* SLE: Copy function now respects the current zoom level. To get the old behavior (copy and snap to grid), press Ctrl+Shift+C.
* SLE: Bugfix: Tool pane lost focus after changing ground or sky texture.
* Main window: Updated homepage address.
* Main window: Replaced Mawane's email with the URL to his obituary :(

## 8.1.2016
* SLE: Draw tool is now zoom-aware and allows adjusting threshold.
* Added an option to disable framebuffer usage.
* If there are several older settings files after updating, the latest of them is loaded instead of the first found.
* SLE: Bugfix: Picture/texture limit was not being checked.

## 27.8.2015
* Replay viewer: Bugfix: Replay playing was not working at all.

## 25.8.2015
* SLE: Improved rendering performance for levels with lots of textures.
* SLE: Added a shortcut key for saving as picture.
* SLE: Added an option to show inactive grass edges.
* SLE: Select also objects, pictures and textures inside the polygon with left Alt.
* SLE: Added an option to capture pictures and textures from borders only.
* SLE: Improved handling of possible topology errors with fonts in text tool.
* SLE & Replay viewer: Snapshot is saved with high resolution.
* Changelog is displayed in a web page by clicking a link instead of in a text box.
* SLE: Bugfix: Incorrect topology error about too many vertices was reported sometimes.
* SLE: Bugfix: AutoGrass froze with certain polygons.
* SLE: Bugfix: Mouse wheel zoom did not always work on Windows 10.

## 31.5.2015
* SLE: Make numpad `+`/`-` work for tools.
* SLE: Grid can be resized and moved with mouse.
* SLE: Added support for importing images by integrating Radim's Vectrast tool.
* SLE: Level filename can be renamed.
* SLE: Added "Delete level" button.
* SLE: Added an option to show rectangle for maximum dimensions.
* SLE: User can set a custom new level template.
* SLE: Added an option to choose whether to use circles or triangles for vertices.
* SLE: Holding left Alt when selecting a polygon selects all inner polygons as well.
* SLE: Bugfix: Polygon edge selection was slightly inaccurate.
* SLE: Bugfix: Some settings were not applied initially.

## 20.4.2015
* SLE: Added support for apple animation numbers.
* SLE: Gravity apple arrows can be shown.
* SLE: Invisible elements are filtered automatically.
* SLE: Frame tool: Frame can now be placed inside or outside the selected polygon.
* SLE: Vertex size can be adjusted.
* SLE: Vertices are shown only when the corresponding polygon is visible.
* SLE: Show LGR warning message in the toolbar instead of a message box.
* SLE: Level LGR is loaded automatically if possible.
* SLE: Autograss thickness can be changed interactively.
* SLE: Picture tool is always enabled and informs user to select LGR if needed.
* SLE: Improved layout of some text labels.
* SLE: Bugfix: An exception could occur with the picture tool if the LGR had no pictures.
* SLE: Bugfix: Picture/texture count was wrong in level properties window.
* SLE: Bugfix: LGR images were not always loaded correctly.
* SLE: Bugfix: Object tool could crash if LGR was not loaded.
* SLE: Bugfix: Vertex color setting was not working.
* Added dependencies as NuGet packages.
* More robust update checking. Enabled update checking for debug builds, too.

## 25.12.2014
* SLE: Added text tool.
* SLE: Cut/connect tool now detects automatically whether to cut or connect.

## 9.12.2014
* SLE: Vertex tool can now create rectangles (left Shift).
* SLE: Changed "mirror level" to "mirror selection".

## 13.11.2014
* SLE: Bugfix: Topology check didn't detect coinciding vertices.
* SLE: Bugfix: Wrong tool was displayed after changing tool during transforming.
* SLE: Bugfix: Invalid sequence of OpenGL calls could result in a crash sometimes.

## 9.11.2014
* SLE: Properties can be changed for multiple pictures at once.
* SLE: Added "Convert to" -function in context menu.
* SLE: Levels with corrupted top 10 list can be opened.
* SLE: Simplified vertex tool - only edges can be clicked when adding vertices to existing polygons.
* SLE: Level width and height are displayed in level properties window.
* SLE: Polygons can be de-selected by Ctrl+clicking an edge.
* SLE: Change level title to filename only when saving for the first time.
* SLE: Bugfix: Delete key was not working for text boxes.
* SLE: Bugfix: Level could not be saved after undo or redo.
* SLE: Bugfix: Polygon grassness is preserved when cutting.
* SLE: Bugfix: Apples were drawn incorrectly in Pipe tool if LGR was not loaded.
* Replay viewer: Bugfix: The viewer size was set slightly wrong when using text boxes.
* RM: Finished times are truncated to 3 decimal places.

## 19.8.2014
* SLE: Bugfix: Vertex tool was broken when editing a grass polygon.
* SLE: Bugfix: Cut/connect tool did not obey Snap to grid -option.
* SLE: Topology errors are cleared on undo and redo.
* SLE: Bugfix: Polygon operation could cause Elmanager to crash in some situations (fixed IsSimple method).

## 6.7.2014
* SLE: Added "Import level(s)" function.
* SLE: Added "Save as picture" function.
* SLE: Added free selection mode.
* SLE: Added option to show crosshair.
* SLE: The inactive edges of grass polygons are hidden.
* SLE: In pipe tool, radius can be adjusted with Page Up/Down.
* SLE: Removed maximum limit of 20 vertices in smoothing tool.
* SLE: Some settings of tools are saved (number of sides in ellipse, pipe radius, frame radius and smoothing parameters).
* SLE: Improved smoothing tool by widening the range of parameters.
* SLE: If there are many vertices under mouse, the closest one will be taken (but selected vertices are still preferred).
* SLE: Vertices are now rendered using small triangles instead of points. This is faster style and matters in huge levels.
* SLE: Optimized polygon decomposition algorithm a lot. This makes some operations a lot faster.
* SLE: Optimized topology checking with the help of an external library.
* SLE: Bugfix: Undoing an operation was not possible in a certain scenario when bending an edge.
* SLE: Bugfix: The text "Checking topology..." was red in some cases.

## 22.4.2014
* SLE: Selected vertices are now preferred when two or more vertices are under mouse pointer.
* SLE: Added a shortcut key for copying (Ctrl+C).
* SLE: Bugfix: Level width and height must be < 188, not <= 188.

## 7.3.2014
* SLE: Bugfix: The ellipse tool would sometimes create ellipses with an extra vertex, causing topology error.
* Update checker: Elmanager will automatically exit after downloading an update. Added also a warning text for this.
* Update checker: When the download button is clicked, the text of it will turn into "Downloading..." to indicate that the download started.

## 9.2.2014
* SLE: Bugfix: Elmanager could crash when saving a level.

## 8.2.2014
* SLE: Reverted back to old polygon merging/difference algorithm due to bugs in the new one.
* SLE: Level properties are now copyable.
* SLE: Bugfix: Pluralization was wrong in topology check if only 1 problem was found.
* Startup window: Changed link of Mawane's website to email address and added tooltip for it.

## 6.2.2014
* SLE: Bugfix: Topology was not checked when redoing/undoing.
* SLE: Bugfix: CPU is no longer 100% when scrolling the level with arrows.
* SLE: Bugfix: Filename suggestion failed to increment level number when saving a second level.
* SLE: Bugfix: Topology error marks were visible if opening a new level.
* SLE: Improved polygon operation tool slightly; it can now handle some edge cases better (but still not all).
* SLE: Improved help texts of some tools.
* Credited Mawane for testing and SLE toolbar graphics in startup window.

## 24.11.2013
* SLE: Improved scrolling - it's no longer jerky when pressing two arrows at the same time.
* SLE: Bugfix: It was not possible to set grid size to greater than 10.
* SLE: Bugfix: If grass polygons intersected with the head of player, it counted as a topology error.
* SLE: Bugfix: It was not possible to select vertexes when they were exactly on a line in another polygon.
* SLE: Bugfix: If you selected many apples and changed gravity, it only changed gravity on one of them.
* SLE: Bugfix: If you selected polygons and pictures and moved them, then undoed, the pictures wouldn't be selected anymore.
* SLE: Bugfix: Polygon operation tool could cause Elmanager to crash in some situations.

## 23.6.2013
* Bugfix: Small Across levels were not loaded correctly.
* DLL files are now in the same ZIP file as Elmanager.exe, so you won't have to download them separately if they get updated.

## 27.11.2012
* OpenGL graphics context is now explicitly initialized to use 8 depth bits and 8 stencil bits (this probably fixes a ground/sky drawing problem for some people).
* Some other small tweaks.

## 19.5.2012
* Replaced `up.k10x.net` with `www.jappe2.net/upload`.
* Compressed internal levels -> smaller exe size.
* Changelog is shown when new version is detected.
* After updating, Elmanager will now try to load old settings file instead of forcing to reset all.

## 17.1.2012
* SLE: Levels that don't have start or exit object will be handled properly (they are added automatically when opening such a level).

## 4.12.2011
* SLE: Fixed 2 bugs (in cut/connect tool and transform).
* SLE: Improved transform tool slightly (you can now move the elements by dragging from the center).
* SLE: Simplified filename suggestion.
* SLE: Some other small updates.

## 21.8.2011
* SLE: Fixed a couple of bugs.

## 13.8.2011
* SLE: Fixed F4 bug.
* Ability to put Elmanager in cache.
* Source code published.

## 6.5.2011
* SLE: Checking topology lists all problems, not just the first found.
* SLE: Checking topology warns if some apples/flowers are inside ground (unreachable).
* SLE: Intersections are highlighted with a red square instead of circle because they easily got mixed up with apples.
* Replay viewer: Added zoom "bar" (it's a button, click and hold it and move mouse left/right to zoom).
* Replay viewer: Added "Multi spy" (follow driver must be unchecked or no players selected in playlist). Idea by jon.
* SLE: Bugfix: Fixed picture/texture distance bug (finally).
* SLE: Bugfix: Fixed a couple of small bugs.
* When changing some option in rendering settings, the view is updated automatically (no need to close window to preview).

## 20.3.2011
* SLE: Transform: rotate only/scale only -modes (hold Left Shift/Ctrl).
* SLE: "Lock lines": hold Left Shift and move a vertex.
* SLE: Whole kuski body is shown (in LGR mode).
* SLE: Pipes/ellipses are snapped to grid now too.
* SLE: Select all with Ctrl+A.
* Replay viewer: Improved playback speed -bar and playing bar.
* Replay viewer: Event type checkboxes moved to context menu.
* Replay manager: New search options: File size and date modified.
* Replay manager: State of replay list is now saved (order of columns and their widths).
* Replay manager: Upload replay to zworqy or `up.k10x.net` easily (in context menu).

## 23.12.2010
* Level editor: Info texts should be better visible now.
* Level editor: Numpad +/- -keys changed to other +/- keys (next to right shift and backspace, you know).
* Level editor: Editor's name is SLE from now on.
* Replay viewer: Kuski's body is fully visible, including arm animations when rotating (when LGR file is selected etc).
* Replay viewer: Fixed small bug where taskbar was visible when going to fullscreen.
* Replay manager: Names of internals are shown.
* Increased max zoom out & zoom fill margin.

## 4.12.2010
### Level editor
* Topology errors (intersections) are now highlighted with red circles.
* Draw regular polygons with Ellipse -tool by holding Ctrl.
* Top10 is selectable in Level properties window.
* Average shown in top10.
* Previous and Next level -buttons for browsing levels in lev directory.

### Other
* Elmanager code has been converted from VB .NET to C# now.

## 23.11.2010
### Level editor
* Added some toolbar buttons for quick access (thanks to Mawane for the graphics).
* Connect -tool (Cut/connect, press space to switch mode).
* Unsmoothen -tool (this is in the Smoothen tool, just hold Ctrl to unsmooth).
* Level properties -window (shortcut key = F4, you can see amount of level elements and top 10).
* Shortcut keys for tools (the letters should be underlined in the left panel, if not, press Alt).
* Best time for singleplayer shown in a text label.

### General
* Faster startup (at least a bit, some operations are done on a separate thread now (such as update checking), so they won't slow the program down).
* Rendering settings are in a nice property grid now.

### Replay manager
* Maximize state is remembered.

### Replay viewer
* Bugs fixed.

## 11.11.2010
### Level editor & replay viewer
* A lot more flexible rendering settings: you can choose to show ground/grass polys/fill ground polys/pictures/textures/picture frames/vertices/objects and so on. Settings are separate for level editor and replay viewer, of course.

### Level editor
* Support for pictures and textures. LGR file must be specified to enable this.
* Improved context menu.
* Selection filter. Useful for example if you want to delete all textures/pictures from a level.
* Ability to start creating vertex from polygon edge too (earlier you had to click some vertex).
* Ability to have apples placed automatically when creating pipe (press Space to switch between modes). Adjust the other parameter with Ctrl + +/-.
* Combined Merge and Difference tools into one (press Space to switch the mode).
* Combined Apple, Killer and Flower modes into one (use Space here too for switching).

### Replay viewer
*  Use "zoom fill" by default.
*  Do not select any player by default.
*  Maximize the window of replay viewer by deafault (not fullscreen).

## 12.10.2010
### Replay viewer
* You won't need internals as externals anymore (they're as an embedded resource now in the program).
* Drag screen by clicking and holding middle mouse button.
* Fixed the apple indexing bug (this was max annoying). Now all replays should play fine.
* Fixed smaller bug that caused pictures not load in some cases.

### Level editor
* Edge bending: While in Select -mode, hold left shift and then click and hold some edge to bend it. Simple and nice feature, really useful for me at least.
* Fixed the small annoyances mentioned by Ali.

## 28.9.2010
### Replay viewer
* Changed the fullscreen shortcut key to F11 (because Enter interfered with things).

### Level editor
* Fixed small bug in Undo/Redo.
* Optional grid + snap to grid.
* Improved Smoothen -function: you can adjust smoothness with +/- and another parameter with +/- when holding left Ctrl.
* AutoGrass thickness is adjustable in config window, as well as grid size.

## 20.9.2010
### Replay manager
* Improved "Save to textfile" -function performance A LOT.
* Improved replay list performance somewhat.
* Added possibility to include old filename in Rename pattern -function. For example, the pattern "FT" simply attaches the time of the replay to its current filename.

### Level editor
* Mirror level -function.
* Info about current selection is shown at the bottom.
* Filename suggestion according to the given pattern (regular expression).
* Set default level title, or use filename.
* Fixed the bug that was found by Ville_J: When I select to transform a polygon, press delete, then start transforming the empty area, another polygon disappears and the polygon I chose to transform comes up again.
* Other small tweaks (fixed tab order for tools and others...).

## 12.9.2010
* Improved AutoGrass.
* Move screen by clicking and holding middle mouse button (usually the wheel).
* New polygon operation: difference (subtracts the area of second polygon from the first polygon).
* Highlight level elements under mouse cursor (optional).
* Select colors for the editor.

## 3.9.2010
* AutoGrass.
* QuickGrass (applies AutoGrass to all ground polygons).
* Undo/redo (pretty necessary feature).
* Kuski's right wheel and head is also shown, and if the head is touching ground, you will be notified when checking topology.
* Some bug fixes.

## 23.8.2010
* Updated level editor. New features: transformation, cutting, smoothing and mouse locking with Z and X buttons.

## 15.8.2010
* Changed program's name because now it has 2 components - replay manager and level editor.

## 17.7.2010
* Simplified user interface a bit.
* Press F2 to rename a replay from now on. Editbox will appear above the replay list. Press Esc to cancel renaming and enter to accept new name.
* Default ground/sky -option for replay viewer.
* Search only best replays: this option will only pick replays that have the best time in a level. Useful for getting a collection of your best replays for a levelpack, for example.

## 27.6.2010
* Better GUI and added suspensions for players in replay viewer. Many bugs/annoying things fixed too.

## 6.6.2010
* Improved replay viewer. It shows almost in-game graphics. The only difference is that grass is not shown and the bike is not yet complete.

## 9.5.2010
* OpenGL drawing (much faster than earlier).
* LGR file support (you can see ground and sky textures in viewer now).
* DLL files are now separate (there are 6 of them). Put them in the same folder as ReplayManager.exe.
* Some user interface improvements.

## 4.4.2010
* Simplified user interface.
* Separate configuration window.
* Better replay list.
* Ability to generate a database from replays to make searches faster.
* Separate replay properties - window for viewing properties.
* Resizable main window (vertically) to adjust replay list size.

## 22.12.2009
### Replay manager
* Whole new replay list component that has columns. This allows you to see much more information at the same time. You can arrange the list by any of the columns by clicking its header. To reverse the order, click it again. You can also change the order of columns with click+hold+drag.
* Duplicate replay search is a lot faster.
* Duplicate filename search for replays.
* Replay viewer is in separate window now. You can open as many new viewers as you want.
* Program settings are saved now in the same directory as the program itself.
* When saving stuff to textfile, all info (not only rec filenames and times) will be saved in it. This means the infos in the replay list.
* You can have as many compare windows open as you want.
* Use regular expressions to search replays for specific externals.
* Open level file of selected replay with default program.


### Replay viewer
* Select colors for objects/active player/inactive player/ground/sky/driving lines for each player.
* Event list that shows events for currently selected player. Click some event in the list to go in that moment in viewer. You can filter different events with the six checkboxes.
* Zoom in/out with mouse wheel (in addition to left/right click).
* See multiple players' driving lines at the same time by highlighting many replays in the playlist. "Show driver path" must be checked of course. Driving lines aren't visible while playing.
* Playing is controlled now by a timer, which makes playing (hopefully) smoother.
* Current speed shown for currently selected player.
* Separate button for saving replay viewer settings. Note that for driving lines, only the color for 1st player's line is saved. This is mainly because there's no upper bound for the number of replays in the playlist.

## 25.9.2009
* Fixed bugs.

## 24.7.2009
* Shortcut keys for replaylist. Ctrl+X=Move, Ctrl+C=Copy, Ctrl+A=Select all, Ctrl+Z=Remove from list, Ctrl+S=Save times to textfile, Del=Delete selected replays. You can see these shortcut keys in the contextmenu too (except Del).
* Show bike/mouse coordinates (click the text to switch between them).
* Removed textbox for zoom to driver - now you can zoom in/out while playing too.
* Fixed a couple of bugs.
* Added the Remove from list -button back because it was asked.

## 17.7.2009
* Executable size reduced to 80 KB (it was about 130 KB).
* Removed "Remove from list" -button because it was simply useless.

## 12.7.2009
* Ability to reset settings (in contextmenu).
* Fill polygons -checkbox to see which is ground and sky.
* Added support for replays driven in Across levels.
* Search replays driven in Elma/Across levs.
* Better viewer resizing style: just grab the window border while you're in replay viewer mode.
* Show level coordinates at mouse position while moving above viewer.
* Sort replaylist by alphabet/date created/date modified.

## 8.7.2009
* File association - you can associate .rec files with Replay manager by choosing Open with... ---> Browse for ReplayManager.exe ---> always use this program. NOTE: Trying to open multiple replays won't do anything. And, RM must not be already running while opening a recfile.
* Improved saving times to textfile -feature, and fixed a bug in it - it showed wrong total time if it was more than 1 hour.
* Replay and level folder selection are now in contextmenu (open it by right-clicking anywhere in the program, except replay viewer and replay list).
* Ability to delete replays to Recycle bin or permanently (default is Recycle bin). Change this option in the same contextmenu.

## 2.7.2009
* Ability to resize Replay comparison window.
* Fixed the bug in duplicate replay search.
* Fixed Replay manager's initial size. It was too big.

## 30.6.2009
* Keep player visible after he dies/finishes.
* Checkbox for loop playing.
* Antialiasing feature. When enabled, the lines look smoother in replay viewer.
* Added trackbar component like in media players with which you can scroll the replay(s) easily.
* Fixed the bug I talked about.
* Improved replay finishing-detection code.
* Optimized replay playing a bit further.

## 24.6.2009
### Major updates
* Compare replays also with groundtouches. This allows you to compare also replays that have no apples.
* Other events also included in the eventlist (left/right volt, supervolt, turn, groundtouch). There are checkboxes for each of those.
* Merge 2 singleplayer replays like in Elma to one file.
* Play many replays simultaneously. No limit. Level must be the same of course.
* Save times of selected replays to a textfile with 2 or 3 decimals. Total time is included in it.
* Calculate total time of selected replays automatically (with duplicate replay search too).
* Click in eventbox to go in that moment in replay viewer (if lev exists). Switch automatically to viewer if not already.
* Zoom in/out by clicking the level picture with mouse (left click=Zin, right click=Zout). The place you clicked will be the new center of the viewer. ZIn and ZOut buttons removed.
* "Advanced search options" -button with which you can search by number of left/right volts, turns, appletakes etc.
* Calculate trip for both players.
* "Lock viewer" -checkbox with which the replays on the playlist can stay there if you want to check another replay's properties. It must have the same level.
* Program tells after levelfilename if it doesn't exist or level version is wrong. When trying to play a replay with wrong level version, it will ask yes/no.

### Minor updates
* Copy-checkbox and the button moved to contextmenustrip (=CMS).
* "Take subdirectories in Lev folder into account"-->"Search also subdirectories in level folder".
* "Search also subdirectories in Rec folder"-->"Search also subdirectories in replay folder".
* Click total time to show it as 2 or 3 decimals. Checkbox removed. Label moved next to rename buttons.
* "Compare replays" -button moved to CMS.
* "Remove from list" -button moved to CMS.
* Play & Pause moved to same button, and "Stop" to old Pause button.
* "Switch to replay viewer/search options" -button moved out of Replay properties box.
* Replay and level folder paths are remembered in folderbrowserdialog when clicking it.
* "Erroneous replays" -box appears as a new window.
* Replay comparison appears as a new window.
* Removed "Calculate top speed" -checkbox because it's useless. Top speed is always calculated.
* Removed ability to scroll level in viewer with arrow buttons, because it didn't always work, and you can easily use now mouse for that.
* Message "Across levels are not supported" appears if you try to play replays that have Across level.
* ...possibly some other small things that I forgot to put here.

### Bug fixes
* Sometimes the program went back to search options for no reason.
* When pressing zoom in/out after pressing pause, it didn't zoom right area.
* When deleting/removing replay from list while watching it, an exception occurred. Now you can't delete while watching.
* Improved the frame-drawing routines A LOT. Levels with lots of vertices play now much faster than before.
* Improved replay comparison code.

## 22.4.2009
* Select many replays at once. Right-click the replaylist to select all/invert selection. You can delete/compare/rename with pattern the replays you selected.
* Rename selected replays with user-defined pattern. Example: Levelname+Nickname+Time (levelname always required).
* Move/Copy selected replays to anywhere (copied/moved replays will be removed from the list).
* Fixed a small bug with replay comparison, sometimes it didn't show correct filename.

## 16.4.2009
* Improved head position and size.

## 14.4.2009
* Head position should be more or less correct now. There's also a line in the head indicating where the driver is looking at.
* Possibility to stop searching, and you can start inspecting replays even when search is in progress.
* Added Viewer size - box in which you can define the size of the viewer window. Range: 200 to 850, default=500.
* Added Loop delay - box with which you can make replay playing slower. This delay means milliseconds per frame. Range: 0 to 1000, but some 20-30 should give close to actual speed.
* Level and replay folder paths are hidden when switched to replay viewer (they may overlap it if the path is long).
* Added "Save picture as..."-button (saves what you see in replay viewer, format=png).
* Click twice Pause while playing=Stop (goes to start of the replay, in other words).

## 7.4.2009
* Zoom in/out/fill. You can use these also when the replay is playing IF "Zoom to driver" - checkbox is off.
* "Zoom level" -box is only used with "Zoom to driver" -option.
* Previous and next frame -buttons.
* Follow player 2 -checkbox for multiplayer replays.
* Time shown while playing replay. You can also jump to some spot in the replay by typing the time in the box (like 03:25,000), remember 9 characters. Useful in long replays.
* Progress shown while searching replays.
* Wheel rotation now shown while playing.

## 3.4.2009
* Replay playing.

## 1.4.2009
* Replay viewer (no playing yet).

## 27.3.2009
* "Remove from list".
* Total time calculation.

## 25.3.2009
* Compare replays.

## 22.3.2009
* Show top speed of replay.

## 14.3.2009
* Better replay finishing detection.

## 9.3.2009
* Search for erroneous recs.

## 6.3.2009
* Replay renaming.
* Duplicate replay search.

## 5.3.2009
* Initial release.
