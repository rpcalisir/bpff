using BalkanPanoramaFilmFestival.Areas.Admin.ViewModels;
using BalkanPanoramaFilmFestival.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalkanPanoramaFilmFestival.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")] // Only the users with admin role can access to admin panel
    [Area("Admin")] //Specifies that when admin is in the url, it should look for this HomeController
    public class HomeController : Controller
    {
        private readonly UserManager<RegisteredUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<RegisteredUser> userManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();

            var adminUserViewModelList = userList.Select(x => new UserViewModel()
                {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }).ToList();

            return View(adminUserViewModelList);
        }

        public async Task<IActionResult> CompetitionApplications()
        {
            var applicationsList = await _context.CompetitionApplications.ToListAsync();

            var competitionApplicationUserViewModelList = applicationsList.Select(x => new CompetitionApplicationUserViewModel()
            {
                Id =x.Id,
                CompetitionCategory = x.CompetitionCategory,
                OriginalMovieName = x.OriginalMovieName,
                EnglishMovieName = x.EnglishMovieName,
                MovieWebsite = x.MovieWebsite,
                SelectedCountries = x.SelectedCountries,
                SelectedMovieGenres = x.SelectedMovieGenres,
                ProductionYear = x.ProductionYear,
                MovieTimeLength = x.MovieTimeLength,
                MovieLanguage = x.MovieLanguage,

                // Director Section
                DirectorName = x.DirectorName,
                DirectorCompany = x.DirectorCompany,
                DirectorCountry = x.DirectorCountry,
                DirectorPhone = x.DirectorPhone,
                DirectorEmail = x.DirectorEmail,
                DirectorBiographyTr = x.DirectorBiographyTr,
                DirectorBiographyEn = x.DirectorBiographyEn,
                DirectorFilmographyTr = x.DirectorFilmographyTr,
                DirectorFilmographyEn = x.DirectorFilmographyEn,

                UploadedFilePath = x.UploadedFilePath, // Ensure this is included

                Applicant = x.Applicant,
                ApplicantMail = x.ApplicantMail,
                ApplicantCountry = x.ApplicantCountry,


            }).ToList();

            return View(competitionApplicationUserViewModelList);
        }

        [HttpGet]
        public IActionResult DownloadPdf(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            // Decode the URL-encoded file path
            var decodedFilePath = Uri.UnescapeDataString(filePath);

            // Log or debug the decoded file path
            Console.WriteLine($"Decoded file path: {decodedFilePath}");

            // Combine the root directory with the relative file path
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", decodedFilePath.TrimStart('/'));

            // Log or debug the full file path
            Console.WriteLine($"Full file path: {fullFilePath}");

            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
            var fileName = Path.GetFileName(fullFilePath);

            return File(fileBytes, "application/pdf", fileName);
        }



    }
}
