using Domain.Entities;
using System;
using System.Data.Entity;

namespace Stuart.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Genérico <typeparamref name="TContext"/> para obter o Contexto do tipo de parâmetro passado.
        /// </summary>
        /// <typeparam name="TContext">Um tipo que herda de DbContext.</typeparam>
        TContext Context<TContext>() where TContext : DbContext;

        /// <summary>
        /// Genérico para obter o Repositório desejado através tipos de parâmetro Repositório.
        /// </summary>s
        /// <typeparam name="TRepo">Um tipo que herda de Repository.</typeparam>
        Repository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : Entity<TKey>;
        
        /// <summary>
        /// Genérico para obter o Repositório desejado através tipos de parâmetro Repositório.
        /// </summary>s
        /// <typeparam name="TRepo">Um tipo que herda de Repository.</typeparam>
        Repository<TEntity> Repository<TEntity>() where TEntity : Entity<int>;

        /// <summary>
        /// Genérico para salvar alterações do Contexto do tipo de parâmetro passado.
        /// </summary>
        int Commit<TContext>() where TContext : DbContext;

        /// <summary>
        /// Genérico para Rollback no Contexto do tipo de parâmetro passado.
        /// </summary>
        void Rollback<TContext>() where TContext : DbContext;
    }
}