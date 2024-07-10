using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class DashboardController
{   
    private enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public static void DashboardManagement()
    {
        while (true)
        {
            Menu.DashboardManagementMenu();
            AnsiConsole.Markup("[bold green]Enter your choice: [/]");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowDashboard();
                    break;
                case "2":
                    ShowBestSeller();
                    break;
                case "0":
                    StoreManagerController.StoreManagementMenu();
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold red]Invalid choice. Press any key to continue.[/]");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void ShowBestSeller()
    {   
        Console.Clear();
        var db = new ApplicationDbContext();
        var orderItem = db.OrderItems
            .Include(o => o.Product)
            .ToList();
        var bestSeller = new Dictionary<string, int>();
        foreach (var item in orderItem)
        {
            if (bestSeller.ContainsKey(item.Product.Name))
            {
                bestSeller[item.Product.Name] += item.Quantity;
            }
            else
            {
                bestSeller.Add(item.Product.Name, item.Quantity);
            }
        }
        bestSeller = bestSeller.OrderByDescending(x => x.Value).Take(5).ToDictionary(x => x.Key, x => x.Value);
        AnsiConsole.MarkupLine("[bold green]Top 5 best seller products[/]");
        var table = new Table()
            .Centered();
        table.AddColumn("[bold green]Product[/]");
        table.AddColumn("[bold green]Quantity[/]");
        foreach (var item in bestSeller)
        {
            table.AddRow(item.Key, item.Value.ToString());
        }
        table.Expand();
        AnsiConsole.Render(table);
        AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private static void ShowDashboard()
    {
        Console.Clear();
        string year = null;
        while (string.IsNullOrEmpty(year))
        {
            AnsiConsole.Markup("[bold green]Enter year:[/]");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int result))
            {
                year = input;
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input. Please enter a valid year.[/]");
            }
        }

        int yearInt = int.Parse(year);
        //Display revenue by month
        var panel1 = new Panel("")
            .Header("[bold green]Revenue by month[/]");
        panel1.Expand();
        AnsiConsole.Render(panel1);
        RevenueByMotnhOfYear(yearInt);
        //Display revenue by category
        var panel2 = new Panel("")
            .Header("[bold green]Revenue by category[/]");
        panel2.Expand();
        Console.WriteLine();
        Console.WriteLine();
        AnsiConsole.Render(panel2);
        RevenueByCategory(yearInt);
        Console.ReadKey();
    }

    private static void RevenueByMotnhOfYear(int yearInt)
    {
        var db = new ApplicationDbContext();
        var order = db.Orders
            .Where(o => o.CreatedAt.Year == yearInt)
            .ToList();
        var revenueByMonth = new Dictionary<Month, decimal>();
        for (Month month = Month.January; month <= Month.December; month++)
        {
            var revenue = order
            .Where(o => o.CreatedAt.Month == (int)month)
            .Sum(o => o.TotalAmount);
            revenueByMonth.Add(month, revenue);
        }
        var chart = new BarChart()
            .Width(100)
            .Label($"{yearInt} Sales")
            .CenterLabel();

        foreach (var item in revenueByMonth)
        {
            chart.AddItem($"", 0, Color.GreenYellow);
            chart.AddItem($"{item.Key}", (double)item.Value, Color.GreenYellow);
        }
        AnsiConsole.Render(chart);
    }

    private static void RevenueByCategory(int yearInt)
    {
        var db = new ApplicationDbContext();
        var orderItem = db.OrderItems
            .Include(o => o.Product)
            .ThenInclude(p => p.Category)
            .Include(o => o.Order)
            .Where(o => o.Order.CreatedAt.Year == yearInt)
            .ToList();
        var revenueByCategory = new Dictionary<string, decimal>();
        decimal totalRevenue = 0;
        foreach (var item in orderItem)
        {
            if (revenueByCategory.ContainsKey(item.Product.Category.Name))
            {
                revenueByCategory[item.Product.Category.Name] += item.Quantity * item.Product.Price;
                totalRevenue += item.Quantity * item.Product.Price;
            }
            else
            {
                revenueByCategory.Add(item.Product.Category.Name, item.Quantity * item.Product.Price);
                totalRevenue += item.Quantity * item.Product.Price;
            }
        }
        var table = new Table()
            .Centered();
        table.AddColumn("[bold green]Category[/]");
        table.AddColumn("[bold green]Revenue[/]");
        table.AddColumn("[bold green]Percentage[/]");
        foreach (var item in revenueByCategory)
        {
            table.AddRow(item.Key, item.Value.ToString("C"), ((item.Value / totalRevenue) * 100).ToString("0.00") + "%");
        }
        table.Expand();
        AnsiConsole.Render(table);
        AnsiConsole.MarkupLine("[bold green]Press any key to continue...[/]");
        Console.ReadKey();
    }
}
