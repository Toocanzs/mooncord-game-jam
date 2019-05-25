using System.Linq;
using UnityEngine;
/// <summary>
/// Inherit from this to create a singleton scriptable object. Be sure to place the scriptable object inside Resources/Singletons
/// </summary>
/// <typeparam name="T">Class to make a singleton out of</typeparam>
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    const string PATH = "Singletons";
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] results = Resources.LoadAll<T>(PATH);
                if(results.Length > 0)
                {
                    instance = results[0];
                }
                else
                {
                    Debug.LogError($"Singleton of type {typeof(T).Name} not found in {PATH}. Make sure you didn't move the singleton folder from {PATH}" +
                       $" if so, edit SingletonScriptableObject and change the const PATH. Also make sure a SingletonScriptableObject of type {typeof(T).Name} exists in {PATH}");
                }
            }
            return instance;
        }
    }
}