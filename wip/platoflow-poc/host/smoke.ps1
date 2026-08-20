<#
End-to-end smoke test for the platoflow-poc host.

The host must already be running (see README.md):
    dotnet run --project ara3d-sdk/wip/platoflow-poc/host/PlatoFlowHost.csproj

Then:
    powershell -ExecutionPolicy Bypass -File ara3d-sdk/wip/platoflow-poc/host/smoke.ps1

Prints PASS/FAIL per step; exit code is the number of failures.
Written for Windows PowerShell 5.1 (no ternary, no null-coalescing, no -SkipHttpErrorCheck).
#>

param(
    [string]$BaseUrl = "http://127.0.0.1:5214"
)

$ErrorActionPreference = "Stop"
$script:Failures = 0
$script:Step = 0

function Report([string]$name, [bool]$ok, [string]$detail) {
    $script:Step++
    if ($ok) {
        Write-Host ("PASS  {0,-28} {1}" -f $name, $detail) -ForegroundColor Green
    } else {
        Write-Host ("FAIL  {0,-28} {1}" -f $name, $detail) -ForegroundColor Red
        $script:Failures++
    }
}

function PostJson([string]$path, $body) {
    $json = $body | ConvertTo-Json -Depth 20 -Compress
    return Invoke-RestMethod -Method Post -Uri ($BaseUrl + $path) -ContentType "application/json" -Body $json
}

function Rpc([string]$method, $params, [int]$id) {
    $body = @{ jsonrpc = "2.0"; id = $id; method = $method }
    if ($null -ne $params) { $body.params = $params }
    return PostJson "/mcp" $body
}

function CallTool([string]$name, $arguments, [int]$id) {
    $response = Rpc "tools/call" @{ name = $name; arguments = $arguments } $id
    $text = $response.result.content[0].text
    return ($text | ConvertFrom-Json)
}

Write-Host "smoke: $BaseUrl" -ForegroundColor Cyan

# ---- 1. models ----
$models = $null
try {
    $models = Invoke-RestMethod -Uri "$BaseUrl/api/models"
    $ids = ($models | ForEach-Object { $_.id }) -join ", "
    Report "GET /api/models" ($models.Count -ge 1) "$($models.Count) model(s): $ids"
} catch {
    Report "GET /api/models" $false $_.Exception.Message
}

if (-not $models) {
    Write-Host "`nno models - cannot continue" -ForegroundColor Red
    exit 1
}

$first = $models[0]
$withIfc = $models | Where-Object { $_.ifcPath } | Select-Object -First 1

# ---- 2. file serving (content-length on a multi-MB .bos) ----
try {
    $bosUrl = $BaseUrl + $first.bosUrl
    $response = Invoke-WebRequest -Uri $bosUrl -UseBasicParsing
    $declared = [int64]$response.Headers["Content-Length"]
    $actual = $response.RawContentLength
    $ok = ($declared -gt 1000) -and ($declared -eq $actual)
    Report "GET /api/file (.bos)" $ok "content-length $declared, body $actual bytes"
} catch {
    Report "GET /api/file (.bos)" $false $_.Exception.Message
}

# ---- 3. file sandbox ----
try {
    $escaped = Invoke-RestMethod -Uri "$BaseUrl/api/file?path=../../CONTRACTS.md"
    Report "GET /api/file sandbox" $false "traversal was NOT rejected"
} catch {
    $status = 0
    if ($_.Exception.Response) { $status = [int]$_.Exception.Response.StatusCode }
    Report "GET /api/file sandbox" ($status -eq 403) "rejected with HTTP $status"
}

# ---- 4. SQL ----
try {
    $result = PostJson "/api/sql" @{ model = $first.id; sql = "SELECT count(*) AS n FROM EntityText" }
    $n = $result.rows[0][0]
    Report "POST /api/sql count" ($n -gt 0) "$($first.id): $n entities"
} catch {
    Report "POST /api/sql count" $false $_.Exception.Message
}

