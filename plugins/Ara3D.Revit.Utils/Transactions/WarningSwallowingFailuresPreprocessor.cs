using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Deletes every warning-severity failure and continues; errors are left untouched and
/// still roll the transaction back. Backs <see cref="TransactionOptions.SwallowWarnings"/>
/// so the common "I don't care about this warning" case needs no custom preprocessor.
/// </summary>
sealed class WarningSwallowingFailuresPreprocessor : IFailuresPreprocessor
{
    public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
    {
        foreach (var failure in failuresAccessor.GetFailureMessages())
            if (failure.GetSeverity() == FailureSeverity.Warning)
                failuresAccessor.DeleteWarning(failure);

        return FailureProcessingResult.Continue;
    }
}
