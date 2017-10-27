@echo off
REM This tool requires node and handlebarsjs-cli in global mode (https://github.com/meteorbites/handlebarsjs-cli)
echo Running doc generator...
bin\debug\docgenerator ..\XrmCommandBox\bin\Debug\Xrm.exe

echo Transforming template...
mustache doc.json template.tpl > doc.md

echo done