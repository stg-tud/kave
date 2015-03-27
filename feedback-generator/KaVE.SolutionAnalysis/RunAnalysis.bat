@ECHO OFF
REM Before running this script, install ReSharper Command Line Tools (CLT) are installed via chocolatey:
REM 1) Install chocolatey:
REM    $> @powershell -NoProfile -ExecutionPolicy unrestricted -Command "iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin
REM 2) Install CLT:
REM    $> choco install resharper-clt.portable -version 8.2.0.2151

inspectcode.exe ..\..\KaVE.Feedback.sln /project=KaVE.Model /o=tmp.xml /plugin=bin\Debug\KaVE.SolutionAnalysis.dll
PAUSE