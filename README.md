# AppIconConv
Create an icns file for macOS and an ico file for Windows

- Created with .NET Core 5.
- Runs on Windows, macOS, and Linux.
- ICO is compatible with Windows Vista and later.
- ICNS is compatible with OS X Mountain Lion (10.8) and later.


## Build & Publish

### Build : 

```shell
cd AppIconConv
dotnet restore
dotnet build
```

### Publish :

#### Windows

```shell
dotnet publish -r win-x64
```

#### macOS

```shell
dotnet publish -r osx-x64
```

#### Linux

```shell
dotnet publish -r linux-x64
```



## Usage

### Options

- -s, --source

  Required. Path to the image file to be converted, or the path to the folder where the image file is stored.

- -t, --type      (Default: BOTH)

   Type of icon to output.

  - ICO 
  - ICNS
  - BOTH

-  -d, --destination

  Path to the output.

### Examples

If you have prepared the image files of the required size in advance:

```shell
appiconconv -i [image collection directory] -t [ICO | ICNS | BOTH] -o [output directory]
```

To create from a single image file:

```shell
appiconconv -i [image file] -t [ICO | ICNS | BOTH] -o [output directory]
```