# ---- 5. SQL is read-only ----
try {
    $result = PostJson "/api/sql" @{ model = $first.id; sql = "DROP TABLE Entities" }
    Report "POST /api/sql read-only" ($null -ne $result.error) "rejected: $($result.error)"
} catch {
    Report "POST /api/sql read-only" $false $_.Exception.Message
}

# ---- 6. MCP initialize ----
try {
    $init = Rpc "initialize" @{ protocolVersion = "2025-06-18"; capabilities = @{}; clientInfo = @{ name = "smoke"; version = "1" } } 1
    $ok = ($null -ne $init.result.protocolVersion) -and ($null -ne $init.result.serverInfo.name)
    Report "POST /mcp initialize" $ok "$($init.result.serverInfo.name) $($init.result.protocolVersion)"
} catch {
    Report "POST /mcp initialize" $false $_.Exception.Message
}

# ---- 7. MCP tools/list ----
try {
    $list = Rpc "tools/list" $null 2
    $names = ($list.result.tools | ForEach-Object { $_.name }) -join ","
    Report "POST /mcp tools/list" ($list.result.tools.Count -ge 9) "$($list.result.tools.Count) tools: $names"
} catch {
    Report "POST /mcp tools/list" $false $_.Exception.Message
}

# ---- 8. MCP list_node_kinds ----
try {
    $kinds = CallTool "list_node_kinds" @{} 3
    Report "MCP list_node_kinds" ($kinds.Count -ge 10) "$($kinds.Count) node kinds"
} catch {
    Report "MCP list_node_kinds" $false $_.Exception.Message
}

# ---- 9. MCP add_node then the intent queue shows it ----
$newId = $null
try {
    $before = Invoke-RestMethod -Uri "$BaseUrl/api/intents?since=0"
    $added = CallTool "add_node" @{ kind = "table.sql"; x = 120; y = 240; params = @{ sql = "SELECT 1" } } 4
    $newId = $added.id
    $after = Invoke-RestMethod -Uri "$BaseUrl/api/intents?since=$($before.now)"
    $match = $after.intents | Where-Object { $_.intent.t -eq "addNode" -and $_.intent.id -eq $newId }
    $ok = ($null -ne $newId) -and ($null -ne $match)
    Report "MCP add_node + intents" $ok "id=$newId, queued at seq=$($match.seq), now=$($after.now)"
} catch {
    Report "MCP add_node + intents" $false $_.Exception.Message
}

# ---- 10. MCP connect / set_param / set_display all enqueue ----
try {
    $c = CallTool "connect" @{ fromNode = $newId; fromSlot = "out"; toNode = "n2"; toSlot = "in" } 5
    $p = CallTool "set_param" @{ node = $newId; name = "sql"; value = "SELECT 2" } 6
    $d = CallTool "set_display" @{ node = $newId } 7
    $ok = $c.ok -and $p.ok -and $d.ok
    Report "MCP connect/param/display" $ok "seqs $($c.seq),$($p.seq),$($d.seq)"
} catch {
    Report "MCP connect/param/display" $false $_.Exception.Message
}

# ---- 11. /api/state round trip through get_graph + read_node ----
try {
    $doc = @{
        name = "smoke"; display = "n1"
        nodes = @(@{ id = "n1"; kind = "table.sql"; params = @{ sql = "SELECT 1" }; x = 0; y = 0 })
        edges = @()
    }
    $results = @{ n1 = @{ state = "ok"; summary = "3 rows"; table = @{ columns = @("a"); rows = @(@(1), @(2), @(3)) } } }
    $null = PostJson "/api/state" @{ doc = $doc; results = $results }

    $graph = CallTool "get_graph" @{} 8
    $node = CallTool "read_node" @{ node = "n1" } 9
    $ok = ($graph.name -eq "smoke") -and ($node.state -eq "ok") -and ($node.table.rows.Count -eq 3)
    Report "state + get_graph/read_node" $ok "graph '$($graph.name)', n1 $($node.summary), $($node.table.rows.Count) rows"
} catch {
    Report "state + get_graph/read_node" $false $_.Exception.Message
}

