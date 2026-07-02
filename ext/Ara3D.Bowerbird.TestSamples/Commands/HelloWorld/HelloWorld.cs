using System.Windows.Forms;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.HelloWorld;

public class HelloWorldCommand : NamedCommand
{
    public override void Execute()
        => MessageBox.Show("Hello World!");
}
