using System;
using Autodesk.Revit.DB;

namespace Ara3D.Bowerbird.RevitSamples
{
    public static class CategoryExportUtil
    {
        public static bool ShouldSkipExport(
            BuiltInCategory bic,
            bool skip2dItems,
            bool skipViewItems,
            bool skipAnalyticalItems)
        {
            // If bic isn't defined, Enum.GetName returns null.
            var name = Enum.GetName(typeof(BuiltInCategory), bic) ?? bic.ToString();

            // 0) Hard skip: invalid / sentinel
            if (bic == BuiltInCategory.INVALID)
                return true;

            // 1) Hard skip: explicitly obsolete / deprecated / wrong range
            // (These are never worth exporting.)
            if (name.Contains("Obsolete", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Deprecated", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("ToBeDeprecated", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("IdInWrongRange", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("OBSOLETE", StringComparison.OrdinalIgnoreCase))
                return true;

            // ---- Helpers for "keep as data, not geometry" buckets ----

            // Connectivity / topology is BIM-relevant even if you skip geometry.
            // Keep it unless you explicitly decide to remove it elsewhere.
            bool isConnectivity =
                name.Contains("Connector", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("GraphicalWarning_OpenConnector", StringComparison.OrdinalIgnoreCase) ||
                bic == BuiltInCategory.OST_ConnectorElem ||
                bic == BuiltInCategory.OST_ConnectorElemXAxis ||
                bic == BuiltInCategory.OST_ConnectorElemYAxis ||
                bic == BuiltInCategory.OST_ConnectorElemZAxis ||
                bic == BuiltInCategory.OST_ElectricalConnector;

            // Point clouds are heavy, but valuable context.
            // Keep as "reference-only" (path/guid/transform/bounds) in your exporter.
            bool isPointCloud = (bic == BuiltInCategory.OST_PointClouds);

            // Schedules/templates are “document-like” data. Keep only if you want non-geometry BIM docs.
            bool isScheduleOrTemplate =
                name.Contains("Schedule", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Templates", StringComparison.OrdinalIgnoreCase) ||
                bic == BuiltInCategory.OST_Schedules ||
                bic == BuiltInCategory.OST_ScheduleGraphics ||
                bic == BuiltInCategory.OST_ScheduleViewParamGroup;

            // View/presentation state (can be useful; gate behind skipViewItems)
            bool isViewState =
                bic == BuiltInCategory.OST_Views ||
                bic == BuiltInCategory.OST_Viewports ||
                bic == BuiltInCategory.OST_ViewportLabel ||
                bic == BuiltInCategory.OST_Sheets ||
                bic == BuiltInCategory.OST_Cameras ||
                bic == BuiltInCategory.OST_Camera_Lines ||
                bic == BuiltInCategory.OST_SectionBox ||
                bic == BuiltInCategory.OST_Viewers;

            // 2) Hard keep: core 3D model categories that must NEVER be skipped by heuristics
            switch (bic)
            {
                case BuiltInCategory.OST_Walls:
                case BuiltInCategory.OST_Floors:
                case BuiltInCategory.OST_Roofs:
                case BuiltInCategory.OST_Ceilings:
                case BuiltInCategory.OST_Columns:
                case BuiltInCategory.OST_Doors:
                case BuiltInCategory.OST_Windows:
                case BuiltInCategory.OST_Ramps:
                case BuiltInCategory.OST_Stairs:
                case BuiltInCategory.OST_Railings:
                case BuiltInCategory.OST_CurtainWallMullions:
                case BuiltInCategory.OST_CurtainWallPanels:
                case BuiltInCategory.OST_GenericModel:

                // Structural “real” elements
                case BuiltInCategory.OST_StructuralFraming:
                case BuiltInCategory.OST_StructuralColumns:
                case BuiltInCategory.OST_StructuralFoundation:
                case BuiltInCategory.OST_StructuralTruss:
                case BuiltInCategory.OST_StructConnections:
                case BuiltInCategory.OST_Rebar:
                case BuiltInCategory.OST_FabricReinforcement:
                case BuiltInCategory.OST_FabricAreas:
                case BuiltInCategory.OST_StructuralTendons:

                // MEP “real” elements
                case BuiltInCategory.OST_PipeCurves:
                case BuiltInCategory.OST_PipeFitting:
                case BuiltInCategory.OST_PipeAccessory:
                case BuiltInCategory.OST_DuctCurves:
                case BuiltInCategory.OST_DuctFitting:
                case BuiltInCategory.OST_DuctAccessory:
                case BuiltInCategory.OST_DuctTerminal:
                case BuiltInCategory.OST_Conduit:
                case BuiltInCategory.OST_ConduitFitting:
                case BuiltInCategory.OST_CableTray:
                case BuiltInCategory.OST_CableTrayFitting:

                // Common placed components
                case BuiltInCategory.OST_MechanicalEquipment:
                case BuiltInCategory.OST_PlumbingFixtures:
                case BuiltInCategory.OST_LightingFixtures:
                case BuiltInCategory.OST_ElectricalEquipment:
                case BuiltInCategory.OST_ElectricalFixtures:
                case BuiltInCategory.OST_Furniture:
                case BuiltInCategory.OST_Casework:
                case BuiltInCategory.OST_SpecialityEquipment:
                    return false;
            }

            // 2b) Soft-keep buckets: keep as metadata/relations even if you don't render them
            // (You should still skip their geometry in the geometry exporter.)
            if (isConnectivity)
                return false;

            if (isPointCloud)
                return false;

            // Schedules/templates: export only if you are *not* skipping view/doc-like things
            if (isScheduleOrTemplate)
                return skipViewItems;

            // Views/presentation: export only if you want view state
            if (isViewState)
                return skipViewItems;

            // 3) “Probably skip”: drafting / tags / internal junk.

            // 3a) Tags and annotation-ish categories
            // NOTE: This intentionally still skips connector "Tags" (fine), while keeping connectors themselves above.
            if (name.EndsWith("Tags", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Tag", StringComparison.OrdinalIgnoreCase))
                return true;

            // 3b) Hidden lines, cut/projection, outlines, patterns: almost always view graphics
            if (name.Contains("HiddenLines", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("CutPattern", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("SurfacePattern", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Projection", StringComparison.OrdinalIgnoreCase) ||
                (name.Contains("Cut", StringComparison.OrdinalIgnoreCase) && name.Contains("Outlines", StringComparison.OrdinalIgnoreCase)) ||
                name.Contains("Outlines", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Crop", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Callout", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("SectionHead", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("TitleBlock", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("LeaderLine", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Matchline", StringComparison.OrdinalIgnoreCase))
                return true;

            // 3c) Pure 2D annotation / drafting primitives
            if (bic == BuiltInCategory.OST_TextNotes ||
                bic == BuiltInCategory.OST_FilledRegion ||
                bic == BuiltInCategory.OST_MaskingRegion ||
                bic == BuiltInCategory.OST_GenericAnnotation ||
                bic == BuiltInCategory.OST_RevisionClouds ||
                bic == BuiltInCategory.OST_RevisionCloudTags ||
                bic == BuiltInCategory.OST_SpotElevations ||
                bic == BuiltInCategory.OST_SpotCoordinates ||
                bic == BuiltInCategory.OST_SpotSlopes ||
                bic == BuiltInCategory.OST_SpotElevSymbols ||
                bic == BuiltInCategory.OST_SpotCoordinateSymbols ||
                bic == BuiltInCategory.OST_SpotSlopesSymbols ||
                bic == BuiltInCategory.OST_ElevationMarks)
                return skip2dItems;

            // 3d) Internal / UI / authoring artifacts (massive noise)
            if (name.StartsWith("OST_IOS", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("OST_DSR_", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("OST_XRay", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("CrashGraphics", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Regenerated", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("BackedUp", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("RegenerationFailure", StringComparison.OrdinalIgnoreCase))
                return true;

            // 3e) Compass / sun path visualization aids (keep only if exporting view graphics)
            if (name.Contains("Sun", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Compass", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Analemma", StringComparison.OrdinalIgnoreCase))
                return skipViewItems;

            // 3h) “Analytical” is optional: keep only if you want analysis model
            if (name.Contains("Analytical", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("gbXML", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("OST_GbXML", StringComparison.OrdinalIgnoreCase))
                return skipAnalyticalItems;

            // Default: don't skip.
            return false;
        }
    }
}
