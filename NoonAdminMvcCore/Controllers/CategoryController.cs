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

namespace NoonAdminMvcCore.Controllers
{
    //[Authorize(Roles = AuthorizeRoles.Admin)]
    public class CategoryController : Controller
    {

        #region intilaztion repo
        private readonly IUnitOfWork _unitOfWork;

        readonly IGenericRepo<Image> _imageRepository;
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
        public IActionResult Index(string currentFilter, string searchString, int? pageNumber, int? pageSize)
        
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["PageSize"] = pageSize;


            var cats = new List<Category>();
            var categories = _categoryRepository.GetAll().Include(i => i.Image).ToList();
                
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

                foreach (var cat in categories)
                {
                    var _cat = _categoryRepository.Find(c => c.Id == cat.Id

                        && (c.Name.Contains(searchString) || c.NameArabic.Contains(searchString)
                       || c.Parent.Name.Contains(searchString) || c.Parent.NameArabic.Contains(searchString)));

                    if (_cat != null)
                        cats.Add(_cat);
                }
            } // B- No search => Get All
            else
            {
                cats = _categoryRepository.GetAll().Include(i => i.Image).ToList();
              
            }

            if (categories.Any())
            {
                // send role, users count, and the current page number to the view page
                // we will need to get them later in suspend and activate actions below

                ViewBag.Count = categories.Count();
                ViewBag.Page = pageNumber;

                if (searchString != null)
                {
                    ViewBag.Search = searchString;
                }

                // Sepcifiy number of users you want to display in one page
                int rowsPerPage = pageSize ?? 10;
                ViewBag.rowsPerPage = rowsPerPage;


                //return View(users);
                return View(EFModel.Models.PaginatedList<Category>.CreateAsync(cats, pageNumber ?? 1, rowsPerPage));
            }
            else
            {
                // else no users are found
                return NotFound();
            }
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
                        var imgsave = Path.Combine(iweb.WebRootPath, "ImagesGallery" );
                        string filepath = Path.Combine(imgsave, (files.FileName));
                        var straem = new FileStream(filepath, FileMode.Create);
                        files.CopyTo(straem);

                        Image img = new Image()
                        { ImageName =files.FileName,
                            CategoryId = cat.Id
                                                                 
                        };

                        _imageRepository.Add(img);
                        _unitOfWork.Save();

                    }
                    return RedirectToAction("Index");

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
                                        
                        var imgsave = Path.Combine(iweb.WebRootPath, "ImagesGallery", (files.FileName));
                        string filepath = Path.Combine(imgsave, (files.FileName));
                        var straem = new FileStream(filepath, FileMode.Create);
                        files.CopyTo(straem);

                        Image img = _imageRepository.GetAll().Where(i => i.CategoryId== cat.Id).FirstOrDefault();
                        deleteFilefromRoot(img.ImageName);
                        img.ImageName = files.FileName;

                        _imageRepository.Update(img);
                        _unitOfWork.Save();
                      


                    }
                    return RedirectToAction("Index");
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

           
            return RedirectToAction("Index");
        }

        private void deleteFilefromRoot(string  img)
        {
            img = Path.Combine(iweb.WebRootPath, "ImagesGallery", img);
            FileInfo fileinfo = new FileInfo(img);
            if (fileinfo != null)
            {
                System.IO.File.SetAttributes(img, FileAttributes.Normal);
                System.IO.File.Delete(img);
            }
        }
    }
}
