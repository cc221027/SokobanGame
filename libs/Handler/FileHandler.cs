using System.Reflection.Metadata.Ecma335;

namespace libs;

using Newtonsoft.Json;

public static class FileHandler
{
    private static string filePath;
    //private readonly static string envVar = "GAME_SETUP_PATH";
    private static string savePath = "../savedGame.json";
    private static int levelNumber = 3;
    private static string levelPath = "../Level" + levelNumber + ".json";

    static FileHandler()
    {
        Initialize();
    }

    private static void Initialize()
    {
        // if(Environment.GetEnvironmentVariable(envVar) != null){
        //     filePath = Environment.GetEnvironmentVariable(enVar);
        // };
        if (File.Exists(savePath))
        {
            filePath = savePath;
        }
        else {
            filePath = levelPath;
        }
    }

    public static void ChangeLevel()
    {
        levelNumber++;

        if (levelNumber >= 3)
        {
            levelNumber = 1;
            levelPath = "../Level" + levelNumber + ".json";
            filePath = levelPath;
            ReadJson();
        }
        else {
            levelPath = "../Level" + levelNumber + ".json";
            filePath = levelPath;
            ReadJson(); 
        }
    }

    public static dynamic ReadJson()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
            return jsonData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"JSON file not found at path: {filePath}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading JSON file: {ex.Message}");
        }
    }
}
