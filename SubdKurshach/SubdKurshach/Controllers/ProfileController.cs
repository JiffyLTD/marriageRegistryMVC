using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;

namespace SubdKurshach.Controllers
{
    public class ProfileController : Controller
    {
        private AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Profile()
        {
            User? user = _context.Users.Where(x => x.UserName == User.Identity.Name)
                .Include(x => x.UserPassport).ThenInclude(x => x.AllChildrens).ThenInclude(x => x.Child)
                .Include(x => x.UserAddress).FirstOrDefault();

            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public ActionResult ProfileEdit()
        {
            User? user = _context.Users.Where(x => x.UserName == User.Identity.Name)
               .Include(x => x.UserPassport)
               .Include(x => x.UserAddress).FirstOrDefault();

            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult ProfileEdit(User userEdit)
        {
            User? user = _context.Users.Where(x => x.UserName == User.Identity.Name)
              .Include(x => x.UserPassport)
              .Include(x => x.UserAddress).FirstOrDefault();

            if (user != null)                                                                                               
            {
                user.FirstName = userEdit.FirstName;
                user.LastName = userEdit.LastName;
                user.Patronymic = userEdit.Patronymic;
                user.PhoneNumber = userEdit.PhoneNumber;
                user.ProfilePhoto = userEdit.ProfilePhoto;
                user.UserPassport.PassportSeries = userEdit.UserPassport.PassportSeries;
                user.UserPassport.PassportNum = userEdit.UserPassport.PassportNum;
                user.UserPassport.PassportDate = userEdit.UserPassport.PassportDate;
                user.UserPassport.Birthday = userEdit.UserPassport.Birthday;
                user.UserPassport.PassportIssuedByWhom = userEdit.UserPassport.PassportIssuedByWhom;
                user.UserAddress.Country = userEdit.UserAddress.Country;
                user.UserAddress.City = userEdit.UserAddress.City;
                user.UserAddress.Street = userEdit.UserAddress.Street;
                user.UserAddress.HouseNum = userEdit.UserAddress.HouseNum;
                user.UserAddress.AppartNum = userEdit.UserAddress.AppartNum;

                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return RedirectToAction("Profile", "Profile");
        }
    }
}
