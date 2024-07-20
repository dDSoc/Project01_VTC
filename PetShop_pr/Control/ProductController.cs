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
    if (products.Count == 0)
    {
        AnsiConsole.Markup("[bold yellow]No product found! Press any key to back[/]");
        Console.ReadKey();
        return;
    }
    int pageSize = 5;
    int currentPage = 1;
    int totalPages = (int)Math.Ceiling((double)products.Count / pageSize);

    // Loop to show product list
    while (true)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[bold green]Found {products.Count} products[/]");
        var table = new Table()
        {
            Title = new TableTitle($"[bold yellow]Page {currentPage}/{totalPages}[/]"),
        };
        table.AddColumn("[bold]ID[/]");
        table.AddColumn("[bold]Name[/]");
        table.AddColumn("[bold]Price[/]");
        table.AddColumn("[bold]Description[/]");
        table.AddColumn("[bold]Stock[/]");
        table.AddColumn("[bold]Category[/]");
        Console.WriteLine();
        Console.WriteLine();

        // Loop to show product list in each page
        for (int i = (currentPage - 1) * pageSize; i < currentPage * pageSize && i < products.Count; i++)
        {
            var product = products[i];
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Price.ToString(),
                product.Description,
                product.Stock.ToString(),
                product.Category.Name
            );
        }
        table.Expand();
        AnsiConsole.Write(table);

        Console.WriteLine();
        // Show page navigation and action instruction for customer
        AnsiConsole.MarkupLine("[bold]Press '[/][bold red]CTRL + P[/][bold]' for previous page, '[/][bold red]CTRL + N[/][bold]' for next page[/]");
        AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + E[/][bold]' to edit product[/]");
        AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + A[/][bold]' to add product[/]");
        AnsiConsole.MarkupLine("[bold]Press [yellow]ESC[/] key to back.[/]");
        // Read key input from customer
        var keyInfo = Console.ReadKey(true);
        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.P:
                    if (currentPage > 1)
                    {
                        currentPage--;
                        table.Rows.Clear();
                    }
                    break;
                case ConsoleKey.N:
                    if (currentPage < totalPages)
                    {
                        currentPage++;
                        table.Rows.Clear();
                    }
                    break;
                case ConsoleKey.E:
                    EditProductManager();//Call edit product function
                    break;
                case ConsoleKey.A:
                    AddProduct();//Call add product function
                    break;
            }
        }
        else if (keyInfo.Key == ConsoleKey.Escape)
        {
            return;
        }
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
        while (!DataValidator.ValidateProductName(name))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product name.[/]");
            AnsiConsole.Markup("[bold green]Enter product name:[/]");
            name = Console.ReadLine();
        }
        product.Name = name;

        // Get product price
        AnsiConsole.Markup("[bold green]Enter product price (Max 1.000.000): [/]");
        decimal price;
        while (true)
        {
            string priceInput = Console.ReadLine();
            if (DataValidator.ValidatePrice(priceInput) && decimal.TryParse(priceInput, out price) && price <= 1000000)
            {
                break;
            }
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid price.[/]");
            AnsiConsole.Markup("[bold green]Enter product price (Max 1.000.000): [/]");
        }
        product.Price = price;

        // Get product description
        AnsiConsole.Markup("[bold green]Enter product description:[/]");
        string description = Console.ReadLine();
        while (!DataValidator.ValidateDescriptionProduct(description))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product description.[/]");
            AnsiConsole.Markup("[bold green]Enter product description: [/]");
            description = Console.ReadLine();
        }
        product.Description = description;

        // Get product stock
        AnsiConsole.Markup("[bold green]Enter product quantity (Max 1.000.000): [/]");
        int stock;
        while (true)
        {
            string stockInput = Console.ReadLine();
            if (DataValidator.ValidateQuantity(stockInput) && int.TryParse(stockInput, out stock) && stock <= 1000000)
            {
                break;
            }
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

            // Validate category name
            while (!DataValidator.ValidateCategoryName(categoryName))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product category.[/]");
                AnsiConsole.Markup("[bold green]Enter product category:[/]");
                categoryName = Console.ReadLine();
            }

            // Find category by name in database
            category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
                AnsiConsole.MarkupLine("[bold red]Category not found![/]");
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
            AnsiConsole.Markup("[bold green]Product added successfully! Press any key to go back[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.Markup("[bold yellow]Product not added! Press any key to go back[/]");
            Console.ReadKey();
        }
    }

    // Edit product function
   public static void EditProductManager()
    {
        Console.Clear();
        var panel = new Panel("[bold green]Edit Product[/]");
        using var db = new ApplicationDbContext();
        int productId;

        AnsiConsole.Markup("[bold green]Enter product ID to edit:[/]");
        string input = Console.ReadLine();
        while (!DataValidator.ValidateID(input) || !int.TryParse(input, out productId))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product ID.[/]");
            AnsiConsole.Markup("[bold green]Enter product ID to edit:[/]");
            input = Console.ReadLine();
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
            Menu.ProductEditMenu(productId); // Call product edit menu
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            // Switch case for user choice
            switch (choice)
            {
                case "1":
                    EditProductName(product); // Call edit product name function
                    break;
                case "2":
                    EditProductPrice(product); // Call edit product price function
                    break;
                case "3":
                    EditProductDescription(product); // Call edit product description function
                    break;
                case "4":
                    EditProductStock(product); // Call edit product stock function
                    break;
                case "5":
                    EditProductCategory(product); // Call edit product category function
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

            // Validate category name
            while (!DataValidator.ValidateCategoryName(categoryName))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid product category.[/]");
                AnsiConsole.Markup("[bold green]Enter new product category:[/]");
                categoryName = Console.ReadLine();
            }

            // Find category by name in database
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
            AnsiConsole.Markup("[bold green]Product updated successfully! Press any key to go back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product category not updated! Press any key to go back[/]");
            Console.ReadKey();
        }
    }


    // Edit product stock function for store manager
    private static void EditProductStock(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product stock:[/]");
        int stock;
        string input;

        // Get new product stock
        while (true)
        {
            input = Console.ReadLine();
            if (DataValidator.ValidateQuantity(input) && int.TryParse(input, out stock) && stock <= 1000000)
            {
                break;
            }
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid quantity (Max 1.000.000).[/]");
            AnsiConsole.Markup("[bold green]Enter new product stock:[/]");
        }

        // Confirm updating product stock
        AnsiConsole.Markup("[bold yellow]Are you sure you want to change? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Stock = stock;
            AnsiConsole.Markup("[bold green]Product updated successfully! Press any key to go back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product stock not updated! Press any key to go back[/]");
            Console.ReadKey();
        }
    }

    // Edit product description function
    private static void EditProductDescription(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product description:[/]");
        string description = Console.ReadLine();

        // Validate product description
        while (!DataValidator.ValidateDescriptionProduct(description))
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
            AnsiConsole.Markup("[bold green]Product updated successfully! Press any key to go back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product description not updated! Press any key to go back[/]");
            Console.ReadKey();
        }
    }


    // Edit product price function
    private static void EditProductPrice(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product price:[/]");
        decimal price;

        // Validate product price
        while (true)
        {
            string input = Console.ReadLine();
            if (DataValidator.ValidatePrice(input) && decimal.TryParse(input, out price) && price <= 1000000)
            {
                break;
            }
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid price (Max 1.000.000).[/]");
            AnsiConsole.Markup("[bold green]Enter new product price:[/]");
        }

        // Confirm updating product price
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            product.Price = price;
            AnsiConsole.Markup("[bold green]Product updated successfully! Press any key to go back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product price not updated! Press any key to go back[/]");
            Console.ReadKey();
        }
    }


    // Edit product name function
    private static void EditProductName(Product product)
    {
        AnsiConsole.Markup("[bold green]Enter new product name:[/]");
        string name = Console.ReadLine();

        // Validate product name
        while (!DataValidator.ValidateProductName(name))
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
            AnsiConsole.Markup("[bold green]Product updated successfully! Press any key to go back[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Product name not updated! Press any key to go back[/]");
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