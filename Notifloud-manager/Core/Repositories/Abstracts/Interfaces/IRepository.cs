using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Abstracts.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        public DbContext Context { get; }
        public DbSet<T> DBSet { get; }
    }
}
