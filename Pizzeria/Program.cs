using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pizzeria.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=items.db";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSqlite<PizzaDB>(connectionString);


builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Pizzeria API", 
        Version = "v1",
        Description = "Pizza Yolo",
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pizzeria API V1");
    });
}

app.MapGet("/", () => "Ecole Supérieur Polytechnique DIC 3!");
app.MapGet("/pizzas", async (PizzaDB db) =>
{
    return await db.Pizzas.ToListAsync();
});

app.MapPost("/pizzas", async (PizzaDB db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizzas/{pizza.Id}", pizza);
});

app.MapGet("/pizzas/{id}", async (PizzaDB db, int id) =>
{
    return await db.Pizzas.FindAsync(id)
        is Pizza pizza
            ? Results.Ok(pizza)
            : Results.NotFound();
});

app.MapPut("/pizzas/{id}", async (PizzaDB db, Pizza pizza, int id) =>
{
    if (pizza.Id != id)
    {
        return Results.BadRequest();
    }
    db.Entry(pizza).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/pizzas/{id}", async (PizzaDB db, int id) =>
{
    if (await db.Pizzas.FindAsync(id) is Pizza pizza)
    {
        db.Pizzas.Remove(pizza);
        await db.SaveChangesAsync();
        return Results.Ok(pizza);
    }
    return Results.NotFound();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
