using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using UnityEngine;

namespace Kalevala.Persistence
{
    [Serializable]
    public class JSONPersistence : IPersistence
    {
        public string Extension { get { return ".json"; } }

        public string FilePath { get; private set;}

        /// <summary>
        /// Initializes the JSONPersistence object.
        /// </summary>
        /// <param name="path">The path of the save file
        /// without the file extension</param>
        public JSONPersistence(string path)
        {
            FilePath = path + Extension;
        }

        /// <summary>
        /// Saves data.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data to save</param>
        public void Save<T>(T data)
        {
            string jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(FilePath, jsonData, Encoding.UTF8);
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>loaded data of the given type</returns>
        public T Load<T>()
        {
            if (File.Exists(FilePath))
            {
                Debug.Log("Data loaded");
                string jsonData = File.ReadAllText(FilePath, Encoding.UTF8);
                return JsonUtility.FromJson<T>(jsonData);
            }
            else
            {
                Debug.Log("Data doesn't exist");
                return default(T);
            }

            //try
            //{
            //    string jsonData = File.ReadAllText(FilePath, Encoding.UTF8);
            //    return JsonUtility.FromJson<T>(jsonData);
            //}
            //catch (FileNotFoundException e)
            //{
            //    Debug.LogWarning("No save file created yet.\n" + e);
            //    return default(T);
            //}
            //catch (IsolatedStorageException e2)
            //{
            //    Debug.LogWarning("No save file created yet.\n" + e2);
            //    return default(T);
            //}
        }
    }
}
