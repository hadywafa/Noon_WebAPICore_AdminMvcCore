using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EFModel.Models.EFModels;
using SqlServerDBContext;
using Repository.UnitWork;
using Microsoft.AspNetCore.Identity;
using EFModel.Models;
using Repository.GenericRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NoonAdminMvcCore.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace NoonAdminMvcCore.Controllers
{

    public class ProductController : Controller
    {
        #region Intitilazition Repo
        private readonly IUnitOfWork _unitOfWork;


        private readonly UserManager<User> _userManager;
        private readonly IGenericRepo<User> _userRepository;
        private readonly IGenericRepo<Product> _productRepository;
        private readonly IGenericRepo<Image> _imageRepository;
        private readonly IConfiguration Configuration;
        private readonly IGenericRepo<Brand> _BrandsRepository;

        readonly IGenericRepo<Category> _categoryRepository;
        #endregion


        private readonly IWebHostEnvironment iweb;
        public ProductController(IUnitOfWork unitOfWork, UserManager<User> userManager, IWebHostEnvironment _iwab, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            Configuration = configuration;
            _userRepository = _unitOfWork.Users;
            _productRepository = _unitOfWork.Products;
            _imageRepository = _unitOfWork.Images;
            _categoryRepository = _unitOfWork.Categories;
            _BrandsRepository = _unitOfWork.Brands;
            iweb = _iwab;
        }

        // GET: Product
        public async Task<IActionResult> Display(string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["PageSize"] = pageSize;


            var prods = new List<Product>();
            var products = _productRepository.GetAll().Include(i => i.ImagesGallery)
                .Include(c => c.Category).Include(s => s.Seller.User).ToList();
            if (!(String.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(currentFilter)))
            {
                // Case: first search but not first page => second, third ...etc
                if (string.IsNullOrEmpty(searchString))
                {
                    searchString = currentFilter;
                    ViewData["CurrentFilter"] = searchString;
                }
                else
                {
                    // Case: Search changed
                    //If the search string is changed during paging, the page has to be reset to 1
                    pageNumber = 1;
                }

                foreach (var prod in products)
                {

                    var _prod = await _productRepository.Find(p => p.Id == prod.Id

                        && (p.Name.Contains(searchString)
                        || p.NameArabic.Contains(searchString) || p.Quantity.ToString().Contains(searchString)
                        || p.Seller.User.FirstName.Contains(searchString) || p.Seller.User.LastName.Contains(searchString)
                        || p.Category.Name.Contains(searchString) || p.Category.NameArabic.Contains(searchString)));

                    if (_prod != null)
                        prods.Add(_prod);
                }
            } // B- No search => Get All
            else
            {
                prods = _productRepository.GetAll().Include(i => i.ImagesGallery)
               .Include(c => c.Category).Include(s => s.Seller.User).ToList();
            }

            if (products.Any())
            {
                // send role, users count, and the current page number to the view page
                // we will need to get them later in suspend and activate actions below

                ViewBag.Count = products.Count();
                ViewBag.Page = pageNumber;

                if (searchString != null)
                {
                    ViewBag.Search = searchString;
                }

                // Sepcifiy number of users you want to display in one page
                int rowsPerPage = pageSize ?? 10;
                ViewBag.rowsPerPage = rowsPerPage;


                //return View(users);
                return View("Index", EFModel.Models.PaginatedList<Product>.CreateAsync(prods, pageNumber ?? 1, rowsPerPage));
            }
            else
            {
                // else no users are found
                return NotFound();
            }
        }



        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Seller = new SelectList(await _userManager.GetUsersInRoleAsync("Seller"), "Id", "FirstName");
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().Where(c => c.ParentID == null).ToList(), "Id", "Name");
            ViewBag.Brands = new SelectList(_BrandsRepository.GetAll().ToList(), "Id", "Name");
            var productViewModel = new ProductViewModel { IsActive = true };
            return View("productForm", productViewModel);
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productVM, [FromForm] IFormFile[] files)
        {
            if (ModelState.IsValid)
            {

                if (productVM.Id == null)
                {
                    #region addproduct
                    Product prod = new Product()
                    {
                        Name = productVM.Name,
                        NameArabic = productVM.NameArabic,
                        ModelNumber = productVM.ModelNumber,
                        Description = productVM.Description,
                        DescriptionArabic = productVM.DescriptionArabic,
                        BuyingPrice = productVM.BuyingPrice,
                        SellingPrice = productVM.SellingPrice,
                        Discount = productVM.Discount,
                        Quantity = productVM.Quantity,
                        Weight = productVM.Weight,
                        SellerId = productVM.SellerId,
                        CategoryId = productVM.CategoryId,
                        IsAvailable = productVM.IsActive,
                        Revenue = (productVM.SellingPrice - productVM.BuyingPrice),
                        MaxQuantityPerOrder = productVM.MaxQuantityPerOrder,
                        Brand = await _BrandsRepository.GetById(productVM.BrandId),
                };
                    await _productRepository.Add(prod);
                    await _unitOfWork.Save();
                    #endregion

                    #region Fileimage
                    files = productVM.Images;
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            //============
                            //var filepath = Configuration["imagesPath"] + "/images/" + item.FileName;
                            //var stream = new FileStream(filepath, FileMode.Create);
                            //item.CopyTo(stream);
                            //============
                            var imgSave = Path.Combine(iweb.WebRootPath, "Images");
                            string filePath = Path.Combine(imgSave, (item.FileName));
                            var stream = new FileStream(filePath, FileMode.Create);
                            await item.CopyToAsync(stream);
                            Image img = new Image()
                            {
                                ImageName = item.FileName,
                                ProductId = prod.Id
                            };

                            if (files.First() == item)
                            {
                                prod.ImageThumb = item.FileName;
                            }
                            
                            await _imageRepository.Add(img);

                        }

                        await _productRepository.Update(prod);

                        await _unitOfWork.Save();
                    }
                    #endregion

                    return RedirectToAction("Display");
                }
                else
                { //if productVm is not null  for Updtening

                    #region Update product
                    int targtedId = int.Parse(productVM.Id);
                    var prod = await _productRepository.GetById(targtedId);

                    if (prod == null)
                    {
                        return NotFound();
                    }

                    prod.Name = productVM.Name;
                    prod.NameArabic = productVM.NameArabic;
                    prod.ModelNumber = productVM.ModelNumber;
                    prod.Description = productVM.Description;
                    prod.DescriptionArabic = productVM.DescriptionArabic;
                    prod.BuyingPrice = productVM.BuyingPrice;
                    prod.SellingPrice = productVM.SellingPrice;
                    prod.Discount = productVM.Discount;
                    prod.Quantity = productVM.Quantity;
                    prod.Weight = productVM.Weight;
                    prod.SellerId = productVM.SellerId;
                    prod.CategoryId = productVM.CategoryId;
                    prod.IsAvailable = productVM.IsActive;
                    prod.Revenue = (productVM.SellingPrice - productVM.BuyingPrice);
                    prod.MaxQuantityPerOrder = productVM.MaxQuantityPerOrder;
                    prod.Brand = await _BrandsRepository.GetById(productVM.BrandId);

                    await _productRepository.Update(prod);
                    await _unitOfWork.Save();
                    #endregion

                    #region update ImageName
                    files = productVM.Images;
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            var imgSave = Path.Combine(iweb.WebRootPath, "Images");
                            string filePath = Path.Combine(imgSave, (item.FileName));
                            var stream = new FileStream(filePath, FileMode.Create);
                            await item.CopyToAsync(stream);
                            Image img = await _imageRepository.GetAll().Where(i => i.ProductId == prod.Id).FirstOrDefaultAsync();

                            img.ImageName = item.FileName;
                            await _imageRepository.Update(img);

                            if (files.First() == item)
                            {
                                prod.ImageThumb = item.FileName;
                            }

                        }

                        await _productRepository.Update(prod);

                        await _unitOfWork.Save();

                    }

                    #endregion

                    return RedirectToAction("Display");
                }
            }

            return RedirectToAction();
        }

        //GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            #region fetch data for updating
            var productVM = await _productRepository.Find(u => u.Id == id, u => u.Brand);


            if (productVM == null)
            {
                return NotFound();
            }

            var productViewmodel = new ProductViewModel
            {
                Id = productVM.Id.ToString(),
                Name = productVM.Name,
                NameArabic = productVM.NameArabic,
                ModelNumber = productVM.ModelNumber,
                Description = productVM.Description,
                DescriptionArabic = productVM.DescriptionArabic,
                BuyingPrice = productVM.BuyingPrice,
                SellingPrice = productVM.SellingPrice,
                Discount = productVM.Discount,
                Quantity = productVM.Quantity,
                Weight = productVM.Weight,
                SellerId = productVM.SellerId,
                CategoryId = productVM.CategoryId,
                MaxQuantityPerOrder = productVM.MaxQuantityPerOrder,
                BrandId = productVM.Brand.Id,
                IsActive = productVM.IsAvailable
            };

            ViewBag.Seller = new SelectList(await _userManager.GetUsersInRoleAsync("Seller"), "Id", "FirstName");
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "Name");
            ViewBag.Brands = new SelectList(_BrandsRepository.GetAll().ToList(), "Id", "Name");

            #endregion

            return View("ProductForm", productViewmodel);
        }

        public async Task<ActionResult> Suspend(int id, string currentFilter, int? pageNumber)
        {
            // get the product
            var prod = await _productRepository.GetById(id);

            //suspend the product
            prod.IsAvailable = false;

            //update database
            await _productRepository.Update(prod);

            // save updates
            await _unitOfWork.Save();

            return RedirectToAction("Display", new { currentFilter = currentFilter, pageNumber = pageNumber });
        }

        public async Task<ActionResult> Activate(int id, string currentFilter, int? pageNumber)
        {
            // get the product
            var prod = await _productRepository.GetById(id);

            // suspend the product
            prod.IsAvailable = true;

            // update database
            await _productRepository.Update(prod);

            // save updates
            await _unitOfWork.Save();

            return RedirectToAction("Display", new { currentFilter = currentFilter, pageNumber = pageNumber });
        }

        private void deleteFilefromRoot(string img)
        {
            img = Path.Combine(iweb.WebRootPath, "Images", img);
            FileInfo fileinfo = new FileInfo(img);
            if (fileinfo != null)
            {
                System.IO.File.SetAttributes(img, FileAttributes.Normal);
                System.IO.File.Delete(img);
            }
        }
    }

}
