using Core.Repositories.Abstracts.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Abstracts
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext, IDisposable
    {
        public TContext Context { get; }

        public UnitOfWork(TContext context) => Context = context;

        public async Task<int> CompletedAsync() => await Context.SaveChangesAsync();

        public async Task DisposeAsync() => await Context.DisposeAsync();

        public void Dispose() => Context.Dispose();

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class => new Repository<TEntity>(Context);
    }
}