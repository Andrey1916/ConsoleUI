using System.Text;

namespace ConsoleUI.Menu;

public class ScrollConsoleMenu : ConsoleMenuBase
{
    private int _firstItemIndexOnDisplay;
    private int _lastItemIndexOnDisplay;
    private int _headerSize;

    public override void ShowDialog()
    {
        _isOpen = true;

        Render(RenderAction.NeedRerender);

        while (true)
        {
            var renderAction = RenderAction.RerenderNoNeeded;

            var key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    _isOpen = false;
                    return;

                case ConsoleKey.UpArrow:
                    if (_items.Count > 0)
                    {
                        renderAction = SelectPreviousItem();
                    }

                    break;

                case ConsoleKey.DownArrow:
                    if (_items.Count > 0)
                    {
                        renderAction = SelectNextItem();
                    }

                    break;

                case ConsoleKey.Enter:
                    if (_items.Count > 0)
                    {
                        var currentItem = _items[_currentMenuItemIndex];
                        currentItem.ChooseItem();

                        renderAction = RenderAction.NeedRerender;
                    }

                    break;
            }

            SelectedItem = _items[_currentMenuItemIndex];

            if (CustomEvents is not null
                && CustomEvents.TryGetValue(key.Key, out var handler))
            {
                handler.Invoke();

                renderAction = RenderAction.NeedRerender;
            }

            if (!_isOpen)
            {
                Console.CursorVisible = true;
                return;
            }

            if (renderAction == RenderAction.RerenderNoNeeded)
            {
                continue;
            }

            Render(renderAction);
        }
    }

    public override void CloseDialog()
    {
        _isOpen = false;
    }

    protected virtual void Render(RenderAction renderAction)
    {
        Console.Clear();
        Console.CursorVisible = false;

        if (Header is not null)
        {
            RenderHeader();
        }

        _headerSize = Console.CursorTop;

        switch (renderAction)
        {
            case RenderAction.NeedRerender:
                RenderMenuItems();
                break;

            case RenderAction.NeedReversRerender:
                RenderMenuItemsRevers();
                break;
        }
    }

    protected virtual void RenderHeader()
    {
        Console.WriteLine($"--- {Header}\n");
    }

    protected virtual void RenderMenuItems()
    {
        int consoleWidth = Console.WindowWidth;
        int menuHeight = Console.WindowHeight - Console.CursorTop;
        int lineInUse = 0;

        for (int i = _firstItemIndexOnDisplay; ; ++i)
        {
            if (i == _items.Count)
            {
                return;
            }

            string item = RenderItem(i);

            int itemLength = item.Length;
            int linesCount = 1;

            while (itemLength > consoleWidth)
            {
                ++linesCount;
                itemLength -= consoleWidth;
            }

            if (linesCount + lineInUse >= menuHeight)
            {
                return;
            }

            _lastItemIndexOnDisplay = i;

            lineInUse += linesCount;

            if (i == _currentMenuItemIndex)
            {
                PrintSelectedItem(item);
            }
            else
            {
                Console.WriteLine(item);
            }
        }
    }

    protected void RenderMenuItemsRevers()
    {
        int consoleWidth = Console.WindowWidth;

        int menuHeight = Console.WindowHeight - Console.CursorTop - 1;
        int lineInUse = 0;

        string currentItem = string.Empty;
        var itemsToPrint = new LinkedList<string>();

        for (int i = _currentMenuItemIndex; ; --i)
        {
            if (i < 0)
            {
                break;
            }

            string item = RenderItem(i);

            int itemLength = item.Length;
            int linesCount = 1;

            while (itemLength > consoleWidth)
            {
                ++linesCount;
                itemLength -= consoleWidth;
            }

            if (linesCount + lineInUse > menuHeight)
            {
                break;
            }

            _firstItemIndexOnDisplay = i;
            lineInUse += linesCount;

            if (i == _currentMenuItemIndex)
            {
                currentItem = item;
            }
            else
            {
                itemsToPrint.AddFirst(item);
            }
        }

        if (itemsToPrint.Count > 0)
        {
            var builder = new StringBuilder();
            builder.AppendJoin('\n', itemsToPrint);

            Console.WriteLine(
                builder.ToString()
                );
        }

        PrintSelectedItem(currentItem);
    }

    private static void PrintSelectedItem(string item)
    {
        var textColor = Console.ForegroundColor;
        Console.ForegroundColor = Console.BackgroundColor;
        Console.BackgroundColor = textColor;

        Console.WriteLine(item);

        textColor = Console.ForegroundColor;
        Console.ForegroundColor = Console.BackgroundColor;
        Console.BackgroundColor = textColor;
    }

    private string RenderItem(int index)
    {
        int consoleWidth = Console.WindowWidth;
        string item = _items[index].Render();

        if (PrintItemNumber)
        {
            item = $"{index + 1}. {item}";
        }

        if (index == _currentMenuItemIndex)
        {
            item = $" -- {item}";

            if (item.Length > consoleWidth)
            {
                int lastLineLength = item.Length;

                while (lastLineLength > consoleWidth)
                {
                    lastLineLength -= consoleWidth;
                }

                item += new string(' ', consoleWidth - lastLineLength);
            }
            else
            {
                item += new string(' ', consoleWidth - item.Length);
            }
        }
        else
        {
            item = $"    {item}";
        }

        return item;
    }

    public enum RenderAction
    {
        NeedRerender,
        RerenderNoNeeded,
        NeedReversRerender
    }

    protected RenderAction SelectPreviousItem()
    {
        if (_firstItemIndexOnDisplay > 0
            && _currentMenuItemIndex == _firstItemIndexOnDisplay)
        {
            --_firstItemIndexOnDisplay;
        }

        --_currentMenuItemIndex;

        if (_currentMenuItemIndex < 0)
        {
            if (IsMenuLooped)
            {
                _currentMenuItemIndex =
                _lastItemIndexOnDisplay = _items.Count - 1;

                return RenderAction.NeedReversRerender;
            }

            _currentMenuItemIndex =
            _firstItemIndexOnDisplay = 0;

            return RenderAction.RerenderNoNeeded;
        }

        return RenderAction.NeedRerender;
    }

    protected RenderAction SelectNextItem()
    {
        var action = RenderAction.NeedRerender;

        if (_lastItemIndexOnDisplay < _items.Count - 1
            && _currentMenuItemIndex >= _lastItemIndexOnDisplay)
        {
            ++_firstItemIndexOnDisplay;
            ++_lastItemIndexOnDisplay;

            action = RenderAction.NeedReversRerender;
        }

        ++_currentMenuItemIndex;

        if (_currentMenuItemIndex >= _items.Count)
        {
            if (IsMenuLooped)
            {
                _currentMenuItemIndex = 0;

                int menuCapacity = GetMenuCapacity();

                _firstItemIndexOnDisplay = 0;
                _lastItemIndexOnDisplay = menuCapacity - 1;
            }
            else
            {
                _lastItemIndexOnDisplay =
                _currentMenuItemIndex = _items.Count - 1;
                
                return RenderAction.RerenderNoNeeded;
            }
        }

        return action;
    }

    private int GetMenuCapacity()
    {
        int menuHeight = Console.WindowHeight - _headerSize;
        int lineInUse = 0;

        for (int i = _firstItemIndexOnDisplay; ; ++i)
        {
            if (i == _items.Count)
            {
                return lineInUse;
            }

            int linesCount = GetItemHeightInWindow(i);

            if (linesCount + lineInUse >= menuHeight)
            {
                return lineInUse;
            }

            lineInUse += linesCount;
        }
    }

    private int GetItemHeightInWindow(int index)
    {
        int consoleWidth = Console.WindowWidth;
        string item = _items[index].Render();

        if (PrintItemNumber)
        {
            item = $"{index + 1}. {item}";
        }

        const int shiftFromLeft = 4;

        int itemLength = item.Length + shiftFromLeft;
        int linesCount = 1;

        while (itemLength > consoleWidth)
        {
            ++linesCount;
            itemLength -= consoleWidth;
        }

        return linesCount;
    }
}
