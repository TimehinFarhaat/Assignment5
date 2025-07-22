using CSharpMvcBasics.DTO;
using CSharpMvcBasics.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;


public class GalleryController : Controller
{
    private readonly IImageService _imageService;

    public GalleryController(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<IActionResult> Index(string category = null, string sortOrder = null)
    {
        var filter = new ImageFilterParamsDto
        {
            Category = category,
            SortOrder = sortOrder
        };

        var images = await _imageService.GetImagesAsync(filter);

        var vm = new GalleryViewModelDto
        {
            Upload = new ImageUploadDto(),
            Images = images.ToList(),
        };

        return View(vm);
    }







    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(GalleryViewModelDto model)
    {

        if (!ModelState.IsValid || model.Upload?.ImageFile == null || model.Upload.ImageFile.Length == 0)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["Error"] = "Upload failed due to: " + string.Join("; ", errors);
            return RedirectToAction("Index");
        }


        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        try
        {
            var (success, message) = await _imageService.UploadImageAsync(model.Upload, uploadPath);

            if (!success)
            {
                TempData["Error"] = "Upload failed: " + (message ?? "Inappropriate content, duplicate, or wrong category.");
            }
            else
            {
                TempData["Success"] = "Image uploaded successfully!";
                if (!string.IsNullOrWhiteSpace(message))
                {
                    TempData["Note"] = $"Title corrected to '{message}' based on image analysis.";
                }
            }
        }
        catch (Exception ex)
        {
            // TempData["Error"] = "An error occurred during upload. Please try again later.";
            TempData["Error"] = $"An error occurred during upload: {ex.Message}";
            Console.WriteLine(ex);
        }

        return RedirectToAction("Index");
    }




}
