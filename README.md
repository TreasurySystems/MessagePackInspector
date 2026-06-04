MessagePack Inspector
============================

This is a Fiddler Classic message inspector for MessagePack using [Nerdbank.MessagePack](https://github.com/AArnott/Nerdbank.MessagePack).

Installation for Fiddler4
------------

Copy all files from `/bin/Release/net48/` into fiddlers `Inspectors` folder (can usually be located at "`C:\Program Files\Fiddler\Inspectors`" or "`%LOCALAPPDATA%\Programs\Fiddler\Inspectors`") and then start Fiddler.


Development
-----------

The project requires a reference to your `Fiddler.exe` file in order to compile.
Make sure to install Fiddler Classic before trying to build the project.

You will probably need to modifyfy the .csproj file and update the path to fiddler executables
