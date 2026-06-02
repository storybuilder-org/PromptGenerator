using System.Text.Json;

namespace PromptGeneratorPrototype;

/// <summary>
/// Prototype console app for StoryCAD Prompt Generator.
/// Demonstrates combinatorial prompt generation from StoryCAD data.
/// </summary>
class Program
{
    private static readonly Random _random = new();
    private static Dictionary<string, List<string>> _lists = new();
    private static ToolsData? _tools;
    private static ControlsData? _controls;

    static async Task Main(string[] args)
    {
        Console.WriteLine("StoryCAD Prompt Generator Prototype");
        Console.WriteLine("====================================\n");

        // Load data
        await LoadDataAsync();

        // Interactive loop
        while (true)
        {
            Console.WriteLine("\nSelect a prompt profile:");
            Console.WriteLine("  1. Minimal (Genre, Theme, Tone, Dramatic Situation)");
            Console.WriteLine("  2. Character-First (Archetype, Role, Trait, Wound, Motive)");
            Console.WriteLine("  3. Problem-First (Conflict, Goal, Opposition)");
            Console.WriteLine("  4. World-First (Genre, WorldType, Locale, Season, Tone)");
            Console.WriteLine("  5. Full Story Premise (Protagonist vs Antagonist)");
            Console.WriteLine("  6. Generate All Profiles");
            Console.WriteLine("  Q. Quit");
            Console.Write("\nChoice: ");

            var input = Console.ReadLine()?.Trim().ToUpperInvariant();

            Console.WriteLine();

            switch (input)
            {
                case "1":
                    GenerateMinimalPrompt();
                    break;
                case "2":
                    GenerateCharacterPrompt();
                    break;
                case "3":
                    GenerateProblemPrompt();
                    break;
                case "4":
                    GenerateWorldPrompt();
                    break;
                case "5":
                    GenerateFullPremise();
                    break;
                case "6":
                    GenerateAllPrompts();
                    break;
                case "Q":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static async Task LoadDataAsync()
    {
        Console.WriteLine("Loading data...");

        // Load Lists.json
        var listsJson = await File.ReadAllTextAsync("Data/Lists.json");
        _lists = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(listsJson) ?? new();
        Console.WriteLine($"  Loaded {_lists.Count} list categories");

        // Load Tools.json
        var toolsJson = await File.ReadAllTextAsync("Data/Tools.json");
        _tools = JsonSerializer.Deserialize<ToolsData>(toolsJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Console.WriteLine($"  Loaded {_tools?.MasterPlots?.Count ?? 0} master plots");
        Console.WriteLine($"  Loaded {_tools?.DramaticSituations?.Count ?? 0} dramatic situations");
        Console.WriteLine($"  Loaded {_tools?.StockScenes?.Count ?? 0} stock scene categories");

        // Load Controls.json
        var controlsJson = await File.ReadAllTextAsync("Data/Controls.json");
        _controls = JsonSerializer.Deserialize<ControlsData>(controlsJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Console.WriteLine($"  Loaded {_controls?.ConflictTypes?.Count ?? 0} conflict categories");

        Console.WriteLine("Data loaded successfully.");
    }

    static string PickRandom(string listName)
    {
        if (_lists.TryGetValue(listName, out var list) && list.Count > 0)
        {
            return list[_random.Next(list.Count)];
        }
        return $"[{listName} not found]";
    }

    static void GenerateMinimalPrompt()
    {
        Console.WriteLine("=== MINIMAL PROFILE ===\n");

        var genre = PickRandom("Genre");
        var theme = PickRandom("Theme");
        var tone = PickRandom("Tone");
        var situation = GetRandomDramaticSituation();

        // Show raw elements
        Console.WriteLine("Elements:");
        Console.WriteLine($"  Genre:     {genre}");
        Console.WriteLine($"  Theme:     {theme}");
        Console.WriteLine($"  Tone:      {tone}");
        Console.WriteLine($"  Situation: {situation.Name}");

        if (situation.Roles?.Count > 0)
        {
            Console.WriteLine($"  Roles:     {string.Join(", ", situation.Roles)}");
        }

        // Synthesized premise
        Console.WriteLine("\n--- STORY PREMISE ---");
        var roles = situation.Roles?.Count >= 2
            ? $"a {situation.Roles[0].ToLower()} and a {situation.Roles[1].ToLower()}"
            : "two characters locked in conflict";

        Console.WriteLine(WrapText(
            $"Write a {tone.ToLower()} {genre.ToLower()} story. " +
            $"The dramatic situation is \"{situation.Name}\" — involving {roles}. " +
            $"Through their struggle, the story explores the theme: \"{theme}\"",
            70));

        if (!string.IsNullOrEmpty(situation.Notes))
        {
            Console.WriteLine($"\nSituation context: {WrapText(situation.Notes, 70)}");
        }
    }

    static void GenerateCharacterPrompt()
    {
        Console.WriteLine("=== CHARACTER-FIRST PROFILE ===\n");

        var archetype = PickRandom("Archetype");
        var role = PickRandom("Role");
        var trait = PickRandom("Trait");
        var wound = PickRandom("Wound");
        var motive = PickRandom("Motive");

        // Show raw elements
        Console.WriteLine("Elements:");
        Console.WriteLine($"  Archetype: {archetype}");
        Console.WriteLine($"  Role:      {role}");
        Console.WriteLine($"  Trait:     {trait}");
        Console.WriteLine($"  Wound:     {wound}");
        Console.WriteLine($"  Motive:    {motive}");

        // Synthesized premise
        Console.WriteLine("\n--- STORY PREMISE ---");
        // Handle motives that may start with "to" or action verbs
        var motivePhrase = motive.ToLower();
        if (!motivePhrase.StartsWith("to ") && !motivePhrase.Contains("ing "))
            motivePhrase = $"pursue {motivePhrase}";
        Console.WriteLine(WrapText(
            $"When a {trait.ToLower()} {role.ToLower()} — shaped by the wound of " +
            $"\"{wound.ToLower()}\" — finds themselves driven to {motivePhrase}, " +
            $"they must confront both external obstacles and their own {archetype.ToLower()} nature. " +
            $"What happens when their deepest wound becomes the key to their transformation?",
            70));
    }

    static void GenerateProblemPrompt()
    {
        Console.WriteLine("=== PROBLEM-FIRST PROFILE ===\n");

        var conflict = GetRandomConflict();
        var goal = PickRandom("Goal");
        var opposition = PickRandom("Opposition");
        var outcome = PickRandom("Outcome");

        // Show raw elements
        Console.WriteLine("Elements:");
        Console.WriteLine($"  Conflict:    {conflict.Category} / {conflict.Subcategory}");
        Console.WriteLine($"  Example:     {conflict.Example}");
        Console.WriteLine($"  Goal:        {goal}");
        Console.WriteLine($"  Opposition:  {opposition}");
        Console.WriteLine($"  Outcome:     {outcome}");

        // Synthesized premise (follows StoryCAD premise template)
        Console.WriteLine("\n--- STORY PREMISE ---");
        var goalPhrase = goal.ToLower().StartsWith("to ") ? goal.ToLower() : $"to {goal.ToLower()}";
        Console.WriteLine(WrapText(
            $"When the protagonist seeks {goalPhrase}, they face a " +
            $"{conflict.Category.ToLower()} conflict: \"{conflict.Example}\" " +
            $"Standing in their way: {opposition.ToLower()}. " +
            $"After escalating confrontations, the story resolves when " +
            $"{outcome.ToLower()}.",
            70));

        Console.WriteLine("\nThis is your Story Problem. Now ask: Who is the protagonist? " +
            "Who or what is the antagonist? What are their competing goals?");
    }

    static void GenerateWorldPrompt()
    {
        Console.WriteLine("=== WORLD-FIRST PROFILE ===\n");

        var genre = PickRandom("Genre");
        var worldType = PickRandom("WorldType");
        var locale = PickRandom("Locale");
        var season = PickRandom("Season");
        var tone = PickRandom("Tone");
        var stockScene = GetRandomStockScene();

        // Show raw elements
        Console.WriteLine("Elements:");
        Console.WriteLine($"  Genre:       {genre}");
        Console.WriteLine($"  World Type:  {worldType}");
        Console.WriteLine($"  Locale:      {locale}");
        Console.WriteLine($"  Season:      {season}");
        Console.WriteLine($"  Tone:        {tone}");
        Console.WriteLine($"  Stock Scene: [{stockScene.Category}] {stockScene.Scene}");

        // Synthesized premise
        Console.WriteLine("\n--- STORY PREMISE ---");
        Console.WriteLine(WrapText(
            $"In a {worldType.ToLower()} world, {season.ToLower()} settles over {locale.ToLower()}. " +
            $"The {tone.ToLower()} {genre.ToLower()} story begins when: {stockScene.Scene.ToLower()}. " +
            $"What conflict emerges from this moment? Who is caught in its grip?",
            70));
    }

    static void GenerateFullPremise()
    {
        Console.WriteLine("=== FULL STORY PREMISE ===\n");
        Console.WriteLine("Following StoryCAD's Premise → Problem → Characters workflow:\n");

        // Genre and setting
        var genre = PickRandom("Genre");
        var locale = PickRandom("Locale");
        var tone = PickRandom("Tone");

        // Protagonist elements
        var protRole = PickRandom("Role");
        var protArchetype = PickRandom("Archetype");
        var protTrait = PickRandom("Trait");
        var protWound = PickRandom("Wound");
        var protGoal = PickRandom("Goal");

        // Antagonist elements
        var antagRole = PickRandom("Role");
        var antagArchetype = PickRandom("Archetype");
        var antagTrait = PickRandom("Trait");

        // Conflict
        var conflict = GetRandomConflict();
        var outcome = PickRandom("Outcome");
        var theme = PickRandom("Theme");

        // Show raw elements organized by story structure
        Console.WriteLine("STORY ELEMENTS:");
        Console.WriteLine($"  Genre/Tone:    {genre} / {tone}");
        Console.WriteLine($"  Setting:       {locale}");
        Console.WriteLine($"  Theme:         {theme}");
        Console.WriteLine();
        Console.WriteLine("PROTAGONIST:");
        Console.WriteLine($"  Role:          {protRole} ({protArchetype})");
        Console.WriteLine($"  Trait:         {protTrait}");
        Console.WriteLine($"  Wound:         {protWound}");
        Console.WriteLine($"  Goal:          {protGoal}");
        Console.WriteLine();
        Console.WriteLine("ANTAGONIST:");
        Console.WriteLine($"  Role:          {antagRole} ({antagArchetype})");
        Console.WriteLine($"  Trait:         {antagTrait}");
        Console.WriteLine($"  Opposition:    Blocks protagonist's goal");
        Console.WriteLine();
        Console.WriteLine("STORY PROBLEM:");
        Console.WriteLine($"  Conflict Type: {conflict.Category} / {conflict.Subcategory}");
        Console.WriteLine($"  Example:       {conflict.Example}");
        Console.WriteLine($"  Outcome:       {outcome}");

        // Synthesized full premise (StoryCAD template style)
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("STORY PREMISE");
        Console.WriteLine(new string('=', 60) + "\n");

        var protGoalPhrase = protGoal.ToLower().StartsWith("to ") ? protGoal.ToLower() : $"to {protGoal.ToLower()}";
        var premise = $"When a {protTrait.ToLower()} {protRole.ToLower()} ({protArchetype.ToLower()}) " +
            $"— still carrying the wound of \"{protWound.ToLower()}\" — " +
            $"seeks {protGoalPhrase}, " +
            $"they come into conflict with a {antagTrait.ToLower()} {antagRole.ToLower()} ({antagArchetype.ToLower()}) " +
            $"who stands in their way.\n\n" +
            $"Set in {locale.ToLower()}, this {tone.ToLower()} {genre.ToLower()} explores " +
            $"a {conflict.Category.ToLower()} conflict: \"{conflict.Example}\" " +
            $"Through their struggle, the story illuminates the theme: \"{theme}\"\n\n" +
            $"The story ends when {outcome.ToLower()}.";

        Console.WriteLine(WrapText(premise, 70));

        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("Next steps in StoryCAD:");
        Console.WriteLine("  1. Create a Story Overview and paste this premise");
        Console.WriteLine("  2. Create a Problem element for the Story Problem");
        Console.WriteLine("  3. Create Character elements for Protagonist and Antagonist");
        Console.WriteLine("  4. Link the characters to the Problem's Protagonist/Antagonist tabs");
    }

    static void GenerateAllPrompts()
    {
        GenerateMinimalPrompt();
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        GenerateCharacterPrompt();
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        GenerateProblemPrompt();
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        GenerateWorldPrompt();
        Console.WriteLine("\n" + new string('-', 50) + "\n");
        GenerateFullPremise();
    }

    static DramaticSituation GetRandomDramaticSituation()
    {
        if (_tools?.DramaticSituations == null || _tools.DramaticSituations.Count == 0)
        {
            return new DramaticSituation { Name = "[No dramatic situations loaded]" };
        }

        var keys = _tools.DramaticSituations.Keys.ToList();
        var key = keys[_random.Next(keys.Count)];
        return _tools.DramaticSituations[key];
    }

    static (string Category, string Subcategory, string Example) GetRandomConflict()
    {
        if (_controls?.ConflictTypes == null || _controls.ConflictTypes.Count == 0)
        {
            return ("[No conflicts loaded]", "", "");
        }

        var conflictType = _controls.ConflictTypes[_random.Next(_controls.ConflictTypes.Count)];
        var category = conflictType.Category ?? "Unknown";

        if (conflictType.SubCategories == null || conflictType.SubCategories.Count == 0)
        {
            return (category, "", "");
        }

        var subCategory = conflictType.SubCategories[_random.Next(conflictType.SubCategories.Count)];
        var subName = subCategory.Name ?? "Unknown";

        if (subCategory.Examples == null || subCategory.Examples.Count == 0)
        {
            return (category, subName, "");
        }

        var example = subCategory.Examples[_random.Next(subCategory.Examples.Count)];
        return (category, subName, example);
    }

    static (string Category, string Scene) GetRandomStockScene()
    {
        if (_tools?.StockScenes == null || _tools.StockScenes.Count == 0)
        {
            return ("[No stock scenes loaded]", "");
        }

        var keys = _tools.StockScenes.Keys.ToList();
        var category = keys[_random.Next(keys.Count)];
        var scenes = _tools.StockScenes[category];

        if (scenes == null || scenes.Count == 0)
        {
            return (category, "");
        }

        var scene = scenes[_random.Next(scenes.Count)];
        return (category, scene);
    }

    static string WrapText(string text, int width)
    {
        if (string.IsNullOrEmpty(text)) return "";

        // Simple word wrap
        var words = text.Replace("\n", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > width)
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = currentLine.Length == 0 ? word : currentLine + " " + word;
            }
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine);

        return string.Join("\n    ", lines);
    }
}

// Data models for JSON deserialization

public class ToolsData
{
    public Dictionary<string, DramaticSituation>? DramaticSituations { get; set; }
    public List<MasterPlot>? MasterPlots { get; set; }
    public Dictionary<string, List<string>>? StockScenes { get; set; }
    public List<MasterPlot>? BeatSheets { get; set; }
}

public class DramaticSituation
{
    public string Name { get; set; } = "";
    public string? Notes { get; set; }
    public List<string>? Roles { get; set; }
    public List<string>? Descriptions { get; set; }
    public List<string>? Examples { get; set; }
}

public class MasterPlot
{
    public string Name { get; set; } = "";
    public string? Notes { get; set; }
    public List<PlotScene>? Scenes { get; set; }
}

public class PlotScene
{
    public string Name { get; set; } = "";
    public string? Notes { get; set; }
}

public class ControlsData
{
    public List<ConflictType>? ConflictTypes { get; set; }
    public List<string>? RelationTypes { get; set; }
}

public class ConflictType
{
    public string? Category { get; set; }
    public List<ConflictSubCategory>? SubCategories { get; set; }
}

public class ConflictSubCategory
{
    public string? Name { get; set; }
    public List<string>? Examples { get; set; }
}
