using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SearchApp.Constants;

namespace SearchApp.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseAppMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(AppConstants.Routing.ErrorRoute);
            }

            app.MapGet(AppConstants.Routing.RootRoute, (IWebHostEnvironment env) => 
                Results.Redirect(env.IsDevelopment() ? AppConstants.Development.FrontendDevUrl : AppConstants.Routing.RootRoute));

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.MapRazorPages();
        }
    }
}
