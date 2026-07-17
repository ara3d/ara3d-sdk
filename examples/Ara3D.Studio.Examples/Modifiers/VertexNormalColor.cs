namespace Ara3D.Studio.Samples.Modifiers;

public enum VertexNormalColorScheme
{
    /// <summary>Map each normal component from [-1, 1] to [0, 1] on R, G, and B.</summary>
    RgbSigned,

    /// <summary>Map the absolute value of each normal component to R, G, and B.</summary>
    RgbAbsolute,

    /// <summary>Blend between two colors based on alignment with a reference direction.</summary>
    FacingDirection,

    /// <summary>Map horizontal facing angle to hue and the Z component to brightness.</summary>
    CyclicHue,
}

[Category(nameof(Categories.Meshes))]
public class VertexNormalColor : IModifier
{
    public VertexNormalColorScheme Scheme { get; set; } = VertexNormalColorScheme.RgbSigned;

    public Vector3 ReferenceDirection { get; set; } = Vector3.UnitY;
    public Vector3 FacingAwayColor { get; set; } = new(0, 0, 1);
    public Vector3 FacingTowardColor { get; set; } = new(1, 0, 0);

    public static Vector3 SignedNormalToRgb(Vector3 normal)
        => (normal + Vector3.One) * 0.5f;

    public static Vector3 AbsoluteNormalToRgb(Vector3 normal)
        => normal.Abs;

    public Vector3 FacingDirectionColor(Vector3 normal)
    {
        var reference = ReferenceDirection.Normalize;
        var alignment = Math.Clamp((normal.Dot(reference) + 1) * 0.5f, 0, 1);
        return FacingAwayColor.Lerp(FacingTowardColor, alignment);
    }

    public static Vector3 CyclicHueColor(Vector3 normal)
    {
        var hue = (float)(normal.Y.Atan2(normal.X).Turns + 0.5);
        var brightness = Math.Clamp((float)(normal.Z * 0.5 + 0.5), 0, 1);
        var color = Palettes.Phase.GetColors().GetColor(hue);
        return color.ToVector3() * brightness;
    }

    public Vector3 GetColor(Vector3 normal)
        => Scheme switch
        {
            VertexNormalColorScheme.RgbSigned => SignedNormalToRgb(normal),
            VertexNormalColorScheme.RgbAbsolute => AbsoluteNormalToRgb(normal),
            VertexNormalColorScheme.FacingDirection => FacingDirectionColor(normal),
            VertexNormalColorScheme.CyclicHue => CyclicHueColor(normal),
            _ => SignedNormalToRgb(normal),
        };

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext context)
    {
        var normals = mesh.DerivedVertexNormals();
        var colors = normals.Select(GetColor);
        return mesh.ToColored(colors);
    }
}
