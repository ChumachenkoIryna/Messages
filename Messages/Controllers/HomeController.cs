using Messages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;

namespace Messages.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessageContext _context;
        private static User CurrentUser { get; set; }
        public HomeController(MessageContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            var model = new IndexModel
            {
                Messages = _context.Messages.ToList(),
                LoginModel = new LoginModel(),
                RegisterModel = new RegisterModel()
            };
            return View(model);
           
        }
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            if (_context.Messages == null)
                return Problem("—писок сообщений пуст!");
            List<Message> list = await _context.Messages.ToListAsync();
            foreach(var message in list)
            {
                Console.WriteLine(message.User.Login);
            }
            string response = JsonConvert.SerializeObject(list);
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                return Json(CurrentUser);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }



        [HttpPost]
        public IActionResult Login(LoginModel logon)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.ToList().Count == 0)
                {
                    return Problem("Wrong login or password!");
                }
                var users = _context.Users.Where(a => a.Login == logon.Login);
                if (users.ToList().Count == 0)
                {
                    return Problem("Wrong login or password!");

                }
                var user = users.First();
                string? salt = user.Salt;

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + logon.Password);

                //вычисл€ем хеш-представление в байтах  
                byte[] byteHash = SHA256.HashData(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                if (user.Password != hash.ToString())
                {
                    return Problem("Wrong login or password!");

                }
                CurrentUser = user;
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);

                string response = "јвторизаци€ прошла успешно!";
                return Json(response);
            }
            return Problem("ѕроблемы при авторизации");
        }

        [HttpPost]
        public async Task<IActionResult> InsertMessage(string text)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("LastName") != null
               && HttpContext.Session.GetString("FirstName") != null)
                {
                    _context.Attach(CurrentUser);
                    Message m=new Message();
                    m.Text = text;
                    m.User = CurrentUser;
                    //message.User = _context.Users.Where(u => u.FirstName.Equals(HttpContext.Session.GetString("FirstName"))
                    //&& u.LastName.Equals(HttpContext.Session.GetString("LastName"))).First();
                    m .Date = DateTime.Now;
                _context.Add(m);
                    await _context.SaveChangesAsync();
                    string response = "—ообщение успешно добавлено!";
                    return Json(response); 
                }
            }
            return Problem("ѕроблемы при добавлении сообщени€!");
        }


        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            CurrentUser = null;
            return Ok();
        }
        [HttpPost]
        
        public IActionResult Register(RegisterModel reg)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.FirstName = reg.FirstName;
                user.LastName = reg.LastName;
                user.Login = reg.Login;

                byte[] saltbuf = new byte[16];

                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(saltbuf);

                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + reg.Password);

                //вычисл€ем хеш-представление в байтах  
                byte[] byteHash = SHA256.HashData(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                user.Password = hash.ToString();
                user.Salt = salt;
                _context.Users.Add(user);
                _context.SaveChanges();
                return Json("–егистраци€ прошла успешно");
            }

            return Problem("ѕроблема регистрации");
        }
    }
}
