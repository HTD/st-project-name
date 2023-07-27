# st-project-name

*(c)2023 by CodeDog Sp. z o.o.*

This tool properly renames STM32CubeIDE / TouchGFX projects.

In order to do so, the project name must be changed in several files.
This tools finds the necessary files, lines that needs the changes applied
and applies the changes.

After renaming, the project should be re-imported into STM32CubeIDE workspace.

**CAUTION:** It is strongly recommended to use with Git version control!

When `-c|--commit`  option is used, all uncommitted changes, including
untracked files will be committed with the default commit message.

You can provide a custom commit message with `-m` option.

If the project root directory is NOT under Git version control,
a new Git repository will be initialized, with the default `.gitignore` file
and all relevant project files added. The changes will be committed as
previously mentioned.

Don't use `-c` or `-m` options if you prefer to manage the version control
manually by yourself.

Of course, using `-c` and `-m` options requires `Git` to be installed
and accessible from the system path. If `git` command is not available,
the `commit` step will be just skipped.

This tool is also useful to start new repositories for new TouchGFX projects.

## Prerequisites

 - .NET version 7 or higher.
 - Git (optional).

## Installation

In current version - fully manual. Just copy the executable file to a
directory that is in the OS search path.

This program should work on most Windows and Linux versions.

## Usage

#### Syntax

> **st-project-name** *<--help |--commit |-m> <project_root_path> <new_project_name>*

#### Parameters

 - `project_root_path` - A path to the root directory of the STM32CubeIDE /
   TouchGFX project. It is **optional**, if not provided, the current directory
   will be used.

- `new_project_name` - A valid STM32CubeIDE project name. It is also
  **optional**, if not provided, a project name analysis will be performed on
  the target project and possible issues listed.




#### Options

 - `--commit|-c` - commits all uncommitted / untracked changes before
   proceeding.<br>
   If the `Git` repository is not found, it is created and initialized with
   the default `.gitignore` file and default commit message.
 - `-m` <commit_message>* - provides a commit message to `-c` option, works
   exactly like `-c`, but the custom commit message is used.


#### Examples

##### To show project name analysis:
```
cd D:\Sources\Embedded\MyProject
st-project-name
```
or
```
st-project-name D:\Sources\Embedded\MyProject
```
##### To rename a project:
```
cd D:\Sources\Embedded\MyProject
st-project-name MyNewProject
```
or
```
st-project-name D:\Sources\Embedded\MyProject MyNewProject
```

##### To rename a project with a commit before applying:
```
cd D:\Sources\Embedded\MyProject
st-project-name -c MyNewProject
```
or
```
st-project-name D:\Sources\Embedded\MyProject -c MyNewProject
```

##### To rename a project with a commit and custom message before applying:
```
cd D:\Sources\Embedded\MyProject
st-project-name -m "Before rename." MyNewProject
```
or
```
st-project-name D:\Sources\Embedded\MyProject -m "Before rename." MyNewProject
```



## Disclaimer

The author doesn't guarantee this software will work as intended.
The code was tested on several projects and it worked.
Always have backup before modifying your projects.

Please report any issues found on GitHub.