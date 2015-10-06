86ME (86Duino Motion Editor)
---------
_VERSION_: v1.5

* A new button to save 86Duino Frame Files.
* A new button to load 86Duino Frame Files.

_VERSION_: v1.4

* Refine UIs of Motion Group.
* Add a ComboBox for setting bluetooth buad rate.
* Make saving existing projects more convenient.
* Add TOWERPRO_SG90 to servo list.
* Fix bugs that changing tabpage MotionConfig may enable some controls unexpectedly.

_VERSION_: v1.3

* Move sketch generation functions to MotionElement.cs.
* Add a tabpage named MotionTrigger for setting the condition to trigger the selected motion.
* Add three methods(auto, keyboard, bluetooth, ps2 controller) to trigger motions.
* Integrate all motions and triggers into generated sketches now.
* Add a function to insert intermediate frame.
* 86ME can check whether a project needs to be saved now.
* Update content of tool tips.

_VERSION_: v1.2

* Change the checkbox "Auto Play" to "Sync", and add a tracebar for tuning the speed of synchronizing motors.
* Some UIs move to the new groupbox called global settings.
* The method of adding new actions is more instinctive that users can choose action type in the same area.
* Add some captions to contorls. They will apear when the mouse move on them.
* After loading a picture of robot, the file name of the picture will be shown.
* Support infinite loop now.
* Support generating sketches containing nested loops now.
* For loading rbm files more conveniently, users can drag a rbm file into 86ME.exe and it will be loaded automatically.

_VERSION_: v1.1b

* Fix bugs that the generated files contain space in their names.
* Fix bugs that 86ME cannot load pictures containing space in file names. 

_VERSION_: v1.1

* Refine UIs of "Robot Configuration", "Motion List", "Motion Test".
* Replace the button "MotionTest" to Play, Pause, Stop buttons.
* Generate loop functions while generating 86Duino programs if there are flag-goto objects in the motionlist.
* Add a new action type "HomeFrame".
* Update information of Help->About.

_VERSION_: v1.0

* This is the Robot Motion Editor of the open-source 86Duino electronic platforms.
* The motion editor is designed to edit servo motions and generate 86Duino programs for controlling RC servos through Servo86 library of 86Duino.

#### INSTALLATION ####

1. System reqirement: Windows XP/Vista/7/8/8.1/10 with .NET framework 4.
2. Download [86ME](https://github.com/Sayter99/86ME/releases/download/86ME/86ME_v1.5.zip).
3. Unzip 86ME and execute 86ME.exe to start.

#### DOCUMENTS ####

The 86ME tutorial pages for installation instructions and usage.

* [Current Version](http://www.86duino.com/index.php?p=11544&lang=TW)
* [Version 1.4](http://www.86duino.com/index.php?p=12778&lang=TW)
* [Version 1.3](http://www.86duino.com/index.php?p=12646&lang=TW)
* [Version 1.2](http://www.86duino.com/index.php?p=12298&lang=TW)
* [Version 1.1](http://www.86duino.com/index.php?p=11850&lang=TW)

#### CREDITS ####

* 86ME is an open source project. 86ME is derived from RoboME of [RBgod](https://github.com/RoBoardGod/RoBoME) and is developed by [Sayter](sayter@dmp.com.tw).
* All of them are members of the RoBoard team in DMP Electronic Inc.

**If you find any bug, or want to provide software patches or to request enhancements about 86ME, please report to the 86Duino [forum](http://www.86duino.com/?page_id=85).**
