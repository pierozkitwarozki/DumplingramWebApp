using System.Threading.Tasks;
using Dumplingram.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Dumplingram.API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Dumplingram.API.SignalR;
using Dumplingram.API.Services;
namespace Dumplingram.API
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
            services.AddDbContext<DataContxt>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddControllers();
            services.AddCors();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(AuthRepository).Assembly);
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IConnectionGroupRepository, ConnectionGroupRepository>();
           
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    options.Events = new JwtBearerEvents 
                    {
                        // Signal R Authorization
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];
                            var paths = context.HttpContext.Request.Path;
                            if(!string.IsNullOrEmpty(accessToken) && paths.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyMethod().AllowCredentials().AllowAnyHeader().WithOrigins("http://localhost:4200"));

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
            });
        }
    }
}
