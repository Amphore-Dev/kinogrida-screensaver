# Kinogrida — macOS

Generated from [amphore-saver](https://github.com/Amphore-Dev/amphore-saver) `7d284b4` (v1.0.0) by `scripts/export-saver.sh`.
Do not edit here: change amphore-saver and export again.

Requires Xcode 16+ and [XcodeGen](https://github.com/yonaskolb/XcodeGen) (`brew install xcodegen`).

```bash
make            # generate Kinogrida.xcodeproj and build Kinogrida.saver
make install    # build + install to ~/Library/Screen Savers
make remove     # uninstall
make release    # sign (Developer ID), notarize and staple into dist/
```
