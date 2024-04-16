namespace libs;

public sealed class InputHandler{

    private static InputHandler? _instance;
    private GameEngine engine;

    public static InputHandler Instance {
        get{
            if(_instance == null)
            {
                _instance = new InputHandler();
            }
            return _instance;
        }
    }

    private InputHandler() {
        //INIT PROPS HERE IF NEEDED
        engine = GameEngine.Instance;
    }

    public void Handle(ConsoleKeyInfo keyInfo)
    {
        GameObject focusedObject = engine.GetFocusedObject();

        if (focusedObject != null) {

            if(keyInfo.Key == ConsoleKey.Z && keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control)) {
                engine.RestoreMap();
                return;
            }

            engine.StoreMap();

            // Handle keyboard input to move the player
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    CollisionSituation(focusedObject, 0, -1);
                    break;
                case ConsoleKey.DownArrow:
                    CollisionSituation(focusedObject, 0, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    CollisionSituation(focusedObject, -1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    CollisionSituation(focusedObject, 1, 0);
                    break;
                default:
                    break;
            }

            // var objOnMyPos = engine.GetMap().Get(focusedObject.PosY, focusedObject.PosX);
            // if (objOnMyPos is ICollidable) {
            //     if (objOnMyPos is not Box) {
            //         engine.RestoreMap();
            //     }
            // }
        }

        void CollisionSituation(GameObject player, int dx, int dy)
        {
            int nextPlayerPosX = player.PosX + dx;
            int nextPlayerPosY = player.PosY + dy;

            // get object at next position to which player moves
            GameObject nextObject = engine.GetMap().Get(nextPlayerPosY, nextPlayerPosX);

            if (nextObject is Box box) {
                // calculate next position of box
                int nextBoxPosX = box.PosX + dx;
                int nextBoxPosY = box.PosY + dy;

                // position behind original box position = position box will be moved to -> is there an obstacle?
                GameObject posBehindBox = engine.GetMap().Get(nextBoxPosY, nextBoxPosX);

                // if obstacle behind box -> cant push
                if (posBehindBox is Obstacle or Box) {
                    engine.RestoreMap();
                } 
                else if (posBehindBox is Goal goal) {
                    if (goal.Color == ConsoleColor.Yellow) {
                        engine.RestoreMap();
                    }
                    else {
                        box.Move(dx, dy);
                        player.Move(dx, dy);
                        goal.Color = ConsoleColor.Yellow;
                    }
                }
                else {
                    box.Move(dx, dy);
                    player.Move(dx, dy);
                }
            }
            else if (nextObject is Obstacle) {
                // player cant move cos wall / other obstacle
                engine.RestoreMap();
            }
            else {
                player.Move(dx, dy);
            }
        }
        
    }

}