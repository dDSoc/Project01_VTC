using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class ProductController
{
    // Product management menu for store manager
    public static void ManagementProduct()
    {
        while (true)
        {
            Menu.ProductManagementMenu();//Call product management menu
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProduct();//Call add product function
                    break;
                case "2":
                    EditProductManager();//Call edit product function
                    break;
                case "3":
                    ShowProduct();//Call show product function
                    break;
                case "0":
                    StoreManagerController.StoreManagementMenu();//Call store management menu to back to store management menu
                    break;
                default:
                    AnsiConsole.Markup("[bold red]Invalid choice. Press any key to continue.[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Show product list for store manager
    private static void ShowProduct()
    {
        Console.Clear();
        using var db = new ApplicationDbContext();
        var products = db.Products.Include(p => p.Category).ToList();
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Price");
        table.AddColumn("Description");
        table.AddColumn("Stock");
        table.AddColumn("Category");
        // Add product to table
        foreach (var product in products)
        {
            table.AddRow(product.Id.ToString(), product.Name, product.Price.ToString(), product.Description, product.Stock.ToString(), product.Category.Name);
        }
        AnsiConsole.Render(table);

        // Option to edit or add product or back
        AnsiConsole.MarkupLine("[bold yellow]1. Edit product[/]");
        AnsiConsole.MarkupLine("[bold yellow]2. Add product[/]");
        AnsiConsole.MarkupLine("[bold yellow]0. Back[/]");
        AnsiConsole.Markup("[bold green]Enter your choice:[/]");
        string choice = Console.ReadLine();
        // Switch case for user choice
        switch (choice)
        {
            case "1":
                EditProductManager();//Call edit product function
                break;
            case "2":
                AddProduct();//Call add product function
                break;
            case "0":
                return;
            default:
                AnsiConsole.Markup("[bold red]Invalid choice. Press any key to continue.[/]");
                Console.ReadKey();
                break;
        }
    }

    // Add product function
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
        AnsiConsole.Markup("[bold green]Enter product price (Max 1.000.000): [/]");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0 || price > 1000000)
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid price.[/]");
            AnsiConsole.Markup("[bold green]Enter product price (Max 1.000.000): [/]");
        }
        product.Price = price;

        // Get product description
        AnsiConsole.Markup("[bold green]Enter product description:[/]");
        string description = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(description))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product description.[/]");
            AnsiConsole.Markup("[bold green]Enter product description: [/]");
            description = Console.ReadLine();
        }
        product.Description = description;

        // Get product stock
        AnsiConsole.Markup("[bold green]Enter product quantity (Max 1.000.000): [/]");
        int stock;
        while (!int.TryParse(Console.ReadLine(), out stock) || stock <= 0 || stock > 1000000)
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid quantity.[/]");
            AnsiConsole.Markup("[bold green]Enter product quantity(Max 1.000.000): [/]");
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
            AnsiConsole.Markup("[bold red]Category not found![/]");
            }
        }

        // Confirm adding product
        AnsiConsole.Markup("[bold yellow]Are you sure you want to add product? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Category = category;
            db.Products.Add(product);
            db.SaveChanges();
            AnsiConsole.Markup("[bold green]Product added successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.Markup("[bold yellow]Product not added!, press any key to back[/]");
            Console.ReadKey();
        }
    }
    // Edit product function
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
        // Show product information
        while (true)
        {
            Console.Clear();
            Menu.ProductEditMenu(productId);//Call product edit menu
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();
            // Switch case for user choice
            switch (choice)
            {
            case "1":
                EditProductName(product);//Call edit product name function
                break;
            case "2":
                EditProductPrice(product);//Call edit product price function
                break;
            case "3":
                EditProductDescription(product);//Call edit product description function
                break;
            case "4":
                EditProductStock(product);//Call edit product stock function
                break;
            case "5":
                EditProductCategory(product);//Call edit product category function
                break;
            case "0":
                return;
            default:
                AnsiConsole.Markup("[bold red]Invalid choice. Press any key to continue.[/]");
                Console.ReadKey();
                break;
            }
            db.SaveChanges();
        }
    }
    // Edit product name function
    private static void EditProductCategory(Product product)
    {
        using var db = new ApplicationDbContext();
        Category category = null;
        while (category == null)
        {
            AnsiConsole.Markup("[bold green]Enter new product category:[/]");
            string categoryName = Console.ReadLine();
            // Find category by name in database
            while (string.IsNullOrWhiteSpace(categoryName))//Check if category name is null or empty
            {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product category.[/]");
            AnsiConsole.Markup("[bold green]Enter new product category:[/]");
            categoryName = Console.ReadLine();
            }
            category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
            AnsiConsole.Markup("[bold red]Category not found![/]");
            }
        }
        // Confirm updating product category
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Category = category;
            AnsiConsole.Markup("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product category not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    // Edit product stock function for store manager
    private static void EditProductStock(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product stock:[/]");
        int stock;
        // Get new product stock
        while (!int.TryParse(Console.ReadLine(), out stock) || stock <= 0 || stock > 1000000)//Check if stock is not a number or less than 0 or greater than 1.000.000
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid quantity (Max 1.000.000).[/]");
            AnsiConsole.Markup("[bold green]Enter new product stock:[/]");
        }
        // Confirm updating product stock
        AnsiConsole.Markup("[bold yellow]Are you sure you want to change? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Stock = stock;
            AnsiConsole.Markup("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product stock not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }
    // Edit product description function
    private static void EditProductDescription(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product description:[/]");
        string description = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(description))//Check if description is null or empty
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product description.[/]");
            AnsiConsole.Markup("[bold green]Enter new product description:[/]");
            description = Console.ReadLine();
        }
        // Confirm updating product description
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Description = description;
            AnsiConsole.Markup("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product description not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    // Edit product price function
    private static void EditProductPrice(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product price:[/]");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0 || price > 1000000)//Check if price is not a number or less than 0 or greater than 1.000.000
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid price (Max 1.000.000).[/]");
            AnsiConsole.Markup("[bold green]Enter new product price:[/]");
        }
        // Confirm updating product price
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Price = price;
            AnsiConsole.Markup("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product price not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    // Edit product name function
    private static void EditProductName(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product name:[/]");
        string name = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(name))//Check if name is null or empty
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product name.[/]");
            AnsiConsole.Markup("[bold green]Enter new product name:[/]");
            name = Console.ReadLine();
        }
        // Confirm updating product name
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Name = name;
            AnsiConsole.Markup("[bold green]Product updated successfully!, press any key to back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product name not updated!, press any key to back[/]");
            Console.ReadKey();
        }
    }

    // Show product by ID
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