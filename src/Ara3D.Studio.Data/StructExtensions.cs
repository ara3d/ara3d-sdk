namespace Ara3D.Studio.Data
{
    public static class StructExtensions
    {
        public static List<InstancedMeshStruct> InstancedMeshes(this IRenderScene self)
        {
            var r = new List<InstancedMeshStruct>();
            for (var i=0; i < self.InstanceGroups.Count; i++)
            {
                var group = self.InstanceGroups[i];
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

    }
}