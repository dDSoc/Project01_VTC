using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class MenuController
{
    public static List<Product> products = new List<Product>();
    public static void CustomerManagementMenu()
    {
        while(true)
        {
            Menu.CustomerMenu();
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchProduct();
                    break;
                case "2":
                    UserController.ShowUserOrderedList();
                    break;
                case "3":
                    UserController.CartController();
                    break;
                case "4":
                    UserController.EditProfileController();
                    break;
                case "0":
                    UserController.Logout();
                    if(UserController.user == null)
                    {
                        DefaultMenuController();
                    }
                    else
                    {
                        CustomerManagementMenu();
                    }
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static void SearchProduct()
    {
        while (true)
        {
            Menu.CustomerMenu_Search();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchController.SearchProductByName();
                    ShowProductListResult();
                    break;
                case "2":
                    SearchController.SearchProductByPriceRange();
                    ShowProductListResult();
                    break;
                case "3":
                    SearchController.SearchProductByCategory();
                    ShowProductListResult();
                    break;
                case "0":
                    if(UserController.user == null)
                    {
                        DefaultMenuController();
                    }
                    else
                    {
                        CustomerManagementMenu();
                    }
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static void ShowProductListResult()
    {
        Console.Clear();
        if(products.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold yellow]No product found! Press any key to continue[/]");
            Console.ReadKey();
            return;
        }
        int pageSize = 5;
        int currentPage = 1;
        int totalPages = (int)Math.Ceiling((double)products.Count / pageSize);

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
            AnsiConsole.MarkupLine("[bold]Press '[/][bold red]CTRL + P[/][bold]' for previous page, '[/][bold red]CTRL + N[/][bold]' for next page[/]");
            AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + A[/][bold]' to add to cart[/]");
            AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + L[/][bold]' to view cart[/]");
            AnsiConsole.MarkupLine("[bold]Press [yellow]ESC[/] key to exit.[/]");

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
                        UserController.AddToCart();
                        break;
                    case ConsoleKey.L:
                        UserController.CartController();
                        break;
                }
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                return;
            }
    }
    }


    public static void DefaultMenuController()
    {
        while (true)
        {
            Menu.DefaultMenu();
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchProduct();
                    break;
                case "2":
                    UserController.Login();
                    break;
                case "3":
                    UserController.Register();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("");
                    AnsiConsole.MarkupLine("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }
}   