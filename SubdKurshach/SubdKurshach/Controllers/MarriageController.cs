using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Families;
using SubdKurshach.Models.Info;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;

namespace SubdKurshach.Controllers
{
    public class MarriageController : Controller
    {
        AppDbContext _context;
        UserManager<User> _userManager;

        public MarriageController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult NewMarriage()
        {
            var allHusbands = _context.husbands.Include(x => x.User).ToList();
            var allHusbandsInFamilies = _context.marriages.ToList();

            List<Husband> unmarriedHusbands = new();

            foreach(var husbands in allHusbands)
            {
                foreach(var familyHusband in allHusbandsInFamilies)
                {
                    if (husbands.HusbandId != familyHusband.HusbandId)
                    {
                        unmarriedHusbands.Add(husbands);               //ищем всех незамужних мужчин
                    }
                }
            }

            var allWives = _context.wives.Include(x => x.User).ToList();
            var allWivesInFamilies = _context.marriages.ToList();

            List<Wife> unmarriedWives = new();

            foreach (var wives in allWives)
            {
                foreach (var familyWife in allWivesInFamilies)
                {
                    if (wives.WifeId != familyWife.WifeId)
                    {
                        unmarriedWives.Add(wives);               //ищем всех незамужних женщин
                    }
                }
            }

            AllUsersForMarriageViewModel marriageViewModel = new()
            {
                husbands = unmarriedHusbands,
                wives = unmarriedWives
            };
            return View(marriageViewModel);
        }
        [HttpPost]
        public async Task<ActionResult> NewMarriage(AllUsersForMarriageViewModel marriageViewModel)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Employee employee = _context.employees.Where(x => x.UserId == user.Id).First();

            Marriage marriage = new()
            {
                HusbandId = marriageViewModel.HusbandId,
                WifeId = marriageViewModel.WifeId,
                EmployeeId = employee.EmployeeId,
                MarriageDate = DateTime.Now
            };

            _context.marriages.Add(marriage);
            _context.SaveChanges();

            Wife? wife = _context.wives.Where(x => x.WifeId == marriageViewModel.WifeId)
                .Include(x => x.User)
                .ThenInclude(x => x.UserPassport).FirstOrDefault();

            if (wife != null)
            {
                List<AllChildrens>? allChildrens = _context.allChildrens.Where(x => x.PassportId == wife.User.UserPassport.PassportId)
                    .Include(x => x.Child).ToList();

                var i = 0;
                foreach (var children in allChildrens) {
                    if (children.ChildId != null)
                    {
                        Family family = new()
                        {
                            MarriageId = marriage.MarriageId,
                            AllChildrensId = children.ChildrensId
                        };
                        _context.families.Add(family);
                        _context.SaveChanges();
                        break;
                    }
                    else if(i > 0)
                    {
                        Family family = new()
                        {
                            MarriageId = marriage.MarriageId
                        };
                        _context.families.Add(family);
                        _context.SaveChanges();
                        break;
                    }
                    i++;
                }
            }

            return RedirectToAction("Index","Home");
        }
    }
}
