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
using evoting_backend_app.Models;
using Microsoft.Extensions.Options;
using evoting_backend_app.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace evoting_backend_app
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
            services.Configure<EVotingDatabaseSettings>(Configuration.GetSection(nameof(EVotingDatabaseSettings)));
            services.AddSingleton<IEVotingDatabaseSettings>(sp => sp.GetRequiredService<IOptions<EVotingDatabaseSettings>>().Value);

            services.AddSingleton<VotersService>();
            services.AddSingleton<VotingsService>();
            services.AddSingleton<RegistrationRequestsService>();
            services.AddSingleton<CoordinatorsService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<EnumSchemaFilter>(); // Display enum variables of model as strings instead of integers
                c.EnableAnnotations(); // Enable endpoints annotations
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-Voting Backend", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "evoting_backend_app v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    
}

// Needed to display enum variables of model as strings instead of integers
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            model.Type = "string";
            model.Enum.Clear();
        }
    }
}
