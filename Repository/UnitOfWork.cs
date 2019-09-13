using Microsoft.Win32.SafeHandles;
using Stuart.Domain;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Runtime.InteropServices;

namespace Stuart.Repository
{
    public class UnitOfWork<T> : IUnitOfWork where T : DbContext
    {
        private readonly T Context;
        private DbContextTransaction Transaction;

        /// <summary>
        /// Construtor de Unidade de trabalho, injetada com os Contextos.
        /// </summary>
        /// <param name="context"/>
        public UnitOfWork(T context) => Context = context;

        /// <summary>
        /// Begin a db transaction context
        /// </summary>
        public void Begin() => Transaction = Context.Database.BeginTransaction();

        #region finishers
        protected bool Disposed { get; private set; } = false;
        protected SafeHandle Handle { get; } = new SafeFileHandle(IntPtr.Zero, true);

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
            if (Disposed) return;

            if (disposing)
            {
                Handle.Dispose();
                Transaction?.Dispose();
                Context.Dispose();
            }

            Disposed = true;
        }

        /// <summary>
        /// Try committing all changes in transaction and perform Rollback if fail
        /// </summary>
        public void Commit()
        {
            try
            {
                // commit transaction if there is one active
                Transaction?.Commit();
            }
            catch (DbException dbex)
            {
                // rollback if there was an exception
                Transaction?.Rollback();

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
                Transaction?.Rollback();
            }
            finally { Dispose(); }
        }
        #endregion
    }
}
