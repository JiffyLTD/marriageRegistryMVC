using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Families;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;
using System.Diagnostics;

namespace SubdKurshach.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _context;
        SignInManager<User> SignInManager;

        public HomeController(SignInManager<User> signInManager, AppDbContext context)
        {
            SignInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            if (SignInManager.IsSignedIn(User))
            {
                var marriage = _context.marriages.Include(x => x.Wife).ThenInclude(x => x.User)
                                                 .Include(x => x.Husband).ThenInclude(x => x.User).ToList();

                var divorce = _context.divorces.Include(x => x.Family).ThenInclude(x => x.Marriage).ThenInclude(x => x.Wife).ThenInclude(x => x.User).
                                                Include(x => x.Family).ThenInclude(x => x.Marriage).ThenInclude(x => x.Husband).ThenInclude(x => x.User).ToList();

                divorce.Reverse();
                marriage.Reverse();

                AllMarriagesViewModel allMarriages = new()
                {
                    marriages = marriage,
                    divorces = divorce
                };

                return View(allMarriages);
            }
            else
                return RedirectToAction("Login","Account");
        }
    }
}