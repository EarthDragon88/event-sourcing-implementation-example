using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Domain.Events.InventoryItem;
using Demo.Infrastructure;
using Demo.WebClient.DbModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Serialization.Json;
using Rebus.ServiceProvider;

namespace Demo.WebClient
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
            services.AutoRegisterHandlersFromAssemblyOf<Startup>();

            services.AddRebus(configure => configure
                .Transport(t => t.UseRabbitMq("amqp://localhost:5672", "client"))
                .Serialization(s => s.UseNewtonsoftJson(JsonInfrastructure.JsonSettings)));


            services.AddDbContext<InventoryContext>(options =>
                options.UseSqlServer("Server=localhost;Database=InventoryContext;Trusted_Connection=True;"));

            

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<InventoryContext>();
                context.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.ApplicationServices.UseRebus(async bus =>
            {
                await bus.Subscribe<InventoryItemCreated>();
                await bus.Subscribe<InventoryItemRenamed>();
                await bus.Subscribe<ItemsCheckedIntoInventory>();
                await bus.Subscribe<ItemsRemovedFromInventory>();
                await bus.Subscribe<InventoryItemDeactivated>();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
