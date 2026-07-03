using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.RevitSamples;

public static class ApplicationUtils
{
    /// <summary>
    /// Opens an RVT/RFA/RTE without blocking on dialogs as much as Revit allows in a full UI session.
    /// - Detaches (discard worksets) so "central / another user / permissions" are far less likely to block.
    /// - Closes all worksets on open to reduce link/workset churn.
    /// - Suppresses many dialogs via DialogBoxShowing.
    /// - Suppresses many warnings via FailuresProcessing.
    ///
    /// IMPORTANT:
    /// Dialog IDs vary by Revit version / language / context. This function logs every dialog ID it suppresses
    /// so you can add exact rules for your environment.
    /// </summary>
    public static Document OpenNoDialog(
        this UIApplication uiapp,
        string userVisiblePath,
        bool audit = false,
        bool detachFromCentralDiscardWorksets = true,
        bool closeAllWorksets = true,
        Action<string>? log = null)
    {
        if (uiapp == null) throw new ArgumentNullException(nameof(uiapp));
        if (string.IsNullOrWhiteSpace(userVisiblePath))
            throw new ArgumentException("Path is empty.", nameof(userVisiblePath));

        log ??= (_ => { });

        var app = uiapp.Application;

        // --- handlers we will attach temporarily ---
        var suppressedDialogs = new List<string>();
        var handledFailures = new List<string>();

        EventHandler<DialogBoxShowingEventArgs>? dialogHandler = null;
        EventHandler<FailuresProcessingEventArgs>? failuresHandler = null;

        dialogHandler = (sender, e) =>
        {
            try
            {
                // Works for both "TaskDialog" and Win32-style dialogs.
                // We will:
                //  1) Log dialog id so you can add exact routing
                //  2) Choose a "continue/OK/Yes" default rather than "Cancel"
                var id = e.DialogId ?? "(null)";
                suppressedDialogs.Add(id);

                // --- Task dialogs (most Revit prompts) ---
                if (e is TaskDialogShowingEventArgs t)
                {
                    // Log extra info if you want it (limited; message text is not exposed)
                    log($"[DialogBoxShowing][TaskDialog] Id={t.DialogId}");

                    // IMPORTANT: Dialog IDs differ; treat these as best-effort defaults.
                    // Use the log output to refine the switch for your specific prompts.
                    switch (t.DialogId)
                    {
                        // Upgrade prompts / schema updates (common-ish ids vary)
                        // Choose "Yes/OK" to proceed.
                        default:
                            // Prefer "Yes" then "Ok" then "Close". Many dialogs accept one of these.
                            // If the dialog doesn't support it, Revit may ignore it.
                            t.OverrideResult((int)TaskDialogResult.Yes);
                            break;
                    }

                    return;
                }

                // --- Native dialogs (rare, but happen) ---
                if (e is DialogBoxShowingEventArgs d)
                {
                    log($"[DialogBoxShowing][DialogBox] Id={d.DialogId}");

                    // Win32 dialog result codes are not standardized across all Revit dialogs.
                    // Empirically, 1 is often "OK" / "Yes" / "Continue". This is best-effort.
                    // Use logged Ids to override specific ones more carefully if needed.
                    d.OverrideResult(1);
                    return;
                }

                // Unknown type: do nothing (but we already logged id)
                log($"[DialogBoxShowing][Unknown] Id={id}");
            }
            catch (Exception ex)
            {
                log($"[DialogBoxShowing] handler exception: {ex}");
                // Do not throw from event handlers.
            }
        };

        failuresHandler = (sender, e) =>
        {
            try
            {
                var fa = e.GetFailuresAccessor();
                if (fa == null) return;

                var messages = fa.GetFailureMessages();
                if (messages == null || messages.Count == 0)
                {
                    e.SetProcessingResult(FailureProcessingResult.Continue);
                    return;
                }

                // Collect info for logging
                foreach (var m in messages)
                {
                    var defId = m.GetFailureDefinitionId();
                    var sev = m.GetSeverity();
                    handledFailures.Add($"{sev}: {defId.Guid}");
                }

                // Delete warnings to reduce blocking behavior.
                fa.DeleteAllWarnings();

                // If there are still errors, Revit may still abort opening.
                // We ask to continue; if it's unrecoverable, OpenDocumentFile will throw.
                e.SetProcessingResult(FailureProcessingResult.Continue);
            }
            catch (Exception ex)
            {
                log($"[FailuresProcessing] handler exception: {ex}");
            }
        };

        // Attach handlers (important: do this only around the open)
        uiapp.DialogBoxShowing += dialogHandler;
        app.FailuresProcessing += failuresHandler;

        try
        {
            // Convert user-visible path to a ModelPath
            ModelPath modelPath;
            if (userVisiblePath.StartsWith("RSN://", StringComparison.OrdinalIgnoreCase) ||
                userVisiblePath.StartsWith("BIM 360://", StringComparison.OrdinalIgnoreCase) ||
                userVisiblePath.StartsWith("ACC://", StringComparison.OrdinalIgnoreCase))
            {
                // If you're using cloud models, you should pass a proper ModelPath you obtained via API.
                // Best-effort fallback:
                throw new NotSupportedException("Cloud paths require a proper cloud ModelPath (Project/Model GUIDs).");
            }
            else
            {
                // Normal local / UNC paths
                var fullPath = Path.GetFullPath(userVisiblePath);
                modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fullPath);
            }

            // Build open options
            var openOpts = new OpenOptions
            {
                Audit = audit
            };

            if (detachFromCentralDiscardWorksets)
            {
                // This is the single most effective way to avoid “central/in use by another user” friction.
                openOpts.DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets;
            }

            if (closeAllWorksets)
            {
                var wkc = new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);
                openOpts.SetOpenWorksetsConfiguration(wkc);
            }

            log($"Opening: {userVisiblePath}");
            var doc = app.OpenDocumentFile(modelPath, openOpts);

            log($"Opened OK. Suppressed dialogs: {suppressedDialogs.Count}, failures seen: {handledFailures.Count}");
            return doc;
        }
        finally
        {
            // Always detach handlers
            if (dialogHandler != null) uiapp.DialogBoxShowing -= dialogHandler;
            if (failuresHandler != null) app.FailuresProcessing -= failuresHandler;

            /*
            // Optional: dump what happened so you can harden the switches above
            if (suppressedDialogs.Count > 0)
                log("Suppressed dialog ids:\n  " + string.Join("\n  ", suppressedDialogs.Distinct()));

            if (handledFailures.Count > 0)
                log("Failures encountered:\n  " + string.Join("\n  ", handledFailures.Distinct()));
            */
        }
    }
}