using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stuart.Domain
{
    public interface IRepository<T> : IRepository<T, int> where T : IEntity<int> { }

    public interface IRepository<T, K> where T : IEntity<K>
    {
        #region getters
        T Get(K key, string includes = "", bool isreadonly = false);
        IQueryable<T> GetAll(bool isreadonly = false);
        IQueryable<T> GetAll(string includes, bool isreadonly = false);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>> orderExpression = null,
                                int? skip = null,
                                int? top = null,
                                string includes = "",
                                bool isreadonly = false);
        #endregion

        #region setters
        T Add(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        T Update(T entity);
        IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        #endregion

        #region removals
        T Remove(T entity);
        T Remove(K key);
        IEnumerable<T> RemoveRange(IEnumerable<T> entities);
        IEnumerable<T> RemoveRange(IEnumerable<K> keys);
        #endregion
    }
}
