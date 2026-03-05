using UnityEngine;


public static class SaveManager
{
    private const string SaveKey = "FlippingSave";

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static GameSaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return null;

        string json = PlayerPrefs.GetString(SaveKey);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }

   public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }
}
