@echo off

set m=%1

if "%m%" == "Debug" (
	copy "robots.dev.txt" "robots.txt"
)

if "%m%" == "Release" (
	echo "Release"
)