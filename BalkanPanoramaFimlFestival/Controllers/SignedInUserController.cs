using BalkanPanoramaFilmFestival.Extensions;
using BalkanPanoramaFilmFestival.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.Models.CompetitionApplication;
using BalkanPanoramaFilmFestival.Services;
using BalkanPanoramaFilmFestival.ViewModels.Account;
using BalkanPanoramaFilmFestival.ViewModels.CompetitionApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BalkanPanoramaFilmFestival.Controllers
{
    // Only SignedIn User can use this Controller and its methods
    [Authorize]
    public class SignedInUserController : Controller
    {
        private readonly SignInManager<RegisteredUser> _signInManager;
        private readonly UserManager<RegisteredUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ICompetitionApplicationFormService _competitionApplicationFormService;

        public SignedInUserController(SignInManager<RegisteredUser> signInManager,
            UserManager<RegisteredUser> userManager,
            ApplicationDbContext context,
            ICompetitionApplicationFormService countryService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _competitionApplicationFormService = countryService;
        }

        public IActionResult CompetitionApplication()
        {
            //ViewBag.Countries = _countryService.GetAllCountries();

            var model = new CompetitionApplicationUserViewModel
            {
                CompetitionCategory = string.Empty,
                OriginalMovieName = string.Empty,
                EnglishMovieName = string.Empty,
                MovieWebsite = string.Empty,
                SelectedCountries = new List<string>(),
                SelectedMovieGenres = new List<string>(),
                ProductionYear = string.Empty,
                MovieTimeLength = string.Empty,
                MovieLanguage = string.Empty,

                // Direction Section
                DirectorName = string.Empty,
                DirectorCountry = string.Empty,
                DirectorPhone = string.Empty,
                DirectorEmail = string.Empty,
                DirectorBiographyTr = string.Empty,
                DirectorBiographyEn = string.Empty,
                DirectorFilmographyTr = string.Empty,
                DirectorFilmographyEn = string.Empty,

                // Movie Tag
                MovieScript = string.Empty,
                Cinematographer = string.Empty,
                MovieFiction = string.Empty,
                MovieActors = string.Empty,
                BestActress = string.Empty,
                BestActor = string.Empty,

                // Producer
                ProducerName = string.Empty,
                ProducerCompany = string.Empty,
                ProducerCountry = string.Empty,
                ProducerPhone = string.Empty,
                ProducerEmail = string.Empty,

                // Sinopsis
                SinopsisTr = string.Empty,
                SinopsisEn = string.Empty,
                FestivalsAttended = string.Empty,
                AwardsReceived = string.Empty,
                PremierStatus = string.Empty,
                FirstScreening = string.Empty,

                // Movie Technical Information
                MovieTechInfoColor = string.Empty,
                ScreenSize = string.Empty,
                MovieTechInfoAudio = string.Empty,

                // MEDIA
                UploadedMoviePicturesFilePaths = string.Empty,
                UploadedMoviePosterFilePath = string.Empty,
                UploadedMovieSubtitleFilePath = string.Empty,
                UploadedDirectorPhotoFilePath = string.Empty,
                UploadedBestActressPhotoFilePath = string.Empty,
                UploadedBestActressPhotoName = string.Empty,

                AllCountries = _competitionApplicationFormService.GetAllCountries(), // Fetch the country list
                AllMovieGenres = _competitionApplicationFormService.GetAllGenres(), // Fetch the genre list
            };

            return View(model); // viewmodel data is being passed to cshtml here, when page is first being displayed.
        }

        [HttpPost]
        public async Task<IActionResult> CompetitionApplication(CompetitionApplicationUserViewModel model)
        {
            //if (!model.CompetitionCategory.Any())
            //{
            //    ModelState.AddModelError(string.Empty, "At least one competition category must be selected.");
            //}

            //if (!model.SelectedCountries.Any())
            //{
            //    ModelState.AddModelError(string.Empty, "At least one country must be selected.");
            //}

            //if (model.SelectedCountries.Count > 3)
            //{
            //    ModelState.AddModelError(string.Empty, "Max 3 countries can be selected.");
            //}

            //In case of signup form data is not valid, return the view without deleting the form data
            if (!ModelState.IsValid)
            {
                // Ensure the form retains the selected values
                // This assumes SelectedCountries and SelectedMovieGenres
                // are strings in the form of comma-separated values or lists
                var selectedCountriesList = Request.Form["SelectedCountries"].ToList();
                var selectedMovieGenres = Request.Form["SelectedMovieGenres"].ToList();
                if (selectedCountriesList != null && selectedMovieGenres != null)
                {
                    model.SelectedCountries = selectedCountriesList!;
                    model.SelectedMovieGenres = selectedMovieGenres!;
                }

                //ViewBag.Countries = _countryService.GetAllCountries();
                //return View(model); // Return the view with validation errors
                ModelState.AddModelError(string.Empty, "One of the inputs is not in correct!");

                model.AllCountries = _competitionApplicationFormService.GetAllCountries(); // Re-fetch the country list for view
                model.AllMovieGenres = _competitionApplicationFormService.GetAllGenres(); // Re-fetch the country list for view
                return View(model);
            }

            var signedInUser = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            if (signedInUser!.Email != null)
            {
                // Access country name from the hidden fields
                var directorCountryName = Request.Form["DirectorCountryName"];
                var producerCountryName = Request.Form["ProducerCountryName"];

                // Handle Pdf file upload
                if (model.UploadedPdfFile != null && model.UploadedPdfFile.Length > 0)
                {
                    // Validate file type
                    if (!model.UploadedPdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(string.Empty, "The file must be a PDF.");
                        return View(model);
                    }

                    // Validate file size (e.g., max 20 MB)
                    if (model.UploadedPdfFile.Length > 20 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "The file size must be less than 20 MB.");

                        return View(model);
                    }

                    // Define the path to save the file
                    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Ensure the uploads directory exists
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    // Generate a unique file name to prevent overwriting
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" +
                        Path.GetFileName(model.UploadedPdfFile.FileName);
                    var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedPdfFile.CopyToAsync(fileStream);
                    }

                    // Store the relative file path in the database
                    model.UploadedPdfFilePath = "/uploads/" + uniqueFileName;
                }

                // Handle Movie Pictures upload

                // Assume model.UploadedFiles is the property that contains the list of uploaded files
                var uploadedMoviePicturesFilesPaths = await HandleMoviePicturesFilesUploadAsync(model.UploadedMoviePictures!);


                // Check if there were any issues with file uploads
                if (string.IsNullOrEmpty(uploadedMoviePicturesFilesPaths) && !ModelState.IsValid)
                {
                    // Return the view with validation errors if any files failed to upload
                    ModelState.AddModelError(string.Empty, "Failed to upload movie pictures.");
                    return View(model);
                }

                // Continue processing if the files were uploaded successfully
                model.UploadedMoviePicturesFilePaths = uploadedMoviePicturesFilesPaths;

                // Handle Best Actress Photo upload
                string UploadedBestActressPhotoFilePath = string.Empty;
                if (model.UploadedBestActressPhoto != null && model.UploadedBestActressPhotoName != null)
                {
                    UploadedBestActressPhotoFilePath = await HandleUploadedBestActressPhotoUploadAsync(
                        model.UploadedBestActressPhoto, model.UploadedBestActressPhotoName);

                    if (string.IsNullOrEmpty(UploadedBestActressPhotoFilePath))
                    {
                        // Handle the case where the file upload failed
                        ModelState.AddModelError(string.Empty, "Best Actress Photo could not be uploaded.");

                        return View(model); // Assuming you want to redisplay the form with the error messages
                    }
                }

                // Continue processing if the files were uploaded successfully
                model.UploadedBestActressPhotoFilePath = UploadedBestActressPhotoFilePath;

                // Handle Best Actor Photo upload
                string UploadedBestActorPhotoFilePath = string.Empty;
                if (model.UploadedBestActorPhoto != null && model.UploadedBestActorPhotoName != null)
                {
                    UploadedBestActorPhotoFilePath = await HandleUploadedBestActorPhotoUploadAsync(
                        model.UploadedBestActorPhoto, model.UploadedBestActorPhotoName);

                    if (string.IsNullOrEmpty(UploadedBestActorPhotoFilePath))
                    {
                        // Handle the case where the file upload failed
                        ModelState.AddModelError(string.Empty, "Best Actor Photo could not be uploaded.");

                        return View(model); // Assuming you want to redisplay the form with the error messages
                    }
                }

                // Continue processing if the files were uploaded successfully
                model.UploadedBestActorPhotoFilePath = UploadedBestActorPhotoFilePath;

                var user = new CompetitionApplicationUser
                {
                    CompetitionCategory = model.CompetitionCategoryDescription, // Comes from the page form
                    OriginalMovieName = model.OriginalMovieName, // Comes from the page form
                    EnglishMovieName = model.EnglishMovieName, // Comes from the page form
                    MovieWebsite = model.MovieWebsite, // Comes from the page form
                    SelectedCountries = string.Join(", ", model.SelectedCountries), // Store as a comma-separated string
                    SelectedMovieGenres = string.Join(", ", model.SelectedMovieGenres), // Comes from the page form
                    ProductionYear = model.ProductionYear, // Comes from the page form
                    MovieTimeLength = model.MovieTimeLength,
                    MovieLanguage = model.MovieLanguage,

                    // Direction Section
                    DirectorName = model.DirectorName, // Comes from the page form
                    //DirectorCountry = model.DirectorCountry,
                    DirectorCountry = directorCountryName!,
                    DirectorPhone = model.DirectorPhone,
                    DirectorEmail = model.DirectorEmail,
                    DirectorBiographyTr = model.DirectorBiographyTr,
                    DirectorBiographyEn = model.DirectorBiographyEn,
                    DirectorFilmographyTr = model.DirectorFilmographyTr,
                    DirectorFilmographyEn = model.DirectorFilmographyEn,

                    // Movie Tag
                    MovieScript = model.MovieScript,
                    Cinematographer = model.Cinematographer,
                    MovieFiction = model.MovieFiction,
                    MovieActors = model.MovieActors,
                    BestActress = model.BestActress,
                    BestActor = model.BestActor,

                    // Producer
                    ProducerName = model.ProducerName,
                    ProducerCompany = model.ProducerCompany,
                    //ProducerCountry = model.ProducerCountry,
                    ProducerCountry = producerCountryName!,
                    ProducerPhone = model.ProducerPhone,
                    ProducerEmail = model.ProducerEmail,
                    ProducerWebsite = model.ProducerWebsite,

                    // FILM WORK OPERATION CERTIFICATE
                    UploadedPdfFilePath = model.UploadedPdfFilePath!,

                    // Sinopsis
                    SinopsisTr = model.SinopsisTr,
                    SinopsisEn = model.SinopsisEn,
                    FestivalsAttended = model.FestivalsAttended,
                    AwardsReceived = model.AwardsReceived,
                    PremierStatus = model.PremierStatus,
                    FirstScreening = model.FirstScreening,

                    // Movie Technical Information
                    MovieTechInfoColor = model.MovieTechInfoColor,
                    ScreenSize = model.ScreenSize,
                    MovieTechInfoAudio = model.MovieTechInfoAudio,

                    // MEDIA
                    UploadedMoviePicturesFilePaths = model.UploadedMoviePicturesFilePaths,
                    UploadedMoviePosterFilePath = model.UploadedMoviePosterFilePath,
                    UploadedMovieSubtitleFilePath = model.UploadedMovieSubtitleFilePath,
                    UploadedDirectorPhotoFilePath   = model.UploadedDirectorPhotoFilePath,
                    UploadedBestActressPhotoFilePath = model.UploadedBestActressPhotoFilePath,
                    UploadedBestActorPhotoFilePath = model.UploadedBestActorPhotoFilePath,

                    Applicant = $"{signedInUser.FirstName} {signedInUser.LastName}",
                    ApplicantMail = signedInUser.Email,
                    ApplicantCountry = signedInUser.Country,
                };

                // Save the form data to the database
                _context.CompetitionApplications.Add(user);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Your application has been submitted successfully.";
                    return RedirectToAction(nameof(CompetitionApplication)); // Redirect to prevent resubmission on refresh
                }
            }

            ModelState.AddModelError(string.Empty, "An error occurred while processing your application");

            return View(model);
        }

        // In case a user wants to go to this page, 
        // it will be directed to LoginPath page
        // which is configured on ConfigureApplicationCookie in Program.cs
        public async Task<IActionResult> Profile()
        {
            if (User.Identity?.Name == null)
            {
                return RedirectToAction("SignIn", "Home"); // Redirect to signIn if user identity is null
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home"); // Redirect to signIn if user is not found
            }

            var signedInUserViewModel = new SignedInUserViewModel
            {
                FirstName = currentUser.FirstName ?? string.Empty,
                LastName = currentUser.LastName ?? string.Empty,
                Email = currentUser.Email ?? string.Empty,
                PhoneNumber = currentUser.PhoneNumber ?? string.Empty
            };

            return View(signedInUserViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)
        {
            //In case of password change form data is not valid,
            //return the view without deleting the form data
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Because of this controller has [Authorize] on it, 
            // User cannot be null, that's why it is safe to put ! next to it.
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            bool checkOldPassword = await _userManager.CheckPasswordAsync(currentUser!, model.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifre yanlış");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser!, model.PasswordOld, model.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(d => d.Description).ToList());
                return View();
            }

            // Update the security stamp after password change,
            // so sessions in other browsers can become signed out.
            await _userManager.UpdateSecurityStampAsync(currentUser!);

            // Signout and signin the user after the password change,
            // to update the cookie.
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser!, model.PasswordNew, true, false);

            TempData["SuccessMessage"] = "Şifre başarı ile değiştirildi";

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.message = "You are not authorized to access this page. Contact the administrator for authorization.";

            return View();
        }


        #region Private Implementation
        private async Task<string> HandleMoviePicturesFilesUploadAsync(List<IFormFile> uploadedMoviePicturesFiles)
        {
            var uploadedFilePaths = new List<string>();

            // Check if there are any files uploaded
            if (uploadedMoviePicturesFiles == null || !uploadedMoviePicturesFiles.Any())
            {
                ModelState.AddModelError(string.Empty, "Please upload at least one picture.");
                return string.Empty; // Return empty string
            }

            // Check the number of uploaded files
            if (uploadedMoviePicturesFiles.Count > 5)
            {
                ModelState.AddModelError(string.Empty, "You can upload a maximum of 5 pictures.");
                return string.Empty; // Return empty string
            }

            // Validate total size of the uploaded files (max 10 MB)
            var totalSize = uploadedMoviePicturesFiles.Sum(f => f.Length);
            if (totalSize > 10 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "The total size of the uploaded pictures must be less than 10 MB.");
                return string.Empty; // Return empty string
            }

            // Directory to save uploaded files
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");

            // Ensure the uploads directory exists
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Loop through each uploaded file
            foreach (var file in uploadedMoviePicturesFiles)
            {
                // Validate file type
                if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "All uploaded files must be pictures.");
                    return string.Empty; // Return empty string
                }

                // Generate a unique file name to prevent overwriting
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Store the relative file path for future use
                uploadedFilePaths.Add("/pictures/" + uniqueFileName);
            }

            // Join the file paths into a single string separated by commas
            return string.Join(",", uploadedFilePaths); // Return a single string containing all file paths
        }

        private async Task<string> HandleUploadedBestActressPhotoUploadAsync(IFormFile UploadedBestActressPhoto, string actressName)
        {
            if (UploadedBestActressPhoto == null)
            {
                ModelState.AddModelError(string.Empty, "Please upload a photo for the Best Actress.");
                return string.Empty;
            }

            // Validate file size (max 2 MB)
            if (UploadedBestActressPhoto.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "The photo size must be less than 2 MB.");
                return string.Empty;
            }

            // Validate file type
            if (!UploadedBestActressPhoto.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The uploaded file must be a picture.");
                return string.Empty;
            }

            // Sanitize the actress name for file naming
            string sanitizedFileName = Path.GetInvalidFileNameChars()
                                          .Aggregate(actressName, (current, c) => current.Replace(c, '_'));

            // Generate a unique file name with the actress's name
            string fileExtension = Path.GetExtension(UploadedBestActressPhoto.FileName);
            string newFileName = sanitizedFileName + fileExtension;

            // Directory to save uploaded files
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");

            // Ensure the uploads directory exists
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Save the file to the server
            var filePath = Path.Combine(uploadsFolderPath, newFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await UploadedBestActressPhoto.CopyToAsync(fileStream);
            }

            // Return the relative file path
            return "/pictures/" + newFileName;
        }

        private async Task<string> HandleUploadedBestActorPhotoUploadAsync(IFormFile UploadedBestActorPhoto, string actorName)
        {
            if (UploadedBestActorPhoto == null)
            {
                ModelState.AddModelError(string.Empty, "Please upload a photo for the Best Actor.");
                return string.Empty;
            }

            // Validate file size (max 2 MB)
            if (UploadedBestActorPhoto.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "The photo size must be less than 2 MB.");
                return string.Empty;
            }

            // Validate file type
            if (!UploadedBestActorPhoto.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The uploaded file must be a picture.");
                return string.Empty;
            }

            // Sanitize the actor name for file naming
            string sanitizedFileName = Path.GetInvalidFileNameChars()
                                          .Aggregate(actorName, (current, c) => current.Replace(c, '_'));

            // Generate a unique file name with the actor's name
            string fileExtension = Path.GetExtension(UploadedBestActorPhoto.FileName);
            string newFileName = sanitizedFileName + fileExtension;

            // Directory to save uploaded files
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");

            // Ensure the uploads directory exists
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Save the file to the server
            var filePath = Path.Combine(uploadsFolderPath, newFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await UploadedBestActorPhoto.CopyToAsync(fileStream);
            }

            // Return the relative file path
            return "/pictures/" + newFileName;
        }

        #endregion

    }
}
