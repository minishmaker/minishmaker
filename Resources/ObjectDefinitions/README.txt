User changes should always go in the UserObjectDefinitions folder in the project folder, that way new objects are also shareable.
Examples can be found by looking at the ObjectDefinitions folder of the editor, any changes to these will NOT affect the editor.
Any filename, foldername or folder depth can be used to keep it organized, the editor will crawl through the folders for any .xml files.

Giving an entry of a type the same name as an already existing entry of that type will overwrite that entry, the exception to this is enums, the values inside those will be added or overwritten instead.
Instead of overwriting filters to add more children parentNames can be used to add them in from the entry itself.

While filters, enums and elementSets are seperated in the examples, they can all be put together in a single folder, this just felt more organized.