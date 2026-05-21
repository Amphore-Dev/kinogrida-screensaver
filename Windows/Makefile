BINARY   = bin\Release\net9.0-windows\Kinogrida.exe
SCR_NAME = Kinogrida.scr
INSTALL_DIR = C:\Windows\System32

.PHONY: all release debug clean install remove run

all: release

release:
	dotnet build -c Release

debug:
	dotnet build -c Debug

clean:
	dotnet clean
	if exist bin rmdir /s /q bin
	if exist obj rmdir /s /q obj

install: release
	copy /Y "$(BINARY)" "$(INSTALL_DIR)\$(SCR_NAME)"

remove:
	del /F /Q "$(INSTALL_DIR)\$(SCR_NAME)"

run: release
	"$(BINARY)" /s
