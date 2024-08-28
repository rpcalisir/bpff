using BalkanPanoramaFilmFestival.Areas.Admin.ViewModels;
using BalkanPanoramaFilmFestival.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text;

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

        [Authorize(Roles = "developer")] // Only the users with admin role can access to admin panel
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Optionally handle the error here (e.g., logging or returning a user-friendly message)
                TempData["ErrorMessage"] = "There was an error deleting the user.";
                return RedirectToAction("UserList");
            }

            // Optionally set a success message
            TempData["SuccessMessage"] = "User deleted successfully.";

            // Redirect back to the UserList view after deletion
            return RedirectToAction("UserList");
        }

        [Authorize(Roles = "admin")] // Only the users with admin role can access to admin panel
        [HttpGet]
        public async Task<IActionResult> DownloadUserList()
        {
            var users = await _userManager.Users.ToListAsync();

            // Create a CSV string
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("FirstName,LastName,Email,PhoneNumber");

            foreach (var user in users)
            {
                csvBuilder.AppendLine($"{user.FirstName},{user.LastName},{user.Email},{user.PhoneNumber}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(csvBytes, "text/csv", "UserList.csv");
        }

        public async Task<IActionResult> CompetitionApplications()
        {
            var applicationsList = await _context.CompetitionApplications.ToListAsync();

            var competitionApplicationUserViewModelList = applicationsList.Select(x => new CompetitionApplicationUserViewModel()
            {
                Id = x.Id,
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

                // Movie Tag
                MovieScript = x.MovieScript,
                Cinematographer = x.Cinematographer,
                MovieFiction = x.MovieFiction,
                MovieActors = x.MovieActors,
                BestActress = x.BestActress,
                BestActor = x.BestActor,

                // Producer
                ProducerName = x.ProducerName,
                ProducerCompany = x.ProducerCompany,
                ProducerCountry = x.ProducerCountry,
                ProducerPhone = x.ProducerPhone,
                ProducerEmail = x.ProducerEmail,

                // FILM WORK OPERATION CERTIFICATE
                UploadedPdfFilePath = x.UploadedPdfFilePath, // Ensure this is included

                // Sinopsis
                SinopsisTr = x.SinopsisTr,
                SinopsisEn = x.SinopsisEn,
                FestivalsAttended = x.FestivalsAttended,
                AwardsReceived = x.AwardsReceived,
                PremierStatus = x.PremierStatus,
                FirstScreening = x.FirstScreening,

                // Movie Technical Information
                MovieTechInfoColor = x.MovieTechInfoColor,
                ScreenSize = x.ScreenSize,
                MovieTechInfoAudio = x.MovieTechInfoAudio,

                // MEDIA
                UploadedMoviePicturesFilePaths = x.UploadedMoviePicturesFilePaths,
                UploadedMoviePosterFilePath = x.UploadedMoviePosterFilePath,
                UploadedMovieSubtitleFilePath = x.UploadedMovieSubtitleFilePath,
                UploadedDirectorPhotoFilePath = x.UploadedDirectorPhotoFilePath,
                UploadedBestActressPhotoFilePath = x.UploadedBestActressPhotoFilePath,
                UploadedBestActorPhotoFilePath = x.UploadedBestActorPhotoFilePath,

                // DOWNLOADABLE SCREENING COPY OF THE FILM
                MovieLink = x.MovieLink,
                MovieLinkPassword = x.MovieLinkPassword,
                TrailerLink = x.TrailerLink,
                TrailerLinkPassword = x.TrailerLinkPassword,
                DownloadableCopyCheck = x.DownloadableCopyCheck,

                // APPLICANT
                ApplicantName = x.ApplicantName,
                ApplicantCompany = x.ApplicantCompany,
                ApplicantCountry = x.ApplicantCountry,
                ApplicantPhone = x.ApplicantPhone,
                ApplicantEmail = x.ApplicantEmail,

            }).ToList();

            return View(competitionApplicationUserViewModelList);
        }

        [HttpGet]
        public IActionResult DownloadPdf(string uploadedPdfFilePath, string applicantEmail)
        {
            if (string.IsNullOrEmpty(uploadedPdfFilePath))
            {
                return BadRequest("Uploaded Pdf File Path is required.");
            }

            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant Email is required.");
            }

            // Sanitize applicant email for safe use in the file path
            var sanitizedEmail = applicantEmail.Replace('@', '_').Replace('.', '_');

            // Define the path to the folder where the pdf file is stored
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            // Remove any leading slashes or unwanted characters from the file path
            var cleanedFilePath = uploadedPdfFilePath.Trim('\"').TrimStart('/');

            // Ensure the file path does not contain extra directories and get the file name
            var fileName = Path.GetFileName(cleanedFilePath);

            // Create a search pattern based on the sanitized email
            var searchPattern = $"FilmCertificatePdf_{sanitizedEmail}_*{Path.GetExtension(fileName)}";

            // Find the file that matches the search pattern
            var filePath = Directory.GetFiles(uploadsFolderPath, searchPattern).FirstOrDefault();

            // Check if the file exists
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return BadRequest("Pdf file could not be found.");
            }

            // Read the file bytes from the correct full file path
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Return the file as a download with the correct file name
            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpGet]
        public IActionResult DownloadMoviePictures(string applicantEmail, string uploadedMoviePicturesFilePaths)
        {
            // Validate the parameters
            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            if (string.IsNullOrEmpty(uploadedMoviePicturesFilePaths))
            {
                return BadRequest("Uploaded Movie Pictures File Paths is required.");
            }

            // Sanitize the applicantEmail to be safe for file naming
            var sanitizedEmail = applicantEmail.Replace('@', '_').Replace('.', '_');

            // Split the string into individual file paths
            var filePaths = uploadedMoviePicturesFilePaths
                .Trim('[', ']', '\"') // Trim brackets and quotes
                .Split(',')            // Split by comma
                .Select(path => path.Trim()) // Trim whitespace around each path
                .ToList();

            if (filePaths == null || !filePaths.Any())
            {
                return BadRequest("Uploaded Movie Pictures File could not be found!");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < filePaths.Count; i++)
                    {
                        var filePath = filePaths[i];
                        if (string.IsNullOrEmpty(filePath)) continue;

                        // Remove any trailing quotes or other unwanted characters
                        var cleanedFilePath = filePath.Trim('\"');

                        // Define the path to the file in the wwwroot/pictures directory
                        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

                        if (System.IO.File.Exists(fullFilePath))
                        {
                            // Use a standardized name like MoviePicture1, MoviePicture2, etc.
                            var fileExtension = Path.GetExtension(fullFilePath).ToLowerInvariant();
                            var fileName = $"MoviePicture{i + 1}{fileExtension}";
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

                // Return the zip file with applicant's email in the name
                var zipFileName = $"movie_pictures_{sanitizedEmail}.zip";
                return File(memoryStream.ToArray(), "application/zip", zipFileName);
            }
        }

        [HttpGet]
        public IActionResult DownloadUploadedMoviePoster(string UploadedMoviePosterFilePath, string applicantEmail)
        {
            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            if (string.IsNullOrEmpty(UploadedMoviePosterFilePath))
            {
                return BadRequest("Movie poster file path is required.");
            }

            // Sanitize the applicantEmail to be safe for file naming
            var sanitizedEmail = applicantEmail.Replace('@', '_').Replace('.', '_');

            // Define the path to the folder where the movie poster is stored
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");

            // Remove any leading slashes or unwanted characters from the file path
            var cleanedFilePath = UploadedMoviePosterFilePath.Trim('\"').TrimStart('/');

            // Ensure the file path does not contain extra directories
            var fileName = Path.GetFileName(cleanedFilePath);

            // Create a search pattern based on the sanitized email
            var searchPattern = $"MoviePoster_{sanitizedEmail}_*{Path.GetExtension(fileName)}";

            // Find the file that matches the search pattern
            var filePath = Directory.GetFiles(uploadsFolderPath, searchPattern).FirstOrDefault();

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return BadRequest("Movie poster file not found!");
            }

            // Get the file name from the full file path
            fileName = Path.GetFileName(filePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedMovieSubtitle(string UploadedMovieSubtitleFilePath, string applicantEmail)
        {
            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            if (string.IsNullOrEmpty(UploadedMovieSubtitleFilePath))
            {
                return BadRequest("Movie subtitle file path is required.");
            }

            // Sanitize the applicantEmail to be safe for file naming
            var sanitizedEmail = applicantEmail.Replace('@', '_').Replace('.', '_');

            // Define the path to the folder where the movie subtitles are stored
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "subtitles");

            // Clean the file path, remove any leading slashes
            var cleanedFilePath = UploadedMovieSubtitleFilePath.Trim('\"').TrimStart('/');

            // Ensure the file path does not include the 'subtitles' directory if it's already included
            if (cleanedFilePath.StartsWith("subtitles/", StringComparison.OrdinalIgnoreCase))
            {
                cleanedFilePath = cleanedFilePath.Substring("subtitles/".Length);
            }

            // Combine the folder path with the file name
            var fullFilePath = Path.Combine(uploadsFolderPath, cleanedFilePath);

            if (!System.IO.File.Exists(fullFilePath))
            {
                return BadRequest("Movie subtitle file not found!");
            }

            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedDirectorPhoto(string uploadedDirectorPhotoFilePath, string applicantEmail)
        {
            // Validate the applicantEmail
            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            // Validate the uploadedDirectorPhotoFilePath
            if (string.IsNullOrEmpty(uploadedDirectorPhotoFilePath))
            {
                return BadRequest("Uploaded Director Photo File Path is required.");
            }

            // Clean the file path
            var cleanedFilePath = uploadedDirectorPhotoFilePath.Trim('\"').TrimStart('/');

            // Ensure the file path does not include the 'pictures' directory as it is already part of the file path
            var fileName = Path.GetFileName(cleanedFilePath);

            // Define the path to the folder where the director photos are stored
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pictures");

            // Combine the folder path with the cleaned file name
            var fullFilePath = Path.Combine(uploadsFolderPath, fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(fullFilePath))
            {
                return BadRequest("Director photo file not found.");
            }

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedBestActressPhoto(string uploadedBestActressPhotoFilePath, string applicantEmail)
        {
            if (string.IsNullOrEmpty(uploadedBestActressPhotoFilePath))
            {
                return BadRequest("Uploaded Best Actress Photo File Path is required.");
            }

            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            // Clean the file path
            var cleanedFilePath = uploadedBestActressPhotoFilePath.Trim('\"');

            // Ensure the path is relative to the wwwroot folder by removing "pictures/" prefix
            var relativeFilePath = cleanedFilePath.StartsWith("/pictures/")
                ? cleanedFilePath.Substring("/pictures/".Length)
                : cleanedFilePath.TrimStart('/');

            // Construct the full file path to the specific directory
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pictures");
            var fullFilePath = Path.Combine(uploadsFolderPath, relativeFilePath);

            // Check if the file exists
            if (!System.IO.File.Exists(fullFilePath))
            {
                return BadRequest("Uploaded Best Actress Photo File could not be found!");
            }

            // Extract the file name
            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedBestActorPhoto(string uploadedBestActorPhotoFilePath, string applicantEmail)
        {
            if (string.IsNullOrEmpty(uploadedBestActorPhotoFilePath))
            {
                return BadRequest("Uploaded Best Actor Photo File Path is required.");
            }

            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            // Sanitize inputs to prevent path traversal attacks
            var cleanedFilePath = uploadedBestActorPhotoFilePath.Trim('\"');

            // Sanitize applicant email for safe file path use
            var sanitizedEmail = applicantEmail.Replace('@', '_').Replace('.', '_');

            // Extract the original file name from the cleaned file path
            var fileName = Path.GetFileName(cleanedFilePath);

            // Construct the file name exactly as it was saved (assuming it already includes the sanitized email)
            // We should not add the email again if it is already part of the file name
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pictures", fileName);

            // Verify if the constructed path points to an existing file
            if (!System.IO.File.Exists(fullFilePath))
            {
                return BadRequest("Uploaded Best Actor Photo File could not be found!");
            }

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadAllFiles(string applicantEmail,
                                        string uploadedPdfFilePath,
                                        string uploadedMoviePicturesFilePaths,
                                        string uploadedMoviePosterFilePath,
                                        string uploadedMovieSubtitleFilePath,
                                        string uploadedDirectorPhotoFilePath,
                                        string uploadedBestActressPhotoFilePath,
                                        string uploadedBestActorPhotoFilePath)
        {
            if (string.IsNullOrEmpty(applicantEmail))
            {
                return BadRequest("Applicant email is required.");
            }

            // List to hold all file paths
            var filePaths = new List<string>();

            // Add each file path to the list if it's not null or empty
            if (!string.IsNullOrEmpty(uploadedPdfFilePath))
                filePaths.Add(uploadedPdfFilePath.Trim('\"'));

            if (!string.IsNullOrEmpty(uploadedMoviePicturesFilePaths))
            {
                // Split the string of paths and add each to the list
                filePaths.AddRange(uploadedMoviePicturesFilePaths
                                    .Trim('[', ']', '\"') // Trim brackets and quotes
                                    .Split(',')
                                    .Select(path => path.Trim()));
            }

            if (!string.IsNullOrEmpty(uploadedMoviePosterFilePath))
                filePaths.Add(uploadedMoviePosterFilePath.Trim('\"'));

            if (!string.IsNullOrEmpty(uploadedMovieSubtitleFilePath))
                filePaths.Add(uploadedMovieSubtitleFilePath.Trim('\"'));

            if (!string.IsNullOrEmpty(uploadedDirectorPhotoFilePath))
                filePaths.Add(uploadedDirectorPhotoFilePath.Trim('\"'));

            if (!string.IsNullOrEmpty(uploadedBestActressPhotoFilePath))
                filePaths.Add(uploadedBestActressPhotoFilePath.Trim('\"'));

            if (!string.IsNullOrEmpty(uploadedBestActorPhotoFilePath))
                filePaths.Add(uploadedBestActorPhotoFilePath.Trim('\"'));

            if (filePaths.Count == 0)
            {
                return BadRequest("No files to download.");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (string.IsNullOrEmpty(filePath)) continue;

                        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

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

                memoryStream.Position = 0;

                // Create zip file name using applicantEmail
                var zipFileName = $"all_files_{applicantEmail}.zip";

                return File(memoryStream.ToArray(), "application/zip", zipFileName);
            }
        }

        public IActionResult DownloadAllApplicantData(int id)
        {
            //Retrieve the specific record from the database using the id
            var application = _context.CompetitionApplications.Find(id);

            if (application == null)
            {
                return BadRequest("User could not be found!");
            }

            var csvData = new StringBuilder();
            csvData.AppendLine("Id,CompetitionCategory,OriginalMovieName,EnglishMovieName,MovieWebsite," +
                             "SelectedCountries,SelectedMovieGenres,ProductionYear,MovieTimeLength, " +
                             "MovieLanguage,DirectorName,DirectorCompany,DirectorCountry,DirectorPhone, " +
                             "DirectorEmail,DirectorBiographyTr,DirectorBiographyEn,DirectorFilmographyTr, " +
                             "DirectorFilmographyEn,MovieScript,Cinematographer,MovieFiction,MovieActors, " +
                             "BestActress,BestActor,ProducerName,ProducerCompany,ProducerCountry,ProducerPhone, " +
                             "ProducerEmail,SinopsisTr,SinopsisEn,FestivalsAttended, " +
                             "AwardsReceived,PremierStatus,FirstScreening,MovieTechInfoColor,ScreenSize, " +
                             "MovieTechInfoAudio, " +
                             "MovieLink, " +
                             "MovieLinkPassword,TrailerLink,TrailerLinkPassword,DownloadableCopyCheck, " +
                             "ApplicantName,ApplicantCompany,ApplicantCountry,ApplicantPhone,ApplicantEmail");

            csvData.AppendLine($"{application.Id},{application.CompetitionCategory}," +
                    $"{application.OriginalMovieName},{application.EnglishMovieName}," +
                    $"{application.MovieWebsite},{application.SelectedCountries}," +
                    $"{application.SelectedMovieGenres},{application.ProductionYear}," +
                    $"{application.MovieTimeLength},{application.MovieLanguage}," +
                    $"{application.DirectorName},{application.DirectorCompany}," +
                    $"{application.DirectorCountry},{application.DirectorPhone}," +
                    $"{application.DirectorEmail},{application.DirectorBiographyTr}," +
                    $"{application.DirectorBiographyEn},{application.DirectorFilmographyTr}," +
                    $"{application.DirectorFilmographyEn},{application.MovieScript}," +
                    $"{application.Cinematographer},{application.MovieFiction}," +
                    $"{application.MovieActors},{application.BestActress}," +
                    $"{application.BestActor},{application.ProducerName}," +
                    $"{application.ProducerCompany},{application.ProducerCountry}," +
                    $"{application.ProducerPhone},{application.ProducerEmail}," +
                    $"{application.SinopsisTr},{application.SinopsisEn}," +
                    $"{application.FestivalsAttended},{application.AwardsReceived}," +
                    $"{application.PremierStatus},{application.FirstScreening}," +
                    $"{application.MovieTechInfoColor},{application.ScreenSize}," +
                    $"{application.MovieTechInfoAudio},{application.MovieLink}," +
                    $"{application.MovieLinkPassword},{application.TrailerLink}," +
                    $"{application.TrailerLinkPassword},{application.DownloadableCopyCheck}," +
                    $"{application.ApplicantName},{application.ApplicantCompany}," +
                    $"{application.ApplicantCountry},{application.ApplicantPhone}," +
                    $"{application.ApplicantEmail}");

            // Add BOM for UTF-8
            var bom = Encoding.UTF8.GetBytes("\uFEFF");

            //var csvContent = Encoding.UTF8.GetBytes(csvData.ToString());
            var csvContent = bom.Concat(Encoding.UTF8.GetBytes(csvData.ToString())).ToArray();
            var fileName = $"CompetitionApplication_{application.ApplicantEmail}.csv";

            return File(csvContent, "text/csv", fileName);
        }
    }
}
