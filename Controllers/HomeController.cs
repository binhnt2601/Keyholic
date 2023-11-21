using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BanPhimCanhCach.Models;
using BanPhimCanhCach.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Drawing.Printing;
using BanPhimCanhCach.Services;

namespace BanPhimCanhCach.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["HotProduct"] = await _productRepository.GetHotProduct();
        ViewData["NewProduct"] = await _productRepository.GetNewProduct();

        return View();
    }

    public async Task<IActionResult> ProductDetail(int? id)
    {
        if(id == null) {
            return NotFound();
        }
        var product = await _productRepository.GetById(id);
        if(product == null)
        {
            return NotFound();
        }
        product.ViewCount += 1;
        await _productRepository.Save();
        return View(product);
    }
    public async Task<IActionResult> Shop(int? priceRange, int? currentPage, string sortOrder = "asc")
    {
        currentPage = currentPage ?? 1;
        var filterModel = await _productRepository.GetAllProductVariants(priceRange, currentPage, sortOrder);
        var allProduct = filterModel.Products;
        int totalProducts = filterModel.ProductCount;

        int countPages = (int)Math.Ceiling((double)totalProducts / 9);
        if (currentPage > countPages) currentPage = countPages;
        if (currentPage < 1) currentPage = 1;

        var pagingModel = new PagingModel()
        {
            countpages = countPages,
            currentpage = (int)currentPage,
            generateUrl = (pageNumber) => Url.Action("Shop", "Home", new
            {
                currentPage = pageNumber,
                priceRange = priceRange,
                sortOrder = sortOrder
            })
        };

        ViewBag.pagingModel = pagingModel;
        ViewBag.totalProducts = totalProducts;
        ViewBag.countProductByPriceRange = await _productRepository.CountPriceRangeProduct();
        return View(allProduct);
    }
    public IActionResult AboutUs()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [Authorize]
    public async Task<IActionResult> UserOrder(string orderStatus)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userOrder = await _orderRepository.GetUserOrder(userId, orderStatus);
        return View(userOrder);
    }
    [Authorize]
    public async Task<IActionResult> UserOrderInfo(string orderId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderRepository.GetUserOrderInfo(userId, orderId);
        return View(order);
    }
}

public class FilterModel
{
    public string  MinPrice { get; set; }
    public string MaxPrice { get; set; }
    public bool Order { get; set; }
}




    