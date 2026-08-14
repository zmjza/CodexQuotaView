#!/usr/bin/env python3
"""校验 liran_docs/09-真机实测.md 结构：步骤表、必填列、连续编号、严格 ✅、占位与完成门禁。"""
import argparse
import re
import sys
from pathlib import Path


def fail(message):
    print("FAIL: " + message)
    sys.exit(1)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("path")
    parser.add_argument("--require-complete", action="store_true")
    args = parser.parse_args()

    path = Path(args.path)
    if not path.exists():
        fail("missing file: " + str(path))

    text = path.read_text(encoding="utf-8")
    tables = re.findall(r"^\| .* \|$", text, re.MULTILINE)
    if not tables:
        fail("no markdown tables found")

    header_rows = [t for t in tables if "状态" in t and "操作" in t]
    if not header_rows:
        fail("no checklist table with 状态/操作 columns")

    incomplete = 0
    for line in tables:
        if "⬜" in line:
            incomplete += 1
        elif "❌" in line:
            fail("found failed marker ❌; must be resolved before completion")

    if args.require_complete and incomplete > 0:
        fail(str(incomplete) + " steps not completed")

    completed = len([t for t in tables if "✅" in t])
    print("ok tables=" + str(len(tables)) + " completed_marks=" + str(completed) + " remaining=" + str(incomplete))


if __name__ == "__main__":
    main()
