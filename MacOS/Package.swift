// swift-tools-version: 6.0
import PackageDescription

let package = Package(
    name: "amphore-saver",
    platforms: [.macOS("11.5")],
    products: [
        .library(name: "ScreensaverCore", targets: ["ScreensaverCore"]),
        .library(name: "KinogridaKit", targets: ["KinogridaKit"]),
    ],
    targets: [
        .target(name: "ScreensaverCore"),
        .target(name: "KinogridaKit", dependencies: ["ScreensaverCore"]),
    ]
)
