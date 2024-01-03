using Core.Repositories.Abstracts.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Abstracts
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public DbContext Context { get; }
        public DbSet<T> DBSet { get; }

        public Repository(DbContext context)
        {
            Context = context;
            DBSet = Context.Set<T>();
        }

        public void Dispose() => throw new NotImplementedException();
    }
}
