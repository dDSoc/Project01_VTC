using System;
using System.Collections.Generic;
using Spectre.Console;

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
            Menu.Panel();
            using (var context = new ApplicationDbContext())
            {
                // Get login information from the user
                var username = AnsiConsole.Ask<string>("[bold yellow]->[/] [bold]Username:[/] ");
                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold yellow]->[/] [bold]Password:[/] ")
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
                                // Thực hiện các hành động dành cho store manager
                                break;
                            case "shop_owner":
                                // Thực hiện các hành động dành cho shop owner
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
        user.FullName = AnsiConsole.Ask<string>("Enter full name: ");
        string email;
        User existingUser; // Declare the existingUser variable
        string username; // Declare the username variable
        do
        {
            username = AnsiConsole.Ask<string>("Enter username: ");
            existingUser = db.Users.FirstOrDefault(u => u.Username == username); // Assign the existingUser value
            if (existingUser != null)
            {
                AnsiConsole.Markup("[bold red]Username already exists![/]");
            }
        } while (existingUser != null);
        user.Username = username;
        do
        {
            email = AnsiConsole.Ask<string>("Enter email: ");
            existingUser = db.Users.FirstOrDefault(u => u.Email == email); // Assign the existingUser value
            if (existingUser != null)
            {
                AnsiConsole.Markup("[bold red]Email already exists![/]");
            }
        } while (existingUser != null);
        user.Email = email;
        user.Address = AnsiConsole.Ask<string>("Enter address: ");
        user.Password = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter password: ")
            .Secret());
        user.Role = "customer";
        db.Users.Add(user);
        db.SaveChanges();
        UserController.user = user;
        AnsiConsole.Markup("[bold green]Registration successful, press any key to continue[/]");
        Console.ReadKey();
        //Go to the customer management menu
        MenuController.CustomerManagementMenu();
    }

    //Add product to cart
    public static void AddToCart()
    {
        if (user == null)
        {
            Console.WriteLine("You are not logged in.");
            Console.WriteLine("1. Log in");
            Console.WriteLine("2. Register a new account");
            Console.WriteLine("0. Cancel");
            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    Register();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
        }

        var productId = AnsiConsole.Ask<int>("[bold green]Enter ProductID: [/]");
        var quantity = AnsiConsole.Ask<int>("[bold green]Enter quantity: [/]");

        using var db = new ApplicationDbContext();
        var product = db.Products.Find(productId);
        if (product == null)
        {
            AnsiConsole.Markup("[bold red]Product not found[/]");
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
            Console.WriteLine("Empty cart");
            return;
        }
        AnsiConsole.Markup("[bold green]Your cart \n[/]");
        var table = new Table();
        table.AddColumn("[bold]ID[/]");
        table.AddColumn("[bold]Product Name[/]");
        table.AddColumn("[bold]Quantity[/]");
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
                product.Price.ToString("c0"),
                (cart.Quantity * product.Price).ToString("c0")
            );
            total += cart.Quantity * product.Price;
        }
        AnsiConsole.Render(table);
        AnsiConsole.Markup($"[bold green]Total:[/] [bold green]{total.ToString("c0")}\n[/]");
    }

    //Cart controller
    public static void CartController()
    {
        if(user == null)
        {
            Console.WriteLine("You are not logged in.");
            Console.WriteLine("1. Log in");
            Console.WriteLine("2. Register a new account");
            Console.WriteLine("0. Back");
            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    Register();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
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
                AnsiConsole.Markup("[bold]Cart is empty, press ESC to go back[/]");
            }
            else
            {
                AnsiConsole.Markup("[bold]Press [/][bold red]CTRL + R[/] [bold]to remove a product from the cart[/]\n");
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
                        default:
                            break;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return;
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
            Console.ReadKey();
        }
    }
    private static void Checkout()
    {
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
        db.Carts.RemoveRange(carts);
        db.SaveChanges();
        Console.WriteLine("Đã thanh toán thành công, nhấn phím bất kỳ để tiếp tục");
        Console.ReadKey();
        MenuController.CustomerManagementMenu();
    }

    public static void RemoveFromCart()
    {
        Console.WriteLine("Enter the cart ID to remove: ");
        int cartId = int.Parse(Console.ReadLine());
        using var db = new ApplicationDbContext();
        var cart = db.Carts.Find(cartId);
        if (cart == null)
        {
            AnsiConsole.Markup("[bold red]No matching cart ID found[/]");
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
        Console.WriteLine("Nhập ID sản phẩm cần cập nhật: ");
        int cartId = int.Parse(Console.ReadLine());
        Console.WriteLine("Nhập số lượng mới: ");
        int quantity = int.Parse(Console.ReadLine());
        using var db = new ApplicationDbContext();
        var cart = db.Carts.Find(cartId);
        if (cart == null)
        {
            Console.WriteLine("Không tìm thấy sản phẩm trong giỏ hàng");
            return;
        }
        cart.Quantity = quantity;
        cart.UpdatedAt = DateTime.Now;
        db.SaveChanges();
    }
    public static void ShowUserOrderedList()
    {
        using var db = new ApplicationDbContext();
        var orders = db.Orders.Where(o => o.UserId == user.Id).ToList();
        if (orders.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("Không có đơn hàng nào");
            Console.WriteLine("Nhấn phím bất kỳ để quay lại");
            Console.ReadKey();
            return;
        }
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Tổng tiền");
        table.AddColumn("Trạng thái");
        table.AddColumn("Ngày tạo");
        table.AddColumn("Ngày cập nhật");
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
        Console.WriteLine("Nhập ID đơn hàng để xem chi tiết");
        var orderIdInput = Console.ReadLine();
        if (string.IsNullOrEmpty(orderIdInput))
        {
            Console.Clear();
            Console.WriteLine("ID đơn hàng không được để trống");
            Console.WriteLine("Nhấn phím bất kỳ để quay lại");
            Console.ReadKey();
            return;
        }
        if (!int.TryParse(orderIdInput, out int orderId))
        {
            Console.Clear();
            Console.WriteLine("ID đơn hàng không hợp lệ");
            Console.WriteLine("Nhấn phím bất kỳ để quay lại");
            Console.ReadKey();
            return;
        }
        var order = db.Orders.Find(orderId);
        if (order == null)
        {
            Console.Clear();
            Console.WriteLine("Không tìm thấy đơn hàng");
            Console.WriteLine("Nhấn phím bất kỳ để quay lại");
            Console.ReadKey();
            return;
        }
        var orderDetail = db.OrderItems.Where(od => od.OrderId == orderId).ToList();
        if (orderDetail.Count == 0)
        {   Console.Clear();
            Console.WriteLine("Đơn hàng trống");
            Console.WriteLine("Nhấn phím bất kỳ để quay lại");
            Console.ReadKey();
            return;
        }
        var orderDetailTable = new Table();
        orderDetailTable.AddColumn("Tên sản phẩm");
        orderDetailTable.AddColumn("Số lượng");
        orderDetailTable.AddColumn("Giá");
        orderDetailTable.AddColumn("Thành tiền");
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
        AnsiConsole.Render(orderDetailTable);
        Console.WriteLine($"Tổng tiền: {total.ToString("c0")}");
        Console.WriteLine("Nhấn phím bất kỳ để quay lại");
        Console.ReadKey();
    }
    public static void EditProfile()
    {
        Console.Clear();
        Table userInfo = new Table();
        userInfo.AddColumn("ID");
        userInfo.AddColumn("Tên");
        userInfo.AddColumn("Email");
        userInfo.AddColumn("Địa chỉ");
        userInfo.AddRow(user.Id.ToString(), user.FullName, user.Email, user.Address);
        AnsiConsole.Render(userInfo);
        Console.WriteLine("Nhập tên mới: ");
        string newName = Console.ReadLine();
        while (string.IsNullOrEmpty(newName))
        {
            Console.Clear();
            Console.WriteLine("Tên không được để trống. Vui lòng nhập lại.");
            Console.WriteLine("Nhập tên mới: ");
            newName = Console.ReadLine();
        }
        user.FullName = newName;

        Console.WriteLine("Nhập email mới: ");
        string newEmail = Console.ReadLine();
        while (string.IsNullOrEmpty(newEmail))
        {
            Console.Clear();
            Console.WriteLine("Email không được để trống. Vui lòng nhập lại.");
            Console.WriteLine("Nhập tên mới: ");
            newEmail = Console.ReadLine();
        }
        using var db = new ApplicationDbContext();
        var existingUser = db.Users.FirstOrDefault(u => u.Email == newEmail);
        if (existingUser != null)
        {
            Console.Clear();
            Console.WriteLine("Email đã tồn tại. Vui lòng nhập email khác.");
            Console.ReadKey();
            return;
        }
        user.Email = newEmail;
        Console.WriteLine("Nhập địa chỉ mới: ");
        string newAddress = Console.ReadLine();
        while (string.IsNullOrEmpty(newAddress))
        {
            Console.Clear();
            Console.WriteLine("Địa chỉ không được để trống. Vui lòng nhập lại.");
            Console.WriteLine("Nhập địa chỉ mới: ");
            newAddress = Console.ReadLine();
        }
        user.Address = newAddress;
        db.Users.Update(user);
        db.SaveChanges();
        Console.WriteLine("Cập nhật thông tin thành công");
        Console.WriteLine("Nhấn phím bất kỳ để quay lại");
        Console.ReadKey();
    }

    public static void Logout()
    {
        user = null;
        Console.Clear();
        AnsiConsole.Markup("[bold green]Logout successful[/]");
        AnsiConsole.Markup("[bold green]Press any key to continue[/]");
        Console.ReadKey();
    }
}