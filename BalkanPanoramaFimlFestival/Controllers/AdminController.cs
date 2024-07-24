using BalkanPanoramaFimlFestival.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Action to display the Admin Panel
    public async Task<IActionResult> AdminPanel()
    {
        var contactForms = await _context.ContactForms.ToListAsync();
        return View(contactForms);
    }

    // Action to display details of a contact form
    public async Task<IActionResult> Details(int id)
    {
        var contactForm = await _context.ContactForms.FindAsync(id);
        if (contactForm == null)
        {
            return NotFound();
        }
        return View(contactForm);
    }

    // Action to delete a contact form
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var contactForm = await _context.ContactForms.FindAsync(id);
        if (contactForm != null)
        {
            _context.ContactForms.Remove(contactForm);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(AdminPanel));
    }
}
