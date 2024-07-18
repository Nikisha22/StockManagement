using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagement.Models;
using StockManagement.Security;
using System.Diagnostics;
using System.Security.Cryptography;

namespace StockManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly StockManagementDatabaseContext _context;
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _env;
        public HomeController(StockManagementDatabaseContext context, IDataProtectionProvider provider, DataSecurityKey key, IWebHostEnvironment env)
        {
            _context = context;
            _protector = provider.CreateProtector(key.key);
            _env = env;
        }

        public IActionResult Index()
        {
            var prod= _context.Products.ToList();
            var p = prod.Select(e => new ProductEdit
            {
                PId = e.PId,
                Pimage = e.Pimage,
                PName = e.PName,
                PPrice = e.PPrice,
                PurchaseOuantity = e.PurchaseOuantity,
                SaleQuantity = e.SaleQuantity,
                SupplierId = e.SupplierId,
                CategoryId = e.CategoryId,
                Stock = e.Stock,
                EcId = _protector.Protect(Convert.ToString(e.PId))


            }).ToList();
            return View(p);
        }

        [HttpGet]
        public IActionResult Create() {
            var productEdit = new ProductEdit();
            var prodCategories = _context.ProdCategories.ToList();
            ViewData["cate"] = new SelectList(prodCategories,nameof(ProdCategory.PcId),nameof(ProdCategory.PcName));
           var p=_context.Products.ToList();
            return View();
/*            return View();
*/        }

        [HttpPost]
        public IActionResult Create(ProductEdit u)
        {
                short maxid;
                if (_context.Products.Any())

                    maxid = Convert.ToInt16(_context.Products.Max(x => x.PId) + 1);
                else

                    maxid = 1;
                u.PId = maxid;
                if (u != null)
                {
                    string fileName = "ProductImage" + Guid.NewGuid() + Path.GetExtension(u.ProductFile!.FileName);
                    string filePath = Path.Combine(_env.WebRootPath, "ProductImage", fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        u.ProductFile.CopyTo(stream);
                    }
                    u.Pimage = fileName;

                }
                Product p = new()
                {
                    PId = u.PId,
                    Pimage = u.Pimage,
                    PName = u.PName,
                    PPrice = u.PPrice,
                    PurchaseOuantity=u.PurchaseOuantity,
                    CategoryId = u.CategoryId,
                    SaleQuantity = u.SaleQuantity,
                    Stock=u.Stock,
                    SupplierId = u.SupplierId,

                };

                _context.Products.Add(p);
                _context.SaveChanges();
            return Content("Success");

            /*                return RedirectToAction("Index");
            */
        }

        /* public ActionResult Search(string searchString)
         {
             // Retrieve all customers from the database
             var products = _context.Products.ToList();

             // Filter customers by name if search query is provided
             if (!string.IsNullOrEmpty(searchString))
             {
                 products = products
                     .Where(c => c.CategoryId.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                     .ToList();
             }

             // Pass the filtered customers to the view
             return View(products);
         }
 */
        /* public ActionResult Search(string searchString)
         {
             // Retrieve all products from the database
             var products = _context.Products.ToList();

             // Filter products by category ID if search query is provided
             if (!string.IsNullOrEmpty(searchString) && short.TryParse(searchString, out short searchCategoryId))
             {
                 products = products.Where(p => p.CategoryId == searchCategoryId).ToList();
             }

             // Pass the filtered products to the view
             ViewData["Products"] = products;
             ViewData["Category"] = searchString;

             return View("SearchResults");
         }
 */
        
        public IActionResult ProductList()
        {

            ViewData["cat"] = new SelectList(_context.ProdCategories, nameof(ProdCategory.PcId), nameof(ProdCategory.PcName));
            return View();

        }

        
        public IActionResult GetProductList(int PcId)
        {
            
            var product = _context.Products.Where(x => x.CategoryId == PcId).ToList();
            return PartialView("_ProductList", product);
        }


        [HttpGet]
        public IActionResult Delete(string id)
        {
            int UserId = Convert.ToInt32(_protector.Unprotect(id));
            var u = _context.Products.Where(x => x.PId == UserId).FirstOrDefault();
            
            if (u == null)
            {
                return NotFound();
            }
            _context.Products.Remove(u);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(string id)
        {
            var UserId=Convert.ToInt16( _protector.Unprotect(id));

            var product = _context.Products.Where(e => e.PId == UserId).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            var prodCategories = _context.ProdCategories.ToList();
            ViewData["cate"] = new SelectList(prodCategories, nameof(ProdCategory.PcId), nameof(ProdCategory.PcName));

            // Fetch suppliers from the database
            var suppliers = _context.Suppliers.ToList();
            ViewData["suppliers"] = new SelectList(suppliers, nameof(Supplier.SId), nameof(Supplier.SName));


            ProductEdit model = new ProductEdit
            {
                PId = product.PId,
                PName = product.PName,
                PPrice = product.PPrice,
                Pimage = product.Pimage,
                SaleQuantity = product.SaleQuantity,
                PurchaseOuantity = product.PurchaseOuantity,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductEdit m)
        {
            try
            {
                var product = _context.Products.Find(m.PId);
                if (product == null)
                {
                    return NotFound();
                }

                if (m.ProductFile != null)
                {
                    string fileName = "Updated" + Guid.NewGuid() + Path.GetExtension(m.ProductFile.FileName);
                    string filePath = Path.Combine(_env.WebRootPath, "ProductImage", fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        m.ProductFile.CopyTo(stream);
                    }
                    product.Pimage = fileName;
                }

                product.PName = m.PName;
                product.PPrice = m.PPrice;
                product.SaleQuantity = m.SaleQuantity;
                product.PurchaseOuantity = m.PurchaseOuantity;
                product.Stock = m.Stock;
                product.CategoryId = m.CategoryId;
                product.SupplierId = m.SupplierId;

                _context.Products.Update(product);
                _context.SaveChanges();

                return Json(product); // or RedirectToAction("Index") if that's the intended action
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }




        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public IActionResult Update(ProductEdit m)
         {
             try
             {
                 var product = _context.Products.Find(m.PId);
                 if (product == null)
                 {
                     return NotFound();
                 }

                 if (m.ProductFile != null)
                 {
                     string fileName = "Updated" + Guid.NewGuid() + Path.GetExtension(m.ProductFile.FileName);
                     string filePath = Path.Combine(_env.WebRootPath, "ProductImage", fileName);
                     using (FileStream stream = new FileStream(filePath, FileMode.Create))
                     {
                         m.ProductFile.CopyTo(stream);
                     }
                     m.Pimage = fileName;
                 }
                 Product p = new()
                 {

                     PName = m.PName,
                     PPrice = m.PPrice,
                     Pimage = m.Pimage,
                     SaleQuantity = m.SaleQuantity,
                     PurchaseOuantity = m.PurchaseOuantity,
                     Stock = m.Stock,
                     CategoryId = m.CategoryId,
                     SupplierId = m.SupplierId,



                 };
                 _context.Products.Update(product);
                 _context.SaveChanges();
                 return Json(product);
                 return RedirectToAction("Index");
             }
             catch (Exception ex) {
                 return Json(ex);
             }
         }*/

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
