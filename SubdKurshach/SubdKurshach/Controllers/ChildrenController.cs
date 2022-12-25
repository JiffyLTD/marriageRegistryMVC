using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Users;

namespace SubdKurshach.Controllers
{
    public class ChildrenController : Controller
    {
        private AppDbContext _context;
        private UserManager<User> _userManager;

        public ChildrenController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult AddChild()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddChild(Child newChild)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);

            Child child = new()
            {
                FirstName = newChild.FirstName,
                LastName = newChild.LastName,
                Patronymic = newChild.Patronymic,
                BirthCertificate= newChild.BirthCertificate,
                Birthday = newChild.Birthday
            };
            _context.children.Add(child);
            _context.SaveChanges();

            AllChildrens allChildrens = new()
            {
                PassportId = user.PassportId,
                ChildId = child.ChildId
            };
            _context.allChildrens.Add(allChildrens);
            _context.SaveChanges();

            return RedirectToAction("Profile", "Profile");
        }
        [HttpGet]
        public async Task<ActionResult> EditChild(int? id)
        {
            Child? child = _context.children.Find(id);

            return View(child);
        }
        [HttpPost]
        public async Task<ActionResult> EditChild(Child childEdit)
        {
            Child? child = _context.children.Find(childEdit.ChildId);

            if(child != null)
            {
                child.FirstName = childEdit.FirstName;
                child.LastName = childEdit.LastName;
                child.Patronymic = childEdit.Patronymic;
                child.BirthCertificate = childEdit.BirthCertificate;
                child.Birthday = childEdit.Birthday;

                _context.Entry(child).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return RedirectToAction("Profile", "Profile");
        }
        public ActionResult DeleteChild(int? id)
        {
            Child? child = _context.children.Find(id);

            if (child != null)
            {
                _context.allChildrens.Remove(_context.allChildrens.Where(x => x.ChildId == child.ChildId).FirstOrDefault());

                _context.children.Remove(child);
                _context.SaveChanges();
            }

            return RedirectToAction("Profile", "Profile");
        }
    }
}
