using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class OderController
{
    public static void ManagementOrder()
    {
        while (true)
        {
            Menu.OrderManagementMenu();
            AnsiConsole.MarkupLine("[bold green]Enter your choice:[/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearhOrder();
                    break;
                case "2":
                    ShowOrderList();
                    AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
                    Console.ReadKey();
                    break;
                case "3":
                    // UpdateOrderStatus();
                    break;
                case "0":
                    StoreManagerController.StoreManagementMenu();
                    break;
            }
        }
    }

    private static void ShowOrderList()
    {
        using var db = new ApplicationDbContext();
        var orders = db.Orders.Include(o => o.OrderItems).ToList();
        foreach (var order in orders)
        {
            AnsiConsole.MarkupLine("[bold green]Order ID:[/]");
            AnsiConsole.MarkupLine(order.Id.ToString());
            AnsiConsole.MarkupLine("[bold green]Order date:[/]");
            AnsiConsole.MarkupLine(order.CreatedAt.ToString());
            AnsiConsole.MarkupLine("[bold green]Order status:[/]");
            AnsiConsole.MarkupLine(order.Status);
            AnsiConsole.MarkupLine("[bold green]Order details:[/]");
            foreach (var detail in order.OrderItems)
            {
                AnsiConsole.MarkupLine(detail.Product.Name + " x" + detail.Quantity + " = " + detail.Product.Price * detail.Quantity);
            }
            AnsiConsole.MarkupLine("[bold green]Total amount:[/]");
            AnsiConsole.MarkupLine(order.TotalAmount.ToString());
            AnsiConsole.MarkupLine("");
        }
    }

    private static void SearhOrder()
    {
        using var db = new ApplicationDbContext();
        AnsiConsole.MarkupLine("[bold green]Enter order ID:[/]");
        string input = Console.ReadLine();
        int orderId;
        while (true)
        {
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out orderId))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                AnsiConsole.MarkupLine("[bold green]Enter order ID:[/]");
                Console.ReadKey();
                return;
            }
            var order = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                AnsiConsole.MarkupLine("[bold red]Order not found![/]");
                AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }
            break;
        }
        ShowOrderDetail(orderId);

    }

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
        foreach (var detail in orderDetails)
        {
            tableOrderDetails.AddRow(detail.Product.Name, detail.Quantity.ToString(), detail.Product.Price.ToString());
            totalAmount += detail.Product.Price * detail.Quantity;
        }
        tableOrderDetails.Expand();

        var tableTotalAmount = new Table()
        {
            Border = TableBorder.Rounded,
        };
        tableTotalAmount.AddColumn($"[bold green]Total amount: {totalAmount}[/]");
        tableTotalAmount.Expand();

        var mainTable = new Table()
        {
            Title = new TableTitle($"[bold green]OrderID {OrderID} details[/]"),
        };
        mainTable.AddColumn(new TableColumn(table));
        mainTable.AddRow(tableOrderDetails);
        mainTable.AddRow(tableTotalAmount);
        mainTable.Expand();
        AnsiConsole.Render(mainTable);
        AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
        Console.ReadKey();
    }
}