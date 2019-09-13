using System;

namespace Stuart.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Starts a transaction using the database context
        /// </summary>
        void Begin();

        /// <summary>
        /// Confirm changes made to transactional opened context.
        /// </summary>
        void Commit();

        /// <summary>
        ///  Rollback no Contexto do tipo de parâmetro passado.
        /// </summary>
        void Rollback();
    }
}
