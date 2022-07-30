# IsoDataManager

IsoDataManager is a project for managing AutoCAD .dwg files.

## Installation

Use NuGet Manager to install AutoCAD libraries: 

- AutoCAD.NET
- AutoCAD.NET.Core
- AutoCAD.NET.Model

## Usage

In Visual Studio:
- Press Build Button
- Press Rebuild Solution

To use created .dll file open AutoCAD, then type the command ```NETLOAD```, then choose created .dll file. After that, you will be able to use 2 commands:
- VTReadFromIsoRN
- VTWriteToIsoRN

```VTReadFromIsoRN``` - creates an Excel file in the directory of the opened .dwg file. This Excel contains data about the blocks that are noted in the ```Settings.cs```. 

```VTWriteToIsoRN``` - uploads data from the Excel file to the .dwg file.

You can store multiple .dwg files in the directory, so the Excel file will contain data about all of them. 