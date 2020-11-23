using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Models;
using PaymentGateway.Services;
using PaymentGateway.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PaymentGateway
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
            services.AddCors();
            services.AddDbContext<PaymentGatewayContext>(opt => opt.UseInMemoryDatabase("PaymentGateway"));
            services.AddControllers();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankService, BankService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway", Version = "v1" });
            });
            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, PaymentsAuthenticationHandler>("BasicAuthentication", null);

            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddFile("PaymentGateway.log", append: true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI( c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway v1"); 
                    c.RoutePrefix = "swagger/ui"; 
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
