# st-project-name

(c)2023 by CodeDog Sp. z o.o.

This tool renames STM32CubeIDE / TouchGFX projects.

In order to do so, the project name must be changed in several files.
This tools finds the necessary files, lines that needs the changes applied
and applies the changes.

After renaming, the project should be re-imported into STM32CubeIDE workspace.

**CAUTION:** Always use on version controlled project to be able to discard
the changes if anything goes wrong. This tool renames and modifies project
files so always have a backup before using this tool on a project.

## Usage

### Displaying project information

Go to the main project directory like:
```
cd D:\Source\Embedded\Templates\STM32H745I-DISCO
```

To just see the some informations about the project:
```
st-project-name
```
This would display information like this:
```
PROJECT FILES SUMMARY:

.project path: ".\STM32CubeIDE\.project"
.ioc path: ".\STM32H745I-DISCO.ioc"
.touchgfx path: ".\CM7\TouchGFX\STM32H745I-DISCO.touchgfx"
.touchgfx.part path: ".\CM7\TouchGFX\ApplicationTemplate.touchgfx.part"
target.config path: ".\CM7\TouchGFX\target.config"

PROJECT CONTENT SUMMARY:

.ioc name: STM32H745I-DISCO
Project name: STM32H745I-DISCO
Project link name: STM32H745I-DISCO.ioc
Project link location: PARENT-1-PROJECT_LOC/STM32H745I-DISCO.ioc
TouchGFX application name: STM32H745I-DISCO
TouchGFX project file: ../../STM32H745I-DISCO.ioc
TouchGFX .part application name: STM32H745I-DISCO
TouchGFX .part project file: ../../STM32H745I-DISCO.ioc
TouchGFX target config project file: ../../STM32H745I-DISCO.ioc
```

If any issues are found they will be displayed at the end of the summary.

### Renaming the project

Go to the main project directory like:
```
cd D:\Source\Embedded\Templates\STM32H745I-DISCO
```

Use a command like this to rename the project:
```
st-project-name STM32H745_TEMPLATE
```

If the directory contains project and the new project name is valid
the program will perform changes returning an output like:
```
Renaming .ioc file...OK.
Changing the STM32CubeIDE project name...OK.
Changing the STM32CubeIDE project link name...OK.
Changing the STM32CubeIDE project link location URI...OK.
Changing TouchGFX main project name...OK.
Changing TouchGFX main project file...OK.
Changing TouchGFX part project name...OK.
Changing TouchGFX part project file...OK.
Changing TouchGFX target.config project file...OK.
Saving changes in project file...OK.
Saving changes in TouchGFX .project file...OK.
Saving changes in TouchGFX .part file...OK.
Saving changes in TouchGFX target.config file...OK.
Renaming .touchgfx file...OK.
Renaming .touchgfx.part file...OK.
DONE.
```

## Disclaimer

The author doesn't guarantee this software will work as intended.
The code was tested on several projects and it worked.
Always have backup before modifying your projects.