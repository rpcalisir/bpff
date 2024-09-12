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
        private readonly TimeZoneInfo _turkeyTimeZone;

        public SignedInUserController(SignInManager<RegisteredUser> signInManager,
            UserManager<RegisteredUser> userManager,
            ApplicationDbContext context,
            ICompetitionApplicationFormService countryService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _competitionApplicationFormService = countryService;

            _turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
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
                DirectorCompany = string.Empty,
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

                // DOWNLOADABLE SCREENING COPY OF THE FILM
                MovieLink = string.Empty,
                MovieLinkPassword = string.Empty,
                TrailerLink = string.Empty,
                TrailerLinkPassword = string.Empty,
                DownloadableCopyCheck = false,

                // APPLICANT
                ApplicantName = string.Empty,
                ApplicantCompany = string.Empty,
                ApplicantCountry = string.Empty,
                ApplicantPhone = string.Empty,
                ApplicantEmail = string.Empty,

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
                //// Ensure the form retains the selected values
                //// This assumes SelectedCountries and SelectedMovieGenres
                //// are strings in the form of comma-separated values or lists
                //var selectedCountriesList = Request.Form["SelectedCountries"].ToList();
                //var selectedMovieGenres = Request.Form["SelectedMovieGenres"].ToList();
                //if (selectedCountriesList != null && selectedMovieGenres != null)
                //{
                //    model.SelectedCountries = selectedCountriesList!;
                //    model.SelectedMovieGenres = selectedMovieGenres!;
                //}

                ////ViewBag.Countries = _countryService.GetAllCountries();
                ////return View(model); // Return the view with validation errors
                //ModelState.AddModelError(string.Empty, "One of the inputs is not in correct!");

                //model.AllCountries = _competitionApplicationFormService.GetAllCountries(); // Re-fetch the country list for view
                //model.AllMovieGenres = _competitionApplicationFormService.GetAllGenres(); // Re-fetch the country list for view
                //model.CompetitionCategory = Request.Form["CompetitionCategory"]!;
                //model.MovieTimeLength = Request.Form["MovieTimeLength"]!;
                var updatedModel = PopulateModelAndViewData(model);
                return View(updatedModel);
            }

            var signedInUser = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            if (signedInUser!.Email != null)
            {
                // Access country name from the hidden fields
                var directorCountryName = Request.Form["DirectorCountryName"];
                var producerCountryName = Request.Form["ProducerCountryName"];
                var applicantCountryName = Request.Form["ApplicantCountryName"];

                #region UploadedPdfFile
                // Handle Pdf File Upload
                if (model.UploadedPdfFile != null && model.UploadedPdfFile.Length > 0)
                {
                    // Validate file type
                    if (!model.UploadedPdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(string.Empty, "The file must be a PDF.");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Validate file size (e.g., max 20 MB)
                    if (model.UploadedPdfFile.Length > 20 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "The file size must be less than 20 MB.");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Define the path to save the file
                    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Ensure the uploads directory exists
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    // Sanitize the applicant's email for safe file use
                    var sanitizedEmail = model.ApplicantEmail.Replace('@', '_').Replace('.', '_');

                    // Generate a unique file name to prevent overwriting
                    var uniqueFileName = $"FilmCertificatePdf_{sanitizedEmail}_{Guid.NewGuid()}.pdf";
                    var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedPdfFile.CopyToAsync(fileStream);
                    }

                    // Store the relative file path in the database
                    model.UploadedPdfFilePath = "/uploads/" + uniqueFileName;
                }
                #endregion

                #region UploadedMoviePictures
                // Handle Movie Pictures Files Upload
                // Assume model.UploadedFiles is the property that contains the list of uploaded files
                var uploadedMoviePicturesFilesPaths = 
                    await HandleMoviePicturesFilesUploadAsync(model.UploadedMoviePictures!, model.ApplicantEmail);

                // Check if there were any issues with file uploads
                if (string.IsNullOrEmpty(uploadedMoviePicturesFilesPaths) && !ModelState.IsValid)
                {
                    // Return the view with validation errors if any files failed to upload
                    ModelState.AddModelError(string.Empty, "Failed to upload movie pictures.");
                    var updatedModel = PopulateModelAndViewData(model);
                    return View(updatedModel);
                }

                // Continue processing if the files were uploaded successfully
                model.UploadedMoviePicturesFilePaths = uploadedMoviePicturesFilesPaths;
                #endregion

                #region UploadedMoviePoster
                // Handle Movie Poster File Upload
                if (model.UploadedMoviePoster != null && model.UploadedMoviePoster.Length > 0)
                {
                    // Validate file type
                    if (!model.UploadedMoviePoster.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(string.Empty, "The file must be an image.");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Validate file size (e.g., max 3 MB)
                    if (model.UploadedMoviePoster.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "The file size must be less than 3 MB.");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Define the path to save the file
                    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");

                    // Ensure the uploads directory exists
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    // Sanitize the email for the file name
                    var sanitizedEmail = model.ApplicantEmail.Replace("@", "_").Replace(".", "_");

                    // Generate the new file name "MoviePoster_{sanitizedEmail}_{uniqueId}{fileExtension}"
                    var fileExtension = Path.GetExtension(model.UploadedMoviePoster.FileName);
                    var uniqueId = Guid.NewGuid().ToString(); // Generate a unique ID
                    var newFileName = $"MoviePoster_{sanitizedEmail}_{uniqueId}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, newFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedMoviePoster.CopyToAsync(fileStream);
                    }

                    // Store the relative file path in the database
                    model.UploadedMoviePosterFilePath = "/pictures/" + newFileName;
                }
                #endregion

                #region UploadedMovieSubtitle
                // Handle Movie Subtitle File Upload
                if (model.UploadedMovieSubtitle != null && model.UploadedMovieSubtitle.Length > 0)
                {
                    // Check file extension
                    var allowedExtensions = new[] { ".srt", ".ass", ".sub" };
                    var fileExtension = Path.GetExtension(model.UploadedMovieSubtitle.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError(string.Empty, "The file must be a valid subtitle format (.srt, .ass, .sub).");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Validate file size (e.g., max 2 MB)
                    if (model.UploadedMovieSubtitle.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "The file size must be less than 2 MB.");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Define the path to save the file
                    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/subtitles");

                    // Ensure the uploads directory exists
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    // Generate a unique ID
                    var uniqueId = Guid.NewGuid().ToString("N");

                    // Construct the new file name
                    var sanitizedEmail = model.ApplicantEmail.Replace("@", "_").Replace(".", "_");
                    var newFileName = $"MovieSubtitle_{sanitizedEmail}_{uniqueId}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, newFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedMovieSubtitle.CopyToAsync(fileStream);
                    }

                    // Store the relative file path in the database
                    model.UploadedMovieSubtitleFilePath = $"/subtitles/{newFileName}";
                }
                #endregion

                #region UploadedDirectorPhoto
                // Handle Director Photo File Upload
                if (model.UploadedDirectorPhoto != null && model.UploadedDirectorPhoto.Length > 0)
                {
                    // Check file extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.UploadedDirectorPhoto.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError(string.Empty, "The file must be a valid image format (.jpg, .jpeg, .png, .gif).");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Validate file size (e.g., max 3 MB)
                    if (model.UploadedDirectorPhoto.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "The file size must be less than 3 MB.");
                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }

                    // Define the path to save the file
                    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");

                    // Ensure the uploads directory exists
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    // Generate a unique file name
                    var sanitizedEmail = model.ApplicantEmail.Replace("@", "_").Replace(".", "_");
                    var uniqueId = Guid.NewGuid().ToString(); // Unique identifier for each file
                    var newFileName = $"DirectorPhoto_{sanitizedEmail}_{uniqueId}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, newFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedDirectorPhoto.CopyToAsync(fileStream);
                    }

                    // Store the relative file path in the database
                    model.UploadedDirectorPhotoFilePath = $"/pictures/{newFileName}";
                }
                #endregion

                #region UploadedBestActressPhoto
                // Handle Best Actress Photo upload
                string uploadedBestActressPhotoFilePath = string.Empty;
                if (model.UploadedBestActressPhoto != null && model.UploadedBestActressPhotoName != null)
                {
                    // Pass the applicant email to the upload handler
                    uploadedBestActressPhotoFilePath = await HandleUploadedBestActressPhotoUploadAsync(
                        model.UploadedBestActressPhoto,
                        model.UploadedBestActressPhotoName,
                        model.ApplicantEmail); // Ensure applicantEmail is included

                    if (string.IsNullOrEmpty(uploadedBestActressPhotoFilePath))
                    {
                        // Handle the case where the file upload failed
                        ModelState.AddModelError(string.Empty, "Best Actress Photo could not be uploaded.");

                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }
                }

                // Continue processing if the files were uploaded successfully
                model.UploadedBestActressPhotoFilePath = uploadedBestActressPhotoFilePath;
                #endregion

                #region UploadedBestActorPhoto
                // Handle Best Actor Photo upload
                string UploadedBestActorPhotoFilePath = string.Empty;
                if (model.UploadedBestActorPhoto != null && model.UploadedBestActorPhotoName != null)
                {
                    // Assuming applicantEmail is available in the model
                    string applicantEmail = model.ApplicantEmail; // Adjust this line based on where you get the applicant email

                    UploadedBestActorPhotoFilePath = await HandleUploadedBestActorPhotoUploadAsync(
                        model.UploadedBestActorPhoto, model.UploadedBestActorPhotoName, applicantEmail);

                    if (string.IsNullOrEmpty(UploadedBestActorPhotoFilePath))
                    {
                        // Handle the case where the file upload failed
                        ModelState.AddModelError(string.Empty, "Best Actor Photo could not be uploaded.");

                        var updatedModel = PopulateModelAndViewData(model);
                        return View(updatedModel);
                    }
                }

                // Continue processing if the files were uploaded successfully
                model.UploadedBestActorPhotoFilePath = UploadedBestActorPhotoFilePath;
                #endregion

                var user = new CompetitionApplicationUser
                {
                    CompetitionCategory = model.CompetitionCategoryDescription, // Comes from the page form
                    CompetitionApplicationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone),
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
                    DirectorCompany = model.DirectorCompany,
                    DirectorCountry = directorCountryName!,
                    DirectorPhone = model.DirectorPhone,
                    DirectorEmail = model.DirectorEmail,
                    DirectorBiographyTr = model.DirectorBiographyTr,
                    DirectorBiographyEn = model.DirectorBiographyEn,
                    DirectorFilmographyTr = model.DirectorFilmographyTr,
                    DirectorFilmographyEn = model.DirectorFilmographyEn,

                    // Movie Tag
                    MovieScript = $"{model.ScreenWriter1};{model.ScreenWriter2 ?? string.Empty};{model.ScreenWriter3 ?? string.Empty}",
                    Cinematographer = model.Cinematographer,
                    MovieFiction = $"{model.MovieFiction1};{model.MovieFiction2 ?? string.Empty}",

                    // Producer
                    ProducerName = model.ProducerName,
                    ProducerCompany = model.ProducerCompany,
                    //ProducerCountry = model.ProducerCountry,
                    ProducerCountry = producerCountryName!,
                    ProducerPhone = model.ProducerPhone,
                    ProducerEmail = model.ProducerEmail,

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
                    UploadedMoviePicturesFilePaths = model.UploadedMoviePicturesFilePaths!,
                    UploadedMoviePosterFilePath = model.UploadedMoviePosterFilePath!,
                    UploadedMovieSubtitleFilePath = model.UploadedMovieSubtitleFilePath,
                    UploadedDirectorPhotoFilePath   = model.UploadedDirectorPhotoFilePath!,
                    UploadedBestActressPhotoFilePath = model.UploadedBestActressPhotoFilePath,
                    UploadedBestActorPhotoFilePath = model.UploadedBestActorPhotoFilePath,

                    // DOWNLOADABLE SCREENING COPY OF THE FILM
                     MovieLink = model.MovieLink,
                     MovieLinkPassword  = model.MovieLinkPassword,
                     TrailerLink = model.MovieLink,
                     TrailerLinkPassword = model.MovieLinkPassword,
                     DownloadableCopyCheck = model.DownloadableCopyCheck,

                    // APPLICANT
                    ApplicantName = model.ApplicantName,
                    ApplicantCompany = model.ApplicantCompany,
                    ApplicantCountry = applicantCountryName!,
                    ApplicantPhone = model.ApplicantPhone,
                    ApplicantEmail = model.ApplicantEmail,
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
        private CompetitionApplicationUserViewModel PopulateModelAndViewData(CompetitionApplicationUserViewModel model)
        {
            model.CompetitionCategory = Request.Form["CompetitionCategory"]!;

            model.OriginalMovieName = Request.Form["OriginalMovieName"]!;
            model.EnglishMovieName = Request.Form["EnglishMovieName"]!;


            // Ensure the form retains the selected values
            var selectedCountriesList = Request.Form["SelectedCountries"].ToList();
            var selectedMovieGenres = Request.Form["SelectedMovieGenres"].ToList();

            if (selectedCountriesList != null && selectedMovieGenres != null)
            {
                model.SelectedCountries = selectedCountriesList!;
                model.SelectedMovieGenres = selectedMovieGenres!;
            }

            // Re-fetch the country and genre lists for the view
            model.AllCountries = _competitionApplicationFormService.GetAllCountries();
            model.AllMovieGenres = _competitionApplicationFormService.GetAllGenres();

            // Retain other form values
            //model.ProductionYear = Request.Form["ProductionYear"]!;
            model.MovieTimeLength = Request.Form["MovieTimeLengthName"]!;
            //model.MovieLanguage = Request.Form["MovieLanguageName"]!;

            //model.DirectorCountry = Request.Form["DirectorCountryName"]!;
            //model.ProducerCountry = Request.Form["ProducerCountryName"]!;
            //model.ApplicantCountry = Request.Form["ApplicantCountryName"]!;

            model.ScreenWriter1 = Request.Form["ScreenWriter1"]!;
            model.MovieFiction1 = Request.Form["MovieFiction1"]!;

            // Add a model error if necessary
            ModelState.AddModelError(string.Empty, "One of the inputs is not correct!");

            return model;
        }

        private async Task<string> HandleMoviePicturesFilesUploadAsync(List<IFormFile> uploadedMoviePicturesFiles, string applicantEmail)
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

            // Loop through each uploaded file and rename them with applicant email and unique ID
            foreach (var file in uploadedMoviePicturesFiles)
            {
                // Validate file type
                if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "All uploaded files must be pictures.");
                    return string.Empty; // Return empty string
                }

                // Sanitize the email to avoid invalid characters in file names
                var sanitizedEmail = applicantEmail.Replace("@", "_").Replace(".", "_");

                // Generate a unique file name with applicant email and unique ID
                var uniqueId = Guid.NewGuid().ToString();
                var uniqueFileName = $"MoviePicture_{sanitizedEmail}_{uniqueId}{Path.GetExtension(file.FileName)}";
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

        private async Task<string> HandleUploadedBestActressPhotoUploadAsync(IFormFile uploadedBestActressPhoto, string actressName, string applicantEmail)
        {
            if (uploadedBestActressPhoto == null)
            {
                ModelState.AddModelError(string.Empty, "Please upload a photo for the Best Actress.");
                return string.Empty;
            }

            // Validate file size (max 2 MB)
            if (uploadedBestActressPhoto.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "The photo size must be less than 2 MB.");
                return string.Empty;
            }

            // Validate file type
            if (!uploadedBestActressPhoto.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The uploaded file must be a picture.");
                return string.Empty;
            }

            // Sanitize the actress name for file naming
            string sanitizedActressName = Path.GetInvalidFileNameChars()
                                                .Aggregate(actressName, (current, c) => current.Replace(c, '_'));

            // Sanitize the applicant email for file naming
            string sanitizedEmail = applicantEmail.Replace("@", "_").Replace(".", "_");

            // Generate a unique identifier
            string uniqueId = Guid.NewGuid().ToString();

            // Generate a new file name with the actress's name, email, and unique ID
            string fileExtension = Path.GetExtension(uploadedBestActressPhoto.FileName);
            string newFileName = $"BestActressPhoto_{sanitizedActressName}_{sanitizedEmail}_{uniqueId}{fileExtension}";

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
                await uploadedBestActressPhoto.CopyToAsync(fileStream);
            }

            // Return the relative file path
            return $"/pictures/{newFileName}";
        }

        private async Task<string> HandleUploadedBestActorPhotoUploadAsync(IFormFile uploadedBestActorPhoto, string actorName, string applicantEmail)
        {
            if (uploadedBestActorPhoto == null)
            {
                ModelState.AddModelError(string.Empty, "Please upload a photo for the Best Actor.");
                return string.Empty;
            }

            // Validate file size (max 2 MB)
            if (uploadedBestActorPhoto.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "The photo size must be less than 2 MB.");
                return string.Empty;
            }

            // Validate file type
            if (!uploadedBestActorPhoto.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The uploaded file must be a picture.");
                return string.Empty;
            }

            // Sanitize the actor name for file naming
            string sanitizedActorName = Path.GetInvalidFileNameChars()
                                            .Aggregate(actorName, (current, c) => current.Replace(c, '_'));

            // Generate a unique file name with the actor's name and applicant email
            string fileExtension = Path.GetExtension(uploadedBestActorPhoto.FileName);
            string uniqueId = Guid.NewGuid().ToString(); // Unique identifier
            string newFileName = $"BestActorPhoto_{sanitizedActorName}_{applicantEmail.Replace('@', '_').Replace('.', '_')}_{uniqueId}{fileExtension}";

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
                await uploadedBestActorPhoto.CopyToAsync(fileStream);
            }

            // Return the relative file path
            return $"/pictures/{newFileName}";
        }

        #endregion

    }
}
