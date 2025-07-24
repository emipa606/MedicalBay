# GitHub Copilot Instructions for RimWorld Modding Project: LTF MedBay

## Mod Overview and Purpose

LTF MedBay is a RimWorld mod designed to enhance the medical capabilities of colonies by introducing advanced medical facilities, new functionalities for tending to and regenerating pawns, and improving overall medical efficiency during crises. The mod focuses on automating medical care, providing a streamlined healing process, and improving the recovery time of injured pawns, making colony management less stressful and more efficient during battles or hazardous events.

## Key Features and Systems

- **Advanced Medical Bay Components**: Including `Comp_LTF_MedBay` and `Comp_LTF_FilthCompressor`, which manage therapeutic processes and environment cleanliness respectively.
  
- **Healing Management**: A comprehensive `HealingManager` system that orchestrates regeneration and tending requests for pawns, managing medical tasks automatically based on predefined parameters.

- **Filth Compression**: Utilizing `Comp_LTF_FilthCompressor` to handle room filth and ensure a cleaner, healthier environment for recuperating pawns.

- **Graphic Overlays**: The `Graphic_EcgOverlay` provides a visual ECG representation for active medical beds, adding a thematic layer to medical activities.

- **Mod Compatibility Checks**: The `ModCompatibilityCheck` ensures that LTF MedBay integrates smoothly with other installed mods, avoiding potential conflicts and redundancies.

## Coding Patterns and Conventions

- **Class and Method Naming**: PascalCase is used for class names and method names, emphasizing clarity and readability.
  
- **Encapsulation**: Private helper methods are used, such as `MedBayInit()` and `CompressorInit()`, to encapsulate initialization logic that does not need to be exposed publicly.
  
- **Extension of Core Classes**: The mod extends core RimWorld classes like `ThingComp` to introduce new functionalities while leveraging existing game mechanics.
  
- **Static Utility Extensions**: Several static utility classes, such as `Tools` and `BuildingTools`, provide helper methods for common tasks shared across the mod.

## XML Integration

While the specific XML content is not provided, XML files are likely used in the mod to define:
- **Defining New Object Types**: XML files might be utilized to create new game objects like medical devices or room types.
- **Balance and Configuration**: Parameters for healing rates, power consumption, refueling, and other customizable settings could be defined in XML.
- **Harmony Patches**: XML may be used to outline patch targets and ensure Harmony patches are appropriately applied to desired game functions.

## Harmony Patching

Harmony is employed to patch RimWorld methods, enabling the mod to seamlessly integrate new functionalities without altering the game's source code directly. Thorough testing of Harmony patches is critical to prevent potential game-breaking bugs:
- **Targeting Specific Methods**: Harmony patches should specifically target problem areas or enhancement points in RimWorld's methods via annotations.
- **Pre and Postfix**: Utilize Harmony’s pre- and postfix techniques to inject mod logic before or after vanilla methods execute.

## Suggestions for Copilot

To enhance collaboration and efficiency when using GitHub Copilot in your development workflow, consider these suggestions:
- **Code Comments**: Maintain detailed inline comments and method summaries to guide Copilot’s context understanding.
- **Consistent Naming**: Ensure method names and variables are consistently descriptive to provide Copilot with meaningful context.
- **Use Copilot for Boilerplate Code**: Leverage Copilot for generating repetitive patterns, such as initializing properties or creating simple value checks.
- **Iterating Through Lists**: Use Copilot to aid in creating loops, especially when iterating over lists of pawns or game objects.
- **Testing Patterns**: Use Copilot for creating basic unit tests and integration tests to ensure mod stability with varied RimWorld configurations.

Adhering to these guidelines will facilitate the seamless integration of GitHub Copilot into your RimWorld mod development process, making your coding workflow more productive and efficient.
