using Microsoft.EntityFrameworkCore;

namespace Pizzeria.Models
{
    public class Pizza
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Description { get; set; }
    }

    class PizzaDB : DbContext
    {
        public PizzaDB(DbContextOptions<PizzaDB> options) : base(options)
        {
        }
        public DbSet<Pizza> Pizzas { get; set; } = null!;
    }
}
