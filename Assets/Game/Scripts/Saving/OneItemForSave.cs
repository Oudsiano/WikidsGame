using System;

namespace Saving
{
    [Serializable]
    public class OneItemForSave
    {
        public int count;
        public string name;

        public OneItemForSave(int _c, string _n) // TODO construct and _c _n rename
        {
            count = _c;
            name = _n;
        }
    }
}