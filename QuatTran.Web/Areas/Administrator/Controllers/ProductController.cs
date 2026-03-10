using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IProductImageService productImageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoryAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ProductDto model,
            IFormFile? mainImageFile,
            List<IFormFile>? additionalImageFiles)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoryAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
                return View(model);
            }

            if (mainImageFile != null && mainImageFile.Length > 0)
            {
                model.ImageUrl = await SaveFile(mainImageFile);
            }

            int newProductId = await _productService.AddProductAsync(model);

            if (additionalImageFiles != null && additionalImageFiles.Count > 0)
            {
                foreach (var file in additionalImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var imageUrl = await SaveFile(file);
                        var imageDto = new ProductImageDto
                        {
                            ProductId = newProductId,
                            ImageUrl = imageUrl,
                            IsMain = false
                        };
                        await _productImageService.AddProductImageAsync(imageDto);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var categories = await _categoryService.GetAllCategoryAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto model, IFormFile? mainImageFile)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoryAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", model.CategoryId);
                return View(model);
            }

            if (mainImageFile != null && mainImageFile.Length > 0)
            {
                model.ImageUrl = await SaveFile(mainImageFile);
            }

            await _productService.UpdateProduct(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UploadImages(int productId)
        {
            ViewBag.ProductId = productId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int productId, List<IFormFile> files)
        {
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var imageUrl = await SaveFile(file);
                        var dto = new ProductImageDto
                        {
                            ProductId = productId,
                            ImageUrl = imageUrl,
                            IsMain = false
                        };
                        await _productImageService.AddProductImageAsync(dto);
                    }
                }
            }
            return RedirectToAction(nameof(Edit), new { id = productId });
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + uniqueFileName;
        }
    }
}
