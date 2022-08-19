# JollyWizard.Quantower.Indicators.VolumeDeltaPack
A pack of Volume Delta related indicators for the quantower platform.

## Overview

This is an indicator pack for Quantower.

Indicators Included:

* Singular Volume Delta
* Cumulative Volume Delta

## Usage

Main documentation will be updated when the uploading of sibling projects is complete.

## Installation

### Release

Git the current release `.zip` from [github releases](https://github.com/JollyWizard/JollyWizard.Quantower.Indicators.VolumeDeltaPack/releases).

Unpack the release and look for the installer `.exe` ([JollyWizard.Quantower.IndicatorDeploy.exe](https://github.com/JollyWizard/JollyWizard.Quantower.IndicatorDeploy)).

Run the installer and `enter` through the prompts. (Less nerdy version in the future).

* Quantower must be running on the first install, or you must have nerd powers and read the other directions.
* `Run as Administrator` may be required. Depends on Quantower install location and your windows setup.

### Dev Build

If you build the project with VS2022, it should install the indicator.

## Manual Install

You are looking for the `{Quantower Folder}\Settings\Scripts\Indicators` folder. Anything you put in there it will autodetect if it is a valid indicator with all it's `.dll` parts. 

## Dependency

For the installer, Dotnet 4.8 is required. It is the same version used by Quantower Algo project templates, so it is should be bundled with Quantower.

The developer dependencies are installed through nuget and you do not need any Visual Studio extensions.
