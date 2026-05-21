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

**Requirements:** macOS 11.5+, Xcode with macOS SDK

### Project Structure

```
MacOS/
├── Kinogrida.xcodeproj
└── Kinogrida/
    ├── KinogridaView.swift
    ├── KinogridaEngine.swift
    ├── Classes/
    │   ├── BaseShape/KGBaseShape.swift
    │   ├── ArcShape/KGArcShape.swift
    │   └── SquareShape/KGSquareShape.swift
    └── Constants/
        ├── KGColors.swift
        ├── KGGridConfig.swift
        └── KGTypes.swift
```

### Build from source

```bash
cd MacOS

# Build release
make

# Build + install to ~/Library/Screen Savers/
make install

# Remove
make remove

# Clean build artifacts
make clean
```

After `make install`, open **System Settings → Screen Saver** and select **Kinogrida**.

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
