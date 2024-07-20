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

        // Validate product name
        while (!DataValidator.ValidateProductName(productName))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product name.[/]");
            AnsiConsole.Markup("[bold][green]Please enter the name of the product you want to search: [/][/]");
            productName = Console.ReadLine();
        }

        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Name.Contains(productName)).ToList();
        MenuController.products = products;
    }

    // Search product by category name
    public static void SearchProductByCategory()
    {
        AnsiConsole.Markup("[bold][green]Enter the name of the category to search: [/][/]");
        string categoryName = Console.ReadLine().Trim(); // Remove whitespace

        // Validate category name
        while (!DataValidator.ValidateCategoryName(categoryName))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid category name.[/]");
            AnsiConsole.Markup("[bold][green]Enter the name of the category to search: [/][/]");
            categoryName = Console.ReadLine().Trim(); // Remove whitespace
        }

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
            string minPriceInput = Console.ReadLine();
            if (DataValidator.ValidatePrice(minPriceInput) && decimal.TryParse(minPriceInput, out minPrice) && minPrice >= 0)
            {
                break;
            }
            AnsiConsole.MarkupLine("[red]Invalid input! Please enter a valid price.[/]");
        }

        decimal maxPrice;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the maximum price: [/]");
            string maxPriceInput = Console.ReadLine();
            if (DataValidator.ValidatePrice(maxPriceInput) && decimal.TryParse(maxPriceInput, out maxPrice) && maxPrice >= 0)
            {
                break;
            }
            AnsiConsole.MarkupLine("[red]Invalid input! Please enter a valid price.[/]");
        }

        using var db = new ApplicationDbContext();
        var products = db.Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        MenuController.products = products; // Set products list
    }
}