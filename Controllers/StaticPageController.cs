using Microsoft.AspNetCore.Mvc;

namespace Servicii_publice.Controllers
{
	public class StaticPageController : Controller
	{
		public IActionResult Index() => View();

		public IActionResult About()
		{
			ViewData["Title"] = "Despre Servicii Publice";
			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Title"] = "Contact Servicii Publice";
			return View();
		}

		public IActionResult FAQ()
		{
			ViewData["Title"] = "Întrebări frecvente";
			return View();
		}
	}
}
