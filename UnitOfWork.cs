using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Stuart.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, dynamic> contextDictionary;
        private readonly Dictionary<Type, dynamic> repositoryDictionary;

        #region constructor
        /// <summary>
        /// Construtor de Unidade de trabalho, injetada com os Contextos.
        /// </summary>
        /// <param name="contexts"/>
        public UnitOfWork(IEnumerable<DbContext> contexts)
        {
            if (contextDictionary == null)
                contextDictionary = new Dictionary<Type, dynamic>();

            if (repositoryDictionary == null)
                repositoryDictionary = new Dictionary<Type, dynamic>();

            foreach (var ctx in contexts)
                if (!contextDictionary.ContainsKey(ctx.GetType())) contextDictionary.Add(ctx.GetType(), ctx);
        }
        #endregion

        #region getters
        /// <summary>
        /// Genérico <typeparamref name="TContext"/> para obter o Contexto do tipo de parâmetro passado.
        /// </summary>
        /// <typeparam name="TContext">Um tipo que herda de DbContext.</typeparam>
        public TContext Context<TContext>() where TContext : DbContext
        {
            contextDictionary.TryGetValue(typeof(TContext), out dynamic _ctx);

            return _ctx ?? (TContext)Activator.CreateInstance(typeof(TContext));
        }

        /// <summary>
        /// Generic implementation of repository getter using entity and key  types as type parameters
        /// </summary>
        /// <typeparam name="TRepo">type inheriting Repository.</typeparam>
        public Repository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : Entity<TKey>
        {
            Type repositoryType = typeof(Repository<TEntity, TKey>);

            if (repositoryDictionary.TryGetValue(repositoryType, out dynamic repository))
                return repository;
            
            Type contextType = repositoryType.GetProperty("Context").PropertyType;
            var context = contextDictionary[contextType];

            var repo = (Repository<TEntity, TKey>)Activator.CreateInstance(repositoryType, context);
            repositoryDictionary.Add(repositoryType, repo);
            return repo;
        }

        /// <summary>
        /// Generic implementation of repository getter using a entity type with int key as type parameter
        /// </summary>
        /// <typeparam name="TRepo">type inheriting Repository.</typeparam>
        public Repository<TEntity> Repository<TEntity>() where TEntity : Entity<int> => (Repository<TEntity>) Repository<TEntity, int>();
        #endregion

        #region finishers
        /// <summary>
        /// Generic saveChanges implementation using context as type parameter
        /// </summary>
        public int Commit<TContext>() where TContext : DbContext
        {
            if (contextDictionary.TryGetValue(typeof(TContext), out dynamic ctx))
                return ctx.SaveChanges();
            else
                throw new NullReferenceException($"Commit fails. No instance of related {typeof(TContext).Name} context found.");
        }

        /// <summary>
        /// Generic rollback implementation using context as type parameter
        /// </summary>
        public void Rollback<TContext>() where TContext : DbContext
        {
            if (contextDictionary.TryGetValue(typeof(TContext), out dynamic ctx))
                (ctx as DbContext).ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }

        /// <summary>
        /// Dispose all UnitOfWork's context instances
        /// </summary>
        public void Dispose() => contextDictionary.ToList().ForEach(pair => (pair.Value as DbContext).Dispose());
        #endregion
    }
}