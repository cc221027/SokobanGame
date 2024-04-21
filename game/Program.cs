using libs;

class Program
{    
    static void Main(string[] args)
    {
        //Setup
        Console.CursorVisible = false;
        var engine = GameEngine.Instance;
        var inputHandler = InputHandler.Instance;
        
        while (true) {

            engine.Setup();

            // Main game loop
            while (true)
            {
                engine.Render();

                if(engine.WinCheck()) {
                    FileHandler.ChangeLevel();
                    break;
                }

                // Handle keyboard input
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                inputHandler.Handle(keyInfo);
            }
        }
    }
}