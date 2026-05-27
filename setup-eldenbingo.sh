#!/usr/bin/env bash
set -euo pipefail

GITHUB_REPO="awsker/EldenBingo"
GITHUB_LATEST_RELEASE_API_URL="https://api.github.com/repos/${GITHUB_REPO}/releases/latest"
DEFAULT_INSTALL_DIR="$HOME/.local/share/eldenbingo"
INSTALL_DIR="$DEFAULT_INSTALL_DIR"
DEFAULT_PREFIX="$DEFAULT_INSTALL_DIR/wineprefix"
DEFAULT_LAUNCHER_PATH="$HOME/.local/bin/eldenbingo"
DEFAULT_DESKTOP_FILE_PATH="$HOME/.local/share/applications/eldenbingo.desktop"
LAUNCHER_PATH="$DEFAULT_LAUNCHER_PATH"
DESKTOP_FILE_PATH="$DEFAULT_DESKTOP_FILE_PATH"
SHOULD_INIT_PREFIX=1
USE_PROTON_LAUNCHER=0
STEAM_CLIENT_INSTALL_PATH=""
ELDEN_RING_COMPATDATA_DIR=""
PROTON_PATH=""
PREFIX_PATH_OVERRIDE=""
FORCE_OVERWRITE=0
SKIP_SYSTEM_PACKAGE_INSTALL=0
DOCTOR_ONLY=0

print_usage() {
  cat <<EOF
Usage: $(basename "$0") [--install-dir PATH]

Options:
  --install-dir, -d PATH   Extract and install EldenBingo release files to PATH.
                           Default: $DEFAULT_INSTALL_DIR

  --prefix-path PATH       Override the Wine prefix path used to run EldenBingo.
                           If omitted and Steam Elden Ring is detected, the Steam
                           compat prefix is used automatically.

  --launcher-path PATH     Path for generated launcher script.
                           Default: $DEFAULT_LAUNCHER_PATH

  --desktop-file PATH      Path for generated desktop entry file.
                           Default: $DEFAULT_DESKTOP_FILE_PATH

  --force, -f              Overwrite existing launcher/desktop files without prompts.

  --skip-system-install    Never attempt package-manager installs.
                           Use this on immutable hosts (e.g. SteamOS/Bazzite)
                           when dependencies are already available.

  --doctor                 Run environment diagnostics only and exit.
                           Does not install packages, download releases, or
                           modify launcher/desktop files.

  --help, -h               Show this help message.

Behavior:
  1) Detect Elden Ring Steam install and Proton tool.
  2) Use Elden Ring Steam prefix by default when found.
  3) If not found, prompt for standalone prefix unless --prefix-path is provided.

Examples:
  $(basename "$0")
  $(basename "$0") --force
  $(basename "$0") --skip-system-install
  $(basename "$0") --doctor
  $(basename "$0") --install-dir "$HOME/.local/share/eldenbingo-dev"
  $(basename "$0") --prefix-path "$HOME/.local/share/eldenbingo/wineprefix"
  $(basename "$0") --launcher-path "$HOME/.local/bin/eldenbingo-dev" --force
EOF
}

log() {
  printf '[setup] %s\n' "$*"
}

warn() {
  printf '[setup][warn] %s\n' "$*" >&2
}

die() {
  printf '[setup][error] %s\n' "$*" >&2
  exit 1
}

ask_yes_no() {
  local prompt="$1"
  local reply
  while true; do
    read -r -p "$prompt [y/N]: " reply
    case "${reply,,}" in
      y|yes) return 0 ;;
      n|no|"") return 1 ;;
      *) printf 'Please answer y or n.\n' ;;
    esac
  done
}

