using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class ProductController
{
    public static void ManagementProduct()
    {
        while (true)
        {
            Menu.ProductManagementMenu();
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    EditProductManager();
                    break;
                case "0":
                    StoreManagerController.StoreManagementMenu();
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold red]Invalid choice. Press any key to continue.[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static void AddProduct()
    {
        using var db = new ApplicationDbContext();
        Product product = new Product();

        // Get product name
        AnsiConsole.Markup("[bold green]Enter product name:[/]");
        string name = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(name))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product name.[/]");
            AnsiConsole.Markup("[bold green]Enter product name:[/]");
            name = Console.ReadLine();
        }
        
        product.Name = name;

        // Get product price
        AnsiConsole.Markup("[bold green]Enter product price: [/]");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid price.[/]");
            AnsiConsole.Markup("[bold green]Enter product price: [/]");
        }
        product.Price = price;

        // Get product description
        AnsiConsole.Markup("[bold green]Enter product description:[/]");
        string description = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(description))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product description.[/]");
            AnsiConsole.Markup("[bold green]Enter product description:[/]");
            description = Console.ReadLine();
        }
        product.Description = description;

        // Get product stock
        AnsiConsole.Markup("[bold green]Enter product quantity:[/]");
        int stock;
        while (!int.TryParse(Console.ReadLine(), out stock) || stock <= 0)
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid quantity.[/]");
            AnsiConsole.Markup("[bold green]Enter product quantity:[/]");
        }
        product.Stock = stock;

        // Get product category
        Category category = null;
        while (category == null)
        {
            AnsiConsole.Markup("[bold green]Enter product category:[/]");
            string categoryName = Console.ReadLine();
            category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
            AnsiConsole.MarkupLine("[bold red]Category not found![/]");
            }
        }

        // Confirm adding product
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to add this product? (Y/N):[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Category = category;
            db.Products.Add(product);
            db.SaveChanges();
            AnsiConsole.MarkupLine("[bold green]Product added successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold yellow]Product not added!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    public static void EditProductManager()
    {
        Console.Clear();
        var Panel = new Panel("[bold green]Edit Product[/]");
        using var db = new ApplicationDbContext();
        int productId;
        AnsiConsole.Markup("[bold green]Enter product ID to edit:[/]");
        while (!int.TryParse(Console.ReadLine(), out productId))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product ID.[/]");
            AnsiConsole.Markup("[bold green]Enter product ID to edit:[/]");
        }
        // Find the product by ID
        var product = db.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            AnsiConsole.MarkupLine("[bold red]Product not found![/]");
            Console.ReadKey();
            return;
        }
        
        while (true)
        {
            Console.Clear();
            Menu.ProductEditMenu(productId);
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();
            switch (choice)
            {
            case "1":
                EditProductName(product);
                break;
            case "2":
                EditProductPrice(product);
                break;
            case "3":
                EditProductDescription(product);
                break;
            case "4":
                EditProductStock(product);
                break;
            case "5":
                EditProductCategory(product);
                break;
            case "0":
                return;
            default:
                AnsiConsole.MarkupLine("[bold red]Invalid choice. Press any key to continue.[/]");
                Console.ReadKey();
                break;
            }
            db.SaveChanges();
        }
    }

    private static void EditProductCategory(Product product)
    {
        using var db = new ApplicationDbContext();
        Category category = null;
        while (category == null)
        {
            AnsiConsole.Markup("[bold green]Enter new product category:[/]");
            string categoryName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(categoryName))
            {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product category.[/]");
            AnsiConsole.Markup("[bold green]Enter new product category:[/]");
            categoryName = Console.ReadLine();
            }
            category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
            AnsiConsole.MarkupLine("[bold red]Category not found![/]");
            }
        }
        // Confirm updating product category
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to update the product category? (Y/N):[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Category = category;
            AnsiConsole.MarkupLine("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Product category not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    private static void EditProductStock(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product stock:[/]");
        int stock;

        while (!int.TryParse(Console.ReadLine(), out stock) || stock <= 0 || stock > 1000000)
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid quantity (Max 1.000.000).[/]");
            AnsiConsole.Markup("[bold green]Enter new product stock:[/]");
        }
        // Confirm updating product stock
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to update the product stock? (Y/N):[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Stock = stock;
            AnsiConsole.MarkupLine("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Product stock not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    private static void EditProductDescription(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product description:[/]");
        string description = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(description))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product description.[/]");
            AnsiConsole.MarkupLine("[bold green]Enter new product description:[/]");
            description = Console.ReadLine();
        }
        // Confirm updating product description
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to update the product description? (Y/N):[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Description = description;
            AnsiConsole.MarkupLine("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Product description not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    private static void EditProductPrice(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product price:[/]");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0 || price > 1000000)
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid price (Max 1.000.000).[/]");
            AnsiConsole.Markup("[bold green]Enter new product price:[/]");
        }
        // Confirm updating product price
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to update the product price? (Y/N):[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Price = price;
            AnsiConsole.MarkupLine("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Product price not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    private static void EditProductName(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product name:[/]");
        string name = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(name))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product name.[/]");
            AnsiConsole.Markup("[bold green]Enter new product name:[/]");
            name = Console.ReadLine();
        }
        // Confirm updating product name
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to update the product name? (Y/N):[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Name = name;
            AnsiConsole.MarkupLine("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Product name not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }
    public static Table ShowProducById(int productId)
    {
        using var db = new ApplicationDbContext();
        var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            AnsiConsole.MarkupLine("[bold red]Product not found![/]");
            Console.ReadKey();
            return null;
        }
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Price");
        table.AddColumn("Description");
        table.AddColumn("Stock");
        table.AddColumn("Category");
        table.AddRow(product.Id.ToString(), product.Name, product.Price.ToString(), product.Description, product.Stock.ToString(), product.Category.Name);
        table.Expand();
        return table;
    }
}