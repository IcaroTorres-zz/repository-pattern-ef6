using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Stuart.Repository
{
    public class Repository<TEntity> : Repository<TEntity, int>, IRepository<TEntity, int> where TEntity : Entity<int>
    {
        public Repository(DbContext context) : base(context) { }
    }

    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey>
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
        /// <returns cref="TEntity">Entity with given key.</returns>
        public TEntity Get(TKey key, string includes = "", bool isreadonly = false) =>
            isreadonly ? Find(e => e.Id.Equals(key), includes: includes).AsNoTracking().SingleOrDefault()
                       : Find(e => e.Id.Equals(key), includes: includes).SingleOrDefault();

        /// <summary>
        /// Get all entities from database entity set.
        /// </summary>
        /// <param name="isreadonly"></param>
        /// <returns cref="IQueryable{TEntity}">Entities in database set.</returns>
        public IQueryable<TEntity> GetAll(bool isreadonly = false) => isreadonly ? _context.Set<TEntity>().AsNoTracking() : _context.Set<TEntity>();

        /// <summary>
        /// Get all entities from database entity set, including desired navigation properties.
        /// </summary>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns cref="IQueryable{TEntity}">Entities in database set.</returns>
        public IQueryable<TEntity> GetAll(string includes, bool isreadonly = false) => Find(includes: includes, isreadonly: isreadonly);

        /// <summary>
        /// Retrieve entities with optional expression predicate, ordering and property includes.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderExpression"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <param name="includes"></param>
        /// <param name="isreadonly"></param>
        /// <returns cref="IQueryable{TEntity}">Entities in database set.</returns>
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate = null,
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderExpression = null,
                                        int? skip = null,
                                        int? top = null,
                                        string includes = "",
                                        bool isreadonly = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate ?? (e => true));

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
        public TEntity Add(TEntity entity) => _context.Set<TEntity>().Add(entity);

        /// <summary>
        /// Add given entities to database entity set.
        /// </summary>
        /// <param name="entities"></param>
        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().AddRange(entities);

        /// <summary>
        /// Set given entity to be updated to database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public TEntity Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        /// <summary>
        /// Set given entities to be updated to database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
            => entities.Select(e => { _context.Entry(e).State = EntityState.Modified; return e; });
        #endregion

        #region removals
        /// <summary>
        /// Remove given entity from database entity set.
        /// </summary>
        /// <param name="entity"></param>
        public TEntity Remove(TEntity entity) => _context.Set<TEntity>().Remove(entity);

        /// <summary>
        /// Remove an entity from database entity set got by given key.
        /// </summary>
        /// <param name="key"></param>
        public TEntity Remove(TKey key) => _context.Set<TEntity>().Remove(_context.Set<TEntity>().Find(key));

        /// <summary>
        /// Remove all given entities from database set.
        /// </summary>
        /// <param name="entities"></param>
        public IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().RemoveRange(entities);

        /// <summary>
        /// Remove all entities from database set with key in given keys.
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<TEntity> RemoveRange(IEnumerable<TKey> keys)
        {
            var entities = _context.Set<TEntity>()
                                   .Join(keys,
                                        entity => entity.Id,
                                        key => key,
                                        (entity, key) => entity);

            return _context.Set<TEntity>().RemoveRange(entities);
        }
        #endregion

        #region finishers
        /// <summary>
        /// Commit changes on entities to database or fails if no related context instance found.
        /// </summary>
        /// <returns cref="int">Number of changes.</returns>
        public int Commit()
        {
            if (_context != null) return _context.SaveChanges();
            else throw new NullReferenceException($"Commit fails. No instance of related {_context.GetType().Name} context found.");
        }

        /// <summary>
        /// Rollback changes on entities to database to avoid missmanipulation of errores,
        /// or fails if no related context instance found.
        /// </summary>
        public void Rollback()
        {
            if (_context != null) _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            else throw new NullReferenceException($"Commit fails. No instance of related {_context.GetType().Name} context found.");
        }
        #endregion
    }
}
