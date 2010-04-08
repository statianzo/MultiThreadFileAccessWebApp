using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MultithreadedFileAccessWebApp.Framework;

namespace MultithreadedFileAccessWebApp
{
	public class MvcApplication : HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new {controller = "FileProvider", action = "Index", id = ""} // Parameter defaults
				);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterRoutes(RouteTable.Routes);

			DataAccess.StartUp();
			QueuedWriter.StartWorker();
		}
	}
}