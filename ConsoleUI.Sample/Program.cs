using ConsoleUI.Menu;

var consoleMenu = new ConsoleMenu
{
    Header = "Main Menu",
    IsMenuLooped = true,
    PrintItemNumber = true
};

consoleMenu.AddItems(
    new SimpleConsoleMenuItem("Option 1", Option1Handler),
    new SimpleConsoleMenuItem("Option 2"),
    new SimpleConsoleMenuItem("Option 3"),
    new SimpleConsoleMenuItem("Option 4")
    );

consoleMenu.ShowDialog();



static void Option1Handler()
{
    var scrollConsole = new ScrollConsoleMenu
    {
        Header = "Menu with a lot of items",
        IsMenuLooped = true,
        PrintItemNumber = true
    };

    for (int i = 1; i <= 30; i++)
    {
        scrollConsole.AddItems(
            new SimpleConsoleMenuItem($"Option { i }")
            );
    }

    scrollConsole.AddEventHandler(ConsoleKey.Backspace, scrollConsole.CloseDialog);
    
    scrollConsole.ShowDialog();
}