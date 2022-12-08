using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Info;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;

namespace SubdKurshach.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _context;
        private UserManager<User> _userManager; 
        private SignInManager<User> _signInManager; 

        public AccountController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                UserAddress userAddress = new ()
                {
                    Country = registerViewModel.Country,
                    City= registerViewModel.City,
                    Street= registerViewModel.Street,
                    HouseNum= registerViewModel.HouseNum,
                    AppartNum= registerViewModel.AppartNum
                };
                _context.userAddresses.Add(userAddress);

                UserPassport userPassport = new()
                {
                    PassportSeries= registerViewModel.PassportSeries,
                    PassportNum = registerViewModel.PassportNum,
                    PassportDate= registerViewModel.PassportDate,
                    PassportIssuedByWhom = registerViewModel.PassportIssuedByWhom,
                    Birthday= registerViewModel.Birthday
                };
                _context.usersPassports.Add(userPassport);
                _context.SaveChanges();

                AllChildrens allChildrens = new()
                {
                    PassportId = userPassport.PassportId
                };
                _context.allChildrens.Add(allChildrens);
                _context.SaveChanges();

                User user = new();
                Employee? employee = null;
                if (_context.Users.ToList().Count() == 0)
                {
                    user.UserName = registerViewModel.Email;
                    user.Email = registerViewModel.Email;
                    user.FirstName = registerViewModel.FirstName;
                    user.LastName = registerViewModel.LastName;
                    user.Patronymic = registerViewModel.Patronymic;
                    user.PhoneNumber = registerViewModel.PhoneNum;
                    user.ProfilePhoto = registerViewModel.ProfilePhoto;
                    user.RegisterDate = DateTime.Now;
                    user.AddresId = userAddress.AddressId;
                    user.PassportId = userPassport.PassportId;
                    user.RoleId = 1;

                    employee = new()
                    {
                        UserId = user.Id
                    };
                }
                else if (!registerViewModel.isUser)
                {
                    user.UserName = registerViewModel.Email;
                    user.Email = registerViewModel.Email;
                    user.FirstName = registerViewModel.FirstName;
                    user.LastName = registerViewModel.LastName;
                    user.Patronymic = registerViewModel.Patronymic;
                    user.PhoneNumber = registerViewModel.PhoneNum;
                    user.ProfilePhoto = registerViewModel.ProfilePhoto;
                    user.RegisterDate = DateTime.Now;
                    user.AddresId = userAddress.AddressId;
                    user.PassportId = userPassport.PassportId;
                    user.RoleId = 2;

                    employee = new()
                    {
                        UserId = user.Id
                    };
                }
                else
                {
                    user.UserName = registerViewModel.Email;
                    user.Email = registerViewModel.Email;
                    user.FirstName = registerViewModel.FirstName;
                    user.LastName = registerViewModel.LastName;
                    user.Patronymic = registerViewModel.Patronymic;
                    user.PhoneNumber = registerViewModel.PhoneNum;
                    user.ProfilePhoto = registerViewModel.ProfilePhoto;
                    user.RegisterDate = DateTime.Now;
                    user.AddresId = userAddress.AddressId;
                    user.PassportId = userPassport.PassportId;
                    user.RoleId = 3;
                }
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);//добавляем нового пользователя в БД

                if (result.Succeeded)
                {
                    if (employee != null)
                    {
                        _context.employees.Add(employee);
                        _context.SaveChanges();
                    }

                    if (registerViewModel.Gender == "Male")
                    {
                        Husband husband = new()
                        {
                            UserId = user.Id
                        };
                        _context.husbands.Add(husband);
                        _context.SaveChanges();
                    }
                    else
                    {
                        Wife wife = new()
                        {
                            UserId = user.Id
                        };
                        _context.wives.Add(wife);
                        _context.SaveChanges();
                    }
                    await _signInManager.SignInAsync(user, false);//устанавливаем куки
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("RegisterError", error.Description);  //ошибка регистрации
                    }
                }
            }
            return View(registerViewModel);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe, false);//проверка параметров входа
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && Url.IsLocalUrl(loginViewModel.ReturnUrl))
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Неправильный логин и (или) пароль");//вывод ошибки входа
                }
            }
            return View(loginViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();//Удаляем куки
            return RedirectToAction("Login", "Account");
        }

        public async Task<ActionResult> DeleteProfile()
        {
            User? user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                Husband husband = _context.husbands.Where(x => x.UserId == user.Id).FirstOrDefault();
                Wife wife = _context.wives.Where(x => x.UserId == user.Id).FirstOrDefault();

                if(husband != null)
                {
                    _context.husbands.Remove(husband);
                    _context.SaveChanges();

                    await _signInManager.SignOutAsync();
                    await _userManager.DeleteAsync(user);
                }
                if (wife != null)
                {
                    _context.wives.Remove(wife);
                    _context.SaveChanges();

                    await _signInManager.SignOutAsync();
                    await _userManager.DeleteAsync(user);
                }
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
