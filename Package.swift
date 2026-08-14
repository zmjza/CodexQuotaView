// swift-tools-version: 6.0

import PackageDescription

let package = Package(
    name: "CodexQuotaView",
    platforms: [
        .macOS(.v14)
    ],
    products: [
        .library(name: "CodexQuotaViewCore", targets: ["CodexQuotaViewCore"]),
        .library(
            name: "CodexQuotaViewWidgetContract",
            targets: ["CodexQuotaViewWidgetContract"]
        ),
        .library(
            name: "CodexQuotaViewFutureContracts",
            targets: ["CodexQuotaViewFutureContracts"]
        ),
        .executable(name: "CodexQuotaView", targets: ["CodexQuotaView"]),
        .executable(
            name: "CodexQuotaViewActivityHook",
            targets: ["CodexQuotaViewActivityHook"]
        ),
        .executable(name: "CodexQuotaViewProbe", targets: ["CodexQuotaViewProbe"])
    ],
    dependencies: [
        .package(
            url: "https://github.com/sparkle-project/Sparkle",
            exact: "2.9.2"
        )
    ],
    targets: [
        .target(
            name: "CodexQuotaViewCore",
            path: "Sources/QuotaViewCore",
            swiftSettings: [.swiftLanguageMode(.v5)]
        ),
        .target(
            name: "CodexQuotaViewWidgetContract",
            path: "Sources/QuotaViewWidgetContract",
            swiftSettings: [.swiftLanguageMode(.v5)]
        ),
        .target(
            name: "CodexQuotaViewFutureContracts",
            dependencies: ["CodexQuotaViewCore"],
            path: "Sources/QuotaViewFutureContracts",
            swiftSettings: [.swiftLanguageMode(.v5)]
        ),
        .executableTarget(
            name: "CodexQuotaView",
            dependencies: [
                "CodexQuotaViewCore",
                "CodexQuotaViewWidgetContract",
                .product(name: "Sparkle", package: "Sparkle")
            ],
            path: "Sources/QuotaView",
            swiftSettings: [.swiftLanguageMode(.v5)]
        ),
        .executableTarget(
            name: "CodexQuotaViewProbe",
            dependencies: ["CodexQuotaViewCore"],
            path: "Sources/QuotaViewProbe",
            swiftSettings: [.swiftLanguageMode(.v5)]
        ),
        .executableTarget(
            name: "CodexQuotaViewActivityHook",
            path: "Sources/QuotaViewActivityHook",
            swiftSettings: [.swiftLanguageMode(.v5)]
        ),
        .testTarget(
            name: "CodexQuotaViewCoreTests",
            dependencies: [
                "CodexQuotaViewCore",
                "CodexQuotaViewWidgetContract",
                "CodexQuotaViewFutureContracts",
                "CodexQuotaView"
            ],
            path: "Tests/QuotaViewCoreTests",
            swiftSettings: [.swiftLanguageMode(.v5)]
        )
    ]
)
