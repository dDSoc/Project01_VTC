using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class OderController
{
    // Order management menu function
    public static void ManagementOrder()
    {
        while (true)
        {   
            Menu.OrderManagementMenu();//Call order management menu
            AnsiConsole.Markup("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();
            // Switch case for order management menu
            switch (choice)
            {
                case "1":
                    SearhOrder();//Call search order function
                    break;
                case "2":
                    ShowAllOrderList();//Call show all order list function
                    AnsiConsole.MarkupLine("[bold green]Press any key to continue.[/]");
                    Console.ReadKey();
                    break;
                case "3":
                    UpdateOrderStatus();//Call update order status function
                    break;
                case "0":
                    StoreManagerController.StoreManagementMenu();//Call store management menu function to back to store management menu
                    break;
            }
        }
    }

    // Update order status function
    private static void UpdateOrderStatus()
    {
        using var db = new ApplicationDbContext();
        AnsiConsole.Markup("[bold green]Enter order ID:[/]");
        string input = Console.ReadLine();
        int orderId;
        // Loop until user enter a valid order ID
        while (true)
        {
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out orderId))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                AnsiConsole.Markup("[bold green]Enter order ID:[/]");
                input = Console.ReadLine();
                continue;
            }
            var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                AnsiConsole.MarkupLine("[bold red]Order not found, Press any key to continue.[/]");
                Console.ReadKey();
                return;
            }
            else
            {
                UpdateOrderStatusByID(orderId);//Call update order status by ID function
                break;
            }
        }
    }

    // Show all order list function
    private static void ShowAllOrderList()
    {
        using var db = new ApplicationDbContext();
        var orders = db.Orders.Include(o => o.OrderItems).ToList();
        // Check if there is no order
        if (orders.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No order found![/]");
            return;
        }
        // Display all orders
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn("[bold green]Order ID[/]");
        table.AddColumn("[bold green]Order date[/]");
        table.AddColumn("[bold green]Order status[/]");
        table.AddColumn("[bold green]Total amount[/]");
        foreach (var order in orders)
        {   
            var orderDetails = db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToList();
            decimal totalAmount = 0;
            foreach (var detail in orderDetails)
            {
                totalAmount += detail.Product.Price * detail.Quantity;
            }
            table.AddRow(order.Id.ToString(), order.CreatedAt.ToString(), order.Status, totalAmount.ToString());
        }
        table.Expand();
        AnsiConsole.Render(table);
        // Ask user if they want to update order status
        AnsiConsole.MarkupLine("[bold green]Do you want to update order status? (Y/N)[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            AnsiConsole.MarkupLine("[bold green]Enter order ID to update status:[/]");
            int orderId;
            while(true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out orderId))
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                    AnsiConsole.MarkupLine("[bold green]Enter order ID:[/]");
                    continue;
                }
                var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    AnsiConsole.MarkupLine("[bold red]Order not found![/]");
                    AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    UpdateOrderStatusByID(orderId);
                    break;
                }
            }
        }
        else if (confirm.ToUpper() == "N")
        {
            return;
        }
    }

    // Search order function
    private static void SearhOrder()
    {
        using var db = new ApplicationDbContext();
        AnsiConsole.Markup("[bold green]Enter order ID:[/]");
        string input = Console.ReadLine();
        int orderId;
        // Loop until user enter a valid order ID
        while (true)
        {
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out orderId))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                AnsiConsole.Markup("[bold green]Enter order ID:[/]");
                Console.ReadKey();
                return;
            }
            var order = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                AnsiConsole.MarkupLine("[bold red]Order not found, press any key to continue.[/]");
                Console.ReadKey();
                return;
            }
            break;
        }
        ShowOrderDetail(orderId);//Call show order detail function
    }

    // Show order detail function by order ID
    public static void ShowOrderDetail(int OrderID)
    {
        Console.Clear();
        using var db = new ApplicationDbContext();
        var order = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == OrderID);
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn("[bold green]Order ID[/]");
        table.AddColumn("[bold green]Order date[/]");
        table.AddColumn("[bold green]Order status[/]");
        table.AddRow(order.Id.ToString(), order.CreatedAt.ToString(), order.Status);
        table.Expand();

        var orderDetails = db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToList();
        var tableOrderDetails = new Table()
        {
            Border = TableBorder.Rounded,
        };
        tableOrderDetails.AddColumn("[bold green]Product name[/]");
        tableOrderDetails.AddColumn("[bold green]Quantity[/]");
        tableOrderDetails.AddColumn("[bold green]Price[/]");

        decimal totalAmount = 0;
        // Display order details in table
        foreach (var detail in orderDetails)
        {
            tableOrderDetails.AddRow(detail.Product.Name, detail.Quantity.ToString(), detail.Product.Price.ToString());
            totalAmount += detail.Product.Price * detail.Quantity;//Calculate total amount
        }
        tableOrderDetails.Expand();

        var tableTotalAmount = new Table()
        {
            Border = TableBorder.Rounded,
        };
        tableTotalAmount.AddColumn($"[bold green]Total amount: {totalAmount}[/]");
        tableTotalAmount.Expand();
        // Display order details
        var mainTable = new Table()
        {
            Title = new TableTitle($"[bold green]OrderID {OrderID} details[/]"),
        };
        mainTable.AddColumn(new TableColumn(table));
        mainTable.AddRow(tableOrderDetails);
        mainTable.AddRow(tableTotalAmount);
        mainTable.Expand();
        AnsiConsole.Render(mainTable);
        // Ask user if they want to update order status
        AnsiConsole.MarkupLine("[bold green]Do you want do update order status? (Y/N)[/]");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            UpdateOrderStatusByID(OrderID);
            AnsiConsole.MarkupLine("[bold green] Updated successfully! Press any key to continue...[/]");
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
            Console.ReadKey();
        }
    }

    // Update order status by order ID function
    private static void UpdateOrderStatusByID(int OrderID)
    {
        using var db = new ApplicationDbContext();
        var order = db.Orders.FirstOrDefault(o => o.Id == OrderID);
        if (order == null)
        {
            AnsiConsole.MarkupLine("[bold red]Order not found![/]");
            return;
        }

        //Display order details
        var orderDetails = db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == OrderID).ToList();
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn("[bold green]Product name[/]");
        table.AddColumn("[bold green]Quantity[/]");
        table.AddColumn("[bold green]Price[/]");
        table.AddColumn("[bold green]Status[/]");
        // Display order details in table
        decimal totalAmount = 0;
        foreach (var detail in orderDetails)
        {
            table.AddRow(detail.Product.Name, detail.Quantity.ToString(), detail.Product.Price.ToString(), order.Status);
            totalAmount += detail.Product.Price * detail.Quantity;
        }
        table.Expand();
        AnsiConsole.Render(table);
        string newStatus;
        // Loop until user enter a valid order status
        while (true)
        {
            AnsiConsole.MarkupLine("[bold green]Enter new order status:[/]");
            newStatus = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newStatus))
            {
            break;
            }
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter a valid order status.[/]");
        }
        order.Status = newStatus;
        AnsiConsole.MarkupLine("[bold green]Do you want to update the order status? (y/n)[/]");
        string answer = Console.ReadLine();
        if (answer.ToLower() == "y")
        {
            db.SaveChanges();
            AnsiConsole.MarkupLine("[bold green]Order status updated successfully! Press any key to continue...[/]");
            Console.ReadKey();
        }
    }
}