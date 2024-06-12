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
            Border = BoxBorder.Double,
            Padding = new Padding(1, 1),
        };

        // Hiển thị tiêu đề
        AnsiConsole.Write(panel);
    }

    public static void MainMenu()
    {
        Console.Clear();
        Panel();
    }

    //Display the default menu screen
    public static void DefaultMenu()
    {
        Console.Clear();
        // Display the large title
        var titlePanel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader("[bold yellow]WELLCOME TO PET SHOP[/]"),
            Padding = new Padding(1, 1),
        };

        // Display the menu table
        var menuTable = new Table();
        menuTable.AddColumn(new TableColumn("").LeftAligned());
        menuTable.AddRow(titlePanel);
        menuTable.AddRow("[bold yellow]1.[/][bold] Search[/]");
        menuTable.AddRow("[bold yellow]2.[/][bold] Login[/]");
        menuTable.AddRow("[bold yellow]3.[/][bold] Register[/]");
        menuTable.AddRow("[bold yellow]0.[/][bold] Exit[/]");

        AnsiConsole.Write(menuTable);
    }


    public static void CustomerMenu()
    {
        Console.Clear();
        // Tạo tiêu đề lớn
        var titlePanel = new Panel(new FigletText("PET SHOP").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader("[bold yellow]WELLCOME TO PET SHOP[/]"),
            Padding = new Padding(1, 1),
        };

        // Tạo bảng menu
        var menuTable = new Table();
        menuTable.AddColumn(new TableColumn("").LeftAligned());
        menuTable.AddRow(titlePanel);
        menuTable.AddRow("[bold yellow]1.[/][bold] Search[/]");
        menuTable.AddRow("[bold yellow]2.[/][bold] Show odered list[/]");
        menuTable.AddRow("[bold yellow]3.[/][bold] View cart[/]");
        menuTable.AddRow("[bold yellow]4.[/][bold] Your vouchers[/]");
        menuTable.AddRow("[bold yellow]5.[/][bold] Edit your info[/]");
        menuTable.AddRow("[bold yellow]0.[/][bold] Log out[/]");

        AnsiConsole.Write(menuTable);
    }

    public static void CustomerMenu_Search()
    {
        Console.Clear();
        // Hiển thị thông tin đã nhập
        var table = new Table();
        table.AddColumn(new TableColumn("Search menu").LeftAligned());

        table.AddRow($"[bold yellow]1.[/] [bold]Search by name[/] ");
        table.AddRow($"[bold yellow]2.[/] [bold]Search by price[/]");
        table.AddRow($"[bold yellow]3.[/] [bold]Search by category[/]");
        table.AddRow($"[bold yellow]0.[/] [bold]Back[/]");

        AnsiConsole.Write(table);

        Console.WriteLine("Enter your choice:");

    }

    public static void SubSearchMenu()
    {
        Console.Clear();
        // Tạo tiêu đề lớn
        var titlePanel = new Panel(new FigletText("LAPTOP STORE").Centered().Color(Color.Aqua))
        {
            Border = BoxBorder.Double,
            Padding = new Padding(1, 1),
        };

        // Tạo bảng menu
        var menuTable = new Table();
        menuTable.AddColumn(new TableColumn("").LeftAligned());
        menuTable.AddRow(titlePanel);
        menuTable.AddRow("[bold yellow]1.[/][bold] Search by name[/]");
        menuTable.AddRow("[bold yellow]2.[/][bold] Search by price[/]");
        menuTable.AddRow("[bold yellow]3.[/][bold] Search by category[/]");
        menuTable.AddRow("[bold yellow]4.[/][bold] Back[/]");

        AnsiConsole.Write(menuTable);
        var choice = AnsiConsole.Ask<string>("[yellow]Enter your choice: [/]");
    }

    public static void StoreManagerMenu()
    {
        Panel();

        // Hiển thị thông tin đã nhập
        var table = new Table();
        table.AddColumn(new TableColumn("Store manager menu").LeftAligned());
        table.AddRow($"[bold yellow]1.[/] [bold]Add new product[/] ");
        table.AddRow($"[bold yellow]2.[/] [bold]Edit product[/]");
        table.AddRow($"[bold yellow]3.[/] [bold]Delete product[/]");
        table.AddRow($"[bold yellow]4.[/] [bold]View product list[/]");

        AnsiConsole.Write(table);
        Console.WriteLine("Enter your choice:");
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
        Console.WriteLine("Enter your choice:");
    }
}