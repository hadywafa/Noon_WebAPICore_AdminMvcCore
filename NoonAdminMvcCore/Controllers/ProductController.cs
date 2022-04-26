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

namespace NoonAdminMvcCore.Controllers
{
    public class ProductController : Controller
    {
        // Unit Of Work which is responsible on operations on Context
        private readonly IUnitOfWork _unitOfWork;

        // User Repo which is responsible on operations on user
        private readonly UserManager<User> _userManager;
        readonly IGenericRepo<User> _userRepository;
        readonly IGenericRepo<Product> _productRepository;
        readonly IGenericRepo<Images> _imageRepository;
       
        readonly IGenericRepo<Category> _categoryRepository;
        // User Repo which is responsible on operations on user



        public ProductController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userRepository = _unitOfWork.Users;
            _productRepository = _unitOfWork.Products;
            _imageRepository = _unitOfWork.Images;
            _categoryRepository = _unitOfWork.Categories;
        }

        // GET: Product
        public IActionResult Index()
        {
            var product = _productRepository.GetAll();
            if (product.Any())
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


        private readonly IWebHostEnvironment iweb;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productVM, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {

                if (_productRepository.GetById(productVM.Id) == null)
                {
                    Product prod = new Product()
                    {
                        Name = productVM.Name,
                        NameArabic = productVM.NameArabic,
                        Description = productVM.Description,
                        DescriptionArabic = productVM.DescriptionArabic,
                        Price = productVM.Price,
                        Quantity = productVM.Quantity,
                        Weight = productVM.Weight,


                    };
                    _productRepository.Add(prod);
                    _unitOfWork.Save();
                    if (files != null)
                    {
                        foreach (var item in files)
                        {
                            var imgsave = Path.Combine(iweb.WebRootPath, "Images", (item.FileName + DateTime.Now.ToShortDateString()));
                            var straem = new FileStream(imgsave, FileMode.Create);
                            item.CopyTo(straem);

                            Images img = new Images()
                            {
                                //productid = productVM.Id,
                                Image = imgsave

                            };

                            _imageRepository.Add(img);
                            _unitOfWork.Save();

                           
                        }

                    }

                   
                    return View("Index");
                }
                else
                {
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
                }

                if (files != null)
                {
                    foreach (var item in files)
                    {
                        var imgsave = Path.Combine(iweb.WebRootPath, "Images", (item.FileName + DateTime.Now.ToShortDateString()));
                        var straem = new FileStream(imgsave, FileMode.Create);
                        item.CopyTo(straem);

                        Images img = _imageRepository.GetAll().Where(i => i.Id == productVM.Id ).FirstOrDefault();

                        img.Image = imgsave;
                        _unitOfWork.Save();
                    }

                }
                return View("Index");
            }

            return RedirectToAction();
        }

        //GET: Product/Edit/5
        public IActionResult Edit(int? id)
        {
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

            return View("ProductForm", productViewmodel);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var product = _productRepository.GetById(id);

            _productRepository.Remove(product);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
