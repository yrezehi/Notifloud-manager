﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Repositories.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<IEntity> ConditionalWhere<IEntity>(this DbSet<IEntity> source, bool condition, Expression<Func<IEntity, bool>> predicate) where IEntity : class =>
            condition ? source.Where(predicate) : source;

        public static async Task<int> ConditionalCount<IEntity>(this DbSet<IEntity> source, Expression<Func<IEntity, bool>> predicate) where IEntity : class =>
            predicate != null ? await source.Where(predicate).AsQueryable().CountAsync() : await source.CountAsync();

        public static IQueryable<IEntity> PaginateQuerable<IEntity>(this IQueryable<IEntity> source, int page, int size) where IEntity : class =>
            source.Skip(page).Take(size);
    }
}
