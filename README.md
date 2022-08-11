# FTP.NET
FTP.NET is a free and open source command line interface FTP client written in .NET

## Requirements
- .NET 6.0 SDK: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- NSIS Compiler (for packaging): https://nsis.sourceforge.io/

## Building in Visual Studio Community
- Open the `FTP.sln` solution file
- Build > Build Solution, then Run or Debug the solution using the `Debug | Any CPU` configuration.

## Building in Command Line Interface
- `dotnet build FTP.sln`
- `cd Client`
- `dotnet run`

## Packaging in Nullsoft Install System
- Clean and build the solution with the `Release | Any CPU` configuration.
- Compile `ClientInstaller.nsi` using the NSIS compiler
- Test installer on Windows machine

##
![UploadDemo](https://user-images.githubusercontent.com/14935352/183861375-afb28bac-3f0a-4355-b421-dc787e111ed6.gif)
