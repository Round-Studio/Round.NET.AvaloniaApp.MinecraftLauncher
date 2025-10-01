using System.Text.Json;

namespace RMCL.Config;

public class Config<T> where T : new()
{
    public T Data { get; set; }
    public string Path { get; private set; }
    
    public Config(string configFile)
    {
        Path = configFile;
        Load();
    }
    
    public void Load()
    {
        if (!File.Exists(Path))
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            Save();
            return;
        }
        
        var json = File.ReadAllText(Path);
        if (string.IsNullOrEmpty(json))
        {
            Save();
        }
        else
        {
            try
            {
                Data = JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                Save();
            }
        }
    }
    
    public void Save()
    {
        if (Data == null)
        {
            Data = new T(); // 现在这里可以正常工作了
        }
        
        string jsresult = JsonSerializer.Serialize(Data, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(Path, jsresult);
    }
}