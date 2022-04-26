using System.Collections.Generic;
using System.Linq;
using BL.AppPolicy;
using Microsoft.AspNetCore.Mvc;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Repository.GenericRepository;
using Repository.UnitWork;

namespace JWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        #region Inject Product Repository in Author Controller

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Product> _productRepo;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productRepo = _unitOfWork.Products;
        }

        #endregion

        [Authorize(Roles = AuthorizeRoles.Customer)]
        [HttpGet("GetAll")]
        public List<Product> GetAll()
        {
            return _productRepo.GetAll().ToList();
        }

        [HttpPost]
        public Product Post(Product book)
        {
            _productRepo.Add(book);
            _unitOfWork.Save();
            return book;
        }


    }
}
