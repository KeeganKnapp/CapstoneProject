#!/usr/bin/env bash
set -euo pipefail

# ------------------------------
# Config
# ------------------------------
AVD_NAME="${AVD_NAME:-MauiPhone}"
API_LEVEL="${API_LEVEL:-34}"
SYSTEM_IMAGE="${SYSTEM_IMAGE:-system-images;android-35;google_apis;x86_64}"
BUILD_TOOLS="${BUILD_TOOLS:-34.0.0}"
DEVICE_PROFILE="${DEVICE_PROFILE:-pixel_5}"
CMDLINE_VER="11076708"  # commandlinetools zip rev from Google

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SDK_ROOT="$REPO_ROOT/tools/android-sdk"
export ANDROID_SDK_ROOT="$SDK_ROOT"              # <-- use env (no --sdk_root)
export ANDROID_AVD_HOME="$SDK_ROOT/avd"          # keep AVDs in repo

# Java 17
if [ -z "${JAVA_HOME:-}" ]; then
  if [ -d /usr/lib/jvm/java-17-openjdk-amd64 ]; then
    export JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
  else
    echo "JDK 17 not found. On Ubuntu: sudo apt-get install -y openjdk-17-jdk" >&2
    exit 1
  fi
fi

# Put repo-local tools first
export PATH="$SDK_ROOT/emulator:$SDK_ROOT/platform-tools:$SDK_ROOT/cmdline-tools/latest/bin:$JAVA_HOME/bin:$PATH"

echo "==> SDK root: $ANDROID_SDK_ROOT"
echo "==> AVD home: $ANDROID_AVD_HOME"

# ------------------------------
# Install cmdline-tools (idempotent)
# ------------------------------
if [ ! -x "$SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" ]; then
  echo ">> Installing Android commandline-tools..."
  mkdir -p "$SDK_ROOT/cmdline-tools"
  tmpdir="$(mktemp -d)"
  (
    cd "$tmpdir"
    curl -fsSL -o cmdline-tools.zip "https://dl.google.com/android/repository/commandlinetools-linux-${CMDLINE_VER}_latest.zip"
    unzip -q cmdline-tools.zip
    mkdir -p "$SDK_ROOT/cmdline-tools/latest"
    cp -a cmdline-tools/* "$SDK_ROOT/cmdline-tools/latest/"
  )
  rm -rf "$tmpdir"
fi

# Ensure writable (in case someone ran with sudo before)
chmod -R u+rwX "$SDK_ROOT"

# ------------------------------
# Accept licenses (avoid SIGPIPE from `yes`)
# ------------------------------
echo ">> Accepting Android SDK licenses..."
set +o pipefail
yes | "$SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" --licenses >/dev/null 2>&1 || true
set -o pipefail

# ------------------------------
# Install packages (idempotent)
# ------------------------------
echo ">> Installing SDK packages..."
"$SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" \
  "platform-tools" \
  "emulator" \
  "platforms;android-${API_LEVEL}" \
  "build-tools;${BUILD_TOOLS}" \
  "${SYSTEM_IMAGE}"

# ------------------------------
# Create AVD in repo (no --sdk_root)
# ------------------------------
mkdir -p "$ANDROID_AVD_HOME"
if ! emulator -list-avds 2>/dev/null | grep -qx "$AVD_NAME"; then
  echo ">> Creating AVD '$AVD_NAME'..."
  # Use the avdmanager that comes with cmdline-tools
  echo "no" | "$SDK_ROOT/cmdline-tools/latest/bin/avdmanager" create avd \
    -n "$AVD_NAME" \
    -k "$SYSTEM_IMAGE" \
    -d "$DEVICE_PROFILE" \
    --force
fi

# ------------------------------
# Start emulator and wait for boot
# ------------------------------
echo ">> Using emulator: $(command -v emulator)"
echo ">> Using adb:      $(command -v adb)"

adb kill-server || true
adb start-server

if ! adb devices | grep -qE '^emulator-[0-9]+\s+device$'; then
  echo ">> Starting emulator '$AVD_NAME'..."
  emulator -avd "$AVD_NAME" -gpu swiftshader_indirect -no-snapshot -no-boot-anim >/dev/null 2>&1 &
fi

echo ">> Waiting for device..."
adb wait-for-device

# Occasionally shows 'offline' for a moment
for _ in $(seq 1 10); do
  adb devices | grep -qE '^emulator-[0-9]+\s+device$' && break
  sleep 1
done

echo ">> Waiting for Android to finish booting..."
for _ in $(seq 1 180); do
  booted="$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')"
  [ "$booted" = "1" ] && break
  sleep 1
done
adb shell input keyevent 82 || true  # unlock

# ------------------------------
# Build & deploy MAUI app (repo-local SDK/JDK)
# ------------------------------
echo ">> Building & deploying MAUI app (Android)..."
dotnet build -t:Run -f net8.0-android \
  -p:AndroidSdkDirectory="$SDK_ROOT" \
  -p:JavaSdkDirectory="$JAVA_HOME"

echo ">> Done."

