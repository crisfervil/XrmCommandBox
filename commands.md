# metadata-export

Metadata Export tool for dynamics 365

## Options

### -f, --file
File name where to export the data to



### -e, --entity
Name of the entity which metadata you want to export



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





## Examples
Exports the Account metadata to the accounts.json file
```
xrm metadata-export --file AccountLeads.xml --entity account --connection Url=http://myCrmServer/myorg
```

# wr-sync

Synchronizes Web Resoources with local directories

## Options

### -w, --watch
Initiates the command in watch mode



### -n, --name-filters
Only the web resources beggining with thesee filters are going to be included



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





# associate

Imports N to N relationshipts to CRM

## Options

### -f, --file
File containing the data to import

The input file should contain a data table named as the intersect entity name, with at least one column for each of the related records. Any additional attributes will be ignored. 
            You can use the lookup tool to resolve the GUIDs of the key attributes

The intersect entity name for the relationship between Accounts and Leads is accountsleads, so the input xml should be:
            
            ``` xml
            <datatable name="accountsleads">
                <row>
                    <accountid>A1E29A4E-78CE-4164-BC26-A93D8B449F87</accountid>
                    <leadid>1B3C099D-BBDC-4C9E-8668-BF53C5871A2B</leadid>
                    <accountid1.name>contoso</accountid1.name>
                    <leadid1.name>contoso lead</leadid1.name>
                </row>
            </datatable>
            ```
            
### -e, --continue-on-error
Continue if there&#39;s an error while processing the command



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





## Examples
Associate the Accounts and Leads contained in the AcccountLeads.xml file
```
xrm associate --file AccountLeads.xml --continue-on-error False --connection Url=http://myCrmServer/myorg
```

# lookup

Updates a specific column from a data table querying data in CRM

## Options

### -f, --file
File containing the data table with the data



### -l, --column
Required. Name of the column to lookup



### -n, --entity
Name of the entity where search for the data



### -m, --match-attributes
Attributes used to know if the record exists. Default is display attribute



### -y, --match-columns
Columns containing the with the data to lookup. The number of columns must match the number of matching attributes



### -e, --continue-on-error
Continue if there&#39;s an error while processing the command



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





# publish-all

Publishes all existing customizations in the environment

## Options

### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





# delete

Delete all the records returned by the specified query

## Options

### -q, --fetch-query
Fetch query to retrieve the records to delete



### -e, --continue-on-error
Continue if there&#39;s an error while processing the command



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





# export

Exports an entity or query from CRM to a file

## Options

### -r, --row-number
Adds the row number to each exported record



### -f, --file
Path of the file where to save the exported data



### -e, --entity
Name of the entity you want to export



### -q, --fetch-query
Fetch query to retrieve the records to export



### -s, --page-size
Number of records to retrieve from a page



### -p, --page
Page of records to retrieve



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





## Examples
Export all the existing accounts to an Accounts.xml file
```
xrm export --file Accounts.xml --entity account --connection DEV
```
Export all the existing contacts returned by a fetch query to the contacts.xml file
```
xrm export --file Contacts.xml --connection DEV --config-file ContactsQuery.xml
```

# import

Imports information from a file to Dynamics

## Options

### -f, --file
File containing the data to import



### -e, --continue-on-error
Continue if there&#39;s an error while processing the command



### -m, --match-attributes
Attributes used to know if the record exists. Default is id attribute



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





# solution-import

Imports the specified solution .zip file into CRM

## Options

### -s, --solution-file
Required. .zip file containing the solution to import



### -a, --async
Indicates wether the import should be performed asynchronously



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





# solution-export

Exports the specified solution to a .zip file

## Options

### -s, --solution-name
Required. Unique name of the solution to export



### -c, --connection
Required. Connection string, or name of a connection string to use



### -f, --config-file
Xml file containing the command options



### -l, --log-level
Sets the current logging output. Can be Debug, Info, Error





## Examples
Export mysolution to the mysolution.zip file in the current directory
```
xrm solution-export --solution-name mysolution --connection TestEnvironment
```

