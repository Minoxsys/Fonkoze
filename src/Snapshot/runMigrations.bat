@echo off

cd ".\packages\FluentMigrator.Tools.1.0.1.0\tools\AnyCPU\40"

SET param1Ok=0
IF "%1" == "migrateup" (
	SET param1Ok=1
) ELSE (
	IF "%1" == "migratedown" (
		SET param1Ok=1
	) ELSE (
		IF "%1" == "migrateuptests" (
			SET param1Ok=1
		)))
			
SET param2Ok=0 
IF "%2" == "dev" (
	SET param2Ok=1
) ELSE (
	IF "%2" == "staging" (
		SET param2Ok=1
	) ELSE (
		IF "%2" == "production" (
			SET param2Ok=1
		)))

IF %param1Ok% == 1 (
pause
	IF %param2Ok% == 1 (
	pause
		%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "..\..\..\..\..\MigrationsMSBuild\%2\%1.msbuild"
	) ELSE ( 
	pause
		GOTO ERROR_PARAM2
	)
) ELSE (
pause
	GOTO ERROR_PARAM1
)

:ERROR_PARAM1
ECHO "Please supply one of `migrateup`, `migratedown`, `migrateuptests` scripts to run for the first param"
GOTO END

:ERROR_PARAM2
ECHO "Please supply one of `dev`, `staging`, `production` values to specify deploy environment for the second param"
GOTO END

:END

cd "..\..\..\..\.."