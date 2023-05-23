using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using Cyanometer.Core.Extensions;
using Cyanometer.AirQuality.Extensions;
using Cyanometer.Web.Services.Implementation;
using Cyanometer.Web.Services.Abstract;
using Cyanometer.Core;
using Microsoft.Extensions.Logging;
using Cyanometer.AirQuality.Services;

namespace Cyanometer.Web;

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
        var physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        services.AddSingleton(physicalProvider);
        services.AddCyanoCore();
        services.AddAirQuality();
        services.AddSingleton<IImagesFileManager, ImagesFileManager>();
        services.AddSingleton<IImagesManager, ImagesManager>();

        services.AddRazorPages();
        services.AddControllers();
    }

    static string DumpToken(string token)
    {
        if (token?.Length > 0)
        {
            return token[0..1];
        }
        return "-";
    }
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    { 
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            // dev tokens
            UploadTokens.Init(
                "C14A3DB5-BEE5-4612-9F9C-62972B2C6C83",
                "72DCCD9B-B71B-4B73-8A06-EC137C44F49E",
                "161A6F24-66F6-41F9-BC7D-8BA8A949159A",
                "4AF854D9-647E-4DB0-9219-AD78D64C058D");
            SabraAirQualityCredentials.Init(Configuration["Sabra:Username"], Configuration["Sabra:Password"]);
            AqicnCredentials.Init(Configuration["Aqicn:Token"]);
        }
        else
        {
            app.UseExceptionHandler("/Error");
            UploadTokens.Init(Configuration["LJ_TOKEN"], Configuration["WR_TOKEN"], Configuration["DR_TOKEN"], Configuration["GE_TOKEN"]);
            SabraAirQualityCredentials.Init(Configuration["SABRA_USERNAME"], Configuration["SABRA_PASSWORD"]);
            AqicnCredentials.Init(Configuration["AQICN_TOKEN"]);
        }
        logger.LogInformation("TOKENS: LJ:{LJ} WR:{WR} DR:{DR} GE:{GE}",
            DumpToken(UploadTokens.Instance.Ljubljana), 
            DumpToken(UploadTokens.Instance.Wroclaw),
            DumpToken(UploadTokens.Instance.Millstatt), 
            DumpToken(UploadTokens.Instance.Geneva));

        app.UseStatusCodePages();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}
