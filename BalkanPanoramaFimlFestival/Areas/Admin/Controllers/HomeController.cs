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

        [HttpGet]
        public IActionResult DownloadUploadedMoviePoster(string UploadedMoviePosterFilePath)
        {
            if (string.IsNullOrEmpty(UploadedMoviePosterFilePath))
            {
                return NotFound();
            }

            var cleanedFilePath = UploadedMoviePosterFilePath.Trim('\"');
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedMovieSubtitle(string UploadedMovieSubtitleFilePath)
        {
            if (string.IsNullOrEmpty(UploadedMovieSubtitleFilePath))
            {
                return NotFound();
            }

            var cleanedFilePath = UploadedMovieSubtitleFilePath.Trim('\"');
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedDirectorPhoto(string UploadedDirectorPhotoFilePath)
        {
            if (string.IsNullOrEmpty(UploadedDirectorPhotoFilePath))
            {
                return NotFound();
            }

            var cleanedFilePath = UploadedDirectorPhotoFilePath.Trim('\"');
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedBestActressPhoto(string UploadedBestActressPhotoFilePath)
        {
            if (string.IsNullOrEmpty(UploadedBestActressPhotoFilePath))
            {
                return NotFound();
            }

            var cleanedFilePath = UploadedBestActressPhotoFilePath.Trim('\"');
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadUploadedBestActorPhoto(string UploadedBestActorPhotoFilePath)
        {
            if (string.IsNullOrEmpty(UploadedBestActorPhotoFilePath))
            {
                return NotFound();
            }

            var cleanedFilePath = UploadedBestActorPhotoFilePath.Trim('\"');
            var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanedFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(fullFilePath);

            // Return the file as a download
            return File(System.IO.File.ReadAllBytes(fullFilePath), "application/octet-stream", fileName);
        }

        [HttpGet]
        public IActionResult DownloadAllFiles(string uploadedPdfFilePath, string uploadedMoviePicturesFilePaths,
                                      string uploadedMoviePosterFilePath, string uploadedMovieSubtitleFilePath,
                                      string uploadedDirectorPhotoFilePath, string uploadedBestActressPhotoFilePath,
                                      string uploadedBestActorPhotoFilePath)
        {
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
                return NotFound("No files to download.");
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
                return File(memoryStream.ToArray(), "application/zip", "all_files.zip");
            }
        }

        public IActionResult DownloadAllApplicantData(int id)
        {
            //Retrieve the specific record from the database using the id
            var application = _context.CompetitionApplications.Find(id);

            if (application == null)
            {
                return NotFound();
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
