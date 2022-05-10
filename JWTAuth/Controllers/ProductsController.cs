using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BL.ViewModels.RequestVModels;
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

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _productRepo = _unitOfWork.Products;
        }

        #endregion

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var productList = _productRepo.GetAll().Include(x=>x.ImagesGallery).Include(x => x.Brand).Include(x => x.Category)
                .Include(x => x.Seller).Include(x => x.Seller.User).Include(x => x.ProductHighlights)
                .Include(x => x.Specifications).Include(x => x.Orders).AsNoTracking();

            var vmProductsList = _mapper.Map<IEnumerable<Product>, IEnumerable<VmProduct>>(productList);

            return Ok(vmProductsList);
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById( [FromQuery] int id)
        {
            var Product = await _productRepo.Find(x => x.Id == id, x => x.ImagesGallery, x => x.Brand, x => x.Category,
                x => x.Seller, x => x.Seller.User, x => x.ProductHighlights, x => x.Specifications, x => x.Orders);
            var vmProduct = _mapper.Map<Product,VmProduct>(Product);

            return Ok(vmProduct);
        }



        [HttpGet("GetAllCategoriesJson")]
        public async Task<IActionResult> GetAllCategoriesJson()
        {
            //var connStr = _configuration["ConnectionStrings:DefaultConnection"];
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            CreateSqlFunction(connStr);
            CreateSqlSp(connStr);
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

        
        [HttpGet("GetCategoryById")]
        public IActionResult GetCategoryById()
        {
            var productList = _productRepo.GetAll().Include(x=>x.ImagesGallery).Include(x => x.Brand).Include(x => x.Category)
                .Include(x => x.Seller).Include(x => x.Seller.User).Include(x => x.ProductHighlights)
                .Include(x => x.Specifications).Include(x => x.Orders).AsNoTracking();

            var vmProductsList = _mapper.Map<IEnumerable<Product>, IEnumerable<VmProduct>>(productList);

            return Ok(vmProductsList);
        }

        //[HttpGet("GetCategoryPath")]
        //public async Task<IActionResult> GetCategoryPath()
        //{
 


        //    //return Ok(vmProductsList);
        //}

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

        private async void CreateSqlFunction(string connectionString)
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
                            children = JSON_QUERY(dbo.GetJson([Id]))
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

        private async void CreateSqlSp(string connectionString)
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



    }
}
