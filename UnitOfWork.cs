using Microsoft.Win32.SafeHandles;
using Ninject;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Runtime.InteropServices;

namespace Synapse.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        [Inject]
        private TContext Context { get; set; }
        private DbContextTransaction Transaction;

        /// <summary>
        /// Construtor de Unidade de trabalho, injetada com os Contextos.
        /// </summary>
        /// <param name="context"/>
        public UnitOfWork() { }

        /// <summary>
        /// Begin a db transaction context
        /// </summary>
        public void BeginTransaction() => Transaction = Context.Database.BeginTransaction();

        #region finishers
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Instantiate a SafeHandle instance.
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// Dispose all unmanaged objects and the opened context
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        /// <summary>
        /// Dispose all unmanaged objects and the opened context
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                handle.Dispose();
                Transaction.Dispose();
                Context.Dispose();
            }

            disposed = true;
        }
        #endregion

        /// <summary>
        /// Try committing all changes in transaction and perform Rollback if fail
        /// </summary>
        public void Commit()
        {
            try
            {
                // commit transaction if there is one active
                if (Transaction != null)
                    Transaction.Commit();
            }
            catch(DbException dbex)
            {
                // rollback if there was an exception
                if (Transaction != null)
                    Transaction.Rollback();

                throw dbex;
            }
            finally { Dispose(); }
        }

        /// <summary>
        /// Discard all unsaved changes, dispatched when Commit fails and used when some part of a transaction fails
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (Transaction != null)
                    Transaction.Rollback();
            }
            finally { Dispose(); }
        }
    }
}
