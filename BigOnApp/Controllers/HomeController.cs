using BigOnApp.DAL.context;
using BigOnApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BigOnApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Subscribe(string email)
    {
        bool isEmail = Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (!isEmail)
        {
            return Json(new
            {
                error = true,
                message = "Zehmet olmasa duzgn email daxil edin"
            });
        }

        var dbEmail = _context.Subscribers.FirstOrDefault(s => s.EmailAdreess == email);
        if (dbEmail != null && !dbEmail.IsAprovved)
        {
            return Json(new
            {
                error = true,
                message = "Bu email artiq qeydiyatdan kecib zehmet olmasa mailnizi tesqid edin"
            });
        }

        var newSubscriber = new Subscriber
        {
            EmailAdreess = email,
            CreatedAt = DateTime.UtcNow.AddHours(4)
        };


        _context.Subscribers.Add(newSubscriber);
        _context.SaveChanges();
        return Ok(new
        {
            success = true,
            message = $"Bu {email} ugurla qeydiyyatdan kecdi",
        });
    }
}
