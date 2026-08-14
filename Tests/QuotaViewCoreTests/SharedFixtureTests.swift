import Foundation
import XCTest

final class SharedFixtureTests: XCTestCase {
    private var fixtureDirectory: URL {
        URL(fileURLWithPath: #filePath)
            .deletingLastPathComponent()
            .deletingLastPathComponent()
            .deletingLastPathComponent()
            .appendingPathComponent("Shared/fixtures", isDirectory: true)
    }

    private static let availableStates: Set<String> = ["available", "refreshing", "unavailable", "error"]
    private static let healthStates: Set<String> = ["normal", "warning", "exhausted", "offline", "error", "unknown"]
    private static let riskStates: Set<String> = ["normal", "warning", "exhausted", "unknown"]

    func testSharedFixturesMatchSchemaInvariants() throws {
        let names = ["available", "warning", "exhausted", "unavailable", "error"]
        for name in names {
            let url = fixtureDirectory.appendingPathComponent("\(name).json")
            let data = try Data(contentsOf: url)
            let object = try XCTUnwrap(
                JSONSerialization.jsonObject(with: data) as? [String: Any]
            )
            XCTAssertEqual(object["schemaVersion"] as? Int, 1, name)
            XCTAssertTrue(
                Self.availableStates.contains(
                    object["availability"] as? String ?? ""
                ),
                "\(name): availability"
            )
            XCTAssertTrue(
                Self.healthStates.contains(
                    object["serviceHealth"] as? String ?? ""
                ),
                "\(name): serviceHealth"
            )
            if let window = object["primaryWindow"] as? [String: Any] {
                for key in ["usedPercent", "remainingPercent"] {
                    if let value = window[key] as? Double {
                        XCTAssertTrue((0...100).contains(value), "\(name): \(key)")
                    }
                }
                if let risk = window["risk"] as? String {
                    XCTAssertTrue(Self.riskStates.contains(risk), "\(name): risk")
                }
            }
            if let buckets = object["dailyActivity"] as? [[String: Any]] {
                XCTAssertLessThanOrEqual(buckets.count, 183, name)
            }
            let sanitized = object["sanitizedError"] as? [String: Any]
            if let sanitized {
                XCTAssertNotNil(sanitized["code"], name)
            }
        }
    }
}
