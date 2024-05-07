# Client File Formats

### Setting up a client format

The client format is the definition record that is used to identify how the client has
sent in there file. The system will use the records to determine where to get the debtor
information.

1. File --> Drops --> Client File Format
2. Select the +File Format in the upper right corner of the screen or select the pencil icon
for the client format you want to edit. (If editing skip to step 5.)
3. If Adding a new Client format fill out the following info on the left side of the window.
	- File Format Name: [Required]
	- File Start Line: [This is the line the records start on.]
	- Client Debtor Number Column: [This is required because the application will use this column
	to identify how many records. note this should be a column used for every account, it should 
	not be blank.]
4. Click Add. By Clicking add you will add the format to the database this is required prior to 
adding the detail information. (The application needs to know the ID of the format, and wont get this 
until the format is added to the db.) Click the edit button for the File Format you just created.
5. On the right side is the file format detail information. Fill out the following fields.
	- Field: (These are predefined fields from the debtor record.)
	- File Column: This is the location of the field. This should be a Alphabetic character even if
	the file is a csv. The application will open both xls, xlsx and csv files with Excel and read the 
	file that way. So make sure this is a alphabetic character.
	- Column type: options are string, int, double, datetime, long and StateZip (note int, long, StateZip
	will be deprecated in the next version, They currently do not change the process but will cause the 
	system to error.)
	- Special Case: This field is used to split data. the options are split1, split2, split3, split4. 
	These "splits" identify the information you want to retrieve from the field. For Example The field you
	want to split shows doe, john j. You need the DebtorFirstName you would enter the column and select the 
	split2 special case. for the DebtorLastName you would enter the same column and select the split1. This 
	will get the information before the comma.

	**Note: Currently only the comma is used to split a column.**

6. Click Add File Format Detail button. If the add is successful you should see the box populate on the 
left side of the window.
7. Continue through the rest of the fields required for the client.


