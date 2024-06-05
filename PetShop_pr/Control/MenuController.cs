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
            Menu.MainMenu();
            Login();
        }
    }

    //Customer management menu controller
    public static void CustomerManagementMenu()
    {
        while(true)
        {
            Menu.CustomerMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchProduct();
                    break;
                case "2":
                    // CustomerManager.DeleteCustomer();
                    break;
                case "3":
                    CartController.ShowCart();
                    break;
                case "4":
                    // CustomerManager.ShowCustomerList();
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

    //Order management menu controller
    public static void OrderManagementMenu()
    {
        while(true)
        {
            // Menu.OrderMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // OderManager.AddOrder();
                    break;
                case "2":
                    // OderManager.DeleteOrder();
                    break;
                case "3":
                    // OderManager.UpdateOrder();
                    break;
                case "4":
                    // OderManager.ShowOrderList();
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

    public static void Login()
    {
        using (var context = new ApplicationDbContext())
        {
            // Lấy thông tin đăng nhập từ người dùng
            var username = AnsiConsole.Ask<string>("[bold yellow]->[/] [bold]Username:[/] ");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold yellow]->[/] [bold]Password:[/] ")
                    .Secret());

            // Tìm người dùng trong cơ sở dữ liệu
            var user = context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                AnsiConsole.Markup("[bold red]Username does not exist![/]");
            }
            else
            {
                CartController.user = user;
                // Kiểm tra mật khẩu
                if (user.Password != password)
                {
                    AnsiConsole.Markup("[bold red]Invalid password![/]");
                }
                else
                {
                    // // Đăng nhập thành công
                    // AnsiConsole.Markup("[bold green]Login successful![/]");
                    
                    // Kiểm tra vai trò người dùng
                    switch (user.Role.ToLower())
                    {
                        case "customer":
                            CustomerManagementMenu();
                            break;
                        case "store_manager":
                            AnsiConsole.Markup("[bold yellow]Welcome, Store Manager![/]");
                            // Thực hiện các hành động dành cho store manager
                            break;
                        case "shop_owner":
                            AnsiConsole.Markup("[bold yellow]Welcome, Shop Owner![/]");
                            // Thực hiện các hành động dành cho shop owner
                            break;
                        default:
                            AnsiConsole.Markup("[bold red]Unknown role![/]");
                            break;
                    }
                }
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
                    ShowProductList();
                    break;
                case "2":
                    SearchController.SearchProductByPriceRange();
                    ShowProductList();
                    break;
                case "3":
                    SearchController.SearchProductByCategory();
                    ShowProductList();
                    break;
                case "0":
                    CustomerManagementMenu();
                    break;
                default:
                    Console.WriteLine("Chức năng không tồn tại");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static void ShowProductList()
    {
         Console.WriteLine("Found " + products.Count + " products");
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Tên sản phẩm");
            table.AddColumn("Mô tả");
            table.AddColumn("Giá");
            table.AddColumn("Số lượng");

            int pageSize = 5;
            int currentPage = 1;
            int totalPages = (int)Math.Ceiling((double)products.Count / pageSize);

            while (true)
            {
                Console.Clear();
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
                Console.WriteLine("Press 'Left arow' for previous page, 'Right arow' for next page, or any other key to exit.");
                Console.WriteLine("Press 'C' to create order");
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.LeftArrow && currentPage > 1)
                {
                    currentPage--;
                    table.Rows.Clear();
                }
                else if (key == ConsoleKey.RightArrow && currentPage < totalPages)
                {
                    currentPage++;
                    table.Rows.Clear();
                }
                else if (key == ConsoleKey.C)
                {
                    CartController.AddToCart();
                }
                else
                {
                    break;
                }
            }
    }

}   