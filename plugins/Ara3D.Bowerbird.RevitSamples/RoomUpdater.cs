using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

public class RoomUpdater : IUpdater
{
    private readonly UpdaterId _id;
    private readonly AddInId _appId;

    public Action<IEnumerable<ElementId>> ChangedElementsAction;

    public RoomUpdater(AddInId id, Action<IEnumerable<ElementId>> changedElementsAction)
    {
        _appId = id;
        _id = new UpdaterId(_appId, new Guid("FBFBF6B2-4C06-42d4-97C1-D1B4EB593EFF"));
        ChangedElementsAction = changedElementsAction;
    }

    public void Execute(UpdaterData data)
    {
        var elements = data.GetAddedElementIds()
            .Concat(data.GetDeletedElementIds())
            .Concat(data.GetModifiedElementIds());
        ChangedElementsAction(elements);
    }

    public ChangePriority GetChangePriority() 
        => ChangePriority.FloorsRoofsStructuralWalls;

    public UpdaterId GetUpdaterId() 
        => _id;

    public string GetUpdaterName() 
        => "Room Updater";

    public string GetAdditionalInformation()
        => "Room updater";

    public void RegisterForRoomChanges()
    {
        var filter = new ElementMulticategoryFilter(
        [
            BuiltInCategory.OST_RoomSeparationLines,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_GenericModel,
        ]);

        UpdaterRegistry.RegisterUpdater(this);

        // Geometry changes
        UpdaterRegistry.AddTrigger(_id, filter, Element.GetChangeTypeGeometry());
        
        // Added / deleted
        UpdaterRegistry.AddTrigger(_id, filter, Element.GetChangeTypeElementAddition());
        UpdaterRegistry.AddTrigger(_id, filter, Element.GetChangeTypeElementDeletion());
    }

}