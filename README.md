[![Windows Build](https://ci.appveyor.com/api/projects/status/github/crisfervil/xrmcommandbox?svg=true)](https://ci.appveyor.com/project/crisfervil/xrmcommandbox)

# XrmCommandBox Overview

Command line tool for Dynamics 365. Because the command line is cool again!


# How to use it

[Download the latest release of the program from GitHub](https://github.com/crisfervil/XrmCommandBox/releases). Unzip the contents to any folder and open a console in that directory.

Type xrm and the command you want to use.

``` console
xrm [command] [--option optionvalue]
````

To view a list of available commands type help

``` console
xrm help
```

To view a list of available options for each command type the command and --help
```console
xrm command --help
``` 

You can add it to your project as a nuget package too
```console
PM > Install-Package XrmCommandBox
```

Then, it will be available in the following directory, related to your project

```
..\packages\XrmCommandBox.X.X.X\tools\xrm.exe
```

(Replace X.X.X by the version of XrmToolbox Installed)

## Available Commands

### Associate

Imports N to N relationshipts to CRM

Options: 

``` console
  -f, --file                 File containing the data to import

  -e, --continue-on-error    Continue if there's an error while processing the command

  -c, --connection           Required. Connection string, or name of a connection string to use

  -l, --log-level            Sets the current logging output. Can be Debug, Info, Error
```

### Lookup

Updates a specific column from a data table querying data in CRM

Options:

``` console
 -f, --file                 File containing the data table with the data

 -l, --column               Required. Name of the column to lookup

 -n, --entity               Name of the entity where search for the data

 -m, --match-attributes     Attributes used to know if the record exists. Default is display attribute

 -y, --match-columns        Columns containing the with the data to lookup. The number of columns must match the
                            number of matching attributes

 -e, --continue-on-error    Continue if there's an error while processing the command

 -c, --connection           Required. Connection string, or name of a connection string to use

 -f, --config-file          Xml file containing the command options

 -l, --log-level            Sets the current logging output. Can be Debug, Info, Error
```

### Publish All

Publishes all existing customizations in the environment

Options:

```
  -c, --connection     Required. Connection string, or name of a connection string to use

  -f, --config-file    Xml file containing the command options

  -l, --log-level      Sets the current logging output. Can be Debug, Info, Error
```

### Delete

Delete all the records returned by the specified query

Options:

``` console
  -q, --fetch-query          Fetch query to retrieve the records to delete

  -e, --continue-on-error    Continue if there's an error while processing the command

  -c, --connection           Required. Connection string, or name of a connection string to use

  -f, --config-file          Xml file containing the command options

  -l, --log-level            Sets the current logging output. Can be Debug, Info, Error

```

### Export

Exports an entity or query from CRM to a file

Options:

``` console
  -r, --row-number     Adds the row number to each exported record

  -f, --file           Path of the file where to save the exported data

  -e, --entity         Name of the entity you want to export

  -q, --fetch-query    Fetch query to retrieve the records to export

  -s, --page-size      (Default: 5000) Number of records to retrieve from a page

  -p, --page           Page of records to retrieve

  -c, --connection     Required. Connection string, or name of a connection string to use

  -f, --config-file    Xml file containing the command options

  -l, --log-level      Sets the current logging output. Can be Debug, Info, Error
```

Example:

``` console
xrm export --connection DEV --entity account --file Accounts.xml --log-level Info --page 1 --page-size 5000
```

### Import

Imports information from a file to Dynamics

Options:

```console
  -f, --file                 File containing the data to import

  -e, --continue-on-error    Continue if there's an error while processing the command

  -m, --match-attributes     Attributes used to know if the record exists. Default is id attribute

  -c, --connection           Required. Connection string, or name of a connection string to use

  -f, --config-file          Xml file containing the command options

  -l, --log-level            Sets the current logging output. Can be Debug, Info, Error
```


### Solution Import

Imports the specified solution .zip file into CRM

Options:

```console
  -s, --solution-file    Required. .zip file containing the solution to import

  -a, --async            Indicates wether the import should be performed asynchronously

  -c, --connection       Required. Connection string, or name of a connection string to use

  -f, --config-file      Xml file containing the command options

  -l, --log-level        Sets the current logging output. Can be Debug, Info, Error
```

### Solution Export

Export the specified solution to a .zip file

Options:

``` console
  -s, --solution-name    Required. Unique name of the solution to export

  -c, --connection       Required. Connection string, or name of a connection string to use

  -f, --config-file      Xml file containing the command options

  -l, --log-level        Sets the current logging output. Can be Debug, Info, Error
```