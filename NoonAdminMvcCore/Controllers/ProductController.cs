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
using BL.AppPolicy;

namespace NoonAdminMvcCore.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin)]
    public class ProductController : Controller
    {
        #region Intitilazition Repo
        private readonly IUnitOfWork _unitOfWork;

        
        private readonly UserManager<User> _userManager;
        readonly IGenericRepo<User> _userRepository;
        readonly IGenericRepo<Product> _productRepository;
        readonly IGenericRepo<Images> _imageRepository;
       
        readonly IGenericRepo<Category> _categoryRepository;
        #endregion


        private readonly IWebHostEnvironment iweb;
        public ProductController(IUnitOfWork unitOfWork, UserManager<User> userManager, IWebHostEnvironment _iwab)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userRepository = _unitOfWork.Users;
            _productRepository = _unitOfWork.Products;
            _imageRepository = _unitOfWork.Images;
            _categoryRepository = _unitOfWork.Categories;
            iweb = _iwab;
        }

        // GET: Product
        public IActionResult Index()
        {
            var product = _productRepository.GetAll().Include(i=>i.Images).Include(c=>c.Category).Include(s=>s.Seller.User);
            if (product !=null)
            {
                return View(product);
            }

            return NotFound();
        }



        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Seller = new SelectList(await _userManager.GetUsersInRoleAsync("Seller"), "Id", "FirstName");
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "Name");
            return View("productForm");
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productVM, [FromForm] IFormFile[] files)
        {
            if (ModelState.IsValid)
            {

                if (_productRepository.GetById(productVM.Id) == null)
                {

                    #region addproduct
                    Product prod = new Product()
                    {
                        Name = productVM.Name,
                        NameArabic = productVM.NameArabic,
                        Description = productVM.Description,
                        DescriptionArabic = productVM.DescriptionArabic,
                        Price = productVM.Price,
                        Quantity = productVM.Quantity,
                        Weight = productVM.Weight,
                        SellerId = productVM.SellerId,
                        CategoryId = productVM.CategoryId,
                        

                    };
                    _productRepository.Add(prod);
                    _unitOfWork.Save();
                    #endregion

                    #region Fileimage
                    files = productVM.Images;
                    if (files != null)
                    {

                        foreach (var item in files)
                        {
                            var imgsave = Path.Combine(iweb.WebRootPath, "Images");
                            string filepath = Path.Combine(imgsave, (item.FileName));
                            var straem = new FileStream(filepath, FileMode.Create);
                            item.CopyTo(straem);

                            Images img = new Images()
                            {
                                ProductId = prod.Id,
                                Image = item.FileName

                            };

                            _imageRepository.Add(img);
                            _unitOfWork.Save();

                           
                        }

                    }
                    #endregion


                    return View("Index", _productRepository.GetAll().Include(i => i.Images).Include(c => c.Category).Include(s => s.Seller.User));
                }
                else
                { //if productVm is not null  for Updtening

                    #region Update product
                    var prod = _productRepository.Find(u => u.Id == productVM.Id);

                    if (prod == null)
                    {
                        return NotFound();
                    }

                    prod.Name = productVM.Name;
                    prod.NameArabic = productVM.NameArabic;
                    prod.Price = productVM.Price;
                    prod.Quantity = productVM.Quantity;
                    prod.Description = productVM.Description;
                    prod.DescriptionArabic = productVM.DescriptionArabic;
                    prod.Weight = productVM.Weight;
                    _productRepository.Update(prod);
                    _unitOfWork.Save();
                    #endregion

                    #region update Image
                    files = productVM.Images;
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            var imgsave = Path.Combine(iweb.WebRootPath, "Images", (item.FileName));
                            string filepath = Path.Combine(imgsave, item.FileName);
                            var straem = new FileStream(filepath, FileMode.Create);
                            item.CopyTo(straem);

                            Images img = _imageRepository.GetAll().Where(i => i.ProductId == prod.Id).FirstOrDefault();
                            deleteFilefromRoot(img.Image);
                            img.Image = item.FileName;
                            _imageRepository.Update(img);
                            _unitOfWork.Save();
                        }

                    }

                    #endregion

                    return View("Index", _productRepository.GetAll().Include(i => i.Images).Include(c => c.Category).Include(s => s.Seller.User));
                }

               
              
            }

            return RedirectToAction();
        }

        //GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            #region fetch data for updating
            var productVM = _productRepository.Find(u => u.Id == id);
           
       
            if (productVM == null)
            {
                return NotFound();
            }

            var productViewmodel = new ProductViewModel
            {
                Name = productVM.Name,
                NameArabic = productVM.NameArabic,
                Description = productVM.Description,
                DescriptionArabic = productVM.DescriptionArabic,
                Price = productVM.Price,
                Quantity = productVM.Quantity,
                Weight = productVM.Weight,

            };

            ViewBag.Seller = new SelectList(await _userManager.GetUsersInRoleAsync("Seller"), "Id", "FirstName");
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().ToList(), "Id", "Name");

            #endregion
            return View("ProductForm", productViewmodel);
        }



        [HttpGet]
       
        public IActionResult Delete(int id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var product = _productRepository.GetById(id);
            var img = _imageRepository.GetAll().Where(i => i.ProductId == id);

            if (img.Count() > 0)
            {
                foreach (var item in img)
                {
                    deleteFilefromRoot(item.Image);
                }
            }
            _productRepository.Remove(product);

            if (product == null)
            {
                return NotFound();
            }

            return View("Index", _productRepository.GetAll().Include(i => i.Images).Include(c => c.Category).Include(s => s.Seller.User));
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
