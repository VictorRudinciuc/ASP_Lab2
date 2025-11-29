using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using projectAsp.Services;

namespace projectAsp.Controllers
{
    [Authorize] 
    public class AdminController : Controller
    {
        private readonly UserStore _store;
        public AdminController(UserStore store) => _store = store;

        public async Task<IActionResult> Users()
        {
            var list = await _store.GetAllAsync();
            return View(list); 
        }
    }
}