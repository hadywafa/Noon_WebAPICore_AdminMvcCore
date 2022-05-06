using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoonAdminMvcCore.Models;
using Repository.GenericRepository;
using Repository.UnitWork;
using Microsoft.AspNetCore.Authorization;
using System;
using EFModel.Enums;

namespace NoonAdminMvcCore.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin)]
    public class UserController : Controller
    {
        #region Inject Dependencies

        // Unit Of Work which is responsible on operations on Context
        private readonly IUnitOfWork _unitOfWork;

        // User Repo which is responsible on operations on user
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly IGenericRepo<User> _userRepository;
        readonly IGenericRepo<Address> _addressRepository;
        readonly IGenericRepo<Admin> _adminRepo;
        readonly IGenericRepo<Customer> _customerRepo;
        readonly IGenericRepo<Seller> _sellerRepo;
        readonly IGenericRepo<Shipper> _shipperRepo;

        // Constructor
        public UserController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _userRepository = _unitOfWork.Users;
            _addressRepository = _unitOfWork.Addresses;
            _adminRepo = _unitOfWork.Admins;
            _customerRepo = _unitOfWork.Customers;
            _sellerRepo = _unitOfWork.Sellers;
            _shipperRepo = _unitOfWork.Shippers;
        }

        #endregion

        // GET: Customers
        public async Task<ActionResult> Index(string role, string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            // To maintain the searched keyword and show it again in the view
            ViewData["CurrentFilter"] = searchString;
            ViewData["PageSize"] = pageSize;

            // Get all users including his phone and address
            // 1- Initializing a List of Users
            var users = new List<User>();

            // 2- Get Users by role
            var data = await _userManager.GetUsersInRoleAsync(role);

            // A- Get user in case of search
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

                foreach (var user in data)
                {
                    var _user =
                        _userRepository.Find(u => u.Id == user.Id

                        && (u.FirstName.Contains(searchString)
                        || u.LastName.Contains(searchString)
                        || u.Email.Contains(searchString)), u => u.Addresses);

                    if (_user != null)
                        users.Add(_user);
                }
            } // B- No search => Get All
            else
            {
                // Loop in users including their addresses
                foreach (var user in data)
                {
                    var _user = _userRepository.Find(u => u.Id == user.Id, u => u.Addresses);
                    users.Add(_user);
                }
            }

            // 3- Check if there are users
            if (users.Any())
            {
                // send role, users count, and the current page number to the view page
                // we will need to get them later in suspend and activate actions below
                ViewBag.Role = role;
                ViewBag.Count = users.Count();
                ViewBag.Page = pageNumber;

                if(searchString != null)
                {
                    ViewBag.Search = searchString;
                }

                // Sepcifiy number of users you want to display in one page
                int rowsPerPage = pageSize ?? 10;
                ViewBag.rowsPerPage = rowsPerPage;


                //return View(users);
                return View(EFModel.Models.PaginatedList<User>.CreateAsync(users, pageNumber ?? 1, rowsPerPage));
            }
            else
            {
                // else no users are found
                return NotFound();
            }
        }

        // GET: User/Create
        public ActionResult Create(string role = AuthorizeRoles.Customer)
        {
            var userViewModel = new UserViewModel { IsActive = true, Role = role };
            return View("UserForm", userViewModel);
        }

        // POST: Customers/Save (Responsible on creating and Post updates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // means you create new user not updating
                if (model.Id == null)
                {
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Balance = model.Balance,
                        IsActive = model.IsActive,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        UserName = model.Email
                    };

                    // Add user and save Context
                    await _userManager.CreateAsync(user, model.Password);

                    // Add To UserRoles table
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Add the new user to his role table
                    switch (model.Role)
                    {
                        case AuthorizeRoles.Admin:
                            _adminRepo.Add(new Admin() { User = user });
                            break;
                        case AuthorizeRoles.Customer:
                            _customerRepo.Add(new Customer() { User = user });
                            break;
                        case AuthorizeRoles.Seller:
                            _sellerRepo.Add(new Seller() { User = user });
                            break;
                        case AuthorizeRoles.Shipper:
                            _shipperRepo.Add(new Shipper() { User = user });
                            break;
                    }

                    // Save
                    _unitOfWork.Save();

                    // Initialize new Address
                    var address = new Address
                    {
                        User = user,
                        Street = model.Street,
                        City = model.City,
                        PostalCode = model.PostalCode
                    };

                    // Add to the Address table
                    _addressRepository.Add(address);
                    _unitOfWork.Save();

                    return RedirectToAction("Index", new { role = model.Role });

                }
                else // updating
                {
                    var user = _userManager.Users.FirstOrDefault(u => u.Id == model.Id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    var gotUser = _userRepository.Find(u => u.Id == user.Id, u => u.Addresses);

                    // update basic info
                    gotUser.FirstName = model.FirstName;
                    gotUser.LastName = model.LastName;
                    gotUser.Balance = model.Balance;
                    gotUser.IsActive = model.IsActive;
                    gotUser.Email = model.Email;
                    gotUser.PhoneNumber = model.PhoneNumber;
                    gotUser.Addresses.FirstOrDefault()!.City = model.City;
                    gotUser.Addresses.FirstOrDefault()!.Street = model.Street;

                    // Get current Role
                    var role = _userManager.GetRolesAsync(gotUser).Result.FirstOrDefault();

                    // Remove Current Role
                    await _userManager.RemoveFromRoleAsync(gotUser, role);

                    //Add new Role
                    await _userManager.AddToRoleAsync(gotUser, model.Role);

                    // Add the new user to his role table
                    switch (model.Role)
                    {
                        case "Admin":
                            _adminRepo.Add(new Admin() { User = user });
                            break;
                        case "Customer":
                            _customerRepo.Add(new Customer() { User = user });
                            break;
                        case "Seller":
                            _sellerRepo.Add(new Seller() { User = user });
                            break;
                        case "Shipper":
                            _shipperRepo.Add(new Shipper() { User = user });
                            break;
                    }

                    // Remove the previous role table
                    switch (role)
                    {
                        case "Admin":
                            _adminRepo.RemoveById(user.Id);
                            break;
                        case "Customer":
                            _customerRepo.RemoveById(user.Id);
                            break;
                        case "Seller":
                            _sellerRepo.RemoveById(user.Id);
                            break;
                        case "Shipper":
                            _shipperRepo.RemoveById(user.Id);
                            break;
                    }

                    _unitOfWork.Save();
                }
            }

            return RedirectToAction("Index", new { role = model.Role });
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var user = _userRepository.Find(u => u.Id == id, u => u.Addresses);

            if (user == null)
            {
                return NotFound();
            }

            var role = await _userManager.GetRolesAsync(user);

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = "",
                Balance = user.Balance,
                Role = role.FirstOrDefault(),
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber,
                Street = user.Addresses.FirstOrDefault()?.Street,
                City = user.Addresses.FirstOrDefault()?.City,
            };

            return View("UserForm", userViewModel);
        }

        public ActionResult Suspend(string id, string role, string currentFilter, int? pageNumber)
        {
            // get the user
            var user = _userRepository.GetById(id);

            //suspend the user
            user.IsActive = false;

            //update database
            _userRepository.Update(user);

            // save updates
            _unitOfWork.Save();

            return RedirectToAction("Index", new { role = role, currentFilter = currentFilter, pageNumber = pageNumber });
        }

        public ActionResult Activate (string id, string role, string currentFilter, int? pageNumber)
        {
            // get the user
            var user = _userRepository.GetById(id);

            // suspend the user
            user.IsActive = true;

            // update database
            _userRepository.Update(user);

            // save updates
            _unitOfWork.Save();

            return RedirectToAction("Index", new { role = role, currentFilter = currentFilter, pageNumber = pageNumber });
        }
    }
}