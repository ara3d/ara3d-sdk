namespace Ara3D.PropKit;

public enum PropStepSize { Small, Large }

public interface IPropStepper
{
    object Step(
        PropDescriptor descriptor, 
        IPropValidator validator, 
        object value, 
        int direction, 
        PropStepSize size);
}