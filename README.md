Mirrors AR Game
===============

While augmented reality research has grown into a mature field over the
last years, the aspects of situational awareness and presence of
augmented reality (AR) are still quite open research topics. This
project is about designing and implementing a collaborative game to
explore the different perception of situational awareness, presence and
workload in a physical and an AR environment.

Game Concept
------------
The goal of the game is to solve a puzzle by controlling laser beams using mirrors
in such a way that a predefined target is hit. The game can be played by
one or more local players and one or more remote players.

There are cards present for the local players that represent mirror bases. These
must be placed on the table, which will be the locations for the mirrors. The local
players will be able to see the mirrors they place through the use of AR technology.
Each of the local players will only be given a few of the mirror bases needed to solve
the puzzle, and as such solving the puzzle requires cooperation from all local players.

The remote players can also see the placed mirrors, and can rotate them to influence
the path of the laser beam(s). Only by cooperation between local players (who can
only move the mirror bases) and remote players (who can only rotate them) it becomes
possible to hit the target and as such solve the puzzle.

The game provides various different types of mirrors with different properties,
allowing for more complex puzzles. One example of such a mirror is a colored mirror,
and then require the target is hit with the right (combination of) colors. Another
way to make puzzles more complex is requiring that the players combine beams together
to create more powerful beams. Other optical components like beam splitters can
also be introduced.

Repository Layout
-----------------
The repository contains two main projects: a Unity project, found under the
'ARGame' directory; and a C++/Qt project, found under the 'MirrorServer'
directory. Furthermore, reports and accompanying documentation can be found
in the 'Reports' directory. The Unity project acts as a client application,
and should be run for everyone who wishes to play. The C++/Qt project acts as a
server application and should be run from a central location.

Building the Unity project
--------------------------
The Unity project is designed for Unity 5 or higher. For playing as a local
player, the application needs to be deployed to Windows, as the Meta One glasses
do not work properly on other platforms. Remote players do not use the Meta One
and so they can also play using other platforms.

Building the server application
-------------------------------
The server application is written in C++ and uses Qt and OpenCV. The application
has been tested and confirmed working on OpenCV 2.4.11 and higher, using Qt 5.4
or higher. Older Qt and OpenCV versions might work, but this is not tested.

First, make sure OpenCV and Qt are installed for the compiler of your choice.
The tested compilers are the MSVC 2015 compiler on Windows, and Apple's LLVM
compiler (clang) version 6.0, though others should also work.

To build the application, run qmake on the MirrorServer.pro file. Make sure an
environment variable 'OPENCV_HOME' is defined and points to the OpenCV
installation. The QMake project file links to the Qt version that the QMake
executable belongs to, and the OpenCV version defined by 'OPENCV_HOME'.

When the QMake step is finished, run the build tool for your compiler (e.g.
nmake for MSVC, make for GNU, and so on). When using Qt Creator, which is
recommended, the build command in Qt Creator automatically invokes QMake and
the correct make utility, provided OPENCV_HOME is set in the build environment.
