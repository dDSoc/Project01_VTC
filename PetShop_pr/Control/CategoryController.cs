using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class CategoryController
{
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
                    AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
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

    public static void AddCategory()
    {
        using var db = new ApplicationDbContext();
        var category = new Category();
        AnsiConsole.Markup("[bold green]Enter category name: [/]");
        string categoryName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(categoryName))
        {
            AnsiConsole.MarkupLine("[bold red]Category name cannot be empty or contain only whitespace![/]");
            AnsiConsole.Markup("[bold green]Enter category name: [/]");
            categoryName = Console.ReadLine();
        }
        category.Name = categoryName;
        AnsiConsole.Markup("[bold green]Enter category description: [/]");
        string categoryDescription = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(categoryDescription))
        {
            AnsiConsole.MarkupLine("[bold red]Category description cannot be empty![/]");
            AnsiConsole.Markup("[bold green]Enter category description: [/]");
            categoryDescription = Console.ReadLine();
        }
        category.Description = categoryDescription;
        db.Categories.Add(category);
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to add this category? (Y/N)[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            db.SaveChanges();
            AnsiConsole.MarkupLine("[bold green]Category added successfully!, press any key to continue...[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Category not added![/]");
        }
    }

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

    public static void UpdateCategory()
    {
        using var db = new ApplicationDbContext();
        ShowCategory();
        int id;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter category ID to update:[/]");
            string input = Console.ReadLine();
            if (int.TryParse(input, out id))
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid category ID.[/]");
            }
        }
        var category = db.Categories.Find(id);
        if (category == null)
        {
            AnsiConsole.MarkupLine("[bold red]Category not found, press any key to back[/]");
            Console.ReadKey();
            return;
        }
        AnsiConsole.Markup("[bold green]Enter new category name:[/]");
        category.Name = Console.ReadLine();
        AnsiConsole.Markup("[bold green]Enter new category description:[/]");
        category.Description = Console.ReadLine();
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update this category? (Y/N)[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            db.SaveChanges();
            AnsiConsole.MarkupLine("[bold green]Category updated successfully!, press any key to continue...[/]");
            Console.ReadKey();
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Category not updated![/]");
            Console.ReadKey();
        }
    }

}