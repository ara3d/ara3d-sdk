# api-snapshot.ps1 — Phase 0.4 of docs/plato-roadmap.md (studio repo).
#
# Builds Ara3D.Geometry (which compiles in the Plato.Generated and Plato.Intrinsics
# shared projects), then runs the ApiSnapshot tool to produce a deterministic,
# sorted public-API listing. This file is the compatibility baseline for the
# extension-method emitter rewrite (Phase 2).
#
# Usage: .\api-snapshot.ps1 [-Configuration Release]

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$sdkRoot      = Split-Path -Parent $PSScriptRoot            # ara3d-sdk
$geometryProj = Join-Path $sdkRoot "src\Ara3D.Geometry\Ara3D.Geometry.csproj"
$toolProj     = Join-Path $PSScriptRoot "ApiSnapshot\ApiSnapshot.csproj"
$geometryDll  = Join-Path $sdkRoot "src\Ara3D.Geometry\bin\$Configuration\net8.0\Ara3D.Geometry.dll"
$outputFile   = Join-Path $sdkRoot "docs\api\plato-generated-api-baseline.txt"

Write-Host "Building Ara3D.Geometry ($Configuration)..."
dotnet build $geometryProj -c $Configuration -v quiet --nologo
if ($LASTEXITCODE -ne 0) { throw "Ara3D.Geometry build failed." }

if (-not (Test-Path $geometryDll)) { throw "Expected output not found: $geometryDll" }

Write-Host "Building ApiSnapshot tool..."
dotnet build $toolProj -c Release -v quiet --nologo
if ($LASTEXITCODE -ne 0) { throw "ApiSnapshot build failed." }

New-Item -ItemType Directory -Force (Split-Path -Parent $outputFile) | Out-Null

Write-Host "Generating API snapshot..."
dotnet run --project $toolProj -c Release --no-build -- $geometryDll $outputFile
if ($LASTEXITCODE -ne 0) { throw "ApiSnapshot run failed." }

Write-Host "Done: $outputFile"
