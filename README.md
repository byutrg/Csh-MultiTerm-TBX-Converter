# MultiTerm-to-TBX-Converter

The following is the first build of the MultiTerm-to-TBX Converter, combining the MultiTerm Mapping Wizard and the MT2TBX Converter

The MultiTerm Mapping Wizard was developed as a means of "mapping" invalid data categories in the XML output of the MultiTerm tool by SDL
Trados. The Mapping Wizard originally output a JSON file with the mappings designated by the user through the GUI.

The MT2TBX Converter was designed seperately to accept a JSON mapping file and the XML file to be converted. The converter applied the
mapping to the XML file and returned a valid TBX file to the user.

In this new tool, the two are combined so that the user has the option to solely generate a mapping file, solely convert by using a
mapping file, or generate both a mapping file and a converted file. 

This project is not associated with SDL in anyway.

# Instructions to run

You must have .NET Framework >= 4.5.2 Runtime installed. ([You can download the runtime here](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net452).)

Once that is done, navigate to the latest release on the [Releases page](https://github.com/byutrg/Csh-MultiTerm-TBX-Converter/releases). Then download the .zip of the latest release.

![image](https://user-images.githubusercontent.com/5855659/185278180-57fdc84f-21bc-4156-b165-e494db38fa97.png)

Once the Zip has been downloaded to your machine, extract it somewhere on your computer. After extracting it, navigate to the newly extracted folder and run "MultiTermTBXConverter.exe" (double-clicking it should work). You may get a Windows message warning you about the tool not coming from a verified publisher. If this happens there is usually a "More info" -> "Run anyway" option you will need to click.

**Do not move the .exe outside of the extracted folder. It requires all of the DLLs in the folder to run.**

![image](https://user-images.githubusercontent.com/5855659/185278284-150d62af-9054-48f7-bbe4-25bb3aca9b9b.png)

BYU Translation Research Group
