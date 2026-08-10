#!/usr/bin/env bash
set -u

ROOT="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

echo "$ROOT"

if ! command -v lazpaint >/dev/null 2>&1; then
    echo "ERROR: lazpaint was not found"
    exit 1
fi

if [[ ! -d "$ROOT" ]] then
    echo "ERROR: folder does not exist: $ROOT" >&2
    exit 1
fi

converted=0
skipped=0
failed=0

while IFS= read -r -d '' pdn; do
    ora="${pdn%.*}.ora"

    if [[ -e "$ora" ]]; then
        echo "SKIP:    $ora already exists"
        ((skipped++))
        continue
    fi

    echo "CONVERT: $pdn"
    echo "      → $ora"

    if lazpaint "$pdn" "$ora"; then
        ((converted++))
    else
        echo "FAILED:  $pdn" >&2
        rm -f -- "$ora"
        ((failed++))
    fi

done < <(find "$ROOT" -type f -iname '*.pdn' -print0)

echo
echo "Done."
echo "Converted: $converted"
echo "Skipped:   $skipped"
echo "Failed:    $failed"