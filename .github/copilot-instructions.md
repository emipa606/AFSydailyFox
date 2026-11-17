# GitHub Copilot Instructions for 【AF】SydailyFox (Continued)

## Mod Overview and Purpose

The 【AF】SydailyFox (Continued) mod is an update to the Amlifurxs mod, enhancing the gameplay experience by adding a cute, docile, and playful fox animal to RimWorld. This mod leverages the improved support for Asset Bundles in RimWorld 1.6 to ensure a smaller mod size compared to previous versions.

## Key Features and Systems

- **Animal**: A new fox animal with a specific attack style.
- **Special Attacks**: The fox can perform special range attacks.
- **Special Buildings**: Two new natural architecture buildings.
- **Special Event**: A new incident involving the fox.
- **Special Resource**: A new resource related to the fox.
- **Special Food**: A new food item.
- **New Joy Kind**: A new joy kind associated with the fox.
- **New Animal Nuzzle Kind**: A new interaction for the fox.
- **Player Benefits**: Additional sounds, music, special effects, and new signs.

## Coding Patterns and Conventions

1. **Namespace and Class Naming**: Use clear and descriptive namespaces and class names. For example, `AnimalProjectile`, `JobDriver_AnimalRangeAttack`, etc.
2. **Method Naming**: Use PascalCase for method names. For example, `Impact`, `Fire`, `TryMakePreToilReservations`.
3. **Variable Naming**: Use camelCase for variable names. For example, `bestAttackTarget`, `friendlyFireShootingTargetScoreOffset`.
4. **Comments**: Provide clear comments for complex logic and changes. Use XML comments for public API methods and properties.
5. **Error Handling**: Ensure proper error handling, especially for methods that interact with the game engine.

## XML Integration

1. **Def Files**: Definitions for new game elements such as animals, buildings, and incidents are defined in XML files. Ensure each def file follows the RimWorld XML schema.
2. **Def File Naming**: Use descriptive names for def files. For example, `AF_FailedExperiments.xml`, `AF_StrangeCapsule.xml`.
3. **Def File Structure**: Organize def files into categories (e.g., Buildings, Animals) for better maintainability.
4. **Def File Validation**: Regularly validate XML files to ensure they conform to the RimWorld schema.

## Harmony Patching

1. **Harmony Patches**: Use Harmony patches to modify existing game behavior. Follow the Harmony documentation for best practices.
2. **Patch Naming**: Use descriptive names for patch methods. For example, `Prefix_Pawn_TryGetAttackVerb`.
3. **Patch Location**: Place patch methods in dedicated classes to keep the code organized.
4. **Patch Testing**: Test patches thoroughly to ensure they do not introduce bugs or conflicts with other mods.

## Suggestions for Copilot

1. **Code Snippets**: Generate code snippets for common tasks such as creating new def files, implementing Harmony patches, and adding new animal behaviors.
2. **Example Files**: Provide example files for new animal definitions, Harmony patches, and XML files to serve as templates.
3. **Debugging Tips**: Offer tips and tricks for debugging Harmony patches and XML integration issues.
4. **Documentation Links**: Include links to the RimWorld API documentation and Harmony documentation for reference.
5. **Localization Guidance**: Provide guidance on adding new languages and localizing mod assets.

## Additional Notes

- **Dependencies**: Ensure all dependencies, such as `brrainz.harmony`, are included in the mod.
- **Localization**: The mod supports multiple languages, including English and Simplified Chinese. Ensure translations are up-to-date and accurate.
- **Error Reporting**: Use the Discord channel for error reporting and provide detailed logs for troubleshooting.

By following these guidelines, you can effectively use GitHub Copilot to enhance your modding experience for the 【AF】SydailyFox (Continued) mod.

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

