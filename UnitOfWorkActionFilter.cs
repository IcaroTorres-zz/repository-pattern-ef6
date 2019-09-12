using System.Data.Entity;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Synapse.Repository
{
    public class UnitOfWorkActionFilter : ActionFilterAttribute
    {
        public IUnitOfWork<DbContext> UnitOfWork { get; private set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            UnitOfWork = actionContext.Request.GetDependencyScope().GetService(typeof(IUnitOfWork<DbContext>)) as IUnitOfWork<DbContext>;
            UnitOfWork.BeginTransaction();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            UnitOfWork = actionExecutedContext.Request.GetDependencyScope().GetService(typeof(IUnitOfWork<DbContext>)) as IUnitOfWork<DbContext>;
            if (actionExecutedContext.Exception == null)
            {
                // commit if no exceptions
                UnitOfWork.Commit();
            }
            else
            {
                // rollback if exception
                UnitOfWork.Rollback();
            }
        }
    }
}