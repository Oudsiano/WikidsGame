using System.Collections.Generic;
using Core.Quests.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Saving
{
    public class SL_objs // TODO rename
    {
        private Dictionary<string, object> objects;

        public SL_objs(string loadJson) // TODO construct
        {
            objects = JsonConvert.DeserializeObject<Dictionary<string, object>>(loadJson);
        }

        private void Unparse<T>(ref T obj, string index) // TODO Rename
        {
            if (objects.ContainsKey(index))
            {
                if (obj is bool)
                {
                    obj = JsonConvert.DeserializeObject<T>(objects[index].ToString().ToLower());
                }
                else if (obj is float)
                {
                    obj = JsonConvert.DeserializeObject<T>(objects[index].ToString()
                        .Replace(',', '.')); // TODO can be cached
                }
                else
                {
                    object newObj = objects[index];

                    if (newObj != null)
                    {
                        string str1 = newObj.ToString();
                        if (str1.Length > 2) // TODO magic numbers
                        {
                            obj = JsonConvert.DeserializeObject<T>(str1);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Загрузка пытается получить больше данных чем есть в файле.");
            }
        }

        internal void Load(ref List<string> obj, string v)
        {
            Unparse<List<string>>(ref obj, v);
        }

        internal void Load(ref double obj, string v)
        {
            Unparse<double>(ref obj, v);
        }

        internal void Load(ref int obj, string v)
        {
            Unparse<int>(ref obj, v);
        }

        internal void Load(ref long obj, string v)
        {
            Unparse<long>(ref obj, v);
        }

        internal void Load(ref string obj, string v)
        {
            Unparse<string>(ref obj, v);
        }

        internal void Load(ref bool obj, string v)
        {
            Unparse<bool>(ref obj, v);
        }

        internal void Load(ref Dictionary<string, string> obj, string v)
        {
            Unparse<Dictionary<string, string>>(ref obj, v);
        }

        internal void Load(ref Dictionary<string, int> obj, string v)
        {
            Unparse<Dictionary<string, int>>(ref obj, v);
        }

        internal void Load(ref Dictionary<int, bool> obj, string v)
        {
            Unparse<Dictionary<int, bool>>(ref obj, v);
        }

        internal void Load(ref Dictionary<string, OneQuestData> obj, string v)
        {
            Unparse<Dictionary<string, OneQuestData>>(ref obj, v);
        }
    }
}