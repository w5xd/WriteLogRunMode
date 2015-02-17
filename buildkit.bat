@rem dependencies
@rem    install 7zip and put it on the path
@rem fetch/install the Install subproject
@rem
del WriteLogRunMode.zip
7z a WriteLogRunMode.zip install.bat uninstall.bat readme.txt
pushd Install\bin\Release
7z a ..\..\..\WriteLogRunMode.zip ConfigureShortcuts.exe* CopyShortcutAssembly.exe*
popd
pushd bin\Release
7z a ..\..\WriteLogRunMode.zip WriteLogRunMode.dll
popd
