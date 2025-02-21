using System.Collections.Generic;
using Core.Quests.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Saving
{
    public class SL_objs
    {
        public Dictionary<string, object> objs;
        public SL_objs(string _load)
        {
            objs = JsonConvert.DeserializeObject<Dictionary<string, object>>(_load);
        }

        private void unpars<T>(ref T _ob, string i)
        {
            if (objs.ContainsKey(i))
            {
                if (_ob is bool)
                {
                    _ob = JsonConvert.DeserializeObject<T>(objs[i].ToString().ToLower());
                }
                else if (_ob is float)
                {
                    _ob = JsonConvert.DeserializeObject<T>(objs[i].ToString().Replace(',', '.'));
                }
                else
                {
                    object obj = objs[i];
                    if (obj != null)
                    {
                        string str1 = obj.ToString();
                        if (str1.Length>2)
                            _ob = JsonConvert.DeserializeObject<T>(str1);
                    }
                }
            }
            else
                Debug.Log("Загрузка пытается получить больше данных чем есть в файле.");
        }

        internal void load(ref List<string> _ob, string v)
        {
            unpars<List<string>>(ref _ob, v);
        }

        internal void load(ref double _ob, string v)
        {
            unpars<double>(ref _ob, v);
        }
        internal void load(ref int _ob, string v)
        {
            unpars<int>(ref _ob, v);
        }
        internal void load(ref long _ob, string v)
        {
            unpars<long>(ref _ob, v);
        }
        internal void load(ref string _ob, string v)
        {
            unpars<string>(ref _ob, v);
        }
        internal void load(ref bool _ob, string v)
        {
            unpars<bool>(ref _ob, v);
        }

        internal void load(ref Dictionary<string, string> _ob, string v)
        {
            unpars<Dictionary<string, string>>(ref _ob, v);
        }

        internal void load(ref Dictionary<string, int> _ob, string v)
        {
            unpars<Dictionary<string, int>>(ref _ob, v);
        }
    
        internal void load(ref Dictionary<int, bool> _ob, string v)
        {
            unpars<Dictionary<int, bool>>(ref _ob, v);
        }
        internal void load(ref Dictionary<string, OneQuestData> _ob, string v)
        {
            unpars<Dictionary<string, OneQuestData>>(ref _ob, v);
        }
    }
}