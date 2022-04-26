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
        public async Task<ActionResult> Index(string role)
        {
            // Get all users including his phone and address

            #region Another Solution

            //var users = new List<User>();
            //switch (role)
            //{
            //    case AuthorizeRoles.Admin:
            //    { users = _userRepository.GetAll().Where(u => u.Admin != null).Include(u => u.Addresses).ToList(); }
            //    break;
            //    case AuthorizeRoles.Customer:
            //    { users = _userRepository.GetAll().Where(u => u.Customer != null).Include(u => u.Addresses).ToList(); }
            //    break;
            //    case AuthorizeRoles.Seller:
            //    { users = _userRepository.GetAll().Where(u => u.Seller != null).Include(u => u.Addresses).ToList(); }
            //    break;
            //    case AuthorizeRoles.Shipper:
            //    { users = _userRepository.GetAll().Where(u => u.Shipper != null).Include(u => u.Addresses).ToList(); }
            //    break;
            //}
            //if (users.Any())
            //{
            //    return View(users);
            //}

            //return NotFound();

            #endregion

            #region Used Solution

            // Initializing a List of Users
            var users = new List<User>();

            // Get Users by role
            var data = await _userManager.GetUsersInRoleAsync(role);

            // Loop in users including their addresses
            foreach (var user in data)
            {
                var _user = _userRepository.Find(u => u.Id == user.Id, u => u.Addresses);
                users.Add(_user);
            }

            // Check if their users
            if (users.Any())
            {
                return View(users);
            }

            return NotFound();

            #endregion

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
                    };
                    await _userManager.SetEmailAsync(user, model.Email);
                    await _userManager.AddPasswordAsync(user, model.Password);
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Add to users table
                    _userRepository.Add(user);

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

                    // update basic info
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Balance = model.Balance;
                    user.IsActive = model.IsActive;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Addresses.FirstOrDefault()!.City = model.City;
                    user.Addresses.FirstOrDefault()!.Street = model.Street;

                    // Get current Role
                    var role = await _userManager.GetRolesAsync(user);

                    // Remove Current Role
                    await _userManager.RemoveFromRoleAsync(user, role.FirstOrDefault());

                    //Add new Role
                    await _userManager.AddToRoleAsync(user, model.Role);

                    _unitOfWork.Save();
                }
            }

            return RedirectToAction("Index", new {role = model.Role});
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