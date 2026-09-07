<#
    run-all.ps1 — apply every seed file in order against the local dev database.

    Usage
      .\run-all.ps1                       reset, then run 01..05
      .\run-all.ps1 -From 04              run 04 onward, no reset
      .\run-all.ps1 -ResetOnly            just clear the sample order

    -I is required: the filtered indexes on TimeLimits, TaxDocuments and
    DocumentNumberRanges reject inserts unless QUOTED_IDENTIFIER is ON.
#>
param(
    [string] $Server   = 'localhost\SQLEXPRESS',
    [string] $Database = 'DotAirOrderNewV',
    [string] $From     = '00',
    [switch] $ResetOnly
)

$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path

$files = Get-ChildItem -Path $here -Filter '*.sql' |
         Sort-Object Name |
         Where-Object { $_.Name.Substring(0,2) -ge $From }

if ($ResetOnly) {
    $files = $files | Where-Object { $_.Name -like '00-*' }
}

foreach ($file in $files) {
    Write-Host "--> $($file.Name)" -ForegroundColor Cyan
    & sqlcmd -S $Server -d $Database -I -b -i $file.FullName
    if ($LASTEXITCODE -ne 0) {
        throw "FAILED: $($file.Name)"
    }
}

Write-Host "all seeds applied" -ForegroundColor Green
