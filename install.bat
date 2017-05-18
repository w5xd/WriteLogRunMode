if exist ConfigureShortcuts.exe (
ConfigureShortcuts WriteLogRunMode RunModeProcessor /install
) else (
  msg * "You need to Extract All to install"
)
