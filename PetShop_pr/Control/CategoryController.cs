using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class CategoryController
{
    // Category management menu
    public static void ManagementCategory()
    {
        while (true)
        {
            Menu.CategoryManagementMenu();
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddCategory();
                    break;
                case "2":
                    ShowCategory();
                    AnsiConsole.Markup("[bold green]Press any key to continue...[/]");
                    Console.ReadKey();
                    break;
                case "3":
                    UpdateCategory();
                    break;
                case "0":
                    StoreManagerController.StoreManagementMenu();
                    break;
            }
        }
    }
    // Add category function
    public static void AddCategory()
    {
        using var db = new ApplicationDbContext();
        var category = new Category();
        
        // Validate category name
        AnsiConsole.Markup("[bold green]Enter category name: [/]");
        string categoryName = Console.ReadLine();
        while (!DataValidator.ValidateCategoryName(categoryName))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid category name. Category name cannot be empty, contain only whitespace, and should be less than 100 characters![/]");
            AnsiConsole.Markup("[bold green]Enter category name: [/]");
            categoryName = Console.ReadLine();
        }
        category.Name = categoryName;

        // Validate category description
        AnsiConsole.Markup("[bold green]Enter category description: [/]");
        string categoryDescription = Console.ReadLine();
        while (!DataValidator.ValidateDescriptionCategory(categoryDescription))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid category description. Description cannot be empty, contain only whitespace, and should be less than 250 characters![/]");
            AnsiConsole.Markup("[bold green]Enter category description: [/]");
            categoryDescription = Console.ReadLine();
        }
        category.Description = categoryDescription;

        db.Categories.Add(category);
        
        // Confirm adding category
        AnsiConsole.Markup("[bold yellow]Are you sure you want to add? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            db.SaveChanges();
            AnsiConsole.Markup("[bold green]Category added successfully! Press any key to continue...[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Category not added![/]");
            Console.ReadKey();
        }
    }

    // Show category function    
    public static void ShowCategory()
    {
        Console.Clear();
        using var db = new ApplicationDbContext();
        var categories = db.Categories.ToList();
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");
        foreach (var category in categories)
        {
            table.AddRow(category.Id.ToString(), category.Name, category.Description);
        }
        table.Expand();
        AnsiConsole.Render(table);
    }
    // Update category function
    public static void UpdateCategory()
    {
        using var db = new ApplicationDbContext();
        // Show all categories
        ShowCategory();
        int id;
        // Get category ID to update
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter category ID to update:[/]");
            string input = Console.ReadLine();
            if (DataValidator.ValidateID(input) && int.TryParse(input, out id))
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid category ID (0 to 1000000).[/]");
            }
        }
        var category = db.Categories.Find(id); // Find category by ID
        if (category == null)
        {
            AnsiConsole.MarkupLine("[bold red]Category not found, press any key to back[/]");
            Console.ReadKey();
            return;
        }

        // Validate new category name
        AnsiConsole.Markup("[bold green]Enter new category name:[/]");
        string newCategoryName = Console.ReadLine();
        while (!DataValidator.ValidateCategoryName(newCategoryName))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid category name. Category name cannot be empty, contain only whitespace, and should be less than 100 characters![/]");
            AnsiConsole.Markup("[bold green]Enter new category name:[/]");
            newCategoryName = Console.ReadLine();
        }
        category.Name = newCategoryName;

        // Validate new category description
        AnsiConsole.Markup("[bold green]Enter new category description:[/]");
        string newCategoryDescription = Console.ReadLine();
        while (!DataValidator.ValidateDescriptionCategory(newCategoryDescription))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid category description. Description cannot be empty, contain only whitespace, and should be less than 250 characters![/]");
            AnsiConsole.Markup("[bold green]Enter new category description:[/]");
            newCategoryDescription = Console.ReadLine();
        }
        category.Description = newCategoryDescription;

        // Confirm updating category
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            db.SaveChanges();
            AnsiConsole.Markup("[bold green]Category updated successfully! Press any key to continue.[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Category not updated! Press any key to continue.[/]");
            Console.ReadKey();
        }
    }
}