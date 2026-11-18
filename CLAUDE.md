# PromptGenerator

The facts about this repo for an AI session. The development process is not in this file.

## What it is

A prototype console app for the StoryCAD Prompt Generator. It loads three JSON data files and prints story prompts from a menu of five profiles. `devdocs/README.md` says the Prompt Generator is planned as a Blazor WASM site in this repo.

## Parts

| Part | What it holds |
|------|---------------|
| `PromptGenerator/PromptGenerator.csproj` | The console app, .NET 10. One source file, `Program.cs` |
| `PromptGenerator/Data/` | `Controls.json`, `Lists.json`, `Tools.json` |
| `devdocs/` | Design and research notes |

The repo has no solution file and no tests. It states no build command.
