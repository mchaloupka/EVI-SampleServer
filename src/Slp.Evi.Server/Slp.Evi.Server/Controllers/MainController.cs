using System.Web.Mvc;
using Slp.Evi.Server.R2RML;

namespace Slp.Evi.Server.Controllers
{
    /// <summary>
    /// Main controller
    /// </summary>
    public class MainController : Controller
    {
        /// <summary>
        /// The index page
        /// </summary>
        public ActionResult Index()
        {
            return RedirectToAction("Sample");
        }

        /// <summary>
        /// The query page
        /// </summary>
        [ValidateInput(false)]
        public ActionResult Query(string query)
        {
            return View(null, null, query);
        }

        /// <summary>
        /// The page informing that the application start failed.
        /// </summary>
        public ActionResult AppStartFailed()
        {
            return View(StorageWrapper.StartException);
        }

        public ActionResult Mapping()
        {
            return View();
        }

/// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (StorageWrapper.StartException != null && filterContext.ActionDescriptor.ActionName != nameof(AppStartFailed))
            {
                filterContext.Result = RedirectToAction(nameof(AppStartFailed));
            }
        }

        public ActionResult Sample()
        {
            return View();
        }
    }
}