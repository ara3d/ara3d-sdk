using System.Diagnostics;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.LaunchDebugger;

public class LaunchDebuggerCommand : NamedCommand
{
    public override void Execute()
        => Debugger.Break();
}
