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

            //���HttpReports
            services.AddHttpReports().AddHttpTransport();

            #region json���л�����
            services.AddControllers().AddNewtonsoftJson(options=> {
                
                //�޸��������Ƶ����л���ʽ������ĸСд
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                //�޸�ʱ������л���ʽ
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            });
            #endregion

            #region JWT Token

            services.Configure<TokenManagement>(Configuration.GetSection("tokenConfig"));

            var token = Configuration.GetSection("tokenConfig").Get<TokenManagement>();

            services.AddAuthentication(x =>
            {
                //��֤middleware����
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                //��ȡ������Ԫ���ݵ�ַ��Ȩ���Ƿ���ҪHTTPS��Ĭ��ֵΪtrue��
                x.RequireHttpsMetadata = false;
                //�����ڳɹ���Ȩ���Ƿ�Ӧ�ý��������ƴ洢�� AuthenticationProperties�С�
                x.SaveToken = true;
                //��ȡ������������֤������ƵĲ�����
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    //��ȡ������һ������ֵ���ò���ֵ�����Ƿ���ö�ǩ��securityToken ��SecurityKey������֤��
                    ValidateIssuerSigningKey = true,
                    //��ȡ�����ý�����ǩ����֤��SecurityKey��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),

                    //��ȡ������һ������ֵ���Կ����Ƿ���������֤�ڼ���֤�����ߡ�
                    ValidateIssuer = false,
                    //��ȡ������һ���ַ��������ַ�����ʾһ����Ч�ķ����ߣ��÷����߽����ڼ�����Ƶķ����ߡ�
                    ValidIssuer = token.Issuer,

                    //��ȡ������һ������ֵ���Կ����Ƿ���������֤�ڼ���֤���ڡ�
                    ValidateAudience = false,
                    //��ȡ������һ���ַ��������ַ�����ʾ��Ч������Ⱥ�壬���ַ��������ڼ�����Ƶ�����Ⱥ�塣
                    ValidAudience = token.Audience,
                };
            });
            services.AddScoped<ITokenAuthenticationService, TokenAuthenticationService>();
            services.AddScoped<IUserService, UserService>();

            #endregion

            #region Swagger����
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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
            //ʹ��HttpReports
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
