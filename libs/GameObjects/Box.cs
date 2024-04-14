namespace libs;

public class Box : GameObject, ICollidable {

    public Box () : base(){
        Type = GameObjectType.Player;
        CharRepresentation = '○';
        Color = ConsoleColor.DarkGreen;
    }
}