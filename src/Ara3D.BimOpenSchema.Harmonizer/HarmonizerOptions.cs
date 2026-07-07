namespace Ara3D.BimOpenSchema.Harmonizer;

public record HarmonizerOptions
{
    /// <summary>
    /// Prefix for canonical parameter descriptor names (e.g. "Bos:Area").
    /// </summary>
    public string CanonicalPrefix { get; init; } = "Bos:";

    /// <summary>
    /// Descriptor group for all canonical parameters. Also used as the
    /// idempotency marker: data that already contains descriptors in this
    /// group is considered harmonized.
    /// </summary>
    public string CanonicalGroup { get; init; } = "Bos";

    /// <summary>
    /// When set, skips source detection and treats the data as coming from this generator.
    /// </summary>
    public SourceKind? SourceOverride { get; init; }

    /// <summary>
    /// Add a canonical category parameter (e.g. "Bos:Category" = "Wall") to every
    /// entity whose source category is recognized.
    /// </summary>
    public bool AddCanonicalCategories { get; init; } = true;

    /// <summary>
    /// Add canonical parameters (SI units) for mapped source parameters.
    /// </summary>
    public bool AddCanonicalParameters { get; init; } = true;
}
