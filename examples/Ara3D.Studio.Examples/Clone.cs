using System;
using System.Linq;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;
using Plato;
using Plato.Geometry;

namespace Ara3D.Studio.Samples
{
    public class Clone : IModelModifier
    {
        public bool AtFaceCenters;

        public static Model3DNode ToNode(TriangleMesh3D mesh, Point3D position, Material mat)
            => new Model3DNode(Guid.NewGuid(), "", mesh, Matrix4x4.CreateTranslation(position), mat); 

        public Model3D Eval(Model3D m, EvalContext eval)
        {
            if (m.Nodes.Count == 0) return m;
            var mat = m.Nodes[0].Material;
            var instancedMesh = PlatonicSolids.TriangulatedCube;
            var mergedMesh = m.ToMesh();
            var points = AtFaceCenters ? mergedMesh.Faces.Map(f => f.Center) : mergedMesh.Points;
            return points.Select(p => ToNode(instancedMesh, p, mat)).ToList();
        }
    }
}
