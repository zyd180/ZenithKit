$ErrorActionPreference = "Stop"

$solutionDir = Split-Path -Parent $MyInvocation.MyCommand.Path | Split-Path -Parent
$publishDir = Join-Path $solutionDir "publish"

if (-Not (Test-Path $publishDir)) {
    New-Item -ItemType Directory -Path $publishDir | Out-Null
}

# Build & publish (framework-dependent, no trimming, single-folder)
dotnet publish "$solutionDir\ZenithKit.App\ZenithKit.App.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained false `
    /p:PublishSingleFile=false `
    /p:PublishTrimmed=false `
    -o $publishDir

Write-Host "Publish completed: $publishDir" -ForegroundColor Green
