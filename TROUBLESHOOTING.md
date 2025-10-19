# Troubleshooting MCP Sleep

Use this guide if the `mcp-sleep` command cannot be found or the server fails to start from a host such as LM Studio.

## 1. Confirm the tool is installed
- Run `dotnet tool list --global | grep McpSleep` to check the global install.
- For local installs, inspect `.config/dotnet-tools.json` and verify the entry for `mcpsleep.tool` exists.

## 2. Ensure the tool directory is on PATH
- **macOS/Linux:**
  ```bash
  echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zprofile
  source ~/.zprofile
  ```
- **Windows:** add `%USERPROFILE%\.dotnet\tools` to the user PATH via *System Properties → Environment Variables*, then restart the host application.
- Still unsure? Print the PATH seen by your host (`echo $PATH` or `Get-ChildItem Env:PATH`) and confirm it includes the tools directory.

## 3. Use an absolute path when PATH is not inherited
Some hosts ignore PATH. Point them directly at the shim:
- macOS/Linux: `/Users/<you>/.dotnet/tools/mcp-sleep`
- Windows: `%USERPROFILE%\.dotnet\tools\mcp-sleep.exe`

## 4. Run the tool through `dotnet`
If you prefer not to expose the shim, invoke the tool via the `dotnet` driver:
```json
{
  "mcpServers": {
    "mcp-sleep-dotnet-tool": {
      "command": "dotnet",
      "args": ["mcp-sleep"],
      "transport": { "stdio": {} }
    }
  }
}
```
This assumes the global tool is installed and `dotnet` is on PATH.

## 5. Use a local tool manifest explicitly
When the launching process runs outside the repository, `dotnet tool run` needs the manifest path:
```json
{
  "mcpServers": {
    "mcp-sleep-dotnet-tool": {
      "command": "dotnet",
      "args": [
        "tool",
        "run",
        "--tool-manifest",
        "/ABSOLUTE/PATH/TO/mcp-sleep-dotnet/.config/dotnet-tools.json",
        "mcp-sleep"
      ],
      "transport": { "stdio": {} }
    }
  }
}
```
Replace `/ABSOLUTE/PATH/...` with the real manifest location.

## 6. Reinstall after rebuilding
If you rebuilt the tool project, reinstall it:
```bash
cd dotnet-tool
rm -rf bin obj
dotnet pack McpSleep.Tool.csproj -c Release
# Global
dotnet tool uninstall --global McpSleep.Tool
dotnet tool install --global McpSleep.Tool --add-source ./bin/Release
# Local (optional)
dotnet tool install McpSleep.Tool --add-source ./bin/Release --version 0.1.0
```

Following these steps resolves the typical `ENOENT` and command-not-found errors encountered while integrating the MCP Sleep tool.
