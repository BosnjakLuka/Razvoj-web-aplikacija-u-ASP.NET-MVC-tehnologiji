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

# Redakcija poznatih secret formata prije pisanja u log (lab-1/ je deliverable koji se commita,
# ne smije sadrzavati stvarne kredencijale).
$secretPatterns = @(
    @{ Regex = 'GOCSPX-[A-Za-z0-9_-]+'; Replacement = '[REDACTED-GOOGLE-CLIENT-SECRET]' }
    @{ Regex = 'AIza[A-Za-z0-9_-]{30,}'; Replacement = '[REDACTED-GOOGLE-API-KEY]' }
    @{ Regex = 'AQ\.[A-Za-z0-9_-]{15,}'; Replacement = '[REDACTED-GEMINI-API-KEY]' }
    @{ Regex = '\d{10,}-[a-z0-9]+\.apps\.googleusercontent\.com'; Replacement = '[REDACTED-GOOGLE-CLIENT-ID]' }
    @{ Regex = '(?i)(Password|Pwd)=[^;"\\]+'; Replacement = '$1=[REDACTED]' }
    @{ Regex = '(?i)"(api[_-]?key|secret|token|password|client_?secret)"\s*:\s*"[^"]*"'; Replacement = '"$1":"[REDACTED]"' }
)

foreach ($p in $secretPatterns) {
    $inputText = [Regex]::Replace($inputText, $p.Regex, $p.Replacement)
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$entry = @(
    "[$timestamp]"
    $inputText.Trim()
    ""
) -join [Environment]::NewLine

Add-Content -Path $logPath -Value $entry
