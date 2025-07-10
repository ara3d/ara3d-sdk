using System.IO;
using Ara3D.Utils;
using BIMOpenSchema;
using MessagePack;
using Newtonsoft.Json;

namespace Ara3D.DataSetBrowser.WPF;

public static class Serialization
{
    public static BIMData LoadBimDataFromJson(FilePath fp)
    {
        using var sr = new StreamReader(fp);
        using var jr = new JsonTextReader(sr);
        var js = JsonSerializer.Create();
        return js.Deserialize<BIMData>(jr);
    }

    public static BIMData LoadBimDataFromMessagePack(FilePath fp)
    {
        var bytes = fp.ReadAllBytes();
        var options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
        var obj = MessagePackSerializer.Typeless.Deserialize(bytes, options);
        return MessagePackDynamicConverter.ConvertTo<BIMData>(obj);
    }
}