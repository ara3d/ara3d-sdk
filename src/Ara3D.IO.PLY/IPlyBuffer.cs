namespace Ara3D.IO.PLY;

public interface IPlyBuffer
{
    string Name { get; }
    int Count { get; }
    void LoadValue(string s);
    void LoadValue(System.IO.BinaryReader br);
    int GetInt(int index);
    double GetDouble(int index);
}