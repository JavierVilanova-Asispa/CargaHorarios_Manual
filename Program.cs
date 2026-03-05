var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el servicio de acceso a datos
builder.Services.AddScoped<CargadorHorario.Web.Data.IDataService, CargadorHorario.Web.Data.DataService>();

// Registrar el servicio de carga de horarios
builder.Services.AddScoped<ICargaHorarioService, CargaHorarioService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
