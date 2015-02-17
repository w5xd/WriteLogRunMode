To build this from source, it should work with Visual Studio Express 2010 as-is
...with one exception...
The References to the three WriteLog assemblies will need to be manually added.

To get started:
Run Visual C# 2010 Express
File Open Project WriteLogRunMode.csproj
View Solution Explorer
Under WriteLogRunMode in the Solution Explorer, right click References and then Add Reference...
In the "Add Reference" dialog, switch to the Browse tab.
In Look In control, browse to WriteLog's Programs installation directory.
Click the Name column to sort by Name
Scroll to and CTRL+left-mouse click these three:
	WriteLogClrTypes.dll
	WriteLogExternalShortcuts.dll
	WriteLogSHortcutHelper.dll

Click OK and the project should build

