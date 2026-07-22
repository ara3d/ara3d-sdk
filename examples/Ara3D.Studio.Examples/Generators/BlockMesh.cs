namespace Ara3D.Studio.Samples.Generators
{
    [Category(Cat.Structures)]
    public class BlockMesh : IGenerator
    {
        [Range(0f, 10f)] public float SizeX = 1;
        [Range(0f, 10f)] public float SizeY = 1;
        [Range(0f, 10f)] public float SizeZ = 1;

        public bool EmptyTop;
        public bool EmptyBottom;
        public bool EmptySides;

        public QuadMesh3D Eval()
        {
            var shape = new CellGridBuilder3D(3, 3, 3)
                .Remove(1, 1, 1);
            if (EmptyTop)
                shape.Remove(1, 1, 2);
            if (EmptyBottom)
                shape.Remove(1, 1, 0);
            if (EmptySides)
            {
                shape.Remove(1, 0, 1)
                    .Remove(1, 2, 1)
                    .Remove(0, 1, 1)
                    .Remove(2, 1, 1);
            }
            return shape.ToMesh().Scale((SizeX, SizeY, SizeZ));
        }
    }
}
