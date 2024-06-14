using System;
using Spectre.Console;

class Menu
{
    // Display the Welcome screen panel
    public static void Panel()
    {
         // Tạo tiêu đề lớn
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.Square,
            Padding = new Padding(1, 1),
        };

        // Hiển thị tiêu đề
        AnsiConsole.Write(panel);
    }

    //Display the default menu screen
    public static void DefaultMenu()
    {
        Console.Clear();
        // Display the large title
         var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("[yellow]Welcome to Pet Shop[/]").Centered(),
        };
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Search[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Login[/]"));
        table2.Expand();

        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] Register[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Exit[/]"));
        table4.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);

        AnsiConsole.Write(mainTable);
    }


    public static void CustomerMenu()
    {
        Console.Clear();
        // Tạo tiêu đề lớn
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("[bold yellow]WELLCOME TO PET SHOP[/]").Centered(),
        };
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Search[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Show odered list[/]"));
        table2.Expand();

        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] View cart[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]4.[/][bold] Your vouchers[/]"));
        table4.Expand();
        var table5 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table5.AddColumn(new TableColumn("[bold yellow]5.[/][bold] Edit your info[/]"));
        table5.Expand();
        var table6 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table6.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Log out[/]"));
        table6.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);
        mainTable.AddRow(table5);
        mainTable.AddRow(table6);

        AnsiConsole.Write(mainTable);
    }


    public static void CustomerMenu_Search()
    {
        Console.Clear();
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
        };
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn(new TableColumn("[bold green]Search Product[/]").Centered()).BorderColor(Color.Green);
        table.Expand();
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Search by name[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Search by price[/]"));
        table2.Expand();

        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] Search by category[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Back[/]"));
        table4.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table);
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);

        AnsiConsole.Write(mainTable);

        AnsiConsole.MarkupLine("[bold green]Enter your choice:[/]");

    }


    public static void StoreManagerMenu()
    {
        Console.Clear();
        // Tạo tiêu đề lớn
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("[bold yellow]MANAGER[/]").Centered(),
        };
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn(new TableColumn("[bold yellow]Store manager[/]").Centered()).BorderColor(Color.Yellow);
        table.Expand();
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Product manager[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Category Manager[/]"));
        table2.Expand();
        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] Order manager[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]4.[/][bold] Dashboard[/]"));
        table4.Expand();
        var table5 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table5.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Log out[/]"));
        table5.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table);
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);
        mainTable.AddRow(table5);
        AnsiConsole.Write(mainTable);
    }

    public static void ShopOwnerMenu()
    {
        Panel();

        // Hiển thị thông tin đã nhập
        var table = new Table();
        table.AddColumn(new TableColumn("Shop owner menu").LeftAligned());
        table.AddRow($"[bold yellow]1.[/] [bold]View dasboard[/] ");
        table.AddRow($"[bold yellow]2.[/] [bold]Exit[/] ");


        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("[bold green]Enter your choice:[/]");
    }
    public static void LoginMenu()
    {
        Console.Clear();
        // Display the large title
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.Square,
            Padding = new Padding(1, 1),
            Header = new PanelHeader("[bold yellow]LOGIN[/]"),
        };

        AnsiConsole.Write(panel);
    }
    // Display the register menu
    public static void RequireLoginMenu()
    {
        Console.Clear();
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
        };
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold red]You are not logged in.[/]").Centered()).BorderColor(Color.Red);
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]1. Log in[/]"));
        table2.Expand();
        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        
        table3.AddColumn(new TableColumn("[bold yellow]2. Register a new account[/]"));
        table3.Expand();

        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]0. Cancel[/]"));
        table4.Expand();


        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);

        AnsiConsole.Write(mainTable);
        AnsiConsole.Markup("[bold green]Enter your choice: [/]\n");
    }

    // Display the product management menu
    public static void ProductManagementMenu()
    {
        Console.Clear();
        // Display the large title
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
        };
        panel.Expand();
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn(new TableColumn("[bold yellow]Product manager[/]").Centered()).BorderColor(Color.Yellow);
        table.Expand();
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Add product[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Edit product[/]"));
        table2.Expand();
        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Back[/]"));
        table3.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table);
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.Expand();
        AnsiConsole.Write(mainTable);
    }
    // Display the product management menu
    public static void ProductEditMenu(int ProductId)
    {
        Console.Clear();
        // Display the large title
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
        };
        panel.Expand();
        var tableProductInfo = ProductController.ShowProducById(ProductId);
        tableProductInfo.Expand();
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn(new TableColumn($"[bold yellow]Edit productID {ProductId}[/]").Centered()).BorderColor(Color.Yellow);
        table.Expand();
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Edit product name[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Edit product price[/]"));
        table2.Expand();
        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] Edit product description[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]4.[/][bold] Edit product stock[/]"));
        table4.Expand();
        var table5 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table5.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Back[/]"));
        table5.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table);
        mainTable.AddRow(tableProductInfo);
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);
        mainTable.AddRow(table5);
        mainTable.Expand();
        AnsiConsole.Write(mainTable);
    }

    // Display the Category management menu
    public static void CategoryManagementMenu()
    {
        Console.Clear();
        // Display the large title
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
        };
        panel.Expand();
        AnsiConsole.Write(panel);
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn(new TableColumn("[bold yellow]Category manager[/]").Centered()).BorderColor(Color.Yellow);
        table.Expand();
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Add category[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Show Category list[/]"));
        table2.Expand();
        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] Update category[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Back[/]"));
        table4.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table);
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);
        mainTable.Expand();
        AnsiConsole.Write(mainTable);
    }

    // Display the Order management menu
    public static void OrderManagementMenu()
    {
        Console.Clear();
        // Display the large title
        var panel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.None,
            Padding = new Padding(1, 1, 1, 1),
        };
        panel.Expand();
        var table = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table.AddColumn(new TableColumn("[bold yellow]Order manager[/]").Centered()).BorderColor(Color.Yellow);
        table.Expand();
        var table1 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table1.AddColumn(new TableColumn("[bold yellow]1.[/][bold] Search order[/]"));
        table1.Expand();
        var table2 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table2.AddColumn(new TableColumn("[bold yellow]2.[/][bold] Show order list[/]"));
        table2.Expand();
        var table3 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table3.AddColumn(new TableColumn("[bold yellow]3.[/][bold] Update order status[/]"));
        table3.Expand();
        var table4 = new Table()
        {
            Border = TableBorder.Rounded,
        };
        table4.AddColumn(new TableColumn("[bold yellow]0.[/][bold] Back[/]"));
        table4.Expand();
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn(panel));
        mainTable.AddRow(table);
        mainTable.AddRow(table1);
        mainTable.AddRow(table2);
        mainTable.AddRow(table3);
        mainTable.AddRow(table4);
        mainTable.Expand();
        AnsiConsole.Write(mainTable);
    }
}