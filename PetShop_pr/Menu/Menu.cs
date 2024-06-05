using System;
using Spectre.Console;

class Menu
{
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
        menuTable.AddRow("[bold yellow]0.[/][bold] Back[/]");

        AnsiConsole.Write(menuTable);

        // Nhận lựa chọn của người dùng
        Console.WriteLine($"Enter your choice:");
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

    public static void TestMenu()
    {
        // Thông tin đơn hàng
        var orderInfoTable = new Table();
        orderInfoTable.AddColumn("Order ID");
        orderInfoTable.AddColumn("Customer Name");
        orderInfoTable.AddColumn("Customer Phone");
        orderInfoTable.AddColumn("Address");
        orderInfoTable.AddRow("7", "Ma Van Truong", "0901234567", "Bac Giang");

        // Thông tin sản phẩm
        var productTable = new Table();
        productTable.AddColumn("No");
        productTable.AddColumn("Laptop Name");
        productTable.AddColumn("Price(VND)");
        productTable.AddColumn("Quantity");
        productTable.AddColumn("Amount(VND)");
        productTable.AddRow("1", "ASUS TUF Gaming F15 FX506HC HN002T", "21,990,000", "1", "21,990,000");

        // Tổng thanh toán
        var totalPaymentTable = new Table();
        totalPaymentTable.AddColumn("TOTAL PAYMENT");
        totalPaymentTable.AddRow("21,990,000");

        // Tạo bảng lồng trong bảng
        var mainTable = new Table();
        mainTable.AddColumn(new TableColumn("Order Information").Centered().Header("[bold yellow]ORDER INFORMATION[/]"));
        mainTable.AddRow(orderInfoTable);
        mainTable.AddEmptyRow();
        mainTable.AddRow(productTable);
        mainTable.AddEmptyRow();
        mainTable.AddRow(totalPaymentTable);

        // Hiển thị bảng lồng nhau
        AnsiConsole.Write(mainTable);

        // Hiển thị hướng dẫn
        AnsiConsole.MarkupLine("[yellow]* Enter money to PAYMENT (enter 0 to skip) or Press combination CTRL + X to CANCEL ORDER or press 'ESC' to EXIT[/]");
        var money = AnsiConsole.Ask<string>("[yellow]Enter money: [/]");
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