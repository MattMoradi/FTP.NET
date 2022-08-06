Name "FTP.NET Client"

OutFile "FTP-Client-Installer.exe"

InstallDir "$PROGRAMFILES\FTP Client"

RequestExecutionLevel admin

Page directory
Page instfiles

Section
	
	RMDir /r $INSTDIR
	SetOutPath $INSTDIR
  
	File /r "Client\bin\Release\net6.0\"
	
	CreateShortCut "$SMPROGRAMS\FTP Client.lnk" "$INSTDIR\Client.exe"
	CreateShortCut "$DESKTOP\FTP Client.lnk" "$INSTDIR\Client.exe"
	
	WriteUninstaller "uninstall.exe"

SectionEnd

Section "Uninstall"

	RMDir /r $INSTDIR
	Delete "$SMPROGRAMS\FTP Client.lnk"
	Delete "$DESKTOP\FTP Client.lnk"

SectionEnd