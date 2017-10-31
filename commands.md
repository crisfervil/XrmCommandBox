# associate

Imports N to N relationshipts to CRM

## Options

Name | Description
--- | ---
-f, --file | File containing the data to import
-e, --continue-on-error | Continue if there&#39;s an error while processing the command
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# lookup

Updates a specific column from a data table querying data in CRM

## Options

Name | Description
--- | ---
-f, --file | File containing the data table with the data
-l, --column | Name of the column to lookup
-n, --entity | Name of the entity where search for the data
-m, --match-attributes | Attributes used to know if the record exists. Default is display attribute
-y, --match-columns | Columns containing the with the data to lookup. The number of columns must match the number of matching attributes
-e, --continue-on-error | Continue if there&#39;s an error while processing the command
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# publish-all

Publishes all existing customizations in the environment

## Options

Name | Description
--- | ---
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# delete

Delete all the records returned by the specified query

## Options

Name | Description
--- | ---
-q, --fetch-query | Fetch query to retrieve the records to delete
-e, --continue-on-error | Continue if there&#39;s an error while processing the command
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# export

Exports an entity or query from CRM to a file

## Options

Name | Description
--- | ---
-r, --row-number | Adds the row number to each exported record
-f, --file | Path of the file where to save the exported data
-e, --entity | Name of the entity you want to export
-q, --fetch-query | Fetch query to retrieve the records to export
-s, --page-size | Number of records to retrieve from a page
-p, --page | Page of records to retrieve
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# import

Imports information from a file to Dynamics

## Options

Name | Description
--- | ---
-f, --file | File containing the data to import
-e, --continue-on-error | Continue if there&#39;s an error while processing the command
-m, --match-attributes | Attributes used to know if the record exists. Default is id attribute
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# solution-import

Imports the specified solution .zip file into CRM

## Options

Name | Description
--- | ---
-s, --solution-file | .zip file containing the solution to import
-a, --async | Indicates wether the import should be performed asynchronously
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

# solution-export

Export the specified solution to a .zip file

## Options

Name | Description
--- | ---
-s, --solution-name | Unique name of the solution to export
-c, --connection | Connection string, or name of a connection string to use
-f, --config-file | Xml file containing the command options
-l, --log-level | Sets the current logging output. Can be Debug, Info, Error

