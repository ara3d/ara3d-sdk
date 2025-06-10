using Ara3D.Geometry;

namespace Ara3D.IO.PLY;

public static class PlyImporter
{
    public static PlyBuffer CreateBuffer(string s, int size, string name)
    {
        switch (s.ToLowerInvariant())
        {
            case "uchar": return new UInt8Buffer(size, name);
            case "uint8": return new UInt8Buffer(size, name);
            case "byte": return new UInt8Buffer(size, name);
            case "ubyte": return new UInt8Buffer(size, name);
            case "char": return new UInt8Buffer(size, name);

            case "int8": return new Int8Buffer(size, name);
            case "sbyte": return new Int8Buffer(size, name);

            case "ushort": return new UInt16Buffer(size, name);
            case "uint16": return new UInt16Buffer(size, name);

            case "short": return new Int16Buffer(size, name);
            case "int16": return new Int16Buffer(size, name);

            case "uint32": return new UInt32Buffer(size, name);
            case "uint": return new UInt32Buffer(size, name);

            case "int": return new Int32Buffer(size, name);
            case "int32": return new UInt32Buffer(size, name);

            case "float": return new SingleBuffer(size, name);
            case "float32": return new SingleBuffer(size, name);
            case "single": return new SingleBuffer(size, name);

            case "double": return new DoubleBuffer(size, name);
            case "float64": return new DoubleBuffer(size, name);

            default: throw new Exception("bad PLY format " + s);
        }
    }

    public static IReadOnlyList<string> SplitStrings(string line)
        => line.Split(new[] { ' ', ',', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries);


    public static string ReadLineFromFileStream(FileStream fs)
    {
        var bytes = new List<byte>();
        int b;
        while ((b = fs.ReadByte()) != -1)
        {
            if (b == '\n')
                break;
            if (b == '\r')
            {
                int next = fs.ReadByte();
                if (next != '\n' && next != -1)
                    fs.Position--;
                break;
            }
            bytes.Add((byte)b);
        }
        if (bytes.Count == 0 && b == -1)
            return null;
        return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
    }

    public static TriangleMesh3D LoadMesh(string fileName)
        => LoadBuffers(fileName).ToMesh();

    public static List<IPlyBuffer> LoadBuffers(string fileName)
    {
        var vertexBuffers = new List<PlyBuffer>();
        PlyBuffer faceSizeBuffer = null;
        ListBuffer indexBuffer = null;
        var faceBuffers = new List<PlyBuffer>();
        var materialBuffers = new List<PlyBuffer>();
        var fmt = "";

        var num_materials = 0;
        var num_vertices = 0;
        var num_faces = 0;
        var cur_element = "";

        using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            // Parse the ASCII text at the start up to "end_header\n"
            var done = false;
            while (!done)
            {
                var line = ReadLineFromFileStream(file);
                var words = SplitStrings(line);

                if (words.Count == 0)
                    continue;

                switch (words[0])
                {
                    case "ply":
                        break;
                    case "comment":
                        break;
                    case "format":
                        fmt = words[1];
                        break;
                    case "element":
                    {
                        switch (cur_element = words[1])
                        {
                            case "vertex":
                                num_vertices = int.Parse(words[2]);
                                break;
                            case "face":
                                num_faces = int.Parse(words[2]);
                                break;
                            case "tristrips":
                                num_faces = int.Parse(words[2]);
                                break;
                            case "material":
                                num_materials = int.Parse(words[2]);
                                break;
                            default:
                                throw new Exception($"Bad PLY element: {line}");
                        }
                        break;
                    }
                    case "property":
                    {
                        if (cur_element == "vertex")
                        {
                            if (words.Count < 3)
                                throw new Exception($"Bad PLY property: {line}");

                            vertexBuffers.Add(CreateBuffer(words[1], num_vertices, words[2]));
                        }
                        else if (cur_element == "face" || cur_element == "tristrips")
                        {
                            if (words[1] == "list")
                            {
                                if (words.Count != 5 && words[4] != "vertex_indices")
                                    throw new Exception("Only vertex_indices support being in a list");

                                if (indexBuffer != null || faceSizeBuffer != null)
                                    throw new Exception("Already found a face size or index buffer");

                                faceSizeBuffer = CreateBuffer(words[2], num_faces, "face_sizes");
                                indexBuffer =
                                    new ListBuffer(CreateBuffer(words[3], num_faces, "vertex_indices"));
                            }
                            else
                            {
                                if (indexBuffer == null || faceSizeBuffer == null)
                                    throw new Exception(
                                        "The vertex_indices should be before other face properties");

                                faceBuffers.Add(CreateBuffer(words[1], num_faces, words[2]));
                            }
                        }
                    }
                        break;
                    case "end_header":
                        done = true;
                        break;

                    default:
                        throw new Exception($"bad PLY field: {line}");
                }
            }

            if (faceSizeBuffer == null)
                throw new Exception("No face size buffer found");
            if (indexBuffer == null)
                throw new Exception("No index buffer found");
            if (vertexBuffers.Count == 0)
                throw new Exception("No vertex buffers found");

            if (fmt == "ascii")
            {
                using (var streamReader = new StreamReader(file))
                {

                    for (var i = 0; i != num_vertices; ++i)
                    {
                        var line = streamReader.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected end of file");

                        var values = SplitStrings(line);

                        if (values.Count != vertexBuffers.Count)
                            throw new Exception(
                                $"bad PLY vertex line, expected {vertexBuffers.Count} properties, but found {values.Count}");

                        var index = 0;
                        foreach (var buffer in vertexBuffers)
                            buffer.LoadValue(values[index++]);
                    }

                    for (var i = 0; i != num_faces; ++i)
                    {
                        var line = streamReader.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected end of file");

                        var values = SplitStrings(line);
                        faceSizeBuffer.LoadValue(values[0]);
                        var cnt = faceSizeBuffer.RecentInt;
                        if (cnt < 0)
                            throw new Exception("Could not determine face size");

                        if (values.Count != cnt + 1)
                            throw new Exception($"Expected {cnt + 1} values, but foumd {values.Count}");

                        for (var j = 0; j != cnt; ++j)
                            indexBuffer.LoadValue(values[j + 1]);

                        if (faceBuffers.Count > 0)
                            throw new Exception("Face buffers not yet implemented");
                    }

                    for (var i = 0; i != num_materials; ++i)
                    {
                        var line = streamReader.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected end of file");

                        var values = SplitStrings(line);
                        faceSizeBuffer.LoadValue(values[0]);

                        if (values.Count != materialBuffers.Count)
                            throw new Exception(
                                $"bad PLY vertex line, expected {materialBuffers.Count} properties, but found {values.Count}");

                        var index = 0;
                        foreach (var buffer in materialBuffers)
                            buffer.LoadValue(values[index++]);
                    }
                }
            }
            else if (fmt == "binary_little_endian")
            {
                using (var binaryReader = new System.IO.BinaryReader(file))
                {
                    for (var i = 0; i != num_vertices; ++i)
                    {
                        foreach (var buffer in vertexBuffers)
                            buffer.LoadValue(binaryReader);
                    }

                    for (var i = 0; i != num_faces; ++i)
                    {
                        faceSizeBuffer.LoadValue(binaryReader);
                        var cnt = faceSizeBuffer.RecentInt;
                        if (cnt < 0)
                            throw new Exception("Could not determine face size");

                        for (var j = 0; j != cnt; ++j)
                            indexBuffer.LoadValue(binaryReader);

                        foreach (var faceBuffer in faceBuffers)
                            faceBuffer.LoadValue(binaryReader);
                    }

                    for (var i = 0; i != num_materials; ++i)
                    {
                        foreach (var buffer in materialBuffers)
                            buffer.LoadValue(binaryReader);
                    }

                }
            }
            else
            {
                throw new Exception($"Unrecognized PLY format {fmt}");
            }
        }

        var allBuffers = new List<IPlyBuffer>();
        allBuffers.AddRange(vertexBuffers);
        allBuffers.Add(faceSizeBuffer);
        allBuffers.Add(indexBuffer);
        allBuffers.AddRange(faceBuffers);
        allBuffers.AddRange(materialBuffers);
        return allBuffers;
    }

