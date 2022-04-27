using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using BL.AppPolicy;
using EFModel.Models;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoonAdminMvcCore.Models;
using Repository.GenericRepository;
using Repository.UnitWork;
using Microsoft.AspNetCore.Authorization;
using System;

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
        public async Task<ActionResult> Index(string role, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentRole"] = role;

            // Get all users including his phone and address

            // Initializing a List of Users
            var users = new List<User>();

            // Get Users by role
            var data = await _userManager.GetUsersInRoleAsync(role);

            // Get Searched user if existed
            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var user in data)
                {
                    var _user =
                        _userRepository.Find(u => u.Id == user.Id

                        && (u.FirstName.Contains(searchString)
                        || u.LastName.Contains(searchString)
                        || u.Email.Contains(searchString)), 

                    u => u.Addresses);

                    if( _user != null )
                        users.Add(_user);
                }

            }
            else
            {
                // Loop in users including their addresses
                foreach (var user in data)
                {
                    var _user = _userRepository.Find(u => u.Id == user.Id, u => u.Addresses);
                    users.Add(_user);
                }
            }

            // Check if their users
            if (users.Any())
            {
                return View(users);
            }

            return NotFound();



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

                    // Add to the role's table of new user
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

                // updating
                else
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
                    var role = await _userManager.GetRolesAsync(gotUser);

                    // Remove Current Role
                    await _userManager.RemoveFromRoleAsync(gotUser, role.FirstOrDefault());

                    //Add new Role
                    await _userManager.AddToRoleAsync(gotUser, model.Role);

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

        public async Task<ActionResult> SuspendAsync(string id)
        {
            // get the user
            var user = _userRepository.GetById(id);

            //suspend the user
            user.IsActive = false;

            //update database
            _userRepository.Update(user);

            // save updates
            _unitOfWork.Save();

            // Get current Role
            var role = await _userManager.GetRolesAsync(user);

            // Initializing a List of Users
            var users = new List<User>();

            // Get Users by role
            var data = await _userManager.GetUsersInRoleAsync(role.FirstOrDefault());

            // Loop in users including their addresses
            foreach (var _u in data)
            {
                var _user = _userRepository.Find(u => u.Id == _u.Id, u => u.Addresses);
                users.Add(_user);
            }

            return PartialView("_UserPartial", users);
        }

        public async Task<ActionResult> ActivateAsync(string id)
        {
            // get the user
            var user = _userRepository.GetById(id);

            // suspend the user
            user.IsActive = true;

            // update database
            _userRepository.Update(user);

            // save updates
            _unitOfWork.Save();

            // Get current Role
            var role = await _userManager.GetRolesAsync(user);

            // Initializing a List of Users
            var users = new List<User>();

            // Get Users by role
            var data = await _userManager.GetUsersInRoleAsync(role.FirstOrDefault());

            // Loop in users including their addresses
            foreach (var _u in data)
            {
                var _user = _userRepository.Find(u => u.Id == _u.Id, u => u.Addresses);
                users.Add(_user);
            }

            return PartialView("_UserPartial", users);
        }
    }
}