using System.Reflection;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderItemRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<MenuItemRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<ReservationTableRepository>();
builder.Services.AddScoped<RestaurantRepository>();
builder.Services.AddScoped<TableRepository>();
builder.Services.AddDbContext<RestaurantReservationsContext>();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = "RestaurantReservationAPI.xml";
    var xmlCommentsFullPath= Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

