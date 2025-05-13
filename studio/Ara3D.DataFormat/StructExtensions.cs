namespace Ara3D.Data
{
    public static class StructExtensions
    {
        public static unsafe List<InstancedMeshStruct> InstancedMeshes(this IRenderScene self)
        {
            var r = new List<InstancedMeshStruct>();
            for (var i=0; i < self.Groups.Count; i++)
            {
                var group = self.Groups[i];
                var mesh = self.Meshes[(int)group.MeshIndex];

                for (var j = 0; j < group.InstanceCount; j++)
                {
                    var instance = self.Instances[(int)group.BaseInstance + j];
                    r.Add(new InstancedMeshStruct(instance, mesh));
                }
            }

            return r;
        }

        public static long NumTriangles(this MeshSliceStruct self)
            => self.IndexCount / 3;

        public static long NumTriangles(this IRenderScene self)
            => self.InstancedMeshes().Sum(x => x.Mesh.NumTriangles());

        public static Bounds TotalBounds(this IRenderScene self)
            => self.Instances.Count == 0 ? Bounds.Empty : self.Instances.Select(x => x.Bounds)
                .Aggregate((a, b) => a.Union(b));
    }
}