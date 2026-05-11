# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [0.2.0] - 2026-05-11

### Changed
- Renamed project from **MiCodeAutoToolBox** to **ZenithKit** (directories, files, namespaces, all references)
- Renamed display name across UI: window title, header logo, tray tooltip, storage folder name

### Fixed
- Fixed typo in `scripts/publish.ps1` (removed erroneous first line)

### Added
- `.gitignore` — excludes `bin/`, `obj/`, `publish/`, `.vs/`, NuGet caches
- `.editorconfig` — unified code style (4-space indent, CRLF, file-scoped namespaces, naming conventions)
- `CHANGELOG.md` — this file

### Removed
- Deleted empty scaffold file `Class1.cs`

## [0.1.0] - 2026-05-11

### Added
- Initial release with modular plugin architecture (Core + App separation)
- 9 utility modules:
  - Clipboard history — record and search clipboard (text/image/RTF)
  - Screenshot & annotation — capture, draw, quick save
  - File search — filename index search and recent files
  - Launcher — global quick commands / app launch
  - Batch rename — pattern/numbering/case rename
  - Archive — zip compress/decompress
  - Checksum — MD5/SHA1/SHA256 compute and verify
  - Image tools — format convert, resize, compress
  - PDF tools — offline PDF merge/split
  - Text diff — dual-file text difference comparison
- MVVM architecture with CommunityToolkit.Mvvm
- DI container via Microsoft.Extensions.DependencyInjection
- Command bus for menu/hotkey/tray command dispatch
- Storage service with atomic migration (AppData / Portable / Custom)
- AES-256-GCM encryption service with PBKDF2 key derivation
- Dark theme UI (left nav + content area layout)
- System tray integration with context menu
- Storage location switcher control
- Publish script (`scripts/publish.ps1`) — framework-dependent win-x64
