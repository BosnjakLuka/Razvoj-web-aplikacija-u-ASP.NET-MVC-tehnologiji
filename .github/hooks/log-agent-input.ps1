$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$logDir = Join-Path $repoRoot "lab-1"
$logPath = Join-Path $logDir "agent_log.txt"

if (-not (Test-Path -Path $logDir)) {
    New-Item -Path $logDir -ItemType Directory | Out-Null
}

$inputText = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($inputText)) {
    exit 0
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$entry = @(
    "[$timestamp]"
    $inputText.Trim()
    ""
) -join [Environment]::NewLine

Add-Content -Path $logPath -Value $entry
