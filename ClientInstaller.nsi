!include "FileFunc.nsh"

Name "FTP.NET Client"

OutFile "FTP-Client-Installer.exe"

InstallDir "$PROGRAMFILES\FTP Client"

LicenseData "LICENSE"

InstallDirRegKey HKLM "Software\FTP.NET" "Install_Dir"

RequestExecutionLevel admin

Page license
Page directory
Page instfiles

Section
	
	RMDir /r $INSTDIR
	SetOutPath $INSTDIR
  
	File /r "Client\bin\Release\net6.0\"
	
	CreateShortCut "$SMPROGRAMS\FTP Client.lnk" "$INSTDIR\Client.exe"
	CreateShortCut "$DESKTOP\FTP Client.lnk" "$INSTDIR\Client.exe"
	
	; Write the installation path to registry
	WriteRegStr HKLM "SOFTWARE\FTP.NET" "Install_Dir" "$INSTDIR"
	
	; Registry keys for Programs and Features
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "DisplayName" "FTP.NET Client"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "DisplayVersion" "1.0.0"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "Publisher" "FTP Group 5"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "HelpLink" "https://github.com/MattMoradi/FTP.NET/issues"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "UninstallString" '"$INSTDIR\uninstall.exe"'
	 
	${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
	IntFmt $0 "0x%08X" $0
	
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "EstimatedSize" "$0"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET" "NoRepair" 1
	
	WriteUninstaller "uninstall.exe"

SectionEnd

Section "Uninstall"

	; Delete registry keys
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FTP.NET"
	DeleteRegKey HKLM "SOFTWARE\FTP.NET"
  
	RMDir /r $INSTDIR
	Delete "$SMPROGRAMS\FTP Client.lnk"
	Delete "$DESKTOP\FTP Client.lnk"

SectionEnd