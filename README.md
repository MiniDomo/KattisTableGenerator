# Kattis Table Generator
Easily generate a table in markdown showcasing your solutions to problems on [Kattis](https://open.kattis.com/)!  
[Download v3.0.3](https://github.com/minidomo/KattisTableGenerator/releases/tag/v3.0.3)  
Previous versions that were not in this repository
- [1.0.0](https://github.com/MiniDomo/Kattis/tree/v1.0.0/KattisTableGenerator)
- [2.0.0](https://github.com/MiniDomo/Kattis/tree/v2.0.0/KattisTableGenerator)

## Prerequisites
- [.NET Core 3.1](https://dotnet.microsoft.com/download) (or compatible) installed

## Setting Up `config.json`
Before running the program, `config.json` must be configured.  
Initially the `config.json` should look like this:
```json
{
    "Ignore": [],
    "Url": [],
    "Folders": [],
    "Base:" []
}
```
The `Ignore` property is a String array that can be filled with the names of files, directories, or extensions that are to be ignored during the process. By default, the program ignores extensions not found in `extensions.json`, which can be modified by the user.

The `Url` property is a String array that can be filled with the GitHub urls of where your solutions are located. Urls that are not from github are ignored.  

The `Folders` property is an Object array where each object should have the properties `BaseUrl` (String) and `Paths` (String array). This causes the program to search a given local directory on the user's computer and finds Kattis solutions to hyperlink it to a given GitHub url of where their solutions are uploaded. This gives better performance than the `Url` property as it does not require any online requests. Some important details are that `BaseUrl` must be a GitHub url of where your solutions are located online and `Paths` consists of existing directories on your local computer.

The `Base` property is a String array that should be filled with a path to a markdown file specifying a *header* for the generated `README.md`. This markdown file should contain all information you'd like to display prior to the table appearing on your `README.md`. If this is property is left empty, a default header will be generated.

## Other Information/Limitations
- The generated table will be in a file called `README.md`.
- The program will process items in `Folders` first and then `Url` regardless of position in `config.json`.
- Files must be named as the Kattis problem ID, such as `hello.java` for [Hello World!](https://open.kattis.com/problems/hello). However, if a sub-directory has the name as the Kattis problem ID, then the main file, the one to be hyperlinked, can be called either the same Kattis problem ID or `main`; otherwise it will not be considered in the table.
- If the program encounters a problem that has been already solved in the same programming language and was added to the table, it will not re-add or create a duplicate in the table.
- When checking a directory, the program will also check sub-directories (if not ignored in `config.json`) but only of the given directory; it will not check directories within sub-directories. This applies for both uses of `Url` and `Folders`.  

## `config.json` Example
In the example below, all files and directories that are named `hello` will be ignored. In addition, `abc.java` and files ending with `.cs` will be ignored. To view an example of a table generated via this program, [check here](https://github.com/minidomo/Kattis).
```json
{
    "Ignore": [
        "hello",
        "abc.java",
        ".cs"
    ],
    "Url": [
        "https://github.com/MiniDomo/Kattis/tree/master/Java"
    ],
    "Folders": [
        {
            "BaseUrl": "https://github.com/minidomo/Kattis/tree/master/C%2B%2B",
            "Paths": [
                "C:\\Kattis\\C++"
            ]
        },
        {
            "BaseUrl": "https://github.com/minidomo/Kattis/tree/master/C%23",
            "Paths": [
                "C:\\Kattis\\C#"
            ]
        }
    ],
    "Base": ["C:\\Kattis\\BASE_README.md"]
}
```

## Running
To obtain faster runtimes when generating the table, generate all current Kattis problem IDs and Names. You only need to do this once unless new problems are added to Kattis. It is not recommended to manually edit `KattisMapping.txt`. To generate the problem IDs and Names in `KattisMapping.txt`, use the `--map` flag:
```shell
KattisTableGenerator --map
```
To generate the table, type the following in command prompt:
```shell
KattisTableGenerator
```

## Building
To build the code, type the following in command prompt:
```shell
git clone https://github.com/MiniDomo/KattisTableGenerator.git
cd KattisTableGenerator
dotnet restore
```