using Ara3D.Domo;

namespace Ara3D.Domo.Tests;

public class RepositoryTests
{
    class TestState
    {
        public int N;
    }

    class RejectOddRepository : AggregateRepository<TestState>
    {
        public override bool Validate(TestState state)
            => state != null && state.N % 2 == 0;
    }

    [Test]
    public static void Update_notifies_correct_old_and_new_values()
    {
        var repo = new AggregateRepository<TestState>();
        var model = repo.Add(new TestState { N = 1 });
        RepositoryChangeArgs args = null;
        repo.RepositoryChanged += (_, e) => args = e;

        repo.Update(model.Id, s => new TestState { N = s.N + 1 });

        Assert.That(args, Is.Not.Null);
        Assert.That(args.ChangeType, Is.EqualTo(RepositoryChangeType.ModelUpdated));
        Assert.That(((TestState)args.OldValue).N, Is.EqualTo(1));
        Assert.That(((TestState)args.NewValue).N, Is.EqualTo(2));
    }

    [Test]
    public static void GetValue_returns_stored_value_not_model()
    {
        var repo = new AggregateRepository<TestState>();
        var model = repo.Add(new TestState { N = 42 });
        IRepository untyped = repo;

        var value = untyped.GetValue(model.Id);

        Assert.That(value, Is.InstanceOf<TestState>());
        Assert.That(((TestState)value).N, Is.EqualTo(42));
        Assert.That(value, Is.Not.InstanceOf<IModel>());
    }

    [Test]
    public static void OnModelRemoved_receives_model_without_throw()
    {
        var repo = new AggregateRepository<TestState>();
        var model = repo.Add(new TestState { N = 1 });
        var modelId = model.Id;
        IModel<TestState> removed = null;
        repo.OnModelRemoved(m =>
        {
            removed = m;
            Assert.That(m.Id, Is.EqualTo(modelId));
            Assert.That(m.Value.N, Is.EqualTo(1));
        });

        model.Delete();

        Assert.That(removed, Is.Not.Null);
        Assert.That(removed.Id, Is.EqualTo(modelId));
    }

    [Test]
    public static void AggregateRepository_add_update_delete()
    {
        var repo = new AggregateRepository<TestState>();
        Assert.That(repo.Count, Is.EqualTo(0));

        var model = repo.Add(new TestState { N = 1 });
        Assert.That(repo.Count, Is.EqualTo(1));
        Assert.That(repo.GetValue(model.Id).N, Is.EqualTo(1));

        repo.Update(model.Id, s => new TestState { N = s.N + 10 });
        Assert.That(repo.GetValue(model.Id).N, Is.EqualTo(11));

        model.Delete();
        Assert.That(repo.Count, Is.EqualTo(0));
    }

    [Test]
    public static void SingletonRepository_rejects_second_add()
    {
        var repo = new SingletonRepository<TestState>();
        Assert.Throws<Exception>(() => repo.Add(new TestState()));
    }

    [Test]
    public static void Update_returns_false_when_unchanged()
    {
        var repo = new AggregateRepository<TestState>();
        var model = repo.Add(new TestState { N = 5 });
        var changed = false;
        repo.RepositoryChanged += (_, _) => changed = true;

        var updated = repo.Update(model.Id, s => s);

        Assert.That(updated, Is.False);
        Assert.That(changed, Is.False);
    }

    [Test]
    public static void Update_returns_false_when_validation_fails()
    {
        var repo = new RejectOddRepository();
        var model = repo.Add(new TestState { N = 2 });

        var updated = repo.Update(model.Id, _ => new TestState { N = 3 });

        Assert.That(updated, Is.False);
        Assert.That(repo.GetValue(model.Id).N, Is.EqualTo(2));
    }
}
