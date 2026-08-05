using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Ara3D.DoorClearance.Tests;

/// <summary>
/// Serializes verdicts to a deterministic CSV (culture-invariant, LF newlines, no timestamp) and
/// a human-readable run log. Only the CSV is hashed; the timestamp lives in the run log alone.
/// </summary>
public static class VerdictReport
{
    public static string VerdictText(Verdict v)
        => v switch
        {
            Verdict.Pass => "pass",
            Verdict.Fail => "fail",
            Verdict.NotApplicable => "not_applicable",
            Verdict.Inconclusive => "inconclusive",
            _ => throw new ArgumentOutOfRangeException(nameof(v)),
        };

    public static byte[] ToCsvBytes(IReadOnlyList<VerdictRecord> verdicts)
    {
        var sb = new StringBuilder();
        sb.Append("globalId,ruleId,verdict,evidence,citation\n");
        foreach (var v in verdicts)
            sb.Append(Field(v.GlobalId)).Append(',')
                .Append(Field(v.RuleId)).Append(',')
                .Append(VerdictText(v.Verdict)).Append(',')
                .Append(Field(v.Evidence)).Append(',')
                .Append(Field(v.Citation)).Append('\n');
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public static string Sha256Hex(byte[] bytes)
        => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    public static string BuildRunLog(string modelPath, ModelFacts facts, RuleSet rules,
        IReadOnlyList<VerdictRecord> verdicts, string csvSha256)
    {
        var counts = verdicts.GroupBy(v => v.Verdict).ToDictionary(g => g.Key, g => g.Count());
        var sb = new StringBuilder();
        sb.AppendLine("Ara3D.DoorClearance run log");
        sb.AppendLine($"timestampUtc: {DateTime.UtcNow:O}");
        sb.AppendLine($"model: {modelPath}");
        sb.AppendLine($"entityCount: {facts.EntityCount}");
        sb.AppendLine($"doorCount: {facts.Doors.Count}");
        sb.AppendLine($"obstacleCount: {facts.Obstacles.Count}");
        sb.AppendLine($"rules: {string.Join(", ", rules.Rules.Select(r => r.Id))}");
        foreach (var rule in rules.Rules)
            sb.AppendLine($"  {rule.Id}: {rule.Requirement.Kind} [{rule.Citation}]");
        sb.AppendLine($"verdicts: {verdicts.Count}");
        foreach (var v in Enum.GetValues<Verdict>())
            sb.AppendLine($"  {VerdictText(v)}: {counts.GetValueOrDefault(v, 0)}");
        sb.AppendLine($"verdicts.csv sha256: {csvSha256}");
        sb.AppendLine($"os: {RuntimeInformation.OSDescription}");
        sb.AppendLine($"dotnet: {Environment.Version}");
        return sb.ToString();
    }

    private static string Field(string text)
        => "\"" + text.Replace("\"", "\"\"") + "\"";
}