expand_home_path() {
  local input="$1"
  if [[ "$input" == "~" ]]; then
    printf '%s\n' "$HOME"
  elif [[ "$input" == ~/* ]]; then
    printf '%s/%s\n' "$HOME" "${input#~/}"
  else
    printf '%s\n' "$input"
  fi
}

parse_args() {
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --install-dir|-d)
        [[ $# -ge 2 ]] || die "Missing value for $1"
        INSTALL_DIR="$(expand_home_path "$2")"
        shift 2
        ;;
      --prefix-path)
        [[ $# -ge 2 ]] || die "Missing value for $1"
        PREFIX_PATH_OVERRIDE="$(expand_home_path "$2")"
        shift 2
        ;;
      --launcher-path)
        [[ $# -ge 2 ]] || die "Missing value for $1"
        LAUNCHER_PATH="$(expand_home_path "$2")"
        shift 2
        ;;
      --desktop-file)
        [[ $# -ge 2 ]] || die "Missing value for $1"
        DESKTOP_FILE_PATH="$(expand_home_path "$2")"
        shift 2
        ;;
      --force|-f)
        FORCE_OVERWRITE=1
        shift
        ;;
      --skip-system-install)
        SKIP_SYSTEM_PACKAGE_INSTALL=1
        shift
        ;;
      --doctor)
        DOCTOR_ONLY=1
        shift
        ;;
      --help|-h)
        print_usage
        exit 0
        ;;
      *)
        die "Unknown argument: $1"
        ;;
    esac
  done

  DEFAULT_PREFIX="$INSTALL_DIR/wineprefix"
}

require_supported_distro() {
  [[ -f /etc/os-release ]] || die "Cannot detect distro (missing /etc/os-release)."
  # shellcheck disable=SC1091
  source /etc/os-release

  DISTRO_ID="${ID:-}"
  DISTRO_LIKE="${ID_LIKE:-}"

  if [[ "$DISTRO_ID" =~ ^(arch|manjaro|endeavouros)$ ]] || [[ "$DISTRO_LIKE" == *arch* ]]; then
    DISTRO_FAMILY="arch"
    PKG_MANAGER="pacman"
    PKG_INSTALL=(pacman -S --needed)
    PKG_UPDATE=()
    REQUIRED_PACKAGES=(wine winetricks curl cabextract unzip xdg-utils)
  elif [[ "$DISTRO_ID" =~ ^(ubuntu|debian|linuxmint|pop)$ ]] || [[ "$DISTRO_LIKE" == *debian* ]]; then
    DISTRO_FAMILY="ubuntu"
    PKG_MANAGER="apt"
    PKG_UPDATE=(apt update)
    PKG_INSTALL=(apt install -y)
    REQUIRED_PACKAGES=(wine64 winetricks curl cabextract unzip xdg-utils)
  elif [[ "$DISTRO_ID" =~ ^(fedora|rhel|centos|rocky|almalinux)$ ]] || [[ "$DISTRO_LIKE" == *fedora* ]] || [[ "$DISTRO_LIKE" == *rhel* ]]; then
    DISTRO_FAMILY="fedora"
    PKG_MANAGER="dnf"
    PKG_UPDATE=()
    PKG_INSTALL=(dnf install -y)
    REQUIRED_PACKAGES=(wine winetricks curl cabextract unzip xdg-utils)
  else
    die "Unsupported distro: ID=${DISTRO_ID:-unknown}, ID_LIKE=${DISTRO_LIKE:-unknown}. Supported: Arch, Ubuntu/Debian, Fedora/RHEL families."
  fi

  log "Detected distro family: $DISTRO_FAMILY (package manager: $PKG_MANAGER)"
}

is_immutable_host() {
  [[ -e /run/ostree-booted || -e /usr/bin/rpm-ostree || -e /usr/lib/extension-release.d ]]
}

run_privileged() {
  if [[ "$EUID" -eq 0 ]]; then
    "$@"
    return $?
  fi

  if command -v sudo >/dev/null 2>&1; then
    sudo "$@"
    return $?
  fi

  return 127
}

check_runtime_dependencies() {
  local missing_runtime=()
  local tool
  for tool in wine winetricks curl unzip; do
    if ! command -v "$tool" >/dev/null 2>&1; then
      missing_runtime+=("$tool")
    fi
  done

  if (( ${#missing_runtime[@]} == 0 )); then
    log "All required runtime commands already installed."
    return 0
  fi

  warn "Missing runtime command(s): ${missing_runtime[*]}"

  if [[ "$SKIP_SYSTEM_PACKAGE_INSTALL" -eq 1 ]]; then
    die "Dependencies missing and --skip-system-install was set. Install these tools manually first: ${missing_runtime[*]}"
  fi

  if ! command -v "$PKG_MANAGER" >/dev/null 2>&1; then
    die "Cannot auto-install dependencies: package manager '$PKG_MANAGER' is unavailable. Missing: ${missing_runtime[*]}"
  fi

  if ask_yes_no "Install required packages with $PKG_MANAGER now?"; then
    if (( ${#PKG_UPDATE[@]} > 0 )); then
      if ! run_privileged "${PKG_UPDATE[@]}"; then
        die "Failed to update package metadata with $PKG_MANAGER."
      fi
    fi

    if ! run_privileged "${PKG_INSTALL[@]}" "${REQUIRED_PACKAGES[@]}"; then
      if is_immutable_host; then
        die "Package install failed on an immutable host. Install dependencies via your host workflow, then rerun with --skip-system-install. Missing: ${missing_runtime[*]}"
      fi

      die "Failed to install dependencies with $PKG_MANAGER. Missing: ${missing_runtime[*]}"
    fi
  else
    die "Cannot continue without dependencies."
  fi

  # Re-check after install.
  for tool in wine winetricks curl unzip; do
    command -v "$tool" >/dev/null 2>&1 || die "Dependency still missing after install: $tool"
  done
}

print_doctor_report() {
  local tool
  local tools=(wine winetricks curl unzip)

  log "Doctor mode: running non-destructive environment diagnostics"
  log "Distro family: $DISTRO_FAMILY (package manager: $PKG_MANAGER)"

  if is_immutable_host; then
    log "Host model: immutable/ostree-style detected"
  else
    log "Host model: mutable"
  fi

  if command -v "$PKG_MANAGER" >/dev/null 2>&1; then
    log "Package manager command available: $PKG_MANAGER"
  else
    warn "Package manager command not available: $PKG_MANAGER"
  fi

  if [[ "$SKIP_SYSTEM_PACKAGE_INSTALL" -eq 1 ]]; then
    log "Option active: --skip-system-install"
  fi

  for tool in "${tools[@]}"; do
    if command -v "$tool" >/dev/null 2>&1; then
      log "Dependency OK: $tool ($(command -v "$tool"))"
    else
      warn "Dependency missing: $tool"
    fi
  done

  detect_elden_ring_install

  if [[ -n "$ELDEN_RING_INSTALL_DIR" ]]; then
    log "Elden Ring install detected: $ELDEN_RING_INSTALL_DIR"
  else
    warn "Elden Ring install not detected via Steam appmanifest"
  fi

  if [[ -n "$ELDEN_RING_PREFIX" ]]; then
    log "Elden Ring compat prefix detected: $ELDEN_RING_PREFIX"
  else
    warn "Elden Ring compat prefix not detected"
  fi

  if [[ -n "$PROTON_PATH" ]]; then
    log "Proton detected: $PROTON_PATH"
  else
    warn "Proton path not detected"
  fi

  log "Configured install dir: $INSTALL_DIR"
  log "Configured launcher path: $LAUNCHER_PATH"
  log "Configured desktop file path: $DESKTOP_FILE_PATH"

  if [[ -n "$PREFIX_PATH_OVERRIDE" ]]; then
    log "Configured prefix override: $PREFIX_PATH_OVERRIDE"
  else
    log "Configured prefix override: (none)"
  fi

  if [[ -f "$LAUNCHER_PATH" ]]; then
    log "Existing launcher file found: $LAUNCHER_PATH"
  else
    log "Existing launcher file not found: $LAUNCHER_PATH"
  fi

  if [[ -f "$DESKTOP_FILE_PATH" ]]; then
    log "Existing desktop entry found: $DESKTOP_FILE_PATH"
  else
    log "Existing desktop entry not found: $DESKTOP_FILE_PATH"
  fi

  log "Doctor mode complete"
}

download_and_install_latest_release() {
  local release_json
  local release_tag
  local asset_urls
  local asset_url
  local tmp_archive
  local extract_dir
  local extracted_exe
  local source_root

  log "Fetching latest release metadata for $GITHUB_REPO"
  release_json="$(curl -fsSL "$GITHUB_LATEST_RELEASE_API_URL")" || die "Failed to fetch latest release metadata."

  release_tag="$(printf '%s\n' "$release_json" | sed -nE 's/.*"tag_name"[[:space:]]*:[[:space:]]*"([^"]+)".*/\1/p' | head -n1)"
  asset_urls="$(printf '%s\n' "$release_json" \
    | grep -Eo '"browser_download_url"[[:space:]]*:[[:space:]]*"[^"]+"' \
    | sed -E 's/.*"([^"]+)"/\1/' \
    | grep -Ei '\.zip$' || true)"

  # Prefer the canonical release zip name when multiple zip assets are present.
  asset_url="$(printf '%s\n' "$asset_urls" | grep -E '/EldenBingo_v[^/]*\.zip$' | head -n1 || true)"
  if [[ -z "$asset_url" ]]; then
    asset_url="$(printf '%s\n' "$asset_urls" | head -n1 || true)"
  fi

  [[ -n "$asset_url" ]] || die "No .zip release asset found for latest release."

  tmp_archive="$(mktemp --suffix=.zip)"
  extract_dir="$(mktemp -d)"

  log "Downloading EldenBingo release ${release_tag:-unknown}: $asset_url"
  curl -fL "$asset_url" -o "$tmp_archive"

  log "Extracting release archive"
  unzip -q "$tmp_archive" -d "$extract_dir"

  extracted_exe="$(find "$extract_dir" -type f -name 'EldenBingo.exe' | head -n1 || true)"
  [[ -n "$extracted_exe" ]] || die "Could not find EldenBingo.exe in downloaded release archive."

  source_root="$(dirname "$extracted_exe")"
  mkdir -p "$INSTALL_DIR"

  log "Installing release files to: $INSTALL_DIR"
  cp -a "$source_root"/. "$INSTALL_DIR"/

  EXE_PATH="$INSTALL_DIR/EldenBingo.exe"
  [[ -f "$EXE_PATH" ]] || die "Install completed but EldenBingo.exe was not found at $EXE_PATH"

  rm -rf "$extract_dir"
  rm -f "$tmp_archive"
}

add_unique_path() {
  local candidate="$1"
  local existing

  [[ -n "$candidate" ]] || return 0

  for existing in "${STEAM_LIBRARY_PATHS[@]:-}"; do
    if [[ "$existing" == "$candidate" ]]; then
      return 0
    fi
  done

  STEAM_LIBRARY_PATHS+=("$candidate")
}

collect_steam_library_paths() {
  local root
  local library_vdf
  local path_line
  local parsed_path
  local default_roots=(
    "$HOME/.steam/steam"
    "$HOME/.local/share/Steam"
    "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam"
  )

  STEAM_LIBRARY_PATHS=()

  for root in "${default_roots[@]}"; do
    if [[ -d "$root/steamapps" ]]; then
      add_unique_path "$root"
      if [[ -z "$STEAM_CLIENT_INSTALL_PATH" ]]; then
        STEAM_CLIENT_INSTALL_PATH="$root"
      fi
    fi

    library_vdf="$root/steamapps/libraryfolders.vdf"
    if [[ -f "$library_vdf" ]]; then
      while IFS= read -r path_line; do
        parsed_path="$(sed -E 's/.*"path"[[:space:]]+"([^"]+)".*/\1/' <<< "$path_line")"
        parsed_path="${parsed_path//\\\\/\\}"
        if [[ -d "$parsed_path/steamapps" ]]; then
          add_unique_path "$parsed_path"
        fi
      done < <(grep -E '"path"[[:space:]]+"' "$library_vdf" || true)
    fi
  done
}

detect_proton_path() {
  local config_info
  local version_file
  local version_name
  local candidate
  local best_candidate
  local line

  PROTON_PATH=""
  config_info="$ELDEN_RING_COMPATDATA_DIR/config_info"
  version_file="$ELDEN_RING_COMPATDATA_DIR/version"

  if [[ -f "$config_info" ]]; then
    while IFS= read -r line; do
      candidate="$(sed -nE 's/.*([A-Za-z0-9_./ -]+\/proton).*/\1/p' <<< "$line" | head -n1)"
      if [[ -n "$candidate" && -x "$candidate" ]]; then
        PROTON_PATH="$candidate"
        break
      fi
    done < "$config_info"
  fi

  if [[ -z "$PROTON_PATH" && -f "$version_file" ]]; then
    version_name="$(tr -d '\r\n' < "$version_file")"
    if [[ -n "$version_name" ]]; then
      for candidate in \
        "$STEAM_CLIENT_INSTALL_PATH/steamapps/common/$version_name/proton" \
        "$HOME/.steam/root/compatibilitytools.d/$version_name/proton" \
        "$HOME/.local/share/Steam/compatibilitytools.d/$version_name/proton" \
        "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/compatibilitytools.d/$version_name/proton"
      do
        if [[ -x "$candidate" ]]; then
          PROTON_PATH="$candidate"
          break
        fi
      done
    fi
  fi

  if [[ -z "$PROTON_PATH" ]]; then
    best_candidate=""
    for candidate in \
      "$STEAM_CLIENT_INSTALL_PATH/steamapps/common/Proton"*/proton \
      "$HOME/.steam/root/compatibilitytools.d"/*/proton \
      "$HOME/.local/share/Steam/compatibilitytools.d"/*/proton \
      "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/compatibilitytools.d"/*/proton
    do
      if [[ -x "$candidate" ]]; then
        best_candidate="$candidate"
      fi
    done

    if [[ -n "$best_candidate" ]]; then
      PROTON_PATH="$best_candidate"
    fi
  fi
}

detect_elden_ring_install() {
  local library
  local manifest_path
  local compat_prefix
  local install_dir

  ELDEN_RING_PREFIX=""
  ELDEN_RING_INSTALL_DIR=""
  ELDEN_RING_COMPATDATA_DIR=""

  collect_steam_library_paths

  for library in "${STEAM_LIBRARY_PATHS[@]:-}"; do
    manifest_path="$library/steamapps/appmanifest_1245620.acf"
    ELDEN_RING_COMPATDATA_DIR="$library/steamapps/compatdata/1245620"
    compat_prefix="$ELDEN_RING_COMPATDATA_DIR/pfx"
    install_dir="$library/steamapps/common/ELDEN RING"

    if [[ -f "$manifest_path" ]]; then
      if [[ -d "$install_dir" ]]; then
        ELDEN_RING_INSTALL_DIR="$install_dir"
      else
        ELDEN_RING_INSTALL_DIR="$library/steamapps/common/ELDEN RING"
      fi

      if [[ -d "$compat_prefix" ]]; then
        ELDEN_RING_PREFIX="$compat_prefix"
        detect_proton_path
        break
      fi
    fi
  done
}

select_wine_prefix() {
  local input

  detect_elden_ring_install

  if [[ -n "$PREFIX_PATH_OVERRIDE" ]]; then
    WINEPREFIX_PATH="$PREFIX_PATH_OVERRIDE"

    if [[ -n "$ELDEN_RING_PREFIX" && "$WINEPREFIX_PATH" == "$ELDEN_RING_PREFIX" ]]; then
      SHOULD_INIT_PREFIX=0
      if [[ -n "$PROTON_PATH" ]]; then
        USE_PROTON_LAUNCHER=1
      else
        USE_PROTON_LAUNCHER=0
      fi
      log "Using prefix override (matches Elden Ring Steam prefix): $WINEPREFIX_PATH"
    else
      SHOULD_INIT_PREFIX=1
      USE_PROTON_LAUNCHER=0
      log "Using prefix override: $WINEPREFIX_PATH"
    fi

    return 0
  fi

  if [[ -n "$ELDEN_RING_PREFIX" ]]; then
    log "Detected Elden Ring Steam install: $ELDEN_RING_INSTALL_DIR"
    log "Using Elden Ring prefix for integration: $ELDEN_RING_PREFIX"
    WINEPREFIX_PATH="$ELDEN_RING_PREFIX"
    SHOULD_INIT_PREFIX=0
    if [[ -n "$PROTON_PATH" ]]; then
      USE_PROTON_LAUNCHER=1
      log "Detected Proton for Elden Ring: $PROTON_PATH"
    else
      USE_PROTON_LAUNCHER=0
      warn "Could not detect Elden Ring Proton tool. Launcher will use wine."
    fi
    return 0
  fi

  warn "No Elden Ring Steam installation with Proton prefix was found."
  warn "Game integration features require EldenBingo and Elden Ring in the same prefix. The bingo board can work without it, but map and launching integration will not."

  if ! ask_yes_no "Create/use a standalone Wine prefix anyway?"; then
    die "Setup cancelled. Install Elden Ring via Steam first, launch once, then rerun setup for integration support."
  fi

  read -r -p "Wine prefix path [$DEFAULT_PREFIX]: " input
  input="${input:-$DEFAULT_PREFIX}"
  WINEPREFIX_PATH="$(expand_home_path "$input")"
  SHOULD_INIT_PREFIX=1
  USE_PROTON_LAUNCHER=0
}

confirm_overwrite_targets() {
  if [[ "$FORCE_OVERWRITE" -eq 1 ]]; then
    log "Force mode enabled: existing launcher/desktop files will be overwritten."
    return 0
  fi

  if [[ -f "$LAUNCHER_PATH" ]]; then
    if ! ask_yes_no "Launcher already exists at $LAUNCHER_PATH. Overwrite it?"; then
      die "Aborted to avoid overwriting launcher."
    fi
  fi

  if [[ -f "$DESKTOP_FILE_PATH" ]]; then
    if ! ask_yes_no "Desktop entry already exists at $DESKTOP_FILE_PATH. Overwrite it?"; then
      die "Aborted to avoid overwriting desktop entry."
    fi
  fi
}

create_wine_prefix() {
  if [[ "$SHOULD_INIT_PREFIX" -eq 0 ]]; then
    log "Skipping wineboot for Steam-managed prefix: $WINEPREFIX_PATH"
    return 0
  fi

  log "Creating/updating Wine prefix: $WINEPREFIX_PATH"
  mkdir -p "$WINEPREFIX_PATH"
  WINEARCH=win64 WINEPREFIX="$WINEPREFIX_PATH" wineboot -u
}

install_windows_dependencies() {
  log "Installing common Windows dependencies (corefonts, vcrun2022, dotnetdesktop8) via winetricks"
  if ! WINEPREFIX="$WINEPREFIX_PATH" winetricks -q corefonts vcrun2022 dotnetdesktop8; then
    warn "winetricks reported an issue."
  fi
}

desktop_escape_exec_arg() {
  local arg="$1"
  arg="${arg//\\/\\\\}"
  arg="${arg//\"/\\\"}"
  printf '"%s"' "$arg"
}

create_launcher_script() {
  mkdir -p "$(dirname "$LAUNCHER_PATH")"

  if [[ "$USE_PROTON_LAUNCHER" -eq 1 && -n "$PROTON_PATH" && -n "$ELDEN_RING_COMPATDATA_DIR" && -n "$STEAM_CLIENT_INSTALL_PATH" ]]; then
    cat > "$LAUNCHER_PATH" <<EOF
#!/usr/bin/env bash
set -euo pipefail

unset DOTNET_ROOT
unset DOTNET_ROOT_X64
unset DOTNET_ROOT_X86
unset DOTNET_MULTILEVEL_LOOKUP

export STEAM_COMPAT_CLIENT_INSTALL_PATH="$STEAM_CLIENT_INSTALL_PATH"
export STEAM_COMPAT_DATA_PATH="$ELDEN_RING_COMPATDATA_DIR"
export ELDEN_RING_INSTALL_DIR="$ELDEN_RING_INSTALL_DIR"

APP_DIR="$(dirname "$EXE_PATH")"
cd "\$APP_DIR"

exec "$PROTON_PATH" run "./$(basename "$EXE_PATH")" "\$@"
EOF
    log "Launcher created (Proton mode): $LAUNCHER_PATH"
  else
    cat > "$LAUNCHER_PATH" <<EOF
#!/usr/bin/env bash
set -euo pipefail

unset DOTNET_ROOT
unset DOTNET_ROOT_X64
unset DOTNET_ROOT_X86
unset DOTNET_MULTILEVEL_LOOKUP

export ELDEN_RING_INSTALL_DIR="$ELDEN_RING_INSTALL_DIR"

APP_DIR="$(dirname "$EXE_PATH")"
cd "\$APP_DIR"

WINEPREFIX="$WINEPREFIX_PATH" \\
  wine "./$(basename "$EXE_PATH")" "\$@"
EOF
    log "Launcher created (Wine mode): $LAUNCHER_PATH"
  fi

  chmod +x "$LAUNCHER_PATH"
}

create_desktop_entry() {
  local desktop_exec

  mkdir -p "$(dirname "$DESKTOP_FILE_PATH")"
  desktop_exec="$(desktop_escape_exec_arg "$LAUNCHER_PATH")"

  cat > "$DESKTOP_FILE_PATH" <<EOF
[Desktop Entry]
Type=Application
Name=EldenBingo
Comment=Launch EldenBingo via Wine
Exec=$desktop_exec
Icon=wine
Terminal=false
Categories=Game;
StartupNotify=true
EOF

  chmod +x "$DESKTOP_FILE_PATH"

  if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database "$(dirname "$DESKTOP_FILE_PATH")" >/dev/null 2>&1 || true
  fi

  log "Desktop entry created: $DESKTOP_FILE_PATH"
}

main() {
  parse_args "$@"
  require_supported_distro

  if [[ "$DOCTOR_ONLY" -eq 1 ]]; then
    print_doctor_report
    exit 0
  fi

  check_runtime_dependencies
  select_wine_prefix
  download_and_install_latest_release
  confirm_overwrite_targets
  create_wine_prefix
  install_windows_dependencies
  create_launcher_script
  create_desktop_entry
  log "Setup complete. Installed EldenBingo to: $INSTALL_DIR"
  log "Start EldenBingo with: $LAUNCHER_PATH"
}

main "$@"
