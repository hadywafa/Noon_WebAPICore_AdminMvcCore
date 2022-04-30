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
using Microsoft.AspNetCore.Authorization;
using BL.AppPolicy;

namespace NoonAdminMvcCore.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin)]
    public class CategoryController : Controller
    {

        #region intilaztion repo
        private readonly IUnitOfWork _unitOfWork;

        readonly IGenericRepo<Images> _imageRepository;
        readonly IGenericRepo<Category> _categoryRepository;

        #endregion
        private readonly IWebHostEnvironment iweb;
        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment _iwab)
        {
            _unitOfWork = unitOfWork;
             iweb = _iwab;
            _imageRepository = _unitOfWork.Images;
            _categoryRepository = _unitOfWork.Categories;
        }

        // GET: Category
        public IActionResult Index()
        {
            var cat= _categoryRepository.GetAll().Include(c=>c.Image);
            if (cat.Any())
            {

                return View("Index",cat);
            }

            return NotFound();
        }

        


        // GET: Category/Create
        public IActionResult Create()
        {
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().Where(c => c.ParentID == null).ToList(), "Id", "Name");
            return View("CategoryForm");
        }


       
        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryViewModel categoryVM,[FromForm] IFormFile files)
        {
            if (ModelState.IsValid)
            {

                if (categoryVM.Id == null)
                {
                    files = categoryVM.image;
                    if (files != null)
                    {

                        Category cat = new Category()
                        {
                            Name = categoryVM.Name,
                            NameArabic = categoryVM.NameArabic,
                            Description = categoryVM.Description,
                            DescriptionArabic = categoryVM.DescriptionArabic,
                            ParentID = categoryVM.ParentID == null ? null : categoryVM.ParentID,
                                  

                        };
                        _categoryRepository.Add(cat);
                        _unitOfWork.Save();
                        var imgsave = Path.Combine(iweb.WebRootPath, "Images" );
                        string filepath = Path.Combine(imgsave, (files.FileName));
                        var straem = new FileStream(filepath, FileMode.Create);
                        files.CopyTo(straem);

                        Images img = new Images()
                        { Image =files.FileName,
                            CategoryId = cat.Id
                                                                 
                        };

                        _imageRepository.Add(img);
                        _unitOfWork.Save();

                    }
                    return View("Index", _categoryRepository.GetAll().Include(c => c.Image));

                }
                else
                {
                    int targtedId = int.Parse(categoryVM.Id);
                    var cat = _categoryRepository.GetById(targtedId);

                    if (cat == null)
                    {
                        return NotFound();
                    }

                    cat.Name = categoryVM.Name;
                    cat.NameArabic = categoryVM.NameArabic;

                    cat.Description = categoryVM.Description;
                    cat.DescriptionArabic = categoryVM.DescriptionArabic;

                    _categoryRepository.Update(cat);
                    _unitOfWork.Save();
                    if (files != null)
                    {
                                        
                        var imgsave = Path.Combine(iweb.WebRootPath, "Images", (files.FileName));
                        string filepath = Path.Combine(imgsave, (files.FileName));
                        var straem = new FileStream(filepath, FileMode.Create);
                        files.CopyTo(straem);

                        Images img = _imageRepository.GetAll().Where(i => i.CategoryId== cat.Id).FirstOrDefault();
                        deleteFilefromRoot(img.Image);
                        img.Image = files.FileName;

                        _imageRepository.Update(img);
                        _unitOfWork.Save();
                      


                    }
                    return View("Index", _categoryRepository.GetAll().Include(c => c.Image));
                }

                
            }

            return RedirectToAction();
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int? id)
        {
            #region fetch data for Updating
            var categoryVM = _categoryRepository.Find(u => u.Id == id);

            if (categoryVM == null)
            {
                return NotFound();
            }

            var catViewmodel = new CategoryViewModel
            {
                Id = categoryVM.Id.ToString(),
                Name = categoryVM.Name,
                NameArabic = categoryVM.NameArabic,
                Description = categoryVM.Description,
                DescriptionArabic = categoryVM.DescriptionArabic,
                ParentID = categoryVM.ParentID == null ? null : categoryVM.ParentID
            };
            ViewBag.Category = new SelectList(_categoryRepository.GetAll().Where(c => c.ParentID == null).ToList(), "Id", "Name");
            #endregion
            return View("CategoryForm", catViewmodel);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var cat = _categoryRepository.GetById(id);
            var img = _imageRepository.GetAll().Where(i => i.CategoryId == id).FirstOrDefault();
           
            
            _categoryRepository.Remove(cat);
            _unitOfWork.Save();

           
            return View("Index",_categoryRepository.GetAll().Include(c => c.Image));
        }

        private void deleteFilefromRoot(string  img)
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
