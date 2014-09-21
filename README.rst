######
Bomber
######

A 2-player variation on Bomberman with platformer-style movement.

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

Player 1
--------

A/D - move
W - jump
S - bomb

Player 2
--------

Left/Right - move
Up - jump
Down - bomb
