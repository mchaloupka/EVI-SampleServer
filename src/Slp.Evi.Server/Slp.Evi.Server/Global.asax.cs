using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Slp.Evi.Server.R2RML;
using VDS.RDF.Configuration;

namespace Slp.Evi.Server
{
    /// <summary>
    /// Mvc Application
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application start.
        /// </summary>
        protected void Application_Start()
        {
            ConfigurationLoader.AddObjectFactory(new R2RMLStorageFactoryForQueryHandler());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StorageWrapper.AppStart();
        }

        /// <summary>
        /// Application end.
        /// </summary>
        protected void Application_End()
        {
            StorageWrapper.AppEnd();
        }
    }
}
