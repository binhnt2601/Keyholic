using BanPhimCanhCach.Models;
using Microsoft.AspNetCore.Mvc;

namespace BanPhimCanhCach.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll(int itemPerPage, int currentPage);
        Task<Product> GetById(int? id);
        Task Add(Product product);
        void Update(Product product);
        Task Delete(int? id);
        Task Save();
        bool ProductExists(int id);
        Task<List<Product>> GetHotProduct();
        Task<List<Product>> GetNewProduct();
        Task AddProductVariant(ProductVariant ProductVariant);
        Task AddProductImage(ProductImage productImage);

        Task<ProductImage> GetProductImage(int? id);
        Task<ProductVariant> GetProductVariant(int id);
         Task<List<ProductVariant>> GetProductVariants(int? id);
        Task<List<ProductImage>> GetProductImages(int? id);

        void UpdateProductVariant(ProductVariant productVariant);
        void UpdateProductImage(ProductImage productImage);
        void DeleteProductVariant(ProductVariant productVariant);
        void DeleteProductImage(ProductImage productImage);
        Task<FilterModel> GetAllProductVariants(int? priceRange, int? currentPage, string sortOrder = "asc");
        Task<int> CountProduct();
        Task<int> CountProductVariant();
        Task<bool> CheckInventory(int productId, int quantity);
        Task<bool> UpdateProductQuantity(int productId, int quantity);
        Task<SortedList<string, int>> CountPriceRangeProduct();
    }

}