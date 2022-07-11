# JollyWizard.Quantower.IndicatorDeploy
A project to help deploy indicator projects for the Quantower platform.

## Overview

This project builds an installer system for Quantower Indicator Projects.

As a dependency it enables build tasks:

* Automatically detect running Quantower instances and include their assemblies as references.
  * This prevents hardcoding local paths into repository like Quantower Algo.
* Automatically deploy builds to the current Quantower instance.
* Package Builds with the installer for distribution.

## Usage

Main documentation will be updated when the uploading of sibling projects is complete.

## Required

For the installer, Dotnet 4.8 is required. It is the same version used by Quantower Algo project templates, so it is should be bundled with Quantower.

For development tools, it currently requires either Quantower to be running or that the `QuantowerRoot` environment variable is set to the Quantower folder with `Start.lnk`.
