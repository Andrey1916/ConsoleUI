namespace ConsoleUI.Menu;

public abstract class ConsoleMenuItemBase<T> : IConsoleMenuItem
{
    protected ConsoleMenuItemBase(T content)
    {
        Content = content;
    }

    protected ConsoleMenuItemBase(T content, Action<T> action)
    {
        Content = content;
        Action = action;
    }

    public T Content { get; private set; }
    public Action<T>? Action { get; private set; }

    public virtual void ChooseItem()
        => Action?.Invoke(Content);

    public virtual string Render()
    {
        return Content?.ToString() ?? "null";
    }
}
