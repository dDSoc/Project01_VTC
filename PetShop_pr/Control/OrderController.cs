using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class OderController
{
    public enum OrderStatus
    {
        Pending = 1,
        Completed = 2
    }
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
                    SearchOrder();//Call search order function
                    break;
                case "2":
                    ShowAllOrderList();//Call show all order list function
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

        // Loop until user enters a valid order ID
        while (true)
        {
            if (!DataValidator.ValidateID(input) || !int.TryParse(input, out orderId))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                AnsiConsole.Markup("[bold green]Enter order ID:[/]");
                input = Console.ReadLine();
                continue;
            }

            var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                AnsiConsole.Markup("[bold red]Order not found, Press any key to continue.[/]");
                Console.ReadKey();
                return;
            }
            else
            {
                UpdateOrderStatusByID(orderId); // Call update order status by ID function
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
            Border = TableBorder.Rounded,
            Title = new TableTitle($"[bold yellow]Page {currentPage}/{totalPages}[/]"),
        };
        table.AddColumn("[bold green]Order ID[/]");
        table.AddColumn("[bold green]Order date[/]");
        table.AddColumn("[bold green]Order status[/]");
        table.AddColumn("[bold green]Total amount[/]");
        Console.WriteLine();
        Console.WriteLine();

        // Loop to show order list in each page
        for (int i = (currentPage - 1) * pageSize; i < currentPage * pageSize && i < orders.Count; i++)
        {
            var order = orders[i];
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

        Console.WriteLine();
        // Show page navigation and action instruction for user
        AnsiConsole.MarkupLine("[bold]Press '[/][bold red]CTRL + P[/][bold]' for previous page, '[/][bold red]CTRL + N[/][bold]' for next page[/]");
        AnsiConsole.MarkupLine("[bold]Press '[/][bold green]CTRL + E[/][bold]' to update order status[/]");
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
                case ConsoleKey.E:
                    AnsiConsole.Markup("[bold green]Enter order ID to update status:[/]");
                    int orderId;
                    while (true)
                    {
                        string input = Console.ReadLine();
                        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out orderId))
                        {
                            AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                            AnsiConsole.Markup("[bold green]Enter order ID:[/]");
                            continue;
                        }
                        var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
                        if (order == null)
                        {
                            AnsiConsole.MarkupLine("[bold red]Order not found![/]");
                            AnsiConsole.Markup("[bold green]Press any key to continue...[/]");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            UpdateOrderStatusByID(orderId);
                            break;
                        }
                    }
                    break;
            }
        }
        else if (keyInfo.Key == ConsoleKey.Escape)
        {
            return;
        }
    }
}


    // Search order function
    private static void SearchOrder()
    {
        using var db = new ApplicationDbContext();
        AnsiConsole.Markup("[bold green]Enter order ID:[/]");
        string input = Console.ReadLine();
        int orderId;

        // Loop until user enters a valid order ID
        while (true)
        {
            if (!DataValidator.ValidateID(input) || !int.TryParse(input, out orderId))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid order ID![/]");
                AnsiConsole.Markup("[bold green]Enter order ID:[/]");
                input = Console.ReadLine();
                continue;
            }

            var order = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                AnsiConsole.Markup("[bold red]Order not found, press any key to continue.[/]");
                Console.ReadKey();
                return;
            }
            break;
        }

        ShowOrderDetail(orderId); // Call show order detail function
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
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update order status? ([/][bold green]Y[/]/[bold red]N[/])");
        string confirm = Console.ReadLine();
        if (confirm.ToUpper() == "Y")
        {
            UpdateOrderStatusByID(OrderID);
            AnsiConsole.Markup("[bold green] Updated successfully! Press any key to continue...[/]");
        }
        else if (confirm.ToUpper() == "N")
        {
            AnsiConsole.Markup("[bold green]Press any key to continue...[/]");
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

        // Display order details
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
        
        // Loop until user enters a valid order status
        OrderStatus newStatus;
        while (true)
        {
            AnsiConsole.Markup("[bold green]Enter new order status (1 for Pending, 2 for Completed):[/]");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int status) && Enum.IsDefined(typeof(OrderStatus), status))
            {
                newStatus = (OrderStatus)status;
                break;
            }
            AnsiConsole.MarkupLine("[bold red]Invalid input! Please enter 1 for Pending or 2 for Completed.[/]");
        }

        order.Status = newStatus.ToString();
        AnsiConsole.Markup("[bold yellow]Are you sure you want to update? ([/][bold green]Y[/]/[bold red]N[/])");
        string answer = Console.ReadLine();
        if (answer.ToLower() == "y")
        {
            db.SaveChanges();
            AnsiConsole.Markup("[bold green]Order status updated successfully! Press any key to continue.[/]");
            Console.ReadKey();
        }
    }
}