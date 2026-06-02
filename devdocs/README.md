# Fiction Prompt Generator — design & research docs (PromptGenerator#1)

## Provenance — read this first
These documents did **not** originate in this repo. They were authored in the
**Collaborator** repo under `devdocs/issue_61_prompt_generator/` while the Prompt
Generator was tracked as **Collaborator#61**.

On **2026-05-05** the tracking issue was **transferred** Collaborator#61 →
**StoryCADAPI#11** ("Fiction Prompt Generator for StoryCAD") via the `transferIssue`
GraphQL mutation, and parented to umbrella **StoryCAD#1246** (Release 4.2) as the first
formal sub-issue under the new sub-issue policy.

On **2026-06-02** the issue and all design docs were migrated from StoryCADAPI to this
dedicated **PromptGenerator** repo, where they are now tracked as **PromptGenerator#1**.
The Prompt Generator is planned as a **Blazor WASM** site in this repo.

> **History note:** 3 of these files (`data_inventory.md`, `research.md`,
> `status_log.md`) had git history in Collaborator; the other 6 were never committed
> there. References to "Collaborator#61", "API-Samples repo", "issue #61", and
> "StoryCADAPI#11" inside these docs are historical — the current home is
> **PromptGenerator#1** in this repo.

## Contents
- `status_log.md` — session log / decisions (Phase: Research/Design)
- `research.md` — research notes + business context (Getting Attention lead-gen driver)
- `data_inventory.md` — StoryCAD data catalog usable for generation
- `interaction_design.md` — data → UI interaction patterns
- `broad_story_prompt.md`, `story_element_prompt.md` — prompt schema definitions
- `blazor_github_pages_deployment.md` — Blazor WASM → GitHub Pages deploy research
- `perplexity_prompt.md` — reusable universal LLM prompt template
- `prompt_generator_samples.md` — integration notes
