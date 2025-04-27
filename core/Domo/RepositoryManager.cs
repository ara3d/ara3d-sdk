using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Domo
{
    public class RepositoryManager 
        : IRepositoryManager    
    {
        private readonly IList<IRepository> _repositories = new List<IRepository>();

        public void Dispose()
        {
            RepositoryChanged = null;
            this.DeleteAllRepositories();
        }

        public IRepository AddRepository(IRepository repository)
        {
            _repositories.Add(repository);
            RepositoryChanged?.Invoke(this, new RepositoryChangeArgs 
                { ChangeType = RepositoryChangeType.RepositoryAdded, Repository = repository });
            repository.RepositoryChanged += (sender, args) => RepositoryChanged?.Invoke(sender, args);
            return repository;
        }

        public IReadOnlyList<IRepository> GetRepositories()
            => _repositories.ToList();

        public void DeleteRepository(IRepository repository)
        {
            _repositories.Remove(repository);
            repository.Dispose();
            RepositoryChanged?.Invoke(this, new RepositoryChangeArgs 
                { ChangeType = RepositoryChangeType.RepositoryDeleted, Repository = repository});
        }

        public IRepository GetRepository(Type type)
            => _repositories.FirstOrDefault(r => r.ValueType == type);

        public event EventHandler<RepositoryChangeArgs> RepositoryChanged;
    }
}
