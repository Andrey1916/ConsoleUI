using System.Runtime.Versioning;

namespace ConsoleUI.Extensions;

public static class ConsoleExtension
{
    [UnsupportedOSPlatform("android")]
    [UnsupportedOSPlatform("browser")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
    public static string? ReadLine()
    {
        string retString = string.Empty;
        int curIndex = 0;

        do
        {
            ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

            switch (readKeyResult.Key)
            {
                case ConsoleKey.Escape:
                    return null;

                case ConsoleKey.Enter:
                    return retString;

                case ConsoleKey.Backspace:
                    if (curIndex > 0)
                    {
                        retString = retString.Remove(retString.Length - 1);
                        Console.Write(readKeyResult.KeyChar);
                        Console.Write(' ');
                        Console.Write(readKeyResult.KeyChar);
                        curIndex--;
                    }
                    break;

                default:
                    retString += readKeyResult.KeyChar;
                    Console.Write(readKeyResult.KeyChar);
                    curIndex++;
                    break;
            }
        }
        while (true);
    }
}
