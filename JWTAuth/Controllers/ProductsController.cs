using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BL.ViewModels.RequestVModels;
using EFModel.Enums;
using Microsoft.AspNetCore.Mvc;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly IMapper _mapper;
        private readonly IGenericRepo<Product> _productRepo;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productRepo = _unitOfWork.Products;
        }

        #endregion

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var productList = _productRepo.GetAll().Include(x=>x.ImagesGallery).Include(x => x.Brand).Include(x => x.Category)
                .Include(x => x.Seller).Include(x => x.Seller.User).Include(x => x.ProductHighlights)
                .Include(x => x.Specifications).Include(x => x.Orders).AsNoTracking().ToList();

            var vmProductsList = _mapper.Map<IEnumerable<Product>, IEnumerable<VmProduct>>(productList);

            return Ok(vmProductsList);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById( int id)
        {
            var Product = _productRepo.Find(x => x.Id == id, x => x.ImagesGallery, x => x.Brand, x => x.Category,
                x => x.Seller, x => x.Seller.User, x => x.ProductHighlights, x => x.Specifications, x => x.Orders);
            var vmProduct = _mapper.Map<Product,VmProduct>(Product);

            return Ok(vmProduct);
        }

        [HttpPost]
        public Product Post(Product book)
        {
            _productRepo.Add(book);
            _unitOfWork.Save();
            return book;
        }

        [HttpGet("GetAllCategories")]
        public string GetAllCategories()
        {
            var queryWithForJson = "exec [dbo].[spGetJson]";
            using (var conn = new SqlConnection("Data Source=.;Initial Catalog=Noon_API_MVC;Integrated Security=True"))
            {
                using (var cmd = new SqlCommand(queryWithForJson, conn))
                {
                    conn.Open();
                    var jsonResult = new StringBuilder();
                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        jsonResult.Append("[]");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            jsonResult.Append(reader.GetValue(0).ToString());
                        }
                    }
                    var json = JsonConvert.DeserializeObject(jsonResult.ToString());

                    return json.ToString();

                }

            }
        }

    }
}
