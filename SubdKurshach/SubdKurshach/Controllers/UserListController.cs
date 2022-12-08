using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;

namespace SubdKurshach.Controllers
{
    public class UserListController : Controller
    {
        AppDbContext _context;

        public UserListController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ShowUserList()
        {
            UsersListViewModel usersListViewModel = new()
            {
                users = _context.users.Include(x => x.UserRole).OrderBy(x => x.RoleId).ToList()
            };
            return View(usersListViewModel);
        }
        [HttpGet]
        public ActionResult EditUserRole(string id)
        {
            User? user = _context.users.Find(id);
            return View(user);
        }
        [HttpPost]
        public ActionResult EditUserRole(User userEditRole)
        {
            User? user = _context.users.Find(userEditRole.Id);
            user.RoleId = userEditRole.RoleId;
            _context.SaveChanges();

            return RedirectToAction("ShowUserList", "UserList");
        }
    }
}
