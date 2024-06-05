using System;
using System.Collections.Generic;
using Spectre.Console;

public static class CartController
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
        using var db = new ApplicationDbContext();
        var carts = db.Carts.Where(c => c.UserId == user.Id).ToList();
        if (carts.Count == 0)
        {
            Console.WriteLine("Giỏ hàng trống");
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
}