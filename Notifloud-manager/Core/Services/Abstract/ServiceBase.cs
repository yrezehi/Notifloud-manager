using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using Core.Services.Abstract.Interfaces;
using Core.Models.DTO;
using Core.Utils;
using Core.Models.Abstracts.Interfaces;
using Core.Repositories.Abstracts.Interfaces;
using Core.Repositories.Extensions;

namespace Core.Services.Abstract
{
    public class ServiceBase<T> : IDisposable, IServiceBase<T> where T : class
    {
        public ServiceBase(IUnitOfWork unitOfWork) =>
            (DBSet, UnitOfWork, DbContext) = (unitOfWork.Repository<T>().DBSet, unitOfWork, unitOfWork.Repository<T>().Context);

        protected internal IUnitOfWork UnitOfWork { get; set; }
        protected internal DbContext DbContext { get; set; }
        protected DbSet<T> DBSet { get; set; }

        private static int DEFAULT_PAGE_SIZE = 10;

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression) =>
            DBSet.Where(expression);

        public virtual bool Any(Expression<Func<T, bool>> expression) =>
            DBSet.Any(expression);

        public virtual async Task<int> Count(Expression<Func<T, bool>>? expression = null) =>
            await (expression != null ? DBSet.CountAsync(expression) : DBSet.CountAsync());

        public virtual IQueryable<T> OrderBy<TValue>(Expression<Func<T, TValue>> orderByExpression) =>
            DBSet.OrderBy(orderByExpression);

        public virtual async Task<IEnumerable<T>> GetAll(int? page = null) =>
            page == null ? await DBSet.ToListAsync() : await DBSet.PaginateQuerable(page.Value, DEFAULT_PAGE_SIZE).ToListAsync();

        public virtual async Task<PaginateDTO<T>> Paginate(int currentPage, Expression<Func<T, bool>>? expression = null)
        {
            var items = DBSet.ConditionalWhere(expression != null, expression!).Skip(currentPage * 10).Take(DEFAULT_PAGE_SIZE);
            var itemsCount = await DBSet.ConditionalCount(expression!);

            return new PaginateDTO<T>()
            {
                Items = items,
                Page = currentPage,
                Total = itemsCount,
                Pages = (int)Math.Ceiling((double)itemsCount / 10),
            };
        }

        public async Task<IEnumerable<T>> SearchByProperty<TValue>(string propertyName, TValue value, int? page)
        {
            // TODO: nah, fix you laz..
            if (value is string && string.IsNullOrEmpty(value as string))
            {
                return page == null ? await DBSet.ToListAsync() : await DBSet.PaginateQuerable(page.Value, DEFAULT_PAGE_SIZE).ToListAsync();
            }

            IEntity enetityInstance = (IEntity)Activator.CreateInstance(typeof(T))!;

            if (!ReflectionUtil.ContainsProperty(enetityInstance, propertyName))
                throw new ArgumentException($"Property is not allowed to be searched or does not exists!");

            ParameterExpression parameter = Expression.Parameter(typeof(T), "property");
            MemberExpression body = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!;
            MethodCallExpression expression = Expression.Call(body, containsMethod, Expression.Constant(value, typeof(string)), Expression.Constant(StringComparison.OrdinalIgnoreCase));

            var predicate = Expression.Lambda<Func<T, bool>>(expression, parameter);

            IEnumerable<T> enetities = page == null ? await DBSet.Where(predicate).ToListAsync() : await DBSet.Where(predicate).PaginateQuerable(page.Value, DEFAULT_PAGE_SIZE).ToListAsync();

            return enetities;
        }

        public async Task<T> FindByProperty<TValue>(Expression<Func<T, TValue>> selector, TValue value)
        {
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(selector.Body, Expression.Constant(value, typeof(TValue))), selector.Parameters);

            T? entity = await DBSet.FirstOrDefaultAsync(predicate);

            if (entity == null)
                throw new Exception($"Find by property was not found!");

            return entity;
        }

        public async Task<IEnumerable<T>> FindAllByProperty<TValue>(Expression<Func<T, TValue>> selector, TValue value)
        {
            var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(selector.Body, Expression.Constant(value, typeof(TValue))), selector.Parameters);

            IEnumerable<T> entites = await DBSet.Where(predicate).ToListAsync();

            if (!entites.Any())
                throw new ArgumentException($"Find all by property was not found!");

            return entites;
        }

        public virtual async Task<T> FindById(int id)
        {
            var entity = await DBSet.FindAsync(id);

            if (entity == null)
                throw new ArgumentException("Entity Not Found");

            return entity;
        }

        public virtual async Task<T?> NullableFindById(int id) =>
            await DBSet.FindAsync(id);


        public virtual async Task<T> Delete(int id)
        {
            var targetEntitiy = await DBSet.FindAsync(id);

            if (targetEntitiy != null)
            {
                DBSet.Remove(targetEntitiy);

                return targetEntitiy;
            }

            throw new ArgumentException(nameof(IEntity));
        }

        public async Task<T> Update(IEntity entityToUpdate)
        {
            T entity = await DBSet.FirstOrDefaultAsync(entity => ((IEntity)entity).Id == entityToUpdate.Id);

            if (entity != null)
            {
                var dtoProperties = ReflectionUtil.GetInterfacedObjectProperties(entityToUpdate.GetType());

                foreach (var property in dtoProperties)
                {
                    var dtoPropertyValue = ReflectionUtil.GetValueOf(entityToUpdate, property.Name);

                    if (dtoPropertyValue != null)
                    {
                        if (ReflectionUtil.ContainsProperty(entity, property.Name))
                            ReflectionUtil.SetValueOf(entity, property.Name, dtoPropertyValue);
                    }
                }

                DBSet.Update(entity);

                return entity;
            }

            throw new ArgumentException(nameof(T));
        }

        public async Task<T> Update(T entityToUpdate)
        {
            var updatedEntity = await DBSet.AsNoTracking().FirstOrDefaultAsync(entity => ((IEntity)entityToUpdate).Id == ((IEntity)entity).Id);

            if (updatedEntity != null)
            {
                ReflectionUtil.MapEntity<T>((IEntity)entityToUpdate, (IEntity)updatedEntity);

                DBSet.Update(updatedEntity);

                await UnitOfWork.CompletedAsync();

                return updatedEntity;
            }

            throw new ArgumentException("Entity Not Found");
        }

        public async Task<T> UpdateNecessary(T entityToUpdate, int id)
        {
            var entityToBeUpdated = await DBSet.AsNoTracking().FirstOrDefaultAsync(entity => ((IEntity)entity).Id == id);

            if (entityToBeUpdated != null)
            {
                foreach (var property in ReflectionUtil.GetObjectProperties(entityToUpdate.GetType()))
                {
                    var propertyValue = ReflectionUtil.GetValueOf(entityToUpdate, property.Name);

                    if (!ReflectionUtil.IsNullOrDefault(propertyValue))
                    {
                        ReflectionUtil.SetValueOf(entityToBeUpdated, property.Name, propertyValue);
                    }
                }

                DBSet.Update(entityToBeUpdated);

                await UnitOfWork.CompletedAsync();

                return entityToBeUpdated;
            }

            throw new ArgumentException("Entity Not Found");
        }

        public async Task<T> Create(T entity)
        {
            await DBSet.AddAsync(entity);

            await UnitOfWork.CompletedAsync();

            return entity;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
