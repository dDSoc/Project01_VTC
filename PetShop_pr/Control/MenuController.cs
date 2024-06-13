using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class MenuController
{
    public static List<Product> products = new List<Product>();

    //Main menu controller
    public static void MainMenuController()
    {
    
        while (true)
        {
            Menu.DefaultMenu();
            DefaultMenuController();
        }
    }

    //Customer management menu controller
    public static void CustomerManagementMenu()
    {
        while(true)
        {
            Menu.CustomerMenu();
            AnsiConsole.MarkupLine("[bold green]Enter your choice:[/]");
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
                    // UserController.ShowVoucher();
                    break;
                case "5":
                    UserController.EditProfileController();
                    break;
                case "0":
                    UserController.Logout();
                    MainMenuController();
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold yellow]Function does not exist, press any key to continue[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    //Product management menu controller
    public static void ProductManagementMenu()
    {
        while(true)
        {
            // Menu.ProductMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // ProductManager.AddProduct();
                    break;
                case "2":
                    // ProductManager.DeleteProduct();
                    break;
                case "3":
                    // ProductManager.UpdateProduct();
                    break;
                case "4":
                    // ProductManager.ShowProductList();
                    break;
                case "5":
                    // ProductManager.SearchProduct();
                    break;
                case "0":
                    MainMenuController();
                    break;
                default:
                    Console.WriteLine("Chức năng không tồn tại");
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
                Console.WriteLine("Found " + products.Count + " products");
                var table = new Table();
                table.AddColumn("[bold]ID[/]");
                table.AddColumn("[bold]Tên sản phẩm[/]");
                table.AddColumn("[bold]Mô tả[/]");
                table.AddColumn("[bold]Giá[/]");
                table.AddColumn("[bold]Số lượng[/]");
                Console.WriteLine($"Page {currentPage}/{totalPages}");
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

                AnsiConsole.Render(table);

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
                        // default:
                        //     break;
                    }
                // var key = Console.ReadKey(true).Key;
                // if (key == ConsoleKey.LeftArrow && currentPage > 1)
                // {
                //     currentPage--;
                //     table.Rows.Clear();
                // }
                // else if (key == ConsoleKey.RightArrow && currentPage < totalPages)
                // {
                //     currentPage++;
                //     table.Rows.Clear();
                // }
                // else if (key == ConsoleKey.C)
                // {
                //     UserController.AddToCart();
                // }
                // else if (key == ConsoleKey.V)
                // {
                //     UserController.CartController();
                // }
                // else if (key == ConsoleKey.Escape)
                // {
                //     break;
                // }
                // else
                // {
                //     break;
                // }
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
            AnsiConsole.MarkupLine("[bold green]Enter your choice:[/]");
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