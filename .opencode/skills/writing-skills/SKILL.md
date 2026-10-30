---
name: writing-skills
description: Rules and template for authoring and maintaining OpenCode skills in NetAdmin.
---

# Writing Skills

## Purpose
Ensure every skill works seamlessly in both **OpenCode** and **Visual Studio (GitHub Copilot)** while keeping context usage minimal and actionable.

## File Location & Naming
- Place each skill in `.opencode/skills/<kebab-case-name>/SKILL.md`.
- The directory name must match the `name:` field in the YAML frontmatter.

## Required Structure
1. **YAML Frontmatter**:
   ```yaml
   ---
   name: <kebab-case-name>
   description: <One sentence describing when and why to use this skill.>
   ---
   ```
2. **Concise English Body**:
   - Write strictly in **English**.
   - Keep the file short and scannable (aim for 20–45 lines, bullet points over prose).
   - State concrete rules, base classes, and file paths—avoid generic programming advice.
   - Enforce NetAdmin core principles: maximum code reuse in `NetAdmin.Core` and dedicated `NetAdmin.Providers.<Name>` projects.

## Registration Checklist
Whenever a skill is created, renamed, or removed:
1. Add or update its link in `AGENTS.md`.
2. Add or update its link in `.github/copilot-instructions.md`.
