using System;
using Spectre.Console;
using System.Collections.Generic;

public static class SearchController
{
    // Search product by name
    public static void SearchProductByName()
    {
        AnsiConsole.Markup("[bold][green]Please enter the name of the product you want to search: [/][/]");
        string productName = Console.ReadLine();
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Name.Contains(productName)).ToList();
        MenuController.products = products;
    }
    // Search product by category name
    public static void SearchProductByCategory()
    {
        AnsiConsole.Markup("[bold][green]Enter the name of the category to search: [/][/]");
        string categoryName = Console.ReadLine().Trim(); // Remove whitespace

        using var db = new ApplicationDbContext();
        var category = db.Categories.FirstOrDefault(c => c.Name.ToLower().Contains(categoryName.ToLower()));

        if (category == null)
        {
            MenuController.products = new List<Product>(); // Empty list
        }
        else
        {
            var products = db.Products.Where(p => p.CategoryId == category.Id).ToList();
            MenuController.products = products; // Set products list
        }
    }
    // Search product by price range
    public static void SearchProductByPriceRange()
    {
        decimal minPrice;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the minimum price: [/]");
            if (decimal.TryParse(Console.ReadLine(), out minPrice) && minPrice >= 0)
            {
            break;
            }
            AnsiConsole.MarkupLine("[yellow]Invalid input! Please enter a valid decimal number greater than or equal to 0.[/]");
        }
        decimal maxPrice;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the maximum price: [/]");
            if (decimal.TryParse(Console.ReadLine(), out maxPrice) && maxPrice >= 0)
            {
            break;
            }
            AnsiConsole.MarkupLine("[yellow]Invalid input! Please enter a valid decimal number greater than or equal to 0.[/]");
        }
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        MenuController.products = products; // Set products list
    }
}