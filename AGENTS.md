# MCP Sleep Agent Guide

This guide helps you decide which version of the MCP Sleep server to integrate and explains how the tool behaves from an agent author's perspective.

## Choosing an installation path

| Scenario | Recommended flavor | Why it helps |
| --- | --- | --- |
| Prototyping or running inside a repo without extra build steps | `single-file/mcp_sleep.cs` | No project scaffolding; execute directly with `dotnet` once you reference the absolute path. |
| Sharing across machines, CI, or keeping agents agnostic of file paths | `dotnet-tool` global tool | Installs `mcp-sleep` globally; add the .NET tools directory to PATH once and every host can launch it. |
| Keeping everything self-contained inside a repo | `dotnet-tool` local tool | Add a tool manifest and run it via `dotnet tool run mcp-sleep`; no PATH edits needed. |

**Quick rules of thumb**
- Prefer the single-file script when demoing or you want edits to take effect immediately.
- Prefer the global tool when multiple agents or users need the same command without remembering paths.
- Both variants expose an identical MCP surface, so you can switch later without updating prompts or workflow logic.

## Agent integration checklist
- Register the command in your MCP configuration (see `README.md` for full JSON snippets).
- Ensure the host environment has the `ModelContextProtocol.Server` runtime dependency available (handled automatically for both options once you run them through `dotnet`).
- Forward standard input/output unchanged; the server uses stdio for the MCP transport.
- Surface stderr logs if you want visibility into connection and request flow; the server defaults to informational logging.

## Tool contract
- Tool name: `sleep`
- Parameters: `{ "ms": number }` (`ms` is clamped to zero or greater before sleeping).
- Response: JSON string with the shape `{ "slept": <ms> }`.
- Behavior: the server awaits `Task.Delay(ms)` asynchronously, keeping the connection alive while the agent presents a streaming indicator such as `mcp/... ({"ms":4000}...)`.

## Error handling notes
- Non-integer input results in MCP validation errors before the tool executes.
- Negative values are treated as zero to prevent exceptions.
- If the process terminates mid-call, agents should surface a generic transport failure; retry logic is left to the caller.

## Sample interaction timeline

User:
```text
Make a joke with a punch line. Wait 4 seconds before the punchline.
```

Assistant:
```text
Why don't scientists trust atoms?

mcp/sleep ({"ms":4000}...)

Because they make up everything!
```

Keep this file handy when wiring the sleep tool into new agents so you can pick the right deployment path and understand the runtime expectations.

## Diagnosing issues
- `dotnet tool list --global | grep McpSleep` confirms the tool is installed.
- `mcp-sleep --help` checks that the command is on PATH and launches.
- Add the tools directory to PATH if needed:
	- macOS/Linux: `echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zprofile` then `source ~/.zprofile`.
	- Windows: add `%USERPROFILE%\.dotnet\tools` to the user PATH via System Properties, then restart your shell/app.
- `dotnet tool uninstall --global McpSleep.Tool` followed by `dotnet tool install --global McpSleep.Tool --add-source ./bin/Release` forces a rebuild and reinstall when you tweak the tool code.
- If your host still reports `ENOENT`, point the config at the absolute command (e.g., `$HOME/.dotnet/tools/mcp-sleep` or `%USERPROFILE%\.dotnet\tools\mcp-sleep.exe`).
- Local tool install: run `dotnet new tool-manifest`, then `dotnet tool install McpSleep.Tool --add-source ./dotnet-tool/bin/Release --version 0.1.0`, and use `dotnet tool run --tool-manifest /ABSOLUTE/PATH/TO/.config/dotnet-tools.json mcp-sleep` when the working directory differs.
- When debugging PATH issues, print the environment from the host (`echo $PATH` or `Get-ChildItem Env:PATH`) to verify the `.dotnet/tools` directory is present; add it and restart the host if missing.
- For command-not-found scenarios, the step-by-step checklist in `TROUBLESHOOTING.md` covers absolute command usage, invoking via `dotnet`, and manifest-based launches.


