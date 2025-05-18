namespace Ara3D.Studio.Data
{
    public struct InstancedMeshStruct
    {
        public readonly InstanceStruct Instance;
        public readonly MeshSliceStruct Mesh;
        public InstancedMeshStruct(InstanceStruct instance, MeshSliceStruct mesh)
        {
            Instance = instance;
            Mesh = mesh;
        }
    }
}