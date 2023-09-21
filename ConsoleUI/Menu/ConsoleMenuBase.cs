namespace ConsoleUI.Menu;

public abstract class ConsoleMenuBase
{
    public string? Header { get; set; }
    public bool IsMenuLooped { get; set; }
    public bool PrintItemNumber { get; set; }
    public virtual IConsoleMenuItem SelectedItem { get; protected set; }

    protected readonly IList<IConsoleMenuItem> _items;
    protected int _currentMenuItemIndex;
    protected bool _isOpen;

    protected IDictionary<ConsoleKey, Action> CustomEvents { get; set; }


    protected ConsoleMenuBase()
    {
        _items = new List<IConsoleMenuItem>();
        CustomEvents = new Dictionary<ConsoleKey, Action>();
    }

    public virtual bool AddEventHandler(ConsoleKey key, Action action)
    {
        return CustomEvents.TryAdd(key, action);
    }

    public virtual void AddItems(IConsoleMenuItem item)
    {
        _items.Add(item);
    }

    public virtual void AddItems(params IConsoleMenuItem[] items)
    {
        foreach (var item in items)
        {
            _items.Add(item);
        }
    }

    public virtual void AddItems(IEnumerable<IConsoleMenuItem> items)
    {
        foreach (var item in items)
        {
            _items.Add(item);
        }
    }

    public virtual void ClearItems()
    {
        _currentMenuItemIndex = 0;
        _items.Clear();
    }

    public abstract void ShowDialog();

    public abstract void CloseDialog();
}
