using System.Text;
using BL.Helper;
using EFModel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.CustomRepository.AuthRepo;
using Repository.GenericRepository;
using Repository.UnitWork;
using SqlServerDBContext;

namespace JWTAuth
{
    public class Startup
    {
        #region Inject Configuration

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Register Repository

            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>)); //<===VIP
            services.AddScoped<IAuthRepo, AuthRepo>(); //<===VIP

            #endregion

            #region Register Controllers

            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTAuth", Version = "v1" }); });

            #endregion

            #region Register DB EF codeFirst Context

            services.AddDbContext<SqlContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            #endregion

            #region Register IdentityUser to my DB Context

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<SqlContext>()
                .AddDefaultTokenProviders();

            #endregion

            #region Binding Jwt Configuration in appsetting.json to Jwt.cs class

            services.Configure<Jwt>(Configuration.GetSection("JWT"));

            #endregion

            #region Register JWT Options for Review Token Signature

            services.AddAuthentication(defaultOptions =>
            {
                defaultOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                defaultOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                defaultOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(extraOptions =>
            {
                extraOptions.RequireHttpsMetadata = false;
                extraOptions.SaveToken = true;
                extraOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                };
            });

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTAuth v1"));
            }

            #region Allow Accessing Form Local Angular App

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            #endregion

            app.UseRouting();
            app.UseAuthentication(); //1
            app.UseAuthorization(); //2

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

    }
}