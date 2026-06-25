namespace Ara3D.IfcTypes.Ifc2x3;

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

public class IfcAreaMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcBoolean
    : TypeAliasBaseClass, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(BOOLEAN), IfcTypeKind.Alias, 0);
}

public class IfcBoxAlignment
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(IfcLabel), IfcTypeKind.Alias, 0);
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

public class IfcDayInMonthNumber
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcDaylightSavingHour
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

public class IfcHourInDay
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
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
    : TypeAliasBaseClass, IfcConditionCriterionSelect, IfcSimpleValue
{
    public static TypeDetails Type = new(typeof(STRING), IfcTypeKind.Alias, 0);
}

public class IfcLengthMeasure
    : TypeAliasBaseClass, IfcMeasureValue, IfcSizeSelect
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
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcLinearVelocityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
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

public class IfcMinuteInHour
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfElasticityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfLinearSubgradeReactionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfRotationalSubgradeReactionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcModulusOfSubgradeReactionMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
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
    : TypeAliasBaseClass, IfcAppliedValueSelect, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcMonthInYearNumber
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
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
    : TypeAliasBaseClass, IfcMeasureValue, IfcOrientationSelect
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
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

public class IfcRadioActivityMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcRatioMeasure
    : TypeAliasBaseClass, IfcAppliedValueSelect, IfcMeasureValue, IfcSizeSelect
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
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcSecondInMinute
    : TypeAliasBaseClass
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

public class IfcSoundPowerMeasure
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

public class IfcText
    : TypeAliasBaseClass, IfcMetricValueSelect, IfcSimpleValue
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

public class IfcTimeMeasure
    : TypeAliasBaseClass, IfcMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcTimeStamp
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
}

public class IfcTorqueMeasure
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
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
    : TypeAliasBaseClass, IfcDerivedMeasureValue
{
    public static TypeDetails Type = new(typeof(REAL), IfcTypeKind.Alias, 0);
}

public class IfcYearNumber
    : TypeAliasBaseClass
{
    public static TypeDetails Type = new(typeof(INTEGER), IfcTypeKind.Alias, 0);
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
