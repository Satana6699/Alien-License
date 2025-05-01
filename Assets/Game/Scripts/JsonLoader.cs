using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts
{
    public static class JsonLoader
    {
        public static BlockList LoadLevel(string jsonPath)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(jsonPath);

            if (jsonFile != null)
            {
                BlockList data = JsonUtility.FromJson<BlockList>(jsonFile.text);
                foreach (var block in data.blocks)
                {
                    Debug.Log($"Тип: {block.blockType}, Кол-во позиций: {block.positions.Count}");
                }

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
