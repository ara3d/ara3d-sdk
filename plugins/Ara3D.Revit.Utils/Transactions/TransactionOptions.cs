using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Failure-handling policy for <see cref="Transactions.Run(Document, string, Action{Document}, TransactionOptions)"/>.
/// The default (no preprocessor, warnings not swallowed) is Revit's own default behavior —
/// nothing is hidden unless the caller opts in.
/// </summary>
public readonly record struct TransactionOptions(
    IFailuresPreprocessor? FailuresPreprocessor = null,
    bool SwallowWarnings = false);
