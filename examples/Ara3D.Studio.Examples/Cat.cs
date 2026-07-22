namespace Ara3D.Studio.Samples;

/// <summary>
/// Menu category labels for example scripts. Values are the display text and may encode a
/// nested menu path with the shared <see cref="MenuPath.Delimiter"/> (" &gt; "), e.g.
/// "Experimental &gt; Demos". Named constants keep the enum's typo-safety and reuse while
/// allowing spaces, punctuation, and multi-level paths that C# enum identifiers forbid.
///
/// The first segment names what the tool <em>does</em> (function-first taxonomy); the
/// engine-fixed interface (IGenerator/IModifier/ITool) supplies the top-level menu. See
/// tracker/decisions/2026-07-21-generators-modifiers-menu-taxonomy.md.
/// </summary>
public static class Cat
{
    // Generators — what geometry is created.
    public const string Primitives = "Primitives";
    public const string Surfaces = "Surfaces";
    public const string Curves = "Curves";
    public const string Volumetric = "Volumetric";
    public const string Patterns = "Patterns";
    public const string Fields = "Fields";
    public const string Structures = "Structures";

    // Modifiers — how geometry is transformed.
    public const string Transform = "Transform";
    public const string Deform = "Deform";
    public const string Cleanup = "Cleanup";
    public const string Remesh = "Remesh";
    public const string Cut = "Cut";
    public const string Combine = "Combine";
    public const string Surface = "Surface";
    public const string Convert = "Convert";
    public const string Analyze = "Analyze";
    public const string Color = "Color";
    public const string Display = "Display";
    public const string Select = "Select";

    // Maturity escape hatch, one per kind. Deep nesting under it is expected.
    public const string Experimental = "Experimental";
    public const string ExperimentalDemos = Experimental + MenuPath.Delimiter + "Demos";
    public const string ExperimentalTests = Experimental + MenuPath.Delimiter + "Tests";
    public const string ExperimentalBim = Experimental + MenuPath.Delimiter + "BIM";
}
