using Ara3D.Domo;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Services
{
    public interface IRepositoryManager
    {
        IReadOnlyList<IRepository> GetRepositories();
    }

    public class RepositoryManager : IRepositoryManager
    {
        public List<IRepository> Repositories { get; } = new List<IRepository>();

        public IReadOnlyList<IRepository> GetRepositories()
            => Repositories.ToList();

        public void AddRepository(IRepository repository)
            => Repositories.Add(repository);
    }

    public static class RepositoryManagerExtensions
    {
        public static ISingletonRepository<T> GetSingletonRepository<T>(this IRepositoryManager manager)
            => manager.GetRepositories().OfType<ISingletonRepository<T>>().FirstOrDefault();

        public static IAggregateRepository<T> GetAggregateRepository<T>(this IRepositoryManager manager)
            => manager.GetRepositories().OfType<IAggregateRepository<T>>().FirstOrDefault();
    }
}
