# Kattis Table Generator
Easily generate a table in markdown showcasing your solutions to problems on [Kattis](https://open.kattis.com/)!  
[Download](https://github.com/MiniDomo/KattisTableGenerator/releases/download/v2.1.0/KattisTableGenerator.zip)

## Prerequisites
- [.NET Core 2.2](https://dotnet.microsoft.com/download) (or compatible) installed

## Setup
Before running the program, `Config.txt` must be configured.  
Blank lines in `Config.txt` are ignored. In addition, there are 3 keywords:  
- `IGNORE` - Indicates that the upcoming lines are things to be ignored during the process.
  - Things that can be ignored: names of files, directories, extensions.
  - The program already ignores invalid extensions for Kattis. To see valid extensions, see [ValidExtensions.cs](https://github.com/MiniDomo/KattisTableGenerator/blob/master/static/ValidExtensions.cs).
  - In the example below, things that will be ignored when checking will be files that have the extension `.cs`, any file or directory with the name `hello` so something like `hello.java` or `hello.cpp` will be ignored, and the file `abc.cpp` will be ignored.
    ```
    IGNORE
    .cs
    hello
    abc.cpp
    ```
- `URL` - Indicates that the upcoming lines are urls that contain your solutions.
  - Only accepts github urls and ignores other urls.
- `FOLDER` - Indicates that the upcoming lines contain directory addresses to your solutions and links to hyperlink them.
  - Before adding directory addresses, a github url must be given to be able to hyperlink your solutions. Other urls will be ignored. After a valid url is given, upcoming directory addresses will be hyperlinked using that url. If a directory address is given without a url, it will be ignored.
  - In the example below, the program will read the folders Java, C#, and C++ and hyperlink any solutions found with their corresponding url above.
    ```
    FOLDER
    TO:https://github.com/MiniDomo/Kattis/tree/master/Java
    C:\Users\User\Documents\GitHub\Kattis\Java
    C:\Users\User\Documents\GitHub\Kattis\C#
    TO:https://github.com/MiniDomo/Kattis/tree/master/C%2B%2B
    C:\Users\User\Documents\GitHub\Kattis\C++
    ```
Here is a full example. In this example, all files and directories that are named `2048` or have the extension `.cpp` will be ignored. In addition, `abc.java` and `parking.cs` will be ignored.
```
FOLDER
TO:https://github.com/MiniDomo/Kattis/tree/master/C%2B%2B
C:\Users\User\Documents\Kattis\C++ Solutions

TO:https://github.com/MiniDomo/Kattis/tree/master/C%23
C:\Users\User\Documents\Kattis\C# Solutions
C:\Users\User\Documents\Kattis\Java Solutions

IGNORE
2048
abc.java
.cpp
parking.cs

URL
https://github.com/MiniDomo/Kattis/tree/master/C%2B%2B
https://github.com/MiniDomo/Kattis/tree/master/C%23
```

## Other Information/Limitations
- The program will check `FOLDER` first then `URL` regardless of position in `Config.txt`  
- Files must be named as the Kattis problem ID, such as `hello.java` for [Hello World!](https://open.kattis.com/problems/hello). However, if a sub-directory has the name as the Kattis problem ID, then the main file, the one to be hyperlinked, can be called either the same Kattis problem ID or `main`; otherwise it will not be considered in the table.
- If the program encounters a problem that has been already solved in the same programming language and was added to the table, it will not re-add or create a duplicate in the table.
- When checking a directory, the program will also check sub-directories (if not ignored in `Config.txt`) but only of the given directory; it will not check directories within sub-directories. This applies for both uses of `URL` and `FOLDER`.  

## Running
To generate the table, type the following in command prompt:
```shell
dotnet KattisTableGenerator.dll
```
To obtain faster runtimes in generating the table, generate all Kattis problem IDs and Names. To generate the problem IDs and Names in KattisMapping.txt, use the `-map` flag:
```shell
dotnet KattisTableGenerator.dll -map
```

## Building
To build the code, type the following in command prompt:
```shell
git clone https://github.com/MiniDomo/KattisTableGenerator.git
cd KattisTableGenerator
dotnet restore
```