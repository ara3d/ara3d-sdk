# Stdio end-to-end test pattern for MCP servers

Written for porting into `dotnet-greenhouse`, whose test suite still never launches its MCP server
as a live subprocess. Reference implementation:
`tests/Ara3D.Ifc.Mcp.Tests/StdioServerProcess.cs` + `StdioEndToEndTests.cs` (Ara3D SDK).

## The blind spot

Under the stdio transport, **the server's own stdin is the JSON-RPC request stream**. Every
in-process test — including the ones that feed a `StringReader` to the transport pump — replaces
that stream with a test double, so no in-process test can observe anything the server does *to*
its real stdin. The whole class of defect is invisible:

- a spawned child process that inherits stdin and consumes protocol lines;
- a spawned child that inherits stdin, blocks waiting for EOF, and is then waited on — a deadlock
  that exists **only** because the handle was inherited;
- anything else that reads, closes, or blocks console stdin.

Greenhouse shipped a total stdio outage of exactly this shape (fixed in `5a40c81`): five
`ProcessStartInfo` sites never set `RedirectStandardInput`, and `ResponseGovernance` runs `git` on
every response, so every tool call hung forever. 546 passing tests said nothing.

## The pattern

One test class, three assertions, all bounded.

1. **Launch the real host binary as a child process.** Redirect stdin/stdout/stderr,
   `UseShellExecute = false`, UTF-8 without BOM on both stdin and stdout. Do not shell out to
   `dotnet run` inside the timed window — a cold build then looks exactly like a hung server. In
   the Ara3D SDK the host executable, its `deps.json` and its `runtimeconfig.json` are already
   copied next to the tests by the existing `ProjectReference`, so the test launches
   `<TestDirectory>/ara3d-ifc-mcp.exe` with no build step at all. Where that is not true, build once
   in `[OneTimeSetUp]`, outside the measured window.
2. **Hold stdin open for the whole test.** Closing it after the write ends the pump and hides the
   bug; a real client keeps the pipe open. Close it only in `Dispose`, then `WaitForExit`, then
   `Kill(entireProcessTree: true)`.
3. **Drain stdout and stderr on background handlers** (`BeginOutputReadLine` /
   `BeginErrorReadLine`) into a `BlockingCollection<string>` and a `StringBuilder`. A full stderr
   pipe is itself a hang. Stderr is the diagnostic the failure message prints.
4. **Every read is bounded.** `BlockingCollection.TryTake(out line, remaining)` against a deadline.
   The test must **fail on timeout, never hang** — that is the entire point. Skip lines that do not
   parse as JSON or carry a different id; a server is allowed to log chatter.
5. **Send `initialize`, then a `tools/call`, then a second call.** The second call proves the pump
   survived the first — a half-consumed stdin shows up there and nowhere else.

## Assert only what cannot drift

The reply is the signal. Assert:

- a response arrived before the deadline (this is the regression guard);
- `jsonrpc == "2.0"`;
- `id` equals the request id;
- exactly one of `result` / `error` is present.

Do **not** assert the protocol version string, the tool list, or a specific error code. Those churn
with ordinary protocol work and would turn this guard into a maintenance tax; an `error` reply
still proves the transport delivered.

Suggested budgets: 20 s for the first response (process start plus JIT), 10 s per later call.

## Proving the guard works

Injecting the defect and watching the test fail is part of building it. In the Ara3D SDK, before
`mcp.Start()`:

```csharp
var child = Process.Start(new ProcessStartInfo("cmd.exe", "/c sort")
{
    RedirectStandardOutput = true,   // note: stdin deliberately NOT redirected
    UseShellExecute = false,
    CreateNoWindow = true
})!;
child.WaitForExit();
```

`sort` inherits the protocol stdin and waits for an EOF the client will never send, so the server
deadlocks before it pumps a single line. Observed: 3/3 tests failed in 1 minute with
`No JSON-RPC response with id 1 arrived before the timeout` and an empty stderr — the server never
even printed its startup banner. Adding `RedirectStandardInput = true` alone makes it green again.

A weaker variant — spawning the same child but **not** waiting on it — did **not** fail the test:
the parent's pump won the race for the first lines. Worth knowing when writing the injection: the
deterministic reproduction is the inherited-handle *deadlock*, not the read race.

## Porting notes for greenhouse

- The natural home is a test that launches `greenhouse-mcp` and calls a verb whose implementation
  shells out — `ResponseGovernance` runs `git`, so almost any verb qualifies.
- Keep the timeout generous but finite; CI hanging forever is worse than CI failing.
- One end-to-end test per transport is enough. Everything else stays in-process.
