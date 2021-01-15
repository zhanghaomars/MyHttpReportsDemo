using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text;
using WebAPI2.APIToken;

namespace WebAPI2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //添加HttpReports
            services.AddHttpReports().AddHttpTransport();

            #region json序列化设置
            services.AddControllers().AddNewtonsoftJson(options=> {
                
                //修改属性名称的序列化方式，首字母小写
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                //修改时间的序列化方式
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            });
            #endregion

            #region JWT Token

            services.Configure<TokenManagement>(Configuration.GetSection("tokenConfig"));

            var token = Configuration.GetSection("tokenConfig").Get<TokenManagement>();

            services.AddAuthentication(x =>
            {
                //认证middleware配置
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                //获取或设置元数据地址或权限是否需要HTTPS。默认值为true。
                x.RequireHttpsMetadata = false;
                //定义在成功授权后是否应该将承载令牌存储在 AuthenticationProperties中。
                x.SaveToken = true;
                //获取或设置用于验证身份令牌的参数。
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    //获取或设置一个布尔值，该布尔值控制是否调用对签名securityToken 的SecurityKey进行验证。
                    ValidateIssuerSigningKey = true,
                    //获取或设置将用于签名验证的SecurityKey。
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),

                    //获取或设置一个布尔值，以控制是否在令牌验证期间验证发行者。
                    ValidateIssuer = false,
                    //获取或设置一个字符串，该字符串表示一个有效的发行者，该发行者将用于检查令牌的发行者。
                    ValidIssuer = token.Issuer,

                    //获取或设置一个布尔值，以控制是否在令牌验证期间验证受众。
                    ValidateAudience = false,
                    //获取或设置一个字符串，该字符串表示有效的受众群体，该字符串将用于检查令牌的受众群体。
                    ValidAudience = token.Audience,
                };
            });
            services.AddScoped<ITokenAuthenticationService, TokenAuthenticationService>();
            services.AddScoped<IUserService, UserService>();

            #endregion

            #region Swagger配置
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme{
                                Reference = new OpenApiReference {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"}
                           },new string[] { }
                        }
                    });
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //使用HttpReports
            app.UseHttpReports();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
