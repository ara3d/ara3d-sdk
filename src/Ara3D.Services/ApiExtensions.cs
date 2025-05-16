using Ara3D.Domo;
using System.Linq;

namespace Ara3D.Services
{
    public static class ApiExtensions
    {
        public static TService GetService<TService>(this IServiceManager app) where TService: IService
            => app.GetServices().OfType<TService>().FirstOrDefault();

        public static TRepo GetRepo<TRepo>(this IServiceManager app) where TRepo: IRepository
            => app.GetRepositories().OfType<TRepo>().FirstOrDefault();
    }
}