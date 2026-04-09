using Ara3D.Geometry;
using Ara3D.Studio.API;
using Ara3D.Studio.WpfControls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Ara3D.Bowerbird.RevitSamples;

public class UpArrow : IGenerator
{
    [Range(1, 32)] public int Count = 16;
    [Range(0f, 1f)] public float ShaftWidth = 0.01f;
    [Range(0f, 5f)] public float ShaftHeight = 0.8f;
    [Range(0f, 5f)] public float TipWidth = 0.2f;
    [Range(0f, 5f)] public float TipHeight = 0.2f;

    public QuadGrid3D Eval()
    {
        var totalHeight = ShaftHeight + TipHeight;
        var halfOutLine = new Point3D[]
        {
            (0, 0, 0),
            (ShaftWidth / 2, 0, 0),
            (ShaftWidth / 2, 0, ShaftHeight),
            (TipWidth / 2, 0, ShaftHeight),
            (0, 0, totalHeight),
        };

        return halfOutLine.Revolve(Vector3.UnitZ, Count);
    }
}

public class CommandDirectContextDemoArrowWithUI : NamedCommand, IDirectContext3DServer
{
    public override string Name => "Direct Context Demo - Draw Arrow with UI";

    public Guid ServerGuid { get; private set; } = Guid.NewGuid();
    public Outline m_boundingBox;
    public UpArrow Arrow = new();
    public PropertyControlContainer PropContainerUi;
    public UIApplication UiApp;
    public Window Window;
    public RenderMesh Mesh;

    public Guid GetServerId()
        => ServerGuid;

    public static Window CreatePropertyWindow(
        PropertyControlContainer control,
        string title = "Properties",
        double width = 400,
        double height = 600,
        Window? owner = null)
    {
        var window = new Window
        {
            Title = title,
            Width = width,
            Height = height,
            Content = control,
            Owner = owner,
            WindowStartupLocation = owner != null
                ? WindowStartupLocation.CenterOwner
                : WindowStartupLocation.CenterScreen
        };

        return window;
    }

    public static Window ShowPropertyWindow(
        PropertyControlContainer control,
        string title = "Properties",
        double width = 400,
        double height = 600,
        Window? owner = null)
    {
        var window = CreatePropertyWindow(control, title, width, height, owner);
        window.Show(); // modeless  
        return window;
    }

    public override void Execute(object argument)
    {
        Window?.Close();

        UiApp = (UIApplication)argument;

        //Set bounding box: TEMP, thi
        m_boundingBox = new Outline(new XYZ(0, 0, 0), new XYZ(10, 10, 10));

        PropContainerUi = new PropertyControlContainer();
        PropContainerUi.ConnectToModel(Arrow);
        
        Window = ShowPropertyWindow(PropContainerUi);

        RegisterSelfAsDirectContext3DServer(true);

        EvaluateMesh();
        PropContainerUi.Wrapper.Props.PropertyChanged += Props_PropertyChanged;
        Window.Closed += W_Closed;
    }

    public void RegisterSelfAsDirectContext3DServer(bool registerOrUnregister)
    {
        // Register this class as a server with the DirectContext3D service.
        var directContext3DService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        directContext3DService.AddServer(this);

        var msDirectContext3DService = directContext3DService as MultiServerService;
        if (msDirectContext3DService == null)
            throw new Exception("Expected a MultiServerService");

        // Get current list 
        var serverIds = msDirectContext3DService.GetActiveServerIds();

        if (registerOrUnregister)
        {
            serverIds.Add(ServerGuid);
        }
        else
        {
            serverIds.Remove(ServerGuid);
        }

        // Add the new server to the list of active servers.
        msDirectContext3DService.SetActiveServers(serverIds);
    }

    private void W_Closed(object sender, EventArgs e)
    {
        PropContainerUi.Wrapper.Props.PropertyChanged -= Props_PropertyChanged;
        RegisterSelfAsDirectContext3DServer(false);
        DeleteMeshStorage();
    }
    
    public void DeleteMeshStorage()
    {
        FaceBufferStorage?.Dispose();
        FaceBufferStorage = null;
        UiApp.ActiveUIDocument?.UpdateAllOpenViews();
    }

    public void EvaluateMesh()
    {
        var tmp = PropContainerUi.Wrapper.Evaluate();
        if (tmp is QuadGrid3D quadGrid3D)
        {
            Mesh = quadGrid3D.Triangulate().ToRenderMesh();
            UiApp.ActiveUIDocument?.UpdateAllOpenViews();
        }
        else
        {
            DeleteMeshStorage();
        }
    }

    private void Props_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        EvaluateMesh();
    }

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
    public BufferStorage FaceBufferStorage;

    public void RenderScene(View dBView, DisplayStyle displayStyle)
    {
        if (FaceBufferStorage == null)
            FaceBufferStorage = new BufferStorage(Mesh);
        else
            FaceBufferStorage.Update(Mesh);
        FaceBufferStorage?.Render();
    }
}