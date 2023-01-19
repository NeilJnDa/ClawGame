using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class InterfaceFinder
{
    public static List<T> GetAllByInterface<T>() where T:class
    {   
        List<T> interfaces = new List<T>();
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            if (!rootGameObject.activeInHierarchy) continue;
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
            foreach (var childInterface in childrenInterfaces)
            {
                interfaces.Add(childInterface);
            }
        }
        return interfaces;
    }
}
