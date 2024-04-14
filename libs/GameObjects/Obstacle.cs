namespace libs;

public class Obstacle : GameObject, ICollidable {
    public Obstacle () : base() {
        this.Type = GameObjectType.Obstacle;
        this.CharRepresentation = '█';
        this.Color = ConsoleColor.Cyan;
    }
}