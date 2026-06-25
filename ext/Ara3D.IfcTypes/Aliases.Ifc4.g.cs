namespace Ara3D.IfcTypes.Ifc4;

public class IfcAbsorbedDoseMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcAccelerationMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcAmountOfSubstanceMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcAngularVelocityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcArcIndex
    : TypeAliasBaseClass, IfcSegmentIndexSelect
{
    public static TypeDetails Type = new(typeof(IfcPositiveInteger), IfcTypeKind.Alias, 1);
}

public class IfcAreaDensityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcAreaMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcBinary
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(BINARY), IfcTypeKind.Alias, 0);
}

public class IfcBoolean
    : TypeAliasBaseClass, IfcModulusOfRotationalSubgradeReactionSelect, IfcModulusOfSubgradeReactionSelect, IfcModulusOfTranslationalSubgradeReactionSelect, IfcRotationalStiffnessSelect, IfcSimpleValue, IfcTranslationalStiffnessSelect, IfcWarpingStiffnessSelect
{
    public static TypeDetails Type = new(typeof(BOOLEAN), IfcTypeKind.Alias, 0);
}

public class IfcBoxAlignment
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(IfcLabel), IfcTypeKind.Alias, 0);
}

public class IfcCardinalPointReference
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcComplexNumber
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 1);
}

public class IfcCompoundPlaneAngleMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 1);
}

public class IfcContextDependentMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcCountMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(NUMBER), IfcTypeKind.Alias, 0);
}

public class IfcCurvatureMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcDate
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcDateTime
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcDayInMonthNumber
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcDayInWeekNumber
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcDescriptiveMeasure
    : TypeAliasBaseClass, IfcMeasureValue, IfcSizeSelect
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcDimensionCount
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcDoseEquivalentMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcDuration
    : TypeAliasBaseClass, IfcSimpleValue, IfcTimeOrRatioSelect
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcDynamicViscosityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcElectricCapacitanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcElectricChargeMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcElectricConductanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcElectricCurrentMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcElectricResistanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcElectricVoltageMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcEnergyMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcFontStyle
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcFontVariant
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcFontWeight
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcForceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcFrequencyMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcGloballyUniqueId
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcHeatFluxDensityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcHeatingValueMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcIdentifier
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcIlluminanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcInductanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcInteger
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcIntegerCountRateMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcIonConcentrationMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcIsothermalMoistureCapacityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcKinematicViscosityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLabel
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcLanguageId
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(IfcIdentifier), IfcTypeKind.Alias, 0);
}

public class IfcLengthMeasure
    : TypeAliasBaseClass, IfcBendingParameterSelect, IfcMeasureValue, IfcSizeSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLinearForceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLinearMomentMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLinearStiffnessMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue, IfcTranslationalStiffnessSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLinearVelocityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLineIndex
    : TypeAliasBaseClass, IfcSegmentIndexSelect
{
    public static TypeDetails Type = new(typeof(IfcPositiveInteger), IfcTypeKind.Alias, 1);
}

public class IfcLogical
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(LOGICAL), IfcTypeKind.Alias, 0);
}

public class IfcLuminousFluxMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLuminousIntensityDistributionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLuminousIntensityMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMagneticFluxDensityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMagneticFluxMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMassDensityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMassFlowRateMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMassMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMassPerLengthMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfElasticityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfLinearSubgradeReactionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue, IfcModulusOfTranslationalSubgradeReactionSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfRotationalSubgradeReactionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue, IfcModulusOfRotationalSubgradeReactionSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfSubgradeReactionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue, IfcModulusOfSubgradeReactionSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMoistureDiffusivityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMolecularWeightMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMomentOfInertiaMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMonetaryMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMonthInYearNumber
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcNonNegativeLengthMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(IfcLengthMeasure), IfcTypeKind.Alias, 0);
}

public class IfcNormalisedRatioMeasure
    : TypeAliasBaseClass, IfcColourOrFactor, IfcMeasureValue, IfcSizeSelect
{
    public static TypeDetails Type = new(typeof(IfcRatioMeasure), IfcTypeKind.Alias, 0);
}

public class IfcNumericMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(NUMBER), IfcTypeKind.Alias, 0);
}

public class IfcParameterValue
    : TypeAliasBaseClass, IfcMeasureValue, IfcTrimmingSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcPHMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcPlanarForceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcPlaneAngleMeasure
    : TypeAliasBaseClass, IfcBendingParameterSelect, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcPositiveInteger
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(IfcInteger), IfcTypeKind.Alias, 0);
}

public class IfcPositiveLengthMeasure
    : TypeAliasBaseClass, IfcHatchLineDistanceSelect, IfcMeasureValue, IfcSizeSelect
{
    public static TypeDetails Type = new(typeof(IfcLengthMeasure), IfcTypeKind.Alias, 0);
}

public class IfcPositivePlaneAngleMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(IfcPlaneAngleMeasure), IfcTypeKind.Alias, 0);
}

public class IfcPositiveRatioMeasure
    : TypeAliasBaseClass, IfcMeasureValue, IfcSizeSelect
{
    public static TypeDetails Type = new(typeof(IfcRatioMeasure), IfcTypeKind.Alias, 0);
}

public class IfcPowerMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcPresentableText
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcPressureMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcPropertySetDefinitionSet
    : TypeAliasBaseClass, IfcPropertySetDefinitionSelect
{
    public static TypeDetails Type = new(typeof(IfcPropertySetDefinition), IfcTypeKind.Entity, 1);
}

public class IfcRadioActivityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcRatioMeasure
    : TypeAliasBaseClass, IfcMeasureValue, IfcSizeSelect, IfcTimeOrRatioSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcReal
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcRotationalFrequencyMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcRotationalMassMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcRotationalStiffnessMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue, IfcRotationalStiffnessSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSectionalAreaIntegralMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSectionModulusMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcShearModulusMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSolidAngleMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSoundPowerLevelMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSoundPowerMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSoundPressureLevelMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSoundPressureMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSpecificHeatCapacityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSpecularExponent
    : TypeAliasBaseClass, IfcSpecularHighlightSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSpecularRoughness
    : TypeAliasBaseClass, IfcSpecularHighlightSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcTemperatureGradientMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcTemperatureRateOfChangeMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcText
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcTextAlignment
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcTextDecoration
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcTextFontName
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcTextTransformation
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcThermalAdmittanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcThermalConductivityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcThermalExpansionCoefficientMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcThermalResistanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcThermalTransmittanceMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcThermodynamicTemperatureMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcTime
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcTimeMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcTimeStamp
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcTorqueMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcURIReference
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcVaporPermeabilityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcVolumeMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcVolumetricFlowRateMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcWarpingConstantMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcWarpingMomentMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue, IfcWarpingStiffnessSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class REAL
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(double), IfcTypeKind.System, 0);
}

public class NUMBER
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(double), IfcTypeKind.System, 0);
}

public class LOGICAL
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(bool), IfcTypeKind.System, 0);
}

public class BOOLEAN
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(bool), IfcTypeKind.System, 0);
}

public class STRING
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(string), IfcTypeKind.System, 0);
}

public class BINARY
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(string), IfcTypeKind.System, 0);
}

public class INTEGER
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(long), IfcTypeKind.System, 0);
}
