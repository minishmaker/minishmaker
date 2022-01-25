//hjson, because comments are a must to explain stuff
	//BE AWARE THAT THIS FILE SHOULD NOT BE EDITED AS IT CAN BE UPDATED
	//to make changes use the userListSettings.hjson file
	//any entry in there will either overwrite the ones in here if it has the same name or get added if it does not
	
	
	//targetType is what should be looked at to determine the path through the filters
	//targetPos is where the data is located (starts at 1)
	//should a targetType not be defined it will look at the defaultChildTargetType of the filter above it
	
	//values for (defaultChild)TargetType are list, bit, byte, short, tri, int
		//list is the list index in the application, needs no targetPos, first list is 1
		//bit is 1 bit, (defaultChild)TargetPos is what bit to look at(9 is the 2nd byte its 1st bit)
		//byte is 1 byte, pos is the byte to look at
		//short is 2 bytes, pos is the byte to start at(5 get the combined value of the 5th and 6th bytes)
		//tri (for lack of a better name) is 3 bytes, pos is the byte to start at
		//int is 4 bytes, pos is the byte to start at
	
	//targetValues hold the numbers to filter on in string form, if using hex the string should start with 0x
	//if there is no targetValues that filter will used when there is no match
	
	//elements contains all the form elements to show
	//element types are enum, text, number, hexNumber and bit
		//enum creates a dropdown list with all entries of an enum specified in the enum file,
		//text just creates a text label element
		//number creates a box for decimal number input
		//hexNumber creates a box for hexadecimal number input
		//bit creates a checkbox, valueType is ignored, valuePos is the bit position
	
		//valueType uses the values used in (default)TargetType other than list and adds the part type
		//part is x bits, x determined by valueLength, valuePos is the bit to start at
	
		//label is a bit of text that precedes the created element, required when making a text element
		//enumType is the name of the enum to use in the dropdown list, only used when making an enum element
		//column is the column to put the element in
		//row is the row to put the element in
	
	//childrenNames matches the given names with the filters in 'filters' and adds them to the filter.
	//childrenSetName matches the given name with a set of filters in 'filterSets' and adds them to the filter.
	//elementNames and elementSetName do the same as the above but for form elements
	
	//children contains more filter objects to filter on, once there is no more children the form elements of that filter and all its parents get used