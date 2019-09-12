using System;
using System.Data.Entity;

namespace Synapse.Repository
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// Inicia um contexto transacional
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Confirma alterações do Contexto do tipo de parâmetro passado.
        /// </summary>
        void Commit();

        /// <summary>
        ///  Rollback no Contexto do tipo de parâmetro passado.
        /// </summary>
        void Rollback();
    }
}
