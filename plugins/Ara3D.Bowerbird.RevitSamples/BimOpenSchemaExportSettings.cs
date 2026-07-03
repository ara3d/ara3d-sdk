
using Ara3D.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// Controls which elements, categories, and data are exported
/// when writing BIM Open Schema files.
/// </summary>
public sealed class BimOpenSchemaExportSettings
{
    // -----------------------------
    // Enums
    // -----------------------------

    /// <summary>
    /// Revit-like detail levels.
    /// Serialized as strings: "Coarse", "Medium", "Fine".
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DetailLevelEnum
    {
        Coarse = 1,
        Medium = 2,
        Fine = 3,
    }

    // -----------------------------
    // Serialization options
    // -----------------------------

    /// <summary>
    /// Canonical JSON options for this settings file.
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

    public string ToJsonString()
        => JsonSerializer.Serialize(this, JsonOptions);

    // -----------------------------
    // Non-serialized properties
    // -----------------------------

    /// <summary>
    /// Output folder is controlled externally, not by JSON.
    /// </summary>
    [JsonIgnore]
    public DirectoryPath Folder { get; set; } = DefaultFolder;

    // -----------------------------
    // Core switches
    // -----------------------------

    public bool IncludeLinks { get; set; } = true;
    public bool IncludeGeometry { get; set; } = true;
    public bool IncludeParameters { get; set; } = true;
    public bool IncludeAnnotationElements { get; set; } = false;
    
    public DetailLevelEnum DetailLevel { get; set; } = DetailLevelEnum.Fine;

    // -----------------------------
    // Class name filters
    // -----------------------------

    /// <summary>
    /// Wildcard patterns (e.g. "*Line*", "HVAC*") for class names to skip.
    /// </summary>
    public List<string> SkippedClassNames { get; set; } =
        new()
        {
            "*Line*",
            "*Point*",
            "Hub",
            "*Analytical*",
            "*Schedule*",
            "*Detail*",
            "*Arc*",
            "*Dimension*",
            "*Template*",
            "*Scheme*",
            "*Sequence*",
            "*Region*",
            "*Reference*",
            "*Style*",
            "*Origin*"
        };

    /// <summary>
    /// Explicit overrides that force inclusion even if skipped.
    /// </summary>
    public List<string> IncludedClassNamesOverride { get; set; } =
        new()
        {
            "Phase",
            "Level",
            "Grid",
            "ProjectInfo",
            "ProjectLocation",
            "BasePoint",
            "SurveyPoint",
            "DesignOption",
            "Workset",
        };

    // -----------------------------
    // Category filters
    // -----------------------------

    public List<string> SkippedCategories { get; set; } =
        new()
        {
            "*Line*",
            "*AnalyticalSurfaces*",
            "OST_WeakDims",
            "*Legend*",
            "*Dimensions*",
            "OST_PropertySet",
            "*Path*",
            "*Annotation*",
            "*Load*",
            "*Schedule*",
            "*Graphics*",
            "*Demand*",
            "*Marks*",
            "*Symbol*",
            "*Label*",
            "OST_SunStudy"
        };

    public List<string> IncludedCategoriesOverride { get; set; } =
        new();

    // -----------------------------
    // Defaults
    // -----------------------------

    public static DirectoryPath DefaultFolder =>
        SpecialFolders.MyDocuments.RelativeFolder("BIM Open Schema");

    // -----------------------------
    // Load / Save helpers
    // -----------------------------
        
    public static BimOpenSchemaExportSettings Load(string jsonPath)
    {
        if (!File.Exists(jsonPath))
            return new BimOpenSchemaExportSettings();

        var json = File.ReadAllText(jsonPath);

        // Deserialize into defaults (important!)
        var settings = JsonSerializer.Deserialize<BimOpenSchemaExportSettings>(
            json,
            JsonOptions);

        return settings ?? new BimOpenSchemaExportSettings();
    }

    public void Save(string jsonPath)
    {
        var json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(jsonPath, json);
    }

    public static string GetUserSettingsPath()
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(baseDir, "Ara 3D", "BimOpenSchema");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "ExportSettings.json");
    }

    public static BimOpenSchemaExportSettings LoadDefaultOrCreate()
    {
        var path = GetUserSettingsPath();
        if (!Path.Exists(path))
            SaveDefault();
        return Load(path);
    }

    public static void SaveDefault()
    {
        var r = new BimOpenSchemaExportSettings();
        r.Save(GetUserSettingsPath());
    }
}