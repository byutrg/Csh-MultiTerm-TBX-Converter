# MultiTerm-to-TBX-Converter

The following is the first build of the MultiTerm-to-TBX Converter, combining the MultiTerm Mapping Wizard and the MT2TBX Converter

The MultiTerm Mapping Wizard was developed as a means of "mapping" invalid data categories in the XML output of the MultiTerm tool by SDL
Trados. The Mapping Wizard originally output a JSON file with the mappings designated by the user through the GUI.

The MT2TBX Converter was designed seperately to accept a JSON mapping file and the XML file to be converted. The converter applied the
mapping to the XML file and returned a valid TBX file to the user.

In this new tool, the two are combined so that the user has the option to solely generate a mapping file, solely convert by using a
mapping file, or generate both a mapping file and a converted file. 

This project is not associated with SDL in anyway.

BYU Translation Research Group
