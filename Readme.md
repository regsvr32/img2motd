# img2motd

convert a image file (pixel-style better) to text what can be printed colorfully in Linux shell.

## Dependences

* [dotnet core 2.2 or higher](https://dotnet.microsoft.com/download/dotnet-core/2.2)

*Run on Linux will crash when loading local libraries, currently works well on Windows system only, copy the generated file to Linux machine.*

## Usage

1. clone

```
git clone https://github.com/regsvr32/img2motd.git
cd img2motd
```

2. build

```
dotnet build img2motd.csproj -c Release -o bin/Release
cd bin/Release
```

3. convert

```
dotnet img2motd.dll $IMAGE_FILE_PATH
```

**Relpace `$IMAGE_FILE_PATH` by the file you want to convert**

Optional arguments:

* specific output file path, for example, `--out /etc/motd` (remember sudo), then the 'picture' can be printed ervertime you login. just like:

  ![](https://bakaya.ro/picture/img2motd.png)

* specific a background color like `--background #ffffff` if you customized your shell's background and there are some pixels have alpha-value neither 255 nor 0 in the image.
