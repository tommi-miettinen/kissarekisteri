

namespace ConsoleApplication
{
    public static class KeyboardNavigation
    {
        public static int GetMenuChoice(string message, string[] options)
        {
            int selectedOption = 0;
            ConsoleKey keyPressed;


            do
            {
                Console.Clear();
                Console.WriteLine(message);

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }

                keyPressed = Console.ReadKey(true).Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    selectedOption = (selectedOption - 1 + options.Length) % options.Length;
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    selectedOption = (selectedOption + 1) % options.Length;
                }

            } while (keyPressed != ConsoleKey.Enter);

            return selectedOption;
        }
    }
}
