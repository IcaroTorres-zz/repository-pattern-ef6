using Stuart.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Stuart.Repository
{
    public class Repository<T> : Repository<T, int>, IRepository<T, int> where T : Entity<int>
    {
        public Repository(DbContext context) : base(context) { }
    }

    public class Repository<T, K> : IRepository<T, K> where T : Entity<K>
    {
        protected DbContext _context;
        public Repository(DbContext context) => _context = context;

        #region getters
        /// <summary>
        /// Get an entity from database entity set with given key, including desired navigation properties.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns cref="T">Entity with given key.</returns>
        public T Get(K key, string includes = "", bool isreadonly = false) =>
            isreadonly ? Find(e => e.Id.Equals(key), includes: includes).AsNoTracking().SingleOrDefault()
                       : Find(e => e.Id.Equals(key), includes: includes).SingleOrDefault();

        /// <summary>
        /// Get all entities from database entity set.
        /// </summary>
        /// <param name="isreadonly"></param>
        /// <returns cref="IQueryable{T}">Entities in database set.</returns>
        public IQueryable<T> GetAll(bool isreadonly = false) => isreadonly ? _context.Set<T>().AsNoTracking() : _context.Set<T>();

        /// <summary>
        /// Get all entities from database entity set, including desired navigation properties.
        /// </summary>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns cref="IQueryable{T}">Entities in database set.</returns>
        public IQueryable<T> GetAll(string includes, bool isreadonly = false) => Find(includes: includes, isreadonly: isreadonly);

        /// <summary>
        /// Retrieve entities with optional expression predicate, ordering and property includes.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderExpression"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns cref="IQueryable{T}">Entities in database set.</returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderExpression = null,
                                        int? skip = null,
                                        int? top = null,
                                        string includes = "",
                                        bool isreadonly = false)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate ?? (e => true));

            orderExpression = orderExpression ?? (q => q.OrderBy(e => e.Id));

            query = top != null ? orderExpression(query).Skip(skip ?? 0).Take(top.Value)
                                : orderExpression(query).Skip(skip ?? 0);

            foreach (var property in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(property);

            return isreadonly ? query.AsNoTracking() : query;
        }
        #endregion

        #region setters
        /// <summary>
        /// Add given entity to database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public T Add(T entity) => _context.Set<T>().Add(entity);

        /// <summary>
        /// Add given entities to database entity set.
        /// </summary>
        /// <param name="entities"></param>
        public IEnumerable<T> AddRange(IEnumerable<T> entities) => _context.Set<T>().AddRange(entities);

        /// <summary>
        /// Set given entity to be updated to database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        /// <summary>
        /// Set given entities to be updated to database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
            => entities.Select(e => { _context.Entry(e).State = EntityState.Modified; return e; });
        #endregion

        #region removals
        /// <summary>
        /// Remove given entity from database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public T Remove(T entity) => _context.Set<T>().Remove(entity);

        /// <summary>
        /// Remove an entity from database entity set got by given key.
        /// </summary>
        /// <param name="key"></param>
        public T Remove(K key) => _context.Set<T>().Remove(_context.Set<T>().Find(key));

        /// <summary>
        /// Remove all given entities from database set.
        /// </summary>
        /// <param name="entities"></param>
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

        /// <summary>
        /// Remove all entities from database set with key in given keys.
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<T> RemoveRange(IEnumerable<K> keys)
        {
            var entities = _context.Set<T>()
                                   .Join(keys,
                                        entity => entity.Id,
                                        key => key,
                                        (entity, key) => entity);

            return _context.Set<T>().RemoveRange(entities);
        }
        #endregion
    }
}
