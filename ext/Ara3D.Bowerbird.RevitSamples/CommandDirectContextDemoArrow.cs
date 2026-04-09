using System;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class CommandDirectContextDemoArrow : NamedCommand, IDirectContext3DServer
    {
        public override string Name => "Direct Context Demo - Draw Arrow";

        public Guid Guid { get; } = Guid.NewGuid();
        public Outline m_boundingBox;

        public static QuadGrid3D UpArrow(Number length, Number minorRadius, Number majorRadius, int radialSegments,
            Number percentTail)
        {
            var first = Vector3.Default;
            var last = Vector3.UnitZ * length;
            var mid = first.Lerp(last, percentTail);

            var a = first + -Vector3.UnitX * minorRadius;
            var b = mid + -Vector3.UnitX * minorRadius;
            var c = mid + -Vector3.UnitX * majorRadius;

            var profile = Intrinsics.MakeArray(first, a, b, c, last).Map(v => v.Point3D);
            var surface = SurfaceConstructors.Revolve(profile, Vector3.UnitZ, radialSegments);
            return surface;
        }

        public override void Execute(object argument)
        {
            var app = (UIApplication)argument;

            //Set bounding box: TEMP, thi
            m_boundingBox = new Outline(new XYZ(0, 0, 0), new XYZ(10, 10, 10));

            Mesh = UpArrow(10, 1, 2, 12, 0.75f).Triangulate().ToRenderMesh();
            if (Mesh == null)
                return;

            // Register this class as a server with the DirectContext3D service.
            var directContext3DService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            directContext3DService.AddServer(this);

            var msDirectContext3DService = directContext3DService as MultiServerService;
            if (msDirectContext3DService == null)
                throw new Exception("Expected a MultiServerService");

            // Get current list 
            var serverIds = msDirectContext3DService.GetActiveServerIds();
            serverIds.Add(GetServerId());

            // Add the new server to the list of active servers.
            msDirectContext3DService.SetActiveServers(serverIds);

            app.ActiveUIDocument?.UpdateAllOpenViews();
        }

        public Guid GetServerId() => Guid;
        public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
        public string GetName() => Name;
        public string GetVendorId() => "Ara 3D Inc.";
        public string GetDescription() => "Demonstrates using the DirectContext3D API";
        public bool CanExecute(View dBView) => dBView.ViewType == ViewType.ThreeD;
        public string GetApplicationId() => "Bowerbird";
        public string GetSourceId() => "";
        public bool UsesHandles() => false;
        public Outline GetBoundingBox(View dBView) => m_boundingBox;
        public bool UseInTransparentPass(View dBView) => true;
        public RenderMesh Mesh;
        public BufferStorage FaceBufferStorage;
        //public BufferStorage EdgeBufferStorage;

        public void RenderScene(View dBView, DisplayStyle displayStyle)
        {
            if (Mesh == null)
                return;
            if (FaceBufferStorage == null)
                FaceBufferStorage = new BufferStorage(Mesh);
            FaceBufferStorage.Render();
        }
    }
}
