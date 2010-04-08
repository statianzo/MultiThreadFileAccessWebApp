using System;
using System.IO;
using System.Web.Mvc;
using MultithreadedFileAccessWebApp.Framework;

namespace MultithreadedFileAccessWebApp.Controllers
{
	public class FileProviderController : Controller
	{
		private static readonly string StorageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage");

		public ActionResult Index()
		{
			return new ContentResult { Content = "Why hello there" };
		}
		public ActionResult GetFile(string fileName)
		{
			if (QueuedWriter.CompletedFiles.Contains(fileName))
				return new FilePathResult(Path.Combine(StorageDir, fileName), "text/html");
			QueuedWriter.Enqueue(fileName);
			return new ContentResult { Content = DataAccess.GetDescription(fileName), ContentType = "text/html"};
		}
	}
}