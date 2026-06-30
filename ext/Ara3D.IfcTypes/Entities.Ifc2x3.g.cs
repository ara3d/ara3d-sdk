#pragma warning disable CS0108
namespace Ara3D.IfcTypes.Ifc2x3;

public partial class Ifc2DCompositeCurve
   : IfcCompositeCurve
{
    public static Ifc2DCompositeCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFC2DCOMPOSITECURVE"u8;
    public const uint ENTITY_CODE = 3947031395;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect ];
}

public partial class IfcActionRequest
   : IfcControl
{
    public static IfcActionRequest Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCACTIONREQUEST"u8;
    public const uint ENTITY_CODE = 1511108338;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> RequestID = new("RequestID", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, RequestID ];
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
   : EntityBaseClass
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

public partial class IfcAngularDimension
   : IfcDimensionCurveDirectedCallout
{
    public static IfcAngularDimension Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANGULARDIMENSION"u8;
    public const uint ENTITY_CODE = 1891106325;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Contents ];
}

public partial class IfcAnnotation
   : IfcProduct
{
    public static IfcAnnotation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATION"u8;
    public const uint ENTITY_CODE = 3507439686;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation ];
}

public partial class IfcAnnotationCurveOccurrence
   : IfcAnnotationOccurrence, IfcDraughtingCalloutElement
{
    public static IfcAnnotationCurveOccurrence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONCURVEOCCURRENCE"u8;
    public const uint ENTITY_CODE = 1280242788;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
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

public partial class IfcAnnotationFillAreaOccurrence
   : IfcAnnotationOccurrence
{
    public static IfcAnnotationFillAreaOccurrence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONFILLAREAOCCURRENCE"u8;
    public const uint ENTITY_CODE = 640257799;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPoint> FillStyleTarget = new("FillStyleTarget", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcGlobalOrLocalEnum> GlobalOrLocal = new("GlobalOrLocal", 4, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Item, Styles, Name, FillStyleTarget, GlobalOrLocal ];
}

public partial class IfcAnnotationOccurrence
   : IfcStyledItem
{
    public static IfcAnnotationOccurrence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONOCCURRENCE"u8;
    public const uint ENTITY_CODE = 1032160087;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
}

public partial class IfcAnnotationSurface
   : IfcGeometricRepresentationItem
{
    public static IfcAnnotationSurface Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONSURFACE"u8;
    public const uint ENTITY_CODE = 2056624741;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGeometricRepresentationItem> Item = new("Item", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTextureCoordinate> TextureCoordinates = new("TextureCoordinates", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Item, TextureCoordinates ];
}

public partial class IfcAnnotationSurfaceOccurrence
   : IfcAnnotationOccurrence
{
    public static IfcAnnotationSurfaceOccurrence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONSURFACEOCCURRENCE"u8;
    public const uint ENTITY_CODE = 84905240;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
}

public partial class IfcAnnotationSymbolOccurrence
   : IfcAnnotationOccurrence, IfcDraughtingCalloutElement
{
    public static IfcAnnotationSymbolOccurrence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONSYMBOLOCCURRENCE"u8;
    public const uint ENTITY_CODE = 1583779087;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
}

public partial class IfcAnnotationTextOccurrence
   : IfcAnnotationOccurrence, IfcDraughtingCalloutElement
{
    public static IfcAnnotationTextOccurrence Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCANNOTATIONTEXTOCCURRENCE"u8;
    public const uint ENTITY_CODE = 3377995880;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
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
   : EntityBaseClass, IfcObjectReferenceSelect
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
    public readonly IfcAttribute<IfcDateTimeSelect> ApplicableDate = new("ApplicableDate", 4, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> FixedUntilDate = new("FixedUntilDate", 5, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, AppliedValue, UnitBasis, ApplicableDate, FixedUntilDate ];
}

public partial class IfcAppliedValueRelationship
   : EntityBaseClass
{
    public static IfcAppliedValueRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPLIEDVALUERELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1207568455;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAppliedValue> ComponentOfTotal = new("ComponentOfTotal", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAppliedValue> Components = new("Components", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcArithmeticOperatorEnum> ArithmeticOperator = new("ArithmeticOperator", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ComponentOfTotal, Components, ArithmeticOperator, Name, Description ];
}

public partial class IfcApproval
   : EntityBaseClass
{
    public static IfcApproval Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPROVAL"u8;
    public const uint ENTITY_CODE = 771577372;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcText> Description = new("Description", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> ApprovalDateTime = new("ApprovalDateTime", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcLabel> ApprovalStatus = new("ApprovalStatus", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ApprovalLevel = new("ApprovalLevel", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> ApprovalQualifier = new("ApprovalQualifier", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> Identifier = new("Identifier", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Description, ApprovalDateTime, ApprovalStatus, ApprovalLevel, ApprovalQualifier, Name, Identifier ];
}

public partial class IfcApprovalActorRelationship
   : EntityBaseClass
{
    public static IfcApprovalActorRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPROVALACTORRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3943192587;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActorSelect> Actor = new("Actor", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcApproval> Approval = new("Approval", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcActorRole> Role = new("Role", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Actor, Approval, Role ];
}

public partial class IfcApprovalPropertyRelationship
   : EntityBaseClass
{
    public static IfcApprovalPropertyRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPROVALPROPERTYRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2182728579;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProperty> ApprovedProperties = new("ApprovedProperties", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcApproval> Approval = new("Approval", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ApprovedProperties, Approval ];
}

public partial class IfcApprovalRelationship
   : EntityBaseClass
{
    public static IfcApprovalRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCAPPROVALRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1503631090;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcApproval> RelatedApproval = new("RelatedApproval", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcApproval> RelatingApproval = new("RelatingApproval", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RelatedApproval, RelatingApproval, Description, Name ];
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
    public readonly IfcAttribute<IfcIdentifier> AssetID = new("AssetID", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCostValue> OriginalValue = new("OriginalValue", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> CurrentValue = new("CurrentValue", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> TotalReplacementCost = new("TotalReplacementCost", 8, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcActorSelect> Owner = new("Owner", 9, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcActorSelect> User = new("User", 10, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPerson> ResponsiblePerson = new("ResponsiblePerson", 11, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCalendarDate> IncorporationDate = new("IncorporationDate", 12, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> DepreciatedValue = new("DepreciatedValue", 13, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, AssetID, OriginalValue, CurrentValue, TotalReplacementCost, Owner, User, ResponsiblePerson, IncorporationDate, DepreciatedValue ];
}

public partial class IfcAsymmetricIShapeProfileDef
   : IfcIShapeProfileDef
{
    public static IfcAsymmetricIShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCASYMMETRICISHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 3607974385;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TopFlangeWidth = new("TopFlangeWidth", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TopFlangeThickness = new("TopFlangeThickness", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TopFlangeFilletRadius = new("TopFlangeFilletRadius", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInY = new("CentreOfGravityInY", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, OverallWidth, OverallDepth, WebThickness, FlangeThickness, FilletRadius, TopFlangeWidth, TopFlangeThickness, TopFlangeFilletRadius, CentreOfGravityInY ];
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

public partial class IfcBeam
   : IfcBuildingElement
{
    public static IfcBeam Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEAM"u8;
    public const uint ENTITY_CODE = 3562220184;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcBeamType
   : IfcBuildingElementType
{
    public static IfcBeamType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEAMTYPE"u8;
    public const uint ENTITY_CODE = 2765867472;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBeamTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBezierCurve
   : IfcBSplineCurve
{
    public static IfcBezierCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBEZIERCURVE"u8;
    public const uint ENTITY_CODE = 3784534775;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Degree, ControlPointsList, CurveForm, ClosedCurve, SelfIntersect ];
}

public partial class IfcBlobTexture
   : IfcSurfaceTexture
{
    public static IfcBlobTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBLOBTEXTURE"u8;
    public const uint ENTITY_CODE = 3517409251;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> RasterFormat = new("RasterFormat", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> RasterCode = new("RasterCode", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, TextureType, TextureTransform, RasterFormat, RasterCode ];
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

public partial class IfcBoundaryEdgeCondition
   : IfcBoundaryCondition
{
    public static IfcBoundaryEdgeCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYEDGECONDITION"u8;
    public const uint ENTITY_CODE = 2472611581;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcModulusOfLinearSubgradeReactionMeasure> LinearStiffnessByLengthX = new("LinearStiffnessByLengthX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfLinearSubgradeReactionMeasure> LinearStiffnessByLengthY = new("LinearStiffnessByLengthY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfLinearSubgradeReactionMeasure> LinearStiffnessByLengthZ = new("LinearStiffnessByLengthZ", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfRotationalSubgradeReactionMeasure> RotationalStiffnessByLengthX = new("RotationalStiffnessByLengthX", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfRotationalSubgradeReactionMeasure> RotationalStiffnessByLengthY = new("RotationalStiffnessByLengthY", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfRotationalSubgradeReactionMeasure> RotationalStiffnessByLengthZ = new("RotationalStiffnessByLengthZ", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LinearStiffnessByLengthX, LinearStiffnessByLengthY, LinearStiffnessByLengthZ, RotationalStiffnessByLengthX, RotationalStiffnessByLengthY, RotationalStiffnessByLengthZ ];
}

public partial class IfcBoundaryFaceCondition
   : IfcBoundaryCondition
{
    public static IfcBoundaryFaceCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYFACECONDITION"u8;
    public const uint ENTITY_CODE = 2562956589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcModulusOfSubgradeReactionMeasure> LinearStiffnessByAreaX = new("LinearStiffnessByAreaX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfSubgradeReactionMeasure> LinearStiffnessByAreaY = new("LinearStiffnessByAreaY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfSubgradeReactionMeasure> LinearStiffnessByAreaZ = new("LinearStiffnessByAreaZ", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LinearStiffnessByAreaX, LinearStiffnessByAreaY, LinearStiffnessByAreaZ ];
}

public partial class IfcBoundaryNodeCondition
   : IfcBoundaryCondition
{
    public static IfcBoundaryNodeCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYNODECONDITION"u8;
    public const uint ENTITY_CODE = 2407292458;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLinearStiffnessMeasure> LinearStiffnessX = new("LinearStiffnessX", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearStiffnessMeasure> LinearStiffnessY = new("LinearStiffnessY", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearStiffnessMeasure> LinearStiffnessZ = new("LinearStiffnessZ", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRotationalStiffnessMeasure> RotationalStiffnessX = new("RotationalStiffnessX", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRotationalStiffnessMeasure> RotationalStiffnessY = new("RotationalStiffnessY", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRotationalStiffnessMeasure> RotationalStiffnessZ = new("RotationalStiffnessZ", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LinearStiffnessX, LinearStiffnessY, LinearStiffnessZ, RotationalStiffnessX, RotationalStiffnessY, RotationalStiffnessZ ];
}

public partial class IfcBoundaryNodeConditionWarping
   : IfcBoundaryNodeCondition
{
    public static IfcBoundaryNodeConditionWarping Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBOUNDARYNODECONDITIONWARPING"u8;
    public const uint ENTITY_CODE = 2919905048;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWarpingMomentMeasure> WarpingStiffness = new("WarpingStiffness", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, LinearStiffnessX, LinearStiffnessY, LinearStiffnessZ, RotationalStiffnessX, RotationalStiffnessY, RotationalStiffnessZ, WarpingStiffness ];
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

public partial class IfcBSplineCurve
   : IfcBoundedCurve
{
    public static IfcBSplineCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBSPLINECURVE"u8;
    public const uint ENTITY_CODE = 3214482937;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<INTEGER> Degree = new("Degree", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCartesianPoint> ControlPointsList = new("ControlPointsList", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcBSplineCurveForm> CurveForm = new("CurveForm", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<LOGICAL> ClosedCurve = new("ClosedCurve", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<LOGICAL> SelfIntersect = new("SelfIntersect", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Degree, ControlPointsList, CurveForm, ClosedCurve, SelfIntersect ];
}

public partial class IfcBuilding
   : IfcSpatialStructureElement
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

public partial class IfcBuildingElement
   : IfcElement
{
    public static IfcBuildingElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENT"u8;
    public const uint ENTITY_CODE = 1804826109;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcBuildingElementComponent
   : IfcBuildingElement
{
    public static IfcBuildingElementComponent Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTCOMPONENT"u8;
    public const uint ENTITY_CODE = 2779939960;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcBuildingElementPart
   : IfcBuildingElementComponent
{
    public static IfcBuildingElementPart Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPART"u8;
    public const uint ENTITY_CODE = 145338828;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcBuildingElementProxy
   : IfcBuildingElement
{
    public static IfcBuildingElementProxy Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPROXY"u8;
    public const uint ENTITY_CODE = 1258167731;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElementCompositionEnum> CompositionType = new("CompositionType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, CompositionType ];
}

public partial class IfcBuildingElementProxyType
   : IfcBuildingElementType
{
    public static IfcBuildingElementProxyType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTPROXYTYPE"u8;
    public const uint ENTITY_CODE = 365776395;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBuildingElementProxyTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcBuildingElementType
   : IfcElementType
{
    public static IfcBuildingElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCBUILDINGELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1329496405;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
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

public partial class IfcCalendarDate
   : EntityBaseClass, IfcDateTimeSelect, IfcObjectReferenceSelect
{
    public static IfcCalendarDate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCALENDARDATE"u8;
    public const uint ENTITY_CODE = 353641985;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDayInMonthNumber> DayComponent = new("DayComponent", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMonthInYearNumber> MonthComponent = new("MonthComponent", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcYearNumber> YearComponent = new("YearComponent", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ DayComponent, MonthComponent, YearComponent ];
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
    public readonly IfcAttribute<REAL> Scale = new("Scale", 3, IfcTypeKind.Alias, 0);
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
    public readonly IfcAttribute<REAL> Scale2 = new("Scale2", 4, IfcTypeKind.Alias, 0);
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
    public readonly IfcAttribute<REAL> Scale2 = new("Scale2", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<REAL> Scale3 = new("Scale3", 6, IfcTypeKind.Alias, 0);
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

public partial class IfcChamferEdgeFeature
   : IfcEdgeFeature
{
    public static IfcChamferEdgeFeature Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCHAMFEREDGEFEATURE"u8;
    public const uint ENTITY_CODE = 1949032836;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Width = new("Width", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Height = new("Height", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, FeatureLength, Width, Height ];
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

public partial class IfcClassification
   : EntityBaseClass
{
    public static IfcClassification Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATION"u8;
    public const uint ENTITY_CODE = 1675978639;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Source = new("Source", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Edition = new("Edition", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCalendarDate> EditionDate = new("EditionDate", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Source, Edition, EditionDate, Name ];
}

public partial class IfcClassificationItem
   : EntityBaseClass
{
    public static IfcClassificationItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATIONITEM"u8;
    public const uint ENTITY_CODE = 2505353128;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationNotationFacet> Notation = new("Notation", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcClassification> ItemOf = new("ItemOf", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> Title = new("Title", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Notation, ItemOf, Title ];
}

public partial class IfcClassificationItemRelationship
   : EntityBaseClass
{
    public static IfcClassificationItemRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATIONITEMRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 273119318;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationItem> RelatingItem = new("RelatingItem", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcClassificationItem> RelatedItems = new("RelatedItems", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ RelatingItem, RelatedItems ];
}

public partial class IfcClassificationNotation
   : EntityBaseClass, IfcClassificationNotationSelect
{
    public static IfcClassificationNotation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATIONNOTATION"u8;
    public const uint ENTITY_CODE = 1267406829;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationNotationFacet> NotationFacets = new("NotationFacets", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ NotationFacets ];
}

public partial class IfcClassificationNotationFacet
   : EntityBaseClass
{
    public static IfcClassificationNotationFacet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATIONNOTATIONFACET"u8;
    public const uint ENTITY_CODE = 3429090224;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> NotationValue = new("NotationValue", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ NotationValue ];
}

public partial class IfcClassificationReference
   : IfcExternalReference, IfcClassificationNotationSelect
{
    public static IfcClassificationReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLASSIFICATIONREFERENCE"u8;
    public const uint ENTITY_CODE = 1249450268;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassification> ReferencedSource = new("ReferencedSource", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name, ReferencedSource ];
}

public partial class IfcClosedShell
   : IfcConnectedFaceSet, IfcShell
{
    public static IfcClosedShell Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCLOSEDSHELL"u8;
    public const uint ENTITY_CODE = 2374515303;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ CfsFaces ];
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

public partial class IfcColourSpecification
   : EntityBaseClass, IfcColour
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
   : IfcBuildingElement
{
    public static IfcColumn Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLUMN"u8;
    public const uint ENTITY_CODE = 4230436045;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcColumnType
   : IfcBuildingElementType
{
    public static IfcColumnType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOLUMNTYPE"u8;
    public const uint ENTITY_CODE = 2387334149;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColumnTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
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
    public override IfcAttribute[] Attributes => [ Name, Description, UsageName, HasProperties ];
}

public partial class IfcCompositeCurve
   : IfcBoundedCurve
{
    public static IfcCompositeCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPOSITECURVE"u8;
    public const uint ENTITY_CODE = 3290217845;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCompositeCurveSegment> Segments = new("Segments", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<LOGICAL> SelfIntersect = new("SelfIntersect", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Segments, SelfIntersect ];
}

public partial class IfcCompositeCurveSegment
   : IfcGeometricRepresentationItem
{
    public static IfcCompositeCurveSegment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOMPOSITECURVESEGMENT"u8;
    public const uint ENTITY_CODE = 690703830;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransitionCode> Transition = new("Transition", 0, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<BOOLEAN> SameSense = new("SameSense", 1, IfcTypeKind.Alias, 0);
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

public partial class IfcCondition
   : IfcGroup
{
    public static IfcCondition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONDITION"u8;
    public const uint ENTITY_CODE = 1118383528;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcConditionCriterion
   : IfcControl
{
    public static IfcConditionCriterion Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONDITIONCRITERION"u8;
    public const uint ENTITY_CODE = 672941199;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConditionCriterionSelect> Criterion = new("Criterion", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> CriterionDateTime = new("CriterionDateTime", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Criterion, CriterionDateTime ];
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

public partial class IfcConnectionPortGeometry
   : IfcConnectionGeometry
{
    public static IfcConnectionPortGeometry Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONNECTIONPORTGEOMETRY"u8;
    public const uint ENTITY_CODE = 3684747392;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAxis2Placement> LocationAtRelatingElement = new("LocationAtRelatingElement", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcAxis2Placement> LocationAtRelatedElement = new("LocationAtRelatedElement", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcProfileDef> ProfileOfPort = new("ProfileOfPort", 2, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ LocationAtRelatingElement, LocationAtRelatedElement, ProfileOfPort ];
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

public partial class IfcConstraint
   : EntityBaseClass
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
    public readonly IfcAttribute<IfcDateTimeSelect> CreationTime = new("CreationTime", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedGrade = new("UserDefinedGrade", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, ConstraintGrade, ConstraintSource, CreatingActor, CreationTime, UserDefinedGrade ];
}

public partial class IfcConstraintAggregationRelationship
   : EntityBaseClass
{
    public static IfcConstraintAggregationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRAINTAGGREGATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2215331684;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcConstraint> RelatingConstraint = new("RelatingConstraint", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcConstraint> RelatedConstraints = new("RelatedConstraints", 3, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLogicalOperatorEnum> LogicalAggregator = new("LogicalAggregator", 4, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingConstraint, RelatedConstraints, LogicalAggregator ];
}

public partial class IfcConstraintClassificationRelationship
   : EntityBaseClass
{
    public static IfcConstraintClassificationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRAINTCLASSIFICATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1215454554;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstraint> ClassifiedConstraint = new("ClassifiedConstraint", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcClassificationNotationSelect> RelatedClassifications = new("RelatedClassifications", 1, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ ClassifiedConstraint, RelatedClassifications ];
}

public partial class IfcConstraintRelationship
   : EntityBaseClass
{
    public static IfcConstraintRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRAINTRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1885091754;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcConstraint> RelatingConstraint = new("RelatingConstraint", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcConstraint> RelatedConstraints = new("RelatedConstraints", 3, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingConstraint, RelatedConstraints ];
}

public partial class IfcConstructionEquipmentResource
   : IfcConstructionResource
{
    public static IfcConstructionEquipmentResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONEQUIPMENTRESOURCE"u8;
    public const uint ENTITY_CODE = 325370190;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity ];
}

public partial class IfcConstructionMaterialResource
   : IfcConstructionResource
{
    public static IfcConstructionMaterialResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONMATERIALRESOURCE"u8;
    public const uint ENTITY_CODE = 3540649679;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActorSelect> Suppliers = new("Suppliers", 9, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcRatioMeasure> UsageRatio = new("UsageRatio", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity, Suppliers, UsageRatio ];
}

public partial class IfcConstructionProductResource
   : IfcConstructionResource
{
    public static IfcConstructionProductResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONPRODUCTRESOURCE"u8;
    public const uint ENTITY_CODE = 1684371685;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity ];
}

public partial class IfcConstructionResource
   : IfcResource
{
    public static IfcConstructionResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCONSTRUCTIONRESOURCE"u8;
    public const uint ENTITY_CODE = 1336170662;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> ResourceIdentifier = new("ResourceIdentifier", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> ResourceGroup = new("ResourceGroup", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcResourceConsumptionEnum> ResourceConsumption = new("ResourceConsumption", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcMeasureWithUnit> BaseQuantity = new("BaseQuantity", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity ];
}

public partial class IfcContextDependentUnit
   : IfcNamedUnit
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
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
   : IfcNamedUnit
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

public partial class IfcCoordinatedUniversalTimeOffset
   : EntityBaseClass
{
    public static IfcCoordinatedUniversalTimeOffset Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOORDINATEDUNIVERSALTIMEOFFSET"u8;
    public const uint ENTITY_CODE = 1782560382;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcHourInDay> HourOffset = new("HourOffset", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMinuteInHour> MinuteOffset = new("MinuteOffset", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAheadOrBehind> Sense = new("Sense", 2, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ HourOffset, MinuteOffset, Sense ];
}

public partial class IfcCostItem
   : IfcControl
{
    public static IfcCostItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSTITEM"u8;
    public const uint ENTITY_CODE = 204301829;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcCostSchedule
   : IfcControl
{
    public static IfcCostSchedule Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSTSCHEDULE"u8;
    public const uint ENTITY_CODE = 1266701043;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcActorSelect> SubmittedBy = new("SubmittedBy", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcActorSelect> PreparedBy = new("PreparedBy", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> SubmittedOn = new("SubmittedOn", 7, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorSelect> TargetUsers = new("TargetUsers", 9, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcDateTimeSelect> UpdateDate = new("UpdateDate", 10, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcIdentifier> ID = new("ID", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCostScheduleTypeEnum> PredefinedType = new("PredefinedType", 12, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, SubmittedBy, PreparedBy, SubmittedOn, Status, TargetUsers, UpdateDate, ID, PredefinedType ];
}

public partial class IfcCostValue
   : IfcAppliedValue, IfcMetricValueSelect
{
    public static IfcCostValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOSTVALUE"u8;
    public const uint ENTITY_CODE = 4023367015;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> CostType = new("CostType", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Condition = new("Condition", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, AppliedValue, UnitBasis, ApplicableDate, FixedUntilDate, CostType, Condition ];
}

public partial class IfcCovering
   : IfcBuildingElement
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
   : IfcBuildingElementType
{
    public static IfcCoveringType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCOVERINGTYPE"u8;
    public const uint ENTITY_CODE = 1670716522;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCoveringTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcCraneRailAShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcCraneRailAShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCRANERAILASHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 670799064;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallHeight = new("OverallHeight", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseWidth2 = new("BaseWidth2", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> HeadWidth = new("HeadWidth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> HeadDepth2 = new("HeadDepth2", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> HeadDepth3 = new("HeadDepth3", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseWidth4 = new("BaseWidth4", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseDepth1 = new("BaseDepth1", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseDepth2 = new("BaseDepth2", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseDepth3 = new("BaseDepth3", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInY = new("CentreOfGravityInY", 14, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, OverallHeight, BaseWidth2, Radius, HeadWidth, HeadDepth2, HeadDepth3, WebThickness, BaseWidth4, BaseDepth1, BaseDepth2, BaseDepth3, CentreOfGravityInY ];
}

public partial class IfcCraneRailFShapeProfileDef
   : IfcParameterizedProfileDef
{
    public static IfcCraneRailFShapeProfileDef Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCRANERAILFSHAPEPROFILEDEF"u8;
    public const uint ENTITY_CODE = 759909183;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallHeight = new("OverallHeight", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> HeadWidth = new("HeadWidth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> HeadDepth2 = new("HeadDepth2", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> HeadDepth3 = new("HeadDepth3", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebThickness = new("WebThickness", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseDepth1 = new("BaseDepth1", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BaseDepth2 = new("BaseDepth2", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInY = new("CentreOfGravityInY", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, OverallHeight, HeadWidth, Radius, HeadDepth2, HeadDepth3, WebThickness, BaseDepth1, BaseDepth2, CentreOfGravityInY ];
}

public partial class IfcCrewResource
   : IfcConstructionResource
{
    public static IfcCrewResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCREWRESOURCE"u8;
    public const uint ENTITY_CODE = 3676323422;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity ];
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> InternalFilletRadius = new("InternalFilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInX = new("CentreOfGravityInX", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, Width, WallThickness, Girth, InternalFilletRadius, CentreOfGravityInX ];
}

public partial class IfcCurrencyRelationship
   : EntityBaseClass
{
    public static IfcCurrencyRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURRENCYRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3359804106;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMonetaryUnit> RelatingMonetaryUnit = new("RelatingMonetaryUnit", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcMonetaryUnit> RelatedMonetaryUnit = new("RelatedMonetaryUnit", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ExchangeRate = new("ExchangeRate", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateAndTime> RateDateTime = new("RateDateTime", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLibraryInformation> RateSource = new("RateSource", 4, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ RelatingMonetaryUnit, RelatedMonetaryUnit, ExchangeRate, RateDateTime, RateSource ];
}

public partial class IfcCurtainWall
   : IfcBuildingElement
{
    public static IfcCurtainWall Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURTAINWALL"u8;
    public const uint ENTITY_CODE = 2095691047;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcCurtainWallType
   : IfcBuildingElementType
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

public partial class IfcCurveStyle
   : IfcPresentationStyle, IfcPresentationStyleSelect
{
    public static IfcCurveStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESTYLE"u8;
    public const uint ENTITY_CODE = 796586243;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurveFontOrScaledCurveFontSelect> CurveFont = new("CurveFont", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcSizeSelect> CurveWidth = new("CurveWidth", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcColour> CurveColour = new("CurveColour", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, CurveFont, CurveWidth, CurveColour ];
}

public partial class IfcCurveStyleFont
   : EntityBaseClass, IfcCurveStyleFontSelect
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
   : EntityBaseClass, IfcCurveFontOrScaledCurveFontSelect
{
    public static IfcCurveStyleFontAndScaling Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCCURVESTYLEFONTANDSCALING"u8;
    public const uint ENTITY_CODE = 320924324;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCurveStyleFontSelect> CurveFont = new("CurveFont", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> CurveFontScaling = new("CurveFontScaling", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, CurveFont, CurveFontScaling ];
}

public partial class IfcCurveStyleFontPattern
   : EntityBaseClass
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

public partial class IfcDateAndTime
   : EntityBaseClass, IfcDateTimeSelect, IfcObjectReferenceSelect
{
    public static IfcDateAndTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDATEANDTIME"u8;
    public const uint ENTITY_CODE = 1886044525;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCalendarDate> DateComponent = new("DateComponent", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLocalTime> TimeComponent = new("TimeComponent", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ DateComponent, TimeComponent ];
}

public partial class IfcDefinedSymbol
   : IfcGeometricRepresentationItem
{
    public static IfcDefinedSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDEFINEDSYMBOL"u8;
    public const uint ENTITY_CODE = 1355536236;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDefinedSymbolSelect> Definition = new("Definition", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcCartesianTransformationOperator2D> Target = new("Target", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Definition, Target ];
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
    public override IfcAttribute[] Attributes => [ Elements, UnitType, UserDefinedType ];
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

public partial class IfcDiameterDimension
   : IfcDimensionCurveDirectedCallout
{
    public static IfcDiameterDimension Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIAMETERDIMENSION"u8;
    public const uint ENTITY_CODE = 3776486108;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Contents ];
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

public partial class IfcDimensionCalloutRelationship
   : IfcDraughtingCalloutRelationship
{
    public static IfcDimensionCalloutRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIMENSIONCALLOUTRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 968342173;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingDraughtingCallout, RelatedDraughtingCallout ];
}

public partial class IfcDimensionCurve
   : IfcAnnotationCurveOccurrence
{
    public static IfcDimensionCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIMENSIONCURVE"u8;
    public const uint ENTITY_CODE = 2447790904;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
}

public partial class IfcDimensionCurveDirectedCallout
   : IfcDraughtingCallout
{
    public static IfcDimensionCurveDirectedCallout Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIMENSIONCURVEDIRECTEDCALLOUT"u8;
    public const uint ENTITY_CODE = 2485287940;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Contents ];
}

public partial class IfcDimensionCurveTerminator
   : IfcTerminatorSymbol
{
    public static IfcDimensionCurveTerminator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIMENSIONCURVETERMINATOR"u8;
    public const uint ENTITY_CODE = 3746356277;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDimensionExtentUsage> Role = new("Role", 4, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Item, Styles, Name, AnnotatedCurve, Role ];
}

public partial class IfcDimensionPair
   : IfcDraughtingCalloutRelationship
{
    public static IfcDimensionPair Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIMENSIONPAIR"u8;
    public const uint ENTITY_CODE = 3267798661;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingDraughtingCallout, RelatedDraughtingCallout ];
}

public partial class IfcDirection
   : IfcGeometricRepresentationItem, IfcOrientationSelect, IfcVectorOrDirection
{
    public static IfcDirection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDIRECTION"u8;
    public const uint ENTITY_CODE = 1116762488;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<REAL> DirectionRatios = new("DirectionRatios", 0, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ DirectionRatios ];
}

public partial class IfcDiscreteAccessory
   : IfcElementComponent
{
    public static IfcDiscreteAccessory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISCRETEACCESSORY"u8;
    public const uint ENTITY_CODE = 1020050154;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcDiscreteAccessoryType
   : IfcElementComponentType
{
    public static IfcDiscreteAccessoryType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISCRETEACCESSORYTYPE"u8;
    public const uint ENTITY_CODE = 1499596874;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcDistributionChamberElement
   : IfcDistributionFlowElement
{
    public static IfcDistributionChamberElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCHAMBERELEMENT"u8;
    public const uint ENTITY_CODE = 1690940191;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
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

public partial class IfcDistributionControlElement
   : IfcDistributionElement
{
    public static IfcDistributionControlElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDISTRIBUTIONCONTROLELEMENT"u8;
    public const uint ENTITY_CODE = 1571819994;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> ControlElementId = new("ControlElementId", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, ControlElementId ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, FlowDirection ];
}

public partial class IfcDocumentElectronicFormat
   : EntityBaseClass
{
    public static IfcDocumentElectronicFormat Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTELECTRONICFORMAT"u8;
    public const uint ENTITY_CODE = 492243915;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> FileExtension = new("FileExtension", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> MimeContentType = new("MimeContentType", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> MimeSubtype = new("MimeSubtype", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ FileExtension, MimeContentType, MimeSubtype ];
}

public partial class IfcDocumentInformation
   : EntityBaseClass, IfcDocumentSelect
{
    public static IfcDocumentInformation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTINFORMATION"u8;
    public const uint ENTITY_CODE = 1365583644;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> DocumentId = new("DocumentId", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDocumentReference> DocumentReferences = new("DocumentReferences", 3, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcText> Purpose = new("Purpose", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> IntendedUse = new("IntendedUse", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Scope = new("Scope", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Revision = new("Revision", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorSelect> DocumentOwner = new("DocumentOwner", 8, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcActorSelect> Editors = new("Editors", 9, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcDateAndTime> CreationTime = new("CreationTime", 10, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDateAndTime> LastRevisionTime = new("LastRevisionTime", 11, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDocumentElectronicFormat> ElectronicFormat = new("ElectronicFormat", 12, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCalendarDate> ValidFrom = new("ValidFrom", 13, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCalendarDate> ValidUntil = new("ValidUntil", 14, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDocumentConfidentialityEnum> Confidentiality = new("Confidentiality", 15, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDocumentStatusEnum> Status = new("Status", 16, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ DocumentId, Name, Description, DocumentReferences, Purpose, IntendedUse, Scope, Revision, DocumentOwner, Editors, CreationTime, LastRevisionTime, ElectronicFormat, ValidFrom, ValidUntil, Confidentiality, Status ];
}

public partial class IfcDocumentInformationRelationship
   : EntityBaseClass
{
    public static IfcDocumentInformationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTINFORMATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3622737906;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDocumentInformation> RelatingDocument = new("RelatingDocument", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDocumentInformation> RelatedDocuments = new("RelatedDocuments", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> RelationshipType = new("RelationshipType", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RelatingDocument, RelatedDocuments, RelationshipType ];
}

public partial class IfcDocumentReference
   : IfcExternalReference, IfcDocumentSelect
{
    public static IfcDocumentReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOCUMENTREFERENCE"u8;
    public const uint ENTITY_CODE = 1468122623;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
}

public partial class IfcDoor
   : IfcBuildingElement
{
    public static IfcDoor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOOR"u8;
    public const uint ENTITY_CODE = 656740791;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallHeight = new("OverallHeight", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallWidth = new("OverallWidth", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, OverallHeight, OverallWidth ];
}

public partial class IfcDoorLiningProperties
   : IfcPropertySetDefinition
{
    public static IfcDoorLiningProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOORLININGPROPERTIES"u8;
    public const uint ENTITY_CODE = 3739251787;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LiningDepth = new("LiningDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LiningThickness = new("LiningThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ThresholdDepth = new("ThresholdDepth", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> ThresholdThickness = new("ThresholdThickness", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TransomThickness = new("TransomThickness", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> TransomOffset = new("TransomOffset", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> LiningOffset = new("LiningOffset", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ThresholdOffset = new("ThresholdOffset", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CasingThickness = new("CasingThickness", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CasingDepth = new("CasingDepth", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 14, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, LiningDepth, LiningThickness, ThresholdDepth, ThresholdThickness, TransomThickness, TransomOffset, LiningOffset, ThresholdOffset, CasingThickness, CasingDepth, ShapeAspectStyle ];
}

public partial class IfcDoorPanelProperties
   : IfcPropertySetDefinition
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

public partial class IfcDoorStyle
   : IfcTypeProduct
{
    public static IfcDoorStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDOORSTYLE"u8;
    public const uint ENTITY_CODE = 3325682600;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDoorStyleOperationEnum> OperationType = new("OperationType", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDoorStyleConstructionEnum> ConstructionType = new("ConstructionType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<BOOLEAN> ParameterTakesPrecedence = new("ParameterTakesPrecedence", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> Sizeable = new("Sizeable", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, OperationType, ConstructionType, ParameterTakesPrecedence, Sizeable ];
}

public partial class IfcDraughtingCallout
   : IfcGeometricRepresentationItem
{
    public static IfcDraughtingCallout Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDRAUGHTINGCALLOUT"u8;
    public const uint ENTITY_CODE = 3259969064;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDraughtingCalloutElement> Contents = new("Contents", 0, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Contents ];
}

public partial class IfcDraughtingCalloutRelationship
   : EntityBaseClass
{
    public static IfcDraughtingCalloutRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDRAUGHTINGCALLOUTRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1427322582;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDraughtingCallout> RelatingDraughtingCallout = new("RelatingDraughtingCallout", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDraughtingCallout> RelatedDraughtingCallout = new("RelatedDraughtingCallout", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, RelatingDraughtingCallout, RelatedDraughtingCallout ];
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

public partial class IfcDraughtingPreDefinedTextFont
   : IfcPreDefinedTextFont
{
    public static IfcDraughtingPreDefinedTextFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCDRAUGHTINGPREDEFINEDTEXTFONT"u8;
    public const uint ENTITY_CODE = 2897732460;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
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
    public readonly IfcAttribute<BOOLEAN> SameSense = new("SameSense", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ EdgeStart, EdgeEnd, EdgeGeometry, SameSense ];
}

public partial class IfcEdgeFeature
   : IfcFeatureElementSubtraction
{
    public static IfcEdgeFeature Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEDGEFEATURE"u8;
    public const uint ENTITY_CODE = 3380921116;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FeatureLength = new("FeatureLength", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, FeatureLength ];
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

public partial class IfcElectricalBaseProperties
   : IfcEnergyProperties
{
    public static IfcElectricalBaseProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICALBASEPROPERTIES"u8;
    public const uint ENTITY_CODE = 816185201;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricCurrentEnum> ElectricCurrentType = new("ElectricCurrentType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcElectricVoltageMeasure> InputVoltage = new("InputVoltage", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcFrequencyMeasure> InputFrequency = new("InputFrequency", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcElectricCurrentMeasure> FullLoadCurrent = new("FullLoadCurrent", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcElectricCurrentMeasure> MinimumCircuitCurrent = new("MinimumCircuitCurrent", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPowerMeasure> MaximumPowerInput = new("MaximumPowerInput", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPowerMeasure> RatedPowerInput = new("RatedPowerInput", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> InputPhase = new("InputPhase", 13, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, EnergySequence, UserDefinedEnergySequence, ElectricCurrentType, InputVoltage, InputFrequency, FullLoadCurrent, MinimumCircuitCurrent, MaximumPowerInput, RatedPowerInput, InputPhase ];
}

public partial class IfcElectricalCircuit
   : IfcSystem
{
    public static IfcElectricalCircuit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICALCIRCUIT"u8;
    public const uint ENTITY_CODE = 1137620120;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcElectricalElement
   : IfcElement
{
    public static IfcElectricalElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICALELEMENT"u8;
    public const uint ENTITY_CODE = 2832027715;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
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

public partial class IfcElectricDistributionPoint
   : IfcFlowController
{
    public static IfcElectricDistributionPoint Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICDISTRIBUTIONPOINT"u8;
    public const uint ENTITY_CODE = 977534272;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricDistributionPointFunctionEnum> DistributionPointFunction = new("DistributionPointFunction", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedFunction = new("UserDefinedFunction", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, DistributionPointFunction, UserDefinedFunction ];
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

public partial class IfcElectricHeaterType
   : IfcFlowTerminalType
{
    public static IfcElectricHeaterType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCELECTRICHEATERTYPE"u8;
    public const uint ENTITY_CODE = 3768045537;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElectricHeaterTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
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
   : IfcProduct, IfcStructuralActivityAssignmentSelect
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
   : IfcPropertySetDefinition
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

public partial class IfcEnergyProperties
   : IfcPropertySetDefinition
{
    public static IfcEnergyProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCENERGYPROPERTIES"u8;
    public const uint ENTITY_CODE = 191563778;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcEnergySequenceEnum> EnergySequence = new("EnergySequence", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedEnergySequence = new("UserDefinedEnergySequence", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, EnergySequence, UserDefinedEnergySequence ];
}

public partial class IfcEnvironmentalImpactValue
   : IfcAppliedValue
{
    public static IfcEnvironmentalImpactValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCENVIRONMENTALIMPACTVALUE"u8;
    public const uint ENTITY_CODE = 2523616012;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ImpactType = new("ImpactType", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcEnvironmentalImpactCategoryEnum> Category = new("Category", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedCategory = new("UserDefinedCategory", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, AppliedValue, UnitBasis, ApplicableDate, FixedUntilDate, ImpactType, Category, UserDefinedCategory ];
}

public partial class IfcEquipmentElement
   : IfcElement
{
    public static IfcEquipmentElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEQUIPMENTELEMENT"u8;
    public const uint ENTITY_CODE = 1905357187;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcEquipmentStandard
   : IfcControl
{
    public static IfcEquipmentStandard Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEQUIPMENTSTANDARD"u8;
    public const uint ENTITY_CODE = 3662020580;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
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

public partial class IfcExtendedMaterialProperties
   : IfcMaterialProperties
{
    public static IfcExtendedMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTENDEDMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 3977052170;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProperty> ExtendedProperties = new("ExtendedProperties", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, ExtendedProperties, Description, Name ];
}

public partial class IfcExternallyDefinedHatchStyle
   : IfcExternalReference, IfcFillStyleSelect
{
    public static IfcExternallyDefinedHatchStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDHATCHSTYLE"u8;
    public const uint ENTITY_CODE = 1389487359;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
}

public partial class IfcExternallyDefinedSurfaceStyle
   : IfcExternalReference, IfcSurfaceStyleElementSelect
{
    public static IfcExternallyDefinedSurfaceStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDSURFACESTYLE"u8;
    public const uint ENTITY_CODE = 1184975984;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
}

public partial class IfcExternallyDefinedSymbol
   : IfcExternalReference, IfcDefinedSymbolSelect
{
    public static IfcExternallyDefinedSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDSYMBOL"u8;
    public const uint ENTITY_CODE = 2737105744;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
}

public partial class IfcExternallyDefinedTextFont
   : IfcExternalReference, IfcTextFontSelect
{
    public static IfcExternallyDefinedTextFont Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALLYDEFINEDTEXTFONT"u8;
    public const uint ENTITY_CODE = 4127842378;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
}

public partial class IfcExternalReference
   : EntityBaseClass, IfcLightDistributionDataSourceSelect, IfcObjectReferenceSelect
{
    public static IfcExternalReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCEXTERNALREFERENCE"u8;
    public const uint ENTITY_CODE = 2775413369;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Location = new("Location", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIdentifier> ItemReference = new("ItemReference", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
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
    public readonly IfcAttribute<BOOLEAN> Orientation = new("Orientation", 1, IfcTypeKind.Alias, 0);
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
    public readonly IfcAttribute<BOOLEAN> SameSense = new("SameSense", 2, IfcTypeKind.Alias, 0);
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
   : IfcManifoldSolidBrep
{
    public static IfcFacetedBrepWithVoids Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFACETEDBREPWITHVOIDS"u8;
    public const uint ENTITY_CODE = 712432441;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClosedShell> Voids = new("Voids", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Outer, Voids ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcFastenerType
   : IfcElementComponentType
{
    public static IfcFastenerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFASTENERTYPE"u8;
    public const uint ENTITY_CODE = 4273197281;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
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
   : IfcPresentationStyle, IfcPresentationStyleSelect
{
    public static IfcFillAreaStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILLAREASTYLE"u8;
    public const uint ENTITY_CODE = 1860673172;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFillStyleSelect> FillStyles = new("FillStyles", 1, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Name, FillStyles ];
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
    public readonly IfcAttribute<IfcOneDirectionRepeatFactor> TilingPattern = new("TilingPattern", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcFillAreaStyleTileShapeSelect> Tiles = new("Tiles", 1, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> TilingScale = new("TilingScale", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ TilingPattern, Tiles, TilingScale ];
}

public partial class IfcFillAreaStyleTileSymbolWithStyle
   : IfcGeometricRepresentationItem, IfcFillAreaStyleTileShapeSelect
{
    public static IfcFillAreaStyleTileSymbolWithStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFILLAREASTYLETILESYMBOLWITHSTYLE"u8;
    public const uint ENTITY_CODE = 3211913697;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAnnotationSymbolOccurrence> Symbol = new("Symbol", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Symbol ];
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

public partial class IfcFluidFlowProperties
   : IfcPropertySetDefinition
{
    public static IfcFluidFlowProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFLUIDFLOWPROPERTIES"u8;
    public const uint ENTITY_CODE = 4008729092;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPropertySourceEnum> PropertySource = new("PropertySource", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcTimeSeries> FlowConditionTimeSeries = new("FlowConditionTimeSeries", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTimeSeries> VelocityTimeSeries = new("VelocityTimeSeries", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTimeSeries> FlowrateTimeSeries = new("FlowrateTimeSeries", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcMaterial> Fluid = new("Fluid", 8, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTimeSeries> PressureTimeSeries = new("PressureTimeSeries", 9, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedPropertySource = new("UserDefinedPropertySource", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> TemperatureSingleValue = new("TemperatureSingleValue", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> WetBulbTemperatureSingleValue = new("WetBulbTemperatureSingleValue", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeSeries> WetBulbTemperatureTimeSeries = new("WetBulbTemperatureTimeSeries", 13, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcTimeSeries> TemperatureTimeSeries = new("TemperatureTimeSeries", 14, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDerivedMeasureValue> FlowrateSingleValue = new("FlowrateSingleValue", 15, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> FlowConditionSingleValue = new("FlowConditionSingleValue", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLinearVelocityMeasure> VelocitySingleValue = new("VelocitySingleValue", 17, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPressureMeasure> PressureSingleValue = new("PressureSingleValue", 18, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, PropertySource, FlowConditionTimeSeries, VelocityTimeSeries, FlowrateTimeSeries, Fluid, PressureTimeSeries, UserDefinedPropertySource, TemperatureSingleValue, WetBulbTemperatureSingleValue, WetBulbTemperatureTimeSeries, TemperatureTimeSeries, FlowrateSingleValue, FlowConditionSingleValue, VelocitySingleValue, PressureSingleValue ];
}

public partial class IfcFooting
   : IfcBuildingElement
{
    public static IfcFooting Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFOOTING"u8;
    public const uint ENTITY_CODE = 1345078513;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcFootingTypeEnum> PredefinedType = new("PredefinedType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, PredefinedType ];
}

public partial class IfcFuelProperties
   : IfcMaterialProperties
{
    public static IfcFuelProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFUELPROPERTIES"u8;
    public const uint ENTITY_CODE = 3476639258;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> CombustionTemperature = new("CombustionTemperature", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> CarbonContent = new("CarbonContent", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcHeatingValueMeasure> LowerHeatingValue = new("LowerHeatingValue", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcHeatingValueMeasure> HigherHeatingValue = new("HigherHeatingValue", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, CombustionTemperature, CarbonContent, LowerHeatingValue, HigherHeatingValue ];
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

public partial class IfcFurnitureStandard
   : IfcControl
{
    public static IfcFurnitureStandard Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCFURNITURESTANDARD"u8;
    public const uint ENTITY_CODE = 1963718002;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, AssemblyPlace ];
}

public partial class IfcGasTerminalType
   : IfcFlowTerminalType
{
    public static IfcGasTerminalType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGASTERMINALTYPE"u8;
    public const uint ENTITY_CODE = 3688787704;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGasTerminalTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcGeneralMaterialProperties
   : IfcMaterialProperties
{
    public static IfcGeneralMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGENERALMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 3227254261;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMolecularWeightMeasure> MolecularWeight = new("MolecularWeight", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Porosity = new("Porosity", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMassDensityMeasure> MassDensity = new("MassDensity", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, MolecularWeight, Porosity, MassDensity ];
}

public partial class IfcGeneralProfileProperties
   : IfcProfileProperties
{
    public static IfcGeneralProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGENERALPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 2051784107;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMassPerLengthMeasure> PhysicalWeight = new("PhysicalWeight", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Perimeter = new("Perimeter", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MinimumPlateThickness = new("MinimumPlateThickness", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MaximumPlateThickness = new("MaximumPlateThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> CrossSectionArea = new("CrossSectionArea", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileName, ProfileDefinition, PhysicalWeight, Perimeter, MinimumPlateThickness, MaximumPlateThickness, CrossSectionArea ];
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
   : IfcRepresentationContext
{
    public static IfcGeometricRepresentationContext Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGEOMETRICREPRESENTATIONCONTEXT"u8;
    public const uint ENTITY_CODE = 1928810440;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDimensionCount> CoordinateSpaceDimension = new("CoordinateSpaceDimension", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<REAL> Precision = new("Precision", 3, IfcTypeKind.Alias, 0);
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

public partial class IfcGrid
   : IfcProduct
{
    public static IfcGrid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCGRID"u8;
    public const uint ENTITY_CODE = 2792790963;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcGridAxis> UAxes = new("UAxes", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcGridAxis> VAxes = new("VAxes", 8, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcGridAxis> WAxes = new("WAxes", 9, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, UAxes, VAxes, WAxes ];
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
    public readonly IfcAttribute<IfcVirtualGridIntersection> PlacementLocation = new("PlacementLocation", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcVirtualGridIntersection> PlacementRefDirection = new("PlacementRefDirection", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ PlacementLocation, PlacementRefDirection ];
}

public partial class IfcGroup
   : IfcObject
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
    public readonly IfcAttribute<BOOLEAN> AgreementFlag = new("AgreementFlag", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BaseSurface, AgreementFlag ];
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

public partial class IfcHygroscopicMaterialProperties
   : IfcMaterialProperties
{
    public static IfcHygroscopicMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCHYGROSCOPICMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 3896066615;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveRatioMeasure> UpperVaporResistanceFactor = new("UpperVaporResistanceFactor", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> LowerVaporResistanceFactor = new("LowerVaporResistanceFactor", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIsothermalMoistureCapacityMeasure> IsothermalMoistureCapacity = new("IsothermalMoistureCapacity", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcVaporPermeabilityMeasure> VaporPermeability = new("VaporPermeability", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMoistureDiffusivityMeasure> MoistureDiffusivity = new("MoistureDiffusivity", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, UpperVaporResistanceFactor, LowerVaporResistanceFactor, IsothermalMoistureCapacity, VaporPermeability, MoistureDiffusivity ];
}

public partial class IfcImageTexture
   : IfcSurfaceTexture
{
    public static IfcImageTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCIMAGETEXTURE"u8;
    public const uint ENTITY_CODE = 582144863;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> UrlReference = new("UrlReference", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, TextureType, TextureTransform, UrlReference ];
}

public partial class IfcInventory
   : IfcGroup
{
    public static IfcInventory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCINVENTORY"u8;
    public const uint ENTITY_CODE = 3189971553;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInventoryTypeEnum> InventoryType = new("InventoryType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcActorSelect> Jurisdiction = new("Jurisdiction", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPerson> ResponsiblePersons = new("ResponsiblePersons", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcCalendarDate> LastUpdateDate = new("LastUpdateDate", 8, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> CurrentValue = new("CurrentValue", 9, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCostValue> OriginalValue = new("OriginalValue", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, InventoryType, Jurisdiction, ResponsiblePersons, LastUpdateDate, CurrentValue, OriginalValue ];
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
    public readonly IfcAttribute<IfcDateTimeSelect> TimeStamp = new("TimeStamp", 0, IfcTypeKind.Unknown, 0);
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, OverallWidth, OverallDepth, WebThickness, FlangeThickness, FilletRadius ];
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

public partial class IfcLaborResource
   : IfcConstructionResource
{
    public static IfcLaborResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLABORRESOURCE"u8;
    public const uint ENTITY_CODE = 1950317855;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcText> SkillSet = new("SkillSet", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity, SkillSet ];
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
   : EntityBaseClass, IfcLibrarySelect
{
    public static IfcLibraryInformation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIBRARYINFORMATION"u8;
    public const uint ENTITY_CODE = 368329652;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Version = new("Version", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcOrganization> Publisher = new("Publisher", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCalendarDate> VersionDate = new("VersionDate", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLibraryReference> LibraryReference = new("LibraryReference", 4, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Version, Publisher, VersionDate, LibraryReference ];
}

public partial class IfcLibraryReference
   : IfcExternalReference, IfcLibrarySelect
{
    public static IfcLibraryReference Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLIBRARYREFERENCE"u8;
    public const uint ENTITY_CODE = 4036302135;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Location, ItemReference, Name ];
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

public partial class IfcLinearDimension
   : IfcDimensionCurveDirectedCallout
{
    public static IfcLinearDimension Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLINEARDIMENSION"u8;
    public const uint ENTITY_CODE = 785776346;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Contents ];
}

public partial class IfcLocalPlacement
   : IfcObjectPlacement
{
    public static IfcLocalPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLOCALPLACEMENT"u8;
    public const uint ENTITY_CODE = 4159386377;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectPlacement> PlacementRelTo = new("PlacementRelTo", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAxis2Placement> RelativePlacement = new("RelativePlacement", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ PlacementRelTo, RelativePlacement ];
}

public partial class IfcLocalTime
   : EntityBaseClass, IfcDateTimeSelect, IfcObjectReferenceSelect
{
    public static IfcLocalTime Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCLOCALTIME"u8;
    public const uint ENTITY_CODE = 318145335;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcHourInDay> HourComponent = new("HourComponent", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMinuteInHour> MinuteComponent = new("MinuteComponent", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSecondInMinute> SecondComponent = new("SecondComponent", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCoordinatedUniversalTimeOffset> Zone = new("Zone", 3, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDaylightSavingHour> DaylightSavingOffset = new("DaylightSavingOffset", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ HourComponent, MinuteComponent, SecondComponent, Zone, DaylightSavingOffset ];
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FilletRadius = new("FilletRadius", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> EdgeRadius = new("EdgeRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> LegSlope = new("LegSlope", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInX = new("CentreOfGravityInX", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInY = new("CentreOfGravityInY", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, Width, Thickness, FilletRadius, EdgeRadius, LegSlope, CentreOfGravityInX, CentreOfGravityInY ];
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

public partial class IfcMaterial
   : EntityBaseClass, IfcMaterialSelect, IfcObjectReferenceSelect
{
    public static IfcMaterial Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIAL"u8;
    public const uint ENTITY_CODE = 1595842790;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcMaterialClassificationRelationship
   : EntityBaseClass
{
    public static IfcMaterialClassificationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALCLASSIFICATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1549328080;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcClassificationNotationSelect> MaterialClassifications = new("MaterialClassifications", 0, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcMaterial> ClassifiedMaterial = new("ClassifiedMaterial", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ MaterialClassifications, ClassifiedMaterial ];
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
   : EntityBaseClass, IfcMaterialSelect, IfcObjectReferenceSelect
{
    public static IfcMaterialLayer Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLAYER"u8;
    public const uint ENTITY_CODE = 3348622987;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> Material = new("Material", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LayerThickness = new("LayerThickness", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLogical> IsVentilated = new("IsVentilated", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, LayerThickness, IsVentilated ];
}

public partial class IfcMaterialLayerSet
   : EntityBaseClass, IfcMaterialSelect
{
    public static IfcMaterialLayerSet Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLAYERSET"u8;
    public const uint ENTITY_CODE = 104809689;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterialLayer> MaterialLayers = new("MaterialLayers", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> LayerSetName = new("LayerSetName", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ MaterialLayers, LayerSetName ];
}

public partial class IfcMaterialLayerSetUsage
   : EntityBaseClass, IfcMaterialSelect
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
    public override IfcAttribute[] Attributes => [ ForLayerSet, LayerSetDirection, DirectionSense, OffsetFromReferenceLine ];
}

public partial class IfcMaterialList
   : EntityBaseClass, IfcMaterialSelect, IfcObjectReferenceSelect
{
    public static IfcMaterialList Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALLIST"u8;
    public const uint ENTITY_CODE = 2456039154;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> Materials = new("Materials", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Materials ];
}

public partial class IfcMaterialProperties
   : EntityBaseClass
{
    public static IfcMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 195900019;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMaterial> Material = new("Material", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Material ];
}

public partial class IfcMeasureWithUnit
   : EntityBaseClass, IfcAppliedValueSelect, IfcConditionCriterionSelect, IfcMetricValueSelect
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

public partial class IfcMechanicalConcreteMaterialProperties
   : IfcMechanicalMaterialProperties
{
    public static IfcMechanicalConcreteMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALCONCRETEMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 3024874671;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPressureMeasure> CompressiveStrength = new("CompressiveStrength", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MaxAggregateSize = new("MaxAggregateSize", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> AdmixturesDescription = new("AdmixturesDescription", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Workability = new("Workability", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> ProtectivePoreRatio = new("ProtectivePoreRatio", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> WaterImpermeability = new("WaterImpermeability", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, DynamicViscosity, YoungModulus, ShearModulus, PoissonRatio, ThermalExpansionCoefficient, CompressiveStrength, MaxAggregateSize, AdmixturesDescription, Workability, ProtectivePoreRatio, WaterImpermeability ];
}

public partial class IfcMechanicalFastener
   : IfcFastener
{
    public static IfcMechanicalFastener Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALFASTENER"u8;
    public const uint ENTITY_CODE = 747847214;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalDiameter = new("NominalDiameter", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> NominalLength = new("NominalLength", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, NominalDiameter, NominalLength ];
}

public partial class IfcMechanicalFastenerType
   : IfcFastenerType
{
    public static IfcMechanicalFastenerType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALFASTENERTYPE"u8;
    public const uint ENTITY_CODE = 1495427214;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcMechanicalMaterialProperties
   : IfcMaterialProperties
{
    public static IfcMechanicalMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 1998129008;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDynamicViscosityMeasure> DynamicViscosity = new("DynamicViscosity", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfElasticityMeasure> YoungModulus = new("YoungModulus", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfElasticityMeasure> ShearModulus = new("ShearModulus", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> PoissonRatio = new("PoissonRatio", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermalExpansionCoefficientMeasure> ThermalExpansionCoefficient = new("ThermalExpansionCoefficient", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, DynamicViscosity, YoungModulus, ShearModulus, PoissonRatio, ThermalExpansionCoefficient ];
}

public partial class IfcMechanicalSteelMaterialProperties
   : IfcMechanicalMaterialProperties
{
    public static IfcMechanicalSteelMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMECHANICALSTEELMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 3938803731;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPressureMeasure> YieldStress = new("YieldStress", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPressureMeasure> UltimateStress = new("UltimateStress", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> UltimateStrain = new("UltimateStrain", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcModulusOfElasticityMeasure> HardeningModule = new("HardeningModule", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPressureMeasure> ProportionalStress = new("ProportionalStress", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> PlasticStrain = new("PlasticStrain", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRelaxation> Relaxations = new("Relaxations", 12, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Material, DynamicViscosity, YoungModulus, ShearModulus, PoissonRatio, ThermalExpansionCoefficient, YieldStress, UltimateStress, UltimateStrain, HardeningModule, ProportionalStress, PlasticStrain, Relaxations ];
}

public partial class IfcMember
   : IfcBuildingElement
{
    public static IfcMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMEMBER"u8;
    public const uint ENTITY_CODE = 1985401597;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcMemberType
   : IfcBuildingElementType
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
    public override IfcAttribute[] Attributes => [ Name, Description, ConstraintGrade, ConstraintSource, CreatingActor, CreationTime, UserDefinedGrade, Benchmark, ValueSource, DataValue ];
}

public partial class IfcMonetaryUnit
   : EntityBaseClass, IfcUnit
{
    public static IfcMonetaryUnit Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMONETARYUNIT"u8;
    public const uint ENTITY_CODE = 4053228418;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurrencyEnum> Currency = new("Currency", 0, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ Currency ];
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

public partial class IfcMove
   : IfcTask
{
    public static IfcMove Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCMOVE"u8;
    public const uint ENTITY_CODE = 181880182;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpatialStructureElement> MoveFrom = new("MoveFrom", 10, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSpatialStructureElement> MoveTo = new("MoveTo", 11, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcText> PunchList = new("PunchList", 12, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TaskId, Status, WorkMethod, IsMilestone, Priority, MoveFrom, MoveTo, PunchList ];
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
   : IfcRoot
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
    public readonly IfcAttribute<IfcMetric> BenchmarkValues = new("BenchmarkValues", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcMetric> ResultValues = new("ResultValues", 8, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcObjectiveEnum> ObjectiveQualifier = new("ObjectiveQualifier", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedQualifier = new("UserDefinedQualifier", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, ConstraintGrade, ConstraintSource, CreatingActor, CreationTime, UserDefinedGrade, BenchmarkValues, ResultValues, ObjectiveQualifier, UserDefinedQualifier ];
}

public partial class IfcObjectPlacement
   : EntityBaseClass
{
    public static IfcObjectPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOBJECTPLACEMENT"u8;
    public const uint ENTITY_CODE = 3325497275;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
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

public partial class IfcOffsetCurve2D
   : IfcCurve
{
    public static IfcOffsetCurve2D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOFFSETCURVE2D"u8;
    public const uint ENTITY_CODE = 542883257;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> BasisCurve = new("BasisCurve", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> Distance = new("Distance", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<LOGICAL> SelfIntersect = new("SelfIntersect", 2, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, Distance, SelfIntersect ];
}

public partial class IfcOffsetCurve3D
   : IfcCurve
{
    public static IfcOffsetCurve3D Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOFFSETCURVE3D"u8;
    public const uint ENTITY_CODE = 2052721872;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> BasisCurve = new("BasisCurve", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLengthMeasure> Distance = new("Distance", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<LOGICAL> SelfIntersect = new("SelfIntersect", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDirection> RefDirection = new("RefDirection", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ BasisCurve, Distance, SelfIntersect, RefDirection ];
}

public partial class IfcOneDirectionRepeatFactor
   : IfcGeometricRepresentationItem, IfcHatchLineDistanceSelect
{
    public static IfcOneDirectionRepeatFactor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCONEDIRECTIONREPEATFACTOR"u8;
    public const uint ENTITY_CODE = 1975721390;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVector> RepeatFactor = new("RepeatFactor", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ RepeatFactor ];
}

public partial class IfcOpeningElement
   : IfcFeatureElementSubtraction
{
    public static IfcOpeningElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOPENINGELEMENT"u8;
    public const uint ENTITY_CODE = 1554121831;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
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

public partial class IfcOpticalMaterialProperties
   : IfcMaterialProperties
{
    public static IfcOpticalMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCOPTICALMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 2847962057;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveRatioMeasure> VisibleTransmittance = new("VisibleTransmittance", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> SolarTransmittance = new("SolarTransmittance", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ThermalIrTransmittance = new("ThermalIrTransmittance", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ThermalIrEmissivityBack = new("ThermalIrEmissivityBack", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ThermalIrEmissivityFront = new("ThermalIrEmissivityFront", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> VisibleReflectanceBack = new("VisibleReflectanceBack", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> VisibleReflectanceFront = new("VisibleReflectanceFront", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> SolarReflectanceFront = new("SolarReflectanceFront", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> SolarReflectanceBack = new("SolarReflectanceBack", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, VisibleTransmittance, SolarTransmittance, ThermalIrTransmittance, ThermalIrEmissivityBack, ThermalIrEmissivityFront, VisibleReflectanceBack, VisibleReflectanceFront, SolarReflectanceFront, SolarReflectanceBack ];
}

public partial class IfcOrderAction
   : IfcTask
{
    public static IfcOrderAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCORDERACTION"u8;
    public const uint ENTITY_CODE = 96870179;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> ActionID = new("ActionID", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TaskId, Status, WorkMethod, IsMilestone, Priority, ActionID ];
}

public partial class IfcOrganization
   : EntityBaseClass, IfcActorSelect, IfcObjectReferenceSelect
{
    public static IfcOrganization Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCORGANIZATION"u8;
    public const uint ENTITY_CODE = 321185184;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Id = new("Id", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcActorRole> Roles = new("Roles", 3, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcAddress> Addresses = new("Addresses", 4, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Id, Name, Description, Roles, Addresses ];
}

public partial class IfcOrganizationRelationship
   : EntityBaseClass
{
    public static IfcOrganizationRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCORGANIZATIONRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 1147128302;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
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
    public readonly IfcAttribute<BOOLEAN> Orientation = new("Orientation", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ EdgeStart, EdgeEnd, EdgeElement, Orientation ];
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

public partial class IfcPerformanceHistory
   : IfcControl
{
    public static IfcPerformanceHistory Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERFORMANCEHISTORY"u8;
    public const uint ENTITY_CODE = 164555693;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LifeCyclePhase = new("LifeCyclePhase", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LifeCyclePhase ];
}

public partial class IfcPermeableCoveringProperties
   : IfcPropertySetDefinition
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
    public readonly IfcAttribute<IfcIdentifier> PermitID = new("PermitID", 5, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PermitID ];
}

public partial class IfcPerson
   : EntityBaseClass, IfcActorSelect, IfcObjectReferenceSelect
{
    public static IfcPerson Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPERSON"u8;
    public const uint ENTITY_CODE = 1697060002;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Id = new("Id", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> FamilyName = new("FamilyName", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> GivenName = new("GivenName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> MiddleNames = new("MiddleNames", 3, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> PrefixTitles = new("PrefixTitles", 4, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcLabel> SuffixTitles = new("SuffixTitles", 5, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcActorRole> Roles = new("Roles", 6, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcAddress> Addresses = new("Addresses", 7, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Id, FamilyName, GivenName, MiddleNames, PrefixTitles, SuffixTitles, Roles, Addresses ];
}

public partial class IfcPersonAndOrganization
   : EntityBaseClass, IfcActorSelect, IfcObjectReferenceSelect
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
   : EntityBaseClass
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
   : IfcBuildingElement
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
    public readonly IfcAttribute<IfcInteger> Width = new("Width", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> Height = new("Height", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcInteger> ColourComponents = new("ColourComponents", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BINARY> Pixel = new("Pixel", 7, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, TextureType, TextureTransform, Width, Height, ColourComponents, Pixel ];
}

public partial class IfcPlacement
   : IfcGeometricRepresentationItem
{
    public static IfcPlacement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLACEMENT"u8;
    public const uint ENTITY_CODE = 184181550;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCartesianPoint> Location = new("Location", 0, IfcTypeKind.Entity, 0);
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
   : IfcBuildingElement
{
    public static IfcPlate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPLATE"u8;
    public const uint ENTITY_CODE = 3954996169;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcPlateType
   : IfcBuildingElementType
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

public partial class IfcPreDefinedDimensionSymbol
   : IfcPreDefinedSymbol
{
    public static IfcPreDefinedDimensionSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDDIMENSIONSYMBOL"u8;
    public const uint ENTITY_CODE = 1794636499;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedItem
   : EntityBaseClass
{
    public static IfcPreDefinedItem Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDITEM"u8;
    public const uint ENTITY_CODE = 827041254;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedPointMarkerSymbol
   : IfcPreDefinedSymbol
{
    public static IfcPreDefinedPointMarkerSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDPOINTMARKERSYMBOL"u8;
    public const uint ENTITY_CODE = 4075772547;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedSymbol
   : IfcPreDefinedItem, IfcDefinedSymbolSelect
{
    public static IfcPreDefinedSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDSYMBOL"u8;
    public const uint ENTITY_CODE = 1439335441;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
}

public partial class IfcPreDefinedTerminatorSymbol
   : IfcPreDefinedSymbol
{
    public static IfcPreDefinedTerminatorSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPREDEFINEDTERMINATORSYMBOL"u8;
    public const uint ENTITY_CODE = 3265093620;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name ];
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
    public readonly IfcAttribute<LOGICAL> LayerOn = new("LayerOn", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<LOGICAL> LayerFrozen = new("LayerFrozen", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<LOGICAL> LayerBlocked = new("LayerBlocked", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPresentationStyleSelect> LayerStyles = new("LayerStyles", 7, IfcTypeKind.Unknown, 1);
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

public partial class IfcPresentationStyleAssignment
   : EntityBaseClass
{
    public static IfcPresentationStyleAssignment Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRESENTATIONSTYLEASSIGNMENT"u8;
    public const uint ENTITY_CODE = 2807165169;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPresentationStyleSelect> Styles = new("Styles", 0, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Styles ];
}

public partial class IfcProcedure
   : IfcProcess
{
    public static IfcProcedure Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROCEDURE"u8;
    public const uint ENTITY_CODE = 1774744644;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> ProcedureID = new("ProcedureID", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProcedureTypeEnum> ProcedureType = new("ProcedureType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedProcedureType = new("UserDefinedProcedureType", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ProcedureID, ProcedureType, UserDefinedProcedureType ];
}

public partial class IfcProcess
   : IfcObject
{
    public static IfcProcess Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROCESS"u8;
    public const uint ENTITY_CODE = 1826787596;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
}

public partial class IfcProduct
   : IfcObject
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
   : IfcProductRepresentation
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

public partial class IfcProductsOfCombustionProperties
   : IfcMaterialProperties
{
    public static IfcProductsOfCombustionProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPRODUCTSOFCOMBUSTIONPROPERTIES"u8;
    public const uint ENTITY_CODE = 2809396452;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpecificHeatCapacityMeasure> SpecificHeatCapacity = new("SpecificHeatCapacity", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> N20Content = new("N20Content", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> COContent = new("COContent", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> CO2Content = new("CO2Content", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, SpecificHeatCapacity, N20Content, COContent, CO2Content ];
}

public partial class IfcProfileDef
   : EntityBaseClass
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
   : EntityBaseClass
{
    public static IfcProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 2726116117;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ProfileName = new("ProfileName", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProfileDef> ProfileDefinition = new("ProfileDefinition", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ ProfileName, ProfileDefinition ];
}

public partial class IfcProject
   : IfcObject
{
    public static IfcProject Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECT"u8;
    public const uint ENTITY_CODE = 1439394748;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Phase = new("Phase", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRepresentationContext> RepresentationContexts = new("RepresentationContexts", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcUnitAssignment> UnitsInContext = new("UnitsInContext", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, LongName, Phase, RepresentationContexts, UnitsInContext ];
}

public partial class IfcProjectionCurve
   : IfcAnnotationCurveOccurrence
{
    public static IfcProjectionCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTIONCURVE"u8;
    public const uint ENTITY_CODE = 1040464061;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Item, Styles, Name ];
}

public partial class IfcProjectionElement
   : IfcFeatureElementAddition
{
    public static IfcProjectionElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTIONELEMENT"u8;
    public const uint ENTITY_CODE = 2130597890;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcProjectOrder
   : IfcControl
{
    public static IfcProjectOrder Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTORDER"u8;
    public const uint ENTITY_CODE = 567771124;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> ID = new("ID", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProjectOrderTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ID, PredefinedType, Status ];
}

public partial class IfcProjectOrderRecord
   : IfcControl
{
    public static IfcProjectOrderRecord Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROJECTORDERRECORD"u8;
    public const uint ENTITY_CODE = 3171079713;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRelAssignsToProjectOrder> Records = new("Records", 5, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcProjectOrderRecordTypeEnum> PredefinedType = new("PredefinedType", 6, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Records, PredefinedType ];
}

public partial class IfcProperty
   : EntityBaseClass
{
    public static IfcProperty Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTY"u8;
    public const uint ENTITY_CODE = 3277779118;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, Description ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, UpperBoundValue, LowerBoundValue, Unit ];
}

public partial class IfcPropertyConstraintRelationship
   : EntityBaseClass
{
    public static IfcPropertyConstraintRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYCONSTRAINTRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2596573979;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcConstraint> RelatingConstraint = new("RelatingConstraint", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProperty> RelatedProperties = new("RelatedProperties", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RelatingConstraint, RelatedProperties, Name, Description ];
}

public partial class IfcPropertyDefinition
   : IfcRoot
{
    public static IfcPropertyDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYDEFINITION"u8;
    public const uint ENTITY_CODE = 3334093415;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
}

public partial class IfcPropertyDependencyRelationship
   : EntityBaseClass
{
    public static IfcPropertyDependencyRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYDEPENDENCYRELATIONSHIP"u8;
    public const uint ENTITY_CODE = 2230335753;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProperty> DependingProperty = new("DependingProperty", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcProperty> DependantProperty = new("DependantProperty", 1, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Expression = new("Expression", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ DependingProperty, DependantProperty, Name, Description, Expression ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, EnumerationValues, EnumerationReference ];
}

public partial class IfcPropertyEnumeration
   : EntityBaseClass
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
    public override IfcAttribute[] Attributes => [ Name, Description, ListValues, Unit ];
}

public partial class IfcPropertyReferenceValue
   : IfcSimpleProperty
{
    public static IfcPropertyReferenceValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYREFERENCEVALUE"u8;
    public const uint ENTITY_CODE = 3614615320;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> UsageName = new("UsageName", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcObjectReferenceSelect> PropertyReference = new("PropertyReference", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, UsageName, PropertyReference ];
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
   : IfcPropertyDefinition
{
    public static IfcPropertySetDefinition Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROPERTYSETDEFINITION"u8;
    public const uint ENTITY_CODE = 933111983;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, NominalValue, Unit ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, DefiningValues, DefinedValues, Expression, DefiningUnit, DefinedUnit ];
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

public partial class IfcProxy
   : IfcProduct
{
    public static IfcProxy Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCPROXY"u8;
    public const uint ENTITY_CODE = 1569266921;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectTypeEnum> ProxyType = new("ProxyType", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> Tag = new("Tag", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, ProxyType, Tag ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, AreaValue ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, CountValue ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, LengthValue ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, TimeValue ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, VolumeValue ];
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
    public override IfcAttribute[] Attributes => [ Name, Description, Unit, WeightValue ];
}

public partial class IfcRadiusDimension
   : IfcDimensionCurveDirectedCallout
{
    public static IfcRadiusDimension Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRADIUSDIMENSION"u8;
    public const uint ENTITY_CODE = 2620486155;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Contents ];
}

public partial class IfcRailing
   : IfcBuildingElement
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
   : IfcBuildingElementType
{
    public static IfcRailingType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAILINGTYPE"u8;
    public const uint ENTITY_CODE = 2218968665;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRailingTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRamp
   : IfcBuildingElement
{
    public static IfcRamp Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMP"u8;
    public const uint ENTITY_CODE = 1952768055;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRampTypeEnum> ShapeType = new("ShapeType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, ShapeType ];
}

public partial class IfcRampFlight
   : IfcBuildingElement
{
    public static IfcRampFlight Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMPFLIGHT"u8;
    public const uint ENTITY_CODE = 2713085869;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcRampFlightType
   : IfcBuildingElementType
{
    public static IfcRampFlightType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRAMPFLIGHTTYPE"u8;
    public const uint ENTITY_CODE = 386973029;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRampFlightTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcRationalBezierCurve
   : IfcBezierCurve
{
    public static IfcRationalBezierCurve Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRATIONALBEZIERCURVE"u8;
    public const uint ENTITY_CODE = 2905002373;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<REAL> WeightsData = new("WeightsData", 5, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Degree, ControlPointsList, CurveForm, ClosedCurve, SelfIntersect, WeightsData ];
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> InnerFilletRadius = new("InnerFilletRadius", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OuterFilletRadius = new("OuterFilletRadius", 7, IfcTypeKind.Alias, 0);
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
    public readonly IfcAttribute<BOOLEAN> Usense = new("Usense", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> Vsense = new("Vsense", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ BasisSurface, U1, V1, U2, V2, Usense, Vsense ];
}

public partial class IfcReferencesValueDocument
   : EntityBaseClass
{
    public static IfcReferencesValueDocument Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREFERENCESVALUEDOCUMENT"u8;
    public const uint ENTITY_CODE = 983833551;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDocumentSelect> ReferencedDocument = new("ReferencedDocument", 0, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcAppliedValue> ReferencingValues = new("ReferencingValues", 1, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ReferencedDocument, ReferencingValues, Name, Description ];
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

public partial class IfcReinforcementBarProperties
   : EntityBaseClass
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
   : IfcPropertySetDefinition
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
    public readonly IfcAttribute<IfcReinforcingBarRoleEnum> BarRole = new("BarRole", 12, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcReinforcingBarSurfaceEnum> BarSurface = new("BarSurface", 13, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, NominalDiameter, CrossSectionArea, BarLength, BarRole, BarSurface ];
}

public partial class IfcReinforcingElement
   : IfcBuildingElementComponent
{
    public static IfcReinforcingElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCREINFORCINGELEMENT"u8;
    public const uint ENTITY_CODE = 1403002469;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> SteelGrade = new("SteelGrade", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade, MeshLength, MeshWidth, LongitudinalBarNominalDiameter, TransverseBarNominalDiameter, LongitudinalBarCrossSectionArea, TransverseBarCrossSectionArea, LongitudinalBarSpacing, TransverseBarSpacing ];
}

public partial class IfcRelAggregates
   : IfcRelDecomposes
{
    public static IfcRelAggregates Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELAGGREGATES"u8;
    public const uint ENTITY_CODE = 2084011922;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
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

public partial class IfcRelAssignsTasks
   : IfcRelAssignsToControl
{
    public static IfcRelAssignsTasks Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTASKS"u8;
    public const uint ENTITY_CODE = 2342090142;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcScheduleTimeControl> TimeForTask = new("TimeForTask", 7, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingControl, TimeForTask ];
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

public partial class IfcRelAssignsToProcess
   : IfcRelAssigns
{
    public static IfcRelAssignsToProcess Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOPROCESS"u8;
    public const uint ENTITY_CODE = 2767940218;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProcess> RelatingProcess = new("RelatingProcess", 6, IfcTypeKind.Entity, 0);
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
    public readonly IfcAttribute<IfcProduct> RelatingProduct = new("RelatingProduct", 6, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingProduct ];
}

public partial class IfcRelAssignsToProjectOrder
   : IfcRelAssignsToControl
{
    public static IfcRelAssignsToProjectOrder Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTOPROJECTORDER"u8;
    public const uint ENTITY_CODE = 3697988662;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingControl ];
}

public partial class IfcRelAssignsToResource
   : IfcRelAssigns
{
    public static IfcRelAssignsToResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSIGNSTORESOURCE"u8;
    public const uint ENTITY_CODE = 3183946773;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcResource> RelatingResource = new("RelatingResource", 6, IfcTypeKind.Entity, 0);
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
    public readonly IfcAttribute<IfcRoot> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects ];
}

public partial class IfcRelAssociatesAppliedValue
   : IfcRelAssociates
{
    public static IfcRelAssociatesAppliedValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESAPPLIEDVALUE"u8;
    public const uint ENTITY_CODE = 1745891923;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAppliedValue> RelatingAppliedValue = new("RelatingAppliedValue", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingAppliedValue ];
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
    public readonly IfcAttribute<IfcClassificationNotationSelect> RelatingClassification = new("RelatingClassification", 5, IfcTypeKind.Unknown, 0);
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

public partial class IfcRelAssociatesProfileProperties
   : IfcRelAssociates
{
    public static IfcRelAssociatesProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELASSOCIATESPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 2632608379;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProfileProperties> RelatingProfileProperties = new("RelatingProfileProperties", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcShapeAspect> ProfileSectionLocation = new("ProfileSectionLocation", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcOrientationSelect> ProfileOrientation = new("ProfileOrientation", 7, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingProfileProperties, ProfileSectionLocation, ProfileOrientation ];
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

public partial class IfcRelaxation
   : EntityBaseClass
{
    public static IfcRelaxation Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELAXATION"u8;
    public const uint ENTITY_CODE = 204817604;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> RelaxationValue = new("RelaxationValue", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> InitialStress = new("InitialStress", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RelaxationValue, InitialStress ];
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
    public readonly IfcAttribute<INTEGER> RelatingPriorities = new("RelatingPriorities", 7, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<INTEGER> RelatedPriorities = new("RelatedPriorities", 8, IfcTypeKind.Alias, 1);
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
    public readonly IfcAttribute<IfcElement> RelatedElement = new("RelatedElement", 5, IfcTypeKind.Entity, 0);
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

public partial class IfcRelConnectsStructuralElement
   : IfcRelConnects
{
    public static IfcRelConnectsStructuralElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELCONNECTSSTRUCTURALELEMENT"u8;
    public const uint ENTITY_CODE = 2538422970;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcElement> RelatingElement = new("RelatingElement", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStructuralMember> RelatedStructuralMember = new("RelatedStructuralMember", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingElement, RelatedStructuralMember ];
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
    public readonly IfcAttribute<IfcSpatialStructureElement> RelatingStructure = new("RelatingStructure", 5, IfcTypeKind.Entity, 0);
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
    public readonly IfcAttribute<IfcSpace> RelatedSpace = new("RelatedSpace", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcCovering> RelatedCoverings = new("RelatedCoverings", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedSpace, RelatedCoverings ];
}

public partial class IfcRelDecomposes
   : IfcRelationship
{
    public static IfcRelDecomposes Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDECOMPOSES"u8;
    public const uint ENTITY_CODE = 2447326828;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObjectDefinition> RelatingObject = new("RelatingObject", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcObjectDefinition> RelatedObjects = new("RelatedObjects", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingObject, RelatedObjects ];
}

public partial class IfcRelDefines
   : IfcRelationship
{
    public static IfcRelDefines Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINES"u8;
    public const uint ENTITY_CODE = 1550225206;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcObject> RelatedObjects = new("RelatedObjects", 4, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects ];
}

public partial class IfcRelDefinesByProperties
   : IfcRelDefines
{
    public static IfcRelDefinesByProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINESBYPROPERTIES"u8;
    public const uint ENTITY_CODE = 3293188662;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPropertySetDefinition> RelatingPropertyDefinition = new("RelatingPropertyDefinition", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingPropertyDefinition ];
}

public partial class IfcRelDefinesByType
   : IfcRelDefines
{
    public static IfcRelDefinesByType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELDEFINESBYTYPE"u8;
    public const uint ENTITY_CODE = 2782820839;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
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

public partial class IfcRelInteractionRequirements
   : IfcRelConnects
{
    public static IfcRelInteractionRequirements Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELINTERACTIONREQUIREMENTS"u8;
    public const uint ENTITY_CODE = 2885861550;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCountMeasure> DailyInteraction = new("DailyInteraction", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> ImportanceRating = new("ImportanceRating", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSpatialStructureElement> LocationOfInteraction = new("LocationOfInteraction", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSpaceProgram> RelatedSpaceProgram = new("RelatedSpaceProgram", 7, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcSpaceProgram> RelatingSpaceProgram = new("RelatingSpaceProgram", 8, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, DailyInteraction, ImportanceRating, LocationOfInteraction, RelatedSpaceProgram, RelatingSpaceProgram ];
}

public partial class IfcRelNests
   : IfcRelDecomposes
{
    public static IfcRelNests Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELNESTS"u8;
    public const uint ENTITY_CODE = 1994019001;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingObject, RelatedObjects ];
}

public partial class IfcRelOccupiesSpaces
   : IfcRelAssignsToActor
{
    public static IfcRelOccupiesSpaces Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELOCCUPIESSPACES"u8;
    public const uint ENTITY_CODE = 3103990384;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingActor, ActingRole ];
}

public partial class IfcRelOverridesProperties
   : IfcRelDefinesByProperties
{
    public static IfcRelOverridesProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELOVERRIDESPROPERTIES"u8;
    public const uint ENTITY_CODE = 3641033950;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProperty> OverridingProperties = new("OverridingProperties", 6, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatingPropertyDefinition, OverridingProperties ];
}

public partial class IfcRelProjectsElement
   : IfcRelConnects
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
    public readonly IfcAttribute<IfcProduct> RelatedElements = new("RelatedElements", 4, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcSpatialStructureElement> RelatingStructure = new("RelatingStructure", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedElements, RelatingStructure ];
}

public partial class IfcRelSchedulesCostItems
   : IfcRelAssignsToControl
{
    public static IfcRelSchedulesCostItems Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRELSCHEDULESCOSTITEMS"u8;
    public const uint ENTITY_CODE = 1385156791;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatedObjects, RelatedObjectsType, RelatingControl ];
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
    public readonly IfcAttribute<IfcTimeMeasure> TimeLag = new("TimeLag", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSequenceEnum> SequenceType = new("SequenceType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingProcess, RelatedProcess, TimeLag, SequenceType ];
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
    public readonly IfcAttribute<IfcSpatialStructureElement> RelatedBuildings = new("RelatedBuildings", 5, IfcTypeKind.Entity, 1);
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
    public readonly IfcAttribute<IfcSpace> RelatingSpace = new("RelatingSpace", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcElement> RelatedBuildingElement = new("RelatedBuildingElement", 5, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcConnectionGeometry> ConnectionGeometry = new("ConnectionGeometry", 6, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcPhysicalOrVirtualEnum> PhysicalOrVirtualBoundary = new("PhysicalOrVirtualBoundary", 7, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcInternalOrExternalEnum> InternalOrExternalBoundary = new("InternalOrExternalBoundary", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, RelatingSpace, RelatedBuildingElement, ConnectionGeometry, PhysicalOrVirtualBoundary, InternalOrExternalBoundary ];
}

public partial class IfcRelVoidsElement
   : IfcRelConnects
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
   : EntityBaseClass
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
   : IfcObject
{
    public static IfcResource Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRESOURCE"u8;
    public const uint ENTITY_CODE = 1376835163;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
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

public partial class IfcRibPlateProfileProperties
   : IfcProfileProperties
{
    public static IfcRibPlateProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCRIBPLATEPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 977319432;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Thickness = new("Thickness", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RibHeight = new("RibHeight", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RibWidth = new("RibWidth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RibSpacing = new("RibSpacing", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcRibPlateDirectionEnum> Direction = new("Direction", 6, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ ProfileName, ProfileDefinition, Thickness, RibHeight, RibWidth, RibSpacing, Direction ];
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

public partial class IfcRoof
   : IfcBuildingElement
{
    public static IfcRoof Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROOF"u8;
    public const uint ENTITY_CODE = 1812914585;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcRoofTypeEnum> ShapeType = new("ShapeType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, ShapeType ];
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

public partial class IfcRoundedEdgeFeature
   : IfcEdgeFeature
{
    public static IfcRoundedEdgeFeature Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCROUNDEDEDGEFEATURE"u8;
    public const uint ENTITY_CODE = 3159070417;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> Radius = new("Radius", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, FeatureLength, Radius ];
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

public partial class IfcScheduleTimeControl
   : IfcControl
{
    public static IfcScheduleTimeControl Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSCHEDULETIMECONTROL"u8;
    public const uint ENTITY_CODE = 1112103640;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDateTimeSelect> ActualStart = new("ActualStart", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> EarlyStart = new("EarlyStart", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> LateStart = new("LateStart", 7, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> ScheduleStart = new("ScheduleStart", 8, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> ActualFinish = new("ActualFinish", 9, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> EarlyFinish = new("EarlyFinish", 10, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> LateFinish = new("LateFinish", 11, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> ScheduleFinish = new("ScheduleFinish", 12, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTimeMeasure> ScheduleDuration = new("ScheduleDuration", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> ActualDuration = new("ActualDuration", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> RemainingTime = new("RemainingTime", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> FreeFloat = new("FreeFloat", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> TotalFloat = new("TotalFloat", 17, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> IsCritical = new("IsCritical", 18, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> StatusTime = new("StatusTime", 19, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTimeMeasure> StartFloat = new("StartFloat", 20, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> FinishFloat = new("FinishFloat", 21, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> Completion = new("Completion", 22, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ActualStart, EarlyStart, LateStart, ScheduleStart, ActualFinish, EarlyFinish, LateFinish, ScheduleFinish, ScheduleDuration, ActualDuration, RemainingTime, FreeFloat, TotalFloat, IsCritical, StatusTime, StartFloat, FinishFloat, Completion ];
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

public partial class IfcSectionProperties
   : EntityBaseClass
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
   : EntityBaseClass
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

public partial class IfcServiceLife
   : IfcControl
{
    public static IfcServiceLife Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSERVICELIFE"u8;
    public const uint ENTITY_CODE = 20376344;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcServiceLifeTypeEnum> ServiceLifeType = new("ServiceLifeType", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcTimeMeasure> ServiceLifeDuration = new("ServiceLifeDuration", 6, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ServiceLifeType, ServiceLifeDuration ];
}

public partial class IfcServiceLifeFactor
   : IfcPropertySetDefinition
{
    public static IfcServiceLifeFactor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSERVICELIFEFACTOR"u8;
    public const uint ENTITY_CODE = 2295201387;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcServiceLifeFactorTypeEnum> PredefinedType = new("PredefinedType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcMeasureValue> UpperValue = new("UpperValue", 5, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcMeasureValue> MostUsedValue = new("MostUsedValue", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcMeasureValue> LowerValue = new("LowerValue", 7, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, PredefinedType, UpperValue, MostUsedValue, LowerValue ];
}

public partial class IfcShapeAspect
   : EntityBaseClass
{
    public static IfcShapeAspect Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSHAPEASPECT"u8;
    public const uint ENTITY_CODE = 2070624568;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShapeModel> ShapeRepresentations = new("ShapeRepresentations", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<LOGICAL> ProductDefinitional = new("ProductDefinitional", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcProductDefinitionShape> PartOfProductDefinitionShape = new("PartOfProductDefinitionShape", 4, IfcTypeKind.Entity, 0);
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

public partial class IfcSimpleProperty
   : IfcProperty
{
    public static IfcSimpleProperty Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSIMPLEPROPERTY"u8;
    public const uint ENTITY_CODE = 4288830184;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Name, Description ];
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
   : IfcBuildingElement
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
   : IfcBuildingElementType
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

public partial class IfcSolidModel
   : IfcGeometricRepresentationItem, IfcBooleanOperand
{
    public static IfcSolidModel Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSOLIDMODEL"u8;
    public const uint ENTITY_CODE = 2028701031;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcSoundProperties
   : IfcPropertySetDefinition
{
    public static IfcSoundProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSOUNDPROPERTIES"u8;
    public const uint ENTITY_CODE = 2757381299;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcBoolean> IsAttenuating = new("IsAttenuating", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSoundScaleEnum> SoundScale = new("SoundScale", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcSoundValue> SoundValues = new("SoundValues", 6, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, IsAttenuating, SoundScale, SoundValues ];
}

public partial class IfcSoundValue
   : IfcPropertySetDefinition
{
    public static IfcSoundValue Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSOUNDVALUE"u8;
    public const uint ENTITY_CODE = 2613057191;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTimeSeries> SoundLevelTimeSeries = new("SoundLevelTimeSeries", 4, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcFrequencyMeasure> Frequency = new("Frequency", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDerivedMeasureValue> SoundLevelSingleValue = new("SoundLevelSingleValue", 6, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, SoundLevelTimeSeries, Frequency, SoundLevelSingleValue ];
}

public partial class IfcSpace
   : IfcSpatialStructureElement
{
    public static IfcSpace Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACE"u8;
    public const uint ENTITY_CODE = 679641035;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcInternalOrExternalEnum> InteriorOrExteriorSpace = new("InteriorOrExteriorSpace", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ElevationWithFlooring = new("ElevationWithFlooring", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType, InteriorOrExteriorSpace, ElevationWithFlooring ];
}

public partial class IfcSpaceHeaterType
   : IfcEnergyConversionDeviceType
{
    public static IfcSpaceHeaterType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACEHEATERTYPE"u8;
    public const uint ENTITY_CODE = 68188634;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpaceHeaterTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSpaceProgram
   : IfcControl
{
    public static IfcSpaceProgram Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACEPROGRAM"u8;
    public const uint ENTITY_CODE = 2077060621;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> SpaceProgramIdentifier = new("SpaceProgramIdentifier", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> MaxRequiredArea = new("MaxRequiredArea", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> MinRequiredArea = new("MinRequiredArea", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSpatialStructureElement> RequestedLocation = new("RequestedLocation", 8, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcAreaMeasure> StandardRequiredArea = new("StandardRequiredArea", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, SpaceProgramIdentifier, MaxRequiredArea, MinRequiredArea, RequestedLocation, StandardRequiredArea ];
}

public partial class IfcSpaceThermalLoadProperties
   : IfcPropertySetDefinition
{
    public static IfcSpaceThermalLoadProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPACETHERMALLOADPROPERTIES"u8;
    public const uint ENTITY_CODE = 1310605843;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveRatioMeasure> ApplicableValueRatio = new("ApplicableValueRatio", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermalLoadSourceEnum> ThermalLoadSource = new("ThermalLoadSource", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcPropertySourceEnum> PropertySource = new("PropertySource", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcText> SourceDescription = new("SourceDescription", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPowerMeasure> MaximumValue = new("MaximumValue", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPowerMeasure> MinimumValue = new("MinimumValue", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeSeries> ThermalLoadTimeSeriesValues = new("ThermalLoadTimeSeriesValues", 10, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedThermalLoadSource = new("UserDefinedThermalLoadSource", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedPropertySource = new("UserDefinedPropertySource", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermalLoadTypeEnum> ThermalLoadType = new("ThermalLoadType", 13, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableValueRatio, ThermalLoadSource, PropertySource, SourceDescription, MaximumValue, MinimumValue, ThermalLoadTimeSeriesValues, UserDefinedThermalLoadSource, UserDefinedPropertySource, ThermalLoadType ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
}

public partial class IfcSpatialStructureElement
   : IfcProduct
{
    public static IfcSpatialStructureElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALSTRUCTUREELEMENT"u8;
    public const uint ENTITY_CODE = 872665622;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> LongName = new("LongName", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcElementCompositionEnum> CompositionType = new("CompositionType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, LongName, CompositionType ];
}

public partial class IfcSpatialStructureElementType
   : IfcElementType
{
    public static IfcSpatialStructureElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSPATIALSTRUCTUREELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 787986470;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
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
   : IfcBuildingElement
{
    public static IfcStair Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIR"u8;
    public const uint ENTITY_CODE = 3784347268;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStairTypeEnum> ShapeType = new("ShapeType", 8, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, ShapeType ];
}

public partial class IfcStairFlight
   : IfcBuildingElement
{
    public static IfcStairFlight Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIRFLIGHT"u8;
    public const uint ENTITY_CODE = 1991789322;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<INTEGER> NumberOfRiser = new("NumberOfRiser", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> NumberOfTreads = new("NumberOfTreads", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> RiserHeight = new("RiserHeight", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TreadLength = new("TreadLength", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, NumberOfRiser, NumberOfTreads, RiserHeight, TreadLength ];
}

public partial class IfcStairFlightType
   : IfcBuildingElementType
{
    public static IfcStairFlightType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTAIRFLIGHTTYPE"u8;
    public const uint ENTITY_CODE = 335595626;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStairFlightTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
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
    public readonly IfcAttribute<BOOLEAN> DestabilizingLoad = new("DestabilizingLoad", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcStructuralReaction> CausedBy = new("CausedBy", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, CausedBy ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, PredefinedType, OrientationOf2DPlane, LoadedBy, HasResults ];
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

public partial class IfcStructuralCurveConnection
   : IfcStructuralConnection
{
    public static IfcStructuralCurveConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVECONNECTION"u8;
    public const uint ENTITY_CODE = 4144297951;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedCondition ];
}

public partial class IfcStructuralCurveMember
   : IfcStructuralMember
{
    public static IfcStructuralCurveMember Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVEMEMBER"u8;
    public const uint ENTITY_CODE = 2394259173;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcStructuralCurveTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType ];
}

public partial class IfcStructuralCurveMemberVarying
   : IfcStructuralCurveMember
{
    public static IfcStructuralCurveMemberVarying Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALCURVEMEMBERVARYING"u8;
    public const uint ENTITY_CODE = 2882265595;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType ];
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
   : IfcStructuralAction
{
    public static IfcStructuralLinearAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLINEARACTION"u8;
    public const uint ENTITY_CODE = 322418247;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProjectedOrTrueLengthEnum> ProjectedOrTrue = new("ProjectedOrTrue", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, CausedBy, ProjectedOrTrue ];
}

public partial class IfcStructuralLinearActionVarying
   : IfcStructuralLinearAction
{
    public static IfcStructuralLinearActionVarying Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALLINEARACTIONVARYING"u8;
    public const uint ENTITY_CODE = 1324805177;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShapeAspect> VaryingAppliedLoadLocation = new("VaryingAppliedLoadLocation", 12, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStructuralLoad> SubsequentAppliedLoads = new("SubsequentAppliedLoads", 13, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, CausedBy, ProjectedOrTrue, VaryingAppliedLoadLocation, SubsequentAppliedLoads ];
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
   : IfcStructuralLoad
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
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> DeltaT_Constant = new("DeltaT_Constant", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> DeltaT_Y = new("DeltaT_Y", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> DeltaT_Z = new("DeltaT_Z", 3, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Name, DeltaT_Constant, DeltaT_Y, DeltaT_Z ];
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
   : IfcStructuralAction
{
    public static IfcStructuralPlanarAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPLANARACTION"u8;
    public const uint ENTITY_CODE = 1027411938;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcProjectedOrTrueLengthEnum> ProjectedOrTrue = new("ProjectedOrTrue", 11, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, CausedBy, ProjectedOrTrue ];
}

public partial class IfcStructuralPlanarActionVarying
   : IfcStructuralPlanarAction
{
    public static IfcStructuralPlanarActionVarying Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPLANARACTIONVARYING"u8;
    public const uint ENTITY_CODE = 978293566;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcShapeAspect> VaryingAppliedLoadLocation = new("VaryingAppliedLoadLocation", 12, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcStructuralLoad> SubsequentAppliedLoads = new("SubsequentAppliedLoads", 13, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, CausedBy, ProjectedOrTrue, VaryingAppliedLoadLocation, SubsequentAppliedLoads ];
}

public partial class IfcStructuralPointAction
   : IfcStructuralAction
{
    public static IfcStructuralPointAction Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPOINTACTION"u8;
    public const uint ENTITY_CODE = 1770641488;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedLoad, GlobalOrLocal, DestabilizingLoad, CausedBy ];
}

public partial class IfcStructuralPointConnection
   : IfcStructuralConnection
{
    public static IfcStructuralPointConnection Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPOINTCONNECTION"u8;
    public const uint ENTITY_CODE = 3619564870;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, AppliedCondition ];
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

public partial class IfcStructuralProfileProperties
   : IfcGeneralProfileProperties
{
    public static IfcStructuralProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 21487744;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcMomentOfInertiaMeasure> TorsionalConstantX = new("TorsionalConstantX", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMomentOfInertiaMeasure> MomentOfInertiaYZ = new("MomentOfInertiaYZ", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMomentOfInertiaMeasure> MomentOfInertiaY = new("MomentOfInertiaY", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcMomentOfInertiaMeasure> MomentOfInertiaZ = new("MomentOfInertiaZ", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcWarpingConstantMeasure> WarpingConstant = new("WarpingConstant", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ShearCentreZ = new("ShearCentreZ", 12, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> ShearCentreY = new("ShearCentreY", 13, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> ShearDeformationAreaZ = new("ShearDeformationAreaZ", 14, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> ShearDeformationAreaY = new("ShearDeformationAreaY", 15, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSectionModulusMeasure> MaximumSectionModulusY = new("MaximumSectionModulusY", 16, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSectionModulusMeasure> MinimumSectionModulusY = new("MinimumSectionModulusY", 17, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSectionModulusMeasure> MaximumSectionModulusZ = new("MaximumSectionModulusZ", 18, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSectionModulusMeasure> MinimumSectionModulusZ = new("MinimumSectionModulusZ", 19, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSectionModulusMeasure> TorsionalSectionModulus = new("TorsionalSectionModulus", 20, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> CentreOfGravityInX = new("CentreOfGravityInX", 21, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLengthMeasure> CentreOfGravityInY = new("CentreOfGravityInY", 22, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileName, ProfileDefinition, PhysicalWeight, Perimeter, MinimumPlateThickness, MaximumPlateThickness, CrossSectionArea, TorsionalConstantX, MomentOfInertiaYZ, MomentOfInertiaY, MomentOfInertiaZ, WarpingConstant, ShearCentreZ, ShearCentreY, ShearDeformationAreaZ, ShearDeformationAreaY, MaximumSectionModulusY, MinimumSectionModulusY, MaximumSectionModulusZ, MinimumSectionModulusZ, TorsionalSectionModulus, CentreOfGravityInX, CentreOfGravityInY ];
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
    public readonly IfcAttribute<BOOLEAN> IsLinear = new("IsLinear", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TheoryType, ResultForLoadGroup, IsLinear ];
}

public partial class IfcStructuralSteelProfileProperties
   : IfcStructuralProfileProperties
{
    public static IfcStructuralSteelProfileProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTURALSTEELPROFILEPROPERTIES"u8;
    public const uint ENTITY_CODE = 814732545;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAreaMeasure> ShearAreaZ = new("ShearAreaZ", 23, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcAreaMeasure> ShearAreaY = new("ShearAreaY", 24, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> PlasticShapeFactorY = new("PlasticShapeFactorY", 25, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveRatioMeasure> PlasticShapeFactorZ = new("PlasticShapeFactorZ", 26, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileName, ProfileDefinition, PhysicalWeight, Perimeter, MinimumPlateThickness, MaximumPlateThickness, CrossSectionArea, TorsionalConstantX, MomentOfInertiaYZ, MomentOfInertiaY, MomentOfInertiaZ, WarpingConstant, ShearCentreZ, ShearCentreY, ShearDeformationAreaZ, ShearDeformationAreaY, MaximumSectionModulusY, MinimumSectionModulusY, MaximumSectionModulusZ, MinimumSectionModulusZ, TorsionalSectionModulus, CentreOfGravityInX, CentreOfGravityInY, ShearAreaZ, ShearAreaY, PlasticShapeFactorY, PlasticShapeFactorZ ];
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
    public readonly IfcAttribute<IfcStructuralSurfaceTypeEnum> PredefinedType = new("PredefinedType", 7, IfcTypeKind.Enum, 0);
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> SubsequentThickness = new("SubsequentThickness", 9, IfcTypeKind.Alias, 1);
    public readonly IfcAttribute<IfcShapeAspect> VaryingThicknessLocation = new("VaryingThicknessLocation", 10, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, PredefinedType, Thickness, SubsequentThickness, VaryingThicknessLocation ];
}

public partial class IfcStructuredDimensionCallout
   : IfcDraughtingCallout
{
    public static IfcStructuredDimensionCallout Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSTRUCTUREDDIMENSIONCALLOUT"u8;
    public const uint ENTITY_CODE = 1985834640;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ Contents ];
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
    public readonly IfcAttribute<IfcPresentationStyleAssignment> Styles = new("Styles", 1, IfcTypeKind.Entity, 1);
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
    public readonly IfcAttribute<IfcActorSelect> SubContractor = new("SubContractor", 9, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcText> JobDescription = new("JobDescription", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ResourceIdentifier, ResourceGroup, ResourceConsumption, BaseQuantity, SubContractor, JobDescription ];
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

public partial class IfcSurfaceCurveSweptAreaSolid
   : IfcSweptAreaSolid
{
    public static IfcSurfaceCurveSweptAreaSolid Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACECURVESWEPTAREASOLID"u8;
    public const uint ENTITY_CODE = 4130340898;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCurve> Directrix = new("Directrix", 2, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcParameterValue> StartParam = new("StartParam", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcParameterValue> EndParam = new("EndParam", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSurface> ReferenceSurface = new("ReferenceSurface", 5, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SweptArea, Position, Directrix, StartParam, EndParam, ReferenceSurface ];
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

public partial class IfcSurfaceStyle
   : IfcPresentationStyle, IfcPresentationStyleSelect
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
   : EntityBaseClass, IfcSurfaceStyleElementSelect
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
   : EntityBaseClass, IfcSurfaceStyleElementSelect
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
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> Transparency = new("Transparency", 1, IfcTypeKind.Alias, 0);
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
   : EntityBaseClass, IfcSurfaceStyleElementSelect
{
    public static IfcSurfaceStyleShading Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACESTYLESHADING"u8;
    public const uint ENTITY_CODE = 2237861999;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcColourRgb> SurfaceColour = new("SurfaceColour", 0, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ SurfaceColour ];
}

public partial class IfcSurfaceStyleWithTextures
   : EntityBaseClass, IfcSurfaceStyleElementSelect
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
   : EntityBaseClass
{
    public static IfcSurfaceTexture Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSURFACETEXTURE"u8;
    public const uint ENTITY_CODE = 2119552589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<BOOLEAN> RepeatS = new("RepeatS", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> RepeatT = new("RepeatT", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSurfaceTextureEnum> TextureType = new("TextureType", 2, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcCartesianTransformationOperator2D> TextureTransform = new("TextureTransform", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ RepeatS, RepeatT, TextureType, TextureTransform ];
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

public partial class IfcSymbolStyle
   : IfcPresentationStyle, IfcPresentationStyleSelect
{
    public static IfcSymbolStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSYMBOLSTYLE"u8;
    public const uint ENTITY_CODE = 2090104136;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSymbolStyleSelect> StyleOfSymbol = new("StyleOfSymbol", 1, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, StyleOfSymbol ];
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

public partial class IfcSystemFurnitureElementType
   : IfcFurnishingElementType
{
    public static IfcSystemFurnitureElementType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCSYSTEMFURNITUREELEMENTTYPE"u8;
    public const uint ENTITY_CODE = 1911274308;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType ];
}

public partial class IfcTable
   : EntityBaseClass, IfcMetricValueSelect
{
    public static IfcTable Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTABLE"u8;
    public const uint ENTITY_CODE = 1707516689;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<STRING> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTableRow> Rows = new("Rows", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ Name, Rows ];
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
    public readonly IfcAttribute<BOOLEAN> IsHeading = new("IsHeading", 1, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ RowCells, IsHeading ];
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
    public readonly IfcAttribute<IfcIdentifier> TaskId = new("TaskId", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> Status = new("Status", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcLabel> WorkMethod = new("WorkMethod", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> IsMilestone = new("IsMilestone", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<INTEGER> Priority = new("Priority", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, TaskId, Status, WorkMethod, IsMilestone, Priority ];
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
    public readonly IfcAttribute<IfcLabel> WWWHomePageURL = new("WWWHomePageURL", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Purpose, Description, UserDefinedPurpose, TelephoneNumbers, FacsimileNumbers, PagerNumber, ElectronicMailAddresses, WWWHomePageURL ];
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, SteelGrade ];
}

public partial class IfcTerminatorSymbol
   : IfcAnnotationSymbolOccurrence
{
    public static IfcTerminatorSymbol Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTERMINATORSYMBOL"u8;
    public const uint ENTITY_CODE = 976109018;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcAnnotationCurveOccurrence> AnnotatedCurve = new("AnnotatedCurve", 3, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ Item, Styles, Name, AnnotatedCurve ];
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
   : IfcPresentationStyle, IfcPresentationStyleSelect
{
    public static IfcTextStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTSTYLE"u8;
    public const uint ENTITY_CODE = 1641706589;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcCharacterStyleSelect> TextCharacterAppearance = new("TextCharacterAppearance", 1, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTextStyleSelect> TextStyle = new("TextStyle", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTextFontSelect> TextFontStyle = new("TextFontStyle", 3, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, TextCharacterAppearance, TextStyle, TextFontStyle ];
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
   : EntityBaseClass, IfcCharacterStyleSelect
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
   : EntityBaseClass, IfcTextStyleSelect
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

public partial class IfcTextStyleWithBoxCharacteristics
   : EntityBaseClass, IfcTextStyleSelect
{
    public static IfcTextStyleWithBoxCharacteristics Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTSTYLEWITHBOXCHARACTERISTICS"u8;
    public const uint ENTITY_CODE = 230028504;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BoxHeight = new("BoxHeight", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> BoxWidth = new("BoxWidth", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> BoxSlantAngle = new("BoxSlantAngle", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> BoxRotateAngle = new("BoxRotateAngle", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSizeSelect> CharacterSpacing = new("CharacterSpacing", 4, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ BoxHeight, BoxWidth, BoxSlantAngle, BoxRotateAngle, CharacterSpacing ];
}

public partial class IfcTextureCoordinate
   : EntityBaseClass
{
    public static IfcTextureCoordinate Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTURECOORDINATE"u8;
    public const uint ENTITY_CODE = 1304733824;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [  ];
}

public partial class IfcTextureCoordinateGenerator
   : IfcTextureCoordinate
{
    public static IfcTextureCoordinateGenerator Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTURECOORDINATEGENERATOR"u8;
    public const uint ENTITY_CODE = 986362205;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Mode = new("Mode", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcSimpleValue> Parameter = new("Parameter", 1, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ Mode, Parameter ];
}

public partial class IfcTextureMap
   : IfcTextureCoordinate
{
    public static IfcTextureMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTUREMAP"u8;
    public const uint ENTITY_CODE = 1189656152;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVertexBasedTextureMap> TextureMaps = new("TextureMaps", 0, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ TextureMaps ];
}

public partial class IfcTextureVertex
   : EntityBaseClass
{
    public static IfcTextureVertex Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTEXTUREVERTEX"u8;
    public const uint ENTITY_CODE = 1240493628;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcParameterValue> Coordinates = new("Coordinates", 0, IfcTypeKind.Alias, 1);
    public override IfcAttribute[] Attributes => [ Coordinates ];
}

public partial class IfcThermalMaterialProperties
   : IfcMaterialProperties
{
    public static IfcThermalMaterialProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTHERMALMATERIALPROPERTIES"u8;
    public const uint ENTITY_CODE = 230864606;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcSpecificHeatCapacityMeasure> SpecificHeatCapacity = new("SpecificHeatCapacity", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> BoilingPoint = new("BoilingPoint", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermodynamicTemperatureMeasure> FreezingPoint = new("FreezingPoint", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcThermalConductivityMeasure> ThermalConductivity = new("ThermalConductivity", 4, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, SpecificHeatCapacity, BoilingPoint, FreezingPoint, ThermalConductivity ];
}

public partial class IfcTimeSeries
   : EntityBaseClass, IfcMetricValueSelect, IfcObjectReferenceSelect
{
    public static IfcTimeSeries Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTIMESERIES"u8;
    public const uint ENTITY_CODE = 3335580439;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> Name = new("Name", 0, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcText> Description = new("Description", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> StartTime = new("StartTime", 2, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> EndTime = new("EndTime", 3, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcTimeSeriesDataTypeEnum> TimeSeriesDataType = new("TimeSeriesDataType", 4, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcDataOriginEnum> DataOrigin = new("DataOrigin", 5, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedDataOrigin = new("UserDefinedDataOrigin", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcUnit> Unit = new("Unit", 7, IfcTypeKind.Unknown, 0);
    public override IfcAttribute[] Attributes => [ Name, Description, StartTime, EndTime, TimeSeriesDataType, DataOrigin, UserDefinedDataOrigin, Unit ];
}

public partial class IfcTimeSeriesReferenceRelationship
   : EntityBaseClass
{
    public static IfcTimeSeriesReferenceRelationship Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTIMESERIESREFERENCERELATIONSHIP"u8;
    public const uint ENTITY_CODE = 3828356090;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTimeSeries> ReferencedTimeSeries = new("ReferencedTimeSeries", 0, IfcTypeKind.Entity, 0);
    public readonly IfcAttribute<IfcDocumentSelect> TimeSeriesReferences = new("TimeSeriesReferences", 1, IfcTypeKind.Unknown, 1);
    public override IfcAttribute[] Attributes => [ ReferencedTimeSeries, TimeSeriesReferences ];
}

public partial class IfcTimeSeriesSchedule
   : IfcControl
{
    public static IfcTimeSeriesSchedule Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTIMESERIESSCHEDULE"u8;
    public const uint ENTITY_CODE = 3015568630;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcDateTimeSelect> ApplicableDates = new("ApplicableDates", 5, IfcTypeKind.Unknown, 1);
    public readonly IfcAttribute<IfcTimeSeriesScheduleTypeEnum> TimeSeriesScheduleType = new("TimeSeriesScheduleType", 6, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcTimeSeries> TimeSeries = new("TimeSeries", 7, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ApplicableDates, TimeSeriesScheduleType, TimeSeries ];
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

public partial class IfcTransportElement
   : IfcElement
{
    public static IfcTransportElement Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTRANSPORTELEMENT"u8;
    public const uint ENTITY_CODE = 2895867572;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTransportElementTypeEnum> OperationType = new("OperationType", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcMassMeasure> CapacityByWeight = new("CapacityByWeight", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcCountMeasure> CapacityByNumber = new("CapacityByNumber", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, OperationType, CapacityByWeight, CapacityByNumber ];
}

public partial class IfcTransportElementType
   : IfcElementType
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
    public readonly IfcAttribute<BOOLEAN> SenseAgreement = new("SenseAgreement", 3, IfcTypeKind.Alias, 0);
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FlangeEdgeRadius = new("FlangeEdgeRadius", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> WebEdgeRadius = new("WebEdgeRadius", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> WebSlope = new("WebSlope", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> FlangeSlope = new("FlangeSlope", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInY = new("CentreOfGravityInY", 12, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, FlangeWidth, WebThickness, FlangeThickness, FilletRadius, FlangeEdgeRadius, WebEdgeRadius, WebSlope, FlangeSlope, CentreOfGravityInY ];
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

public partial class IfcTwoDirectionRepeatFactor
   : IfcOneDirectionRepeatFactor
{
    public static IfcTwoDirectionRepeatFactor Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTWODIRECTIONREPEATFACTOR"u8;
    public const uint ENTITY_CODE = 4203842640;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcVector> SecondRepeatFactor = new("SecondRepeatFactor", 1, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ RepeatFactor, SecondRepeatFactor ];
}

public partial class IfcTypeObject
   : IfcObjectDefinition
{
    public static IfcTypeObject Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCTYPEOBJECT"u8;
    public const uint ENTITY_CODE = 2249877892;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcLabel> ApplicableOccurrence = new("ApplicableOccurrence", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPropertySetDefinition> HasPropertySets = new("HasPropertySets", 5, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets ];
}

public partial class IfcTypeProduct
   : IfcTypeObject
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> EdgeRadius = new("EdgeRadius", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPlaneAngleMeasure> FlangeSlope = new("FlangeSlope", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> CentreOfGravityInX = new("CentreOfGravityInX", 10, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, FlangeWidth, WebThickness, FlangeThickness, FilletRadius, EdgeRadius, FlangeSlope, CentreOfGravityInX ];
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
   : IfcGeometricRepresentationItem, IfcVectorOrDirection
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

public partial class IfcVertexBasedTextureMap
   : EntityBaseClass
{
    public static IfcVertexBasedTextureMap Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCVERTEXBASEDTEXTUREMAP"u8;
    public const uint ENTITY_CODE = 649636163;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcTextureVertex> TextureVertices = new("TextureVertices", 0, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcCartesianPoint> TexturePoints = new("TexturePoints", 1, IfcTypeKind.Entity, 1);
    public override IfcAttribute[] Attributes => [ TextureVertices, TexturePoints ];
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

public partial class IfcVibrationIsolatorType
   : IfcDiscreteAccessoryType
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
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcVirtualGridIntersection
   : EntityBaseClass
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

public partial class IfcWall
   : IfcBuildingElement
{
    public static IfcWall Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWALL"u8;
    public const uint ENTITY_CODE = 2077320315;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcWallStandardCase
   : IfcWall
{
    public static IfcWallStandardCase Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWALLSTANDARDCASE"u8;
    public const uint ENTITY_CODE = 2426171302;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag ];
}

public partial class IfcWallType
   : IfcBuildingElementType
{
    public static IfcWallType Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWALLTYPE"u8;
    public const uint ENTITY_CODE = 3895821283;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWallTypeEnum> PredefinedType = new("PredefinedType", 9, IfcTypeKind.Enum, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ElementType, PredefinedType ];
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

public partial class IfcWaterProperties
   : IfcMaterialProperties
{
    public static IfcWaterProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWATERPROPERTIES"u8;
    public const uint ENTITY_CODE = 3640297787;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<BOOLEAN> IsPotable = new("IsPotable", 1, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIonConcentrationMeasure> Hardness = new("Hardness", 2, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIonConcentrationMeasure> AlkalinityConcentration = new("AlkalinityConcentration", 3, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcIonConcentrationMeasure> AcidityConcentration = new("AcidityConcentration", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> ImpuritiesContent = new("ImpuritiesContent", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPHMeasure> PHLevel = new("PHLevel", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> DissolvedSolidsContent = new("DissolvedSolidsContent", 7, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ Material, IsPotable, Hardness, AlkalinityConcentration, AcidityConcentration, ImpuritiesContent, PHLevel, DissolvedSolidsContent ];
}

public partial class IfcWindow
   : IfcBuildingElement
{
    public static IfcWindow Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOW"u8;
    public const uint ENTITY_CODE = 548816575;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallHeight = new("OverallHeight", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> OverallWidth = new("OverallWidth", 9, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, ObjectPlacement, Representation, Tag, OverallHeight, OverallWidth ];
}

public partial class IfcWindowLiningProperties
   : IfcPropertySetDefinition
{
    public static IfcWindowLiningProperties Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOWLININGPROPERTIES"u8;
    public const uint ENTITY_CODE = 399706723;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LiningDepth = new("LiningDepth", 4, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> LiningThickness = new("LiningThickness", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> TransomThickness = new("TransomThickness", 6, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> MullionThickness = new("MullionThickness", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> FirstTransomOffset = new("FirstTransomOffset", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> SecondTransomOffset = new("SecondTransomOffset", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> FirstMullionOffset = new("FirstMullionOffset", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcNormalisedRatioMeasure> SecondMullionOffset = new("SecondMullionOffset", 11, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcShapeAspect> ShapeAspectStyle = new("ShapeAspectStyle", 12, IfcTypeKind.Entity, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, LiningDepth, LiningThickness, TransomThickness, MullionThickness, FirstTransomOffset, SecondTransomOffset, FirstMullionOffset, SecondMullionOffset, ShapeAspectStyle ];
}

public partial class IfcWindowPanelProperties
   : IfcPropertySetDefinition
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

public partial class IfcWindowStyle
   : IfcTypeProduct
{
    public static IfcWindowStyle Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWINDOWSTYLE"u8;
    public const uint ENTITY_CODE = 1127398656;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcWindowStyleConstructionEnum> ConstructionType = new("ConstructionType", 8, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcWindowStyleOperationEnum> OperationType = new("OperationType", 9, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<BOOLEAN> ParameterTakesPrecedence = new("ParameterTakesPrecedence", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<BOOLEAN> Sizeable = new("Sizeable", 11, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ApplicableOccurrence, HasPropertySets, RepresentationMaps, Tag, ConstructionType, OperationType, ParameterTakesPrecedence, Sizeable ];
}

public partial class IfcWorkControl
   : IfcControl
{
    public static IfcWorkControl Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKCONTROL"u8;
    public const uint ENTITY_CODE = 2134216975;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public readonly IfcAttribute<IfcIdentifier> Identifier = new("Identifier", 5, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> CreationDate = new("CreationDate", 6, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcPerson> Creators = new("Creators", 7, IfcTypeKind.Entity, 1);
    public readonly IfcAttribute<IfcLabel> Purpose = new("Purpose", 8, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> Duration = new("Duration", 9, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcTimeMeasure> TotalFloat = new("TotalFloat", 10, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> StartTime = new("StartTime", 11, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcDateTimeSelect> FinishTime = new("FinishTime", 12, IfcTypeKind.Unknown, 0);
    public readonly IfcAttribute<IfcWorkControlTypeEnum> WorkControlType = new("WorkControlType", 13, IfcTypeKind.Enum, 0);
    public readonly IfcAttribute<IfcLabel> UserDefinedControlType = new("UserDefinedControlType", 14, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identifier, CreationDate, Creators, Purpose, Duration, TotalFloat, StartTime, FinishTime, WorkControlType, UserDefinedControlType ];
}

public partial class IfcWorkPlan
   : IfcWorkControl
{
    public static IfcWorkPlan Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKPLAN"u8;
    public const uint ENTITY_CODE = 4262694961;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identifier, CreationDate, Creators, Purpose, Duration, TotalFloat, StartTime, FinishTime, WorkControlType, UserDefinedControlType ];
}

public partial class IfcWorkSchedule
   : IfcWorkControl
{
    public static IfcWorkSchedule Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCWORKSCHEDULE"u8;
    public const uint ENTITY_CODE = 302889391;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType, Identifier, CreationDate, Creators, Purpose, Duration, TotalFloat, StartTime, FinishTime, WorkControlType, UserDefinedControlType ];
}

public partial class IfcZone
   : IfcGroup
{
    public static IfcZone Instance = new();
    public static ReadOnlySpan<byte> NAME => "IFCZONE"u8;
    public const uint ENTITY_CODE = 3177690381;
    public override uint EntityTypeCode => ENTITY_CODE;
    public override ReadOnlySpan<byte> EntityTypeName => NAME;
    public override IfcAttribute[] Attributes => [ GlobalId, OwnerHistory, Name, Description, ObjectType ];
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
    public readonly IfcAttribute<IfcPositiveLengthMeasure> FilletRadius = new("FilletRadius", 7, IfcTypeKind.Alias, 0);
    public readonly IfcAttribute<IfcPositiveLengthMeasure> EdgeRadius = new("EdgeRadius", 8, IfcTypeKind.Alias, 0);
    public override IfcAttribute[] Attributes => [ ProfileType, ProfileName, Position, Depth, FlangeWidth, WebThickness, FlangeThickness, FilletRadius, EdgeRadius ];
}
