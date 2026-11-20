# GitHub Copilot Instructions for Medical Bay (Continued) Mod

## Mod Overview and Purpose

**Medical Bay (Continued)** is a RimWorld mod that adds a medical bay building to your game. The medical bay tends and regenerates every unit in a room, reducing their scares and allowing you to configure it to prioritize certain patients. Additionally, the mod includes a filth compressor and draining tiles to keep rooms clean while consuming chemfuel.

## Key Features and Systems

### Medical Bay
- **Healing**: Tends and regenerates all units in the room.
- **Fuel**: Requires medifuel to operate.
- **Conditions**: Certain conditions must be met for full healing (e.g., building quality, therapeutic walls).
- **Placebo Effect**: Enhanced by therapeutic walls made of medistone.
- **Log Button**: Shows statistics and dependencies for healing.

### Filth Compressor
- **Cleanliness**: Removes filth from rooms with draining tiles.
- **Consumption**: Higher consumption with more draining tiles and distant filth.

### Draining Tiles
- **Cleanliness**: Keeps rooms clean by removing filth with the filth compressor.
- **Consumption**: Increases chemfuel consumption.

### Medifuel
- **Function**: Fuels the medical bay.
- **Recipe**: Requires megaspider meat, fuel, and healroot.
- **Advanced Recipes**: Unlockable with research.

### Medistone
- **Construction**: Required for building the medical bay.
- **Placebo Effect**: Enhances the medical bay's effects when used in therapeutic walls.
- **Building Materials**: Can be used to build beds and doors with special qualities.

## Coding Patterns and Conventions

1. **Mod Structure**: The mod is organized into C# files for functionality and XML files for defining game elements.
2. **Namespace**: Use `LTF_MedBay` as the main namespace.
3. **Coding Standards**: Follow RimWorld's coding standards and conventions, such as using `RimWorld` and `Verse` namespaces.
4. **Documentation**: Comment your code thoroughly, especially for complex methods and classes.
5. **Error Handling**: Implement error handling to manage unexpected situations gracefully.

## XML Integration

XML files are crucial for defining game elements. Ensure that:
- **Defs**: Properly define all `RecipeDef`, `ResearchProjectDef`, `ThingDef`, and other required definitions.
- **Attributes**: Use attributes accurately to specify properties like `researchPrerequisites`, `ingredients`, and `products`.
- **Naming**: Follow RimWorld's naming conventions for consistency.

## Harmony Patching

Harmony patching is used to modify existing game behavior. Key points include:
- **Target Methods**: Identify the methods you want to patch.
- **Patch Methods**: Create methods to replace or modify the target methods.
- **Prefix/Suffix**: Use `Prefix` or `Suffix` to modify behavior before or after the target method.
- **Testing**: Thoroughly test patches to ensure they work as expected.

## Suggestions for Copilot

1. **Code Completion**: Use Copilot to complete complex methods and classes, ensuring they follow coding patterns and conventions.
2. **XML Generation**: Utilize Copilot to generate XML definitions, such as `RecipeDef` and `ThingDef`.
3. **Patch Implementation**: Assist in implementing Harmony patches by generating the necessary target and patch methods.
4. **Documentation**: Help document code by generating comments for methods and classes.
5. **Testing**: Assist in writing unit tests for complex functionality.

By following these guidelines, you can effectively use GitHub Copilot to enhance your development process for the Medical Bay (Continued) mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

