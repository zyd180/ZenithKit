# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [0.3.1] - 2026-05-11

### Fixed
- **剪贴板**: `Dispose` 事件取消订阅无效（匿名 lambda 导致内存泄漏）
- **剪贴板**: 增加图片和 RTF 内容类型检测
- **图片工具**: 编码器匹配大小写不一致 bug
- **截图**: `CaptureWindowAsync` 嵌套 Task + `.Result` 反模式改为 async/await
- **截图**: 删除未使用的 `RegionX/Y/W/H` 属性，移除冗余 `SaveDirectory` 赋值
- **PDF 工具**: 输出路径从 `GetCurrentDirectory()` 改为 `IStorageService.CurrentPath`
- **硬盘清理**: 回收站改用 `SHEmptyRecycleBin` API（需管理员权限）
- **硬盘清理**: 消除 `FormatBytes` 重复实现，统一使用 `CleanupCategory.FormatBytes`
- **全局**: 所有 ViewModel 的操作命令增加 try-catch 错误处理
- **文本对比**: `BrowseLeft`/`BrowseRight` 提取公共 `BrowseFile` 方法

## [0.3.0] - 2026-05-11

### Added
- **硬盘清理**模块 — 扫描并清理系统垃圾文件，释放磁盘空间
  - 支持 6 个清理类别：Windows 临时文件、用户临时文件、缩略图缓存、Windows 更新缓存、回收站、DNS 缓存
  - 异步扫描，显示各类别大小
  - 可勾选清理类别，带进度条和结果报告
- Added `InvertBoolConverter` for XAML bool inversion bindings

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
