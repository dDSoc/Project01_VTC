using System;
using Spectre.Console;
using System.Collections.Generic;

public static class SearchController
{
    // Search product by name
    public static void SearchProductByName()
    {
        AnsiConsole.MarkupLine("[bold][green]Please enter the name of the product you want to search:[/][/]");
        string productName = Console.ReadLine();
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Name.Contains(productName)).ToList();
        MenuController.products = products;
    }
    // Search product by category name
    public static void SearchProductByCategory()
    {
        AnsiConsole.MarkupLine("[bold][green]Enter the name of the category to search: [/][/]");
        string categoryName = Console.ReadLine().Trim(); // Xóa khoảng trắng ở đầu và cuối chuỗi

        using var db = new ApplicationDbContext();
        var category = db.Categories.FirstOrDefault(c => c.Name.ToLower().Contains(categoryName.ToLower()));

        if (category == null)
        {
            MenuController.products = new List<Product>();
        }
        else
        {
            var products = db.Products.Where(p => p.CategoryId == category.Id).ToList();
            MenuController.products = products;
        }
    }
    // Search product by price range
    public static void SearchProductByPriceRange()
    {
        AnsiConsole.MarkupLine("[bold green]Enter the minimum price:[/]");
        decimal minPrice;
        while (!decimal.TryParse(Console.ReadLine(), out minPrice))
        {
            AnsiConsole.MarkupLine("[yellow]Invalid input! Please enter a valid decimal number.\n[/]");
            AnsiConsole.MarkupLine("[bold green]Enter the minimum price:[/]");
        }
        AnsiConsole.MarkupLine("[bold green]\nEnter the maximum price:[/]");
        decimal maxPrice;
        while (!decimal.TryParse(Console.ReadLine(), out maxPrice))
        {
            AnsiConsole.MarkupLine("[yellow]Invalid input! Please enter a valid decimal number.\n[/]");
            AnsiConsole.MarkupLine("[bold green]Enter the maximum price:[/]");
        }
        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        MenuController.products = products;
    }
}