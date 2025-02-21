namespace Data
{
    [System.Serializable]
    public class OneLeson // TODO rename
    {
        public int id;
        public string title;
        public bool completed;
        public OneTestQuestion[] tests;
    }
}