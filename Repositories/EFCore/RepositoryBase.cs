﻿using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System.Linq.Expressions;


namespace Repositories.EFCore
{
    public class RepositoryBase<T>(RepositoryContext context) : IRepositoryBase<T> where T : class
    {
        protected readonly RepositoryContext _context = context;

        public void Create(T entity) => _context.Set<T>().Add(entity);

        public void CreateRange(IEnumerable<T> entities) => _context.Set<T>().AddRange(entities);

        public void Delete(T entity) => _context?.Set<T>().Remove(entity);

        public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ?
            _context.Set<T>().AsNoTracking() :
            _context.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
            bool trackChanges) =>
            !trackChanges ?
            _context.Set<T>().Where(expression).AsNoTracking() :
            _context.Set<T>().Where(expression);

        public void Update(T entity) => _context.Set<T>().Update(entity);
    }
}
