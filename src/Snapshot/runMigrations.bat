@echo off
cd ".\packages\FluentMigrator.Tools.1.0.1.0\tools\AnyCPU\40"

IF "%1" == "" (
	ECHO "Please supply one of `migrateup`, `migratedown`, `migrateuptests` scripts to run"
)
IF "%1" == "migrateup" (
 %WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild %1.msbuild
)
IF "%1" == "migratedown" (
 %WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild %1.msbuild
)
IF "%1" == "migrateuptests" (
 %WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild %1.msbuild
)
cd "..\..\..\..\.."