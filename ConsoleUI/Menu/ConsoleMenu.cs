namespace ConsoleUI.Menu;

public class ConsoleMenu : ConsoleMenuBase
{
    public override void ShowDialog()
    {
        _isOpen = true;

        Render();

        while (true)
        {
            var needRerender = false;

            var key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    needRerender = SelectPreviousItem();
                    break;

                case ConsoleKey.DownArrow:
                    needRerender = SelectNextItem();
                    break;

                case ConsoleKey.Enter:
                    var currentItem = _items[_currentMenuItemIndex];

                    currentItem.ChooseItem();
                    needRerender = true;

                    break;
            }

            SelectedItem = _items[_currentMenuItemIndex];

            if (CustomEvents is not null
                && CustomEvents.TryGetValue(key.Key, out var handler))
            {
                handler.Invoke();

                needRerender = true;
            }

            if (!_isOpen)
            {
                Console.CursorVisible = true;
                return;
            }

            if (!needRerender)
            {
                continue;
            }

            Render();
        }
    }

    public override void CloseDialog()
    {
        _isOpen = false;
    }

    protected virtual void Render()
    {
        Console.Clear();
        Console.CursorVisible = false;

        if (Header is not null)
        {
            RenderHeader();
        }

        RenderMenuItems();
    }

    protected virtual void RenderHeader()
    {
        Console.WriteLine($"--- {Header}\n");
    }

    protected virtual void RenderMenuItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            string item = _items[i].Render();

            if (PrintItemNumber)
            {
                item = $"{i + 1}. {item}";
            }

            if (i == _currentMenuItemIndex)
            {
                var textColor = Console.ForegroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.BackgroundColor = textColor;

                int consoleWidth = Console.WindowWidth;
                string selectedItem = $" -- {item}";

                if (selectedItem.Length > consoleWidth)
                {
                    int lastLineLength = selectedItem.Length;

                    while (lastLineLength > consoleWidth)
                    {
                        lastLineLength -= consoleWidth;
                    }

                    Console.WriteLine(selectedItem + new string(' ', consoleWidth - lastLineLength));
                }
                else
                {
                    Console.WriteLine(selectedItem + new string(' ', consoleWidth - selectedItem.Length));
                }

                textColor = Console.ForegroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.BackgroundColor = textColor;
            }
            else
            {
                Console.WriteLine($"    {item}");
            }
        }
    }

    protected virtual bool SelectPreviousItem()
    {
        --_currentMenuItemIndex;

        if (_currentMenuItemIndex >= 0)
        {
            return true;
        }
        
        if (IsMenuLooped)
        {
            _currentMenuItemIndex = _items.Count - 1;
            return true;
        }

        _currentMenuItemIndex = 0;
        return false;
    }

    protected virtual bool SelectNextItem()
    {
        ++_currentMenuItemIndex;

        if (_currentMenuItemIndex < _items.Count)
        {
            return true;
        }

        if (IsMenuLooped)
        {
            _currentMenuItemIndex = 0;
            return true;
        }

        _currentMenuItemIndex = _items.Count - 1;
        return false;
    }
}
