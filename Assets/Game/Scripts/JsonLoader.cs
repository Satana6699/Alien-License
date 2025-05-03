using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts
{
    public static class JsonLoader
    {
        public static LevelData LoadLevel(string jsonPath)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(jsonPath);

            if (jsonFile != null)
            {
                LevelData data = JsonUtility.FromJson<LevelData>(jsonFile.text);

                return data;
            }
            else
            {
                Debug.LogError("Файл не найден в Resources.");

                return null;
            }
        }
    }
}