# ---- 12. MCP sql passthrough ----
try {
    $table = CallTool "sql" @{ model = $first.id; sql = "SELECT Category, count(*) AS n FROM EntityText GROUP BY 1 ORDER BY n DESC LIMIT 3" } 10
    Report "MCP sql" ($table.rows.Count -ge 1) "top category $($table.rows[0][0]) = $($table.rows[0][1])"
} catch {
    Report "MCP sql" $false $_.Exception.Message
}

# ---- 13. append psets against the model that has a source IFC ----
if ($null -eq $withIfc) {
    Report "POST /api/append-psets" $false "no model with an ifcPath (duplex conversion fell back?)"
} else {
    try {
        $ids = PostJson "/api/sql" @{
            model = $withIfc.id
            sql   = "SELECT GlobalId FROM EntityText WHERE GlobalId IS NOT NULL AND length(GlobalId) = 22 LIMIT 2"
        }
        $g1 = $ids.rows[0][0]
        $g2 = $ids.rows[1][0]

        $write = PostJson "/api/append-psets" @{
            model = $withIfc.id
            psetName = "PlatoFlow_Smoke"
            rows = @(
                @{ globalId = $g1; props = @{ embodied_carbon = 12.5; grade = "A" } },
                @{ globalId = $g2; props = @{ embodied_carbon = 43.25; grade = "B" } }
            )
        }

        $exists = Test-Path $write.outPath
        $ok = $exists -and ($write.entitiesAdded -gt 0) -and (-not [string]::IsNullOrWhiteSpace($write.diffSummary))
        Report "POST /api/append-psets" $ok "$($write.entitiesAdded) entities, $($write.diffSummary)"
        Report "enriched file on disk" $exists $write.outPath
    } catch {
        Report "POST /api/append-psets" $false $_.Exception.Message
    }
}

# ---- 14. /api/ask (LLM SQL generation; passes without a key when it reports the key error) ----
try {
    $ask = PostJson "/api/ask" @{ model = $first.id; question = "How many entities are in the model?" }
    if ($ask.sql) {
        $ran = PostJson "/api/sql" @{ model = $first.id; sql = $ask.sql }
        $ok = ($null -ne $ran.rows)
        Report "POST /api/ask" $ok "sql: $($ask.sql)"
    } elseif ($ask.error -like "*ANTHROPIC_API_KEY*") {
        Report "POST /api/ask" $true "no key (expected): $($ask.error)"
    } else {
        Report "POST /api/ask" $false "unexpected: $($ask | ConvertTo-Json -Compress)"
    }
} catch {
    Report "POST /api/ask" $false $_.Exception.Message
}

# ---- 15. CORS ----
try {
    $response = Invoke-WebRequest -Uri "$BaseUrl/api/models" -UseBasicParsing
    $origin = $response.Headers["Access-Control-Allow-Origin"]
    Report "CORS header" ($origin -eq "*") "Access-Control-Allow-Origin: $origin"
} catch {
    Report "CORS header" $false $_.Exception.Message
}

# ---- 16. graph persistence: save ----
# The smoke runs on the host's machine, so file locations are checked directly.
$pocRoot = Split-Path $PSScriptRoot -Parent
$graphsDir = Join-Path $pocRoot "data\graphs"
$graphName = "smoke-graph"
$smokeDoc = [ordered]@{
    name = "smoke saved graph"
    display = $null
    nodes = @(
        [ordered]@{ id = "n1"; kind = "load.model"; params = [ordered]@{ model = $first.id }; x = 40; y = 60 }
    )
    edges = @()
}
try {
    $saved = PostJson "/api/graphs" @{ name = $graphName; doc = $smokeDoc }
    $ok = ($saved.ok -eq $true) -and ($saved.name -eq $graphName)
    Report "POST /api/graphs save" $ok "saved as '$($saved.name)'"
} catch {
    Report "POST /api/graphs save" $false $_.Exception.Message
}

# ---- 17. graph persistence: list has demos and the new save ----
try {
    $graphList = Invoke-RestMethod -Uri "$BaseUrl/api/graphs"
    $ok = ($graphList.demos.Count -ge 1) -and (@($graphList.saved) -contains $graphName)
    Report "GET /api/graphs" $ok "$($graphList.demos.Count) demos; saved: $(@($graphList.saved) -join ', ')"
} catch {
    Report "GET /api/graphs" $false $_.Exception.Message
}

