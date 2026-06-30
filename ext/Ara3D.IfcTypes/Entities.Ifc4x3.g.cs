#pragma warning disable CS0108
namespace Ara3D.IfcTypes.Ifc4x3;

public partial class IfcActionRequest
   : IfcControl
{
    public static IfcActionRequest Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCACTIONREQUEST"u8;
    public const uint ENTITY_CODE = 1511108338;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActionRequestTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, PredefinedType, Status, LongDescription ];
}

public partial class IfcActor
   : IfcObject
{
    public static IfcActor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCACTOR"u8;
    public const uint ENTITY_CODE = 3349624876;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActorSelect> TheActor = new("TheActor", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TheActor ];
}

public partial class IfcActorRole
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcActorRole Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCACTORROLE"u8;
    public const uint ENTITY_CODE = 100396148;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRoleEnum> Role = new("Role", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedRole = new("UserDefinedRole", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Role, UserDefinedRole, Description ];
}

public partial class IfcActuator
   : IfcDistributionControlElement
{
    public static IfcActuator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCACTUATOR"u8;
    public const uint ENTITY_CODE = 796185452;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActuatorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcActuatorType
   : IfcDistributionControlElementType
{
    public static IfcActuatorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCACTUATORTYPE"u8;
    public const uint ENTITY_CODE = 1185848164;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActuatorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcAddress
   : EntityBaseClass, IfcObjectReferenceSelect
{
    public static IfcAddress Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCADDRESS"u8;
    public const uint ENTITY_CODE = 3858321853;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAddressTypeEnum> Purpose = new("Purpose", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedPurpose = new("UserDefinedPurpose", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Purpose, Description, UserDefinedPurpose ];
}

public partial class IfcAdvancedBrep
   : IfcManifoldSolidBrep
{
    public static IfcAdvancedBrep Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCADVANCEDBREP"u8;
    public const uint ENTITY_CODE = 4247103492;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Outer ];
}

public partial class IfcAdvancedBrepWithVoids
   : IfcAdvancedBrep
{
    public static IfcAdvancedBrepWithVoids Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCADVANCEDBREPWITHVOIDS"u8;
    public const uint ENTITY_CODE = 3439879655;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClosedShell> Voids = new("Voids", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Outer, Voids ];
}

public partial class IfcAdvancedFace
   : IfcFaceSurface
{
    public static IfcAdvancedFace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCADVANCEDFACE"u8;
    public const uint ENTITY_CODE = 120859368;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Bounds, FaceSurface, SameSense ];
}

public partial class IfcAirTerminal
   : IfcFlowTerminal
{
    public static IfcAirTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAIRTERMINAL"u8;
    public const uint ENTITY_CODE = 3293878581;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAirTerminalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcAirTerminalBox
   : IfcFlowController
{
    public static IfcAirTerminalBox Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAIRTERMINALBOX"u8;
    public const uint ENTITY_CODE = 1684970626;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAirTerminalBoxTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcAirTerminalBoxType
   : IfcFlowControllerType
{
    public static IfcAirTerminalBoxType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAIRTERMINALBOXTYPE"u8;
    public const uint ENTITY_CODE = 1176320402;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAirTerminalBoxTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcAirTerminalType
   : IfcFlowTerminalType
{
    public static IfcAirTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAIRTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 1876148061;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAirTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcAirToAirHeatRecovery
   : IfcEnergyConversionDevice
{
    public static IfcAirToAirHeatRecovery Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAIRTOAIRHEATRECOVERY"u8;
    public const uint ENTITY_CODE = 2081643409;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAirToAirHeatRecoveryTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcAirToAirHeatRecoveryType
   : IfcEnergyConversionDeviceType
{
    public static IfcAirToAirHeatRecoveryType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAIRTOAIRHEATRECOVERYTYPE"u8;
    public const uint ENTITY_CODE = 3377884601;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAirToAirHeatRecoveryTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcAlarm
   : IfcDistributionControlElement
{
    public static IfcAlarm Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALARM"u8;
    public const uint ENTITY_CODE = 848685364;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAlarmTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcAlarmType
   : IfcDistributionControlElementType
{
    public static IfcAlarmType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALARMTYPE"u8;
    public const uint ENTITY_CODE = 2639371548;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAlarmTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcAlignment
   : IfcLinearPositioningElement
{
    public static IfcAlignment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENT"u8;
    public const uint ENTITY_CODE = 3888330920;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAlignmentTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType ];
}

public partial class IfcAlignmentCant
   : IfcLinearElement
{
    public static IfcAlignmentCant Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTCANT"u8;
    public const uint ENTITY_CODE = 4153010922;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RailHeadDistance = new("RailHeadDistance", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, RailHeadDistance ];
}

public partial class IfcAlignmentCantSegment
   : IfcAlignmentParameterSegment
{
    public static IfcAlignmentCantSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTCANTSEGMENT"u8;
    public const uint ENTITY_CODE = 3264792747;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> StartDistAlong = new("StartDistAlong", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> HorizontalLength = new("HorizontalLength", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> StartCantLeft = new("StartCantLeft", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> EndCantLeft = new("EndCantLeft", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> StartCantRight = new("StartCantRight", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> EndCantRight = new("EndCantRight", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAlignmentCantSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ StartTag, EndTag, StartDistAlong, HorizontalLength, StartCantLeft, EndCantLeft, StartCantRight, EndCantRight, PredefinedType ];
}

public partial class IfcAlignmentHorizontal
   : IfcLinearElement
{
    public static IfcAlignmentHorizontal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTHORIZONTAL"u8;
    public const uint ENTITY_CODE = 437623192;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcAlignmentHorizontalSegment
   : IfcAlignmentParameterSegment
{
    public static IfcAlignmentHorizontalSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTHORIZONTALSEGMENT"u8;
    public const uint ENTITY_CODE = 3522045321;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> StartPoint = new("StartPoint", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> StartDirection = new("StartDirection", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> StartRadiusOfCurvature = new("StartRadiusOfCurvature", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> EndRadiusOfCurvature = new("EndRadiusOfCurvature", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> SegmentLength = new("SegmentLength", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> GravityCenterLineHeight = new("GravityCenterLineHeight", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAlignmentHorizontalSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ StartTag, EndTag, StartPoint, StartDirection, StartRadiusOfCurvature, EndRadiusOfCurvature, SegmentLength, GravityCenterLineHeight, PredefinedType ];
}

public partial class IfcAlignmentParameterSegment
   : EntityBaseClass
{
    public static IfcAlignmentParameterSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTPARAMETERSEGMENT"u8;
    public const uint ENTITY_CODE = 3953384776;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> StartTag = new("StartTag", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> EndTag = new("EndTag", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ StartTag, EndTag ];
}

public partial class IfcAlignmentSegment
   : IfcLinearElement
{
    public static IfcAlignmentSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTSEGMENT"u8;
    public const uint ENTITY_CODE = 2276039737;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAlignmentParameterSegment> DesignParameters = new("DesignParameters", 7, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, DesignParameters ];
}

public partial class IfcAlignmentVertical
   : IfcLinearElement
{
    public static IfcAlignmentVertical Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTVERTICAL"u8;
    public const uint ENTITY_CODE = 2967420274;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcAlignmentVerticalSegment
   : IfcAlignmentParameterSegment
{
    public static IfcAlignmentVerticalSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCALIGNMENTVERTICALSEGMENT"u8;
    public const uint ENTITY_CODE = 2021450595;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> StartDistAlong = new("StartDistAlong", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> HorizontalLength = new("HorizontalLength", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> StartHeight = new("StartHeight", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRatioMeasure> StartGradient = new("StartGradient", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRatioMeasure> EndGradient = new("EndGradient", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> RadiusOfCurvature = new("RadiusOfCurvature", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAlignmentVerticalSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ StartTag, EndTag, StartDistAlong, HorizontalLength, StartHeight, StartGradient, EndGradient, RadiusOfCurvature, PredefinedType ];
}

public partial class IfcAnnotation
   : IfcProduct
{
    public static IfcAnnotation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATION"u8;
    public const uint ENTITY_CODE = 3507439686;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAnnotationTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType ];
}

public partial class IfcAnnotationFillArea
   : IfcGeometricRepresentationItem
{
    public static IfcAnnotationFillArea Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONFILLAREA"u8;
    public const uint ENTITY_CODE = 508923030;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> OuterBoundary = new("OuterBoundary", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCurve> InnerBoundaries = new("InnerBoundaries", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ OuterBoundary, InnerBoundaries ];
}

public partial class IfcApplication
   : EntityBaseClass
{
    public static IfcApplication Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPLICATION"u8;
    public const uint ENTITY_CODE = 365708759;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOrganization> ApplicationDeveloper = new("ApplicationDeveloper", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> Version = new("Version", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ApplicationFullName = new("ApplicationFullName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> ApplicationIdentifier = new("ApplicationIdentifier", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ApplicationDeveloper, Version, ApplicationFullName, ApplicationIdentifier ];
}

public partial class IfcAppliedValue
   : EntityBaseClass, IfcMetricValueSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcAppliedValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPLIEDVALUE"u8;
    public const uint ENTITY_CODE = 777421865;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAppliedValueSelect> AppliedValue = new("AppliedValue", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcMeasureWithUnit> UnitBasis = new("UnitBasis", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDate> ApplicableDate = new("ApplicableDate", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDate> FixedUntilDate = new("FixedUntilDate", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Category = new("Category", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Condition = new("Condition", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcArithmeticOperatorEnum> ArithmeticOperator = new("ArithmeticOperator", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcAppliedValue> Components = new("Components", 9, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, AppliedValue, UnitBasis, ApplicableDate, FixedUntilDate, Category, Condition, ArithmeticOperator, Components ];
}

public partial class IfcApproval
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcApproval Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPROVAL"u8;
    public const uint ENTITY_CODE = 771577372;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identifier = new("Identifier", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> TimeOfApproval = new("TimeOfApproval", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Level = new("Level", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Qualifier = new("Qualifier", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorSelect> RequestingApproval = new("RequestingApproval", 7, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcActorSelect> GivingApproval = new("GivingApproval", 8, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Identifier, Name, Description, TimeOfApproval, Status, Level, Qualifier, RequestingApproval, GivingApproval ];
}

public partial class IfcApprovalRelationship
   : IfcResourceLevelRelationship
{
    public static IfcApprovalRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPROVALRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1503631090;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcApproval> RelatingApproval = new("RelatingApproval", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcApproval> RelatedApprovals = new("RelatedApprovals", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingApproval, RelatedApprovals ];
}

public partial class IfcArbitraryClosedProfileDef
   : IfcProfileDef
{
    public static IfcArbitraryClosedProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCARBITRARYCLOSEDPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3961970563;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> OuterCurve = new("OuterCurve", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, OuterCurve ];
}

public partial class IfcArbitraryOpenProfileDef
   : IfcProfileDef
{
    public static IfcArbitraryOpenProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCARBITRARYOPENPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3935482995;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoundedCurve> Curve = new("Curve", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Curve ];
}

public partial class IfcArbitraryProfileDefWithVoids
   : IfcArbitraryClosedProfileDef
{
    public static IfcArbitraryProfileDefWithVoids Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCARBITRARYPROFILEDEFWITHVOIDS"u8;
    public const uint ENTITY_CODE = 833005510;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> InnerCurves = new("InnerCurves", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, OuterCurve, InnerCurves ];
}

public partial class IfcAsset
   : IfcGroup
{
    public static IfcAsset Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCASSET"u8;
    public const uint ENTITY_CODE = 3348313689;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCostValue> OriginalValue = new("OriginalValue", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> CurrentValue = new("CurrentValue", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> TotalReplacementCost = new("TotalReplacementCost", 8, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcActorSelect> Owner = new("Owner", 9, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcActorSelect> User = new("User", 10, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPerson> ResponsiblePerson = new("ResponsiblePerson", 11, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDate> IncorporationDate = new("IncorporationDate", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCostValue> DepreciatedValue = new("DepreciatedValue", 13, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, OriginalValue, CurrentValue, TotalReplacementCost, Owner, User, ResponsiblePerson, IncorporationDate, DepreciatedValue ];
}

public partial class IfcAsymmetricIShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcAsymmetricIShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCASYMMETRICISHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3607974385;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BottomFlangeWidth = new("BottomFlangeWidth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallDepth = new("OverallDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BottomFlangeThickness = new("BottomFlangeThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> BottomFlangeFilletRadius = new("BottomFlangeFilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TopFlangeWidth = new("TopFlangeWidth", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TopFlangeThickness = new("TopFlangeThickness", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> TopFlangeFilletRadius = new("TopFlangeFilletRadius", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> BottomFlangeEdgeRadius = new("BottomFlangeEdgeRadius", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> BottomFlangeSlope = new("BottomFlangeSlope", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> TopFlangeEdgeRadius = new("TopFlangeEdgeRadius", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> TopFlangeSlope = new("TopFlangeSlope", 14, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, BottomFlangeWidth, OverallDepth, WebThickness, BottomFlangeThickness, BottomFlangeFilletRadius, TopFlangeWidth, TopFlangeThickness, TopFlangeFilletRadius, BottomFlangeEdgeRadius, BottomFlangeSlope, TopFlangeEdgeRadius, TopFlangeSlope ];
}

public partial class IfcAudioVisualAppliance
   : IfcFlowTerminal
{
    public static IfcAudioVisualAppliance Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAUDIOVISUALAPPLIANCE"u8;
    public const uint ENTITY_CODE = 3327421122;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAudioVisualApplianceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcAudioVisualApplianceType
   : IfcFlowTerminalType
{
    public static IfcAudioVisualApplianceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAUDIOVISUALAPPLIANCETYPE"u8;
    public const uint ENTITY_CODE = 2778215378;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAudioVisualApplianceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcAxis1Placement
   : IfcPlacement
{
    public static IfcAxis1Placement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAXIS1PLACEMENT"u8;
    public const uint ENTITY_CODE = 2912178692;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Axis = new("Axis", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, Axis ];
}

public partial class IfcAxis2Placement2D
   : IfcPlacement, IfcAxis2Placement
{
    public static IfcAxis2Placement2D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAXIS2PLACEMENT2D"u8;
    public const uint ENTITY_CODE = 143557545;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> RefDirection = new("RefDirection", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, RefDirection ];
}

public partial class IfcAxis2Placement3D
   : IfcPlacement, IfcAxis2Placement
{
    public static IfcAxis2Placement3D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAXIS2PLACEMENT3D"u8;
    public const uint ENTITY_CODE = 3800828224;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Axis = new("Axis", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDirection> RefDirection = new("RefDirection", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, Axis, RefDirection ];
}

public partial class IfcAxis2PlacementLinear
   : IfcPlacement
{
    public static IfcAxis2PlacementLinear Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAXIS2PLACEMENTLINEAR"u8;
    public const uint ENTITY_CODE = 2371267666;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Axis = new("Axis", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDirection> RefDirection = new("RefDirection", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, Axis, RefDirection ];
}

public partial class IfcBeam
   : IfcBuiltElement
{
    public static IfcBeam Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEAM"u8;
    public const uint ENTITY_CODE = 3562220184;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBeamTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcBeamType
   : IfcBuiltElementType
{
    public static IfcBeamType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEAMTYPE"u8;
    public const uint ENTITY_CODE = 2765867472;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBeamTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBearing
   : IfcBuiltElement
{
    public static IfcBearing Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEARING"u8;
    public const uint ENTITY_CODE = 871826411;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBearingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcBearingType
   : IfcBuiltElementType
{
    public static IfcBearingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEARINGTYPE"u8;
    public const uint ENTITY_CODE = 1637502515;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBearingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBlobTexture
   : IfcSurfaceTexture
{
    public static IfcBlobTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBLOBTEXTURE"u8;
    public const uint ENTITY_CODE = 3517409251;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> RasterFormat = new("RasterFormat", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBinary> RasterCode = new("RasterCode", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, Mode, TextureTransform, Parameter, RasterFormat, RasterCode ];
}

public partial class IfcBlock
   : IfcCsgPrimitive3D
{
    public static IfcBlock Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBLOCK"u8;
    public const uint ENTITY_CODE = 3091221680;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> XLength = new("XLength", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> YLength = new("YLength", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ZLength = new("ZLength", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, XLength, YLength, ZLength ];
}

public partial class IfcBoiler
   : IfcEnergyConversionDevice
{
    public static IfcBoiler Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOILER"u8;
    public const uint ENTITY_CODE = 3960141518;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoilerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcBoilerType
   : IfcEnergyConversionDeviceType
{
    public static IfcBoilerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOILERTYPE"u8;
    public const uint ENTITY_CODE = 3116962222;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoilerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBooleanClippingResult
   : IfcBooleanResult
{
    public static IfcBooleanClippingResult Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOOLEANCLIPPINGRESULT"u8;
    public const uint ENTITY_CODE = 2831743518;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Operator, FirstOperand, SecondOperand ];
}

public partial class IfcBooleanResult
   : IfcGeometricRepresentationItem, IfcBooleanOperand, IfcCsgSelect
{
    public static IfcBooleanResult Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOOLEANRESULT"u8;
    public const uint ENTITY_CODE = 1312774956;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBooleanOperator> Operator = new("Operator", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcBooleanOperand> FirstOperand = new("FirstOperand", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcBooleanOperand> SecondOperand = new("SecondOperand", 2, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Operator, FirstOperand, SecondOperand ];
}

public partial class IfcBorehole
   : IfcGeotechnicalAssembly
{
    public static IfcBorehole Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOREHOLE"u8;
    public const uint ENTITY_CODE = 1221035199;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcBoundaryCondition
   : EntityBaseClass
{
    public static IfcBoundaryCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYCONDITION"u8;
    public const uint ENTITY_CODE = 1350974706;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcBoundaryCurve
   : IfcCompositeCurveOnSurface
{
    public static IfcBoundaryCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYCURVE"u8;
    public const uint ENTITY_CODE = 2685029456;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect ];
}

public partial class IfcBoundaryEdgeCondition
   : IfcBoundaryCondition
{
    public static IfcBoundaryEdgeCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYEDGECONDITION"u8;
    public const uint ENTITY_CODE = 2472611581;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcModulusOfTranslationalSubgradeReactionSelect> TranslationalStiffnessByLengthX = new("TranslationalStiffnessByLengthX", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfTranslationalSubgradeReactionSelect> TranslationalStiffnessByLengthY = new("TranslationalStiffnessByLengthY", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfTranslationalSubgradeReactionSelect> TranslationalStiffnessByLengthZ = new("TranslationalStiffnessByLengthZ", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfRotationalSubgradeReactionSelect> RotationalStiffnessByLengthX = new("RotationalStiffnessByLengthX", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfRotationalSubgradeReactionSelect> RotationalStiffnessByLengthY = new("RotationalStiffnessByLengthY", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfRotationalSubgradeReactionSelect> RotationalStiffnessByLengthZ = new("RotationalStiffnessByLengthZ", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, TranslationalStiffnessByLengthX, TranslationalStiffnessByLengthY, TranslationalStiffnessByLengthZ, RotationalStiffnessByLengthX, RotationalStiffnessByLengthY, RotationalStiffnessByLengthZ ];
}

public partial class IfcBoundaryFaceCondition
   : IfcBoundaryCondition
{
    public static IfcBoundaryFaceCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYFACECONDITION"u8;
    public const uint ENTITY_CODE = 2562956589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcModulusOfSubgradeReactionSelect> TranslationalStiffnessByAreaX = new("TranslationalStiffnessByAreaX", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfSubgradeReactionSelect> TranslationalStiffnessByAreaY = new("TranslationalStiffnessByAreaY", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcModulusOfSubgradeReactionSelect> TranslationalStiffnessByAreaZ = new("TranslationalStiffnessByAreaZ", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, TranslationalStiffnessByAreaX, TranslationalStiffnessByAreaY, TranslationalStiffnessByAreaZ ];
}

public partial class IfcBoundaryNodeCondition
   : IfcBoundaryCondition
{
    public static IfcBoundaryNodeCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYNODECONDITION"u8;
    public const uint ENTITY_CODE = 2407292458;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTranslationalStiffnessSelect> TranslationalStiffnessX = new("TranslationalStiffnessX", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTranslationalStiffnessSelect> TranslationalStiffnessY = new("TranslationalStiffnessY", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTranslationalStiffnessSelect> TranslationalStiffnessZ = new("TranslationalStiffnessZ", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcRotationalStiffnessSelect> RotationalStiffnessX = new("RotationalStiffnessX", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcRotationalStiffnessSelect> RotationalStiffnessY = new("RotationalStiffnessY", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcRotationalStiffnessSelect> RotationalStiffnessZ = new("RotationalStiffnessZ", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, TranslationalStiffnessX, TranslationalStiffnessY, TranslationalStiffnessZ, RotationalStiffnessX, RotationalStiffnessY, RotationalStiffnessZ ];
}

public partial class IfcBoundaryNodeConditionWarping
   : IfcBoundaryNodeCondition
{
    public static IfcBoundaryNodeConditionWarping Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYNODECONDITIONWARPING"u8;
    public const uint ENTITY_CODE = 2919905048;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWarpingStiffnessSelect> WarpingStiffness = new("WarpingStiffness", 7, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, TranslationalStiffnessX, TranslationalStiffnessY, TranslationalStiffnessZ, RotationalStiffnessX, RotationalStiffnessY, RotationalStiffnessZ, WarpingStiffness ];
}

public partial class IfcBoundedCurve
   : IfcCurve, IfcCurveOrEdgeCurve
{
    public static IfcBoundedCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDEDCURVE"u8;
    public const uint ENTITY_CODE = 1147375295;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcBoundedSurface
   : IfcSurface
{
    public static IfcBoundedSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDEDSURFACE"u8;
    public const uint ENTITY_CODE = 68575855;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcBoundingBox
   : IfcGeometricRepresentationItem
{
    public static IfcBoundingBox Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDINGBOX"u8;
    public const uint ENTITY_CODE = 1442717844;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> Corner = new("Corner", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> XDim = new("XDim", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> YDim = new("YDim", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ZDim = new("ZDim", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Corner, XDim, YDim, ZDim ];
}

public partial class IfcBoxedHalfSpace
   : IfcHalfSpaceSolid
{
    public static IfcBoxedHalfSpace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOXEDHALFSPACE"u8;
    public const uint ENTITY_CODE = 3594319974;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoundingBox> Enclosure = new("Enclosure", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ BaseSurface, AgreementFlag, Enclosure ];
}

public partial class IfcBridge
   : IfcFacility
{
    public static IfcBridge Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBRIDGE"u8;
    public const uint ENTITY_CODE = 2427817166;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBridgeTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, PredefinedType ];
}

public partial class IfcBridgePart
   : IfcFacilityPart
{
    public static IfcBridgePart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBRIDGEPART"u8;
    public const uint ENTITY_CODE = 1311799399;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBridgePartTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, UsageType, PredefinedType ];
}

public partial class IfcBSplineCurve
   : IfcBoundedCurve
{
    public static IfcBSplineCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBSPLINECURVE"u8;
    public const uint ENTITY_CODE = 3214482937;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> Degree = new("Degree", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCartesianPoint> ControlPointsList = new("ControlPointsList", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcBSplineCurveForm> CurveForm = new("CurveForm", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLogical> ClosedCurve = new("ClosedCurve", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> SelfIntersect = new("SelfIntersect", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Degree, ControlPointsList, CurveForm, ClosedCurve, SelfIntersect ];
}

public partial class IfcBSplineCurveWithKnots
   : IfcBSplineCurve
{
    public static IfcBSplineCurveWithKnots Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBSPLINECURVEWITHKNOTS"u8;
    public const uint ENTITY_CODE = 3823230804;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> KnotMultiplicities = new("KnotMultiplicities", 5, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcParameterValue> Knots = new("Knots", 6, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcKnotType> KnotSpec = new("KnotSpec", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Degree, ControlPointsList, CurveForm, ClosedCurve, SelfIntersect, KnotMultiplicities, Knots, KnotSpec ];
}

public partial class IfcBSplineSurface
   : IfcBoundedSurface
{
    public static IfcBSplineSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBSPLINESURFACE"u8;
    public const uint ENTITY_CODE = 805726709;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> UDegree = new("UDegree", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> VDegree = new("VDegree", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCartesianPoint> ControlPointsList = new("ControlPointsList", 2, IfcTypeKind.Entity, 2);
    public readonly IfcAttribute<IfcBSplineSurfaceForm> SurfaceForm = new("SurfaceForm", 3, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLogical> UClosed = new("UClosed", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> VClosed = new("VClosed", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> SelfIntersect = new("SelfIntersect", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ UDegree, VDegree, ControlPointsList, SurfaceForm, UClosed, VClosed, SelfIntersect ];
}

public partial class IfcBSplineSurfaceWithKnots
   : IfcBSplineSurface
{
    public static IfcBSplineSurfaceWithKnots Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBSPLINESURFACEWITHKNOTS"u8;
    public const uint ENTITY_CODE = 1048362608;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> UMultiplicities = new("UMultiplicities", 7, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcInteger> VMultiplicities = new("VMultiplicities", 8, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcParameterValue> UKnots = new("UKnots", 9, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcParameterValue> VKnots = new("VKnots", 10, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcKnotType> KnotSpec = new("KnotSpec", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ UDegree, VDegree, ControlPointsList, SurfaceForm, UClosed, VClosed, SelfIntersect, UMultiplicities, VMultiplicities, UKnots, VKnots, KnotSpec ];
}

public partial class IfcBuilding
   : IfcFacility
{
    public static IfcBuilding Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDING"u8;
    public const uint ENTITY_CODE = 761684107;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> ElevationOfRefHeight = new("ElevationOfRefHeight", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ElevationOfTerrain = new("ElevationOfTerrain", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPostalAddress> BuildingAddress = new("BuildingAddress", 11, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, ElevationOfRefHeight, ElevationOfTerrain, BuildingAddress ];
}

public partial class IfcBuildingElementPart
   : IfcElementComponent
{
    public static IfcBuildingElementPart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPART"u8;
    public const uint ENTITY_CODE = 145338828;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuildingElementPartTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcBuildingElementPartType
   : IfcElementComponentType
{
    public static IfcBuildingElementPartType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPARTTYPE"u8;
    public const uint ENTITY_CODE = 2064734788;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuildingElementPartTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBuildingElementProxy
   : IfcBuiltElement
{
    public static IfcBuildingElementProxy Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPROXY"u8;
    public const uint ENTITY_CODE = 1258167731;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuildingElementProxyTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcBuildingElementProxyType
   : IfcBuiltElementType
{
    public static IfcBuildingElementProxyType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPROXYTYPE"u8;
    public const uint ENTITY_CODE = 365776395;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuildingElementProxyTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBuildingStorey
   : IfcSpatialStructureElement
{
    public static IfcBuildingStorey Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGSTOREY"u8;
    public const uint ENTITY_CODE = 2119311079;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> Elevation = new("Elevation", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, Elevation ];
}

public partial class IfcBuildingSystem
   : IfcSystem
{
    public static IfcBuildingSystem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGSYSTEM"u8;
    public const uint ENTITY_CODE = 400067826;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuildingSystemTypeEnum> PredefinedType = new("PredefinedType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, LongName ];
}

public partial class IfcBuiltElement
   : IfcElement
{
    public static IfcBuiltElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILTELEMENT"u8;
    public const uint ENTITY_CODE = 4132484215;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcBuiltElementType
   : IfcElementType
{
    public static IfcBuiltElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILTELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 3613613231;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcBuiltSystem
   : IfcSystem
{
    public static IfcBuiltSystem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILTSYSTEM"u8;
    public const uint ENTITY_CODE = 1344989052;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuiltSystemTypeEnum> PredefinedType = new("PredefinedType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, LongName ];
}

public partial class IfcBurner
   : IfcEnergyConversionDevice
{
    public static IfcBurner Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBURNER"u8;
    public const uint ENTITY_CODE = 2745453489;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBurnerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcBurnerType
   : IfcEnergyConversionDeviceType
{
    public static IfcBurnerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBURNERTYPE"u8;
    public const uint ENTITY_CODE = 1710763865;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBurnerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCableCarrierFitting
   : IfcFlowFitting
{
    public static IfcCableCarrierFitting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLECARRIERFITTING"u8;
    public const uint ENTITY_CODE = 189548391;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableCarrierFittingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCableCarrierFittingType
   : IfcFlowFittingType
{
    public static IfcCableCarrierFittingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLECARRIERFITTINGTYPE"u8;
    public const uint ENTITY_CODE = 376683519;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableCarrierFittingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCableCarrierSegment
   : IfcFlowSegment
{
    public static IfcCableCarrierSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLECARRIERSEGMENT"u8;
    public const uint ENTITY_CODE = 2919648569;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableCarrierSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCableCarrierSegmentType
   : IfcFlowSegmentType
{
    public static IfcCableCarrierSegmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLECARRIERSEGMENTTYPE"u8;
    public const uint ENTITY_CODE = 2588811057;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableCarrierSegmentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCableFitting
   : IfcFlowFitting
{
    public static IfcCableFitting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLEFITTING"u8;
    public const uint ENTITY_CODE = 2859774459;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableFittingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCableFittingType
   : IfcFlowFittingType
{
    public static IfcCableFittingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLEFITTINGTYPE"u8;
    public const uint ENTITY_CODE = 2199923043;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableFittingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCableSegment
   : IfcFlowSegment
{
    public static IfcCableSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLESEGMENT"u8;
    public const uint ENTITY_CODE = 2306544149;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCableSegmentType
   : IfcFlowSegmentType
{
    public static IfcCableSegmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCABLESEGMENTTYPE"u8;
    public const uint ENTITY_CODE = 1401189693;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCableSegmentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCaissonFoundation
   : IfcDeepFoundation
{
    public static IfcCaissonFoundation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCAISSONFOUNDATION"u8;
    public const uint ENTITY_CODE = 3992255512;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCaissonFoundationTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCaissonFoundationType
   : IfcDeepFoundationType
{
    public static IfcCaissonFoundationType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCAISSONFOUNDATIONTYPE"u8;
    public const uint ENTITY_CODE = 757674832;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCaissonFoundationTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCartesianPoint
   : IfcPoint, IfcTrimmingSelect
{
    public static IfcCartesianPoint Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANPOINT"u8;
    public const uint ENTITY_CODE = 2592642523;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> Coordinates = new("Coordinates", 0, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Coordinates ];
}

public partial class IfcCartesianPointList
   : IfcGeometricRepresentationItem
{
    public static IfcCartesianPointList Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANPOINTLIST"u8;
    public const uint ENTITY_CODE = 374385763;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcCartesianPointList2D
   : IfcCartesianPointList
{
    public static IfcCartesianPointList2D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANPOINTLIST2D"u8;
    public const uint ENTITY_CODE = 1003991621;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> CoordList = new("CoordList", 0, IfcTypeKind.Alias, 2);
    public readonly IfcAttribute<IfcLabel> TagList = new("TagList", 1, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ CoordList, TagList ];
}

public partial class IfcCartesianPointList3D
   : IfcCartesianPointList
{
    public static IfcCartesianPointList3D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANPOINTLIST3D"u8;
    public const uint ENTITY_CODE = 2513727068;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> CoordList = new("CoordList", 0, IfcTypeKind.Alias, 2);
    public readonly IfcAttribute<IfcLabel> TagList = new("TagList", 1, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ CoordList, TagList ];
}

public partial class IfcCartesianTransformationOperator
   : IfcGeometricRepresentationItem
{
    public static IfcCartesianTransformationOperator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANTRANSFORMATIONOPERATOR"u8;
    public const uint ENTITY_CODE = 4124277054;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Axis1 = new("Axis1", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDirection> Axis2 = new("Axis2", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCartesianPoint> LocalOrigin = new("LocalOrigin", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcReal> Scale = new("Scale", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Axis1, Axis2, LocalOrigin, Scale ];
}

public partial class IfcCartesianTransformationOperator2D
   : IfcCartesianTransformationOperator
{
    public static IfcCartesianTransformationOperator2D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANTRANSFORMATIONOPERATOR2D"u8;
    public const uint ENTITY_CODE = 293860064;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Axis1, Axis2, LocalOrigin, Scale ];
}

public partial class IfcCartesianTransformationOperator2DnonUniform
   : IfcCartesianTransformationOperator2D
{
    public static IfcCartesianTransformationOperator2DnonUniform Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANTRANSFORMATIONOPERATOR2DNONUNIFORM"u8;
    public const uint ENTITY_CODE = 1393209885;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> Scale2 = new("Scale2", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Axis1, Axis2, LocalOrigin, Scale, Scale2 ];
}

public partial class IfcCartesianTransformationOperator3D
   : IfcCartesianTransformationOperator
{
    public static IfcCartesianTransformationOperator3D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANTRANSFORMATIONOPERATOR3D"u8;
    public const uint ENTITY_CODE = 931556681;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Axis3 = new("Axis3", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Axis1, Axis2, LocalOrigin, Scale, Axis3 ];
}

public partial class IfcCartesianTransformationOperator3DnonUniform
   : IfcCartesianTransformationOperator3D
{
    public static IfcCartesianTransformationOperator3DnonUniform Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM"u8;
    public const uint ENTITY_CODE = 483449928;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> Scale2 = new("Scale2", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> Scale3 = new("Scale3", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Axis1, Axis2, LocalOrigin, Scale, Axis3, Scale2, Scale3 ];
}

public partial class IfcCenterLineProfileDef
   : IfcArbitraryOpenProfileDef
{
    public static IfcCenterLineProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCENTERLINEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2083666828;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Thickness = new("Thickness", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Curve, Thickness ];
}

public partial class IfcChiller
   : IfcEnergyConversionDevice
{
    public static IfcChiller Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCHILLER"u8;
    public const uint ENTITY_CODE = 1291268380;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcChillerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcChillerType
   : IfcEnergyConversionDeviceType
{
    public static IfcChillerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCHILLERTYPE"u8;
    public const uint ENTITY_CODE = 1828365044;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcChillerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcChimney
   : IfcBuiltElement
{
    public static IfcChimney Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCHIMNEY"u8;
    public const uint ENTITY_CODE = 2390011906;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcChimneyTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcChimneyType
   : IfcBuiltElementType
{
    public static IfcChimneyType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCHIMNEYTYPE"u8;
    public const uint ENTITY_CODE = 3914401042;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcChimneyTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCircle
   : IfcConic
{
    public static IfcCircle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCIRCLE"u8;
    public const uint ENTITY_CODE = 1749133735;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, Radius ];
}

public partial class IfcCircleHollowProfileDef
   : IfcCircleProfileDef
{
    public static IfcCircleHollowProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCIRCLEHOLLOWPROFILEDEF"u8;
    public const uint ENTITY_CODE = 1758279288;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WallThickness = new("WallThickness", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Radius, WallThickness ];
}

public partial class IfcCircleProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcCircleProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCIRCLEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3866071551;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Radius ];
}

public partial class IfcCivilElement
   : IfcElement
{
    public static IfcCivilElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCIVILELEMENT"u8;
    public const uint ENTITY_CODE = 913246126;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcCivilElementType
   : IfcElementType
{
    public static IfcCivilElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCIVILELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 3109112334;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcClassification
   : IfcExternalInformation, IfcClassificationReferenceSelect, IfcClassificationSelect
{
    public static IfcClassification Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATION"u8;
    public const uint ENTITY_CODE = 1675978639;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Source = new("Source", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Edition = new("Edition", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDate> EditionDate = new("EditionDate", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcURIReference> Specification = new("Specification", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> ReferenceTokens = new("ReferenceTokens", 6, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Source, Edition, EditionDate, Name, Description, Specification, ReferenceTokens ];
}

public partial class IfcClassificationReference
   : IfcExternalReference, IfcClassificationReferenceSelect, IfcClassificationSelect
{
    public static IfcClassificationReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATIONREFERENCE"u8;
    public const uint ENTITY_CODE = 1249450268;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationReferenceSelect> ReferencedSource = new("ReferencedSource", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> Sort = new("Sort", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Location, Identification, Name, ReferencedSource, Description, Sort ];
}

public partial class IfcClosedShell
   : IfcConnectedFaceSet, IfcShell, IfcSolidOrShell
{
    public static IfcClosedShell Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLOSEDSHELL"u8;
    public const uint ENTITY_CODE = 2374515303;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ CfsFaces ];
}

public partial class IfcClothoid
   : IfcSpiral
{
    public static IfcClothoid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLOTHOID"u8;
    public const uint ENTITY_CODE = 969078135;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> ClothoidConstant = new("ClothoidConstant", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, ClothoidConstant ];
}

public partial class IfcCoil
   : IfcEnergyConversionDevice
{
    public static IfcCoil Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOIL"u8;
    public const uint ENTITY_CODE = 1409689212;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoilTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCoilType
   : IfcEnergyConversionDeviceType
{
    public static IfcCoilType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOILTYPE"u8;
    public const uint ENTITY_CODE = 679451348;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoilTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcColourRgb
   : IfcColourSpecification, IfcColourOrFactor
{
    public static IfcColourRgb Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLOURRGB"u8;
    public const uint ENTITY_CODE = 3581224902;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Red = new("Red", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Green = new("Green", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Blue = new("Blue", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Red, Green, Blue ];
}

public partial class IfcColourRgbList
   : IfcPresentationItem
{
    public static IfcColourRgbList Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLOURRGBLIST"u8;
    public const uint ENTITY_CODE = 3889447890;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> ColourList = new("ColourList", 0, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ ColourList ];
}

public partial class IfcColourSpecification
   : IfcPresentationItem, IfcColour
{
    public static IfcColourSpecification Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLOURSPECIFICATION"u8;
    public const uint ENTITY_CODE = 984402472;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcColumn
   : IfcBuiltElement
{
    public static IfcColumn Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLUMN"u8;
    public const uint ENTITY_CODE = 4230436045;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColumnTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcColumnType
   : IfcBuiltElementType
{
    public static IfcColumnType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLUMNTYPE"u8;
    public const uint ENTITY_CODE = 2387334149;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColumnTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCommunicationsAppliance
   : IfcFlowTerminal
{
    public static IfcCommunicationsAppliance Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMMUNICATIONSAPPLIANCE"u8;
    public const uint ENTITY_CODE = 3266413985;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCommunicationsApplianceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCommunicationsApplianceType
   : IfcFlowTerminalType
{
    public static IfcCommunicationsApplianceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMMUNICATIONSAPPLIANCETYPE"u8;
    public const uint ENTITY_CODE = 1113689321;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCommunicationsApplianceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcComplexProperty
   : IfcProperty
{
    public static IfcComplexProperty Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPLEXPROPERTY"u8;
    public const uint ENTITY_CODE = 2192924248;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> UsageName = new("UsageName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProperty> HasProperties = new("HasProperties", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Specification, UsageName, HasProperties ];
}

public partial class IfcComplexPropertyTemplate
   : IfcPropertyTemplate
{
    public static IfcComplexPropertyTemplate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPLEXPROPERTYTEMPLATE"u8;
    public const uint ENTITY_CODE = 1789814754;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> UsageName = new("UsageName", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcComplexPropertyTemplateTypeEnum> TemplateType = new("TemplateType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPropertyTemplate> HasPropertyTemplates = new("HasPropertyTemplates", 6, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, UsageName, TemplateType, HasPropertyTemplates ];
}

public partial class IfcCompositeCurve
   : IfcBoundedCurve
{
    public static IfcCompositeCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPOSITECURVE"u8;
    public const uint ENTITY_CODE = 3290217845;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSegment> Segments = new("Segments", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLogical> SelfIntersect = new("SelfIntersect", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect ];
}

public partial class IfcCompositeCurveOnSurface
   : IfcCompositeCurve, IfcCurveOnSurface
{
    public static IfcCompositeCurveOnSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPOSITECURVEONSURFACE"u8;
    public const uint ENTITY_CODE = 4133881515;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect ];
}

public partial class IfcCompositeCurveSegment
   : IfcSegment
{
    public static IfcCompositeCurveSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPOSITECURVESEGMENT"u8;
    public const uint ENTITY_CODE = 690703830;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> SameSense = new("SameSense", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCurve> ParentCurve = new("ParentCurve", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Transition, SameSense, ParentCurve ];
}

public partial class IfcCompositeProfileDef
   : IfcProfileDef
{
    public static IfcCompositeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPOSITEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 1348311886;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> Profiles = new("Profiles", 2, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Label = new("Label", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Profiles, Label ];
}

public partial class IfcCompressor
   : IfcFlowMovingDevice
{
    public static IfcCompressor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPRESSOR"u8;
    public const uint ENTITY_CODE = 2902755306;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCompressorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCompressorType
   : IfcFlowMovingDeviceType
{
    public static IfcCompressorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPRESSORTYPE"u8;
    public const uint ENTITY_CODE = 3297355082;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCompressorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCondenser
   : IfcEnergyConversionDevice
{
    public static IfcCondenser Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONDENSER"u8;
    public const uint ENTITY_CODE = 2517229806;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCondenserTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCondenserType
   : IfcEnergyConversionDeviceType
{
    public static IfcCondenserType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONDENSERTYPE"u8;
    public const uint ENTITY_CODE = 2094249038;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCondenserTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcConic
   : IfcCurve
{
    public static IfcConic Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONIC"u8;
    public const uint ENTITY_CODE = 2129705005;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement> Position = new("Position", 0, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Position ];
}

public partial class IfcConnectedFaceSet
   : IfcTopologicalRepresentationItem
{
    public static IfcConnectedFaceSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTEDFACESET"u8;
    public const uint ENTITY_CODE = 2025929673;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFace> CfsFaces = new("CfsFaces", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ CfsFaces ];
}

public partial class IfcConnectionCurveGeometry
   : IfcConnectionGeometry
{
    public static IfcConnectionCurveGeometry Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONCURVEGEOMETRY"u8;
    public const uint ENTITY_CODE = 4068633818;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurveOrEdgeCurve> CurveOnRelatingElement = new("CurveOnRelatingElement", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCurveOrEdgeCurve> CurveOnRelatedElement = new("CurveOnRelatedElement", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ CurveOnRelatingElement, CurveOnRelatedElement ];
}

public partial class IfcConnectionGeometry
   : EntityBaseClass
{
    public static IfcConnectionGeometry Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONGEOMETRY"u8;
    public const uint ENTITY_CODE = 572172191;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcConnectionPointEccentricity
   : IfcConnectionPointGeometry
{
    public static IfcConnectionPointEccentricity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONPOINTECCENTRICITY"u8;
    public const uint ENTITY_CODE = 2135620543;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> EccentricityInX = new("EccentricityInX", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> EccentricityInY = new("EccentricityInY", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> EccentricityInZ = new("EccentricityInZ", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ PointOnRelatingElement, PointOnRelatedElement, EccentricityInX, EccentricityInY, EccentricityInZ ];
}

public partial class IfcConnectionPointGeometry
   : IfcConnectionGeometry
{
    public static IfcConnectionPointGeometry Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONPOINTGEOMETRY"u8;
    public const uint ENTITY_CODE = 247146535;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPointOrVertexPoint> PointOnRelatingElement = new("PointOnRelatingElement", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPointOrVertexPoint> PointOnRelatedElement = new("PointOnRelatedElement", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ PointOnRelatingElement, PointOnRelatedElement ];
}

public partial class IfcConnectionSurfaceGeometry
   : IfcConnectionGeometry
{
    public static IfcConnectionSurfaceGeometry Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONSURFACEGEOMETRY"u8;
    public const uint ENTITY_CODE = 3292868022;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurfaceOrFaceSurface> SurfaceOnRelatingElement = new("SurfaceOnRelatingElement", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcSurfaceOrFaceSurface> SurfaceOnRelatedElement = new("SurfaceOnRelatedElement", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ SurfaceOnRelatingElement, SurfaceOnRelatedElement ];
}

public partial class IfcConnectionVolumeGeometry
   : IfcConnectionGeometry
{
    public static IfcConnectionVolumeGeometry Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONVOLUMEGEOMETRY"u8;
    public const uint ENTITY_CODE = 3055370909;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSolidOrShell> VolumeOnRelatingElement = new("VolumeOnRelatingElement", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcSolidOrShell> VolumeOnRelatedElement = new("VolumeOnRelatedElement", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ VolumeOnRelatingElement, VolumeOnRelatedElement ];
}

public partial class IfcConstraint
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcConstraint Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRAINT"u8;
    public const uint ENTITY_CODE = 3774606772;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcConstraintEnum> ConstraintGrade = new("ConstraintGrade", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> ConstraintSource = new("ConstraintSource", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorSelect> CreatingActor = new("CreatingActor", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTime> CreationTime = new("CreationTime", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedGrade = new("UserDefinedGrade", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, ConstraintGrade, ConstraintSource, CreatingActor, CreationTime, UserDefinedGrade ];
}

public partial class IfcConstructionEquipmentResource
   : IfcConstructionResource
{
    public static IfcConstructionEquipmentResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONEQUIPMENTRESOURCE"u8;
    public const uint ENTITY_CODE = 325370190;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstructionEquipmentResourceTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcConstructionEquipmentResourceType
   : IfcConstructionResourceType
{
    public static IfcConstructionEquipmentResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONEQUIPMENTRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 1033176110;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstructionEquipmentResourceTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcConstructionMaterialResource
   : IfcConstructionResource
{
    public static IfcConstructionMaterialResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONMATERIALRESOURCE"u8;
    public const uint ENTITY_CODE = 3540649679;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstructionMaterialResourceTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcConstructionMaterialResourceType
   : IfcConstructionResourceType
{
    public static IfcConstructionMaterialResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONMATERIALRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 2484875575;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstructionMaterialResourceTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcConstructionProductResource
   : IfcConstructionResource
{
    public static IfcConstructionProductResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONPRODUCTRESOURCE"u8;
    public const uint ENTITY_CODE = 1684371685;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstructionProductResourceTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcConstructionProductResourceType
   : IfcConstructionResourceType
{
    public static IfcConstructionProductResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONPRODUCTRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 3087623469;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstructionProductResourceTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcConstructionResource
   : IfcResource
{
    public static IfcConstructionResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONRESOURCE"u8;
    public const uint ENTITY_CODE = 1336170662;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcResourceTime> Usage = new("Usage", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAppliedValue> BaseCosts = new("BaseCosts", 8, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPhysicalQuantity> BaseQuantity = new("BaseQuantity", 9, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity ];
}

public partial class IfcConstructionResourceType
   : IfcTypeResource
{
    public static IfcConstructionResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 228789974;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAppliedValue> BaseCosts = new("BaseCosts", 9, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPhysicalQuantity> BaseQuantity = new("BaseQuantity", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity ];
}

public partial class IfcContext
   : IfcObjectDefinition
{
    public static IfcContext Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONTEXT"u8;
    public const uint ENTITY_CODE = 1157617030;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ObjectType = new("ObjectType", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Phase = new("Phase", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRepresentationContext> RepresentationContexts = new("RepresentationContexts", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcUnitAssignment> UnitsInContext = new("UnitsInContext", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName, Phase, RepresentationContexts, UnitsInContext ];
}

public partial class IfcContextDependentUnit
   : IfcNamedUnit, IfcResourceObjectSelect
{
    public static IfcContextDependentUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONTEXTDEPENDENTUNIT"u8;
    public const uint ENTITY_CODE = 3300513551;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Dimensions, UnitType, Name ];
}

public partial class IfcControl
   : IfcObject
{
    public static IfcControl Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONTROL"u8;
    public const uint ENTITY_CODE = 3313972656;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification ];
}

public partial class IfcController
   : IfcDistributionControlElement
{
    public static IfcController Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONTROLLER"u8;
    public const uint ENTITY_CODE = 3462062371;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcControllerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcControllerType
   : IfcDistributionControlElementType
{
    public static IfcControllerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONTROLLERTYPE"u8;
    public const uint ENTITY_CODE = 2931344475;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcControllerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcConversionBasedUnit
   : IfcNamedUnit, IfcResourceObjectSelect
{
    public static IfcConversionBasedUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONVERSIONBASEDUNIT"u8;
    public const uint ENTITY_CODE = 1289124;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMeasureWithUnit> ConversionFactor = new("ConversionFactor", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Dimensions, UnitType, Name, ConversionFactor ];
}

public partial class IfcConversionBasedUnitWithOffset
   : IfcConversionBasedUnit
{
    public static IfcConversionBasedUnitWithOffset Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONVERSIONBASEDUNITWITHOFFSET"u8;
    public const uint ENTITY_CODE = 1872223463;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> ConversionOffset = new("ConversionOffset", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Dimensions, UnitType, Name, ConversionFactor, ConversionOffset ];
}

public partial class IfcConveyorSegment
   : IfcFlowSegment
{
    public static IfcConveyorSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONVEYORSEGMENT"u8;
    public const uint ENTITY_CODE = 1215236927;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConveyorSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcConveyorSegmentType
   : IfcFlowSegmentType
{
    public static IfcConveyorSegmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONVEYORSEGMENTTYPE"u8;
    public const uint ENTITY_CODE = 2114148615;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConveyorSegmentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCooledBeam
   : IfcEnergyConversionDevice
{
    public static IfcCooledBeam Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOOLEDBEAM"u8;
    public const uint ENTITY_CODE = 3881119228;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCooledBeamTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCooledBeamType
   : IfcEnergyConversionDeviceType
{
    public static IfcCooledBeamType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOOLEDBEAMTYPE"u8;
    public const uint ENTITY_CODE = 2912393812;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCooledBeamTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCoolingTower
   : IfcEnergyConversionDevice
{
    public static IfcCoolingTower Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOOLINGTOWER"u8;
    public const uint ENTITY_CODE = 2051141211;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoolingTowerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCoolingTowerType
   : IfcEnergyConversionDeviceType
{
    public static IfcCoolingTowerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOOLINGTOWERTYPE"u8;
    public const uint ENTITY_CODE = 628467651;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoolingTowerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCoordinateOperation
   : EntityBaseClass
{
    public static IfcCoordinateOperation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOORDINATEOPERATION"u8;
    public const uint ENTITY_CODE = 3351271378;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoordinateReferenceSystemSelect> SourceCRS = new("SourceCRS", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCoordinateReferenceSystem> TargetCRS = new("TargetCRS", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SourceCRS, TargetCRS ];
}

public partial class IfcCoordinateReferenceSystem
   : EntityBaseClass, IfcCoordinateReferenceSystemSelect
{
    public static IfcCoordinateReferenceSystem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOORDINATEREFERENCESYSTEM"u8;
    public const uint ENTITY_CODE = 3832139183;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> GeodeticDatum = new("GeodeticDatum", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> VerticalDatum = new("VerticalDatum", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, GeodeticDatum, VerticalDatum ];
}

public partial class IfcCosineSpiral
   : IfcSpiral
{
    public static IfcCosineSpiral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSINESPIRAL"u8;
    public const uint ENTITY_CODE = 226879693;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> CosineTerm = new("CosineTerm", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ConstantTerm = new("ConstantTerm", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, CosineTerm, ConstantTerm ];
}

public partial class IfcCostItem
   : IfcControl
{
    public static IfcCostItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSTITEM"u8;
    public const uint ENTITY_CODE = 204301829;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCostItemTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcCostValue> CostValues = new("CostValues", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPhysicalQuantity> CostQuantities = new("CostQuantities", 8, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, PredefinedType, CostValues, CostQuantities ];
}

public partial class IfcCostSchedule
   : IfcControl
{
    public static IfcCostSchedule Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSTSCHEDULE"u8;
    public const uint ENTITY_CODE = 1266701043;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCostScheduleTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> SubmittedOn = new("SubmittedOn", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> UpdateDate = new("UpdateDate", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, PredefinedType, Status, SubmittedOn, UpdateDate ];
}

public partial class IfcCostValue
   : IfcAppliedValue
{
    public static IfcCostValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSTVALUE"u8;
    public const uint ENTITY_CODE = 4023367015;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, Description, AppliedValue, UnitBasis, ApplicableDate, FixedUntilDate, Category, Condition, ArithmeticOperator, Components ];
}

public partial class IfcCourse
   : IfcBuiltElement
{
    public static IfcCourse Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOURSE"u8;
    public const uint ENTITY_CODE = 1476518042;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCourseTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCourseType
   : IfcBuiltElementType
{
    public static IfcCourseType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOURSETYPE"u8;
    public const uint ENTITY_CODE = 4040396890;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCourseTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCovering
   : IfcBuiltElement
{
    public static IfcCovering Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOVERING"u8;
    public const uint ENTITY_CODE = 3840892682;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoveringTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCoveringType
   : IfcBuiltElementType
{
    public static IfcCoveringType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOVERINGTYPE"u8;
    public const uint ENTITY_CODE = 1670716522;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoveringTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCrewResource
   : IfcConstructionResource
{
    public static IfcCrewResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCREWRESOURCE"u8;
    public const uint ENTITY_CODE = 3676323422;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCrewResourceTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcCrewResourceType
   : IfcConstructionResourceType
{
    public static IfcCrewResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCREWRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 1960438110;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCrewResourceTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcCsgPrimitive3D
   : IfcGeometricRepresentationItem, IfcBooleanOperand, IfcCsgSelect
{
    public static IfcCsgPrimitive3D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCSGPRIMITIVE3D"u8;
    public const uint ENTITY_CODE = 1339626996;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement3D> Position = new("Position", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Position ];
}

public partial class IfcCsgSolid
   : IfcSolidModel
{
    public static IfcCsgSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCSGSOLID"u8;
    public const uint ENTITY_CODE = 3465009481;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCsgSelect> TreeRootExpression = new("TreeRootExpression", 0, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ TreeRootExpression ];
}

public partial class IfcCShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcCShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCSHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 1922922321;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Width = new("Width", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WallThickness = new("WallThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Girth = new("Girth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> InternalFilletRadius = new("InternalFilletRadius", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, Width, WallThickness, Girth, InternalFilletRadius ];
}

public partial class IfcCurrencyRelationship
   : IfcResourceLevelRelationship
{
    public static IfcCurrencyRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURRENCYRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3359804106;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMonetaryUnit> RelatingMonetaryUnit = new("RelatingMonetaryUnit", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcMonetaryUnit> RelatedMonetaryUnit = new("RelatedMonetaryUnit", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ExchangeRate = new("ExchangeRate", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> RateDateTime = new("RateDateTime", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLibraryInformation> RateSource = new("RateSource", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingMonetaryUnit, RelatedMonetaryUnit, ExchangeRate, RateDateTime, RateSource ];
}

public partial class IfcCurtainWall
   : IfcBuiltElement
{
    public static IfcCurtainWall Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURTAINWALL"u8;
    public const uint ENTITY_CODE = 2095691047;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurtainWallTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcCurtainWallType
   : IfcBuiltElementType
{
    public static IfcCurtainWallType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURTAINWALLTYPE"u8;
    public const uint ENTITY_CODE = 1160082879;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurtainWallTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCurve
   : IfcGeometricRepresentationItem, IfcGeometricSetSelect
{
    public static IfcCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVE"u8;
    public const uint ENTITY_CODE = 3079632494;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcCurveBoundedPlane
   : IfcBoundedSurface
{
    public static IfcCurveBoundedPlane Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVEBOUNDEDPLANE"u8;
    public const uint ENTITY_CODE = 676770975;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlane> BasisSurface = new("BasisSurface", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCurve> OuterBoundary = new("OuterBoundary", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCurve> InnerBoundaries = new("InnerBoundaries", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ BasisSurface, OuterBoundary, InnerBoundaries ];
}

public partial class IfcCurveBoundedSurface
   : IfcBoundedSurface
{
    public static IfcCurveBoundedSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVEBOUNDEDSURFACE"u8;
    public const uint ENTITY_CODE = 756924914;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> BasisSurface = new("BasisSurface", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoundaryCurve> Boundaries = new("Boundaries", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcBoolean> ImplicitOuter = new("ImplicitOuter", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisSurface, Boundaries, ImplicitOuter ];
}

public partial class IfcCurveSegment
   : IfcSegment
{
    public static IfcCurveSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESEGMENT"u8;
    public const uint ENTITY_CODE = 1588348887;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlacement> Placement = new("Placement", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCurveMeasureSelect> SegmentStart = new("SegmentStart", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCurveMeasureSelect> SegmentLength = new("SegmentLength", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCurve> ParentCurve = new("ParentCurve", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Transition, Placement, SegmentStart, SegmentLength, ParentCurve ];
}

public partial class IfcCurveStyle
   : IfcPresentationStyle
{
    public static IfcCurveStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESTYLE"u8;
    public const uint ENTITY_CODE = 796586243;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurveFontOrScaledCurveFontSelect> CurveFont = new("CurveFont", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcSizeSelect> CurveWidth = new("CurveWidth", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColour> CurveColour = new("CurveColour", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcBoolean> ModelOrDraughting = new("ModelOrDraughting", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, CurveFont, CurveWidth, CurveColour, ModelOrDraughting ];
}

public partial class IfcCurveStyleFont
   : IfcPresentationItem, IfcCurveStyleFontSelect
{
    public static IfcCurveStyleFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESTYLEFONT"u8;
    public const uint ENTITY_CODE = 1108523850;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCurveStyleFontPattern> PatternList = new("PatternList", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, PatternList ];
}

public partial class IfcCurveStyleFontAndScaling
   : IfcPresentationItem, IfcCurveFontOrScaledCurveFontSelect
{
    public static IfcCurveStyleFontAndScaling Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESTYLEFONTANDSCALING"u8;
    public const uint ENTITY_CODE = 320924324;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCurveStyleFontSelect> CurveStyleFont = new("CurveStyleFont", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> CurveFontScaling = new("CurveFontScaling", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, CurveStyleFont, CurveFontScaling ];
}

public partial class IfcCurveStyleFontPattern
   : IfcPresentationItem
{
    public static IfcCurveStyleFontPattern Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESTYLEFONTPATTERN"u8;
    public const uint ENTITY_CODE = 236994256;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> VisibleSegmentLength = new("VisibleSegmentLength", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> InvisibleSegmentLength = new("InvisibleSegmentLength", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ VisibleSegmentLength, InvisibleSegmentLength ];
}

public partial class IfcCylindricalSurface
   : IfcElementarySurface
{
    public static IfcCylindricalSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCYLINDRICALSURFACE"u8;
    public const uint ENTITY_CODE = 3246384204;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, Radius ];
}

public partial class IfcDamper
   : IfcFlowController
{
    public static IfcDamper Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDAMPER"u8;
    public const uint ENTITY_CODE = 1584011894;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDamperTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDamperType
   : IfcFlowControllerType
{
    public static IfcDamperType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDAMPERTYPE"u8;
    public const uint ENTITY_CODE = 4182524806;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDamperTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcDeepFoundation
   : IfcBuiltElement
{
    public static IfcDeepFoundation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDEEPFOUNDATION"u8;
    public const uint ENTITY_CODE = 3223812192;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcDeepFoundationType
   : IfcBuiltElementType
{
    public static IfcDeepFoundationType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDEEPFOUNDATIONTYPE"u8;
    public const uint ENTITY_CODE = 1542911240;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcDerivedProfileDef
   : IfcProfileDef
{
    public static IfcDerivedProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDERIVEDPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2084073208;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> ParentProfile = new("ParentProfile", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCartesianTransformationOperator2D> Operator = new("Operator", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> Label = new("Label", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, ParentProfile, Operator, Label ];
}

public partial class IfcDerivedUnit
   : EntityBaseClass, IfcUnit
{
    public static IfcDerivedUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDERIVEDUNIT"u8;
    public const uint ENTITY_CODE = 2275012698;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDerivedUnitElement> Elements = new("Elements", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcDerivedUnitEnum> UnitType = new("UnitType", 1, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedType = new("UserDefinedType", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Elements, UnitType, UserDefinedType, Name ];
}

public partial class IfcDerivedUnitElement
   : EntityBaseClass
{
    public static IfcDerivedUnitElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDERIVEDUNITELEMENT"u8;
    public const uint ENTITY_CODE = 1549914162;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNamedUnit> Unit = new("Unit", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<INTEGER> Exponent = new("Exponent", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Unit, Exponent ];
}

public partial class IfcDimensionalExponents
   : EntityBaseClass
{
    public static IfcDimensionalExponents Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIMENSIONALEXPONENTS"u8;
    public const uint ENTITY_CODE = 1671467792;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<INTEGER> LengthExponent = new("LengthExponent", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> MassExponent = new("MassExponent", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> TimeExponent = new("TimeExponent", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> ElectricCurrentExponent = new("ElectricCurrentExponent", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> ThermodynamicTemperatureExponent = new("ThermodynamicTemperatureExponent", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> AmountOfSubstanceExponent = new("AmountOfSubstanceExponent", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> LuminousIntensityExponent = new("LuminousIntensityExponent", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ LengthExponent, MassExponent, TimeExponent, ElectricCurrentExponent, ThermodynamicTemperatureExponent, AmountOfSubstanceExponent, LuminousIntensityExponent ];
}

public partial class IfcDirection
   : IfcGeometricRepresentationItem, IfcGridPlacementDirectionSelect, IfcVectorOrDirection
{
    public static IfcDirection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIRECTION"u8;
    public const uint ENTITY_CODE = 1116762488;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> DirectionRatios = new("DirectionRatios", 0, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ DirectionRatios ];
}

public partial class IfcDirectrixCurveSweptAreaSolid
   : IfcSweptAreaSolid
{
    public static IfcDirectrixCurveSweptAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIRECTRIXCURVESWEPTAREASOLID"u8;
    public const uint ENTITY_CODE = 145165971;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> Directrix = new("Directrix", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCurveMeasureSelect> StartParam = new("StartParam", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCurveMeasureSelect> EndParam = new("EndParam", 4, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Directrix, StartParam, EndParam ];
}

public partial class IfcDirectrixDerivedReferenceSweptAreaSolid
   : IfcFixedReferenceSweptAreaSolid
{
    public static IfcDirectrixDerivedReferenceSweptAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIRECTRIXDERIVEDREFERENCESWEPTAREASOLID"u8;
    public const uint ENTITY_CODE = 163467308;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Directrix, StartParam, EndParam, FixedReference ];
}

public partial class IfcDiscreteAccessory
   : IfcElementComponent
{
    public static IfcDiscreteAccessory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISCRETEACCESSORY"u8;
    public const uint ENTITY_CODE = 1020050154;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDiscreteAccessoryTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDiscreteAccessoryType
   : IfcElementComponentType
{
    public static IfcDiscreteAccessoryType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISCRETEACCESSORYTYPE"u8;
    public const uint ENTITY_CODE = 1499596874;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDiscreteAccessoryTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcDistributionBoard
   : IfcFlowController
{
    public static IfcDistributionBoard Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONBOARD"u8;
    public const uint ENTITY_CODE = 1943937367;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDistributionBoardTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDistributionBoardType
   : IfcFlowControllerType
{
    public static IfcDistributionBoardType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONBOARDTYPE"u8;
    public const uint ENTITY_CODE = 860824207;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDistributionBoardTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcDistributionChamberElement
   : IfcDistributionFlowElement
{
    public static IfcDistributionChamberElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCHAMBERELEMENT"u8;
    public const uint ENTITY_CODE = 1690940191;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDistributionChamberElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDistributionChamberElementType
   : IfcDistributionFlowElementType
{
    public static IfcDistributionChamberElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCHAMBERELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 2100497895;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDistributionChamberElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcDistributionCircuit
   : IfcDistributionSystem
{
    public static IfcDistributionCircuit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCIRCUIT"u8;
    public const uint ENTITY_CODE = 1548360464;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName, PredefinedType ];
}

public partial class IfcDistributionControlElement
   : IfcDistributionElement
{
    public static IfcDistributionControlElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCONTROLELEMENT"u8;
    public const uint ENTITY_CODE = 1571819994;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcDistributionControlElementType
   : IfcDistributionElementType
{
    public static IfcDistributionControlElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCONTROLELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 2230984090;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcDistributionElement
   : IfcElement
{
    public static IfcDistributionElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONELEMENT"u8;
    public const uint ENTITY_CODE = 3253451051;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcDistributionElementType
   : IfcElementType
{
    public static IfcDistributionElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1341979763;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcDistributionFlowElement
   : IfcDistributionElement
{
    public static IfcDistributionFlowElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONFLOWELEMENT"u8;
    public const uint ENTITY_CODE = 2529962475;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcDistributionFlowElementType
   : IfcDistributionElementType
{
    public static IfcDistributionFlowElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONFLOWELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 3994801203;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcDistributionPort
   : IfcPort
{
    public static IfcDistributionPort Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONPORT"u8;
    public const uint ENTITY_CODE = 996223226;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFlowDirectionEnum> FlowDirection = new("FlowDirection", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDistributionPortTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDistributionSystemEnum> SystemType = new("SystemType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, FlowDirection, PredefinedType, SystemType ];
}

public partial class IfcDistributionSystem
   : IfcSystem
{
    public static IfcDistributionSystem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONSYSTEM"u8;
    public const uint ENTITY_CODE = 2966760840;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDistributionSystemEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName, PredefinedType ];
}

public partial class IfcDocumentInformation
   : IfcExternalInformation, IfcDocumentSelect
{
    public static IfcDocumentInformation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTINFORMATION"u8;
    public const uint ENTITY_CODE = 1365583644;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcURIReference> Location = new("Location", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Purpose = new("Purpose", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> IntendedUse = new("IntendedUse", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Scope = new("Scope", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Revision = new("Revision", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorSelect> DocumentOwner = new("DocumentOwner", 8, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcActorSelect> Editors = new("Editors", 9, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcDateTime> CreationTime = new("CreationTime", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> LastRevisionTime = new("LastRevisionTime", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> ElectronicFormat = new("ElectronicFormat", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDate> ValidFrom = new("ValidFrom", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDate> ValidUntil = new("ValidUntil", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDocumentConfidentialityEnum> Confidentiality = new("Confidentiality", 15, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDocumentStatusEnum> Status = new("Status", 16, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Identification, Name, Description, Location, Purpose, IntendedUse, Scope, Revision, DocumentOwner, Editors, CreationTime, LastRevisionTime, ElectronicFormat, ValidFrom, ValidUntil, Confidentiality, Status ];
}

public partial class IfcDocumentInformationRelationship
   : IfcResourceLevelRelationship
{
    public static IfcDocumentInformationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTINFORMATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3622737906;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDocumentInformation> RelatingDocument = new("RelatingDocument", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDocumentInformation> RelatedDocuments = new("RelatedDocuments", 3, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> RelationshipType = new("RelationshipType", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingDocument, RelatedDocuments, RelationshipType ];
}

public partial class IfcDocumentReference
   : IfcExternalReference, IfcDocumentSelect
{
    public static IfcDocumentReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTREFERENCE"u8;
    public const uint ENTITY_CODE = 1468122623;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcText> Description = new("Description", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDocumentInformation> ReferencedDocument = new("ReferencedDocument", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, Identification, Name, Description, ReferencedDocument ];
}

public partial class IfcDoor
   : IfcBuiltElement
{
    public static IfcDoor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOOR"u8;
    public const uint ENTITY_CODE = 656740791;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallHeight = new("OverallHeight", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallWidth = new("OverallWidth", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDoorTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDoorTypeOperationEnum> OperationType = new("OperationType", 11, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedOperationType = new("UserDefinedOperationType", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, OverallHeight, OverallWidth, PredefinedType, OperationType, UserDefinedOperationType ];
}

public partial class IfcDoorLiningProperties
   : IfcPreDefinedPropertySet
{
    public static IfcDoorLiningProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOORLININGPROPERTIES"u8;
    public const uint ENTITY_CODE = 3739251787;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LiningDepth = new("LiningDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> LiningThickness = new("LiningThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ThresholdDepth = new("ThresholdDepth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> ThresholdThickness = new("ThresholdThickness", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> TransomThickness = new("TransomThickness", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> TransomOffset = new("TransomOffset", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningOffset = new("LiningOffset", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ThresholdOffset = new("ThresholdOffset", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CasingThickness = new("CasingThickness", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CasingDepth = new("CasingDepth", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 14, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningToPanelOffsetX = new("LiningToPanelOffsetX", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningToPanelOffsetY = new("LiningToPanelOffsetY", 16, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, LiningDepth, LiningThickness, ThresholdDepth, ThresholdThickness, TransomThickness, TransomOffset, LiningOffset, ThresholdOffset, CasingThickness, CasingDepth, ShapeAspectStyle, LiningToPanelOffsetX, LiningToPanelOffsetY ];
}

public partial class IfcDoorPanelProperties
   : IfcPreDefinedPropertySet
{
    public static IfcDoorPanelProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOORPANELPROPERTIES"u8;
    public const uint ENTITY_CODE = 2042941894;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> PanelDepth = new("PanelDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDoorPanelOperationEnum> PanelOperation = new("PanelOperation", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> PanelWidth = new("PanelWidth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDoorPanelPositionEnum> PanelPosition = new("PanelPosition", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, PanelDepth, PanelOperation, PanelWidth, PanelPosition, ShapeAspectStyle ];
}

public partial class IfcDoorType
   : IfcBuiltElementType
{
    public static IfcDoorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOORTYPE"u8;
    public const uint ENTITY_CODE = 2205326319;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDoorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDoorTypeOperationEnum> OperationType = new("OperationType", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcBoolean> ParameterTakesPrecedence = new("ParameterTakesPrecedence", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedOperationType = new("UserDefinedOperationType", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, OperationType, ParameterTakesPrecedence, UserDefinedOperationType ];
}

public partial class IfcDraughtingPreDefinedColour
   : IfcPreDefinedColour
{
    public static IfcDraughtingPreDefinedColour Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDRAUGHTINGPREDEFINEDCOLOUR"u8;
    public const uint ENTITY_CODE = 3795625054;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcDraughtingPreDefinedCurveFont
   : IfcPreDefinedCurveFont
{
    public static IfcDraughtingPreDefinedCurveFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDRAUGHTINGPREDEFINEDCURVEFONT"u8;
    public const uint ENTITY_CODE = 3176071752;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcDuctFitting
   : IfcFlowFitting
{
    public static IfcDuctFitting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDUCTFITTING"u8;
    public const uint ENTITY_CODE = 286836086;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuctFittingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDuctFittingType
   : IfcFlowFittingType
{
    public static IfcDuctFittingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDUCTFITTINGTYPE"u8;
    public const uint ENTITY_CODE = 922394246;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuctFittingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcDuctSegment
   : IfcFlowSegment
{
    public static IfcDuctSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDUCTSEGMENT"u8;
    public const uint ENTITY_CODE = 2069652084;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuctSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDuctSegmentType
   : IfcFlowSegmentType
{
    public static IfcDuctSegmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDUCTSEGMENTTYPE"u8;
    public const uint ENTITY_CODE = 421111644;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuctSegmentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcDuctSilencer
   : IfcFlowTreatmentDevice
{
    public static IfcDuctSilencer Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDUCTSILENCER"u8;
    public const uint ENTITY_CODE = 821901792;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuctSilencerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcDuctSilencerType
   : IfcFlowTreatmentDeviceType
{
    public static IfcDuctSilencerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDUCTSILENCERTYPE"u8;
    public const uint ENTITY_CODE = 3066515080;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuctSilencerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcEarthworksCut
   : IfcFeatureElementSubtraction
{
    public static IfcEarthworksCut Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEARTHWORKSCUT"u8;
    public const uint ENTITY_CODE = 1926198179;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEarthworksCutTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcEarthworksElement
   : IfcBuiltElement
{
    public static IfcEarthworksElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEARTHWORKSELEMENT"u8;
    public const uint ENTITY_CODE = 2128586007;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcEarthworksFill
   : IfcEarthworksElement
{
    public static IfcEarthworksFill Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEARTHWORKSFILL"u8;
    public const uint ENTITY_CODE = 2754986888;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEarthworksFillTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcEdge
   : IfcTopologicalRepresentationItem
{
    public static IfcEdge Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEDGE"u8;
    public const uint ENTITY_CODE = 2965549882;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVertex> EdgeStart = new("EdgeStart", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcVertex> EdgeEnd = new("EdgeEnd", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ EdgeStart, EdgeEnd ];
}

public partial class IfcEdgeCurve
   : IfcEdge, IfcCurveOrEdgeCurve
{
    public static IfcEdgeCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEDGECURVE"u8;
    public const uint ENTITY_CODE = 4051372493;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> EdgeGeometry = new("EdgeGeometry", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> SameSense = new("SameSense", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ EdgeStart, EdgeEnd, EdgeGeometry, SameSense ];
}

public partial class IfcEdgeLoop
   : IfcLoop
{
    public static IfcEdgeLoop Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEDGELOOP"u8;
    public const uint ENTITY_CODE = 1325479016;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOrientedEdge> EdgeList = new("EdgeList", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ EdgeList ];
}

public partial class IfcElectricAppliance
   : IfcFlowTerminal
{
    public static IfcElectricAppliance Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICAPPLIANCE"u8;
    public const uint ENTITY_CODE = 3699799675;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricApplianceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricApplianceType
   : IfcFlowTerminalType
{
    public static IfcElectricApplianceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICAPPLIANCETYPE"u8;
    public const uint ENTITY_CODE = 4222203363;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricApplianceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElectricDistributionBoard
   : IfcFlowController
{
    public static IfcElectricDistributionBoard Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICDISTRIBUTIONBOARD"u8;
    public const uint ENTITY_CODE = 317330310;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricDistributionBoardTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricDistributionBoardType
   : IfcFlowControllerType
{
    public static IfcElectricDistributionBoardType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICDISTRIBUTIONBOARDTYPE"u8;
    public const uint ENTITY_CODE = 3223010614;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricDistributionBoardTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElectricFlowStorageDevice
   : IfcFlowStorageDevice
{
    public static IfcElectricFlowStorageDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICFLOWSTORAGEDEVICE"u8;
    public const uint ENTITY_CODE = 2685000379;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricFlowStorageDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricFlowStorageDeviceType
   : IfcFlowStorageDeviceType
{
    public static IfcElectricFlowStorageDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICFLOWSTORAGEDEVICETYPE"u8;
    public const uint ENTITY_CODE = 2420788771;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricFlowStorageDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElectricFlowTreatmentDevice
   : IfcFlowTreatmentDevice
{
    public static IfcElectricFlowTreatmentDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICFLOWTREATMENTDEVICE"u8;
    public const uint ENTITY_CODE = 2486467058;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricFlowTreatmentDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricFlowTreatmentDeviceType
   : IfcFlowTreatmentDeviceType
{
    public static IfcElectricFlowTreatmentDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICFLOWTREATMENTDEVICETYPE"u8;
    public const uint ENTITY_CODE = 2050120930;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricFlowTreatmentDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElectricGenerator
   : IfcEnergyConversionDevice
{
    public static IfcElectricGenerator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICGENERATOR"u8;
    public const uint ENTITY_CODE = 3133513153;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricGeneratorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricGeneratorType
   : IfcEnergyConversionDeviceType
{
    public static IfcElectricGeneratorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICGENERATORTYPE"u8;
    public const uint ENTITY_CODE = 1023952905;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricGeneratorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElectricMotor
   : IfcEnergyConversionDevice
{
    public static IfcElectricMotor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICMOTOR"u8;
    public const uint ENTITY_CODE = 2669811613;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricMotorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricMotorType
   : IfcEnergyConversionDeviceType
{
    public static IfcElectricMotorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICMOTORTYPE"u8;
    public const uint ENTITY_CODE = 1069776885;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricMotorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElectricTimeControl
   : IfcFlowController
{
    public static IfcElectricTimeControl Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICTIMECONTROL"u8;
    public const uint ENTITY_CODE = 1186683830;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricTimeControlTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcElectricTimeControlType
   : IfcFlowControllerType
{
    public static IfcElectricTimeControlType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICTIMECONTROLTYPE"u8;
    public const uint ENTITY_CODE = 3192508614;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricTimeControlTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElement
   : IfcProduct, IfcInterferenceSelect, IfcStructuralActivityAssignmentSelect
{
    public static IfcElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENT"u8;
    public const uint ENTITY_CODE = 2740753025;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Tag = new("Tag", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcElementarySurface
   : IfcSurface
{
    public static IfcElementarySurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTARYSURFACE"u8;
    public const uint ENTITY_CODE = 623044004;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement3D> Position = new("Position", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Position ];
}

public partial class IfcElementAssembly
   : IfcElement
{
    public static IfcElementAssembly Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTASSEMBLY"u8;
    public const uint ENTITY_CODE = 1851947721;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAssemblyPlaceEnum> AssemblyPlace = new("AssemblyPlace", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcElementAssemblyTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, AssemblyPlace, PredefinedType ];
}

public partial class IfcElementAssemblyType
   : IfcElementType
{
    public static IfcElementAssemblyType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTASSEMBLYTYPE"u8;
    public const uint ENTITY_CODE = 1884542241;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElementAssemblyTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcElementComponent
   : IfcElement
{
    public static IfcElementComponent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTCOMPONENT"u8;
    public const uint ENTITY_CODE = 106112316;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcElementComponentType
   : IfcElementType
{
    public static IfcElementComponentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTCOMPONENTTYPE"u8;
    public const uint ENTITY_CODE = 3322109588;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcElementQuantity
   : IfcQuantitySet
{
    public static IfcElementQuantity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTQUANTITY"u8;
    public const uint ENTITY_CODE = 2079429220;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> MethodOfMeasurement = new("MethodOfMeasurement", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPhysicalQuantity> Quantities = new("Quantities", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, MethodOfMeasurement, Quantities ];
}

public partial class IfcElementType
   : IfcTypeProduct
{
    public static IfcElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 172758729;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ElementType = new("ElementType", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcEllipse
   : IfcConic
{
    public static IfcEllipse Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELLIPSE"u8;
    public const uint ENTITY_CODE = 1311295219;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> SemiAxis1 = new("SemiAxis1", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> SemiAxis2 = new("SemiAxis2", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, SemiAxis1, SemiAxis2 ];
}

public partial class IfcEllipseProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcEllipseProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELLIPSEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 135379651;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> SemiAxis1 = new("SemiAxis1", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> SemiAxis2 = new("SemiAxis2", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, SemiAxis1, SemiAxis2 ];
}

public partial class IfcEnergyConversionDevice
   : IfcDistributionFlowElement
{
    public static IfcEnergyConversionDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCENERGYCONVERSIONDEVICE"u8;
    public const uint ENTITY_CODE = 666967745;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcEnergyConversionDeviceType
   : IfcDistributionFlowElementType
{
    public static IfcEnergyConversionDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCENERGYCONVERSIONDEVICETYPE"u8;
    public const uint ENTITY_CODE = 2323306761;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcEngine
   : IfcEnergyConversionDevice
{
    public static IfcEngine Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCENGINE"u8;
    public const uint ENTITY_CODE = 4060371041;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEngineTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcEngineType
   : IfcEnergyConversionDeviceType
{
    public static IfcEngineType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCENGINETYPE"u8;
    public const uint ENTITY_CODE = 4186151849;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEngineTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcEvaporativeCooler
   : IfcEnergyConversionDevice
{
    public static IfcEvaporativeCooler Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVAPORATIVECOOLER"u8;
    public const uint ENTITY_CODE = 1757195815;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEvaporativeCoolerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcEvaporativeCoolerType
   : IfcEnergyConversionDeviceType
{
    public static IfcEvaporativeCoolerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVAPORATIVECOOLERTYPE"u8;
    public const uint ENTITY_CODE = 2775514815;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEvaporativeCoolerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcEvaporator
   : IfcEnergyConversionDevice
{
    public static IfcEvaporator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVAPORATOR"u8;
    public const uint ENTITY_CODE = 2982541820;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEvaporatorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcEvaporatorType
   : IfcEnergyConversionDeviceType
{
    public static IfcEvaporatorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVAPORATORTYPE"u8;
    public const uint ENTITY_CODE = 4048102996;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEvaporatorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcEvent
   : IfcProcess
{
    public static IfcEvent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVENT"u8;
    public const uint ENTITY_CODE = 3790317085;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEventTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcEventTriggerTypeEnum> EventTriggerType = new("EventTriggerType", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedEventTriggerType = new("UserDefinedEventTriggerType", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcEventTime> EventOccurenceTime = new("EventOccurenceTime", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, PredefinedType, EventTriggerType, UserDefinedEventTriggerType, EventOccurenceTime ];
}

public partial class IfcEventTime
   : IfcSchedulingTime
{
    public static IfcEventTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVENTTIME"u8;
    public const uint ENTITY_CODE = 4110436668;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDateTime> ActualDate = new("ActualDate", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> EarlyDate = new("EarlyDate", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> LateDate = new("LateDate", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ScheduleDate = new("ScheduleDate", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin, ActualDate, EarlyDate, LateDate, ScheduleDate ];
}

public partial class IfcEventType
   : IfcTypeProcess
{
    public static IfcEventType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEVENTTYPE"u8;
    public const uint ENTITY_CODE = 3342300789;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEventTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcEventTriggerTypeEnum> EventTriggerType = new("EventTriggerType", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedEventTriggerType = new("UserDefinedEventTriggerType", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ProcessType, PredefinedType, EventTriggerType, UserDefinedEventTriggerType ];
}

public partial class IfcExtendedProperties
   : IfcPropertyAbstraction
{
    public static IfcExtendedProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTENDEDPROPERTIES"u8;
    public const uint ENTITY_CODE = 1853188283;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProperty> Properties = new("Properties", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, Properties ];
}

public partial class IfcExternalInformation
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcExternalInformation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALINFORMATION"u8;
    public const uint ENTITY_CODE = 2948743570;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcExternallyDefinedHatchStyle
   : IfcExternalReference, IfcFillStyleSelect
{
    public static IfcExternallyDefinedHatchStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDHATCHSTYLE"u8;
    public const uint ENTITY_CODE = 1389487359;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, Identification, Name ];
}

public partial class IfcExternallyDefinedSurfaceStyle
   : IfcExternalReference, IfcSurfaceStyleElementSelect
{
    public static IfcExternallyDefinedSurfaceStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDSURFACESTYLE"u8;
    public const uint ENTITY_CODE = 1184975984;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, Identification, Name ];
}

public partial class IfcExternallyDefinedTextFont
   : IfcExternalReference, IfcTextFontSelect
{
    public static IfcExternallyDefinedTextFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDTEXTFONT"u8;
    public const uint ENTITY_CODE = 4127842378;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, Identification, Name ];
}

public partial class IfcExternalReference
   : EntityBaseClass, IfcLightDistributionDataSourceSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcExternalReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALREFERENCE"u8;
    public const uint ENTITY_CODE = 2775413369;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcURIReference> Location = new("Location", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Location, Identification, Name ];
}

public partial class IfcExternalReferenceRelationship
   : IfcResourceLevelRelationship
{
    public static IfcExternalReferenceRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALREFERENCERELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1909211031;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcExternalReference> RelatingReference = new("RelatingReference", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcResourceObjectSelect> RelatedResourceObjects = new("RelatedResourceObjects", 3, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingReference, RelatedResourceObjects ];
}

public partial class IfcExternalSpatialElement
   : IfcExternalSpatialStructureElement, IfcSpaceBoundarySelect
{
    public static IfcExternalSpatialElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALSPATIALELEMENT"u8;
    public const uint ENTITY_CODE = 1964571028;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcExternalSpatialElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, PredefinedType ];
}

public partial class IfcExternalSpatialStructureElement
   : IfcSpatialElement
{
    public static IfcExternalSpatialStructureElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALSPATIALSTRUCTUREELEMENT"u8;
    public const uint ENTITY_CODE = 1270689859;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName ];
}

public partial class IfcExtrudedAreaSolid
   : IfcSweptAreaSolid
{
    public static IfcExtrudedAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTRUDEDAREASOLID"u8;
    public const uint ENTITY_CODE = 760414336;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> ExtrudedDirection = new("ExtrudedDirection", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, ExtrudedDirection, Depth ];
}

public partial class IfcExtrudedAreaSolidTapered
   : IfcExtrudedAreaSolid
{
    public static IfcExtrudedAreaSolidTapered Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTRUDEDAREASOLIDTAPERED"u8;
    public const uint ENTITY_CODE = 2416391719;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> EndSweptArea = new("EndSweptArea", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, ExtrudedDirection, Depth, EndSweptArea ];
}

public partial class IfcFace
   : IfcTopologicalRepresentationItem
{
    public static IfcFace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACE"u8;
    public const uint ENTITY_CODE = 781347094;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFaceBound> Bounds = new("Bounds", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Bounds ];
}

public partial class IfcFaceBasedSurfaceModel
   : IfcGeometricRepresentationItem, IfcSurfaceOrFaceSurface
{
    public static IfcFaceBasedSurfaceModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACEBASEDSURFACEMODEL"u8;
    public const uint ENTITY_CODE = 2994652321;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConnectedFaceSet> FbsmFaces = new("FbsmFaces", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ FbsmFaces ];
}

public partial class IfcFaceBound
   : IfcTopologicalRepresentationItem
{
    public static IfcFaceBound Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACEBOUND"u8;
    public const uint ENTITY_CODE = 2152074782;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLoop> Bound = new("Bound", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> Orientation = new("Orientation", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Bound, Orientation ];
}

public partial class IfcFaceOuterBound
   : IfcFaceBound
{
    public static IfcFaceOuterBound Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACEOUTERBOUND"u8;
    public const uint ENTITY_CODE = 1893838371;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Bound, Orientation ];
}

public partial class IfcFaceSurface
   : IfcFace, IfcSurfaceOrFaceSurface
{
    public static IfcFaceSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACESURFACE"u8;
    public const uint ENTITY_CODE = 955478517;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> FaceSurface = new("FaceSurface", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> SameSense = new("SameSense", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Bounds, FaceSurface, SameSense ];
}

public partial class IfcFacetedBrep
   : IfcManifoldSolidBrep
{
    public static IfcFacetedBrep Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACETEDBREP"u8;
    public const uint ENTITY_CODE = 4040723506;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Outer ];
}

public partial class IfcFacetedBrepWithVoids
   : IfcFacetedBrep
{
    public static IfcFacetedBrepWithVoids Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACETEDBREPWITHVOIDS"u8;
    public const uint ENTITY_CODE = 712432441;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClosedShell> Voids = new("Voids", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Outer, Voids ];
}

public partial class IfcFacility
   : IfcSpatialStructureElement
{
    public static IfcFacility Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACILITY"u8;
    public const uint ENTITY_CODE = 3804118066;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType ];
}

public partial class IfcFacilityPart
   : IfcSpatialStructureElement
{
    public static IfcFacilityPart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACILITYPART"u8;
    public const uint ENTITY_CODE = 1046131315;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFacilityUsageEnum> UsageType = new("UsageType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, UsageType ];
}

public partial class IfcFacilityPartCommon
   : IfcFacilityPart
{
    public static IfcFacilityPartCommon Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACILITYPARTCOMMON"u8;
    public const uint ENTITY_CODE = 3018973202;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFacilityPartCommonTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, UsageType, PredefinedType ];
}

public partial class IfcFailureConnectionCondition
   : IfcStructuralConnectionCondition
{
    public static IfcFailureConnectionCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFAILURECONNECTIONCONDITION"u8;
    public const uint ENTITY_CODE = 1679012808;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcForceMeasure> TensionFailureX = new("TensionFailureX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> TensionFailureY = new("TensionFailureY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> TensionFailureZ = new("TensionFailureZ", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> CompressionFailureX = new("CompressionFailureX", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> CompressionFailureY = new("CompressionFailureY", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> CompressionFailureZ = new("CompressionFailureZ", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, TensionFailureX, TensionFailureY, TensionFailureZ, CompressionFailureX, CompressionFailureY, CompressionFailureZ ];
}

public partial class IfcFan
   : IfcFlowMovingDevice
{
    public static IfcFan Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFAN"u8;
    public const uint ENTITY_CODE = 2700567456;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFanTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFanType
   : IfcFlowMovingDeviceType
{
    public static IfcFanType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFANTYPE"u8;
    public const uint ENTITY_CODE = 3999264072;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFanTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFastener
   : IfcElementComponent
{
    public static IfcFastener Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFASTENER"u8;
    public const uint ENTITY_CODE = 939314313;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFastenerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFastenerType
   : IfcElementComponentType
{
    public static IfcFastenerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFASTENERTYPE"u8;
    public const uint ENTITY_CODE = 4273197281;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFastenerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFeatureElement
   : IfcElement
{
    public static IfcFeatureElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFEATUREELEMENT"u8;
    public const uint ENTITY_CODE = 3548597237;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFeatureElementAddition
   : IfcFeatureElement
{
    public static IfcFeatureElementAddition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFEATUREELEMENTADDITION"u8;
    public const uint ENTITY_CODE = 2080850745;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFeatureElementSubtraction
   : IfcFeatureElement
{
    public static IfcFeatureElementSubtraction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFEATUREELEMENTSUBTRACTION"u8;
    public const uint ENTITY_CODE = 297830833;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFillAreaStyle
   : IfcPresentationStyle
{
    public static IfcFillAreaStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILLAREASTYLE"u8;
    public const uint ENTITY_CODE = 1860673172;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFillStyleSelect> FillStyles = new("FillStyles", 1, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcBoolean> ModelOrDraughting = new("ModelOrDraughting", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, FillStyles, ModelOrDraughting ];
}

public partial class IfcFillAreaStyleHatching
   : IfcGeometricRepresentationItem, IfcFillStyleSelect
{
    public static IfcFillAreaStyleHatching Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILLAREASTYLEHATCHING"u8;
    public const uint ENTITY_CODE = 11578102;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurveStyle> HatchLineAppearance = new("HatchLineAppearance", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcHatchLineDistanceSelect> StartOfNextHatchLine = new("StartOfNextHatchLine", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCartesianPoint> PointOfReferenceHatchLine = new("PointOfReferenceHatchLine", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCartesianPoint> PatternStart = new("PatternStart", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> HatchLineAngle = new("HatchLineAngle", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ HatchLineAppearance, StartOfNextHatchLine, PointOfReferenceHatchLine, PatternStart, HatchLineAngle ];
}

public partial class IfcFillAreaStyleTiles
   : IfcGeometricRepresentationItem, IfcFillStyleSelect
{
    public static IfcFillAreaStyleTiles Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILLAREASTYLETILES"u8;
    public const uint ENTITY_CODE = 1624792585;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVector> TilingPattern = new("TilingPattern", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcStyledItem> Tiles = new("Tiles", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> TilingScale = new("TilingScale", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ TilingPattern, Tiles, TilingScale ];
}

public partial class IfcFilter
   : IfcFlowTreatmentDevice
{
    public static IfcFilter Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILTER"u8;
    public const uint ENTITY_CODE = 3218732281;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFilterTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFilterType
   : IfcFlowTreatmentDeviceType
{
    public static IfcFilterType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILTERTYPE"u8;
    public const uint ENTITY_CODE = 2892583665;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFilterTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFireSuppressionTerminal
   : IfcFlowTerminal
{
    public static IfcFireSuppressionTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFIRESUPPRESSIONTERMINAL"u8;
    public const uint ENTITY_CODE = 734748586;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFireSuppressionTerminalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFireSuppressionTerminalType
   : IfcFlowTerminalType
{
    public static IfcFireSuppressionTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFIRESUPPRESSIONTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 1473808138;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFireSuppressionTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFixedReferenceSweptAreaSolid
   : IfcDirectrixCurveSweptAreaSolid
{
    public static IfcFixedReferenceSweptAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFIXEDREFERENCESWEPTAREASOLID"u8;
    public const uint ENTITY_CODE = 3182592983;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> FixedReference = new("FixedReference", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Directrix, StartParam, EndParam, FixedReference ];
}

public partial class IfcFlowController
   : IfcDistributionFlowElement
{
    public static IfcFlowController Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWCONTROLLER"u8;
    public const uint ENTITY_CODE = 1745256663;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowControllerType
   : IfcDistributionFlowElementType
{
    public static IfcFlowControllerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWCONTROLLERTYPE"u8;
    public const uint ENTITY_CODE = 3279813135;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFlowFitting
   : IfcDistributionFlowElement
{
    public static IfcFlowFitting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWFITTING"u8;
    public const uint ENTITY_CODE = 90764182;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowFittingType
   : IfcDistributionFlowElementType
{
    public static IfcFlowFittingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWFITTINGTYPE"u8;
    public const uint ENTITY_CODE = 3152900518;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFlowInstrument
   : IfcDistributionControlElement
{
    public static IfcFlowInstrument Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWINSTRUMENT"u8;
    public const uint ENTITY_CODE = 1686116054;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFlowInstrumentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFlowInstrumentType
   : IfcDistributionControlElementType
{
    public static IfcFlowInstrumentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWINSTRUMENTTYPE"u8;
    public const uint ENTITY_CODE = 2837527270;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFlowInstrumentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFlowMeter
   : IfcFlowController
{
    public static IfcFlowMeter Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWMETER"u8;
    public const uint ENTITY_CODE = 340947456;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFlowMeterTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFlowMeterType
   : IfcFlowControllerType
{
    public static IfcFlowMeterType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWMETERTYPE"u8;
    public const uint ENTITY_CODE = 2000178472;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFlowMeterTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFlowMovingDevice
   : IfcDistributionFlowElement
{
    public static IfcFlowMovingDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWMOVINGDEVICE"u8;
    public const uint ENTITY_CODE = 2147655891;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowMovingDeviceType
   : IfcDistributionFlowElementType
{
    public static IfcFlowMovingDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWMOVINGDEVICETYPE"u8;
    public const uint ENTITY_CODE = 696395307;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFlowSegment
   : IfcDistributionFlowElement
{
    public static IfcFlowSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWSEGMENT"u8;
    public const uint ENTITY_CODE = 138616340;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowSegmentType
   : IfcDistributionFlowElementType
{
    public static IfcFlowSegmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWSEGMENTTYPE"u8;
    public const uint ENTITY_CODE = 2432356604;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFlowStorageDevice
   : IfcDistributionFlowElement
{
    public static IfcFlowStorageDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWSTORAGEDEVICE"u8;
    public const uint ENTITY_CODE = 2898108386;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowStorageDeviceType
   : IfcDistributionFlowElementType
{
    public static IfcFlowStorageDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWSTORAGEDEVICETYPE"u8;
    public const uint ENTITY_CODE = 4142556786;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFlowTerminal
   : IfcDistributionFlowElement
{
    public static IfcFlowTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWTERMINAL"u8;
    public const uint ENTITY_CODE = 3130859319;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowTerminalType
   : IfcDistributionFlowElementType
{
    public static IfcFlowTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 733872495;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFlowTreatmentDevice
   : IfcDistributionFlowElement
{
    public static IfcFlowTreatmentDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWTREATMENTDEVICE"u8;
    public const uint ENTITY_CODE = 314821475;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFlowTreatmentDeviceType
   : IfcDistributionFlowElementType
{
    public static IfcFlowTreatmentDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLOWTREATMENTDEVICETYPE"u8;
    public const uint ENTITY_CODE = 751709595;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFooting
   : IfcBuiltElement
{
    public static IfcFooting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFOOTING"u8;
    public const uint ENTITY_CODE = 1345078513;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFootingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFootingType
   : IfcBuiltElementType
{
    public static IfcFootingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFOOTINGTYPE"u8;
    public const uint ENTITY_CODE = 114099353;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFootingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcFurnishingElement
   : IfcElement
{
    public static IfcFurnishingElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFURNISHINGELEMENT"u8;
    public const uint ENTITY_CODE = 1635784606;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFurnishingElementType
   : IfcElementType
{
    public static IfcFurnishingElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFURNISHINGELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1882586014;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcFurniture
   : IfcFurnishingElement
{
    public static IfcFurniture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFURNITURE"u8;
    public const uint ENTITY_CODE = 3405948931;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFurnitureTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFurnitureType
   : IfcFurnishingElementType
{
    public static IfcFurnitureType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFURNITURETYPE"u8;
    public const uint ENTITY_CODE = 3998095675;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAssemblyPlaceEnum> AssemblyPlace = new("AssemblyPlace", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcFurnitureTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, AssemblyPlace, PredefinedType ];
}

public partial class IfcGeographicElement
   : IfcElement
{
    public static IfcGeographicElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOGRAPHICELEMENT"u8;
    public const uint ENTITY_CODE = 471584060;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGeographicElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcGeographicElementType
   : IfcElementType
{
    public static IfcGeographicElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOGRAPHICELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 2041480852;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGeographicElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcGeometricCurveSet
   : IfcGeometricSet
{
    public static IfcGeometricCurveSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMETRICCURVESET"u8;
    public const uint ENTITY_CODE = 2960295997;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Elements ];
}

public partial class IfcGeometricRepresentationContext
   : IfcRepresentationContext, IfcCoordinateReferenceSystemSelect
{
    public static IfcGeometricRepresentationContext Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMETRICREPRESENTATIONCONTEXT"u8;
    public const uint ENTITY_CODE = 1928810440;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDimensionCount> CoordinateSpaceDimension = new("CoordinateSpaceDimension", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> Precision = new("Precision", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAxis2Placement> WorldCoordinateSystem = new("WorldCoordinateSystem", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDirection> TrueNorth = new("TrueNorth", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ContextIdentifier, ContextType, CoordinateSpaceDimension, Precision, WorldCoordinateSystem, TrueNorth ];
}

public partial class IfcGeometricRepresentationItem
   : IfcRepresentationItem
{
    public static IfcGeometricRepresentationItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMETRICREPRESENTATIONITEM"u8;
    public const uint ENTITY_CODE = 1608106874;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcGeometricRepresentationSubContext
   : IfcGeometricRepresentationContext
{
    public static IfcGeometricRepresentationSubContext Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMETRICREPRESENTATIONSUBCONTEXT"u8;
    public const uint ENTITY_CODE = 704017320;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGeometricRepresentationContext> ParentContext = new("ParentContext", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> TargetScale = new("TargetScale", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcGeometricProjectionEnum> TargetView = new("TargetView", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedTargetView = new("UserDefinedTargetView", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ContextIdentifier, ContextType, CoordinateSpaceDimension, Precision, WorldCoordinateSystem, TrueNorth, ParentContext, TargetScale, TargetView, UserDefinedTargetView ];
}

public partial class IfcGeometricSet
   : IfcGeometricRepresentationItem
{
    public static IfcGeometricSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMETRICSET"u8;
    public const uint ENTITY_CODE = 183455396;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGeometricSetSelect> Elements = new("Elements", 0, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Elements ];
}

public partial class IfcGeomodel
   : IfcGeotechnicalAssembly
{
    public static IfcGeomodel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMODEL"u8;
    public const uint ENTITY_CODE = 34812629;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcGeoslice
   : IfcGeotechnicalAssembly
{
    public static IfcGeoslice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOSLICE"u8;
    public const uint ENTITY_CODE = 66322586;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcGeotechnicalAssembly
   : IfcGeotechnicalElement
{
    public static IfcGeotechnicalAssembly Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOTECHNICALASSEMBLY"u8;
    public const uint ENTITY_CODE = 2572603559;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcGeotechnicalElement
   : IfcElement
{
    public static IfcGeotechnicalElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOTECHNICALELEMENT"u8;
    public const uint ENTITY_CODE = 3333231277;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcGeotechnicalStratum
   : IfcGeotechnicalElement
{
    public static IfcGeotechnicalStratum Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOTECHNICALSTRATUM"u8;
    public const uint ENTITY_CODE = 506043921;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGeotechnicalStratumTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcGradientCurve
   : IfcCompositeCurve
{
    public static IfcGradientCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGRADIENTCURVE"u8;
    public const uint ENTITY_CODE = 3480717906;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoundedCurve> BaseCurve = new("BaseCurve", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPlacement> EndPoint = new("EndPoint", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect, BaseCurve, EndPoint ];
}

public partial class IfcGrid
   : IfcPositioningElement
{
    public static IfcGrid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGRID"u8;
    public const uint ENTITY_CODE = 2792790963;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGridAxis> UAxes = new("UAxes", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcGridAxis> VAxes = new("VAxes", 8, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcGridAxis> WAxes = new("WAxes", 9, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcGridTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, UAxes, VAxes, WAxes, PredefinedType ];
}

public partial class IfcGridAxis
   : EntityBaseClass
{
    public static IfcGridAxis Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGRIDAXIS"u8;
    public const uint ENTITY_CODE = 2705774078;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> AxisTag = new("AxisTag", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCurve> AxisCurve = new("AxisCurve", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> SameSense = new("SameSense", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ AxisTag, AxisCurve, SameSense ];
}

public partial class IfcGridPlacement
   : IfcObjectPlacement
{
    public static IfcGridPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGRIDPLACEMENT"u8;
    public const uint ENTITY_CODE = 334024922;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVirtualGridIntersection> PlacementLocation = new("PlacementLocation", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcGridPlacementDirectionSelect> PlacementRefDirection = new("PlacementRefDirection", 2, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ PlacementRelTo, PlacementLocation, PlacementRefDirection ];
}

public partial class IfcGroup
   : IfcObject, IfcSpatialReferenceSelect
{
    public static IfcGroup Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGROUP"u8;
    public const uint ENTITY_CODE = 540599526;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcHalfSpaceSolid
   : IfcGeometricRepresentationItem, IfcBooleanOperand
{
    public static IfcHalfSpaceSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCHALFSPACESOLID"u8;
    public const uint ENTITY_CODE = 3049817347;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> BaseSurface = new("BaseSurface", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> AgreementFlag = new("AgreementFlag", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BaseSurface, AgreementFlag ];
}

public partial class IfcHeatExchanger
   : IfcEnergyConversionDevice
{
    public static IfcHeatExchanger Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCHEATEXCHANGER"u8;
    public const uint ENTITY_CODE = 2012280710;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcHeatExchangerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcHeatExchangerType
   : IfcEnergyConversionDeviceType
{
    public static IfcHeatExchangerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCHEATEXCHANGERTYPE"u8;
    public const uint ENTITY_CODE = 1470914870;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcHeatExchangerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcHumidifier
   : IfcEnergyConversionDevice
{
    public static IfcHumidifier Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCHUMIDIFIER"u8;
    public const uint ENTITY_CODE = 4041992875;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcHumidifierTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcHumidifierType
   : IfcEnergyConversionDeviceType
{
    public static IfcHumidifierType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCHUMIDIFIERTYPE"u8;
    public const uint ENTITY_CODE = 4247700979;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcHumidifierTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcImageTexture
   : IfcSurfaceTexture
{
    public static IfcImageTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCIMAGETEXTURE"u8;
    public const uint ENTITY_CODE = 582144863;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcURIReference> URLReference = new("URLReference", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, Mode, TextureTransform, Parameter, URLReference ];
}

public partial class IfcImpactProtectionDevice
   : IfcElementComponent
{
    public static IfcImpactProtectionDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCIMPACTPROTECTIONDEVICE"u8;
    public const uint ENTITY_CODE = 3206231084;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcImpactProtectionDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcImpactProtectionDeviceType
   : IfcElementComponentType
{
    public static IfcImpactProtectionDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCIMPACTPROTECTIONDEVICETYPE"u8;
    public const uint ENTITY_CODE = 2274240804;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcImpactProtectionDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcIndexedColourMap
   : IfcPresentationItem
{
    public static IfcIndexedColourMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDCOLOURMAP"u8;
    public const uint ENTITY_CODE = 4284624470;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTessellatedFaceSet> MappedTo = new("MappedTo", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Opacity = new("Opacity", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcColourRgbList> Colours = new("Colours", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveInteger> ColourIndex = new("ColourIndex", 3, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ MappedTo, Opacity, Colours, ColourIndex ];
}

public partial class IfcIndexedPolyCurve
   : IfcBoundedCurve
{
    public static IfcIndexedPolyCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDPOLYCURVE"u8;
    public const uint ENTITY_CODE = 3849649469;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPointList> Points = new("Points", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSegmentIndexSelect> Segments = new("Segments", 1, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcLogical> SelfIntersect = new("SelfIntersect", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Points, Segments, SelfIntersect ];
}

public partial class IfcIndexedPolygonalFace
   : IfcTessellatedItem
{
    public static IfcIndexedPolygonalFace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDPOLYGONALFACE"u8;
    public const uint ENTITY_CODE = 505122710;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveInteger> CoordIndex = new("CoordIndex", 0, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ CoordIndex ];
}

public partial class IfcIndexedPolygonalFaceWithVoids
   : IfcIndexedPolygonalFace
{
    public static IfcIndexedPolygonalFaceWithVoids Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDPOLYGONALFACEWITHVOIDS"u8;
    public const uint ENTITY_CODE = 964216797;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveInteger> InnerCoordIndices = new("InnerCoordIndices", 1, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ CoordIndex, InnerCoordIndices ];
}

public partial class IfcIndexedPolygonalTextureMap
   : IfcIndexedTextureMap
{
    public static IfcIndexedPolygonalTextureMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDPOLYGONALTEXTUREMAP"u8;
    public const uint ENTITY_CODE = 1609869528;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTextureCoordinateIndices> TexCoordIndices = new("TexCoordIndices", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Maps, MappedTo, TexCoords, TexCoordIndices ];
}

public partial class IfcIndexedTextureMap
   : IfcTextureCoordinate
{
    public static IfcIndexedTextureMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDTEXTUREMAP"u8;
    public const uint ENTITY_CODE = 3155363591;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTessellatedFaceSet> MappedTo = new("MappedTo", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTextureVertexList> TexCoords = new("TexCoords", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Maps, MappedTo, TexCoords ];
}

public partial class IfcIndexedTriangleTextureMap
   : IfcIndexedTextureMap
{
    public static IfcIndexedTriangleTextureMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINDEXEDTRIANGLETEXTUREMAP"u8;
    public const uint ENTITY_CODE = 1236491587;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveInteger> TexCoordIndex = new("TexCoordIndex", 3, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ Maps, MappedTo, TexCoords, TexCoordIndex ];
}

public partial class IfcInterceptor
   : IfcFlowTreatmentDevice
{
    public static IfcInterceptor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINTERCEPTOR"u8;
    public const uint ENTITY_CODE = 3747360886;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInterceptorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcInterceptorType
   : IfcFlowTreatmentDeviceType
{
    public static IfcInterceptorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINTERCEPTORTYPE"u8;
    public const uint ENTITY_CODE = 3838425478;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInterceptorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcIntersectionCurve
   : IfcSurfaceCurve
{
    public static IfcIntersectionCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINTERSECTIONCURVE"u8;
    public const uint ENTITY_CODE = 2945608229;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Curve3D, AssociatedGeometry, MasterRepresentation ];
}

public partial class IfcInventory
   : IfcGroup
{
    public static IfcInventory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINVENTORY"u8;
    public const uint ENTITY_CODE = 3189971553;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInventoryTypeEnum> PredefinedType = new("PredefinedType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcActorSelect> Jurisdiction = new("Jurisdiction", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPerson> ResponsiblePersons = new("ResponsiblePersons", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcDate> LastUpdateDate = new("LastUpdateDate", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCostValue> CurrentValue = new("CurrentValue", 9, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> OriginalValue = new("OriginalValue", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, Jurisdiction, ResponsiblePersons, LastUpdateDate, CurrentValue, OriginalValue ];
}

public partial class IfcIrregularTimeSeries
   : IfcTimeSeries
{
    public static IfcIrregularTimeSeries Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCIRREGULARTIMESERIES"u8;
    public const uint ENTITY_CODE = 2786556632;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIrregularTimeSeriesValue> Values = new("Values", 8, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, StartTime, EndTime, TimeSeriesDataType, DataOrigin, UserDefinedDataOrigin, Unit, Values ];
}

public partial class IfcIrregularTimeSeriesValue
   : EntityBaseClass
{
    public static IfcIrregularTimeSeriesValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCIRREGULARTIMESERIESVALUE"u8;
    public const uint ENTITY_CODE = 867697161;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDateTime> TimeStamp = new("TimeStamp", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcValue> ListValues = new("ListValues", 1, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ TimeStamp, ListValues ];
}

public partial class IfcIShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcIShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCISHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 1683013415;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallWidth = new("OverallWidth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallDepth = new("OverallDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeThickness = new("FlangeThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FlangeEdgeRadius = new("FlangeEdgeRadius", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> FlangeSlope = new("FlangeSlope", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, OverallWidth, OverallDepth, WebThickness, FlangeThickness, FilletRadius, FlangeEdgeRadius, FlangeSlope ];
}

public partial class IfcJunctionBox
   : IfcFlowFitting
{
    public static IfcJunctionBox Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCJUNCTIONBOX"u8;
    public const uint ENTITY_CODE = 2170279028;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcJunctionBoxTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcJunctionBoxType
   : IfcFlowFittingType
{
    public static IfcJunctionBoxType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCJUNCTIONBOXTYPE"u8;
    public const uint ENTITY_CODE = 4095621468;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcJunctionBoxTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcKerb
   : IfcBuiltElement
{
    public static IfcKerb Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCKERB"u8;
    public const uint ENTITY_CODE = 2608887403;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> Mountable = new("Mountable", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, Mountable ];
}

public partial class IfcKerbType
   : IfcBuiltElementType
{
    public static IfcKerbType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCKERBTYPE"u8;
    public const uint ENTITY_CODE = 3270011059;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> Mountable = new("Mountable", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, Mountable ];
}

public partial class IfcLaborResource
   : IfcConstructionResource
{
    public static IfcLaborResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLABORRESOURCE"u8;
    public const uint ENTITY_CODE = 1950317855;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLaborResourceTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcLaborResourceType
   : IfcConstructionResourceType
{
    public static IfcLaborResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLABORRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 951810023;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLaborResourceTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcLagTime
   : IfcSchedulingTime
{
    public static IfcLagTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLAGTIME"u8;
    public const uint ENTITY_CODE = 2116801068;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTimeOrRatioSelect> LagValue = new("LagValue", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTaskDurationEnum> DurationType = new("DurationType", 4, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin, LagValue, DurationType ];
}

public partial class IfcLamp
   : IfcFlowTerminal
{
    public static IfcLamp Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLAMP"u8;
    public const uint ENTITY_CODE = 377756397;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLampTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcLampType
   : IfcFlowTerminalType
{
    public static IfcLampType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLAMPTYPE"u8;
    public const uint ENTITY_CODE = 584324773;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLampTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcLibraryInformation
   : IfcExternalInformation, IfcLibrarySelect
{
    public static IfcLibraryInformation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIBRARYINFORMATION"u8;
    public const uint ENTITY_CODE = 368329652;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Version = new("Version", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorSelect> Publisher = new("Publisher", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTime> VersionDate = new("VersionDate", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcURIReference> Location = new("Location", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Version, Publisher, VersionDate, Location, Description ];
}

public partial class IfcLibraryReference
   : IfcExternalReference, IfcLibrarySelect
{
    public static IfcLibraryReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIBRARYREFERENCE"u8;
    public const uint ENTITY_CODE = 4036302135;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcText> Description = new("Description", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLanguageId> Language = new("Language", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLibraryInformation> ReferencedLibrary = new("ReferencedLibrary", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, Identification, Name, Description, Language, ReferencedLibrary ];
}

public partial class IfcLightDistributionData
   : EntityBaseClass
{
    public static IfcLightDistributionData Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTDISTRIBUTIONDATA"u8;
    public const uint ENTITY_CODE = 404276647;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlaneAngleMeasure> MainPlaneAngle = new("MainPlaneAngle", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> SecondaryPlaneAngle = new("SecondaryPlaneAngle", 1, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLuminousIntensityDistributionMeasure> LuminousIntensity = new("LuminousIntensity", 2, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ MainPlaneAngle, SecondaryPlaneAngle, LuminousIntensity ];
}

public partial class IfcLightFixture
   : IfcFlowTerminal
{
    public static IfcLightFixture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTFIXTURE"u8;
    public const uint ENTITY_CODE = 2840077262;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLightFixtureTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcLightFixtureType
   : IfcFlowTerminalType
{
    public static IfcLightFixtureType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTFIXTURETYPE"u8;
    public const uint ENTITY_CODE = 351014574;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLightFixtureTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcLightIntensityDistribution
   : EntityBaseClass, IfcLightDistributionDataSourceSelect
{
    public static IfcLightIntensityDistribution Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTINTENSITYDISTRIBUTION"u8;
    public const uint ENTITY_CODE = 762471812;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLightDistributionCurveEnum> LightDistributionCurve = new("LightDistributionCurve", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLightDistributionData> DistributionData = new("DistributionData", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ LightDistributionCurve, DistributionData ];
}

public partial class IfcLightSource
   : IfcGeometricRepresentationItem
{
    public static IfcLightSource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTSOURCE"u8;
    public const uint ENTITY_CODE = 1574621316;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcColourRgb> LightColour = new("LightColour", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> AmbientIntensity = new("AmbientIntensity", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Intensity = new("Intensity", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LightColour, AmbientIntensity, Intensity ];
}

public partial class IfcLightSourceAmbient
   : IfcLightSource
{
    public static IfcLightSourceAmbient Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTSOURCEAMBIENT"u8;
    public const uint ENTITY_CODE = 1474471916;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, LightColour, AmbientIntensity, Intensity ];
}

public partial class IfcLightSourceDirectional
   : IfcLightSource
{
    public static IfcLightSourceDirectional Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTSOURCEDIRECTIONAL"u8;
    public const uint ENTITY_CODE = 163866176;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Orientation = new("Orientation", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, LightColour, AmbientIntensity, Intensity, Orientation ];
}

public partial class IfcLightSourceGoniometric
   : IfcLightSource
{
    public static IfcLightSourceGoniometric Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTSOURCEGONIOMETRIC"u8;
    public const uint ENTITY_CODE = 950122348;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement3D> Position = new("Position", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcColourRgb> ColourAppearance = new("ColourAppearance", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> ColourTemperature = new("ColourTemperature", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLuminousFluxMeasure> LuminousFlux = new("LuminousFlux", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLightEmissionSourceEnum> LightEmissionSource = new("LightEmissionSource", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLightDistributionDataSourceSelect> LightDistributionDataSource = new("LightDistributionDataSource", 9, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, LightColour, AmbientIntensity, Intensity, Position, ColourAppearance, ColourTemperature, LuminousFlux, LightEmissionSource, LightDistributionDataSource ];
}

public partial class IfcLightSourcePositional
   : IfcLightSource
{
    public static IfcLightSourcePositional Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTSOURCEPOSITIONAL"u8;
    public const uint ENTITY_CODE = 1991782538;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> Position = new("Position", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> ConstantAttenuation = new("ConstantAttenuation", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> DistanceAttenuation = new("DistanceAttenuation", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> QuadricAttenuation = new("QuadricAttenuation", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LightColour, AmbientIntensity, Intensity, Position, Radius, ConstantAttenuation, DistanceAttenuation, QuadricAttenuation ];
}

public partial class IfcLightSourceSpot
   : IfcLightSourcePositional
{
    public static IfcLightSourceSpot Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIGHTSOURCESPOT"u8;
    public const uint ENTITY_CODE = 2084681292;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Orientation = new("Orientation", 9, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcReal> ConcentrationExponent = new("ConcentrationExponent", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositivePlaneAngleMeasure> SpreadAngle = new("SpreadAngle", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositivePlaneAngleMeasure> BeamWidthAngle = new("BeamWidthAngle", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LightColour, AmbientIntensity, Intensity, Position, Radius, ConstantAttenuation, DistanceAttenuation, QuadricAttenuation, Orientation, ConcentrationExponent, SpreadAngle, BeamWidthAngle ];
}

public partial class IfcLine
   : IfcCurve
{
    public static IfcLine Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLINE"u8;
    public const uint ENTITY_CODE = 2591592509;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> Pnt = new("Pnt", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcVector> Dir = new("Dir", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Pnt, Dir ];
}

public partial class IfcLinearElement
   : IfcProduct
{
    public static IfcLinearElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLINEARELEMENT"u8;
    public const uint ENTITY_CODE = 2826022314;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcLinearPlacement
   : IfcObjectPlacement
{
    public static IfcLinearPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLINEARPLACEMENT"u8;
    public const uint ENTITY_CODE = 3170425317;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2PlacementLinear> RelativePlacement = new("RelativePlacement", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAxis2Placement3D> CartesianPosition = new("CartesianPosition", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ PlacementRelTo, RelativePlacement, CartesianPosition ];
}

public partial class IfcLinearPositioningElement
   : IfcPositioningElement
{
    public static IfcLinearPositioningElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLINEARPOSITIONINGELEMENT"u8;
    public const uint ENTITY_CODE = 3292604933;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcLiquidTerminal
   : IfcFlowTerminal
{
    public static IfcLiquidTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIQUIDTERMINAL"u8;
    public const uint ENTITY_CODE = 1767222947;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLiquidTerminalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcLiquidTerminalType
   : IfcFlowTerminalType
{
    public static IfcLiquidTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIQUIDTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 66251227;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLiquidTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcLocalPlacement
   : IfcObjectPlacement
{
    public static IfcLocalPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLOCALPLACEMENT"u8;
    public const uint ENTITY_CODE = 4159386377;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement> RelativePlacement = new("RelativePlacement", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ PlacementRelTo, RelativePlacement ];
}

public partial class IfcLoop
   : IfcTopologicalRepresentationItem
{
    public static IfcLoop Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLOOP"u8;
    public const uint ENTITY_CODE = 752393365;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcLShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcLShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLSHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2455248390;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Width = new("Width", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Thickness = new("Thickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FilletRadius = new("FilletRadius", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> EdgeRadius = new("EdgeRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> LegSlope = new("LegSlope", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, Width, Thickness, FilletRadius, EdgeRadius, LegSlope ];
}

public partial class IfcManifoldSolidBrep
   : IfcSolidModel
{
    public static IfcManifoldSolidBrep Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMANIFOLDSOLIDBREP"u8;
    public const uint ENTITY_CODE = 892381835;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClosedShell> Outer = new("Outer", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Outer ];
}

public partial class IfcMapConversion
   : IfcCoordinateOperation
{
    public static IfcMapConversion Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMAPCONVERSION"u8;
    public const uint ENTITY_CODE = 1045754831;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> Eastings = new("Eastings", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> Northings = new("Northings", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> OrthogonalHeight = new("OrthogonalHeight", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> XAxisAbscissa = new("XAxisAbscissa", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> XAxisOrdinate = new("XAxisOrdinate", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> Scale = new("Scale", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> ScaleY = new("ScaleY", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> ScaleZ = new("ScaleZ", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ SourceCRS, TargetCRS, Eastings, Northings, OrthogonalHeight, XAxisAbscissa, XAxisOrdinate, Scale, ScaleY, ScaleZ ];
}

public partial class IfcMappedItem
   : IfcRepresentationItem
{
    public static IfcMappedItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMAPPEDITEM"u8;
    public const uint ENTITY_CODE = 4243798619;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRepresentationMap> MappingSource = new("MappingSource", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCartesianTransformationOperator> MappingTarget = new("MappingTarget", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ MappingSource, MappingTarget ];
}

public partial class IfcMarineFacility
   : IfcFacility
{
    public static IfcMarineFacility Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMARINEFACILITY"u8;
    public const uint ENTITY_CODE = 2338166444;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMarineFacilityTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, PredefinedType ];
}

public partial class IfcMarinePart
   : IfcFacilityPart
{
    public static IfcMarinePart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMARINEPART"u8;
    public const uint ENTITY_CODE = 1309270328;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMarinePartTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, UsageType, PredefinedType ];
}

public partial class IfcMaterial
   : IfcMaterialDefinition
{
    public static IfcMaterial Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIAL"u8;
    public const uint ENTITY_CODE = 1595842790;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Category = new("Category", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Category ];
}

public partial class IfcMaterialClassificationRelationship
   : EntityBaseClass
{
    public static IfcMaterialClassificationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALCLASSIFICATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1549328080;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationSelect> MaterialClassifications = new("MaterialClassifications", 0, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcMaterial> ClassifiedMaterial = new("ClassifiedMaterial", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ MaterialClassifications, ClassifiedMaterial ];
}

public partial class IfcMaterialConstituent
   : IfcMaterialDefinition
{
    public static IfcMaterialConstituent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALCONSTITUENT"u8;
    public const uint ENTITY_CODE = 1589642532;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMaterial> Material = new("Material", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Fraction = new("Fraction", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Category = new("Category", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Material, Fraction, Category ];
}

public partial class IfcMaterialConstituentSet
   : IfcMaterialDefinition
{
    public static IfcMaterialConstituentSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALCONSTITUENTSET"u8;
    public const uint ENTITY_CODE = 3058901612;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMaterialConstituent> MaterialConstituents = new("MaterialConstituents", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, MaterialConstituents ];
}

public partial class IfcMaterialDefinition
   : EntityBaseClass, IfcMaterialSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcMaterialDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALDEFINITION"u8;
    public const uint ENTITY_CODE = 2717279615;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcMaterialDefinitionRepresentation
   : IfcProductRepresentation
{
    public static IfcMaterialDefinitionRepresentation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALDEFINITIONREPRESENTATION"u8;
    public const uint ENTITY_CODE = 3831637234;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> RepresentedMaterial = new("RepresentedMaterial", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Representations, RepresentedMaterial ];
}

public partial class IfcMaterialLayer
   : IfcMaterialDefinition
{
    public static IfcMaterialLayer Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLAYER"u8;
    public const uint ENTITY_CODE = 3348622987;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> Material = new("Material", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> LayerThickness = new("LayerThickness", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> IsVentilated = new("IsVentilated", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Category = new("Category", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> Priority = new("Priority", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, LayerThickness, IsVentilated, Name, Description, Category, Priority ];
}

public partial class IfcMaterialLayerSet
   : IfcMaterialDefinition
{
    public static IfcMaterialLayerSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLAYERSET"u8;
    public const uint ENTITY_CODE = 104809689;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialLayer> MaterialLayers = new("MaterialLayers", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> LayerSetName = new("LayerSetName", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ MaterialLayers, LayerSetName, Description ];
}

public partial class IfcMaterialLayerSetUsage
   : IfcMaterialUsageDefinition
{
    public static IfcMaterialLayerSetUsage Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLAYERSETUSAGE"u8;
    public const uint ENTITY_CODE = 1310956908;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialLayerSet> ForLayerSet = new("ForLayerSet", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLayerSetDirectionEnum> LayerSetDirection = new("LayerSetDirection", 1, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDirectionSenseEnum> DirectionSense = new("DirectionSense", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLengthMeasure> OffsetFromReferenceLine = new("OffsetFromReferenceLine", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ReferenceExtent = new("ReferenceExtent", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ForLayerSet, LayerSetDirection, DirectionSense, OffsetFromReferenceLine, ReferenceExtent ];
}

public partial class IfcMaterialLayerWithOffsets
   : IfcMaterialLayer
{
    public static IfcMaterialLayerWithOffsets Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLAYERWITHOFFSETS"u8;
    public const uint ENTITY_CODE = 2532930601;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLayerSetDirectionEnum> OffsetDirection = new("OffsetDirection", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLengthMeasure> OffsetValues = new("OffsetValues", 8, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Material, LayerThickness, IsVentilated, Name, Description, Category, Priority, OffsetDirection, OffsetValues ];
}

public partial class IfcMaterialList
   : EntityBaseClass, IfcMaterialSelect
{
    public static IfcMaterialList Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLIST"u8;
    public const uint ENTITY_CODE = 2456039154;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> Materials = new("Materials", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Materials ];
}

public partial class IfcMaterialProfile
   : IfcMaterialDefinition
{
    public static IfcMaterialProfile Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROFILE"u8;
    public const uint ENTITY_CODE = 72445323;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMaterial> Material = new("Material", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProfileDef> Profile = new("Profile", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcInteger> Priority = new("Priority", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Category = new("Category", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Material, Profile, Priority, Category ];
}

public partial class IfcMaterialProfileSet
   : IfcMaterialDefinition
{
    public static IfcMaterialProfileSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROFILESET"u8;
    public const uint ENTITY_CODE = 2657384921;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMaterialProfile> MaterialProfiles = new("MaterialProfiles", 2, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcCompositeProfileDef> CompositeProfile = new("CompositeProfile", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, MaterialProfiles, CompositeProfile ];
}

public partial class IfcMaterialProfileSetUsage
   : IfcMaterialUsageDefinition
{
    public static IfcMaterialProfileSetUsage Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROFILESETUSAGE"u8;
    public const uint ENTITY_CODE = 388889708;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialProfileSet> ForProfileSet = new("ForProfileSet", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCardinalPointReference> CardinalPoint = new("CardinalPoint", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ReferenceExtent = new("ReferenceExtent", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ForProfileSet, CardinalPoint, ReferenceExtent ];
}

public partial class IfcMaterialProfileSetUsageTapering
   : IfcMaterialProfileSetUsage
{
    public static IfcMaterialProfileSetUsageTapering Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROFILESETUSAGETAPERING"u8;
    public const uint ENTITY_CODE = 446790374;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialProfileSet> ForProfileEndSet = new("ForProfileEndSet", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCardinalPointReference> CardinalEndPoint = new("CardinalEndPoint", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ForProfileSet, CardinalPoint, ReferenceExtent, ForProfileEndSet, CardinalEndPoint ];
}

public partial class IfcMaterialProfileWithOffsets
   : IfcMaterialProfile
{
    public static IfcMaterialProfileWithOffsets Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROFILEWITHOFFSETS"u8;
    public const uint ENTITY_CODE = 393234729;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> OffsetValues = new("OffsetValues", 6, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, Material, Profile, Priority, Category, OffsetValues ];
}

public partial class IfcMaterialProperties
   : IfcExtendedProperties
{
    public static IfcMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 195900019;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialDefinition> Material = new("Material", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Properties, Material ];
}

public partial class IfcMaterialRelationship
   : IfcResourceLevelRelationship
{
    public static IfcMaterialRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1495515316;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> RelatingMaterial = new("RelatingMaterial", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcMaterial> RelatedMaterials = new("RelatedMaterials", 3, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> MaterialExpression = new("MaterialExpression", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingMaterial, RelatedMaterials, MaterialExpression ];
}

public partial class IfcMaterialUsageDefinition
   : EntityBaseClass, IfcMaterialSelect
{
    public static IfcMaterialUsageDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALUSAGEDEFINITION"u8;
    public const uint ENTITY_CODE = 3705827700;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcMeasureWithUnit
   : EntityBaseClass, IfcAppliedValueSelect, IfcMetricValueSelect
{
    public static IfcMeasureWithUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMEASUREWITHUNIT"u8;
    public const uint ENTITY_CODE = 3172435307;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> ValueComponent = new("ValueComponent", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcUnit> UnitComponent = new("UnitComponent", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ ValueComponent, UnitComponent ];
}

public partial class IfcMechanicalFastener
   : IfcElementComponent
{
    public static IfcMechanicalFastener Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALFASTENER"u8;
    public const uint ENTITY_CODE = 747847214;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalLength = new("NominalLength", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMechanicalFastenerTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, NominalDiameter, NominalLength, PredefinedType ];
}

public partial class IfcMechanicalFastenerType
   : IfcElementComponentType
{
    public static IfcMechanicalFastenerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALFASTENERTYPE"u8;
    public const uint ENTITY_CODE = 1495427214;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMechanicalFastenerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalLength = new("NominalLength", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, NominalDiameter, NominalLength ];
}

public partial class IfcMedicalDevice
   : IfcFlowTerminal
{
    public static IfcMedicalDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMEDICALDEVICE"u8;
    public const uint ENTITY_CODE = 1859277080;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMedicalDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcMedicalDeviceType
   : IfcFlowTerminalType
{
    public static IfcMedicalDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMEDICALDEVICETYPE"u8;
    public const uint ENTITY_CODE = 3485915216;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMedicalDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcMember
   : IfcBuiltElement
{
    public static IfcMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMEMBER"u8;
    public const uint ENTITY_CODE = 1985401597;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMemberTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcMemberType
   : IfcBuiltElementType
{
    public static IfcMemberType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMEMBERTYPE"u8;
    public const uint ENTITY_CODE = 370847317;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMemberTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcMetric
   : IfcConstraint
{
    public static IfcMetric Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMETRIC"u8;
    public const uint ENTITY_CODE = 3079980003;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBenchmarkEnum> Benchmark = new("Benchmark", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> ValueSource = new("ValueSource", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMetricValueSelect> DataValue = new("DataValue", 9, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcReference> ReferencePath = new("ReferencePath", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, ConstraintGrade, ConstraintSource, CreatingActor, CreationTime, UserDefinedGrade, Benchmark, ValueSource, DataValue, ReferencePath ];
}

public partial class IfcMirroredProfileDef
   : IfcDerivedProfileDef
{
    public static IfcMirroredProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMIRROREDPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2831609117;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, ParentProfile, Operator, Label ];
}

public partial class IfcMobileTelecommunicationsAppliance
   : IfcFlowTerminal
{
    public static IfcMobileTelecommunicationsAppliance Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOBILETELECOMMUNICATIONSAPPLIANCE"u8;
    public const uint ENTITY_CODE = 3973920307;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMobileTelecommunicationsApplianceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcMobileTelecommunicationsApplianceType
   : IfcFlowTerminalType
{
    public static IfcMobileTelecommunicationsApplianceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOBILETELECOMMUNICATIONSAPPLIANCETYPE"u8;
    public const uint ENTITY_CODE = 4070928011;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMobileTelecommunicationsApplianceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcMonetaryUnit
   : EntityBaseClass, IfcUnit
{
    public static IfcMonetaryUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMONETARYUNIT"u8;
    public const uint ENTITY_CODE = 4053228418;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Currency = new("Currency", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Currency ];
}

public partial class IfcMooringDevice
   : IfcBuiltElement
{
    public static IfcMooringDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOORINGDEVICE"u8;
    public const uint ENTITY_CODE = 1832483912;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMooringDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcMooringDeviceType
   : IfcBuiltElementType
{
    public static IfcMooringDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOORINGDEVICETYPE"u8;
    public const uint ENTITY_CODE = 3623968288;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMooringDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcMotorConnection
   : IfcEnergyConversionDevice
{
    public static IfcMotorConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOTORCONNECTION"u8;
    public const uint ENTITY_CODE = 389971100;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMotorConnectionTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcMotorConnectionType
   : IfcEnergyConversionDeviceType
{
    public static IfcMotorConnectionType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOTORCONNECTIONTYPE"u8;
    public const uint ENTITY_CODE = 1632314996;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMotorConnectionTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcNamedUnit
   : EntityBaseClass, IfcUnit
{
    public static IfcNamedUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCNAMEDUNIT"u8;
    public const uint ENTITY_CODE = 1984880438;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDimensionalExponents> Dimensions = new("Dimensions", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcUnitEnum> UnitType = new("UnitType", 1, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Dimensions, UnitType ];
}

public partial class IfcNavigationElement
   : IfcBuiltElement
{
    public static IfcNavigationElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCNAVIGATIONELEMENT"u8;
    public const uint ENTITY_CODE = 3767045443;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNavigationElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcNavigationElementType
   : IfcBuiltElementType
{
    public static IfcNavigationElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCNAVIGATIONELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 2501193083;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNavigationElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcObject
   : IfcObjectDefinition
{
    public static IfcObject Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOBJECT"u8;
    public const uint ENTITY_CODE = 670475612;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ObjectType = new("ObjectType", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcObjectDefinition
   : IfcRoot, IfcDefinitionSelect
{
    public static IfcObjectDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOBJECTDEFINITION"u8;
    public const uint ENTITY_CODE = 2119645157;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcObjective
   : IfcConstraint
{
    public static IfcObjective Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOBJECTIVE"u8;
    public const uint ENTITY_CODE = 3511015418;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstraint> BenchmarkValues = new("BenchmarkValues", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLogicalOperatorEnum> LogicalAggregator = new("LogicalAggregator", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcObjectiveEnum> ObjectiveQualifier = new("ObjectiveQualifier", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedQualifier = new("UserDefinedQualifier", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, ConstraintGrade, ConstraintSource, CreatingActor, CreationTime, UserDefinedGrade, BenchmarkValues, LogicalAggregator, ObjectiveQualifier, UserDefinedQualifier ];
}

public partial class IfcObjectPlacement
   : EntityBaseClass
{
    public static IfcObjectPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOBJECTPLACEMENT"u8;
    public const uint ENTITY_CODE = 3325497275;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectPlacement> PlacementRelTo = new("PlacementRelTo", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ PlacementRelTo ];
}

public partial class IfcOccupant
   : IfcActor
{
    public static IfcOccupant Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOCCUPANT"u8;
    public const uint ENTITY_CODE = 4166916084;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOccupantTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TheActor, PredefinedType ];
}

public partial class IfcOffsetCurve
   : IfcCurve
{
    public static IfcOffsetCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOFFSETCURVE"u8;
    public const uint ENTITY_CODE = 350476239;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> BasisCurve = new("BasisCurve", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve ];
}

public partial class IfcOffsetCurve2D
   : IfcOffsetCurve
{
    public static IfcOffsetCurve2D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOFFSETCURVE2D"u8;
    public const uint ENTITY_CODE = 542883257;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> Distance = new("Distance", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> SelfIntersect = new("SelfIntersect", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, Distance, SelfIntersect ];
}

public partial class IfcOffsetCurve3D
   : IfcOffsetCurve
{
    public static IfcOffsetCurve3D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOFFSETCURVE3D"u8;
    public const uint ENTITY_CODE = 2052721872;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> Distance = new("Distance", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> SelfIntersect = new("SelfIntersect", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDirection> RefDirection = new("RefDirection", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, Distance, SelfIntersect, RefDirection ];
}

public partial class IfcOffsetCurveByDistances
   : IfcOffsetCurve
{
    public static IfcOffsetCurveByDistances Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOFFSETCURVEBYDISTANCES"u8;
    public const uint ENTITY_CODE = 3033274150;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPointByDistanceExpression> OffsetValues = new("OffsetValues", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Tag = new("Tag", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, OffsetValues, Tag ];
}

public partial class IfcOpenCrossProfileDef
   : IfcProfileDef
{
    public static IfcOpenCrossProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOPENCROSSPROFILEDEF"u8;
    public const uint ENTITY_CODE = 4049949609;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> HorizontalWidths = new("HorizontalWidths", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> Widths = new("Widths", 3, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> Slopes = new("Slopes", 4, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> Tags = new("Tags", 5, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcCartesianPoint> OffsetPoint = new("OffsetPoint", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, HorizontalWidths, Widths, Slopes, Tags, OffsetPoint ];
}

public partial class IfcOpeningElement
   : IfcFeatureElementSubtraction
{
    public static IfcOpeningElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOPENINGELEMENT"u8;
    public const uint ENTITY_CODE = 1554121831;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOpeningElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcOpenShell
   : IfcConnectedFaceSet, IfcShell
{
    public static IfcOpenShell Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOPENSHELL"u8;
    public const uint ENTITY_CODE = 1398010391;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ CfsFaces ];
}

public partial class IfcOrganization
   : EntityBaseClass, IfcActorSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcOrganization Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCORGANIZATION"u8;
    public const uint ENTITY_CODE = 321185184;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorRole> Roles = new("Roles", 3, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcAddress> Addresses = new("Addresses", 4, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Identification, Name, Description, Roles, Addresses ];
}

public partial class IfcOrganizationRelationship
   : IfcResourceLevelRelationship
{
    public static IfcOrganizationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCORGANIZATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1147128302;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOrganization> RelatingOrganization = new("RelatingOrganization", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcOrganization> RelatedOrganizations = new("RelatedOrganizations", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingOrganization, RelatedOrganizations ];
}

public partial class IfcOrientedEdge
   : IfcEdge
{
    public static IfcOrientedEdge Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCORIENTEDEDGE"u8;
    public const uint ENTITY_CODE = 381139790;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEdge> EdgeElement = new("EdgeElement", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> Orientation = new("Orientation", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ EdgeStart, EdgeEnd, EdgeElement, Orientation ];
}

public partial class IfcOuterBoundaryCurve
   : IfcBoundaryCurve
{
    public static IfcOuterBoundaryCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOUTERBOUNDARYCURVE"u8;
    public const uint ENTITY_CODE = 814001115;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect ];
}

public partial class IfcOutlet
   : IfcFlowTerminal
{
    public static IfcOutlet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOUTLET"u8;
    public const uint ENTITY_CODE = 1448912822;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOutletTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcOutletType
   : IfcFlowTerminalType
{
    public static IfcOutletType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOUTLETTYPE"u8;
    public const uint ENTITY_CODE = 2310266054;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOutletTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcOwnerHistory
   : EntityBaseClass
{
    public static IfcOwnerHistory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOWNERHISTORY"u8;
    public const uint ENTITY_CODE = 520332314;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPersonAndOrganization> OwningUser = new("OwningUser", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcApplication> OwningApplication = new("OwningApplication", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStateEnum> State = new("State", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcChangeActionEnum> ChangeAction = new("ChangeAction", 3, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcTimeStamp> LastModifiedDate = new("LastModifiedDate", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPersonAndOrganization> LastModifyingUser = new("LastModifyingUser", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcApplication> LastModifyingApplication = new("LastModifyingApplication", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTimeStamp> CreationDate = new("CreationDate", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ OwningUser, OwningApplication, State, ChangeAction, LastModifiedDate, LastModifyingUser, LastModifyingApplication, CreationDate ];
}

public partial class IfcParameterizedProfileDef
   : IfcProfileDef
{
    public static IfcParameterizedProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPARAMETERIZEDPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2511775720;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement2D> Position = new("Position", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position ];
}

public partial class IfcPath
   : IfcTopologicalRepresentationItem
{
    public static IfcPath Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPATH"u8;
    public const uint ENTITY_CODE = 1414431256;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOrientedEdge> EdgeList = new("EdgeList", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ EdgeList ];
}

public partial class IfcPavement
   : IfcBuiltElement
{
    public static IfcPavement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPAVEMENT"u8;
    public const uint ENTITY_CODE = 2505301345;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPavementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcPavementType
   : IfcBuiltElementType
{
    public static IfcPavementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPAVEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1065775273;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPavementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcPcurve
   : IfcCurve, IfcCurveOnSurface
{
    public static IfcPcurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPCURVE"u8;
    public const uint ENTITY_CODE = 2486660828;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> BasisSurface = new("BasisSurface", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCurve> ReferenceCurve = new("ReferenceCurve", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ BasisSurface, ReferenceCurve ];
}

public partial class IfcPerformanceHistory
   : IfcControl
{
    public static IfcPerformanceHistory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERFORMANCEHISTORY"u8;
    public const uint ENTITY_CODE = 164555693;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LifeCyclePhase = new("LifeCyclePhase", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPerformanceHistoryTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LifeCyclePhase, PredefinedType ];
}

public partial class IfcPermeableCoveringProperties
   : IfcPreDefinedPropertySet
{
    public static IfcPermeableCoveringProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERMEABLECOVERINGPROPERTIES"u8;
    public const uint ENTITY_CODE = 691971400;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPermeableCoveringOperationEnum> OperationType = new("OperationType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcWindowPanelPositionEnum> PanelPosition = new("PanelPosition", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FrameDepth = new("FrameDepth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FrameThickness = new("FrameThickness", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, OperationType, PanelPosition, FrameDepth, FrameThickness, ShapeAspectStyle ];
}

public partial class IfcPermit
   : IfcControl
{
    public static IfcPermit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERMIT"u8;
    public const uint ENTITY_CODE = 2074085164;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPermitTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, PredefinedType, Status, LongDescription ];
}

public partial class IfcPerson
   : EntityBaseClass, IfcActorSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcPerson Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERSON"u8;
    public const uint ENTITY_CODE = 1697060002;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> FamilyName = new("FamilyName", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> GivenName = new("GivenName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> MiddleNames = new("MiddleNames", 3, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> PrefixTitles = new("PrefixTitles", 4, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> SuffixTitles = new("SuffixTitles", 5, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcActorRole> Roles = new("Roles", 6, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcAddress> Addresses = new("Addresses", 7, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Identification, FamilyName, GivenName, MiddleNames, PrefixTitles, SuffixTitles, Roles, Addresses ];
}

public partial class IfcPersonAndOrganization
   : EntityBaseClass, IfcActorSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcPersonAndOrganization Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERSONANDORGANIZATION"u8;
    public const uint ENTITY_CODE = 1637477396;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPerson> ThePerson = new("ThePerson", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcOrganization> TheOrganization = new("TheOrganization", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcActorRole> Roles = new("Roles", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ ThePerson, TheOrganization, Roles ];
}

public partial class IfcPhysicalComplexQuantity
   : IfcPhysicalQuantity
{
    public static IfcPhysicalComplexQuantity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPHYSICALCOMPLEXQUANTITY"u8;
    public const uint ENTITY_CODE = 3770200107;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPhysicalQuantity> HasQuantities = new("HasQuantities", 2, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Discrimination = new("Discrimination", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Quality = new("Quality", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Usage = new("Usage", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, HasQuantities, Discrimination, Quality, Usage ];
}

public partial class IfcPhysicalQuantity
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcPhysicalQuantity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPHYSICALQUANTITY"u8;
    public const uint ENTITY_CODE = 1923906739;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description ];
}

public partial class IfcPhysicalSimpleQuantity
   : IfcPhysicalQuantity
{
    public static IfcPhysicalSimpleQuantity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPHYSICALSIMPLEQUANTITY"u8;
    public const uint ENTITY_CODE = 611700029;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNamedUnit> Unit = new("Unit", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit ];
}

public partial class IfcPile
   : IfcDeepFoundation
{
    public static IfcPile Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPILE"u8;
    public const uint ENTITY_CODE = 149965647;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPileTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPileConstructionEnum> ConstructionType = new("ConstructionType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType, ConstructionType ];
}

public partial class IfcPileType
   : IfcDeepFoundationType
{
    public static IfcPileType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPILETYPE"u8;
    public const uint ENTITY_CODE = 2571255223;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPileTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcPipeFitting
   : IfcFlowFitting
{
    public static IfcPipeFitting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPIPEFITTING"u8;
    public const uint ENTITY_CODE = 2595033550;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPipeFittingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcPipeFittingType
   : IfcFlowFittingType
{
    public static IfcPipeFittingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPIPEFITTINGTYPE"u8;
    public const uint ENTITY_CODE = 3677478062;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPipeFittingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcPipeSegment
   : IfcFlowSegment
{
    public static IfcPipeSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPIPESEGMENT"u8;
    public const uint ENTITY_CODE = 3444543964;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPipeSegmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcPipeSegmentType
   : IfcFlowSegmentType
{
    public static IfcPipeSegmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPIPESEGMENTTYPE"u8;
    public const uint ENTITY_CODE = 799408564;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPipeSegmentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcPixelTexture
   : IfcSurfaceTexture
{
    public static IfcPixelTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPIXELTEXTURE"u8;
    public const uint ENTITY_CODE = 118615764;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> Width = new("Width", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> Height = new("Height", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> ColourComponents = new("ColourComponents", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBinary> Pixel = new("Pixel", 8, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, Mode, TextureTransform, Parameter, Width, Height, ColourComponents, Pixel ];
}

public partial class IfcPlacement
   : IfcGeometricRepresentationItem
{
    public static IfcPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLACEMENT"u8;
    public const uint ENTITY_CODE = 184181550;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPoint> Location = new("Location", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location ];
}

public partial class IfcPlanarBox
   : IfcPlanarExtent
{
    public static IfcPlanarBox Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLANARBOX"u8;
    public const uint ENTITY_CODE = 2625056540;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement> Placement = new("Placement", 2, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ SizeInX, SizeInY, Placement ];
}

public partial class IfcPlanarExtent
   : IfcGeometricRepresentationItem
{
    public static IfcPlanarExtent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLANAREXTENT"u8;
    public const uint ENTITY_CODE = 3671944755;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> SizeInX = new("SizeInX", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> SizeInY = new("SizeInY", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ SizeInX, SizeInY ];
}

public partial class IfcPlane
   : IfcElementarySurface
{
    public static IfcPlane Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLANE"u8;
    public const uint ENTITY_CODE = 4154753479;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Position ];
}

public partial class IfcPlate
   : IfcBuiltElement
{
    public static IfcPlate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLATE"u8;
    public const uint ENTITY_CODE = 3954996169;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlateTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcPlateType
   : IfcBuiltElementType
{
    public static IfcPlateType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLATETYPE"u8;
    public const uint ENTITY_CODE = 3012845089;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlateTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcPoint
   : IfcGeometricRepresentationItem, IfcGeometricSetSelect, IfcPointOrVertexPoint
{
    public static IfcPoint Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOINT"u8;
    public const uint ENTITY_CODE = 3799561623;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcPointByDistanceExpression
   : IfcPoint
{
    public static IfcPointByDistanceExpression Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOINTBYDISTANCEEXPRESSION"u8;
    public const uint ENTITY_CODE = 3184423839;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurveMeasureSelect> DistanceAlong = new("DistanceAlong", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcLengthMeasure> OffsetLateral = new("OffsetLateral", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> OffsetVertical = new("OffsetVertical", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> OffsetLongitudinal = new("OffsetLongitudinal", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCurve> BasisCurve = new("BasisCurve", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ DistanceAlong, OffsetLateral, OffsetVertical, OffsetLongitudinal, BasisCurve ];
}

public partial class IfcPointOnCurve
   : IfcPoint
{
    public static IfcPointOnCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOINTONCURVE"u8;
    public const uint ENTITY_CODE = 154430901;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> BasisCurve = new("BasisCurve", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcParameterValue> PointParameter = new("PointParameter", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, PointParameter ];
}

public partial class IfcPointOnSurface
   : IfcPoint
{
    public static IfcPointOnSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOINTONSURFACE"u8;
    public const uint ENTITY_CODE = 3955153569;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> BasisSurface = new("BasisSurface", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcParameterValue> PointParameterU = new("PointParameterU", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> PointParameterV = new("PointParameterV", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisSurface, PointParameterU, PointParameterV ];
}

public partial class IfcPolygonalBoundedHalfSpace
   : IfcHalfSpaceSolid
{
    public static IfcPolygonalBoundedHalfSpace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOLYGONALBOUNDEDHALFSPACE"u8;
    public const uint ENTITY_CODE = 797080096;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement3D> Position = new("Position", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoundedCurve> PolygonalBoundary = new("PolygonalBoundary", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ BaseSurface, AgreementFlag, Position, PolygonalBoundary ];
}

public partial class IfcPolygonalFaceSet
   : IfcTessellatedFaceSet
{
    public static IfcPolygonalFaceSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOLYGONALFACESET"u8;
    public const uint ENTITY_CODE = 605400089;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIndexedPolygonalFace> Faces = new("Faces", 2, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPositiveInteger> PnIndex = new("PnIndex", 3, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Coordinates, Closed, Faces, PnIndex ];
}

public partial class IfcPolyline
   : IfcBoundedCurve
{
    public static IfcPolyline Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOLYLINE"u8;
    public const uint ENTITY_CODE = 1622455735;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> Points = new("Points", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Points ];
}

public partial class IfcPolyLoop
   : IfcLoop
{
    public static IfcPolyLoop Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOLYLOOP"u8;
    public const uint ENTITY_CODE = 1197927195;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> Polygon = new("Polygon", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Polygon ];
}

public partial class IfcPolynomialCurve
   : IfcCurve
{
    public static IfcPolynomialCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOLYNOMIALCURVE"u8;
    public const uint ENTITY_CODE = 2801833992;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlacement> Position = new("Position", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcReal> CoefficientsX = new("CoefficientsX", 1, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcReal> CoefficientsY = new("CoefficientsY", 2, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcReal> CoefficientsZ = new("CoefficientsZ", 3, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Position, CoefficientsX, CoefficientsY, CoefficientsZ ];
}

public partial class IfcPort
   : IfcProduct
{
    public static IfcPort Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPORT"u8;
    public const uint ENTITY_CODE = 773015496;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcPositioningElement
   : IfcProduct
{
    public static IfcPositioningElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOSITIONINGELEMENT"u8;
    public const uint ENTITY_CODE = 2209218372;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcPostalAddress
   : IfcAddress
{
    public static IfcPostalAddress Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPOSTALADDRESS"u8;
    public const uint ENTITY_CODE = 2167844468;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> InternalLocation = new("InternalLocation", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> AddressLines = new("AddressLines", 4, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> PostalBox = new("PostalBox", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Town = new("Town", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Region = new("Region", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> PostalCode = new("PostalCode", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Country = new("Country", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Purpose, Description, UserDefinedPurpose, InternalLocation, AddressLines, PostalBox, Town, Region, PostalCode, Country ];
}

public partial class IfcPreDefinedColour
   : IfcPreDefinedItem, IfcColour
{
    public static IfcPreDefinedColour Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDCOLOUR"u8;
    public const uint ENTITY_CODE = 883132221;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedCurveFont
   : IfcPreDefinedItem, IfcCurveStyleFontSelect
{
    public static IfcPreDefinedCurveFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDCURVEFONT"u8;
    public const uint ENTITY_CODE = 128516385;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedItem
   : IfcPresentationItem
{
    public static IfcPreDefinedItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDITEM"u8;
    public const uint ENTITY_CODE = 827041254;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedProperties
   : IfcPropertyAbstraction
{
    public static IfcPreDefinedProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDPROPERTIES"u8;
    public const uint ENTITY_CODE = 3558034524;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcPreDefinedPropertySet
   : IfcPropertySetDefinition
{
    public static IfcPreDefinedPropertySet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDPROPERTYSET"u8;
    public const uint ENTITY_CODE = 126759560;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcPreDefinedTextFont
   : IfcPreDefinedItem, IfcTextFontSelect
{
    public static IfcPreDefinedTextFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDTEXTFONT"u8;
    public const uint ENTITY_CODE = 613620735;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPresentationItem
   : EntityBaseClass
{
    public static IfcPresentationItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRESENTATIONITEM"u8;
    public const uint ENTITY_CODE = 280110816;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcPresentationLayerAssignment
   : EntityBaseClass
{
    public static IfcPresentationLayerAssignment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRESENTATIONLAYERASSIGNMENT"u8;
    public const uint ENTITY_CODE = 1407561121;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLayeredItem> AssignedItems = new("AssignedItems", 2, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcIdentifier> Identifier = new("Identifier", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, AssignedItems, Identifier ];
}

public partial class IfcPresentationLayerWithStyle
   : IfcPresentationLayerAssignment
{
    public static IfcPresentationLayerWithStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRESENTATIONLAYERWITHSTYLE"u8;
    public const uint ENTITY_CODE = 792652293;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLogical> LayerOn = new("LayerOn", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> LayerFrozen = new("LayerFrozen", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> LayerBlocked = new("LayerBlocked", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPresentationStyle> LayerStyles = new("LayerStyles", 7, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, AssignedItems, Identifier, LayerOn, LayerFrozen, LayerBlocked, LayerStyles ];
}

public partial class IfcPresentationStyle
   : EntityBaseClass
{
    public static IfcPresentationStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRESENTATIONSTYLE"u8;
    public const uint ENTITY_CODE = 4040404728;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcProcedure
   : IfcProcess
{
    public static IfcProcedure Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROCEDURE"u8;
    public const uint ENTITY_CODE = 1774744644;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProcedureTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, PredefinedType ];
}

public partial class IfcProcedureType
   : IfcTypeProcess
{
    public static IfcProcedureType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROCEDURETYPE"u8;
    public const uint ENTITY_CODE = 1904674444;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProcedureTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ProcessType, PredefinedType ];
}

public partial class IfcProcess
   : IfcObject, IfcProcessSelect
{
    public static IfcProcess Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROCESS"u8;
    public const uint ENTITY_CODE = 1826787596;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription ];
}

public partial class IfcProduct
   : IfcObject, IfcProductSelect, IfcSpatialReferenceSelect
{
    public static IfcProduct Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRODUCT"u8;
    public const uint ENTITY_CODE = 3372775790;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectPlacement> ObjectPlacement = new("ObjectPlacement", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProductRepresentation> Representation = new("Representation", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcProductDefinitionShape
   : IfcProductRepresentation, IfcProductRepresentationSelect
{
    public static IfcProductDefinitionShape Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRODUCTDEFINITIONSHAPE"u8;
    public const uint ENTITY_CODE = 4066491472;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, Description, Representations ];
}

public partial class IfcProductRepresentation
   : EntityBaseClass
{
    public static IfcProductRepresentation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRODUCTREPRESENTATION"u8;
    public const uint ENTITY_CODE = 2978431027;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRepresentation> Representations = new("Representations", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, Representations ];
}

public partial class IfcProfileDef
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROFILEDEF"u8;
    public const uint ENTITY_CODE = 977691495;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileTypeEnum> ProfileType = new("ProfileType", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> ProfileName = new("ProfileName", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName ];
}

public partial class IfcProfileProperties
   : IfcExtendedProperties
{
    public static IfcProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 2726116117;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> ProfileDefinition = new("ProfileDefinition", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Properties, ProfileDefinition ];
}

public partial class IfcProject
   : IfcContext
{
    public static IfcProject Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECT"u8;
    public const uint ENTITY_CODE = 1439394748;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName, Phase, RepresentationContexts, UnitsInContext ];
}

public partial class IfcProjectedCRS
   : IfcCoordinateReferenceSystem
{
    public static IfcProjectedCRS Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTEDCRS"u8;
    public const uint ENTITY_CODE = 3762950369;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> MapProjection = new("MapProjection", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> MapZone = new("MapZone", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNamedUnit> MapUnit = new("MapUnit", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, GeodeticDatum, VerticalDatum, MapProjection, MapZone, MapUnit ];
}

public partial class IfcProjectionElement
   : IfcFeatureElementAddition
{
    public static IfcProjectionElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTIONELEMENT"u8;
    public const uint ENTITY_CODE = 2130597890;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProjectionElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcProjectLibrary
   : IfcContext
{
    public static IfcProjectLibrary Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTLIBRARY"u8;
    public const uint ENTITY_CODE = 3676489351;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName, Phase, RepresentationContexts, UnitsInContext ];
}

public partial class IfcProjectOrder
   : IfcControl
{
    public static IfcProjectOrder Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTORDER"u8;
    public const uint ENTITY_CODE = 567771124;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProjectOrderTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, PredefinedType, Status, LongDescription ];
}

public partial class IfcProperty
   : IfcPropertyAbstraction
{
    public static IfcProperty Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTY"u8;
    public const uint ENTITY_CODE = 3277779118;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Specification = new("Specification", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification ];
}

public partial class IfcPropertyAbstraction
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcPropertyAbstraction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYABSTRACTION"u8;
    public const uint ENTITY_CODE = 258308818;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcPropertyBoundedValue
   : IfcSimpleProperty
{
    public static IfcPropertyBoundedValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYBOUNDEDVALUE"u8;
    public const uint ENTITY_CODE = 3087662268;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> UpperBoundValue = new("UpperBoundValue", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcValue> LowerBoundValue = new("LowerBoundValue", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcValue> SetPointValue = new("SetPointValue", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification, UpperBoundValue, LowerBoundValue, Unit, SetPointValue ];
}

public partial class IfcPropertyDefinition
   : IfcRoot, IfcDefinitionSelect
{
    public static IfcPropertyDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYDEFINITION"u8;
    public const uint ENTITY_CODE = 3334093415;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcPropertyDependencyRelationship
   : IfcResourceLevelRelationship
{
    public static IfcPropertyDependencyRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYDEPENDENCYRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2230335753;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProperty> DependingProperty = new("DependingProperty", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProperty> DependantProperty = new("DependantProperty", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcText> Expression = new("Expression", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, DependingProperty, DependantProperty, Expression ];
}

public partial class IfcPropertyEnumeratedValue
   : IfcSimpleProperty
{
    public static IfcPropertyEnumeratedValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYENUMERATEDVALUE"u8;
    public const uint ENTITY_CODE = 3538377801;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> EnumerationValues = new("EnumerationValues", 2, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcPropertyEnumeration> EnumerationReference = new("EnumerationReference", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification, EnumerationValues, EnumerationReference ];
}

public partial class IfcPropertyEnumeration
   : IfcPropertyAbstraction
{
    public static IfcPropertyEnumeration Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYENUMERATION"u8;
    public const uint ENTITY_CODE = 623736673;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcValue> EnumerationValues = new("EnumerationValues", 1, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 2, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, EnumerationValues, Unit ];
}

public partial class IfcPropertyListValue
   : IfcSimpleProperty
{
    public static IfcPropertyListValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYLISTVALUE"u8;
    public const uint ENTITY_CODE = 2643420771;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> ListValues = new("ListValues", 2, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification, ListValues, Unit ];
}

public partial class IfcPropertyReferenceValue
   : IfcSimpleProperty
{
    public static IfcPropertyReferenceValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYREFERENCEVALUE"u8;
    public const uint ENTITY_CODE = 3614615320;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcText> UsageName = new("UsageName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcObjectReferenceSelect> PropertyReference = new("PropertyReference", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification, UsageName, PropertyReference ];
}

public partial class IfcPropertySet
   : IfcPropertySetDefinition
{
    public static IfcPropertySet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYSET"u8;
    public const uint ENTITY_CODE = 1978989174;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProperty> HasProperties = new("HasProperties", 4, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, HasProperties ];
}

public partial class IfcPropertySetDefinition
   : IfcPropertyDefinition, IfcPropertySetDefinitionSelect
{
    public static IfcPropertySetDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYSETDEFINITION"u8;
    public const uint ENTITY_CODE = 933111983;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcPropertySetTemplate
   : IfcPropertyTemplateDefinition
{
    public static IfcPropertySetTemplate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYSETTEMPLATE"u8;
    public const uint ENTITY_CODE = 1300894696;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPropertySetTemplateTypeEnum> TemplateType = new("TemplateType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcIdentifier> ApplicableEntity = new("ApplicableEntity", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPropertyTemplate> HasPropertyTemplates = new("HasPropertyTemplates", 6, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, TemplateType, ApplicableEntity, HasPropertyTemplates ];
}

public partial class IfcPropertySingleValue
   : IfcSimpleProperty
{
    public static IfcPropertySingleValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYSINGLEVALUE"u8;
    public const uint ENTITY_CODE = 939331015;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> NominalValue = new("NominalValue", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification, NominalValue, Unit ];
}

public partial class IfcPropertyTableValue
   : IfcSimpleProperty
{
    public static IfcPropertyTableValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYTABLEVALUE"u8;
    public const uint ENTITY_CODE = 1981908299;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> DefiningValues = new("DefiningValues", 2, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcValue> DefinedValues = new("DefinedValues", 3, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcText> Expression = new("Expression", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcUnit> DefiningUnit = new("DefiningUnit", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcUnit> DefinedUnit = new("DefinedUnit", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCurveInterpolationEnum> CurveInterpolation = new("CurveInterpolation", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Name, Specification, DefiningValues, DefinedValues, Expression, DefiningUnit, DefinedUnit, CurveInterpolation ];
}

public partial class IfcPropertyTemplate
   : IfcPropertyTemplateDefinition
{
    public static IfcPropertyTemplate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYTEMPLATE"u8;
    public const uint ENTITY_CODE = 4200139200;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcPropertyTemplateDefinition
   : IfcPropertyDefinition
{
    public static IfcPropertyTemplateDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYTEMPLATEDEFINITION"u8;
    public const uint ENTITY_CODE = 2827533537;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcProtectiveDevice
   : IfcFlowController
{
    public static IfcProtectiveDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROTECTIVEDEVICE"u8;
    public const uint ENTITY_CODE = 2782420526;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProtectiveDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcProtectiveDeviceTrippingUnit
   : IfcDistributionControlElement
{
    public static IfcProtectiveDeviceTrippingUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROTECTIVEDEVICETRIPPINGUNIT"u8;
    public const uint ENTITY_CODE = 1244377875;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProtectiveDeviceTrippingUnitTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcProtectiveDeviceTrippingUnitType
   : IfcDistributionControlElementType
{
    public static IfcProtectiveDeviceTrippingUnitType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROTECTIVEDEVICETRIPPINGUNITTYPE"u8;
    public const uint ENTITY_CODE = 4173200747;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProtectiveDeviceTrippingUnitTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcProtectiveDeviceType
   : IfcFlowControllerType
{
    public static IfcProtectiveDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROTECTIVEDEVICETYPE"u8;
    public const uint ENTITY_CODE = 3919153294;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProtectiveDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcPump
   : IfcFlowMovingDevice
{
    public static IfcPump Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPUMP"u8;
    public const uint ENTITY_CODE = 4168265165;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPumpTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcPumpType
   : IfcFlowMovingDeviceType
{
    public static IfcPumpType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPUMPTYPE"u8;
    public const uint ENTITY_CODE = 640924933;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPumpTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcQuantityArea
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityArea Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYAREA"u8;
    public const uint ENTITY_CODE = 3796205563;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAreaMeasure> AreaValue = new("AreaValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, AreaValue, Formula ];
}

public partial class IfcQuantityCount
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityCount Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYCOUNT"u8;
    public const uint ENTITY_CODE = 2932049789;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCountMeasure> CountValue = new("CountValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, CountValue, Formula ];
}

public partial class IfcQuantityLength
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityLength Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYLENGTH"u8;
    public const uint ENTITY_CODE = 27827418;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> LengthValue = new("LengthValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, LengthValue, Formula ];
}

public partial class IfcQuantityNumber
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityNumber Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYNUMBER"u8;
    public const uint ENTITY_CODE = 3645075915;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNumericMeasure> NumberValue = new("NumberValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, NumberValue, Formula ];
}

public partial class IfcQuantitySet
   : IfcPropertySetDefinition
{
    public static IfcQuantitySet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYSET"u8;
    public const uint ENTITY_CODE = 2245176038;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcQuantityTime
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYTIME"u8;
    public const uint ENTITY_CODE = 3727679831;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTimeMeasure> TimeValue = new("TimeValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, TimeValue, Formula ];
}

public partial class IfcQuantityVolume
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityVolume Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYVOLUME"u8;
    public const uint ENTITY_CODE = 973298816;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVolumeMeasure> VolumeValue = new("VolumeValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, VolumeValue, Formula ];
}

public partial class IfcQuantityWeight
   : IfcPhysicalSimpleQuantity
{
    public static IfcQuantityWeight Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCQUANTITYWEIGHT"u8;
    public const uint ENTITY_CODE = 3233304038;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMassMeasure> WeightValue = new("WeightValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Formula = new("Formula", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, WeightValue, Formula ];
}

public partial class IfcRail
   : IfcBuiltElement
{
    public static IfcRail Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAIL"u8;
    public const uint ENTITY_CODE = 2019186983;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcRailing
   : IfcBuiltElement
{
    public static IfcRailing Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAILING"u8;
    public const uint ENTITY_CODE = 3345183409;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcRailingType
   : IfcBuiltElementType
{
    public static IfcRailingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAILINGTYPE"u8;
    public const uint ENTITY_CODE = 2218968665;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRailType
   : IfcBuiltElementType
{
    public static IfcRailType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAILTYPE"u8;
    public const uint ENTITY_CODE = 3757857215;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRailway
   : IfcFacility
{
    public static IfcRailway Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAILWAY"u8;
    public const uint ENTITY_CODE = 1711612782;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailwayTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, PredefinedType ];
}

public partial class IfcRailwayPart
   : IfcFacilityPart
{
    public static IfcRailwayPart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAILWAYPART"u8;
    public const uint ENTITY_CODE = 1892473607;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailwayPartTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, UsageType, PredefinedType ];
}

public partial class IfcRamp
   : IfcBuiltElement
{
    public static IfcRamp Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMP"u8;
    public const uint ENTITY_CODE = 1952768055;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRampTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcRampFlight
   : IfcBuiltElement
{
    public static IfcRampFlight Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMPFLIGHT"u8;
    public const uint ENTITY_CODE = 2713085869;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRampFlightTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcRampFlightType
   : IfcBuiltElementType
{
    public static IfcRampFlightType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMPFLIGHTTYPE"u8;
    public const uint ENTITY_CODE = 386973029;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRampFlightTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRampType
   : IfcBuiltElementType
{
    public static IfcRampType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMPTYPE"u8;
    public const uint ENTITY_CODE = 3598766703;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRampTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRationalBSplineCurveWithKnots
   : IfcBSplineCurveWithKnots
{
    public static IfcRationalBSplineCurveWithKnots Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRATIONALBSPLINECURVEWITHKNOTS"u8;
    public const uint ENTITY_CODE = 2136405382;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> WeightsData = new("WeightsData", 8, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Degree, ControlPointsList, CurveForm, ClosedCurve, SelfIntersect, KnotMultiplicities, Knots, KnotSpec, WeightsData ];
}

public partial class IfcRationalBSplineSurfaceWithKnots
   : IfcBSplineSurfaceWithKnots
{
    public static IfcRationalBSplineSurfaceWithKnots Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRATIONALBSPLINESURFACEWITHKNOTS"u8;
    public const uint ENTITY_CODE = 2396975254;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> WeightsData = new("WeightsData", 12, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ UDegree, VDegree, ControlPointsList, SurfaceForm, UClosed, VClosed, SelfIntersect, UMultiplicities, VMultiplicities, UKnots, VKnots, KnotSpec, WeightsData ];
}

public partial class IfcRectangleHollowProfileDef
   : IfcRectangleProfileDef
{
    public static IfcRectangleHollowProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRECTANGLEHOLLOWPROFILEDEF"u8;
    public const uint ENTITY_CODE = 1283664311;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WallThickness = new("WallThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> InnerFilletRadius = new("InnerFilletRadius", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> OuterFilletRadius = new("OuterFilletRadius", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, XDim, YDim, WallThickness, InnerFilletRadius, OuterFilletRadius ];
}

public partial class IfcRectangleProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcRectangleProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRECTANGLEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2503913696;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> XDim = new("XDim", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> YDim = new("YDim", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, XDim, YDim ];
}

public partial class IfcRectangularPyramid
   : IfcCsgPrimitive3D
{
    public static IfcRectangularPyramid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRECTANGULARPYRAMID"u8;
    public const uint ENTITY_CODE = 954763055;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> XLength = new("XLength", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> YLength = new("YLength", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Height = new("Height", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, XLength, YLength, Height ];
}

public partial class IfcRectangularTrimmedSurface
   : IfcBoundedSurface
{
    public static IfcRectangularTrimmedSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRECTANGULARTRIMMEDSURFACE"u8;
    public const uint ENTITY_CODE = 2893748188;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> BasisSurface = new("BasisSurface", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcParameterValue> U1 = new("U1", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> V1 = new("V1", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> U2 = new("U2", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> V2 = new("V2", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBoolean> Usense = new("Usense", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBoolean> Vsense = new("Vsense", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisSurface, U1, V1, U2, V2, Usense, Vsense ];
}

public partial class IfcRecurrencePattern
   : EntityBaseClass
{
    public static IfcRecurrencePattern Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRECURRENCEPATTERN"u8;
    public const uint ENTITY_CODE = 1431445093;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRecurrenceTypeEnum> RecurrenceType = new("RecurrenceType", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDayInMonthNumber> DayComponent = new("DayComponent", 1, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcDayInWeekNumber> WeekdayComponent = new("WeekdayComponent", 2, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcMonthInYearNumber> MonthComponent = new("MonthComponent", 3, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcInteger> Position = new("Position", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> Interval = new("Interval", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> Occurrences = new("Occurrences", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimePeriod> TimePeriods = new("TimePeriods", 7, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ RecurrenceType, DayComponent, WeekdayComponent, MonthComponent, Position, Interval, Occurrences, TimePeriods ];
}

public partial class IfcReference
   : EntityBaseClass, IfcAppliedValueSelect, IfcMetricValueSelect
{
    public static IfcReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREFERENCE"u8;
    public const uint ENTITY_CODE = 1936048460;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> TypeIdentifier = new("TypeIdentifier", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> AttributeIdentifier = new("AttributeIdentifier", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> InstanceName = new("InstanceName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> ListPositions = new("ListPositions", 3, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcReference> InnerReference = new("InnerReference", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ TypeIdentifier, AttributeIdentifier, InstanceName, ListPositions, InnerReference ];
}

public partial class IfcReferent
   : IfcPositioningElement
{
    public static IfcReferent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREFERENT"u8;
    public const uint ENTITY_CODE = 1564746148;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReferentTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType ];
}

public partial class IfcRegularTimeSeries
   : IfcTimeSeries
{
    public static IfcRegularTimeSeries Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREGULARTIMESERIES"u8;
    public const uint ENTITY_CODE = 2717202733;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTimeMeasure> TimeStep = new("TimeStep", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeSeriesValue> Values = new("Values", 9, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, StartTime, EndTime, TimeSeriesDataType, DataOrigin, UserDefinedDataOrigin, Unit, TimeStep, Values ];
}

public partial class IfcReinforcedSoil
   : IfcEarthworksElement
{
    public static IfcReinforcedSoil Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCEDSOIL"u8;
    public const uint ENTITY_CODE = 784685403;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReinforcedSoilTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcReinforcementBarProperties
   : IfcPreDefinedProperties
{
    public static IfcReinforcementBarProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCEMENTBARPROPERTIES"u8;
    public const uint ENTITY_CODE = 208435744;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAreaMeasure> TotalCrossSectionArea = new("TotalCrossSectionArea", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> SteelGrade = new("SteelGrade", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReinforcingBarSurfaceEnum> BarSurface = new("BarSurface", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLengthMeasure> EffectiveDepth = new("EffectiveDepth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalBarDiameter = new("NominalBarDiameter", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCountMeasure> BarCount = new("BarCount", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ TotalCrossSectionArea, SteelGrade, BarSurface, EffectiveDepth, NominalBarDiameter, BarCount ];
}

public partial class IfcReinforcementDefinitionProperties
   : IfcPreDefinedPropertySet
{
    public static IfcReinforcementDefinitionProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCEMENTDEFINITIONPROPERTIES"u8;
    public const uint ENTITY_CODE = 1501559820;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> DefinitionType = new("DefinitionType", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSectionReinforcementProperties> ReinforcementSectionDefinitions = new("ReinforcementSectionDefinitions", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, DefinitionType, ReinforcementSectionDefinitions ];
}

public partial class IfcReinforcingBar
   : IfcReinforcingElement
{
    public static IfcReinforcingBar Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGBAR"u8;
    public const uint ENTITY_CODE = 1424876924;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> CrossSectionArea = new("CrossSectionArea", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BarLength = new("BarLength", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReinforcingBarTypeEnum> PredefinedType = new("PredefinedType", 12, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcReinforcingBarSurfaceEnum> BarSurface = new("BarSurface", 13, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, NominalDiameter, CrossSectionArea, BarLength, PredefinedType, BarSurface ];
}

public partial class IfcReinforcingBarType
   : IfcReinforcingElementType
{
    public static IfcReinforcingBarType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGBARTYPE"u8;
    public const uint ENTITY_CODE = 333728212;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReinforcingBarTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> CrossSectionArea = new("CrossSectionArea", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BarLength = new("BarLength", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReinforcingBarSurfaceEnum> BarSurface = new("BarSurface", 13, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> BendingShapeCode = new("BendingShapeCode", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBendingParameterSelect> BendingParameters = new("BendingParameters", 15, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, NominalDiameter, CrossSectionArea, BarLength, BarSurface, BendingShapeCode, BendingParameters ];
}

public partial class IfcReinforcingElement
   : IfcElementComponent
{
    public static IfcReinforcingElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGELEMENT"u8;
    public const uint ENTITY_CODE = 1403002469;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> SteelGrade = new("SteelGrade", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade ];
}

public partial class IfcReinforcingElementType
   : IfcElementComponentType
{
    public static IfcReinforcingElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 2074665645;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcReinforcingMesh
   : IfcReinforcingElement
{
    public static IfcReinforcingMesh Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGMESH"u8;
    public const uint ENTITY_CODE = 3849051190;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MeshLength = new("MeshLength", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MeshWidth = new("MeshWidth", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LongitudinalBarNominalDiameter = new("LongitudinalBarNominalDiameter", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TransverseBarNominalDiameter = new("TransverseBarNominalDiameter", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> LongitudinalBarCrossSectionArea = new("LongitudinalBarCrossSectionArea", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> TransverseBarCrossSectionArea = new("TransverseBarCrossSectionArea", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LongitudinalBarSpacing = new("LongitudinalBarSpacing", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TransverseBarSpacing = new("TransverseBarSpacing", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReinforcingMeshTypeEnum> PredefinedType = new("PredefinedType", 17, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, MeshLength, MeshWidth, LongitudinalBarNominalDiameter, TransverseBarNominalDiameter, LongitudinalBarCrossSectionArea, TransverseBarCrossSectionArea, LongitudinalBarSpacing, TransverseBarSpacing, PredefinedType ];
}

public partial class IfcReinforcingMeshType
   : IfcReinforcingElementType
{
    public static IfcReinforcingMeshType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGMESHTYPE"u8;
    public const uint ENTITY_CODE = 3449936198;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReinforcingMeshTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MeshLength = new("MeshLength", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MeshWidth = new("MeshWidth", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LongitudinalBarNominalDiameter = new("LongitudinalBarNominalDiameter", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TransverseBarNominalDiameter = new("TransverseBarNominalDiameter", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> LongitudinalBarCrossSectionArea = new("LongitudinalBarCrossSectionArea", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> TransverseBarCrossSectionArea = new("TransverseBarCrossSectionArea", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LongitudinalBarSpacing = new("LongitudinalBarSpacing", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TransverseBarSpacing = new("TransverseBarSpacing", 17, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> BendingShapeCode = new("BendingShapeCode", 18, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBendingParameterSelect> BendingParameters = new("BendingParameters", 19, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, MeshLength, MeshWidth, LongitudinalBarNominalDiameter, TransverseBarNominalDiameter, LongitudinalBarCrossSectionArea, TransverseBarCrossSectionArea, LongitudinalBarSpacing, TransverseBarSpacing, BendingShapeCode, BendingParameters ];
}

public partial class IfcRelAdheresToElement
   : IfcRelDecomposes
{
    public static IfcRelAdheresToElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELADHERESTOELEMENT"u8;
    public const uint ENTITY_CODE = 1976144953;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElement> RelatingElement = new("RelatingElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSurfaceFeature> RelatedSurfaceFeatures = new("RelatedSurfaceFeatures", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingElement, RelatedSurfaceFeatures ];
}

public partial class IfcRelAggregates
   : IfcRelDecomposes
{
    public static IfcRelAggregates Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELAGGREGATES"u8;
    public const uint ENTITY_CODE = 2084011922;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectDefinition> RelatingObject = new("RelatingObject", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcObjectDefinition> RelatedObjects = new("RelatedObjects", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingObject, RelatedObjects ];
}

public partial class IfcRelAssigns
   : IfcRelationship
{
    public static IfcRelAssigns Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNS"u8;
    public const uint ENTITY_CODE = 1077973036;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectDefinition> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcObjectTypeEnum> RelatedObjectsType = new("RelatedObjectsType", 5, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType ];
}

public partial class IfcRelAssignsToActor
   : IfcRelAssigns
{
    public static IfcRelAssignsToActor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOACTOR"u8;
    public const uint ENTITY_CODE = 2605624762;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActor> RelatingActor = new("RelatingActor", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcActorRole> ActingRole = new("ActingRole", 7, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingActor, ActingRole ];
}

public partial class IfcRelAssignsToControl
   : IfcRelAssigns
{
    public static IfcRelAssignsToControl Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOCONTROL"u8;
    public const uint ENTITY_CODE = 4063478366;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcControl> RelatingControl = new("RelatingControl", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingControl ];
}

public partial class IfcRelAssignsToGroup
   : IfcRelAssigns
{
    public static IfcRelAssignsToGroup Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOGROUP"u8;
    public const uint ENTITY_CODE = 4014863820;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGroup> RelatingGroup = new("RelatingGroup", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingGroup ];
}

public partial class IfcRelAssignsToGroupByFactor
   : IfcRelAssignsToGroup
{
    public static IfcRelAssignsToGroupByFactor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOGROUPBYFACTOR"u8;
    public const uint ENTITY_CODE = 2543087090;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRatioMeasure> Factor = new("Factor", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingGroup, Factor ];
}

public partial class IfcRelAssignsToProcess
   : IfcRelAssigns
{
    public static IfcRelAssignsToProcess Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOPROCESS"u8;
    public const uint ENTITY_CODE = 2767940218;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProcessSelect> RelatingProcess = new("RelatingProcess", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcMeasureWithUnit> QuantityInProcess = new("QuantityInProcess", 7, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingProcess, QuantityInProcess ];
}

public partial class IfcRelAssignsToProduct
   : IfcRelAssigns
{
    public static IfcRelAssignsToProduct Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOPRODUCT"u8;
    public const uint ENTITY_CODE = 719346156;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProductSelect> RelatingProduct = new("RelatingProduct", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingProduct ];
}

public partial class IfcRelAssignsToResource
   : IfcRelAssigns
{
    public static IfcRelAssignsToResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTORESOURCE"u8;
    public const uint ENTITY_CODE = 3183946773;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcResourceSelect> RelatingResource = new("RelatingResource", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingResource ];
}

public partial class IfcRelAssociates
   : IfcRelationship
{
    public static IfcRelAssociates Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATES"u8;
    public const uint ENTITY_CODE = 1295874853;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDefinitionSelect> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects ];
}

public partial class IfcRelAssociatesApproval
   : IfcRelAssociates
{
    public static IfcRelAssociatesApproval Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESAPPROVAL"u8;
    public const uint ENTITY_CODE = 4071643462;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcApproval> RelatingApproval = new("RelatingApproval", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingApproval ];
}

public partial class IfcRelAssociatesClassification
   : IfcRelAssociates
{
    public static IfcRelAssociatesClassification Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESCLASSIFICATION"u8;
    public const uint ENTITY_CODE = 3023068257;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationSelect> RelatingClassification = new("RelatingClassification", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingClassification ];
}

public partial class IfcRelAssociatesConstraint
   : IfcRelAssociates
{
    public static IfcRelAssociatesConstraint Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESCONSTRAINT"u8;
    public const uint ENTITY_CODE = 4261483450;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Intent = new("Intent", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcConstraint> RelatingConstraint = new("RelatingConstraint", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, Intent, RelatingConstraint ];
}

public partial class IfcRelAssociatesDocument
   : IfcRelAssociates
{
    public static IfcRelAssociatesDocument Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESDOCUMENT"u8;
    public const uint ENTITY_CODE = 4288980404;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDocumentSelect> RelatingDocument = new("RelatingDocument", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingDocument ];
}

public partial class IfcRelAssociatesLibrary
   : IfcRelAssociates
{
    public static IfcRelAssociatesLibrary Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESLIBRARY"u8;
    public const uint ENTITY_CODE = 3433840528;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLibrarySelect> RelatingLibrary = new("RelatingLibrary", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingLibrary ];
}

public partial class IfcRelAssociatesMaterial
   : IfcRelAssociates
{
    public static IfcRelAssociatesMaterial Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESMATERIAL"u8;
    public const uint ENTITY_CODE = 1645853056;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialSelect> RelatingMaterial = new("RelatingMaterial", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingMaterial ];
}

public partial class IfcRelAssociatesProfileDef
   : IfcRelAssociates
{
    public static IfcRelAssociatesProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2246184837;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> RelatingProfileDef = new("RelatingProfileDef", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingProfileDef ];
}

public partial class IfcRelationship
   : IfcRoot
{
    public static IfcRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3799843013;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcRelConnects
   : IfcRelationship
{
    public static IfcRelConnects Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTS"u8;
    public const uint ENTITY_CODE = 438030653;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcRelConnectsElements
   : IfcRelConnects
{
    public static IfcRelConnectsElements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSELEMENTS"u8;
    public const uint ENTITY_CODE = 1392017748;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConnectionGeometry> ConnectionGeometry = new("ConnectionGeometry", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcElement> RelatingElement = new("RelatingElement", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcElement> RelatedElement = new("RelatedElement", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ConnectionGeometry, RelatingElement, RelatedElement ];
}

public partial class IfcRelConnectsPathElements
   : IfcRelConnectsElements
{
    public static IfcRelConnectsPathElements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSPATHELEMENTS"u8;
    public const uint ENTITY_CODE = 3446495999;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> RelatingPriorities = new("RelatingPriorities", 7, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcInteger> RelatedPriorities = new("RelatedPriorities", 8, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcConnectionTypeEnum> RelatedConnectionType = new("RelatedConnectionType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcConnectionTypeEnum> RelatingConnectionType = new("RelatingConnectionType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ConnectionGeometry, RelatingElement, RelatedElement, RelatingPriorities, RelatedPriorities, RelatedConnectionType, RelatingConnectionType ];
}

public partial class IfcRelConnectsPorts
   : IfcRelConnects
{
    public static IfcRelConnectsPorts Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSPORTS"u8;
    public const uint ENTITY_CODE = 524223975;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPort> RelatingPort = new("RelatingPort", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPort> RelatedPort = new("RelatedPort", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcElement> RealizingElement = new("RealizingElement", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingPort, RelatedPort, RealizingElement ];
}

public partial class IfcRelConnectsPortToElement
   : IfcRelConnects
{
    public static IfcRelConnectsPortToElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSPORTTOELEMENT"u8;
    public const uint ENTITY_CODE = 3149271205;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPort> RelatingPort = new("RelatingPort", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDistributionElement> RelatedElement = new("RelatedElement", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingPort, RelatedElement ];
}

public partial class IfcRelConnectsStructuralActivity
   : IfcRelConnects
{
    public static IfcRelConnectsStructuralActivity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSSTRUCTURALACTIVITY"u8;
    public const uint ENTITY_CODE = 2837201183;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralActivityAssignmentSelect> RelatingElement = new("RelatingElement", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcStructuralActivity> RelatedStructuralActivity = new("RelatedStructuralActivity", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingElement, RelatedStructuralActivity ];
}

public partial class IfcRelConnectsStructuralMember
   : IfcRelConnects
{
    public static IfcRelConnectsStructuralMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSSTRUCTURALMEMBER"u8;
    public const uint ENTITY_CODE = 293880220;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralMember> RelatingStructuralMember = new("RelatingStructuralMember", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStructuralConnection> RelatedStructuralConnection = new("RelatedStructuralConnection", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoundaryCondition> AppliedCondition = new("AppliedCondition", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStructuralConnectionCondition> AdditionalConditions = new("AdditionalConditions", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> SupportedLength = new("SupportedLength", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAxis2Placement3D> ConditionCoordinateSystem = new("ConditionCoordinateSystem", 9, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingStructuralMember, RelatedStructuralConnection, AppliedCondition, AdditionalConditions, SupportedLength, ConditionCoordinateSystem ];
}

public partial class IfcRelConnectsWithEccentricity
   : IfcRelConnectsStructuralMember
{
    public static IfcRelConnectsWithEccentricity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSWITHECCENTRICITY"u8;
    public const uint ENTITY_CODE = 1769971157;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConnectionGeometry> ConnectionConstraint = new("ConnectionConstraint", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingStructuralMember, RelatedStructuralConnection, AppliedCondition, AdditionalConditions, SupportedLength, ConditionCoordinateSystem, ConnectionConstraint ];
}

public partial class IfcRelConnectsWithRealizingElements
   : IfcRelConnectsElements
{
    public static IfcRelConnectsWithRealizingElements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSWITHREALIZINGELEMENTS"u8;
    public const uint ENTITY_CODE = 3738501035;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElement> RealizingElements = new("RealizingElements", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> ConnectionType = new("ConnectionType", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ConnectionGeometry, RelatingElement, RelatedElement, RealizingElements, ConnectionType ];
}

public partial class IfcRelContainedInSpatialStructure
   : IfcRelConnects
{
    public static IfcRelContainedInSpatialStructure Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONTAINEDINSPATIALSTRUCTURE"u8;
    public const uint ENTITY_CODE = 3646459757;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProduct> RelatedElements = new("RelatedElements", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcSpatialElement> RelatingStructure = new("RelatingStructure", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedElements, RelatingStructure ];
}

public partial class IfcRelCoversBldgElements
   : IfcRelConnects
{
    public static IfcRelCoversBldgElements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCOVERSBLDGELEMENTS"u8;
    public const uint ENTITY_CODE = 2177806980;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElement> RelatingBuildingElement = new("RelatingBuildingElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCovering> RelatedCoverings = new("RelatedCoverings", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingBuildingElement, RelatedCoverings ];
}

public partial class IfcRelCoversSpaces
   : IfcRelConnects
{
    public static IfcRelCoversSpaces Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCOVERSSPACES"u8;
    public const uint ENTITY_CODE = 1960584869;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpace> RelatingSpace = new("RelatingSpace", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCovering> RelatedCoverings = new("RelatedCoverings", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingSpace, RelatedCoverings ];
}

public partial class IfcRelDeclares
   : IfcRelationship
{
    public static IfcRelDeclares Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDECLARES"u8;
    public const uint ENTITY_CODE = 507665137;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcContext> RelatingContext = new("RelatingContext", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDefinitionSelect> RelatedDefinitions = new("RelatedDefinitions", 5, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingContext, RelatedDefinitions ];
}

public partial class IfcRelDecomposes
   : IfcRelationship
{
    public static IfcRelDecomposes Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDECOMPOSES"u8;
    public const uint ENTITY_CODE = 2447326828;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcRelDefines
   : IfcRelationship
{
    public static IfcRelDefines Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINES"u8;
    public const uint ENTITY_CODE = 1550225206;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcRelDefinesByObject
   : IfcRelDefines
{
    public static IfcRelDefinesByObject Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINESBYOBJECT"u8;
    public const uint ENTITY_CODE = 3547409500;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObject> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcObject> RelatingObject = new("RelatingObject", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingObject ];
}

public partial class IfcRelDefinesByProperties
   : IfcRelDefines
{
    public static IfcRelDefinesByProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINESBYPROPERTIES"u8;
    public const uint ENTITY_CODE = 3293188662;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectDefinition> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPropertySetDefinitionSelect> RelatingPropertyDefinition = new("RelatingPropertyDefinition", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingPropertyDefinition ];
}

public partial class IfcRelDefinesByTemplate
   : IfcRelDefines
{
    public static IfcRelDefinesByTemplate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINESBYTEMPLATE"u8;
    public const uint ENTITY_CODE = 3806156989;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPropertySetDefinition> RelatedPropertySets = new("RelatedPropertySets", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPropertySetTemplate> RelatingTemplate = new("RelatingTemplate", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedPropertySets, RelatingTemplate ];
}

public partial class IfcRelDefinesByType
   : IfcRelDefines
{
    public static IfcRelDefinesByType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINESBYTYPE"u8;
    public const uint ENTITY_CODE = 2782820839;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObject> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcTypeObject> RelatingType = new("RelatingType", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingType ];
}

public partial class IfcRelFillsElement
   : IfcRelConnects
{
    public static IfcRelFillsElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELFILLSELEMENT"u8;
    public const uint ENTITY_CODE = 2079473304;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcOpeningElement> RelatingOpeningElement = new("RelatingOpeningElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcElement> RelatedBuildingElement = new("RelatedBuildingElement", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingOpeningElement, RelatedBuildingElement ];
}

public partial class IfcRelFlowControlElements
   : IfcRelConnects
{
    public static IfcRelFlowControlElements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELFLOWCONTROLELEMENTS"u8;
    public const uint ENTITY_CODE = 785226038;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDistributionControlElement> RelatedControlElements = new("RelatedControlElements", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcDistributionFlowElement> RelatingFlowElement = new("RelatingFlowElement", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedControlElements, RelatingFlowElement ];
}

public partial class IfcRelInterferesElements
   : IfcRelConnects
{
    public static IfcRelInterferesElements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELINTERFERESELEMENTS"u8;
    public const uint ENTITY_CODE = 1364843556;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInterferenceSelect> RelatingElement = new("RelatingElement", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcInterferenceSelect> RelatedElement = new("RelatedElement", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcConnectionGeometry> InterferenceGeometry = new("InterferenceGeometry", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSpatialZone> InterferenceSpace = new("InterferenceSpace", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcIdentifier> InterferenceType = new("InterferenceType", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> ImpliedOrder = new("ImpliedOrder", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingElement, RelatedElement, InterferenceGeometry, InterferenceSpace, InterferenceType, ImpliedOrder ];
}

public partial class IfcRelNests
   : IfcRelDecomposes
{
    public static IfcRelNests Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELNESTS"u8;
    public const uint ENTITY_CODE = 1994019001;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectDefinition> RelatingObject = new("RelatingObject", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcObjectDefinition> RelatedObjects = new("RelatedObjects", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingObject, RelatedObjects ];
}

public partial class IfcRelPositions
   : IfcRelConnects
{
    public static IfcRelPositions Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELPOSITIONS"u8;
    public const uint ENTITY_CODE = 4167272558;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositioningElement> RelatingPositioningElement = new("RelatingPositioningElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProduct> RelatedProducts = new("RelatedProducts", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingPositioningElement, RelatedProducts ];
}

public partial class IfcRelProjectsElement
   : IfcRelDecomposes
{
    public static IfcRelProjectsElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELPROJECTSELEMENT"u8;
    public const uint ENTITY_CODE = 1615168284;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElement> RelatingElement = new("RelatingElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcFeatureElementAddition> RelatedFeatureElement = new("RelatedFeatureElement", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingElement, RelatedFeatureElement ];
}

public partial class IfcRelReferencedInSpatialStructure
   : IfcRelConnects
{
    public static IfcRelReferencedInSpatialStructure Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELREFERENCEDINSPATIALSTRUCTURE"u8;
    public const uint ENTITY_CODE = 702472959;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpatialReferenceSelect> RelatedElements = new("RelatedElements", 4, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcSpatialElement> RelatingStructure = new("RelatingStructure", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedElements, RelatingStructure ];
}

public partial class IfcRelSequence
   : IfcRelConnects
{
    public static IfcRelSequence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELSEQUENCE"u8;
    public const uint ENTITY_CODE = 1835185919;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProcess> RelatingProcess = new("RelatingProcess", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProcess> RelatedProcess = new("RelatedProcess", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLagTime> TimeLag = new("TimeLag", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSequenceEnum> SequenceType = new("SequenceType", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedSequenceType = new("UserDefinedSequenceType", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingProcess, RelatedProcess, TimeLag, SequenceType, UserDefinedSequenceType ];
}

public partial class IfcRelServicesBuildings
   : IfcRelConnects
{
    public static IfcRelServicesBuildings Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELSERVICESBUILDINGS"u8;
    public const uint ENTITY_CODE = 2243065359;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSystem> RelatingSystem = new("RelatingSystem", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSpatialElement> RelatedBuildings = new("RelatedBuildings", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingSystem, RelatedBuildings ];
}

public partial class IfcRelSpaceBoundary
   : IfcRelConnects
{
    public static IfcRelSpaceBoundary Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELSPACEBOUNDARY"u8;
    public const uint ENTITY_CODE = 4011216430;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpaceBoundarySelect> RelatingSpace = new("RelatingSpace", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcElement> RelatedBuildingElement = new("RelatedBuildingElement", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcConnectionGeometry> ConnectionGeometry = new("ConnectionGeometry", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPhysicalOrVirtualEnum> PhysicalOrVirtualBoundary = new("PhysicalOrVirtualBoundary", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcInternalOrExternalEnum> InternalOrExternalBoundary = new("InternalOrExternalBoundary", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingSpace, RelatedBuildingElement, ConnectionGeometry, PhysicalOrVirtualBoundary, InternalOrExternalBoundary ];
}

public partial class IfcRelSpaceBoundary1stLevel
   : IfcRelSpaceBoundary
{
    public static IfcRelSpaceBoundary1stLevel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELSPACEBOUNDARY1STLEVEL"u8;
    public const uint ENTITY_CODE = 1464397256;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRelSpaceBoundary1stLevel> ParentBoundary = new("ParentBoundary", 9, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingSpace, RelatedBuildingElement, ConnectionGeometry, PhysicalOrVirtualBoundary, InternalOrExternalBoundary, ParentBoundary ];
}

public partial class IfcRelSpaceBoundary2ndLevel
   : IfcRelSpaceBoundary1stLevel
{
    public static IfcRelSpaceBoundary2ndLevel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELSPACEBOUNDARY2NDLEVEL"u8;
    public const uint ENTITY_CODE = 141646748;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRelSpaceBoundary2ndLevel> CorrespondingBoundary = new("CorrespondingBoundary", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingSpace, RelatedBuildingElement, ConnectionGeometry, PhysicalOrVirtualBoundary, InternalOrExternalBoundary, ParentBoundary, CorrespondingBoundary ];
}

public partial class IfcRelVoidsElement
   : IfcRelDecomposes
{
    public static IfcRelVoidsElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELVOIDSELEMENT"u8;
    public const uint ENTITY_CODE = 546583627;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElement> RelatingBuildingElement = new("RelatingBuildingElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcFeatureElementSubtraction> RelatedOpeningElement = new("RelatedOpeningElement", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingBuildingElement, RelatedOpeningElement ];
}

public partial class IfcReparametrisedCompositeCurveSegment
   : IfcCompositeCurveSegment
{
    public static IfcReparametrisedCompositeCurveSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREPARAMETRISEDCOMPOSITECURVESEGMENT"u8;
    public const uint ENTITY_CODE = 3026159936;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcParameterValue> ParamLength = new("ParamLength", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Transition, SameSense, ParentCurve, ParamLength ];
}

public partial class IfcRepresentation
   : EntityBaseClass, IfcLayeredItem
{
    public static IfcRepresentation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREPRESENTATION"u8;
    public const uint ENTITY_CODE = 3427936786;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRepresentationContext> ContextOfItems = new("ContextOfItems", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> RepresentationIdentifier = new("RepresentationIdentifier", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> RepresentationType = new("RepresentationType", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRepresentationItem> Items = new("Items", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ ContextOfItems, RepresentationIdentifier, RepresentationType, Items ];
}

public partial class IfcRepresentationContext
   : EntityBaseClass
{
    public static IfcRepresentationContext Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREPRESENTATIONCONTEXT"u8;
    public const uint ENTITY_CODE = 372806269;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ContextIdentifier = new("ContextIdentifier", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ContextType = new("ContextType", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ContextIdentifier, ContextType ];
}

public partial class IfcRepresentationItem
   : EntityBaseClass, IfcLayeredItem
{
    public static IfcRepresentationItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREPRESENTATIONITEM"u8;
    public const uint ENTITY_CODE = 695215177;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcRepresentationMap
   : EntityBaseClass, IfcProductRepresentationSelect
{
    public static IfcRepresentationMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREPRESENTATIONMAP"u8;
    public const uint ENTITY_CODE = 229209244;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement> MappingOrigin = new("MappingOrigin", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcRepresentation> MappedRepresentation = new("MappedRepresentation", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ MappingOrigin, MappedRepresentation ];
}

public partial class IfcResource
   : IfcObject, IfcResourceSelect
{
    public static IfcResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRESOURCE"u8;
    public const uint ENTITY_CODE = 1376835163;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription ];
}

public partial class IfcResourceApprovalRelationship
   : IfcResourceLevelRelationship
{
    public static IfcResourceApprovalRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRESOURCEAPPROVALRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 4146489870;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcResourceObjectSelect> RelatedResourceObjects = new("RelatedResourceObjects", 2, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcApproval> RelatingApproval = new("RelatingApproval", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatedResourceObjects, RelatingApproval ];
}

public partial class IfcResourceConstraintRelationship
   : IfcResourceLevelRelationship
{
    public static IfcResourceConstraintRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRESOURCECONSTRAINTRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2861646214;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstraint> RelatingConstraint = new("RelatingConstraint", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcResourceObjectSelect> RelatedResourceObjects = new("RelatedResourceObjects", 3, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingConstraint, RelatedResourceObjects ];
}

public partial class IfcResourceLevelRelationship
   : EntityBaseClass
{
    public static IfcResourceLevelRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRESOURCELEVELRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2358873753;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description ];
}

public partial class IfcResourceTime
   : IfcSchedulingTime
{
    public static IfcResourceTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRESOURCETIME"u8;
    public const uint ENTITY_CODE = 930309330;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDuration> ScheduleWork = new("ScheduleWork", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ScheduleUsage = new("ScheduleUsage", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ScheduleStart = new("ScheduleStart", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ScheduleFinish = new("ScheduleFinish", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ScheduleContour = new("ScheduleContour", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> LevelingDelay = new("LevelingDelay", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBoolean> IsOverAllocated = new("IsOverAllocated", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> StatusTime = new("StatusTime", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> ActualWork = new("ActualWork", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ActualUsage = new("ActualUsage", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ActualStart = new("ActualStart", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ActualFinish = new("ActualFinish", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> RemainingWork = new("RemainingWork", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> RemainingUsage = new("RemainingUsage", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> Completion = new("Completion", 17, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin, ScheduleWork, ScheduleUsage, ScheduleStart, ScheduleFinish, ScheduleContour, LevelingDelay, IsOverAllocated, StatusTime, ActualWork, ActualUsage, ActualStart, ActualFinish, RemainingWork, RemainingUsage, Completion ];
}

public partial class IfcRevolvedAreaSolid
   : IfcSweptAreaSolid
{
    public static IfcRevolvedAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREVOLVEDAREASOLID"u8;
    public const uint ENTITY_CODE = 4258379750;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis1Placement> Axis = new("Axis", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> Angle = new("Angle", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Axis, Angle ];
}

public partial class IfcRevolvedAreaSolidTapered
   : IfcRevolvedAreaSolid
{
    public static IfcRevolvedAreaSolidTapered Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREVOLVEDAREASOLIDTAPERED"u8;
    public const uint ENTITY_CODE = 3400071717;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> EndSweptArea = new("EndSweptArea", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Axis, Angle, EndSweptArea ];
}

public partial class IfcRightCircularCone
   : IfcCsgPrimitive3D
{
    public static IfcRightCircularCone Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRIGHTCIRCULARCONE"u8;
    public const uint ENTITY_CODE = 882082613;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Height = new("Height", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BottomRadius = new("BottomRadius", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, Height, BottomRadius ];
}

public partial class IfcRightCircularCylinder
   : IfcCsgPrimitive3D
{
    public static IfcRightCircularCylinder Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRIGHTCIRCULARCYLINDER"u8;
    public const uint ENTITY_CODE = 864053624;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Height = new("Height", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, Height, Radius ];
}

public partial class IfcRoad
   : IfcFacility
{
    public static IfcRoad Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROAD"u8;
    public const uint ENTITY_CODE = 1041541469;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRoadTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, PredefinedType ];
}

public partial class IfcRoadPart
   : IfcFacilityPart
{
    public static IfcRoadPart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROADPART"u8;
    public const uint ENTITY_CODE = 2741156652;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRoadPartTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, UsageType, PredefinedType ];
}

public partial class IfcRoof
   : IfcBuiltElement
{
    public static IfcRoof Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROOF"u8;
    public const uint ENTITY_CODE = 1812914585;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRoofTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcRoofType
   : IfcBuiltElementType
{
    public static IfcRoofType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROOFTYPE"u8;
    public const uint ENTITY_CODE = 1461819281;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRoofTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRoot
   : EntityBaseClass
{
    public static IfcRoot Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROOT"u8;
    public const uint ENTITY_CODE = 2047801251;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGloballyUniqueId> GlobalId = new("GlobalId", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcOwnerHistory> OwnerHistory = new("OwnerHistory", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcRoundedRectangleProfileDef
   : IfcRectangleProfileDef
{
    public static IfcRoundedRectangleProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROUNDEDRECTANGLEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3850779449;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RoundingRadius = new("RoundingRadius", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, XDim, YDim, RoundingRadius ];
}

public partial class IfcSanitaryTerminal
   : IfcFlowTerminal
{
    public static IfcSanitaryTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSANITARYTERMINAL"u8;
    public const uint ENTITY_CODE = 67389596;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSanitaryTerminalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSanitaryTerminalType
   : IfcFlowTerminalType
{
    public static IfcSanitaryTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSANITARYTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 3617698420;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSanitaryTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSchedulingTime
   : EntityBaseClass
{
    public static IfcSchedulingTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSCHEDULINGTIME"u8;
    public const uint ENTITY_CODE = 4270119168;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDataOriginEnum> DataOrigin = new("DataOrigin", 1, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedDataOrigin = new("UserDefinedDataOrigin", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin ];
}

public partial class IfcSeamCurve
   : IfcSurfaceCurve
{
    public static IfcSeamCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSEAMCURVE"u8;
    public const uint ENTITY_CODE = 1255063088;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Curve3D, AssociatedGeometry, MasterRepresentation ];
}

public partial class IfcSecondOrderPolynomialSpiral
   : IfcSpiral
{
    public static IfcSecondOrderPolynomialSpiral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECONDORDERPOLYNOMIALSPIRAL"u8;
    public const uint ENTITY_CODE = 870065458;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> QuadraticTerm = new("QuadraticTerm", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LinearTerm = new("LinearTerm", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ConstantTerm = new("ConstantTerm", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, QuadraticTerm, LinearTerm, ConstantTerm ];
}

public partial class IfcSectionedSolid
   : IfcSolidModel
{
    public static IfcSectionedSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECTIONEDSOLID"u8;
    public const uint ENTITY_CODE = 1847523590;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> Directrix = new("Directrix", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProfileDef> CrossSections = new("CrossSections", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Directrix, CrossSections ];
}

public partial class IfcSectionedSolidHorizontal
   : IfcSectionedSolid
{
    public static IfcSectionedSolidHorizontal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECTIONEDSOLIDHORIZONTAL"u8;
    public const uint ENTITY_CODE = 1276374810;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2PlacementLinear> CrossSectionPositions = new("CrossSectionPositions", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Directrix, CrossSections, CrossSectionPositions ];
}

public partial class IfcSectionedSpine
   : IfcGeometricRepresentationItem
{
    public static IfcSectionedSpine Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECTIONEDSPINE"u8;
    public const uint ENTITY_CODE = 1370369702;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCompositeCurve> SpineCurve = new("SpineCurve", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProfileDef> CrossSections = new("CrossSections", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcAxis2Placement3D> CrossSectionPositions = new("CrossSectionPositions", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ SpineCurve, CrossSections, CrossSectionPositions ];
}

public partial class IfcSectionedSurface
   : IfcSurface
{
    public static IfcSectionedSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECTIONEDSURFACE"u8;
    public const uint ENTITY_CODE = 667892294;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> Directrix = new("Directrix", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAxis2PlacementLinear> CrossSectionPositions = new("CrossSectionPositions", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcProfileDef> CrossSections = new("CrossSections", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Directrix, CrossSectionPositions, CrossSections ];
}

public partial class IfcSectionProperties
   : IfcPreDefinedProperties
{
    public static IfcSectionProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECTIONPROPERTIES"u8;
    public const uint ENTITY_CODE = 2363997831;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSectionTypeEnum> SectionType = new("SectionType", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcProfileDef> StartProfile = new("StartProfile", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProfileDef> EndProfile = new("EndProfile", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SectionType, StartProfile, EndProfile ];
}

public partial class IfcSectionReinforcementProperties
   : IfcPreDefinedProperties
{
    public static IfcSectionReinforcementProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSECTIONREINFORCEMENTPROPERTIES"u8;
    public const uint ENTITY_CODE = 180457210;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> LongitudinalStartPosition = new("LongitudinalStartPosition", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LongitudinalEndPosition = new("LongitudinalEndPosition", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> TransversePosition = new("TransversePosition", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReinforcingBarRoleEnum> ReinforcementRole = new("ReinforcementRole", 3, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcSectionProperties> SectionDefinition = new("SectionDefinition", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcReinforcementBarProperties> CrossSectionReinforcementDefinitions = new("CrossSectionReinforcementDefinitions", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ LongitudinalStartPosition, LongitudinalEndPosition, TransversePosition, ReinforcementRole, SectionDefinition, CrossSectionReinforcementDefinitions ];
}

public partial class IfcSegment
   : IfcGeometricRepresentationItem
{
    public static IfcSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSEGMENT"u8;
    public const uint ENTITY_CODE = 1820273696;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransitionCode> Transition = new("Transition", 0, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Transition ];
}

public partial class IfcSegmentedReferenceCurve
   : IfcCompositeCurve
{
    public static IfcSegmentedReferenceCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSEGMENTEDREFERENCECURVE"u8;
    public const uint ENTITY_CODE = 368995249;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoundedCurve> BaseCurve = new("BaseCurve", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPlacement> EndPoint = new("EndPoint", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect, BaseCurve, EndPoint ];
}

public partial class IfcSensor
   : IfcDistributionControlElement
{
    public static IfcSensor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSENSOR"u8;
    public const uint ENTITY_CODE = 4273072641;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSensorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSensorType
   : IfcDistributionControlElementType
{
    public static IfcSensorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSENSORTYPE"u8;
    public const uint ENTITY_CODE = 629106249;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSensorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSeventhOrderPolynomialSpiral
   : IfcSpiral
{
    public static IfcSeventhOrderPolynomialSpiral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSEVENTHORDERPOLYNOMIALSPIRAL"u8;
    public const uint ENTITY_CODE = 3983723411;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> SepticTerm = new("SepticTerm", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> SexticTerm = new("SexticTerm", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> QuinticTerm = new("QuinticTerm", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> QuarticTerm = new("QuarticTerm", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> CubicTerm = new("CubicTerm", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> QuadraticTerm = new("QuadraticTerm", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LinearTerm = new("LinearTerm", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ConstantTerm = new("ConstantTerm", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, SepticTerm, SexticTerm, QuinticTerm, QuarticTerm, CubicTerm, QuadraticTerm, LinearTerm, ConstantTerm ];
}

public partial class IfcShadingDevice
   : IfcBuiltElement
{
    public static IfcShadingDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHADINGDEVICE"u8;
    public const uint ENTITY_CODE = 1623335253;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShadingDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcShadingDeviceType
   : IfcBuiltElementType
{
    public static IfcShadingDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHADINGDEVICETYPE"u8;
    public const uint ENTITY_CODE = 2532923261;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShadingDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcShapeAspect
   : EntityBaseClass, IfcResourceObjectSelect
{
    public static IfcShapeAspect Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHAPEASPECT"u8;
    public const uint ENTITY_CODE = 2070624568;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShapeModel> ShapeRepresentations = new("ShapeRepresentations", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> ProductDefinitional = new("ProductDefinitional", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProductRepresentationSelect> PartOfProductDefinitionShape = new("PartOfProductDefinitionShape", 4, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ ShapeRepresentations, Name, Description, ProductDefinitional, PartOfProductDefinitionShape ];
}

public partial class IfcShapeModel
   : IfcRepresentation
{
    public static IfcShapeModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHAPEMODEL"u8;
    public const uint ENTITY_CODE = 86007925;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ ContextOfItems, RepresentationIdentifier, RepresentationType, Items ];
}

public partial class IfcShapeRepresentation
   : IfcShapeModel
{
    public static IfcShapeRepresentation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHAPEREPRESENTATION"u8;
    public const uint ENTITY_CODE = 3275242445;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ ContextOfItems, RepresentationIdentifier, RepresentationType, Items ];
}

public partial class IfcShellBasedSurfaceModel
   : IfcGeometricRepresentationItem
{
    public static IfcShellBasedSurfaceModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHELLBASEDSURFACEMODEL"u8;
    public const uint ENTITY_CODE = 2611018834;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShell> SbsmBoundary = new("SbsmBoundary", 0, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ SbsmBoundary ];
}

public partial class IfcSign
   : IfcElementComponent
{
    public static IfcSign Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIGN"u8;
    public const uint ENTITY_CODE = 1078373402;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSignTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSignal
   : IfcFlowTerminal
{
    public static IfcSignal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIGNAL"u8;
    public const uint ENTITY_CODE = 2983574903;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSignalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSignalType
   : IfcFlowTerminalType
{
    public static IfcSignalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIGNALTYPE"u8;
    public const uint ENTITY_CODE = 2490365359;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSignalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSignType
   : IfcElementComponentType
{
    public static IfcSignType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIGNTYPE"u8;
    public const uint ENTITY_CODE = 1165595610;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSignTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSimpleProperty
   : IfcProperty
{
    public static IfcSimpleProperty Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIMPLEPROPERTY"u8;
    public const uint ENTITY_CODE = 4288830184;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, Specification ];
}

public partial class IfcSimplePropertyTemplate
   : IfcPropertyTemplate
{
    public static IfcSimplePropertyTemplate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIMPLEPROPERTYTEMPLATE"u8;
    public const uint ENTITY_CODE = 2292871058;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSimplePropertyTemplateTypeEnum> TemplateType = new("TemplateType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> PrimaryMeasureType = new("PrimaryMeasureType", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> SecondaryMeasureType = new("SecondaryMeasureType", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPropertyEnumeration> Enumerators = new("Enumerators", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcUnit> PrimaryUnit = new("PrimaryUnit", 8, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcUnit> SecondaryUnit = new("SecondaryUnit", 9, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcLabel> Expression = new("Expression", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcStateEnum> AccessState = new("AccessState", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, TemplateType, PrimaryMeasureType, SecondaryMeasureType, Enumerators, PrimaryUnit, SecondaryUnit, Expression, AccessState ];
}

public partial class IfcSineSpiral
   : IfcSpiral
{
    public static IfcSineSpiral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSINESPIRAL"u8;
    public const uint ENTITY_CODE = 713585971;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> SineTerm = new("SineTerm", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LinearTerm = new("LinearTerm", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ConstantTerm = new("ConstantTerm", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, SineTerm, LinearTerm, ConstantTerm ];
}

public partial class IfcSite
   : IfcSpatialStructureElement
{
    public static IfcSite Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSITE"u8;
    public const uint ENTITY_CODE = 1193698164;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCompoundPlaneAngleMeasure> RefLatitude = new("RefLatitude", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCompoundPlaneAngleMeasure> RefLongitude = new("RefLongitude", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> RefElevation = new("RefElevation", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> LandTitleNumber = new("LandTitleNumber", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPostalAddress> SiteAddress = new("SiteAddress", 13, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, RefLatitude, RefLongitude, RefElevation, LandTitleNumber, SiteAddress ];
}

public partial class IfcSIUnit
   : IfcNamedUnit
{
    public static IfcSIUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIUNIT"u8;
    public const uint ENTITY_CODE = 3007951189;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSIPrefix> Prefix = new("Prefix", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcSIUnitName> Name = new("Name", 3, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Dimensions, UnitType, Prefix, Name ];
}

public partial class IfcSlab
   : IfcBuiltElement
{
    public static IfcSlab Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSLAB"u8;
    public const uint ENTITY_CODE = 634971579;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSlabTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSlabType
   : IfcBuiltElementType
{
    public static IfcSlabType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSLABTYPE"u8;
    public const uint ENTITY_CODE = 1254033699;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSlabTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSlippageConnectionCondition
   : IfcStructuralConnectionCondition
{
    public static IfcSlippageConnectionCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSLIPPAGECONNECTIONCONDITION"u8;
    public const uint ENTITY_CODE = 1230785851;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> SlippageX = new("SlippageX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> SlippageY = new("SlippageY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> SlippageZ = new("SlippageZ", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, SlippageX, SlippageY, SlippageZ ];
}

public partial class IfcSolarDevice
   : IfcEnergyConversionDevice
{
    public static IfcSolarDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSOLARDEVICE"u8;
    public const uint ENTITY_CODE = 1816778314;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSolarDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSolarDeviceType
   : IfcEnergyConversionDeviceType
{
    public static IfcSolarDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSOLARDEVICETYPE"u8;
    public const uint ENTITY_CODE = 4276976042;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSolarDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSolidModel
   : IfcGeometricRepresentationItem, IfcBooleanOperand, IfcSolidOrShell
{
    public static IfcSolidModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSOLIDMODEL"u8;
    public const uint ENTITY_CODE = 2028701031;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcSpace
   : IfcSpatialStructureElement, IfcSpaceBoundarySelect
{
    public static IfcSpace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACE"u8;
    public const uint ENTITY_CODE = 679641035;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpaceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ElevationWithFlooring = new("ElevationWithFlooring", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, PredefinedType, ElevationWithFlooring ];
}

public partial class IfcSpaceHeater
   : IfcFlowTerminal
{
    public static IfcSpaceHeater Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACEHEATER"u8;
    public const uint ENTITY_CODE = 3835376154;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpaceHeaterTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSpaceHeaterType
   : IfcFlowTerminalType
{
    public static IfcSpaceHeaterType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACEHEATERTYPE"u8;
    public const uint ENTITY_CODE = 68188634;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpaceHeaterTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSpaceType
   : IfcSpatialStructureElementType
{
    public static IfcSpaceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACETYPE"u8;
    public const uint ENTITY_CODE = 1212286099;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpaceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, LongName ];
}

public partial class IfcSpatialElement
   : IfcProduct, IfcInterferenceSelect
{
    public static IfcSpatialElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALELEMENT"u8;
    public const uint ENTITY_CODE = 1736633951;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName ];
}

public partial class IfcSpatialElementType
   : IfcTypeProduct
{
    public static IfcSpatialElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1742207527;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ElementType = new("ElementType", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcSpatialStructureElement
   : IfcSpatialElement
{
    public static IfcSpatialStructureElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALSTRUCTUREELEMENT"u8;
    public const uint ENTITY_CODE = 872665622;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElementCompositionEnum> CompositionType = new("CompositionType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType ];
}

public partial class IfcSpatialStructureElementType
   : IfcSpatialElementType
{
    public static IfcSpatialStructureElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALSTRUCTUREELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 787986470;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcSpatialZone
   : IfcSpatialElement
{
    public static IfcSpatialZone Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALZONE"u8;
    public const uint ENTITY_CODE = 2597717215;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpatialZoneTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, PredefinedType ];
}

public partial class IfcSpatialZoneType
   : IfcSpatialElementType
{
    public static IfcSpatialZoneType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALZONETYPE"u8;
    public const uint ENTITY_CODE = 551942311;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpatialZoneTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, LongName ];
}

public partial class IfcSphere
   : IfcCsgPrimitive3D
{
    public static IfcSphere Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPHERE"u8;
    public const uint ENTITY_CODE = 970498890;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, Radius ];
}

public partial class IfcSphericalSurface
   : IfcElementarySurface
{
    public static IfcSphericalSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPHERICALSURFACE"u8;
    public const uint ENTITY_CODE = 3851516881;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, Radius ];
}

public partial class IfcSpiral
   : IfcCurve
{
    public static IfcSpiral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPIRAL"u8;
    public const uint ENTITY_CODE = 3563358330;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement> Position = new("Position", 0, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Position ];
}

public partial class IfcStackTerminal
   : IfcFlowTerminal
{
    public static IfcStackTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTACKTERMINAL"u8;
    public const uint ENTITY_CODE = 762701637;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStackTerminalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcStackTerminalType
   : IfcFlowTerminalType
{
    public static IfcStackTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTACKTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 557074701;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStackTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcStair
   : IfcBuiltElement
{
    public static IfcStair Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIR"u8;
    public const uint ENTITY_CODE = 3784347268;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStairTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcStairFlight
   : IfcBuiltElement
{
    public static IfcStairFlight Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIRFLIGHT"u8;
    public const uint ENTITY_CODE = 1991789322;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> NumberOfRisers = new("NumberOfRisers", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> NumberOfTreads = new("NumberOfTreads", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RiserHeight = new("RiserHeight", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TreadLength = new("TreadLength", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcStairFlightTypeEnum> PredefinedType = new("PredefinedType", 12, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, NumberOfRisers, NumberOfTreads, RiserHeight, TreadLength, PredefinedType ];
}

public partial class IfcStairFlightType
   : IfcBuiltElementType
{
    public static IfcStairFlightType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIRFLIGHTTYPE"u8;
    public const uint ENTITY_CODE = 335595626;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStairFlightTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcStairType
   : IfcBuiltElementType
{
    public static IfcStairType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIRTYPE"u8;
    public const uint ENTITY_CODE = 1263063756;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStairTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcStructuralAction
   : IfcStructuralActivity
{
    public static IfcStructuralAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALACTION"u8;
    public const uint ENTITY_CODE = 3749586942;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> DestabilizingLoad = new("DestabilizingLoad", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad ];
}

public partial class IfcStructuralActivity
   : IfcProduct
{
    public static IfcStructuralActivity Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALACTIVITY"u8;
    public const uint ENTITY_CODE = 3780403313;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralLoad> AppliedLoad = new("AppliedLoad", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcGlobalOrLocalEnum> GlobalOrLocal = new("GlobalOrLocal", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal ];
}

public partial class IfcStructuralAnalysisModel
   : IfcSystem
{
    public static IfcStructuralAnalysisModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALANALYSISMODEL"u8;
    public const uint ENTITY_CODE = 1204480891;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAnalysisModelTypeEnum> PredefinedType = new("PredefinedType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcAxis2Placement3D> OrientationOf2DPlane = new("OrientationOf2DPlane", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStructuralLoadGroup> LoadedBy = new("LoadedBy", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcStructuralResultGroup> HasResults = new("HasResults", 8, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcObjectPlacement> SharedPlacement = new("SharedPlacement", 9, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, OrientationOf2DPlane, LoadedBy, HasResults, SharedPlacement ];
}

public partial class IfcStructuralConnection
   : IfcStructuralItem
{
    public static IfcStructuralConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCONNECTION"u8;
    public const uint ENTITY_CODE = 3631885372;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoundaryCondition> AppliedCondition = new("AppliedCondition", 7, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedCondition ];
}

public partial class IfcStructuralConnectionCondition
   : EntityBaseClass
{
    public static IfcStructuralConnectionCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCONNECTIONCONDITION"u8;
    public const uint ENTITY_CODE = 1544900841;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcStructuralCurveAction
   : IfcStructuralAction
{
    public static IfcStructuralCurveAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVEACTION"u8;
    public const uint ENTITY_CODE = 2110656253;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProjectedOrTrueLengthEnum> ProjectedOrTrue = new("ProjectedOrTrue", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcStructuralCurveActivityTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, ProjectedOrTrue, PredefinedType ];
}

public partial class IfcStructuralCurveConnection
   : IfcStructuralConnection
{
    public static IfcStructuralCurveConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVECONNECTION"u8;
    public const uint ENTITY_CODE = 4144297951;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> AxisDirection = new("AxisDirection", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedCondition, AxisDirection ];
}

public partial class IfcStructuralCurveMember
   : IfcStructuralMember
{
    public static IfcStructuralCurveMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVEMEMBER"u8;
    public const uint ENTITY_CODE = 2394259173;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralCurveMemberTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDirection> Axis = new("Axis", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType, Axis ];
}

public partial class IfcStructuralCurveMemberVarying
   : IfcStructuralCurveMember
{
    public static IfcStructuralCurveMemberVarying Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVEMEMBERVARYING"u8;
    public const uint ENTITY_CODE = 2882265595;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType, Axis ];
}

public partial class IfcStructuralCurveReaction
   : IfcStructuralReaction
{
    public static IfcStructuralCurveReaction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVEREACTION"u8;
    public const uint ENTITY_CODE = 1770286324;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralCurveActivityTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, PredefinedType ];
}

public partial class IfcStructuralItem
   : IfcProduct, IfcStructuralActivityAssignmentSelect
{
    public static IfcStructuralItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALITEM"u8;
    public const uint ENTITY_CODE = 4224088003;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcStructuralLinearAction
   : IfcStructuralCurveAction
{
    public static IfcStructuralLinearAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLINEARACTION"u8;
    public const uint ENTITY_CODE = 322418247;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, ProjectedOrTrue, PredefinedType ];
}

public partial class IfcStructuralLoad
   : EntityBaseClass
{
    public static IfcStructuralLoad Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOAD"u8;
    public const uint ENTITY_CODE = 1063824;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcStructuralLoadCase
   : IfcStructuralLoadGroup
{
    public static IfcStructuralLoadCase Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADCASE"u8;
    public const uint ENTITY_CODE = 139790428;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRatioMeasure> SelfWeightCoefficients = new("SelfWeightCoefficients", 10, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, ActionType, ActionSource, Coefficient, Purpose, SelfWeightCoefficients ];
}

public partial class IfcStructuralLoadConfiguration
   : IfcStructuralLoad
{
    public static IfcStructuralLoadConfiguration Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADCONFIGURATION"u8;
    public const uint ENTITY_CODE = 158648514;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralLoadOrResult> Values = new("Values", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLengthMeasure> Locations = new("Locations", 2, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ Name, Values, Locations ];
}

public partial class IfcStructuralLoadGroup
   : IfcGroup
{
    public static IfcStructuralLoadGroup Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADGROUP"u8;
    public const uint ENTITY_CODE = 1375763539;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLoadGroupTypeEnum> PredefinedType = new("PredefinedType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcActionTypeEnum> ActionType = new("ActionType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcActionSourceTypeEnum> ActionSource = new("ActionSource", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcRatioMeasure> Coefficient = new("Coefficient", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Purpose = new("Purpose", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, ActionType, ActionSource, Coefficient, Purpose ];
}

public partial class IfcStructuralLoadLinearForce
   : IfcStructuralLoadStatic
{
    public static IfcStructuralLoadLinearForce Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADLINEARFORCE"u8;
    public const uint ENTITY_CODE = 2129281080;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLinearForceMeasure> LinearForceX = new("LinearForceX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearForceMeasure> LinearForceY = new("LinearForceY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearForceMeasure> LinearForceZ = new("LinearForceZ", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearMomentMeasure> LinearMomentX = new("LinearMomentX", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearMomentMeasure> LinearMomentY = new("LinearMomentY", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearMomentMeasure> LinearMomentZ = new("LinearMomentZ", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LinearForceX, LinearForceY, LinearForceZ, LinearMomentX, LinearMomentY, LinearMomentZ ];
}

public partial class IfcStructuralLoadOrResult
   : IfcStructuralLoad
{
    public static IfcStructuralLoadOrResult Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADORRESULT"u8;
    public const uint ENTITY_CODE = 2630778940;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcStructuralLoadPlanarForce
   : IfcStructuralLoadStatic
{
    public static IfcStructuralLoadPlanarForce Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADPLANARFORCE"u8;
    public const uint ENTITY_CODE = 1395413487;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlanarForceMeasure> PlanarForceX = new("PlanarForceX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlanarForceMeasure> PlanarForceY = new("PlanarForceY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlanarForceMeasure> PlanarForceZ = new("PlanarForceZ", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, PlanarForceX, PlanarForceY, PlanarForceZ ];
}

public partial class IfcStructuralLoadSingleDisplacement
   : IfcStructuralLoadStatic
{
    public static IfcStructuralLoadSingleDisplacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADSINGLEDISPLACEMENT"u8;
    public const uint ENTITY_CODE = 2476372503;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> DisplacementX = new("DisplacementX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> DisplacementY = new("DisplacementY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> DisplacementZ = new("DisplacementZ", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> RotationalDisplacementRX = new("RotationalDisplacementRX", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> RotationalDisplacementRY = new("RotationalDisplacementRY", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> RotationalDisplacementRZ = new("RotationalDisplacementRZ", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DisplacementX, DisplacementY, DisplacementZ, RotationalDisplacementRX, RotationalDisplacementRY, RotationalDisplacementRZ ];
}

public partial class IfcStructuralLoadSingleDisplacementDistortion
   : IfcStructuralLoadSingleDisplacement
{
    public static IfcStructuralLoadSingleDisplacementDistortion Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADSINGLEDISPLACEMENTDISTORTION"u8;
    public const uint ENTITY_CODE = 799415584;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurvatureMeasure> Distortion = new("Distortion", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DisplacementX, DisplacementY, DisplacementZ, RotationalDisplacementRX, RotationalDisplacementRY, RotationalDisplacementRZ, Distortion ];
}

public partial class IfcStructuralLoadSingleForce
   : IfcStructuralLoadStatic
{
    public static IfcStructuralLoadSingleForce Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADSINGLEFORCE"u8;
    public const uint ENTITY_CODE = 4104008431;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcForceMeasure> ForceX = new("ForceX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> ForceY = new("ForceY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> ForceZ = new("ForceZ", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTorqueMeasure> MomentX = new("MomentX", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTorqueMeasure> MomentY = new("MomentY", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTorqueMeasure> MomentZ = new("MomentZ", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, ForceX, ForceY, ForceZ, MomentX, MomentY, MomentZ ];
}

public partial class IfcStructuralLoadSingleForceWarping
   : IfcStructuralLoadSingleForce
{
    public static IfcStructuralLoadSingleForceWarping Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADSINGLEFORCEWARPING"u8;
    public const uint ENTITY_CODE = 348142703;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWarpingMomentMeasure> WarpingMoment = new("WarpingMoment", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, ForceX, ForceY, ForceZ, MomentX, MomentY, MomentZ, WarpingMoment ];
}

public partial class IfcStructuralLoadStatic
   : IfcStructuralLoadOrResult
{
    public static IfcStructuralLoadStatic Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADSTATIC"u8;
    public const uint ENTITY_CODE = 1786190166;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcStructuralLoadTemperature
   : IfcStructuralLoadStatic
{
    public static IfcStructuralLoadTemperature Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLOADTEMPERATURE"u8;
    public const uint ENTITY_CODE = 1901015690;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> DeltaTConstant = new("DeltaTConstant", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> DeltaTY = new("DeltaTY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> DeltaTZ = new("DeltaTZ", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DeltaTConstant, DeltaTY, DeltaTZ ];
}

public partial class IfcStructuralMember
   : IfcStructuralItem
{
    public static IfcStructuralMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALMEMBER"u8;
    public const uint ENTITY_CODE = 737290366;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcStructuralPlanarAction
   : IfcStructuralSurfaceAction
{
    public static IfcStructuralPlanarAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPLANARACTION"u8;
    public const uint ENTITY_CODE = 1027411938;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, ProjectedOrTrue, PredefinedType ];
}

public partial class IfcStructuralPointAction
   : IfcStructuralAction
{
    public static IfcStructuralPointAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPOINTACTION"u8;
    public const uint ENTITY_CODE = 1770641488;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad ];
}

public partial class IfcStructuralPointConnection
   : IfcStructuralConnection
{
    public static IfcStructuralPointConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPOINTCONNECTION"u8;
    public const uint ENTITY_CODE = 3619564870;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement3D> ConditionCoordinateSystem = new("ConditionCoordinateSystem", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedCondition, ConditionCoordinateSystem ];
}

public partial class IfcStructuralPointReaction
   : IfcStructuralReaction
{
    public static IfcStructuralPointReaction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPOINTREACTION"u8;
    public const uint ENTITY_CODE = 461236213;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal ];
}

public partial class IfcStructuralReaction
   : IfcStructuralActivity
{
    public static IfcStructuralReaction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALREACTION"u8;
    public const uint ENTITY_CODE = 1656020791;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal ];
}

public partial class IfcStructuralResultGroup
   : IfcGroup
{
    public static IfcStructuralResultGroup Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALRESULTGROUP"u8;
    public const uint ENTITY_CODE = 988038204;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAnalysisTheoryTypeEnum> TheoryType = new("TheoryType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcStructuralLoadGroup> ResultForLoadGroup = new("ResultForLoadGroup", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> IsLinear = new("IsLinear", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TheoryType, ResultForLoadGroup, IsLinear ];
}

public partial class IfcStructuralSurfaceAction
   : IfcStructuralAction
{
    public static IfcStructuralSurfaceAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALSURFACEACTION"u8;
    public const uint ENTITY_CODE = 882638445;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProjectedOrTrueLengthEnum> ProjectedOrTrue = new("ProjectedOrTrue", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcStructuralSurfaceActivityTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, ProjectedOrTrue, PredefinedType ];
}

public partial class IfcStructuralSurfaceConnection
   : IfcStructuralConnection
{
    public static IfcStructuralSurfaceConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALSURFACECONNECTION"u8;
    public const uint ENTITY_CODE = 1448944911;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedCondition ];
}

public partial class IfcStructuralSurfaceMember
   : IfcStructuralMember
{
    public static IfcStructuralSurfaceMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALSURFACEMEMBER"u8;
    public const uint ENTITY_CODE = 2667159637;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralSurfaceMemberTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Thickness = new("Thickness", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType, Thickness ];
}

public partial class IfcStructuralSurfaceMemberVarying
   : IfcStructuralSurfaceMember
{
    public static IfcStructuralSurfaceMemberVarying Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALSURFACEMEMBERVARYING"u8;
    public const uint ENTITY_CODE = 2424380139;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType, Thickness ];
}

public partial class IfcStructuralSurfaceReaction
   : IfcStructuralReaction
{
    public static IfcStructuralSurfaceReaction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALSURFACEREACTION"u8;
    public const uint ENTITY_CODE = 2875859652;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralSurfaceActivityTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, PredefinedType ];
}

public partial class IfcStyledItem
   : IfcRepresentationItem
{
    public static IfcStyledItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTYLEDITEM"u8;
    public const uint ENTITY_CODE = 3343780291;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRepresentationItem> Item = new("Item", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPresentationStyle> Styles = new("Styles", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
}

public partial class IfcStyledRepresentation
   : IfcStyleModel
{
    public static IfcStyledRepresentation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTYLEDREPRESENTATION"u8;
    public const uint ENTITY_CODE = 2259822593;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ ContextOfItems, RepresentationIdentifier, RepresentationType, Items ];
}

public partial class IfcStyleModel
   : IfcRepresentation
{
    public static IfcStyleModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTYLEMODEL"u8;
    public const uint ENTITY_CODE = 1954620269;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ ContextOfItems, RepresentationIdentifier, RepresentationType, Items ];
}

public partial class IfcSubContractResource
   : IfcConstructionResource
{
    public static IfcSubContractResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSUBCONTRACTRESOURCE"u8;
    public const uint ENTITY_CODE = 1994229565;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSubContractResourceTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Usage, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcSubContractResourceType
   : IfcConstructionResourceType
{
    public static IfcSubContractResourceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSUBCONTRACTRESOURCETYPE"u8;
    public const uint ENTITY_CODE = 1643137941;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSubContractResourceTypeEnum> PredefinedType = new("PredefinedType", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType, BaseCosts, BaseQuantity, PredefinedType ];
}

public partial class IfcSubedge
   : IfcEdge
{
    public static IfcSubedge Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSUBEDGE"u8;
    public const uint ENTITY_CODE = 2590396254;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEdge> ParentEdge = new("ParentEdge", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ EdgeStart, EdgeEnd, ParentEdge ];
}

public partial class IfcSurface
   : IfcGeometricRepresentationItem, IfcGeometricSetSelect, IfcSurfaceOrFaceSurface
{
    public static IfcSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACE"u8;
    public const uint ENTITY_CODE = 2364084730;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcSurfaceCurve
   : IfcCurve, IfcCurveOnSurface
{
    public static IfcSurfaceCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACECURVE"u8;
    public const uint ENTITY_CODE = 1255495565;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> Curve3D = new("Curve3D", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPcurve> AssociatedGeometry = new("AssociatedGeometry", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcPreferredSurfaceCurveRepresentation> MasterRepresentation = new("MasterRepresentation", 2, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Curve3D, AssociatedGeometry, MasterRepresentation ];
}

public partial class IfcSurfaceCurveSweptAreaSolid
   : IfcDirectrixCurveSweptAreaSolid
{
    public static IfcSurfaceCurveSweptAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACECURVESWEPTAREASOLID"u8;
    public const uint ENTITY_CODE = 4130340898;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurface> ReferenceSurface = new("ReferenceSurface", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Directrix, StartParam, EndParam, ReferenceSurface ];
}

public partial class IfcSurfaceFeature
   : IfcFeatureElement
{
    public static IfcSurfaceFeature Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACEFEATURE"u8;
    public const uint ENTITY_CODE = 3635236316;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurfaceFeatureTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSurfaceOfLinearExtrusion
   : IfcSweptSurface
{
    public static IfcSurfaceOfLinearExtrusion Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACEOFLINEAREXTRUSION"u8;
    public const uint ENTITY_CODE = 3133299737;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> ExtrudedDirection = new("ExtrudedDirection", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ SweptCurve, Position, ExtrudedDirection, Depth ];
}

public partial class IfcSurfaceOfRevolution
   : IfcSweptSurface
{
    public static IfcSurfaceOfRevolution Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACEOFREVOLUTION"u8;
    public const uint ENTITY_CODE = 12923976;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis1Placement> AxisPosition = new("AxisPosition", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptCurve, Position, AxisPosition ];
}

public partial class IfcSurfaceReinforcementArea
   : IfcStructuralLoadOrResult
{
    public static IfcSurfaceReinforcementArea Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACEREINFORCEMENTAREA"u8;
    public const uint ENTITY_CODE = 2518701742;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> SurfaceReinforcement1 = new("SurfaceReinforcement1", 1, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLengthMeasure> SurfaceReinforcement2 = new("SurfaceReinforcement2", 2, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcRatioMeasure> ShearReinforcement = new("ShearReinforcement", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, SurfaceReinforcement1, SurfaceReinforcement2, ShearReinforcement ];
}

public partial class IfcSurfaceStyle
   : IfcPresentationStyle
{
    public static IfcSurfaceStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLE"u8;
    public const uint ENTITY_CODE = 4071505551;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurfaceSide> Side = new("Side", 1, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcSurfaceStyleElementSelect> Styles = new("Styles", 2, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Name, Side, Styles ];
}

public partial class IfcSurfaceStyleLighting
   : IfcPresentationItem, IfcSurfaceStyleElementSelect
{
    public static IfcSurfaceStyleLighting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLELIGHTING"u8;
    public const uint ENTITY_CODE = 1409349527;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColourRgb> DiffuseTransmissionColour = new("DiffuseTransmissionColour", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcColourRgb> DiffuseReflectionColour = new("DiffuseReflectionColour", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcColourRgb> TransmissionColour = new("TransmissionColour", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcColourRgb> ReflectanceColour = new("ReflectanceColour", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ DiffuseTransmissionColour, DiffuseReflectionColour, TransmissionColour, ReflectanceColour ];
}

public partial class IfcSurfaceStyleRefraction
   : IfcPresentationItem, IfcSurfaceStyleElementSelect
{
    public static IfcSurfaceStyleRefraction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLEREFRACTION"u8;
    public const uint ENTITY_CODE = 3213948220;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcReal> RefractionIndex = new("RefractionIndex", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> DispersionFactor = new("DispersionFactor", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RefractionIndex, DispersionFactor ];
}

public partial class IfcSurfaceStyleRendering
   : IfcSurfaceStyleShading
{
    public static IfcSurfaceStyleRendering Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLERENDERING"u8;
    public const uint ENTITY_CODE = 3420639349;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColourOrFactor> DiffuseColour = new("DiffuseColour", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColourOrFactor> TransmissionColour = new("TransmissionColour", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColourOrFactor> DiffuseTransmissionColour = new("DiffuseTransmissionColour", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColourOrFactor> ReflectionColour = new("ReflectionColour", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColourOrFactor> SpecularColour = new("SpecularColour", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcSpecularHighlightSelect> SpecularHighlight = new("SpecularHighlight", 7, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcReflectanceMethodEnum> ReflectanceMethod = new("ReflectanceMethod", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ SurfaceColour, Transparency, DiffuseColour, TransmissionColour, DiffuseTransmissionColour, ReflectionColour, SpecularColour, SpecularHighlight, ReflectanceMethod ];
}

public partial class IfcSurfaceStyleShading
   : IfcPresentationItem, IfcSurfaceStyleElementSelect
{
    public static IfcSurfaceStyleShading Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLESHADING"u8;
    public const uint ENTITY_CODE = 2237861999;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColourRgb> SurfaceColour = new("SurfaceColour", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Transparency = new("Transparency", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ SurfaceColour, Transparency ];
}

public partial class IfcSurfaceStyleWithTextures
   : IfcPresentationItem, IfcSurfaceStyleElementSelect
{
    public static IfcSurfaceStyleWithTextures Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLEWITHTEXTURES"u8;
    public const uint ENTITY_CODE = 2497588223;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurfaceTexture> Textures = new("Textures", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Textures ];
}

public partial class IfcSurfaceTexture
   : IfcPresentationItem
{
    public static IfcSurfaceTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACETEXTURE"u8;
    public const uint ENTITY_CODE = 2119552589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> RepeatS = new("RepeatS", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBoolean> RepeatT = new("RepeatT", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> Mode = new("Mode", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCartesianTransformationOperator2D> TextureTransform = new("TextureTransform", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcIdentifier> Parameter = new("Parameter", 4, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, Mode, TextureTransform, Parameter ];
}

public partial class IfcSweptAreaSolid
   : IfcSolidModel
{
    public static IfcSweptAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSWEPTAREASOLID"u8;
    public const uint ENTITY_CODE = 3734918784;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> SweptArea = new("SweptArea", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAxis2Placement3D> Position = new("Position", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position ];
}

public partial class IfcSweptDiskSolid
   : IfcSolidModel
{
    public static IfcSweptDiskSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSWEPTDISKSOLID"u8;
    public const uint ENTITY_CODE = 1837973444;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> Directrix = new("Directrix", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> InnerRadius = new("InnerRadius", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> StartParam = new("StartParam", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> EndParam = new("EndParam", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Directrix, Radius, InnerRadius, StartParam, EndParam ];
}

public partial class IfcSweptDiskSolidPolygonal
   : IfcSweptDiskSolid
{
    public static IfcSweptDiskSolidPolygonal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSWEPTDISKSOLIDPOLYGONAL"u8;
    public const uint ENTITY_CODE = 3051361351;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FilletRadius = new("FilletRadius", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Directrix, Radius, InnerRadius, StartParam, EndParam, FilletRadius ];
}

public partial class IfcSweptSurface
   : IfcSurface
{
    public static IfcSweptSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSWEPTSURFACE"u8;
    public const uint ENTITY_CODE = 2515609299;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileDef> SweptCurve = new("SweptCurve", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAxis2Placement3D> Position = new("Position", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptCurve, Position ];
}

public partial class IfcSwitchingDevice
   : IfcFlowController
{
    public static IfcSwitchingDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSWITCHINGDEVICE"u8;
    public const uint ENTITY_CODE = 51716247;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSwitchingDeviceTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSwitchingDeviceType
   : IfcFlowControllerType
{
    public static IfcSwitchingDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSWITCHINGDEVICETYPE"u8;
    public const uint ENTITY_CODE = 1062227407;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSwitchingDeviceTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSystem
   : IfcGroup
{
    public static IfcSystem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSYSTEM"u8;
    public const uint ENTITY_CODE = 4241047294;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcSystemFurnitureElement
   : IfcFurnishingElement
{
    public static IfcSystemFurnitureElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSYSTEMFURNITUREELEMENT"u8;
    public const uint ENTITY_CODE = 3174040268;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSystemFurnitureElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcSystemFurnitureElementType
   : IfcFurnishingElementType
{
    public static IfcSystemFurnitureElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSYSTEMFURNITUREELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1911274308;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSystemFurnitureElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTable
   : EntityBaseClass, IfcMetricValueSelect, IfcObjectReferenceSelect
{
    public static IfcTable Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTABLE"u8;
    public const uint ENTITY_CODE = 1707516689;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTableRow> Rows = new("Rows", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcTableColumn> Columns = new("Columns", 2, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Rows, Columns ];
}

public partial class IfcTableColumn
   : EntityBaseClass
{
    public static IfcTableColumn Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTABLECOLUMN"u8;
    public const uint ENTITY_CODE = 756104283;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identifier = new("Identifier", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcReference> ReferencePath = new("ReferencePath", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Identifier, Name, Description, Unit, ReferencePath ];
}

public partial class IfcTableRow
   : EntityBaseClass
{
    public static IfcTableRow Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTABLEROW"u8;
    public const uint ENTITY_CODE = 4259718863;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> RowCells = new("RowCells", 0, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcBoolean> IsHeading = new("IsHeading", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RowCells, IsHeading ];
}

public partial class IfcTank
   : IfcFlowStorageDevice
{
    public static IfcTank Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTANK"u8;
    public const uint ENTITY_CODE = 17753987;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTankTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcTankType
   : IfcFlowStorageDeviceType
{
    public static IfcTankType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTANKTYPE"u8;
    public const uint ENTITY_CODE = 1925107899;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTankTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTask
   : IfcProcess
{
    public static IfcTask Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTASK"u8;
    public const uint ENTITY_CODE = 13369750;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> WorkMethod = new("WorkMethod", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBoolean> IsMilestone = new("IsMilestone", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> Priority = new("Priority", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTaskTime> TaskTime = new("TaskTime", 11, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTaskTypeEnum> PredefinedType = new("PredefinedType", 12, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, LongDescription, Status, WorkMethod, IsMilestone, Priority, TaskTime, PredefinedType ];
}

public partial class IfcTaskTime
   : IfcSchedulingTime
{
    public static IfcTaskTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTASKTIME"u8;
    public const uint ENTITY_CODE = 2181296591;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTaskDurationEnum> DurationType = new("DurationType", 3, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDuration> ScheduleDuration = new("ScheduleDuration", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ScheduleStart = new("ScheduleStart", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ScheduleFinish = new("ScheduleFinish", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> EarlyStart = new("EarlyStart", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> EarlyFinish = new("EarlyFinish", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> LateStart = new("LateStart", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> LateFinish = new("LateFinish", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> FreeFloat = new("FreeFloat", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> TotalFloat = new("TotalFloat", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcBoolean> IsCritical = new("IsCritical", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> StatusTime = new("StatusTime", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> ActualDuration = new("ActualDuration", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ActualStart = new("ActualStart", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> ActualFinish = new("ActualFinish", 17, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> RemainingTime = new("RemainingTime", 18, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> Completion = new("Completion", 19, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin, DurationType, ScheduleDuration, ScheduleStart, ScheduleFinish, EarlyStart, EarlyFinish, LateStart, LateFinish, FreeFloat, TotalFloat, IsCritical, StatusTime, ActualDuration, ActualStart, ActualFinish, RemainingTime, Completion ];
}

public partial class IfcTaskTimeRecurring
   : IfcTaskTime
{
    public static IfcTaskTimeRecurring Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTASKTIMERECURRING"u8;
    public const uint ENTITY_CODE = 834701598;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRecurrencePattern> Recurrence = new("Recurrence", 20, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin, DurationType, ScheduleDuration, ScheduleStart, ScheduleFinish, EarlyStart, EarlyFinish, LateStart, LateFinish, FreeFloat, TotalFloat, IsCritical, StatusTime, ActualDuration, ActualStart, ActualFinish, RemainingTime, Completion, Recurrence ];
}

public partial class IfcTaskType
   : IfcTypeProcess
{
    public static IfcTaskType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTASKTYPE"u8;
    public const uint ENTITY_CODE = 905142182;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTaskTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> WorkMethod = new("WorkMethod", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ProcessType, PredefinedType, WorkMethod ];
}

public partial class IfcTelecomAddress
   : IfcAddress
{
    public static IfcTelecomAddress Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTELECOMADDRESS"u8;
    public const uint ENTITY_CODE = 2254656692;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> TelephoneNumbers = new("TelephoneNumbers", 3, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> FacsimileNumbers = new("FacsimileNumbers", 4, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> PagerNumber = new("PagerNumber", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ElectronicMailAddresses = new("ElectronicMailAddresses", 6, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcURIReference> WWWHomePageURL = new("WWWHomePageURL", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcURIReference> MessagingIDs = new("MessagingIDs", 8, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Purpose, Description, UserDefinedPurpose, TelephoneNumbers, FacsimileNumbers, PagerNumber, ElectronicMailAddresses, WWWHomePageURL, MessagingIDs ];
}

public partial class IfcTendon
   : IfcReinforcingElement
{
    public static IfcTendon Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTENDON"u8;
    public const uint ENTITY_CODE = 3940259567;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTendonTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> CrossSectionArea = new("CrossSectionArea", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcForceMeasure> TensionForce = new("TensionForce", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPressureMeasure> PreStress = new("PreStress", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> FrictionCoefficient = new("FrictionCoefficient", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> AnchorageSlip = new("AnchorageSlip", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MinCurvatureRadius = new("MinCurvatureRadius", 16, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, PredefinedType, NominalDiameter, CrossSectionArea, TensionForce, PreStress, FrictionCoefficient, AnchorageSlip, MinCurvatureRadius ];
}

public partial class IfcTendonAnchor
   : IfcReinforcingElement
{
    public static IfcTendonAnchor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTENDONANCHOR"u8;
    public const uint ENTITY_CODE = 2726656758;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTendonAnchorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, PredefinedType ];
}

public partial class IfcTendonAnchorType
   : IfcReinforcingElementType
{
    public static IfcTendonAnchorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTENDONANCHORTYPE"u8;
    public const uint ENTITY_CODE = 3520932870;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTendonAnchorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTendonConduit
   : IfcReinforcingElement
{
    public static IfcTendonConduit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTENDONCONDUIT"u8;
    public const uint ENTITY_CODE = 2367256711;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTendonConduitTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, PredefinedType ];
}

public partial class IfcTendonConduitType
   : IfcReinforcingElementType
{
    public static IfcTendonConduitType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTENDONCONDUITTYPE"u8;
    public const uint ENTITY_CODE = 770103455;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTendonConduitTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTendonType
   : IfcReinforcingElementType
{
    public static IfcTendonType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTENDONTYPE"u8;
    public const uint ENTITY_CODE = 2376348759;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTendonTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> CrossSectionArea = new("CrossSectionArea", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> SheathDiameter = new("SheathDiameter", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, NominalDiameter, CrossSectionArea, SheathDiameter ];
}

public partial class IfcTessellatedFaceSet
   : IfcTessellatedItem, IfcBooleanOperand
{
    public static IfcTessellatedFaceSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTESSELLATEDFACESET"u8;
    public const uint ENTITY_CODE = 2960336296;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPointList3D> Coordinates = new("Coordinates", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoolean> Closed = new("Closed", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Coordinates, Closed ];
}

public partial class IfcTessellatedItem
   : IfcGeometricRepresentationItem
{
    public static IfcTessellatedItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTESSELLATEDITEM"u8;
    public const uint ENTITY_CODE = 2306487610;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcTextLiteral
   : IfcGeometricRepresentationItem
{
    public static IfcTextLiteral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTLITERAL"u8;
    public const uint ENTITY_CODE = 134569191;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPresentableText> Literal = new("Literal", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAxis2Placement> Placement = new("Placement", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTextPath> Path = new("Path", 2, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Literal, Placement, Path ];
}

public partial class IfcTextLiteralWithExtent
   : IfcTextLiteral
{
    public static IfcTextLiteralWithExtent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTLITERALWITHEXTENT"u8;
    public const uint ENTITY_CODE = 783027983;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPlanarExtent> Extent = new("Extent", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcBoxAlignment> BoxAlignment = new("BoxAlignment", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Literal, Placement, Path, Extent, BoxAlignment ];
}

public partial class IfcTextStyle
   : IfcPresentationStyle
{
    public static IfcTextStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTSTYLE"u8;
    public const uint ENTITY_CODE = 1641706589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTextStyleForDefinedFont> TextCharacterAppearance = new("TextCharacterAppearance", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTextStyleTextModel> TextStyle = new("TextStyle", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTextFontSelect> TextFontStyle = new("TextFontStyle", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcBoolean> ModelOrDraughting = new("ModelOrDraughting", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, TextCharacterAppearance, TextStyle, TextFontStyle, ModelOrDraughting ];
}

public partial class IfcTextStyleFontModel
   : IfcPreDefinedTextFont
{
    public static IfcTextStyleFontModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTSTYLEFONTMODEL"u8;
    public const uint ENTITY_CODE = 636760693;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTextFontName> FontFamily = new("FontFamily", 1, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcFontStyle> FontStyle = new("FontStyle", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcFontVariant> FontVariant = new("FontVariant", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcFontWeight> FontWeight = new("FontWeight", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSizeSelect> FontSize = new("FontSize", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, FontFamily, FontStyle, FontVariant, FontWeight, FontSize ];
}

public partial class IfcTextStyleForDefinedFont
   : IfcPresentationItem
{
    public static IfcTextStyleForDefinedFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTSTYLEFORDEFINEDFONT"u8;
    public const uint ENTITY_CODE = 4218362128;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColour> Colour = new("Colour", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColour> BackgroundColour = new("BackgroundColour", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Colour, BackgroundColour ];
}

public partial class IfcTextStyleTextModel
   : IfcPresentationItem
{
    public static IfcTextStyleTextModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTSTYLETEXTMODEL"u8;
    public const uint ENTITY_CODE = 3190959443;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSizeSelect> TextIndent = new("TextIndent", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTextAlignment> TextAlign = new("TextAlign", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTextDecoration> TextDecoration = new("TextDecoration", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSizeSelect> LetterSpacing = new("LetterSpacing", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcSizeSelect> WordSpacing = new("WordSpacing", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTextTransformation> TextTransform = new("TextTransform", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSizeSelect> LineHeight = new("LineHeight", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ TextIndent, TextAlign, TextDecoration, LetterSpacing, WordSpacing, TextTransform, LineHeight ];
}

public partial class IfcTextureCoordinate
   : IfcPresentationItem
{
    public static IfcTextureCoordinate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTURECOORDINATE"u8;
    public const uint ENTITY_CODE = 1304733824;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSurfaceTexture> Maps = new("Maps", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Maps ];
}

public partial class IfcTextureCoordinateGenerator
   : IfcTextureCoordinate
{
    public static IfcTextureCoordinateGenerator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTURECOORDINATEGENERATOR"u8;
    public const uint ENTITY_CODE = 986362205;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Mode = new("Mode", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcReal> Parameter = new("Parameter", 2, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Maps, Mode, Parameter ];
}

public partial class IfcTextureCoordinateIndices
   : EntityBaseClass
{
    public static IfcTextureCoordinateIndices Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTURECOORDINATEINDICES"u8;
    public const uint ENTITY_CODE = 3543525805;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveInteger> TexCoordIndex = new("TexCoordIndex", 0, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcIndexedPolygonalFace> TexCoordsOf = new("TexCoordsOf", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ TexCoordIndex, TexCoordsOf ];
}

public partial class IfcTextureCoordinateIndicesWithVoids
   : IfcTextureCoordinateIndices
{
    public static IfcTextureCoordinateIndicesWithVoids Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTURECOORDINATEINDICESWITHVOIDS"u8;
    public const uint ENTITY_CODE = 3719124592;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveInteger> InnerTexCoordIndices = new("InnerTexCoordIndices", 2, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ TexCoordIndex, TexCoordsOf, InnerTexCoordIndices ];
}

public partial class IfcTextureMap
   : IfcTextureCoordinate
{
    public static IfcTextureMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTUREMAP"u8;
    public const uint ENTITY_CODE = 1189656152;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTextureVertex> Vertices = new("Vertices", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcFace> MappedTo = new("MappedTo", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Maps, Vertices, MappedTo ];
}

public partial class IfcTextureVertex
   : IfcPresentationItem
{
    public static IfcTextureVertex Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTUREVERTEX"u8;
    public const uint ENTITY_CODE = 1240493628;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcParameterValue> Coordinates = new("Coordinates", 0, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Coordinates ];
}

public partial class IfcTextureVertexList
   : IfcPresentationItem
{
    public static IfcTextureVertexList Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTUREVERTEXLIST"u8;
    public const uint ENTITY_CODE = 1829175844;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcParameterValue> TexCoordsList = new("TexCoordsList", 0, IfcTypeKind.Alias, 2);
    public override IfcAttribute[] Attributes => [ TexCoordsList ];
}

public partial class IfcThirdOrderPolynomialSpiral
   : IfcSpiral
{
    public static IfcThirdOrderPolynomialSpiral Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTHIRDORDERPOLYNOMIALSPIRAL"u8;
    public const uint ENTITY_CODE = 3471048375;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLengthMeasure> CubicTerm = new("CubicTerm", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> QuadraticTerm = new("QuadraticTerm", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LinearTerm = new("LinearTerm", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ConstantTerm = new("ConstantTerm", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, CubicTerm, QuadraticTerm, LinearTerm, ConstantTerm ];
}

public partial class IfcTimePeriod
   : EntityBaseClass
{
    public static IfcTimePeriod Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTIMEPERIOD"u8;
    public const uint ENTITY_CODE = 1007701959;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTime> StartTime = new("StartTime", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTime> EndTime = new("EndTime", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ StartTime, EndTime ];
}

public partial class IfcTimeSeries
   : EntityBaseClass, IfcMetricValueSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
{
    public static IfcTimeSeries Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTIMESERIES"u8;
    public const uint ENTITY_CODE = 3335580439;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> StartTime = new("StartTime", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> EndTime = new("EndTime", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeSeriesDataTypeEnum> TimeSeriesDataType = new("TimeSeriesDataType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDataOriginEnum> DataOrigin = new("DataOrigin", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedDataOrigin = new("UserDefinedDataOrigin", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 7, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, StartTime, EndTime, TimeSeriesDataType, DataOrigin, UserDefinedDataOrigin, Unit ];
}

public partial class IfcTimeSeriesValue
   : EntityBaseClass
{
    public static IfcTimeSeriesValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTIMESERIESVALUE"u8;
    public const uint ENTITY_CODE = 3069996460;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValue> ListValues = new("ListValues", 0, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ ListValues ];
}

public partial class IfcTopologicalRepresentationItem
   : IfcRepresentationItem
{
    public static IfcTopologicalRepresentationItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTOPOLOGICALREPRESENTATIONITEM"u8;
    public const uint ENTITY_CODE = 1555561512;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcTopologyRepresentation
   : IfcShapeModel
{
    public static IfcTopologyRepresentation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTOPOLOGYREPRESENTATION"u8;
    public const uint ENTITY_CODE = 1550388787;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ ContextOfItems, RepresentationIdentifier, RepresentationType, Items ];
}

public partial class IfcToroidalSurface
   : IfcElementarySurface
{
    public static IfcToroidalSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTOROIDALSURFACE"u8;
    public const uint ENTITY_CODE = 4062637252;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MajorRadius = new("MajorRadius", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MinorRadius = new("MinorRadius", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Position, MajorRadius, MinorRadius ];
}

public partial class IfcTrackElement
   : IfcBuiltElement
{
    public static IfcTrackElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRACKELEMENT"u8;
    public const uint ENTITY_CODE = 2682459974;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTrackElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcTrackElementType
   : IfcBuiltElementType
{
    public static IfcTrackElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRACKELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 22383862;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTrackElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTransformer
   : IfcEnergyConversionDevice
{
    public static IfcTransformer Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSFORMER"u8;
    public const uint ENTITY_CODE = 1563906938;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransformerTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcTransformerType
   : IfcEnergyConversionDeviceType
{
    public static IfcTransformerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSFORMERTYPE"u8;
    public const uint ENTITY_CODE = 2567241530;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransformerTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTransportationDevice
   : IfcElement
{
    public static IfcTransportationDevice Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSPORTATIONDEVICE"u8;
    public const uint ENTITY_CODE = 706866423;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcTransportationDeviceType
   : IfcElementType
{
    public static IfcTransportationDeviceType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSPORTATIONDEVICETYPE"u8;
    public const uint ENTITY_CODE = 2066717999;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcTransportElement
   : IfcTransportationDevice
{
    public static IfcTransportElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSPORTELEMENT"u8;
    public const uint ENTITY_CODE = 2895867572;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransportElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcTransportElementType
   : IfcTransportationDeviceType
{
    public static IfcTransportElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSPORTELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 92928668;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransportElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTrapeziumProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcTrapeziumProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRAPEZIUMPROFILEDEF"u8;
    public const uint ENTITY_CODE = 2575033564;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BottomXDim = new("BottomXDim", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TopXDim = new("TopXDim", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> YDim = new("YDim", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> TopXOffset = new("TopXOffset", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, BottomXDim, TopXDim, YDim, TopXOffset ];
}

public partial class IfcTriangulatedFaceSet
   : IfcTessellatedFaceSet
{
    public static IfcTriangulatedFaceSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRIANGULATEDFACESET"u8;
    public const uint ENTITY_CODE = 3363089572;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcParameterValue> Normals = new("Normals", 2, IfcTypeKind.Alias, 2);
    public readonly IfcAttribute<IfcPositiveInteger> CoordIndex = new("CoordIndex", 3, IfcTypeKind.Alias, 2);
    public readonly IfcAttribute<IfcPositiveInteger> PnIndex = new("PnIndex", 4, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Coordinates, Closed, Normals, CoordIndex, PnIndex ];
}

public partial class IfcTriangulatedIrregularNetwork
   : IfcTriangulatedFaceSet
{
    public static IfcTriangulatedIrregularNetwork Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRIANGULATEDIRREGULARNETWORK"u8;
    public const uint ENTITY_CODE = 2191358852;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInteger> Flags = new("Flags", 5, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Coordinates, Closed, Normals, CoordIndex, PnIndex, Flags ];
}

public partial class IfcTrimmedCurve
   : IfcBoundedCurve
{
    public static IfcTrimmedCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRIMMEDCURVE"u8;
    public const uint ENTITY_CODE = 1528703406;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> BasisCurve = new("BasisCurve", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTrimmingSelect> Trim1 = new("Trim1", 1, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcTrimmingSelect> Trim2 = new("Trim2", 2, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcBoolean> SenseAgreement = new("SenseAgreement", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTrimmingPreference> MasterRepresentation = new("MasterRepresentation", 4, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, Trim1, Trim2, SenseAgreement, MasterRepresentation ];
}

public partial class IfcTShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcTShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTSHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 217492446;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeWidth = new("FlangeWidth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeThickness = new("FlangeThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FlangeEdgeRadius = new("FlangeEdgeRadius", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> WebEdgeRadius = new("WebEdgeRadius", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> WebSlope = new("WebSlope", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> FlangeSlope = new("FlangeSlope", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, FlangeWidth, WebThickness, FlangeThickness, FilletRadius, FlangeEdgeRadius, WebEdgeRadius, WebSlope, FlangeSlope ];
}

public partial class IfcTubeBundle
   : IfcEnergyConversionDevice
{
    public static IfcTubeBundle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTUBEBUNDLE"u8;
    public const uint ENTITY_CODE = 465216733;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTubeBundleTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcTubeBundleType
   : IfcEnergyConversionDeviceType
{
    public static IfcTubeBundleType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTUBEBUNDLETYPE"u8;
    public const uint ENTITY_CODE = 866369589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTubeBundleTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcTypeObject
   : IfcObjectDefinition
{
    public static IfcTypeObject Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTYPEOBJECT"u8;
    public const uint ENTITY_CODE = 2249877892;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> ApplicableOccurrence = new("ApplicableOccurrence", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPropertySetDefinition> HasPropertySets = new("HasPropertySets", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets ];
}

public partial class IfcTypeProcess
   : IfcTypeObject, IfcProcessSelect
{
    public static IfcTypeProcess Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTYPEPROCESS"u8;
    public const uint ENTITY_CODE = 1318230964;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ProcessType = new("ProcessType", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ProcessType ];
}

public partial class IfcTypeProduct
   : IfcTypeObject, IfcProductSelect
{
    public static IfcTypeProduct Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTYPEPRODUCT"u8;
    public const uint ENTITY_CODE = 658519926;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRepresentationMap> RepresentationMaps = new("RepresentationMaps", 6, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Tag = new("Tag", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag ];
}

public partial class IfcTypeResource
   : IfcTypeObject, IfcResourceSelect
{
    public static IfcTypeResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTYPERESOURCE"u8;
    public const uint ENTITY_CODE = 2691944947;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identification = new("Identification", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> LongDescription = new("LongDescription", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ResourceType = new("ResourceType", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, Identification, LongDescription, ResourceType ];
}

public partial class IfcUnitaryControlElement
   : IfcDistributionControlElement
{
    public static IfcUnitaryControlElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCUNITARYCONTROLELEMENT"u8;
    public const uint ENTITY_CODE = 3969197810;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcUnitaryControlElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcUnitaryControlElementType
   : IfcDistributionControlElementType
{
    public static IfcUnitaryControlElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCUNITARYCONTROLELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 3744520674;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcUnitaryControlElementTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcUnitaryEquipment
   : IfcEnergyConversionDevice
{
    public static IfcUnitaryEquipment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCUNITARYEQUIPMENT"u8;
    public const uint ENTITY_CODE = 3695978331;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcUnitaryEquipmentTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcUnitaryEquipmentType
   : IfcEnergyConversionDeviceType
{
    public static IfcUnitaryEquipmentType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCUNITARYEQUIPMENTTYPE"u8;
    public const uint ENTITY_CODE = 4163530947;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcUnitaryEquipmentTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcUnitAssignment
   : EntityBaseClass
{
    public static IfcUnitAssignment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCUNITASSIGNMENT"u8;
    public const uint ENTITY_CODE = 990410120;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcUnit> Units = new("Units", 0, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Units ];
}

public partial class IfcUShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcUShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCUSHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3931088027;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeWidth = new("FlangeWidth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeThickness = new("FlangeThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> EdgeRadius = new("EdgeRadius", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> FlangeSlope = new("FlangeSlope", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, FlangeWidth, WebThickness, FlangeThickness, FilletRadius, EdgeRadius, FlangeSlope ];
}

public partial class IfcValve
   : IfcFlowController
{
    public static IfcValve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVALVE"u8;
    public const uint ENTITY_CODE = 1892890335;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValveTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcValveType
   : IfcFlowControllerType
{
    public static IfcValveType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVALVETYPE"u8;
    public const uint ENTITY_CODE = 1040468647;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcValveTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcVector
   : IfcGeometricRepresentationItem, IfcHatchLineDistanceSelect, IfcVectorOrDirection
{
    public static IfcVector Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVECTOR"u8;
    public const uint ENTITY_CODE = 3000129244;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDirection> Orientation = new("Orientation", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> Magnitude = new("Magnitude", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Orientation, Magnitude ];
}

public partial class IfcVehicle
   : IfcTransportationDevice
{
    public static IfcVehicle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVEHICLE"u8;
    public const uint ENTITY_CODE = 2519469701;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVehicleTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcVehicleType
   : IfcTransportationDeviceType
{
    public static IfcVehicleType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVEHICLETYPE"u8;
    public const uint ENTITY_CODE = 2997566797;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVehicleTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcVertex
   : IfcTopologicalRepresentationItem
{
    public static IfcVertex Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVERTEX"u8;
    public const uint ENTITY_CODE = 2675829729;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcVertexLoop
   : IfcLoop
{
    public static IfcVertexLoop Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVERTEXLOOP"u8;
    public const uint ENTITY_CODE = 1420919631;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVertex> LoopVertex = new("LoopVertex", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ LoopVertex ];
}

public partial class IfcVertexPoint
   : IfcVertex, IfcPointOrVertexPoint
{
    public static IfcVertexPoint Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVERTEXPOINT"u8;
    public const uint ENTITY_CODE = 3704214141;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPoint> VertexGeometry = new("VertexGeometry", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ VertexGeometry ];
}

public partial class IfcVibrationDamper
   : IfcElementComponent
{
    public static IfcVibrationDamper Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVIBRATIONDAMPER"u8;
    public const uint ENTITY_CODE = 3315837290;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDamperTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcVibrationDamperType
   : IfcElementComponentType
{
    public static IfcVibrationDamperType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVIBRATIONDAMPERTYPE"u8;
    public const uint ENTITY_CODE = 1381588682;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVibrationDamperTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcVibrationIsolator
   : IfcElementComponent
{
    public static IfcVibrationIsolator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVIBRATIONISOLATOR"u8;
    public const uint ENTITY_CODE = 3015712760;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVibrationIsolatorTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcVibrationIsolatorType
   : IfcElementComponentType
{
    public static IfcVibrationIsolatorType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVIBRATIONISOLATORTYPE"u8;
    public const uint ENTITY_CODE = 1874719280;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVibrationIsolatorTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcVirtualElement
   : IfcElement
{
    public static IfcVirtualElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVIRTUALELEMENT"u8;
    public const uint ENTITY_CODE = 3712824770;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVirtualElementTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcVirtualGridIntersection
   : EntityBaseClass, IfcGridPlacementDirectionSelect
{
    public static IfcVirtualGridIntersection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVIRTUALGRIDINTERSECTION"u8;
    public const uint ENTITY_CODE = 3806830111;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGridAxis> IntersectingAxes = new("IntersectingAxes", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLengthMeasure> OffsetDistances = new("OffsetDistances", 1, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ IntersectingAxes, OffsetDistances ];
}

public partial class IfcVoidingFeature
   : IfcFeatureElementSubtraction
{
    public static IfcVoidingFeature Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVOIDINGFEATURE"u8;
    public const uint ENTITY_CODE = 2033415299;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVoidingFeatureTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcWall
   : IfcBuiltElement
{
    public static IfcWall Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWALL"u8;
    public const uint ENTITY_CODE = 2077320315;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWallTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcWallStandardCase
   : IfcWall
{
    public static IfcWallStandardCase Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWALLSTANDARDCASE"u8;
    public const uint ENTITY_CODE = 2426171302;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcWallType
   : IfcBuiltElementType
{
    public static IfcWallType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWALLTYPE"u8;
    public const uint ENTITY_CODE = 3895821283;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWallTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcWasteTerminal
   : IfcFlowTerminal
{
    public static IfcWasteTerminal Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWASTETERMINAL"u8;
    public const uint ENTITY_CODE = 1978569455;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWasteTerminalTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcWasteTerminalType
   : IfcFlowTerminalType
{
    public static IfcWasteTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWASTETERMINALTYPE"u8;
    public const uint ENTITY_CODE = 3320508503;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWasteTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcWindow
   : IfcBuiltElement
{
    public static IfcWindow Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOW"u8;
    public const uint ENTITY_CODE = 548816575;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallHeight = new("OverallHeight", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallWidth = new("OverallWidth", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcWindowTypeEnum> PredefinedType = new("PredefinedType", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcWindowTypePartitioningEnum> PartitioningType = new("PartitioningType", 11, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedPartitioningType = new("UserDefinedPartitioningType", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, OverallHeight, OverallWidth, PredefinedType, PartitioningType, UserDefinedPartitioningType ];
}

public partial class IfcWindowLiningProperties
   : IfcPreDefinedPropertySet
{
    public static IfcWindowLiningProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOWLININGPROPERTIES"u8;
    public const uint ENTITY_CODE = 399706723;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LiningDepth = new("LiningDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> LiningThickness = new("LiningThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> TransomThickness = new("TransomThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> MullionThickness = new("MullionThickness", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> FirstTransomOffset = new("FirstTransomOffset", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> SecondTransomOffset = new("SecondTransomOffset", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> FirstMullionOffset = new("FirstMullionOffset", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> SecondMullionOffset = new("SecondMullionOffset", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 12, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningOffset = new("LiningOffset", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningToPanelOffsetX = new("LiningToPanelOffsetX", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningToPanelOffsetY = new("LiningToPanelOffsetY", 15, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, LiningDepth, LiningThickness, TransomThickness, MullionThickness, FirstTransomOffset, SecondTransomOffset, FirstMullionOffset, SecondMullionOffset, ShapeAspectStyle, LiningOffset, LiningToPanelOffsetX, LiningToPanelOffsetY ];
}

public partial class IfcWindowPanelProperties
   : IfcPreDefinedPropertySet
{
    public static IfcWindowPanelProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOWPANELPROPERTIES"u8;
    public const uint ENTITY_CODE = 1008424894;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWindowPanelOperationEnum> OperationType = new("OperationType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcWindowPanelPositionEnum> PanelPosition = new("PanelPosition", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FrameDepth = new("FrameDepth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FrameThickness = new("FrameThickness", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, OperationType, PanelPosition, FrameDepth, FrameThickness, ShapeAspectStyle ];
}

public partial class IfcWindowType
   : IfcBuiltElementType
{
    public static IfcWindowType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOWTYPE"u8;
    public const uint ENTITY_CODE = 2623720583;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWindowTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcWindowTypePartitioningEnum> PartitioningType = new("PartitioningType", 10, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcBoolean> ParameterTakesPrecedence = new("ParameterTakesPrecedence", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedPartitioningType = new("UserDefinedPartitioningType", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType, PartitioningType, ParameterTakesPrecedence, UserDefinedPartitioningType ];
}

public partial class IfcWorkCalendar
   : IfcControl
{
    public static IfcWorkCalendar Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKCALENDAR"u8;
    public const uint ENTITY_CODE = 2726223564;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWorkTime> WorkingTimes = new("WorkingTimes", 6, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcWorkTime> ExceptionTimes = new("ExceptionTimes", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcWorkCalendarTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, WorkingTimes, ExceptionTimes, PredefinedType ];
}

public partial class IfcWorkControl
   : IfcControl
{
    public static IfcWorkControl Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKCONTROL"u8;
    public const uint ENTITY_CODE = 2134216975;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDateTime> CreationDate = new("CreationDate", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPerson> Creators = new("Creators", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Purpose = new("Purpose", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> Duration = new("Duration", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDuration> TotalFloat = new("TotalFloat", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> StartTime = new("StartTime", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTime> FinishTime = new("FinishTime", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, CreationDate, Creators, Purpose, Duration, TotalFloat, StartTime, FinishTime ];
}

public partial class IfcWorkPlan
   : IfcWorkControl
{
    public static IfcWorkPlan Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKPLAN"u8;
    public const uint ENTITY_CODE = 4262694961;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWorkPlanTypeEnum> PredefinedType = new("PredefinedType", 13, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, CreationDate, Creators, Purpose, Duration, TotalFloat, StartTime, FinishTime, PredefinedType ];
}

public partial class IfcWorkSchedule
   : IfcWorkControl
{
    public static IfcWorkSchedule Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKSCHEDULE"u8;
    public const uint ENTITY_CODE = 302889391;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWorkScheduleTypeEnum> PredefinedType = new("PredefinedType", 13, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identification, CreationDate, Creators, Purpose, Duration, TotalFloat, StartTime, FinishTime, PredefinedType ];
}

public partial class IfcWorkTime
   : IfcSchedulingTime
{
    public static IfcWorkTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKTIME"u8;
    public const uint ENTITY_CODE = 2564703307;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRecurrencePattern> RecurrencePattern = new("RecurrencePattern", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDate> StartDate = new("StartDate", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDate> FinishDate = new("FinishDate", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DataOrigin, UserDefinedDataOrigin, RecurrencePattern, StartDate, FinishDate ];
}

public partial class IfcZone
   : IfcSystem
{
    public static IfcZone Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCZONE"u8;
    public const uint ENTITY_CODE = 3177690381;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName ];
}

public partial class IfcZShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcZShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCZSHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3159577188;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Depth = new("Depth", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeWidth = new("FlangeWidth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeThickness = new("FlangeThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNonNegativeLengthMeasure> EdgeRadius = new("EdgeRadius", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, FlangeWidth, WebThickness, FlangeThickness, FilletRadius, EdgeRadius ];
}
