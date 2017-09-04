@rem dependencies
@rem    install 7zip and put it on the path
@rem fetch/install the Install subproject
@echo off
	call GetBuildVer.bat
	setlocal
	set zip=WriteLogRunModeV.2.%_BuildVersion%.zip
	echo Building zip %zip%
	if exist "WriteLogRunMode*.zip" del /q WriteLogRunMode*.zip
	7z a %zip% install.bat uninstall.bat readme.txt
	if exist Install\bin\Release (
		pushd Install\bin\Release
		7z a ..\..\..\%zip% ConfigureShortcuts.exe* CopyShortcutAssembly.exe*
		popd
		if exist bin\Release (
			pushd bin\Release
			7z a ..\..\%zip% WriteLogRunMode.dll
			popd
			move /y %zip% DownloadInstaller
		) else (
		echo Must build Release Configuration !
	)
) else (
	echo Must build Install Release configuration!
	)