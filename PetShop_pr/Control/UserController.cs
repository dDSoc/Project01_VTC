using System;
using System.Collections.Generic;
using Spectre.Console;

public static class UserController
{
    public static User user;
    public static void AddToCart()
    {
        Console.WriteLine("Nhập ProductID: ");
        int productId = int.Parse(Console.ReadLine());
        Console.WriteLine("Nhập số lượng: ");
        int quantity = int.Parse(Console.ReadLine());
        using var db = new ApplicationDbContext();
        var product = db.Products.Find(productId);
        if (product == null)
        {
            Console.WriteLine("Không tìm thấy sản phẩm");
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
        db.SaveChanges();
    }
    public static void ShowCart()
    {
        Console.Clear();
        using var db = new ApplicationDbContext();
        var carts = db.Carts.Where(c => c.UserId == user.Id).ToList();
        if (carts.Count == 0)
        {
            Console.WriteLine("Giỏ hàng trống");
            Console.WriteLine("Nhấn phím bất kỳ để quay lại");
            Console.ReadKey();
            return;
        }
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Tên sản phẩm");
        table.AddColumn("Số lượng");
        table.AddColumn("Giá");
        table.AddColumn("Thành tiền");
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
        Console.WriteLine($"Tổng tiền: {total.ToString("c0")}");
        Console.WriteLine("Nhập 1 để xóa sản phẩm khỏi giỏ hàng");
        Console.WriteLine("Nhập 2 để thanh toán");
        Console.WriteLine("Nhập 0 để quay lại");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                RemoveFromCart();
                break;
            case "2":
                Checkout();
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Chức năng không tồn tại");
                Console.ReadKey();
                break;
        }
        Console.ReadKey();
    }

    private static void Checkout()
    {
        using var db = new ApplicationDbContext();
        var carts = db.Carts.Where(c => c.UserId == user.Id).ToList();
        if (carts.Count == 0)
        {
            Console.WriteLine("Giỏ hàng trống");
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
        Console.WriteLine("Đã thanh toán thành công");
    }

    public static void RemoveFromCart()
    {
        Console.WriteLine("Nhập ID sản phẩm cần xóa: ");
        int cartId = int.Parse(Console.ReadLine());
        using var db = new ApplicationDbContext();
        var cart = db.Carts.Find(cartId);
        if (cart == null)
        {
            Console.WriteLine("Không tìm thấy sản phẩm trong giỏ hàng");
            return;
        }
        db.Carts.Remove(cart);
        db.SaveChanges();
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
}