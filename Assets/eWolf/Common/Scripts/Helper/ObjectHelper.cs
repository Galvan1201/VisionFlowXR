using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace eWolf.Common.Helper
{
    public static class ObjectHelper
    {
        public static GameObject FindChildObject(GameObject baseObject, string name, bool autoCreate = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return baseObject;
            }

            GameObject gameObject = null;
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (Transform child in baseObject.transform)
                {
                    if (child.name == name)
                    {
                        gameObject = child.gameObject;
                        break;
                    }
                }
                if (gameObject == null && autoCreate)
                {
                    gameObject = new GameObject
                    {
                        name = name
                    };
                    gameObject.transform.parent = baseObject.transform;
                }
            }
            return gameObject;
        }

        public static T FindObjectType<T>(Vector3 position, float range)
        {
            IEnumerable<T> items = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<T>();
            foreach (T item in items)
            {
                var monoBehaviour = item as MonoBehaviour;

                var gap = monoBehaviour.transform.position - position;
                var distance = gap.magnitude;
                if (distance < range)
                {
                    return item;
                }
            }

            return default;
        }

        public static T FindObjectType<T>(Vector3 position)
        {
            return FindObjectType<T>(position, 3);
        }

        public static List<GameObject> GetAllChildObjects(GameObject baseObject, string name)
        {
            var go = FindChildObject(baseObject, name);
            return GetAllChildObjects(go);
        }

        public static List<GameObject> GetAllChildObjects(GameObject baseObject)
        {
            List<GameObject> objects = new List<GameObject>();
            if (baseObject == null)
                return objects;

            foreach (Transform child in baseObject.transform)
            {
                objects.Add(child.gameObject);
            }
            return objects;
        }

        public static List<GameObject> GetAllChildObjectsRecursive(GameObject baseObject)
        {
            List<GameObject> objects = new List<GameObject>();
            if (baseObject == null)
                return objects;

            foreach (Transform child in baseObject.transform)
            {
                objects.Add(child.gameObject);
                objects.AddRange(GetAllChildObjectsRecursive(child.gameObject));
            }
            return objects;
        }

        public static void RemoveAllObjectFrom(GameObject baseObject, string name)
        {
            var gameObject = FindChildObject(baseObject, name);
            RemoveAllObjectFrom(gameObject);
        }

        public static void RemoveAllObjectFrom(GameObject gameObject)
        {
            var objects = GetAllChildObjects(gameObject);

            for (int i = 0; i < objects.Count; i++)
            {
                GameObject.DestroyImmediate(objects[i]);
            }
        }

        public static bool RemoveComponent<T>(GameObject baseObject) where T : Component
        {
            var component = baseObject.GetComponent<T>();
            if (component != null)
            {
                GameObject.DestroyImmediate(component);
                return true;
            }
            
            return false;
        }

        public static T GetComponent<T>(GameObject baseObject) where T : Component
        {
            var component = baseObject.GetComponent<T>();
            if (component == null)
            {
                component = baseObject.AddComponent<T>();
            }

            return component;
        }
    }
}