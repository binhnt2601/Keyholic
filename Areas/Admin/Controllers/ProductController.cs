using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BanPhimCanhCach.AppData;
using BanPhimCanhCach.Models;
using BanPhimCanhCach.Repository;
using Microsoft.Extensions.Hosting;

namespace BanPhimCanhCach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/Admin/Product/{action}")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        [TempData]
        public string StatusMessage { get; set; }

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(int currentPage, int pageSize)
        {
            int totalProducts = await _productRepository.CountProduct();
            if (pageSize <= 0) pageSize = 10;

            int countPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", "Product", new
                {
                    currentPage = pageNumber,
                    pageSize = pageSize
                })
            };

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalProducts = totalProducts;

            var productsInPage = await _productRepository.GetAll(pageSize, currentPage);
            return View(productsInPage);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task Create(CreateProductModel product)
        {
            var newProduct = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                IsHot = product.IsHot,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now

            };
            await _productRepository.Add(newProduct);
            await _productRepository.Save();
            if (product.ProductImages != null)
                {
                    foreach (var image in product.ProductImages)
                    {
                        var productImage = new ProductImage()
                        {
                            ImgSrc = image.ImgSrc,
                            ProductId = newProduct.Id
                        };
                        await _productRepository.AddProductImage(productImage);
                    }
                }
            if(product.ProductVariants != null)
            {
                foreach (var Variant in product.ProductVariants)
                {
                    var productVariant = new ProductVariant()
                    {
                        Name = Variant.Name,
                        OriginalPrice = Variant.OriginalPrice,
                        CurrentPrice = Variant.OriginalPrice,
                        Quantity = Variant.Quantity,
                        ImgSrc = Variant.ImgSrc,
                        ProductId = newProduct.Id, 
                        ProductName = newProduct.Name
                    };
                    await _productRepository.AddProductVariant(productVariant);
                }
            }
            await _productRepository.Save();
            StatusMessage = "Thêm sản phẩm thành công";
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductVariants"] = await _productRepository.GetProductVariants(id);
            ViewData["ProductImages"] = await _productRepository.GetProductImages(id);

            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                StatusMessage = "Lỗi: Không tìn thấy Sản phẩm";
            }

            if (ModelState.IsValid)
            {
                product.UpdatedAt = DateTime.Now;
                _productRepository.Update(product);
                await _productRepository.Save();
                StatusMessage = "Đã cập nhật thông tin sản phẩm";
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                StatusMessage = "Lỗi: Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                StatusMessage = "Lỗi: Không tìm thấy sản phẩm";
                return RedirectToAction("Index");
            }
            await _productRepository.Delete(id);
            await _productRepository.Save();
            StatusMessage = "Đã xóa sản phẩm";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task AddProductVariant(ProductVariant ProductVariant)
        {

            await _productRepository.AddProductVariant(ProductVariant);
            await _productRepository.Save();
            StatusMessage = "Đã thêm phân loại cho sản phẩm";
            
        }

        public async Task<IActionResult> EditProductVariant(int id)
        {
            var productVariant = await _productRepository.GetProductVariant(id);
            if(productVariant == null)
               return NotFound();
            return View(productVariant);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductVariant(ProductVariant productVariant)
        {
            _productRepository.UpdateProductVariant(productVariant);
             await _productRepository.Save();
            StatusMessage = "Đã cập nhật thông tin phân loại";
            return RedirectToAction("Edit", new {id = productVariant.ProductId});
        }

        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            var Variant = await _productRepository.GetProductVariant(id);
            if(Variant == null)
            {
                StatusMessage = "Lỗi: Không tìm thấy phân loại!";
                return RedirectToAction("Edit", new { id = Variant.ProductId });
            }
            _productRepository.DeleteProductVariant(Variant);
            await _productRepository.Save();
            StatusMessage = "Đã xóa phân loại sản phẩm";
            return RedirectToAction("Edit", new { id = Variant.ProductId });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var image = await _productRepository.GetProductImage(id);
            _productRepository.DeleteProductImage(image);
            await _productRepository.Save();
            StatusMessage = "Đã cập nhật hình ảnh sản phẩm.";
            return RedirectToAction("Edit", new { id = image.ProductId });
        }
    }
}
