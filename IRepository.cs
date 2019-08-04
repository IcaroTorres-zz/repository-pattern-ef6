using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stuart.Repository
{
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : Entity<int> { }

    public interface IRepository<TEntity, TKey> where TEntity : Entity<TKey>
    {
        #region getters
        TEntity Get(TKey key, string includes = "", bool isreadonly = false);
        IQueryable<TEntity> GetAll(bool isreadonly = false);
        IQueryable<TEntity> GetAll(string includes, bool isreadonly = false);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate = null,
                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderExpression = null,
                                int? skip = null,
                                int? top = null,
                                string includes = "",
                                bool isreadonly = false);
        #endregion

        #region setters
        TEntity Add(TEntity entity);
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);
        TEntity Update(TEntity entity);
        IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);
        #endregion

        #region removals
        TEntity Remove(TEntity entity);
        TEntity Remove(TKey key);
        IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> RemoveRange(IEnumerable<TKey> keys);
        #endregion

        #region finishers
        int Commit();
        void Rollback();
        #endregion
    }
}
