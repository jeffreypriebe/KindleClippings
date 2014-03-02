KindleClippings
===============

Currently works against latest Kindle 5 Clipping file format.

.NET parser for the Amazon Kindle's "My Clippings.txt" file.

The solution has two components, a library (KindleClippings) and GUI (KindleClippingsGUI) which provides a sample front-end for the library.

The GUI uses [Eto.Forms](https://github.com/picoe/Eto) for cross-platform compatibility.

How To Use
----------

__Library__


1. Build the solution and reference the outputted KindleClippings.dll
2. Call MyClippingsParser.Parse() using either (a) the path to a "My Clippings.txt" file or (b) a stream matching the format of a "My Clippings.txt" file to return the clippings
3. Optionally call ClippingOrganizer.GroupClippingsByAuthorAndBook(), passing in the clippings, to organize them

__GUI__

(No Changes to GUI expected in this project.)

1. Connect your Kindle to your PC as a USB drive
1. Copy "My Clippings.txt" from your Kindle (optional)
2. Build/Run the GUI
3. Browse to the location of the "My Clippings.txt" file __OR__ select the connected, removable Kindle device
4. Click "Parse"
5. Expand the appropriate author and book in the tree
6. Double click on a highlight/note entry in the tree to view details


TODO
----

See Original Project.