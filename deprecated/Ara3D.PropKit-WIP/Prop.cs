namespace Ara3D.PropKit;

public record Prop(
    PropDescriptor Descriptor,
    IPropAccessor Accessor,
    IPropValidator Validator = null,
    IPropStepper Stepper = null,
    PropConstraints Constraints = default,
    IPropStringCodec StringCodec = null
);