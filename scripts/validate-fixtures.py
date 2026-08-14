#!/usr/bin/env python3
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
FIXTURES = ROOT / "Shared" / "fixtures"
SCHEMA = json.loads((ROOT / "Shared" / "schemas" / "quota-snapshot.schema.json").read_text())

ALLOWED_AVAILABILITY = set(SCHEMA["properties"]["availability"]["enum"])
ALLOWED_HEALTH = set(SCHEMA["properties"]["serviceHealth"]["enum"])
ALLOWED_RISK = set(SCHEMA["$defs"]["quotaWindow"]["properties"]["risk"]["enum"])
REQUIRED = set(SCHEMA["required"])


def fail(message):
    print(f"FAIL: {message}")
    sys.exit(1)


def check_object(value, path="root"):
    if not isinstance(value, dict):
        fail(f"{path} is not an object")
    missing = REQUIRED - set(value)
    if missing:
        fail(f"{path} missing required fields: {sorted(missing)}")
    if value.get("schemaVersion") != 1:
        fail(f"{path}.schemaVersion must be 1")
    if value["availability"] not in ALLOWED_AVAILABILITY:
        fail(f"{path}.availability invalid: {value['availability']}")
    if value["serviceHealth"] not in ALLOWED_HEALTH:
        fail(f"{path}.serviceHealth invalid: {value['serviceHealth']}")
    for key in ("recentDailyTokens", "tokens30d", "lifetimeTokens"):
        v = value.get(key)
        if v is not None and (not isinstance(v, int) or v < 0):
            fail(f"{path}.{key} invalid: {v!r}")
    reset = value.get("resetCredits")
    if reset is not None and (not isinstance(reset, int) or reset < 0):
        fail(f"{path}.resetCredits invalid: {reset!r}")
    activity = value.get("dailyActivity", [])
    if not isinstance(activity, list) or len(activity) > 183:
        fail(f"{path}.dailyActivity size invalid: {len(activity)}")
    for i, bucket in enumerate(activity):
        if set(bucket) != {"date", "tokens"} or not isinstance(bucket["tokens"], int) or bucket["tokens"] < 0:
            fail(f"{path}.dailyActivity[{i}] invalid: {bucket!r}")
    error = value.get("sanitizedError")
    if error is not None:
        if not isinstance(error, dict) or "code" not in error:
            fail(f"{path}.sanitizedError invalid: {error!r}")
        if len(error.get("message", "")) > 240:
            fail(f"{path}.sanitizedError.message too long")
    for key in ("primaryWindow", "sparkWindow"):
        window = value.get(key)
        if window is None:
            continue
        if not isinstance(window, dict):
            fail(f"{path}.{key} invalid")
        for pct in ("usedPercent", "remainingPercent"):
            p = window.get(pct)
            if p is not None and (not isinstance(p, (int, float)) or not 0 <= p <= 100):
                fail(f"{path}.{key}.{pct} out of range: {p!r}")
        if "risk" in window and window["risk"] not in ALLOWED_RISK:
            fail(f"{path}.{key}.risk invalid: {window['risk']}")


def main():
    files = sorted(FIXTURES.glob("*.json"))
    if not files:
        fail("no fixtures found")
    for path in files:
        check_object(json.loads(path.read_text()), path.name)
        print(f"ok {path.name}")
    print(f"validated {len(files)} fixtures")


if __name__ == "__main__":
    main()
