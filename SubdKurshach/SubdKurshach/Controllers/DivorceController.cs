using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdKurshach.DbContext;
using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Families;
using SubdKurshach.Models.Users;
using SubdKurshach.ViewModel;
using System.Drawing;

namespace SubdKurshach.Controllers
{
    public class DivorceController : Controller
    {
        private AppDbContext _context;
        UserManager<User> _userManager;

        public DivorceController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult NewDivorce()
        {
            var allFamilies = _context.families.Include(x => x.Marriage).
                                                ThenInclude(x => x.Husband).
                                                 ThenInclude(x => x.User).
                                             Include(x => x.Marriage).
                                                ThenInclude(x => x.Wife).
                                                 ThenInclude(x => x.User).ToList();

            var allDivorces = _context.divorces.ToList();

            List<int> allFimiliesId = new();
            List<int> allDivorcedFamiliesId = new();
            List<Family> familiesThatCanGetDivorced = new();

            FamilyListViewModel familyListViewModel = new();
            if (allDivorces.Count > 0)
            {
                foreach (var id in allFamilies)
                {
                    allFimiliesId.Add(id.FamilyId); // собираем список id всех семей
                }
                foreach (var id in allDivorces)
                {
                    allDivorcedFamiliesId.Add(id.FamilyId); // собираем список id всех семей которые уже развелись
                }

                List<int> allNotDivorcedFamiliesId = allFimiliesId.Except(allDivorcedFamiliesId).ToList(); // собираем список id всех семей которые еще не развелись

                foreach (var notDivorcedFamiliyId in allNotDivorcedFamiliesId)
                {
                    familiesThatCanGetDivorced.Add(_context.families.Where(x => x.FamilyId == notDivorcedFamiliyId).FirstOrDefault());
                }

                familyListViewModel = new()
                {
                    Families = familiesThatCanGetDivorced
                };
            }
            else
            {
                familyListViewModel = new()
                {
                    Families = allFamilies
                };
            }

            return View(familyListViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> NewDivorce(FamilyListViewModel familyDivorceModel)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Employee employee = _context.employees.Where(x => x.UserId == user.Id).First();

            Divorce divorce = new()
            {
               FamilyId= familyDivorceModel.FamilyId,
               EmployeeId = employee.EmployeeId,
               DivorceDate = DateTime.Now
            };
            _context.divorces.Add(divorce);
            _context.SaveChanges();

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
                var divorce = _context.divorces.Where(x => x.FamilyId == _context.families.Where(x => x.MarriageId == marriage.MarriageId).FirstOrDefault().FamilyId).FirstOrDefault();
                Employee employee = _context.employees.Include(x => x.User).Where(x => x.EmployeeId == divorce.EmployeeId).FirstOrDefault();

                NewMarriageDoc(marriage, employee);

                string file_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/divorceNewDoc" + marriage.Husband.User.FirstName + marriage.Wife.User.FirstName + ".jpg");
               
                return PhysicalFile(file_path, "application/jpg", "divorceDoc" + marriage.Husband.User.FirstName + marriage.Wife.User.FirstName + ".jpg");
            }

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/divorceDoc.jpg");

            return PhysicalFile(filePath, "application/jpg", "divorceDocumentExample.jpg");
        }

        private static void NewMarriageDoc(Marriage marriage,Employee employee)
        {
            string lastNameHusband = marriage.Husband.User.LastName;
            string firstNameHusband = marriage.Husband.User.FirstName;
            string? patronimycHusband = marriage.Husband.User.Patronymic;
            string birhtdayHusband = marriage.Husband.User.UserPassport.Birthday.ToLongDateString();

            string lastNameWife = marriage.Wife.User.LastName;
            string firstNameWife = marriage.Wife.User.FirstName;
            string? patronimycWife = marriage.Wife.User.Patronymic;
            string birhtdayWife = marriage.Wife.User.UserPassport.Birthday.ToLongDateString();

            string lastNameEmployee = employee.User.LastName;
            string firstNameEmployee = employee.User.FirstName;
            string? patronimycEmployee = employee.User.Patronymic;

            string marriageDate = marriage.MarriageDate.ToLongDateString();

            string dateOfIssueDocument = DateTime.Now.ToLongDateString();

            string marriageId = marriage.MarriageId.ToString();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/divorceDoc.jpg");
            Image a = Image.FromFile(filePath);

            Graphics part2 = Graphics.FromImage(a);

            //муж
            part2.DrawString(lastNameHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(850, 490, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(firstNameHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(550, 570, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(patronimycHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 570, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));


            part2.DrawString(birhtdayHusband,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(500, 720, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //жена
            part2.DrawString(lastNameWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(850, 870, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(firstNameWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(550, 940, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(patronimycWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 940, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));


            part2.DrawString(birhtdayWife,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(500, 1090, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //дата регистрации брака
            part2.DrawString(marriageDate,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(900, 1240, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //id свадьбы
            part2.DrawString(marriageId,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1200, 1760, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //место регистрации
            part2.DrawString("OKSITREND - онлайн ЗАГС",

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1100, 1990, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //дата выдачи документа
            part2.DrawString(dateOfIssueDocument,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 2380, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //кто выдал докуиент
            part2.DrawString("OKSITREND",

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1400, 2530, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            //сотрудник
            part2.DrawString(lastNameEmployee,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(850, 2210, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(firstNameEmployee,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(550, 2290, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            part2.DrawString(patronimycEmployee,

            new Font("Arial", 32, FontStyle.Bold),

            new SolidBrush(Color.Black), new RectangleF(1300, 2290, 0, 0),

            new StringFormat(StringFormatFlags.NoWrap));

            string filePathNew = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents/divorceNewDoc" + firstNameHusband + firstNameWife + ".jpg");
            a.Save(filePathNew, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}

