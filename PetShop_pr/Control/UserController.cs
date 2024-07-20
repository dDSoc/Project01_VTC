using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using Spectre.Console;
using System.Text.RegularExpressions;

public static class UserController
{
    // User object to store the current user information
    public static User user;

    //User login
    public static void Login()
    {
        bool loginSuccess = false;
        do
        {
            Console.Clear();
            Menu.LoginMenu(); // Display login menu
            using (var context = new ApplicationDbContext())
            {
                // Get login information from the user
                var username = AnsiConsole.Ask<string>("[bold Aqua]->[/] [bold Aqua]Username: [/] ");
                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold Aqua]->[/] [bold Aqua]Password: [/] ")
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
                        UserController.user = user; // Assign the user to the user object
                        loginSuccess = true;
                        // Check user role
                        switch (user.Role.ToLower())
                        {
                            case "customer":
                                MenuController.CustomerManagementMenu(); // Call customer management menu
                                break;
                            case "store_manager":
                                StoreManagerController.StoreManagementMenu(); // Call store management menu
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

        // Validate full name
        AnsiConsole.Markup("[bold green]Enter your fullname: [/]");
        string fullName = Console.ReadLine();
        while (!DataValidator.ValidateFullName(fullName))
        {
            AnsiConsole.MarkupLine("[bold red]Fullname cannot be empty or too long![/]");
            AnsiConsole.Markup("[bold green]Enter your fullname: [/]");
            fullName = Console.ReadLine();
        }
        user.FullName = fullName;

        // Validate username
        string username;
        User existingUser;
        do
        {
            username = AnsiConsole.Ask<string>("[bold green]Enter username: [/]");
            existingUser = db.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                AnsiConsole.MarkupLine("[bold red]Username already exists![/]");
            }
            else if (!DataValidator.ValidateUsername(username))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid username format! Username must be alphanumeric and between 5 and 15 characters.[/]");
            }
        } while (existingUser != null || !DataValidator.ValidateUsername(username));
        user.Username = username;

        // Validate email
        string email;
        do
        {
            email = AnsiConsole.Ask<string>("[bold green]Enter email: [/]");
            existingUser = db.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                AnsiConsole.MarkupLine("[bold red]Email already exists![/]");
            }
            else if (!DataValidator.ValidateEmail(email))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid email format![/]");
            }
        } while (existingUser != null || !DataValidator.ValidateEmail(email));
        user.Email = email;

        // Validate address
        AnsiConsole.Markup("[bold green]Enter address: [/]");
        string address = Console.ReadLine();
        while (!DataValidator.ValidateAddress(address))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid address.[/]");
            AnsiConsole.Markup("[bold green]Enter address: [/]");
            address = Console.ReadLine();
        }
        user.Address = address;

        // Validate passwords
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

        // Confirm registration
        string confirm = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold yellow]Are you sure you want to register? ([/][bold green]Y[/]/[bold red]N[/])")
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
            AnsiConsole.Markup("[bold yellow]Registration cancelled, press any key to continue[/]");
            Console.ReadKey();
            return; // Don't register and return to main menu
        }

        db.Users.Add(user);
        db.SaveChanges();
        UserController.user = user;
        AnsiConsole.Markup("[bold green]Registration successful, press any key to continue[/]");
        Console.ReadKey();
        MenuController.CustomerManagementMenu(); // Then go to customer management menu
    }

    //Add product to cart
    public static void AddToCart()
    {
        if (user == null)
        {
            while (true)
            {
                Menu.RequireLoginMenu(); // Show require login menu if user not login
                string choice = Console.ReadLine();
                // Switch case for require login menu
                switch (choice)
                {
                    case "1":
                        Login(); // Call login function
                        return;
                    case "2":
                        Register(); // Call register function
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
        // Get product ID from the user
        while (true)
        {
            var input = AnsiConsole.Ask<string>("[bold green]Enter ProductID: [/]");
            if (DataValidator.ValidateID(input) && int.TryParse(input, out productId) && productId > 0)
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Invalid, please try again![/]");
            }
        }

        int quantity;
        while (true)
        {
            var input = AnsiConsole.Ask<string>("[bold green]Enter quantity: [/]");
            if (DataValidator.ValidateQuantity(input) && int.TryParse(input, out quantity))
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Invalid quantity, please try again![/]");
            }
        }

        using var db = new ApplicationDbContext();
        var product = db.Products.Find(productId);
        if (product == null)
        {
            AnsiConsole.Markup("[bold red]Product not found, press any key to back[/]");
            Console.ReadKey();
            return;
        }

        var cart = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == user.Id);
        // Check if the product is already in the cart
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

        // Confirm adding product to cart
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

    //Show cart function
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
        // Loop to show cart detail
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
                Menu.RequireLoginMenu();//Show require login menu if user not login
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Login();//Call login function
                        return;
                    case "2":
                        Register();//Call register function
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
            ShowCart(); // Show cart detail
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
                            RemoveFromCart();//Call remove from cart function
                            break;
                        case ConsoleKey.B:
                            Checkout();//Call checkout function
                            break;
                        case ConsoleKey.E:
                            UpdateCart();//Call update cart function
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
    //Checkout function
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

        decimal total = 0;
        foreach (var cart in carts)
        {
            var product = db.Products.Find(cart.ProductId);
            total += cart.Quantity * product.Price;
        }

        // Display cart detail
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

        // Confirm checkout order
        AnsiConsole.Markup("[bold green]Do you want to checkout? ([/][bold yellow]Y[/]/[bold red]N[/])");
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
                    return;
                }
            }

            var order = new Order
            {
                UserId = user.Id,
                TotalAmount = total,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Orders.Add(order);
            db.SaveChanges();

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

                product.Stock -= cart.Quantity;
                db.Products.Update(product);
            }

            db.Carts.RemoveRange(carts);
            db.SaveChanges();

            AnsiConsole.Markup("[bold green]Checkout successful, press any key to continue[/]");
            Console.ReadKey();
        }
        else if (confirmation.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Checkout cancelled, press any key to continue[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Invalid choice, press any key to back[/]");
            Console.ReadKey();
        }

        MenuController.CustomerManagementMenu(); // Call customer management menu
    }



    //Remove from cart function
    public static void RemoveFromCart()
    {
        int cartId; 
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the cart ID to remove: [/]");
            string cartIdInput = Console.ReadLine();

            if (DataValidator.ValidateID(cartIdInput) && int.TryParse(cartIdInput, out cartId) && cartId > 0)
            {
                break;
            }
            else
            {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid cart ID.[/]\n");
            }
        }

        using var db = new ApplicationDbContext();
        var cart = db.Carts.Find(cartId);
        if (cart == null)
        {
            AnsiConsole.Markup("[bold red]No matching cart ID found, press any key to back\n[/]");
            Console.ReadKey();
            CartController(); // Call cart controller
            return;
        }

        db.Carts.Remove(cart);
        db.SaveChanges();
        AnsiConsole.Markup("[bold green]Cart removed successfully, press any key to continue[/]");
        Console.ReadKey();
        CartController(); // Call cart controller
    }

    //Update cart function
    public static void UpdateCart()
    {
        ShowCart(); // Show cart detail
        int cartId;
        Cart cart = null;

        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the cart ID to update: [/]");
            string cartIdInput = Console.ReadLine();

            if (DataValidator.ValidateID(cartIdInput) && int.TryParse(cartIdInput, out cartId) && cartId > 0)
            {
                using var db = new ApplicationDbContext();
                cart = db.Carts.Find(cartId);
                if (cart != null)
                {
                    break;
                }
                else
                {
                    AnsiConsole.Markup("[bold red]Cart ID not found, press any key to go back[/]");
                    Console.ReadKey();
                    CartController(); // Call cart controller
                    return;
                }
            }
            else
            {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid cart ID.[/]\n");
            }
        }

        int quantity;
        // Get the new quantity from the user
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter the new quantity: [/]");
            string quantityInput = Console.ReadLine();

            if (DataValidator.ValidateQuantity(quantityInput) && int.TryParse(quantityInput, out quantity) && quantity > 0)
            {
                break;
            }
            else
            {
                AnsiConsole.Markup("[bold red]Invalid input. Please enter a valid quantity.[/]\n");
            }
        }

        using var dbUpdate = new ApplicationDbContext();
        var cartToUpdate = dbUpdate.Carts.Find(cartId);
        cartToUpdate.Quantity = quantity;
        cartToUpdate.UpdatedAt = DateTime.Now;
        dbUpdate.SaveChanges();
        AnsiConsole.Markup("[bold green]Cart updated successfully, press any key to continue[/]");
        Console.ReadKey();
        CartController(); // Call cart controller
    }

    //Show user ordered list
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

    int pageSize = 5;
    int currentPage = 1;
    int totalPages = (int)Math.Ceiling((double)orders.Count / pageSize);

    // Loop to show order list
    while (true)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[bold green]Found {orders.Count} orders[/]");
        var table = new Table()
        {
            Title = new TableTitle($"[bold green]Your Order List - Page {currentPage}/{totalPages}[/]"),
        };
        table.AddColumn("[bold]Order ID[/]");
        table.AddColumn("[bold]Total Amount[/]");
        table.AddColumn("[bold]Status[/]");
        table.AddColumn("[bold]Created At[/]");
        table.AddColumn("[bold]Updated At[/]");
        Console.WriteLine();
        Console.WriteLine();

        // Loop to show order list in each page
        for (int i = (currentPage - 1) * pageSize; i < currentPage * pageSize && i < orders.Count; i++)
        {
            var order = orders[i];
            table.AddRow(
                order.Id.ToString(),
                order.TotalAmount.ToString("c0"),
                order.Status,
                order.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                order.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss")
            );
        }
        table.Expand();
        AnsiConsole.Render(table);

        Console.WriteLine();
        // Show page navigation and action instruction for user
        AnsiConsole.MarkupLine("[bold]Press '[/][bold red]CTRL + P[/][bold]' for previous page, '[/][bold red]CTRL + N[/][bold]' for next page[/]");
        AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + L[/][bold]' to view order details[/]");
        AnsiConsole.MarkupLine("[bold]Press [yellow]ESC[/] key to exit.[/]");

        // Read key input from user
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
                case ConsoleKey.L:
                    AnsiConsole.Markup("[bold green]Enter the order ID to view details: [/]");
                    var orderIdInput = Console.ReadLine();
                    if (string.IsNullOrEmpty(orderIdInput))
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[bold red]Order ID cannot be empty\n[/]");
                        AnsiConsole.Markup("[bold green]Press any key to go back[/]");
                        Console.ReadKey();
                        continue;
                    }
                    if (!int.TryParse(orderIdInput, out int orderId))
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[bold red]Invalid order ID\n[/]");
                        AnsiConsole.Markup("[bold green]Press any key to go back[/]");
                        Console.ReadKey();
                        continue;
                    }
                    var order = db.Orders.Find(orderId);
                    if (order == null)
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[bold yellow]Order not found\n[/]");
                        AnsiConsole.Markup("[bold green]Press any key to go back[/]");
                        Console.ReadKey();
                        continue;
                    }
                    var orderDetail = db.OrderItems.Where(od => od.OrderId == orderId).ToList();
                    if (orderDetail.Count == 0)
                    {   
                        Console.Clear();
                        AnsiConsole.MarkupLine("[bold]Empty order!!\n[/]");
                        AnsiConsole.Markup("[bold green]Press any key to go back[/]");
                        Console.ReadKey();
                        continue;
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

                    AnsiConsole.Markup("[bold green]Press any key to go back[/]");
                    Console.ReadKey();
                    break;
            }
        }
        else if (keyInfo.Key == ConsoleKey.Escape)
        {
            return;
        }
    }
}


    //Edit profile controller
    public static void EditProfileController()
    {
        ShowProfile(); // Show user profile
        // Confirm edit profile
        AnsiConsole.Markup("[bold green]Do you want to edit your profile? ([/][bold yellow]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "N")
        {
            return;
        }
        else if (confirmation.ToUpper() == "Y")
        {
            EditProfile(); // Call edit profile function
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Invalid choice, press any key to go back[/]");
            Console.ReadKey();
        }
    }
    //Show user profile
    private static void ShowProfile()
    {
        Console.Clear();

        using var db = new ApplicationDbContext();
        var currentUser = db.Users.FirstOrDefault(u => u.Id == user.Id);

        if (currentUser == null)
        {
            AnsiConsole.MarkupLine("[bold red]User not found! Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("[bold yellow]WELCOME TO PET SHOP[/]").Centered(),
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

        userInfo.AddRow(currentUser.Id.ToString(), currentUser.Username, currentUser.FullName, currentUser.Email, currentUser.Address);
        
        AnsiConsole.Render(userInfo);
    }

    
    //Edit profile function
    public static void EditProfile()
    {
        AnsiConsole.Markup("[bold green]Enter new name: [/]");
        string newName = Console.ReadLine();
        while (!DataValidator.ValidateFullName(newName)) // Validate name input
        {
            AnsiConsole.MarkupLine("[bold red]Name cannot be empty or too long. Please enter a new name.[/]");
            AnsiConsole.Markup("[bold green]Enter new name: [/]");
            newName = Console.ReadLine();
        }
        user.FullName = newName;

        AnsiConsole.Markup("[bold green]Enter the new email: [/]");
        string newEmail = Console.ReadLine();
        while (!DataValidator.ValidateEmail(newEmail)) // Validate email input
        {
            AnsiConsole.MarkupLine("[bold yellow]Invalid email format or empty email. Please enter a valid email.\n[/]");
            AnsiConsole.Markup("[bold green]Enter new email: [/]");
            newEmail = Console.ReadLine();
        }

        using var db = new ApplicationDbContext();
        var existingUser = db.Users.FirstOrDefault(u => u.Email == newEmail);
        if (existingUser != null)
        {
            AnsiConsole.MarkupLine("[bold red]Email already exists. Please enter a different email.\n[/]");
            Console.ReadKey();
            return;
        }
        user.Email = newEmail;

        AnsiConsole.Markup("[bold green]Enter the new address: [/]");
        string newAddress = Console.ReadLine();
        while (!DataValidator.ValidateAddress(newAddress)) // Validate address input
        {
            AnsiConsole.MarkupLine("[bold yellow]Address cannot be empty or too long. Please enter again.\n[/]");
            AnsiConsole.Markup("[bold green]Enter the new address: [/]");
            newAddress = Console.ReadLine();
        }
        user.Address = newAddress;

        AnsiConsole.Markup("[bold yellow]Are you sure you want to update your profile? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "Y")
        {
            db.Users.Update(user);
            db.SaveChanges();
            AnsiConsole.Markup("[bold green]Profile updated successfully, press any key to continue[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.Markup("[bold yellow]Profile update cancelled[/]");
            Console.ReadKey();
        }
    }

    //User logout function
    public static void Logout()
    {
        AnsiConsole.Markup("[bold yellow]Are you sure you want to log out? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirmation = Console.ReadLine();
        if (confirmation.ToUpper() == "Y")
        {
            user = null;
            AnsiConsole.Markup("[bold green]Log out successful, press any key to continue[/]");
            Console.ReadKey();
        }
        else if (confirmation.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold yellow]Log out cancelled, press any key to continue[/]");
            Console.ReadKey();
        }
        else
        {
            AnsiConsole.Markup("[bold red]Invalid choice, press any key to continue[/]");
            Console.ReadKey();
        }
    }
}