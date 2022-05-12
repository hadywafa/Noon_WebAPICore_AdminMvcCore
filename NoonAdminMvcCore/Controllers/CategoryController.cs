using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EFModel.Models.EFModels;
using System.IO;
using NoonAdminMvcCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Repository.UnitWork;
using Repository.GenericRepository;
using EFModel.Enums;
using EFModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace NoonAdminMvcCore.Controllers
{
    //[Authorize(Roles = AuthorizeRoles.Admin)]
    public class CategoryController : Controller
    {
        #region intilaztion repo

        private readonly IUnitOfWork _unitOfWork;
        readonly IGenericRepo<Image> _imageRepository;
        readonly IGenericRepo<Category> _categoryRepository;
        private readonly IConfiguration Configuration;


        #endregion

        private readonly IWebHostEnvironment iweb;

        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment _iwab , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            iweb = _iwab;
            _imageRepository = _unitOfWork.Images;
            _categoryRepository = _unitOfWork.Categories;
            Configuration = configuration;

        }

        // GET: Category
        public async Task<IActionResult> Display(string currentFilter, string searchString, int? pageNumber,
            int? pageSize)
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
                    var _cat = await _categoryRepository.Find(c =>
                        c.Id == cat.Id && (c.Name.Contains(searchString) || c.NameArabic.Contains(searchString) ||
                                           c.Parent.Name.Contains(searchString) ||
                                           c.Parent.NameArabic.Contains(searchString)));
                    if (_cat != null) cats.Add(_cat);
                }
            } // B- No search => Get All
            else
            {
                cats = await _categoryRepository.GetAll().Include(i => i.Image).ToListAsync();
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
                return View("Index",
                    EFModel.Models.PaginatedList<Category>.CreateAsync(cats, pageNumber ?? 1, rowsPerPage));
            }
            else
            {
                // else no users are found
                return NotFound();
            }
        }

        // GET: Category/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Category = new SelectList(await _categoryRepository.GetAll().ToListAsync(), "Id", "Name");;
            return View("CategoryForm");
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryVM, [FromForm] IFormFile files)
        {
            if (ModelState.IsValid)
            {
                if (categoryVM.Id == null)
                {
                    files = categoryVM.Image;
                    if (files != null)
                    {
                        Category cat = new Category()
                        {
                            Name = categoryVM.Name,
                            NameArabic = categoryVM.NameArabic,
                            Description = categoryVM.Description,
                            DescriptionArabic = categoryVM.DescriptionArabic,
                            ParentID = categoryVM.ParentId,
                            Code = categoryVM.Code,
                            IsTop = categoryVM.IsTop
                        };
                        await _categoryRepository.Add(cat);
                        await _unitOfWork.Save();
                        //
                        //var stream = new FileStream(filepath, FileMode.Create);
                        //var filepath = Configuration["imagesPath"] + "/images/" + files.FileName;
                        var imgSave = Path.Combine(iweb.WebRootPath, "Images");
                        string filePath = Path.Combine(imgSave, (files.FileName));
                        var stream = new FileStream(filePath, FileMode.Create);
                        await files.CopyToAsync(stream);
                        //
                        Image img = new Image() { ImageName = files.FileName, CategoryId = cat.Id };
                        await _imageRepository.Add(img);
                        await _unitOfWork.Save();
                    }

                    return RedirectToAction("Display");
                }
                else
                {
                    int targtedId = int.Parse(categoryVM.Id);
                    var cat = await _categoryRepository.GetById(targtedId);
                    if (cat == null)
                    {
                        return NotFound();
                    }

                    cat.Name = categoryVM.Name;
                    cat.NameArabic = categoryVM.NameArabic;
                    cat.Description = categoryVM.Description;
                    cat.DescriptionArabic = categoryVM.DescriptionArabic;
                    cat.Code = categoryVM.Code;
                    cat.IsTop = categoryVM.IsTop;
                    cat.ParentID = categoryVM.ParentId;
                    await _categoryRepository.Update(cat);
                    await _unitOfWork.Save();

                    files = categoryVM.Image;

                    if (files != null)
                    {
                        //var stream = new FileStream(filepath, FileMode.Create);
                        //var filepath = Configuration["imagesPath"] + "/images/" + files.FileName;
                        var imgSave = Path.Combine(iweb.WebRootPath, "Images");
                        string filePath = Path.Combine(imgSave, (files.FileName));
                        var stream = new FileStream(filePath, FileMode.Create);
                        await files.CopyToAsync(stream);
                        Image img = await _imageRepository.GetAll().Where(i => i.CategoryId == cat.Id).FirstOrDefaultAsync();

                        if (img != null)
                        {
                            deleteFilefromRoot(img.ImageName);
                            img.ImageName = files.FileName;
                            await _imageRepository.Update(img);
                        }
                        else
                        {
                            var newImg = new Image()
                            {
                                ImageName = files?.FileName,
                                CategoryId = cat.Id,
                            };

                           await _imageRepository.Add(newImg);
                        }
  
                        await _unitOfWork.Save();
                    }

                    return RedirectToAction("Display");
                }
            }

            return RedirectToAction();
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            #region fetch data for Updating

            var categoryVM = await _categoryRepository.Find(u => u.Id == id);
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
                ParentId = categoryVM.ParentID,
                Code = categoryVM.Code,
                IsTop = categoryVM.IsTop
            };
            ViewBag.Category = new SelectList(await _categoryRepository.GetAll().ToListAsync(), "Id", "Name");

            #endregion

            return View("CategoryForm", catViewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var cat = await _categoryRepository.GetById(id);
            var img = await _imageRepository.GetAll().Where(i => i.CategoryId == id).FirstOrDefaultAsync();
            await _categoryRepository.Remove(cat);
            await _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        private void deleteFilefromRoot(string img)
        {
            img = Path.Combine(iweb.WebRootPath, "Images", img);
            FileInfo fileInfo = new FileInfo(img);
            if (fileInfo != null)
            {
                System.IO.File.SetAttributes(img, FileAttributes.Normal);
                System.IO.File.Delete(img);
            }
        }
    }
}