# MCP Sleep in .NET

This repository demonstrates two equivalent Model Context Protocol (MCP) servers written in C#. One keeps everything in a single file, while the other is packaged as an installable .NET global tool. Use whichever deployment style fits your environment.

![Terribly timed punchline!](img/terribly-timed-punchline.png)

**Figure: Without this MCP, joke with comedic timing fall short~**

![We can finally deliver a punchline with the right timing!](img/well-timed-punchline.png)

**Figure: With this MCP, we can finally do perfectly timed comedy! (and time callback and actions)**

## Project layout
- `single-file/mcp_sleep.cs` – a self-contained script that you run with the `dotnet` host; useful when you want zero project setup at the cost of referencing the full path.
- `dotnet-tool/` – a conventional .NET project that can be packed and installed as a global tool, giving you a clean command name (`mcp-sleep`).

## Option 1: single-file script
1. Resolve the absolute path to `single-file/mcp_sleep.cs`.
2. Register the server in your MCP configuration:
   ```json
   {
     "mcpServers": {
       "mcp-sleep-dotnet": {
         "command": "dotnet",
         "args": [
           "{{ABSOLUTE_PATH}}/single-file/mcp_sleep.cs"
         ]
       }
     }
   }
   ```
   Replace `{{ABSOLUTE_PATH}}` with the directory that contains this repository.
3. Call the tool from your agent. For example:
  ```text
  User: Make a joke with a punch line. Wait 4 seconds before the punchline.

  Assistant: Why don't scientists trust atoms?
  mcp/mcp-sleep-dotnet ({"ms":4000}...)
  Assistant: Because they make up everything!
  ```

## Option 2: installable .NET tool
1. Change into the tool project:
   ```bash
   cd dotnet-tool
   ```
2. Pack the tool (outputs into `dotnet-tool/bin/Release`):
   ```bash
   dotnet pack McpSleep.Tool.csproj -c Release
   ```
3. Install the tool globally from the freshly built package:
   ```bash
   dotnet tool install --global McpSleep.Tool --add-source ./bin/Release
   ```
  - To update after rebuilding, run `dotnet tool uninstall --global McpSleep.Tool`, then repeat the install command.
4. Ensure your shell (and any GUI hosts) can find the tool:
   - **macOS/Linux (zsh/bash):** add `export PATH="$PATH:$HOME/.dotnet/tools"` to `~/.zprofile` or `~/.bashrc`, then reload the shell (`source ~/.zprofile` or open a new terminal).
   - **Windows (PowerShell):** run `dotnet tool install --global ...` from an elevated prompt and add `%USERPROFILE%\.dotnet\tools` to the user PATH via *System Properties → Environment Variables*, then restart the app.
5. Point your MCP configuration at the new command:
   ```json
   {
     "mcpServers": {
       "sleep": {
         "command": "mcp-sleep",
         "transport": { "stdio": {} }
       }
     }
   }
   ```
   - If your host does not inherit the updated PATH, reference the absolute command (for example: `/Users/<you>/.dotnet/tools/mcp-sleep` on macOS or `%USERPROFILE%\.dotnet\tools\mcp-sleep.exe` on Windows).
   - Need more troubleshooting (PATH checks, local tool manifest, etc.)? See `TROUBLESHOOTING.md`.

Either version exposes the same `sleep` tool that delays for the requested number of milliseconds and reports how long it slept. Pick the one that best matches your deployment workflow.
