# GitHub Copilot Instructions for the 【AF】SydailyFox (Continued) Mod

## Mod Overview and Purpose

The 【AF】SydailyFox (Continued) mod is a continuation and update of Amlifurx's original mod. This mod introduces various new elements to RimWorld, including new animals, items, buildings, and events, enhancing gameplay with unique interactions and challenges. The main attraction of this mod is the introduction of the Sydaily Fox, a playful and resourceful creature inhabiting polar ecosystems.

## Key Features and Systems

- **New Animal**: SydailyFox, a peculiar fox with a distinct attack style.
- **Special Attacks**: Two unique attacks that the SydailyFox can perform.
- **New Buildings**: Integration of natural architecture and additional construction options.
- **Special Event**: An intriguing incident event to enrich game dynamics.
- **Unique Resource and Food**: Adds diversity to resources and consumables.
- **Joy and Interaction**: A new joy activity and an animal nuzzling interaction for player experiences.
- **Sound and Visual Effects**: Including soundtracks and special visual signs.

## Coding Patterns and Conventions

- The mod is developed using C# with the .NET Framework versions 4.7.2 and 4.8.
- Naming conventions follow a camelCase and PascalCase pattern for class names and method names, respectively.
- Consistent use of internal and public access modifiers to manage visibility and encapsulation.

## XML Integration

- XML files are likely used to define game data such as animals, buildings, items, and events.
- Ensure proper linkage between XML definitions and C# classes to maintain consistent game mechanics.
- Follow the RimWorld modding convention for XML schema to achieve seamless in-game integration.

## Harmony Patching

- The mod employs Harmony for patching to modify existing game behavior.
- Classes like `ARA__ManHunter_Patch`, `ARA__VerbCheck_Patch`, and `ARA_FightAI_Patch` show examples of how to safely insert new logic into the game.
- Use HarmonyPrefix and HarmonyPostfix attributes for injecting custom logic into existing RimWorld behaviors.

## Suggestions for Copilot

1. **Animal Logic**: Focus on methods related to animal behavior, such as those in `AnimalProjectile` and `AnimalRangeAttack_Init`, to ensure the SydailyFox's unique attack logic is implemented correctly.

2. **Event System**: Utilize the `IncidentWorker_StrangeCapsule` to derive new incident scenarios, exploring how events spawn and interact with the game world.

3. **Building and PlaceWorker**: Leverage the `PlaceWorker_MegaStatue` class to introduce new construction and placement mechanics.

4. **Thought Workers**: Expand on thought worker classes like `ThoughtWorker_FoxGoodThink` to enrich character interactions and thoughts in relation to the mod's content.

5. **Animations and Effects**: Enhance special effect logic within `AF_PlaySpecialAnimation` and related classes, ensuring synchronization with game events for immersive experiences.

By following these guidelines and exploring the existing class structure, Copilot can help further evolve the mod and integrate new, compelling features. Always ensure compatibility with base game mechanics and other mods where possible.
