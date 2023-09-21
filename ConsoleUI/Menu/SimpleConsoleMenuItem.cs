namespace ConsoleUI.Menu;

public sealed class SimpleConsoleMenuItem<T> : ConsoleMenuItemBase<T>
{
    public SimpleConsoleMenuItem(T content)
        : base(content)
    { }

    public SimpleConsoleMenuItem(T content, Action<T> action)
        : base(content, action)
    { }
}

public sealed class SimpleConsoleMenuItem : IConsoleMenuItem
{
    public SimpleConsoleMenuItem(string content)
    {
        Content = content;
    }

    public SimpleConsoleMenuItem(string content, Action action)
    {
        Content = content;
        Action = action;
    }

    public string Content { get; private set; }
    public Action? Action { get; private set; }

    public void ChooseItem()
        => Action?.Invoke();

    public string Render()
    {
        return Content;
    }
}
