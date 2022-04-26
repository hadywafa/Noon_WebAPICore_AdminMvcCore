using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EFModel.Models.EFModels;
using SqlServerDBContext;
using System.IO;
using NoonAdminMvcCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Repository.UnitWork;
using Repository.GenericRepository;

namespace NoonAdminMvcCore.Controllers
{
    public class CategoryController : Controller
    {
        // Unit Of Work which is responsible on operations on Context
        private readonly IUnitOfWork _unitOfWork;

        // User Repo which is responsible on operations on user

        readonly IGenericRepo<Images> _imageRepository;
        readonly IGenericRepo<Category> _categoryRepository;


        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _imageRepository = _unitOfWork.Images;
            _categoryRepository = _unitOfWork.Categories;
        }

        // GET: Category
        public IActionResult Index()
        {
            var product = _categoryRepository.GetAll();
            if (product.Any())
            {
                return View(product);
            }

            return NotFound();
        }

        // GET: Category/Details/5


        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().Where(c => c.ParentID != null).ToList(), "Id", "Name");
            return View("CategoryForm");
        }


        private readonly IWebHostEnvironment iweb;
        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryViewModel categoryVM, IFormFile files)
        {
            if (!ModelState.IsValid)
            {

                if (_categoryRepository.GetById(categoryVM.Id) == null)
                {

                    if (files != null)
                    {

                        var imgsave = Path.Combine(iweb.WebRootPath, "Images", (files.FileName + DateTime.Now.ToShortDateString()));
                        var straem = new FileStream(imgsave, FileMode.Create);
                        files.CopyTo(straem);

                        Images img = new Images()
                        {     Image = imgsave};

                        _imageRepository.Add(img);
                        _unitOfWork.Save();



                        Category prod = new Category()
                        {
                            Name = categoryVM.Name,
                            NameArabic = categoryVM.NameArabic,
                            Description = categoryVM.Description,
                            DescriptionArabic = categoryVM.DescriptionArabic,
                            ParentID = 1,
                            Image = img


                        };
                        _categoryRepository.Add(prod);
                        _unitOfWork.Save();

                    }





                    return View("Index", _categoryRepository.GetAll());
                }
                else
                {
                    var cat = _categoryRepository.Find(u => u.Id == categoryVM.Id);

                    if (cat == null)
                    {
                        return NotFound();
                    }

                    cat.Name = categoryVM.Name;
                    cat.NameArabic = categoryVM.NameArabic;

                    cat.Description = categoryVM.Description;
                    cat.DescriptionArabic = categoryVM.DescriptionArabic;
                    if (files != null)
                    {

                        var imgsave = Path.Combine(iweb.WebRootPath, "Images", (files
                            .FileName + DateTime.Now.ToShortDateString()));
                        var straem = new FileStream(imgsave, FileMode.Create);
                        files.CopyTo(straem);

                        Images img = _imageRepository.GetAll().Where(i => i.CategoryId == cat.Id).FirstOrDefault();

                        img.Image = imgsave;
                        _unitOfWork.Save();


                    }
                    return View("Index", _categoryRepository.GetAll());
                }

                
            }

            return RedirectToAction();
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int? id)
        {
            var categoryVM = _categoryRepository.Find(u => u.Id == id);

            if (categoryVM == null)
            {
                return NotFound();
            }

            var catViewmodel = new CategoryViewModel
            {
                Name = categoryVM.Name,
                NameArabic = categoryVM.NameArabic,
                Description = categoryVM.Description,
                DescriptionArabic = categoryVM.DescriptionArabic,
                ParentID = categoryVM.ParentID == null ? null : categoryVM.ParentID
            };

            return View("CategoryForm", catViewmodel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var product = _categoryRepository.GetById(id);

            _categoryRepository.Remove(product);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
