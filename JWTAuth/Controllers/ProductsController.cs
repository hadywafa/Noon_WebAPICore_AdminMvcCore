using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFModel.Enums;
using Microsoft.AspNetCore.Mvc;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
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
                    var json  = JsonConvert.DeserializeObject(jsonResult.ToString());

                    return json.ToString();

                }

            }
        }
    }
}
