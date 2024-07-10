using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using Spectre.Console;
using System.Text.RegularExpressions;

public static class UserController
{
    public static User user;

    //User login
    public static void Login()
    {
        bool loginSuccess = false;
        do
        {
            Console.Clear();
            Menu.LoginMenu();
            using (var context = new ApplicationDbContext())
            {
                // Get login information from the user
                var username = AnsiConsole.Ask<string>("[bold Aqua]->[/] [bold Aqua]Username:[/] ");
                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold Aqua]->[/] [bold Aqua]Password:[/] ")
                        .Secret());

                // Find the user in the database
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    AnsiConsole.Markup("[bold red]Username does not exist, input any key to Back![/]");
                    Console.ReadKey();
                }
                else
                {
                    // Check the password
                    if (user.Password != password)
                    {
                        AnsiConsole.Markup("[bold red]Invalid password, input any key to Back![/]");
                        Console.ReadKey();
                    }
                    else
                    {
                        UserController.user = user;
                        loginSuccess = true;
                        // Check user role
                        switch (user.Role.ToLower())
                        {
                            case "customer":
                                MenuController.CustomerManagementMenu();
                                break;
                            case "store_manager":
                                StoreManagerController.StoreManagementMenu();
                                break;
                            default:
                                AnsiConsole.Markup("[bold red]Unknown role![/]");
                                break;
                        }
                    }
                }
            }
        } while (!loginSuccess);
    }

    //User registration
    public static void Register()
    {
        using var db = new ApplicationDbContext();
        var user = new User();
        AnsiConsole.Markup("[bold green]Enter your fullname: [/]");
        string fullName = Console.ReadLine();
        while(string.IsNullOrWhiteSpace(fullName))
        {
            AnsiConsole.MarkupLine("[bold red]Fullname cannot be empty![/]");
            AnsiConsole.Markup("[bold green]Enter your fullname: [/]");
            fullName = Console.ReadLine();
        }
        user.FullName = fullName;
        string email;
        User existingUser; // Declare the existingUser variable
        string username; // Declare the username variable
        do
        {
            username = AnsiConsole.Ask<string>("[bold green]Enter username: [/]");
            existingUser = db.Users.FirstOrDefault(u => u.Username == username); // Assign the existingUser value
            if (existingUser != null)
            {
                AnsiConsole.MarkupLine("[bold red]Username already exists![/]");
            }
        } while (existingUser != null);
        user.Username = username;
        do
        {
            email = AnsiConsole.Ask<string>("[bold green]Enter email: [/]");
            existingUser = db.Users.FirstOrDefault(u => u.Email == email); // Assign the existingUser value
            if (existingUser != null)
            {
            AnsiConsole.MarkupLine("[bold red]Email already exists![/]");
            }
            else if (!IsValidEmail(email))
            {
            AnsiConsole.MarkupLine("[bold red]Invalid email format![/]");
            }
        } while (existingUser != null || !IsValidEmail(email));
        user.Email = email;
        AnsiConsole.Markup("[bold green]Enter address: [/]");
        string address = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(address))
        {
            AnsiConsole.MarkupLine("[bold red]Address cannot be empty![/]");
            AnsiConsole.Markup("[bold green]Enter address: [/]");
            address = Console.ReadLine();
        }
        string password;
        string passwordConfirm;
        do
        {
            password = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold green]Enter password: [/]")
            .Secret());
            passwordConfirm = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold green]Confirm password: [/]")
            .Secret());
            if (password != passwordConfirm)
            {
            AnsiConsole.MarkupLine("[bold red]Passwords do not match, please try again![/]");
            }
        } while (password != passwordConfirm);
        user.Password = password;
        user.Role = "customer";
        string confirm = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold green]Do you want to register this account? (Y/N): [/]")
            .AllowEmpty()
            .Validate(choice =>
            {
            if (choice.ToUpper() != "Y" && choice.ToUpper() != "N")
            {
                return ValidationResult.Error("[bold red]Invalid choice, please enter Y or N[/]");
            }
            return ValidationResult.Success();
            }));
        if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Registration cancelled, press any key to continue[/]");
            Console.ReadKey();
            return; // Don't register and return to main menu
        }
        db.Users.Add(user);
        db.SaveChanges();
        UserController.user = user;
        AnsiConsole.Markup("[bold green]Registration successful, press any key to continue[/]");
        Console.ReadKey();
        MenuController.CustomerManagementMenu();
    }

    //Add product to cart
    public static void AddToCart()
    {
        if (user == null)
        {   
            while (true)
            {
                Menu.RequireLoginMenu();
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Login();
                        return;
                    case "2":
                        Register();
                        return;
                    case "0":
                        return;
                    default:
                        AnsiConsole.Markup("[bold yellow]Invalid choice, press any key to back[/]");
                        Console.ReadKey();
                        break;
                }
            }
        }

        int productId;
        while (true)
        {
            var input = AnsiConsole.Ask<string>("[bold green]Enter ProductID: [/]");
            if (int.TryParse(input, out productId) && productId > 0)
            {
            break;
            }
            else
            {
            AnsiConsole.MarkupLine("[bold red]Invalid, please try again![/]");
            }
        }
        var quantity = AnsiConsole.Ask<int>("[bold green]Enter quantity: [/]");

        using var db = new ApplicationDbContext();
        var product = db.Products.Find(productId);
        if (product == null)
        {
            AnsiConsole.MarkupLine("[bold red]Product not found, press any key to back[/]");
            Console.ReadKey();
            return;
        }

        var cart = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == user.Id);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = user.Id,
                ProductId = productId,
                Quantity = quantity,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Carts.Add(cart);
        }
        else
        {
            cart.Quantity += quantity;
            cart.UpdatedAt = DateTime.Now;
        }
        var confirm = AnsiConsole.Confirm("[bold green]Do you want to add this product to your cart?[/]");
        if (confirm)
        {
            db.SaveChanges();
            AnsiConsole.Markup("[bold green]Product added to cart successfully, press any key to back[/]");
            Console.ReadKey();
        }
        else
        {
            db.Carts.Remove(cart);
            db.SaveChanges();
            AnsiConsole.Markup("[bold yellow]Product not added to cart, press any key to back[/]");
            Console.ReadKey();
        }
    }

    //Show cart
    public static void ShowCart()
    {
        Console.Clear();
        using var db = new ApplicationDbContext();
        var carts = db.Carts.Where(c => c.UserId == user.Id).ToList();
        if (carts.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold green]Cart is empty!!\n[/]");
            return;
        }
        var table = new Table();
        table.AddColumn("[bold]ID[/]");
        table.AddColumn("[bold]Product Name[/]");
        table.AddColumn("[bold]Quantity[/]");
        table.AddColumn("[bold]Stock[/]");
        table.AddColumn("[bold]Price[/]");
        table.AddColumn("[bold]Total[/]");
        decimal total = 0;
        foreach (var cart in carts)
        {
            var product = db.Products.Find(cart.ProductId);
            table.AddRow(
                cart.Id.ToString(),
                product.Name,
                cart.Quantity.ToString(),
                product.Stock.ToString(),
                product.Price.ToString("c0"),
                (cart.Quantity * product.Price).ToString("c0")
            );
            total += cart.Quantity * product.Price;
        }
        table.Expand();
        var TotalAmount = new Table(){
            Border = TableBorder.None,
        };
        TotalAmount.AddColumn($"[bold]Total Amount[/] [bold green]{total.ToString("c0")}[/]");
        TotalAmount.Expand();
        var mainTable = new Table()
        {
            Title = new TableTitle("[bold green]Your cart[/]")
        };
        mainTable.AddColumn(new TableColumn(table));
        mainTable.AddRow(TotalAmount).LeftAligned();
        mainTable.Expand();
        AnsiConsole.Render(mainTable);
    }

    //Cart controller
    public static void CartController()
    {
        if(user == null)
        {
            while (true)
            {
                Menu.RequireLoginMenu();
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Login();
                        return;
                    case "2":
                        Register();
                        return;
                    case "0":
                        return;
                    default:
                        AnsiConsole.Markup("[bold yellow]Invalid choice, press any key to back[/]");
                        Console.ReadKey();
                        break;
                }
            }
        }
        else
        {
            Console.Clear();
            ShowCart();
            using var db = new ApplicationDbContext();
            var carts = db.Carts.Where(c => c.UserId == user.Id).ToList();
            if (carts.Count == 0)
            {
                AnsiConsole.Markup("[bold yellow]-> Press [/][bold green]ESC[/][bold yellow] to go back[/]");
            }
            else
            {
                AnsiConsole.Markup("[bold]Press [/][bold red]CTRL + R[/] [bold]to remove a product from the cart[/]\n");
                AnsiConsole.Markup("[bold]Press[/] [bold green]CTRL + E[/][bold] to edit cart[/]\n");
                AnsiConsole.Markup("[bold]Press[/][bold green] CTRL + B [/][bold]to checkout[/]\n");
                AnsiConsole.Markup("[bold]Press[/] [bold yellow]ESC[/][bold] to exit[/]\n");
            }

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.R:
                            RemoveFromCart();
                            break;
                        case ConsoleKey.B:
                            Checkout();
                            break;
                        case ConsoleKey.E:
                            UpdateCart();
                            break;
                        default:
                            break;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return;
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
    private static void Checkout()
    {
        Console.Clear();
        using var db = new ApplicationDbContext();
        var carts = db.Carts.Where(c => c.UserId == user.Id).ToList();
        if (carts.Count == 0)
        {
            AnsiConsole.Markup("[bold red]Empty cart[/]");
            return;
        }
        var order = new Order
        {
            UserId = user.Id,
            TotalAmount = 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        db.Orders.Add(order);
        db.SaveChanges();
        decimal total = 0;
        foreach (var cart in carts)
        {
            var product = db.Products.Find(cart.ProductId);
            var orderDetail = new OrderItem
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = cart.Quantity,
                Price = product.Price,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.OrderItems.Add(orderDetail);
            total += cart.Quantity * product.Price;
        }
        order.TotalAmount = total;
        // Displey cart detail
        var table = new Table()
        {
            Title = new TableTitle("[bold green]Cart Detail[/]")
        };
        table.AddColumn("[bold]Product Name[/]");
        table.AddColumn("[bold]Quantity[/]");
        table.AddColumn("[bold]Price[/]");
        table.AddColumn("[bold]Total Amount[/]");
        foreach (var cart in carts)
        {
            var product = db.Products.Find(cart.ProductId);
            table.AddRow(
                product.Name,
                cart.Quantity.ToString(),
                product.Price.ToString("c0"),
                (cart.Quantity * product.Price).ToString("c0")
            );
        }
        table.Expand();
        var totalAmount = new Table()
        {
            Border = TableBorder.None,
        };
        totalAmount.AddColumn($"[bold]Total Amount[/] [bold green]{total.ToString("c0")}[/]");
        totalAmount.Expand();
        var mainTable = new Table()
        {
            Title = new TableTitle("[bold green]Order Detail[/]")
        };
        mainTable.AddColumn(new TableColumn(table));
        mainTable.AddRow(totalAmount).LeftAligned();
        mainTable.Expand();
        AnsiConsole.Render(mainTable);
        // Display user information
        var userInfo = new Table()
        {
            Title = new TableTitle("[bold green]User Information[/]")
        };
        userInfo.AddColumn("[bold]Full Name[/]");
        userInfo.AddColumn("[bold]Email[/]");
        userInfo.AddColumn("[bold]Address[/]");
        userInfo.AddRow(user.FullName, user.Email, user.Address);
        userInfo.Expand();
        AnsiConsole.Render(userInfo);
        AnsiConsole.MarkupLine("[bold green]Do you want to checkout? ([/][bold yellow]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "Y")
        {
            // Check stock before checkout
            foreach (var cart in carts)
            {
                var product = db.Products.Find(cart.ProductId);
                if (cart.Quantity > product.Stock)
                {
                    AnsiConsole.MarkupLine("[bold red]Checkout failed, product out of stock, Please remove the product or change the quantity to place an order[/]");
                    AnsiConsole.MarkupLine("[bold yellow]Press any key to back[/]");
                    Console.ReadKey();
                    break;
                }
                if (product != null)
                {
                    product.Stock -= cart.Quantity;
                    db.Products.Update(product);
                    db.Carts.RemoveRange(carts);
                    db.SaveChanges();
                    AnsiConsole.MarkupLine("[bold green]Checkout successful, press any key to continue[/]");
                    Console.ReadKey();
                }

            }
        }
        else if (confirmation.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Checkout cancelled, press any key to continue[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Invalid choice, press any key to back[/]");
            Console.ReadKey();
            return;
        }
        MenuController.CustomerManagementMenu();
    }

    public static void RemoveFromCart()
    {
        int cartId;
        while (true)
        {
            AnsiConsole.Markup("\n[bold green]Enter the cart ID to remove: [/]");
            string cartIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cartIdInput))
            {
            if (int.TryParse(cartIdInput, out cartId))
            {
                if (cartId > 0)
                {
                break;
                }
                else
                {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid cart ID.[/]\n");
                }
            }
            else
            {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid cart ID.[/]\n");
            }
            }
            else
            {
            AnsiConsole.Markup("[bold red]Cart ID cannot be null or empty. Please enter a valid cart ID.[/]\n");
            }
        }
        using var db = new ApplicationDbContext();
        var cart = db.Carts.Find(cartId);
        if (cart == null)
        {
            AnsiConsole.Markup("[bold red]No matching cart ID found, press any key to back\n[/]");
            Console.ReadKey();
            CartController();
            return;
        }
        db.Carts.Remove(cart);
        db.SaveChanges();
        AnsiConsole.Markup("[bold green]Cart removed successfully, press any key to continue[/]");
        Console.ReadKey();
        CartController();
    }
    public static void UpdateCart()
    {
        ShowCart();
        int cartId;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the cart ID to update: [/]");
            string cartIdInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cartIdInput))
            {
            if (int.TryParse(cartIdInput, out cartId))
            {
                if (cartId > 0)
                {
                break;
                }
                else
                {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid cart ID.[/]\n");
                }
            }
            else
            {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid cart ID.[/]\n");
            }
            }
            else
            {
            AnsiConsole.Markup("[bold red]Cart ID cannot be null or empty. Please enter a valid cart ID.[/]\n");
            }
        }

        int quantity;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the new quantity: [/]");
            if (int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
            {
            break;
            }
            else
            {
            AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid quantity.[/]\n");
            }
        }
        using var db = new ApplicationDbContext();
        var cart = db.Carts.Find(cartId);
        if (cart == null)
        {
            AnsiConsole.Markup("[bold yellow]Cart ID not found, press any key to go back[/]");
            Console.ReadKey();
            CartController();
            return;
        }
        cart.Quantity = quantity;
        cart.UpdatedAt = DateTime.Now;
        db.SaveChanges();
        AnsiConsole.Markup("[bold green]Cart updated successfully, press any key to continue[/]");
        Console.ReadKey();
        CartController();
    }
    public static void ShowUserOrderedList()
    {   
        Console.Clear();
        using var db = new ApplicationDbContext();
        var orders = db.Orders.Where(o => o.UserId == user.Id).ToList();
        if (orders.Count == 0)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]No orders found!!\n[/]");
            AnsiConsole.MarkupLine("[bold green]Press any key to go back[/]");
            Console.ReadKey();
            return;
        }
        var table = new Table()
        {
            Title = new TableTitle("[bold green]Your Order List[/]"),
        };
        table.AddColumn("[bold]Oder ID[/]");
        table.AddColumn("[bold]Total Amount[/]");
        table.AddColumn("[bold]Status[/]");
        table.AddColumn("[bold]Created At[/]");
        table.AddColumn("[bold]Updated At[/]");
        foreach (var ordered in orders)
        {
            table.AddRow(
                ordered.Id.ToString(),
                ordered.TotalAmount.ToString("c0"),
                ordered.Status,
                ordered.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                ordered.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss")
            );
        }
        AnsiConsole.Render(table);
        AnsiConsole.Markup("[bold green]Enter the order ID to view details: [/]");
        var orderIdInput = Console.ReadLine();
        if (string.IsNullOrEmpty(orderIdInput))
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Order ID cannot be empty\n[/]");
            AnsiConsole.MarkupLine("[bold green]Press any key to go back[/]");
            Console.ReadKey();
            return;
        }
        if (!int.TryParse(orderIdInput, out int orderId))
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Invalid order ID\n[/]");
            AnsiConsole.MarkupLine("[bold green]Press any key to go back[/]");
            Console.ReadKey();
            return;
        }
        var order = db.Orders.Find(orderId);
        if (order == null)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Order not found\n[/]");
            AnsiConsole.MarkupLine("[bold green]Press any key to go back[/]");
            Console.ReadKey();
            return;
        }
        var orderDetail = db.OrderItems.Where(od => od.OrderId == orderId).ToList();
        if (orderDetail.Count == 0)
        {   Console.Clear();
            AnsiConsole.MarkupLine("[bold]Empty order!!\n[/]");
            AnsiConsole.MarkupLine("[bold green]Press any key to go back[/]");
            Console.ReadKey();
            return;
        }
        var orderDetailTable = new Table();
        orderDetailTable.AddColumn("[bold]Product Name[/]");
        orderDetailTable.AddColumn("[bold]Quantity[/]");
        orderDetailTable.AddColumn("[bold]Price[/]");
        orderDetailTable.AddColumn("[bold]Total Amount[/]");
        decimal total = 0;
        foreach (var od in orderDetail)
        {
            var product = db.Products.Find(od.ProductId);
            orderDetailTable.AddRow(
            product.Name,
            od.Quantity.ToString(),
            product.Price.ToString("c0"),
            (od.Quantity * product.Price).ToString("c0")
            );
            total += od.Quantity * product.Price;
        }
        orderDetailTable.Expand();
        // Total amount
        var totalAmount = new Table()
        {
            Border = TableBorder.None,
        };
        totalAmount.AddColumn($"[bold]Total Amount[/] [bold green]{total.ToString("c0")}[/]");
        // Main table to display order detail
        var mainTable = new Table()
        {
            Title = new TableTitle($"[bold green]OrderID {orderId} Detail[/]")
        };
        mainTable.AddColumn(new TableColumn(orderDetailTable));
        mainTable.AddRow(totalAmount).LeftAligned();
        mainTable.Expand();
        AnsiConsole.Render(mainTable);

        AnsiConsole.MarkupLine("[bold green]Press any key to go back[/]");
        Console.ReadKey();
    }
    public static void EditProfileController()
    {
        ShowProfile();
        AnsiConsole.MarkupLine("[bold green]Do you want to edit your profile? ([/][bold yellow]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "N")
        {
            return;
        }
        else if (confirmation.ToUpper() == "Y")
        {
            EditProfile();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Invalid choice, press any key to go back[/]");
            Console.ReadKey();
        }
    }

    private static void ShowProfile()
    {
        Console.Clear();
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("[bold yellow]WELLCOME TO PET SHOP[/]").Centered(),
        };
        Table userInfo = new Table()
        {
            Title = new TableTitle("[bold green]User Information[/]"),
        };
        userInfo.AddColumn("[bold]User ID[/]");
        userInfo.AddColumn("[bold]User Name[/]");
        userInfo.AddColumn("[bold]Full Name[/]");
        userInfo.AddColumn("[bold]Email[/]");
        userInfo.AddColumn("[bold]Address[/]");
        userInfo.AddRow(user.Id.ToString(), user.Username, user.FullName, user.Email, user.Address);
        AnsiConsole.Render(userInfo);
    }

    public static void EditProfile()
    {
        AnsiConsole.Markup("[bold green]Enter new name: [/]");
        string newName = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(newName))
        {
            Console.Clear();
            Console.WriteLine("Name cannot be empty. Please enter a new name.");
            AnsiConsole.Markup("[bold green]Enter new name: [/]");
            newName = Console.ReadLine();
        }
        user.FullName = newName;

        AnsiConsole.Markup("[bold green]Enter the new email: [/]");
        string newEmail = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(newEmail) || !IsValidEmail(newEmail))
        {
            if (string.IsNullOrWhiteSpace(newEmail))
            {
                AnsiConsole.MarkupLine("[bold yellow]Email cannot be empty. Please enter again.\n[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold yellow]Invalid email format. Please enter a valid email.\n[/]");
            }
            AnsiConsole.Markup("[bold green]Enter new email: [/]");
            newEmail = Console.ReadLine();
        }
        using var db = new ApplicationDbContext();
        var existingUser = db.Users.FirstOrDefault(u => u.Email == newEmail);
        if (existingUser != null)
        {
            Console.Clear();
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Email already exists. Please enter a different email.\n[/]");
            Console.ReadKey();
            return;
        }
        user.Email = newEmail;
        AnsiConsole.Markup("[bold green]Enter the new address: [/]");
        string newAddress = Console.ReadLine();
        while (string.IsNullOrEmpty(newAddress))
        {
            Console.Clear();
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Address cannot be empty. Please enter again.\n[/]");
            AnsiConsole.Markup("[bold green]Enter the new address: [/]");
            newAddress = Console.ReadLine();
        }
        user.Address = newAddress;
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to update your profile? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "Y")
        {
            db.Users.Update(user);
            db.SaveChanges();
            AnsiConsole.MarkupLine("[bold green]Profile updated successfully, press any key to continue[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold yellow]Profile update cancelled[/]");
            Console.ReadKey();
        }
    }
    //User logout function
    public static void Logout()
    {
        AnsiConsole.MarkupLine("[bold yellow]Are you sure you want to log out? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "Y")
        {
            user = null;
            AnsiConsole.MarkupLine("[bold green]Log out successful, press any key to continue[/]");
            Console.ReadKey();
        }
        else if (confirmation.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold yellow]Log out cancelled, press any key to continue[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Invalid choice, press any key to continue[/]");
            Console.ReadKey();
        }
    }

    // Validate email format
    private static bool IsValidEmail(string email)
    {
        // Use regular expression to validate email format
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        return Regex.IsMatch(email, pattern);
    }
}