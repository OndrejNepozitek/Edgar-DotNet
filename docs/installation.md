---
title: Installation
sidebar_label: Installation
---

### Prerequisites
- .NET 4.6.1 installed

### Using the precompiled binaries on Windows
Donwload binaries from the [latest release](https://github.com/OndrejNepozitek/ProceduralLevelGenerator/releases/latest). Run _GUI/GUI.exe_ to start the main GUI application. Or place the content of  _ProceduralLevelGenerator_ directory next to the main exe file of your _.NET_ application and then include all dlls (except the BoostWrapper.dll which is an unmanaged C++ dll) to use the generator from your application.

### Compiling the source code
Open the _MapGeneration.sln_ file in Visual Studio. Compile the _GUI_ project to get GUI binaries or compile the _MapGeneration_ project to get the layout generator as a _DLL_. The target platform must be set to _x86_.