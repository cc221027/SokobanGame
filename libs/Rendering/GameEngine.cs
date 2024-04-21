using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace libs;

using System.Security.Cryptography;
using Newtonsoft.Json;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;

    public static GameEngine Instance {
        get{
            if(_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine() {
        //INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory();
    }

    private GameObject? _focusedObject;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();

    // In a linked list: 1 object always shows to the next and the previous object
    // -> insertion & deletion is easier -> no need to shift elements after insertion/deletion & easier locatable, even after
    private LinkedList<List<GameObject>> gameObjectSnapshots = new LinkedList<List<GameObject>>();

    public void StoreMap() {

        // new list with gameobject clones
        // 1 list represents one snapshot of game
        List<GameObject> gameObjectClones = new List<GameObject>();

        // create clone for each original gameobject of original list -> dont want to effect original object
        foreach (GameObject gameObject in gameObjects) {
            gameObjectClones.Add((GameObject) gameObject.Clone());
        }

        gameObjectSnapshots.AddLast(gameObjectClones);
    }

    public void RestoreMap() {

        // at beginning of game wihtout any move made it obv cant restore
        if (gameObjectSnapshots.Count <= 0) return;

        gameObjects = gameObjectSnapshots.Last!.Value;
        gameObjectSnapshots.RemoveLast();

        // cloned player isnt the actual player, its just a clone -> "promote" clone to original player
        _focusedObject = gameObjects.OfType<Player>().First();
    }


    public void SaveGame() {
        List<GameObject> lastSnapshot = gameObjectSnapshots.Last!.Value;

        // create object which can be 'changed' during runtime -> can add map width and height during game playing
        dynamic gameData = new System.Dynamic.ExpandoObject();
        gameData.gameObjects = lastSnapshot;
        gameData.map = new {
            width = map.MapWidth,
            height = map.MapHeight
        };

        string snapshotJson = JsonConvert.SerializeObject(gameData);
        string filepath = "../savedGame.json";
        File.WriteAllText(filepath, snapshotJson); 
    }


    public Map GetMap() {
        return map;
    }

    public GameObject GetFocusedObject(){
        return _focusedObject;
    }

    private bool characterCreated = false;




    public void Setup(){

        gameObjects.Clear();
        gameObjectSnapshots.Clear();
        map = new Map();
        characterCreated = false;


        //Added for proper display of game characters
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        dynamic gameData = FileHandler.ReadJson();
        
        map.MapWidth = gameData.map.width;
        map.MapHeight = gameData.map.height;

        foreach (var gameObject in gameData.gameObjects)
        {
            if(!characterCreated || gameObject.Type != 0){
                AddGameObject(CreateGameObject(gameObject));
                if(gameObject.Type == 0){characterCreated = true;}
            }
        }
        
        _focusedObject = gameObjects.OfType<Player>().First();
        StoreMap();

    }




    public void Render() {
        
        //Clean the map
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();

        //Render the map
        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            Console.WriteLine();
        }
    }
    
    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject){
        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects(){
        
        gameObjects.ForEach(delegate(GameObject obj)
        {
            map.Set(obj);
        });
    }

    private void DrawObject(GameObject gameObject){
        
        Console.ResetColor();

        if(gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }
        else{
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }

    public bool WinCheck() {
        return gameObjects.OfType<Goal>().All(goal => goal.Color == ConsoleColor.Yellow);
    }
}