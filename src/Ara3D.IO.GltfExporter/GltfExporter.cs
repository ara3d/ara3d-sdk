using Newtonsoft.Json;
using System.Text;

namespace Ara3D.IO.GltfExporter;

public static class GltfExporter
{
    public static byte[] Magic = [0x67, 0x6C, 0x54, 0x46];
    public static byte[] Version = [0x02, 0x00, 0x00, 0x00];
    public static byte[] BinChunkType = [0x42, 0x49, 0x4e, 0x00];
    public static byte[] JsonChunkType = [0x4a, 0x53, 0x4f, 0x4e];

    public static byte[] GetJsonBytes(string json)
    {
        // Convert JSON to UTF-8 byte array (DO THIS FIRST)
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        // Calculate padding needed to align to 4 bytes
        var padding = (4 - (jsonBytes.Length % 4)) % 4;

        // Allocate new array for padded JSON
        var paddedJsonBytes = new byte[jsonBytes.Length + padding];
        Array.Copy(jsonBytes, paddedJsonBytes, jsonBytes.Length);

        // Pad with spaces (0x20)
        for (var i = jsonBytes.Length; i < paddedJsonBytes.Length; i++)
        {
            paddedJsonBytes[i] = 0x20;
        }

        return GetChunkBytes(paddedJsonBytes, JsonChunkType);
    }

    public static byte[] GetChunkBytes(IReadOnlyList<byte> chunkData, byte[] chunkType)
        => BitConverter
            .GetBytes(Convert.ToUInt32(chunkData.Count))
            .Concat(chunkType)
            .Concat(chunkData)
            .ToArray();

    public static void Export(this GltfData data, string filePath)
    {
        var json = JsonConvert.SerializeObject(data, 
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        var jsonChunk = GetJsonBytes(json);
        var binChunk = data.bytes;

        var lengthBytes = BitConverter.GetBytes(Convert.ToUInt32(jsonChunk.Length + binChunk.Count + 12));
        var headerChunk = Magic.Concat(Version).Concat(lengthBytes).ToArray();

        var exportArray = headerChunk
            .Concat(jsonChunk)
            .Concat(binChunk)
            .ToArray();
        File.WriteAllBytes(filePath, exportArray);
    }
}