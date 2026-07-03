using System;

namespace Ara3D.Bowerbird.RevitSamples;

// https://portal.ogc.org/files/?artifact_id=829
// file:///C:/Users/cdigg/Downloads/99-049_OpenGIS_Simple_Features_Specification_For_SQL_Rev_1.1.pdf
public class FeatureGeometry
{
    bool Equals(FeatureGeometry fg) => throw new NotImplementedException();
    bool Disjoint(FeatureGeometry fg) => throw new NotImplementedException();
    bool Intersects(FeatureGeometry fg) => throw new NotImplementedException();
    bool Touches(FeatureGeometry fg) => throw new NotImplementedException();
    bool Crosses(FeatureGeometry fg) => throw new NotImplementedException();
    bool Within(FeatureGeometry fg) => throw new NotImplementedException();
    bool Contains(FeatureGeometry fg) => throw new NotImplementedException();
    bool Overlaps(FeatureGeometry fg) => throw new NotImplementedException();
    bool Relate(FeatureGeometry fg, string intersectionPatternMatrix) => throw new NotImplementedException();
    FeatureGeometry Buffer(double d) => throw new NotImplementedException();
    FeatureGeometry ConvexHull() => throw new NotImplementedException();
    FeatureGeometry Interesection(FeatureGeometry fg) => throw new NotImplementedException();
    FeatureGeometry Union(FeatureGeometry fg) => throw new NotImplementedException();
    FeatureGeometry Difference(FeatureGeometry fg) => throw new NotImplementedException();
    FeatureGeometry SymDifference(FeatureGeometry fg) => throw new NotImplementedException();
}