    public static TriangleMesh3D ToMesh(this IReadOnlyList<IPlyBuffer> buffers)
    {
        var xs = buffers.First(b => b.Name == "x");
        var ys = buffers.First(b => b.Name == "y");
        var zs = buffers.First(b => b.Name == "z");

        if (xs == null) throw new Exception("Missing x property");
        if (ys == null) throw new Exception("Missing y property");
        if (zs == null) throw new Exception("Missing z property");

        // TODO: normals / colors / uv

        var vertices = new List<Vector3>();
        for (var i = 0; i != xs.Count; ++i)
            vertices.Add(new Vector3((float)xs.GetDouble(i), (float)ys.GetDouble(i), (float)zs.GetDouble(i)));

        var indexBuffer = buffers.First(b => b.Name == "vertex_indices");

        var faceSizes = buffers.First(b => b.Name == "face_sizes");
        var numFaces = faceSizes.Count;
        var cur = 0;

        var indices = new List<Integer3>();
        for (var f = 0; f < numFaces; f++)
        {
            var cnt = faceSizes.GetInt(f);
            for (var i = 0; i < cnt - 2; i++)
            {
                indices.Add(new Integer3(
                    indexBuffer.GetInt(cur + i),
                    indexBuffer.GetInt(cur + i + 1),
                    indexBuffer.GetInt(cur + i + 2)));
            }

            cur += cnt;
        }

        return new TriangleMesh3D(vertices.Map(v => v.Point3D), indices);
    }
}