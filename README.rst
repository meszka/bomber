######
Bomber
######

A 2-player variation on Bomberman with platformer-style movement.

You can download a binary release for Windows here:
https://github.com/meszka/bomber/releases/download/v0.1/bomber.zip

If you're on Linux, look below for build instructions.

Build
=====

Requires MonoGame. Tested with MonoGame 3.0.1.

Linux
-----

Requires mono.

::

    xbuild /p:Configuration=Release bomber.sln
    cd bomber
    ./bin/Release/bomber.exe

Or build the ``bomber.sln`` solution using MonoDevelop.

Windows
-------

Build the ``bomber_windows.sln`` solution using Visual Studio.

Controls
========

Hold down the bomb key and release it to throw a bomb. The longer you hold the
stronger the throw.

Red player
----------

* A/D - move

* W - jump

* S - bomb

Blue player
-----------

* Left/Right - move

* Up - jump

* Down - bomb

Creating maps
=============

You can create your own maps by copying and editing the existing map files in
the ``Content/Maps`` directory. A map is just a plain text file consisting of
15 lines with 20 characters each. The characters have the following meaning:

* space - empty space...

* # - "hard" block (cannot be destroyed)

* % - "soft" block (can be destroyed by an explosion)

* 0 - red player's starting position

* 1 - blue player's starting position
