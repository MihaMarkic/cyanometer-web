##########################################################################
# This is the Cake bootstrapper script for PowerShell.
# This file was downloaded from https://github.com/cake-build/resources
# Feel free to change this file to fit your needs.
##########################################################################

<#

.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.

.DESCRIPTION
This Powershell script will download NuGet if missing, restore NuGet tools (including Cake)
and execute your Cake build script with the parameters you provide.

.PARAMETER Script
The build script to execute.
.PARAMETER Target
The build script target to run.
.PARAMETER Configuration
The build configuration to use.
.PARAMETER Verbosity
Specifies the amount of information to be displayed.
.PARAMETER ShowDescription
Shows description about tasks.
.PARAMETER DryRun
Performs a dry run.
.PARAMETER SkipToolPackageRestore
Skips restoring of packages.
.PARAMETER ScriptArgs
Remaining arguments are added here.

.LINK
https://cakebuild.net

#>

[CmdletBinding()]
Param(
    [string]$Script = "appbuild.cake",
    [ValidateSet("GetVersion", "IncreaseVersion", "BuildImage")]
    [string]$Target,
    [string]$Configuration,
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity,
    [switch]$ShowDescription,
    [Alias("WhatIf", "Noop")]
    [switch]$DryRun,
    [switch]$SkipToolPackageRestore,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ScriptArgs,
    [string]$BuildVersion,
	[switch]$Pull,
	[switch]$Push,
	[string]$Dokreg
)
$ErrorActionPreference = 'Stop'

Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

 # Build an array (not a string) of Cake arguments to be joined later
$cakeArguments = @()
if ($Script) { $cakeArguments += "`"$Script`"" }
if ($Target) { $cakeArguments += "--target=`"$Target`"" }
if ($Configuration) { $cakeArguments += "--configuration=$Configuration" }
if ($Verbosity) { $cakeArguments += "--verbosity=$Verbosity" }
if ($ShowDescription) { $cakeArguments += "--showdescription" }
if ($DryRun) { $cakeArguments += "--dryrun" }
if ($BuildVersion) { $cakeArguments += "--buildVersion=$BuildVersion" }
if ($Pull) { $cakeArguments += "--pull=$Pull" }
if ($Push) { $cakeArguments += "--push=$Push" }
if ($Dokreg) { $cakeArguments += "--push=$Dokreg" }
$cakeArguments += $ScriptArgs

# Start Cake
Write-Host "Running build script..."
dotnet cake $cakeArguments
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
