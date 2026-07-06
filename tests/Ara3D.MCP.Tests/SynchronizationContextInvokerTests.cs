namespace Ara3D.MCP.Tests;

[TestFixture]
public class SynchronizationContextInvokerTests
{
    sealed class TestSynchronizationContext : SynchronizationContext
    {
        public int InvokeCount { get; private set; }

        public override void Send(SendOrPostCallback d, object? state)
        {
            InvokeCount++;
            var previous = Current;
            SetSynchronizationContext(this);
            try
            {
                d(state);
            }
            finally
            {
                SetSynchronizationContext(previous);
            }
        }

        public override void Post(SendOrPostCallback d, object? state)
            => Send(d, state);
    }

    [Test]
    public void Invoke_RunsInlineOnSameContext()
    {
        var context = new TestSynchronizationContext();
        var invoker = new SynchronizationContextInvoker(context);

        SynchronizationContext.SetSynchronizationContext(context);
        try
        {
            var result = invoker.Invoke(() => 42);
            Assert.That(result, Is.EqualTo(42));
            Assert.That(context.InvokeCount, Is.EqualTo(0));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(null);
        }
    }

    [Test]
    public void Invoke_MarshalsToTargetContext()
    {
        var context = new TestSynchronizationContext();
        var invoker = new SynchronizationContextInvoker(context);
        int? result = null;

        var thread = new Thread(() => result = invoker.Invoke(() => 42));
        thread.Start();
        thread.Join();

        Assert.That(result, Is.EqualTo(42));
        Assert.That(context.InvokeCount, Is.EqualTo(1));
    }

    [Test]
    public void Invoke_PropagatesException()
    {
        var context = new TestSynchronizationContext();
        var invoker = new SynchronizationContextInvoker(context);

        Assert.Throws<InvalidOperationException>(() =>
            invoker.Invoke<int>(() => throw new InvalidOperationException("fail")));
    }
}
