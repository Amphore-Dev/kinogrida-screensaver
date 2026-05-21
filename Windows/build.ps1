param(
    [string]$Target = "release"
)

$dotnet  = "C:\Program Files\dotnet\dotnet.exe"
$binR    = "$PSScriptRoot\bin\Release\net9.0-windows\Kinogrida.exe"
$pubExe  = "$PSScriptRoot\bin\publish\Kinogrida.exe"
$scrDest = "C:\Windows\System32\Kinogrida.scr"
Set-Location $PSScriptRoot

switch ($Target.ToLower()) {
    "release" { & $dotnet build -c Release }
    "debug"   { & $dotnet build -c Debug }
    "clean"   { & $dotnet clean; Remove-Item bin, obj -Recurse -Force -ErrorAction SilentlyContinue }
    "run"     { & $dotnet build -c Release; & $binR /s }
    "publish" {
        & $dotnet publish -c Release -r win-x64 --self-contained false `
            /p:PublishSingleFile=true /p:PublishDir=bin\publish
        Write-Host "Published: $pubExe"
    }
    "install" {
        & $dotnet publish -c Release -r win-x64 --self-contained false `
            /p:PublishSingleFile=true /p:PublishDir=bin\publish
        Copy-Item $pubExe $scrDest -Force
        Write-Host "Installed: $scrDest"
    }
    "remove"  { Remove-Item $scrDest -Force -ErrorAction SilentlyContinue }
    default   { Write-Host "Targets: release | debug | clean | run | publish | install | remove" }
}
