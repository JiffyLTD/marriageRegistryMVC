using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Families;
using SubdKurshach.Models.Info;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

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

            List<int> allManId = new();
            List<int> allMarriedManId = new();
            List<Husband> unmarriedHusbands = new();

            if (allHusbandsInFamilies.Count() != 0)
            {
                foreach (var id in allHusbands)
                {
                    allManId.Add(id.HusbandId); // собираем список id всех мужчин
                }
                foreach (var id in allHusbandsInFamilies)
                {
                    allMarriedManId.Add(id.HusbandId); // собираем список id всех замужних мужчин
                }

                List<int> allUnmarriedManId = allManId.Except(allMarriedManId).ToList(); // собираем список id всех незамужних мужчин

                foreach (var unmarriedHusbandId in allUnmarriedManId)
                {
                    unmarriedHusbands.Add(_context.husbands.Where(x => x.HusbandId == unmarriedHusbandId).Include(x => x.User).FirstOrDefault());
                }
            }
            else
            {
                foreach (var husbands in allHusbands)
                {
                    unmarriedHusbands.Add(husbands);
                }
            }

            var allWives = _context.wives.Include(x => x.User).ToList();
            var allWivesInFamilies = _context.marriages.ToList();

            List<int> allWomanId = new();
            List<int> allMarriedWomanId = new();
            List<Wife> unmarriedWives = new();

            if (allWivesInFamilies.Count() != 0)
            {
                foreach (var id in allWives)
                {
                    allWomanId.Add(id.WifeId); // собираем список id всех мужчин
                }
                foreach (var id in allWivesInFamilies)
                {
                    allMarriedWomanId.Add(id.WifeId); // собираем список id всех замужних мужчин
                }

                List<int> allUnmarriedWomanId = allWomanId.Except(allMarriedWomanId).ToList(); // собираем список id всех незамужних мужчин

                foreach (var unmarriedWifeId in allUnmarriedWomanId)
                {
                    unmarriedWives.Add(_context.wives.Where(x => x.WifeId == unmarriedWifeId).Include(x => x.User).FirstOrDefault());
                }
            }
            else
            {
                foreach (var wives in allWives)
                {
                   unmarriedWives.Add(wives);               
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
                    }
                    else
                    {
                        Family family = new()
                        {
                            MarriageId = marriage.MarriageId
                        };
                        _context.families.Add(family);
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index","Home");
        }
        public IActionResult DownloadDoc(int id)
        {
            Marriage? marriage = _context.marriages.Where(x => x.MarriageId == id).
                Include(x => x.Wife)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x.UserPassport).
                Include(x => x.Husband)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x.UserPassport).FirstOrDefault();

            if (marriage != null)
            {
                NewMarriageDoc(marriage);
                string file_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/marriageNewDoc" + marriage.Husband.User.FirstName + marriage.Wife.User.FirstName + ".jpg");
                return PhysicalFile(file_path, "application/jpg", "marriageDoc" + marriage.Husband.User.FirstName + marriage.Wife.User.FirstName + ".jpg");
            }

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Documents/marriageDoc.jpg");

            return PhysicalFile(filePath, "application/jpg", "marriageDocumentExample.jpg");
        }

        private static void NewMarriageDoc(Marriage marriage)
        {
            string lastNameHusband = marriage.Husband.User.LastName;
            string firstNameHusband = marriage.Husband.User.FirstName;
            string? patronimycHusband = marriage.Husband.User.Patronymic;
            string birhtdayHusband = marriage.Husband.User.UserPassport.Birthday.ToLongDateString();

            string lastNameWife = marriage.Wife.User.LastName;
            string firstNameWife = marriage.Wife.User.FirstName;
            string? patronimycWife = marriage.Wife.User.Patronymic;
            string birhtdayWife = marriage.Wife.User.UserPassport.Birthday.ToLongDateString();

            string marriageDate = marriage.MarriageDate.ToLongDateString();

            string dateOfIssueDocument = DateTime.Now.ToLongDateString();

            string marriageId = marriage.MarriageId.ToString();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/marriageDoc.jpg");
            Image a = Image.FromFile(filePath);

            Graphics part2 = Graphics.FromImage(a);

            //муж
            part2.DrawString(lastNameHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(850, 560, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(firstNameHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(550, 640, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(patronimycHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 640, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));


            part2.DrawString(birhtdayHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(500, 870, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //жена
            part2.DrawString(lastNameWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(850, 1010, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(firstNameWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(550, 1090, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(patronimycWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 1090, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));


            part2.DrawString(birhtdayWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(500, 1310, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //дата регистрации брака
            part2.DrawString(marriageDate,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(900, 1460, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //id свадьбы
            part2.DrawString(marriageId,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1200, 1760, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //место регистрации
            part2.DrawString("OKSITREND - онлайн ЗАГС",

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1100, 2060, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //дата выдачи документа
            part2.DrawString(dateOfIssueDocument,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 2310, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //кто выдал докуиент
            part2.DrawString("OKSITREND",

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1400, 2490, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            string filePathNew = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/marriageNewDoc" + firstNameHusband + firstNameWife + ".jpg");
            a.Save(filePathNew, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
