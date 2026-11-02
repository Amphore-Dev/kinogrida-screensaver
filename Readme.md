![Kinogrida preview](preview.gif)

# Kinogrida — Screensaver

A screensaver featuring animated geometric shapes — arcs and rounded squares — moving across a grid with smooth transitions and automatic palette changes. Originally ported from the [Kinogrida web project](https://github.com/Amphore-Dev/Kinogrida) (TypeScript/Canvas) to native Swift, then ported again to Windows (C# / WinForms).

## Features

- Procedurally generated grid of arcs and squares
- Automatic palette change with a fade transition
- 5 color palettes, randomly selected
- Settings dialog to adjust palette duration and fade duration

---

## macOS

**Install:** download the signed and notarized **Kinogrida.saver.zip** from the [latest release](https://github.com/Amphore-Dev/kinogrida-screensaver/releases/latest), unzip it and double-click **Kinogrida.saver**, then select it in **System Settings → Screen Saver**. macOS 11.5+, Apple silicon and Intel. Click **Options…** to tune it.

Want Kinogrida, Shades and Psykala mixed together? Get **amphore-saver** from [amphore-saver](https://github.com/Amphore-Dev/amphore-saver).

**Source:** the `MacOS/` folder is generated from [amphore-saver](https://github.com/Amphore-Dev/amphore-saver), where the macOS code is maintained (see `MacOS/README.md`). Make changes there and re-export with `scripts/export-saver.sh Kinogrida <path>/MacOS`.

```bash
cd MacOS
make install   # requires Xcode 16+ and XcodeGen (brew install xcodegen)
```

---

## Windows

**Requirements:** Windows 10+, [.NET 9 Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)

### Project Structure

```
Windows/
├── Kinogrida.csproj
├── Program.cs
├── ScreensaverForm.cs
├── PreviewForm.cs
├── Settings.cs
├── SettingsForm.cs
├── build.ps1
├── Makefile
└── Engine/
    ├── KGTypes.cs
    ├── KGColors.cs
    ├── KGGridConfig.cs
    ├── KGBaseShape.cs
    ├── KGSquareShape.cs
    ├── KGArcShape.cs
    └── KinogridaEngine.cs
```

### Build from source

```powershell
cd Windows

# Build
.\build.ps1

# Run fullscreen
.\build.ps1 run

# Publish single-file exe
.\build.ps1 publish

# Install to C:\Windows\System32\Kinogrida.scr
.\build.ps1 install

# Remove
.\build.ps1 remove
```

Or using Make:

```bash
make           # build
make run       # build + run
make install   # build + install
make remove    # uninstall
make clean     # clean artifacts
```

### Manual install

1. Build with `.\build.ps1 publish`
2. Copy `bin\publish\Kinogrida.exe` to `C:\Windows\System32\Kinogrida.scr`
3. Right-click the desktop → **Personalize → Lock screen → Screen saver**
4. Select **Kinogrida** from the list
