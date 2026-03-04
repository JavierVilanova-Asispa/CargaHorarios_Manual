var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el servicio de acceso a datos
builder.Services.AddScoped<CargaDatos.Web.Data.IDataService, CargaDatos.Web.Data.DataService>();

// Registrar el servicio de carga de datos (SCOPED para poder usar DataService)
builder.Services.AddScoped<ICargaDatosService, CargaDatosService>();

// Registrar el servicio programado (hosted service)
builder.Services.AddHostedService<CargaDatosProgramadaService>();

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
