using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class MenuController
{
    //Product list for search result or show all products
    public static List<Product> products = new List<Product>();

    // Customer management menu
    public static void CustomerManagementMenu()
    {
        while(true)
        {
            // Show customer management menu
            Menu.CustomerMenu();
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();
            // Switch case for customer management menu
            switch (choice)
            {
                case "1":
                    SearchProduct();//Call search product function
                    break;
                case "2":
                    UserController.ShowUserOrderedList();//Call show user ordered list function
                    break;
                case "3":
                    UserController.CartController();//Call cart controller function
                    break;
                case "4":
                    ShowAllProducts();//Call show all products function
                    break;
                case "5":
                    UserController.EditProfileController();//Call edit profile controller function
                    break;
                case "0":
                    UserController.Logout();//Call logout function
                    if(UserController.user == null)
                    {
                        DefaultMenuController();//Call default menu controller function
                    }
                    else
                    {
                        CustomerManagementMenu();//Call customer management menu function
                    }
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Search product function
    public static void SearchProduct()
    {
        while (true)
        {
            Menu.CustomerMenu_Search();//Show search menu
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();
            // Switch case for search menu
            switch (choice)
            {
                case "1":
                    SearchController.SearchProductByName();//Call search product by name function
                    ShowProductListResult();//Then show product list result
                    break;
                case "2":
                    SearchController.SearchProductByPriceRange();//Call search product by price range function
                    ShowProductListResult();//Then show product list result
                    break;
                case "3":
                    SearchController.SearchProductByCategory();//Call search product by category function
                    ShowProductListResult();//Then show product list result
                    break;
                case "0":
                    if(UserController.user == null)
                    {
                        DefaultMenuController();//Call default menu controller function
                    }
                    else
                    {
                        CustomerManagementMenu();//Call customer management menu function
                    }
                    break;
                default:
                    AnsiConsole.Markup("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Show product list result function
    public static void ShowProductListResult()
    {
        Console.Clear();
        if(products.Count == 0)
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
            table.AddColumn("[bold]Product Name[/]");
            table.AddColumn("[bold]Description[/]");
            table.AddColumn("[bold]Price[/]");
            table.AddColumn("[bold]Quantity[/]");
            Console.WriteLine();
            Console.WriteLine();
            // Loop to show product list in each page
            for (int i = (currentPage - 1) * pageSize; i < currentPage * pageSize && i < products.Count; i++)
            {
                var product = products[i];
                table.AddRow(
                    product.Id.ToString(),
                    product.Name,
                    product.Description,
                    product.Price.ToString(),
                    product.Stock.ToString()
                );
            }
            table.Expand();
            AnsiConsole.Write(table);

            Console.WriteLine();
            // Show page navigation and action instruction for customer
            AnsiConsole.MarkupLine("[bold]Press '[/][bold red]CTRL + P[/][bold]' for previous page, '[/][bold red]CTRL + N[/][bold]' for next page[/]");
            AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + A[/][bold]' to add to cart[/]");
            AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + L[/][bold]' to view cart[/]");
            AnsiConsole.MarkupLine("[bold]Press [yellow]ESC[/] key to exit.[/]");
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
                    case ConsoleKey.A:
                        UserController.AddToCart();//Call add to cart function
                        break;
                    case ConsoleKey.L:
                        UserController.CartController();//Call cart controller function
                        break;
                }
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                return;
            }
    }
    }

    // Default menu controller function
    public static void DefaultMenuController()
    {   
        while (true)//Loop to show default menu
        {
            Menu.DefaultMenu();//Show default menu
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();
            //Switch case for default menu
            switch (choice)
            {
                case "1":
                    SearchProduct();//Call search product function
                    break;
                case "2":
                    UserController.Login();//Call login function
                    break;
                case "3":
                    UserController.Register();//Call register function
                    break;
                case "4":
                    ShowAllProducts();//Call show all products function
                    break;
                case "0":
                    Environment.Exit(0);//Exit the program
                    break;
                default:
                    Console.WriteLine("");
                    AnsiConsole.Markup("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Show all products function for customer
    public static void ShowAllProducts()
    {
        using var db = new ApplicationDbContext();
        products = db.Products.ToList();
        ShowProductListResult();
    }
}   