# ---- 18. graph persistence: load returns identical JSON ----
try {
    $loaded = Invoke-RestMethod -Uri "$BaseUrl/api/graph?name=$graphName"
    $sent = $smokeDoc | ConvertTo-Json -Depth 20 -Compress
    $got = $loaded | ConvertTo-Json -Depth 20 -Compress
    Report "GET /api/graph round trip" ($sent -eq $got) "loaded $($got.Length) chars, identical to what was saved"
} catch {
    Report "GET /api/graph round trip" $false $_.Exception.Message
}

# ---- 19. graph persistence: demo loads by name, unknown name errors ----
try {
    $demoName = $graphList.demos[0]
    $demoGraph = Invoke-RestMethod -Uri "$BaseUrl/api/graph?name=$demoName"
    $missing = Invoke-RestMethod -Uri "$BaseUrl/api/graph?name=no-such-graph"
    $ok = ($demoGraph.nodes.Count -ge 1) -and ($null -ne $missing.error)
    Report "GET /api/graph demo+404" $ok "'$demoName' has $($demoGraph.nodes.Count) nodes; unknown -> $($missing.error)"
} catch {
    Report "GET /api/graph demo+404" $false $_.Exception.Message
}

# ---- 20. export-csv writes a parsable file with quoting intact ----
try {
    $export = PostJson "/api/export-csv" @{
        name = "smoke-export"
        table = [ordered]@{
            columns = @("name", "qty", "note")
            rows = @(
                @("plain", 1, "ok"),
                @("comma, inside", 2.5, "quote `" inside"),
                @("multi`nline", $null, $null)
            )
        }
    }
    $exists = ($null -ne $export.outPath) -and (Test-Path $export.outPath)
    $parsed = @()
    if ($exists) { $parsed = @(Import-Csv $export.outPath) }
    $ok = $exists -and ($export.outPath -like "*.csv") -and ($export.rows -eq 3) `
        -and ($parsed.Count -eq 3) `
        -and ($parsed[1].name -eq "comma, inside") `
        -and ($parsed[1].note -eq 'quote " inside') `
        -and ($parsed[2].qty -eq "")
    Report "POST /api/export-csv" $ok "$($export.outPath): $($export.rows) rows, re-parsed $($parsed.Count)"
} catch {
    Report "POST /api/export-csv" $false $_.Exception.Message
}

# ---- 21. traversal name is rejected or sanitized harmlessly ----
try {
    $evil = PostJson "/api/graphs" @{ name = "../evil"; doc = [ordered]@{ name = "evil"; display = $null; nodes = @(); edges = @() } }
    if ($null -ne $evil.error) {
        Report "traversal name" $true "rejected: $($evil.error)"
    } else {
        $clean = $evil.name
        $noPathChars = ($clean -notmatch '[\\/\.]')
        $inGraphs = Test-Path (Join-Path $graphsDir ($clean + ".json"))
        $escapedData = Test-Path (Join-Path $pocRoot "data\evil.json")
        $escapedRoot = Test-Path (Join-Path $pocRoot "evil.json")
        $ok = $noPathChars -and $inGraphs -and (-not $escapedData) -and (-not $escapedRoot)
        Report "traversal name" $ok "sanitized to '$clean', file stayed inside data\graphs"
    }
} catch {
    Report "traversal name" $false $_.Exception.Message
}

# cleanup: keep smoke artifacts out of the saved-graphs menu on the next app run
foreach ($leftover in @("smoke-graph", "evil")) {
    $file = Join-Path $graphsDir ($leftover + ".json")
    if (Test-Path $file) { Remove-Item $file -Force }
}

Write-Host ""
if ($script:Failures -eq 0) {
    Write-Host "ALL $($script:Step) STEPS PASSED" -ForegroundColor Green
} else {
    Write-Host "$($script:Failures) of $($script:Step) STEPS FAILED" -ForegroundColor Red
}
exit $script:Failures
