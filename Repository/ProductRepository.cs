using BanPhimCanhCach.AppData;
using BanPhimCanhCach.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace BanPhimCanhCach.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }
        public async Task<ProductImage> GetProductImage(int? id)
        {
           return await _dbContext.ProductImages.Where(p => p.Id == id).FirstOrDefaultAsync();
        }
        public async Task Delete(int? id)
        {
            var product = await GetById(id);
            var productImages = await GetProductImages(id);
            var productVariants = await GetProductVariants(id);
            if (product != null)
                _dbContext.Products.Remove(product);
                _dbContext.ProductVariants.RemoveRange(productVariants);
                _dbContext.ProductImages.RemoveRange(productImages);
        }

        public async Task<List<Product>> GetAll(int itemPerPage, int currentPage)
        {
            return await _dbContext.Products.OrderByDescending(p => p.UpdatedAt).Skip(itemPerPage * (currentPage - 1)).Take(itemPerPage).Include(p => p.ProductImages).ToListAsync();
        }

        public async Task<Product> GetById(int? id)
        {
            var product = await _dbContext.Products
                         .Include(p => p.ProductVariants)
                         .Include(p => p.ProductImages)
                         .FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }

        public async Task<List<Product>> GetHotProduct()
        {
            var query = (from p in _dbContext.Products

                         orderby p.ViewCount descending

                         select p).Include(p => p.ProductImages);
            return await query.Take(4).ToListAsync();
        }
        public async Task<List<Product>> GetNewProduct()
        {
            var query = (from p in _dbContext.Products
                         orderby p.UpdatedAt descending
                         select p).Include(p => p.ProductImages);
            return await query.Take(4).ToListAsync();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Update(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
        }

        public bool ProductExists(int id)
        {
            return _dbContext.Products.Any(e => e.Id == id);
        }

        public async Task AddProductVariant(ProductVariant productVariant)
        {
            await _dbContext.ProductVariants.AddAsync(productVariant);
        }

        public async Task AddProductImage(ProductImage productImage)
        {

            await _dbContext.ProductImages.AddAsync(productImage);
        }

        public async Task<ProductVariant> GetProductVariant(int id)
        {
            return await _dbContext.ProductVariants.Where(pc => pc.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<ProductVariant>> GetProductVariants(int? id)
        {
            return await _dbContext.ProductVariants.Where(pc => pc.ProductId == id).ToListAsync();
        }

        public async Task<List<ProductImage>> GetProductImages(int? id)
        {
            return await _dbContext.ProductImages.Where(pc => pc.ProductId == id).ToListAsync();
        }

        public void UpdateProductVariant(ProductVariant productVariant)
        {
            _dbContext.Entry(productVariant).State = EntityState.Modified;
        }

        public void DeleteProductVariant(ProductVariant productVariant)
        {

            _dbContext.ProductVariants.Remove(productVariant);

        }
        public void UpdateProductImage(ProductImage productImage)
        {
            _dbContext.Entry(productImage).State = EntityState.Modified;
        }

        public void DeleteProductImage(ProductImage productImage)
        {
            _dbContext.ProductImages.Remove(productImage);
        }

        public async Task<FilterModel> GetAllProductVariants(int? priceRange, int? currentPage, string sortOrder = "asc")
        {
            IQueryable<ProductVariant> query;
            switch (priceRange)
            {
                case 1:
                    query = from product in _dbContext.ProductVariants
                            where (product.CurrentPrice >= 0 && product.CurrentPrice < 1000000)
                            select product;
                    break;
                case 2:
                    query = from product in _dbContext.ProductVariants
                            where (product.CurrentPrice >= 1000000 && product.CurrentPrice < 2000000)
                            select product;
                    break;
                case 3:
                    query = from product in _dbContext.ProductVariants
                            where (product.CurrentPrice >= 2000000 && product.CurrentPrice < 3000000)
                            select product;
                    break;
                case 4:
                    query = from product in _dbContext.ProductVariants
                            where (product.CurrentPrice >= 3000000 && product.CurrentPrice < 5000000)
                            select product;
                    break;
                case 5:
                    query = from product in _dbContext.ProductVariants
                            where (product.CurrentPrice >= 5000000)
                            select product;
                    break;
                default:
                    query = from product in _dbContext.ProductVariants
                            select product;
                    break;
            }
            int productCount = query.Count();
            List<ProductVariant> products = new List<ProductVariant>();
            if (sortOrder == "des")
            {
                products = await query.OrderByDescending(p => p.CurrentPrice).Skip(9 * ((int)currentPage - 1)).Take(9).ToListAsync();   
            }
            else
            {
                products = await query.OrderBy(p => p.CurrentPrice).Skip(9 * ((int)currentPage - 1)).Take(9).ToListAsync();
            };
            var filterModel = new FilterModel { Products = products, ProductCount = productCount };
            return filterModel;
        }

        public async Task<int> CountProduct()
        {
            return await _dbContext.Products.CountAsync();
        }
        public async Task<int> CountProductVariant()
        {
            return await _dbContext.ProductVariants.CountAsync();
        }
        public async Task<bool> UpdateProductQuantity(int productId, int quantity)
        {
            var product = await GetProductVariant(productId);
            if(product == null)
            {
                return false;
            }
            var checkResult = await CheckInventory(product.Id, quantity);
            if(checkResult)
            {
                product.Quantity += quantity;
                return true;
            }
            return false;

        }
        //Kiem tra so luong yeu cau co du so luong dat hay khong
        public async Task<bool> CheckInventory(int productId, int quantity)
        {
            var product = await GetProductVariant(productId);
            if (product == null)
            {
                return false;
            }
            if(quantity > 0)
            {
                return true;
            }
            else if (quantity <= 0 && Math.Abs(quantity) <= product.Quantity)
            {
                return true;
            }
            return false;
        }

        public async Task<SortedList<string, int>> CountPriceRangeProduct()
        {
            var productCountList = new SortedList<string, int>();
            var all = await _dbContext.ProductVariants.CountAsync();
            var from0to1 = await _dbContext.ProductVariants.Where(p => p.CurrentPrice > 0 && p.CurrentPrice <= 1000000).CountAsync();
            var from1to2 = await _dbContext.ProductVariants.Where(p => p.CurrentPrice > 1000000 && p.CurrentPrice <= 2000000).CountAsync();
            var from2to3 = await _dbContext.ProductVariants.Where(p => p.CurrentPrice > 2000000 && p.CurrentPrice <= 3000000).CountAsync();
            var from3to5 = await _dbContext.ProductVariants.Where(p => p.CurrentPrice > 3000000 && p.CurrentPrice < 5000000).CountAsync();
            var over5 = await _dbContext.ProductVariants.Where(p => p.CurrentPrice > 5000000).CountAsync();
            productCountList.Add("all", all);
            productCountList.Add("from0to1", from0to1);
            productCountList.Add("from1to2", from1to2);
            productCountList.Add("from2to3", from2to3);
            productCountList.Add("from3to5", from3to5);
            productCountList.Add("over5", over5);
            return productCountList;
        }
    }
}