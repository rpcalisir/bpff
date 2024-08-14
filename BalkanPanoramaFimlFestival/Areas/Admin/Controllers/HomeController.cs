using BalkanPanoramaFilmFestival.Areas.Admin.ViewModels;
using BalkanPanoramaFilmFestival.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

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
                DirectorCountry = x.DirectorCountry,
                DirectorPhone = x.DirectorPhone,
                DirectorEmail = x.DirectorEmail,
                DirectorBiographyTr = x.DirectorBiographyTr,
                DirectorBiographyEn = x.DirectorBiographyEn,
                DirectorFilmographyTr = x.DirectorFilmographyTr,
                DirectorFilmographyEn = x.DirectorFilmographyEn,

                // FILM WORK OPERATION CERTIFICATE
                UploadedPdfFilePath = x.UploadedPdfFilePath, // Ensure this is included

                // MEDIA
                UploadedMoviePicturesFilePaths = x.UploadedMoviePicturesFilePaths,

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

        [HttpGet]
        public IActionResult DownloadMoviePictures(string uploadedMoviePicturesFilePaths)
        {
            // Split the string into individual file paths
            var filePaths = uploadedMoviePicturesFilePaths
                .Trim('[', ']', '\"') // Trim brackets and quotes
                .Split(',')            // Split by comma
                .Select(path => path.Trim()) // Trim whitespace around each path
                .ToList();

            if (filePaths == null || !filePaths.Any())
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (string.IsNullOrEmpty(filePath)) continue;

                        // Remove any trailing quotes or other unwanted characters
                        var cleanedFilePath = filePath.Trim('\"');

                        // Define the path to the file in the wwwroot/pictures directory
                        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

                        if (System.IO.File.Exists(fullFilePath))
                        {
                            var fileName = Path.GetFileName(fullFilePath);
                            var fileEntry = archive.CreateEntry(fileName);

                            using (var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
                            using (var entryStream = fileEntry.Open())
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }

                // Reset the position of the memory stream
                memoryStream.Position = 0;

                // Return the zip file
                return File(memoryStream.ToArray(), "application/zip", "movie_pictures.zip");
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream" // Default value for unknown file types
            };
        }
    }
}
