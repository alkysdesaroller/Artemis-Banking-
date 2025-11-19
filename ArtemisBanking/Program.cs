using ArtemisBanking.Core.Application;
using ArtemisBanking.Infrastructure.Identity;
using ArtemisBanking.Infrastructure.Persistence;
using ArtemisBanking.Infrastructure.Shared;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddApplicationLayerIoc(builder.Configuration);
builder.Services.AddPersistenceLayerIoc(builder.Configuration);
builder.Services.AddIdentityLayerIocForWebApp(builder.Configuration);
builder.Services.AddSharedLayerIoc(builder.Configuration);
builder.Services.AddHangfireConfiguration(builder.Configuration);

var app = builder.Build();
await app.Services.RunIdentitySeedAsync();
app.UseHangFireJobs();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapGet("/", context =>
{
    context.Response.Redirect($"/Login");
    return Task.CompletedTask;
});

app.Run();