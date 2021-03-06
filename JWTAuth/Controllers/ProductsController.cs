using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.Helpers;
using BL.ViewModels.RequestVModels;
using BL.ViewModels.ResponseVModels;
using EFModel.Enums;
using Microsoft.AspNetCore.Mvc;
using EFModel.Models.EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<Category> _categoryRepo;
        private readonly IGenericRepo<CustProSellReviews> _custProSellReviewsRepo;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _productRepo = _unitOfWork.Products;
            _categoryRepo = _unitOfWork.Categories;
            _custProSellReviewsRepo = _unitOfWork.Reviews;
        }

        #endregion

        #region Product End Points

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var productList =  _productRepo.GetAll().Include(x=>x.ImagesGallery).Include(x => x.Brand).Include(x => x.Category)
                .Include(x => x.Seller).Include(x => x.Seller.User).Include(x => x.ProductHighlights)
                .Include(x => x.Specifications).Include(x => x.Orders).Where(p => p.Quantity > 0);

            var vmProductsList = _mapper.Map<List<Product>, List<VmProduct>>(await productList.ToListAsync());

            return Ok(vmProductsList);
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var product = await _productRepo.Find(x => x.Id == id, x => x.ImagesGallery, x => x.Brand, x => x.Category,
                x => x.Seller, x => x.Seller.User, x => x.ProductHighlights, x => x.Specifications, x => x.Orders);
            var vmProduct = _mapper.Map<Product,VmProduct>(product);

            return Ok(vmProduct);
        }

        //Don't use this End point because it's hell 
        [HttpGet("GetProductsByCatCode/{catCode}")]
        public async Task<IActionResult> GetProductsByCatCode([FromRoute] string catCode)
        {
            int catId = await _categoryRepo.GetAll().Where(x => x.Code == catCode).Select(x => x.Id).FirstOrDefaultAsync();



            var proAll = await _productRepo.GetAll()
                .Include(x => x.ImagesGallery).Include(x => x.Brand).Include(x => x.Category)
                .Include(x => x.Seller).Include(x => x.Seller.User).Include(x => x.ProductHighlights)
                .Include(x => x.Specifications).Include(x => x.Orders).ToListAsync();

            List<Product> productLists = new List<Product>();
            foreach (Product pro in proAll)
            {
                var pathStr = await GetCategoryPathStr(pro.Category.Id);
                var pathArr = pathStr.Split(',').ToList();
                foreach (var str in pathArr)
                {
                    if (str == catCode)
                    {
                        productLists.Add(pro);
                    }
                }
            }

            var vmProductsList = _mapper.Map<IEnumerable<Product>, IEnumerable<VmProduct>>( productLists);

            return Ok(vmProductsList);
        }

        #endregion

        #region Reviews Endpoints

        [HttpGet("GetAllProductReviews/{id}")]
        public async Task<IActionResult> GetAllProductReviews([FromRoute] int id)
        {
            var reviews = await _custProSellReviewsRepo.FindAll(x => x.Product.Id == id, c=>c.Customer.User , x=>x.Seller.User).ToListAsync();
            var vmReviews = _mapper.Map<List<CustProSellReviews>,List<VmReview>>(reviews);

            return Ok(vmReviews);
        }

        #endregion

        #region Category End Points

        [HttpGet("GetTreeCategories")]
        public async Task<IActionResult> GetTreeCategories()
        {
            //var connStr = _configuration["ConnectionStrings:DefaultConnection"];
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            await CreateSqlFunction(connStr);
            await CreateSqlSp(connStr);
            var queryWithForJson = "exec [dbo].[spGetJson]";
            await using (var conn = new SqlConnection(connStr))
            {
                await using (var cmd = new SqlCommand(queryWithForJson, conn))
                {
                    conn.Open();
                    var jsonResult = new StringBuilder();
                    var reader = await cmd.ExecuteReaderAsync();
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

                    return StatusCode(200,json);
                }
            }
        }


        [HttpGet("GetCategoryPath")]
        public async Task<IActionResult> GetCategoryPath([FromQuery] int parentCatId)
        {
            var pathStr = await GetCategoryPathStr(parentCatId);
            
            var pathArr = pathStr.Split(',');

            var categories = new List<Category>();
            foreach (var str in pathArr)
            {
                var cat = await _categoryRepo.Find(x => x.Code == str, x => x.Brands);
                categories.Add(cat);
            }

            var vmCategories = _mapper.Map<ICollection<Category>,ICollection< VmCategory>>(categories);

            return Ok(vmCategories);
        }

        //====Helpful Functions======//

        private async Task<string> GetCategoryPathStr(int parentId)
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            string catpath = "";
            string sql =
                $@"DECLARE @CatId AS int = ${parentId}
                ;WITH
                        Category_CTE([id], [name], [parentId], [level] , [url])
                    AS
                    (
                        SELECT c.Id, c.Name, c.ParentID, 0 , CAST(c.Code AS varchar(max))
                    from [dbo].[Categories] c
                        where c.ParentID is Null

                        UNION  ALL

                        SELECT c.Id, c.Name, c.ParentID , x.[level]+1 , CAST(x.[url]+','+c.Code  AS varchar(max))
                    from [dbo].[Categories] c , Category_CTE x
                    WHERE c.ParentID = x.[id]
                        )
                    SELECT url
                    FROM Category_CTE c
                    where [url]  NOT like '%,' AND c.id = @CatId ";
            await using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                //cmd.Parameters.Add("@Name", SqlDbType.VarChar);
                //cmd.Parameters["@name"].Value = newName;
                cmd.CommandType = CommandType.Text;
                try
                {
                    conn.Open();
                    catpath = (string) await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return catpath;
        }

        private async Task CreateSqlFunction(string connectionString)
        {
            await using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $@"CREATE OR ALTER FUNCTION dbo.GetJson (@parentID int)
                    RETURNS nvarchar(max)
                    AS BEGIN
                        RETURN (
                            SELECT
                            [Id] as id,
                            [Code] as code,
                            [Name] as name,
                            [NameArabic] as nameAr,
                            [ParentID] as parentId,
                            childrens = JSON_QUERY(dbo.GetJson([Id]))
                        FROM [dbo].[Categories]
                        WHERE EXISTS ( SELECT [ParentID]
                        INTERSECT
                            SELECT @parentID)
                        FOR JSON PATH
                        );
                    END;";
                command.CommandType = CommandType.Text;

                // Open the connection and execute the reader.
                connection.Open();
                await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0}: {1:C}", reader[0], reader[1]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
        }

        private async Task CreateSqlSp(string connectionString)
        {
            await using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create the command and set its properties.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = @"CREATE OR ALTER PROCEDURE [dbo].[spGetJson]
                AS
                    begin
                SELECT dbo.GetJson(NULL);
                end ";
                command.CommandType = CommandType.Text;
                //command.CommandType = CommandType.StoredProcedure;

                // Add the input parameter and set its properties.
                //SqlParameter parameter = new SqlParameter();
                //parameter.ParameterName = "@CategoryName";
                //parameter.SqlDbType = SqlDbType.NVarChar;
                //parameter.Direction = ParameterDirection.Input;
                //parameter.Value = categoryName;

                // Add the parameter to the Parameters collection.
                //command.Parameters.Add(parameter);

                // Open the connection and execute the reader.
                connection.Open();
                await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0}: {1:C}", reader[0], reader[1]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
        }

        //private async Task <bool> CheckPath(int catId,string catCode)
        //{
        //    var pathStr = await GetCategoryPathStr(catId);

        //    var pathArr = pathStr.Split(',').ToList();
        //    foreach (var str in pathArr)
        //    {
        //        if (str == catCode)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        #endregion

        
    }
}
