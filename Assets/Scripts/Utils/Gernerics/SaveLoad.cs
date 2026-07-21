using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveLoad
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        string json = JsonConvert.SerializeObject(data, Settings);
        File.WriteAllText(path, json);
    }

    public static T Load<T>(string fileName)
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
            return default;

        return JsonConvert.DeserializeObject<T>(
            File.ReadAllText(path),
            Settings);
    }

    public static bool Exists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    public static void Delete(string fileName)
    {
        string path = GetPath(fileName);

        if (File.Exists(path))
            File.Delete(path);
    }

    private static string GetPath(string fileName)
    {
        if (!fileName.EndsWith(".json"))
            fileName += ".json";

        return Path.Combine(Application.persistentDataPath, fileName);
    }
}