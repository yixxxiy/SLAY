—————EasyExcel—————
EasyExcel is an easy and fast tool for using data from excel in Unity.
It imports excel files, converts each sheet to a ScriptableObject and a C# file. 
It provides simple menus and settings, APIs and full examples.

Using:
  EPPlus - A .NET library that reads and writes Excel files using the Office Open XML format (xlsx). https://github.com/JanKallman/EPPlus.
  EasyExcel only uses EPPlus to parse excel files at editor time, so at runtime it does not depend on EPPlus.

Support:
  1534921818@qq.com

—————Features—————
 •  As easy as a click
 •  Supports Unity 5 - Unity 2018
 •  Generate C# class file and data asset file for each sheet
 •  Use integer or string Key
 •  Specify Key column for each Sheet
 •  Easy and flexible settings
 •  Full examples

—————Settings—————
  Click the menu Tools->EasyExcel->Settings to see details.

—————Import excel files—————
  Click the menu Tools->EasyExcel->Import, then select files' folder and wait.

—————More examples—————
 The xlsx files in EasyExcel/Example/ExcelFiles shows the formats and rules：
 SupportedTypesExample.xlsx: the Sheet SupportedTypes shows the supported types, int float string bool double long and their array.
 KeyColumnExample.xlsx: shows specifing the Key column of a Sheet. For example Name:key
 MultiSheetsExample.xlsx show multiple sheets
 EmptyColumnExample.xlsx show empty column, can be used for writing comments
 EmptySheetExample.xlsx show empty sheet, can be used for writing comments

—————Run the example—————
 Click Tools/EasyExcel/Import and select EasyExcel/Example/ExcelFiles. After few seconds .cs and .asset files will be generated.
 The generated paths by default are EasyExcel/Example/AutoGenCode, Resources/EasyExcelGeneratedAsset. They can be set in Tools/EasyExcel/Settings.
 Open scene ExampleLoadData and play, you will see the data imported.
 ExampleLoadData.cs shows how to initialize and look up data with keys.

If you have any problem please contact me 1534921818@qq.com


