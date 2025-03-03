namespace Web.Data
{
    [System.Serializable]
    public class ConfigData
    {
        public int id;
        public int health;
        public bool isAlive;
        public string SceneNameToLoad;
        public int sceneToLoad;
        public bool testSuccess;
        
        public ConfigData(int id, int health, bool isAlive, string sceneNameToLoad, int sceneToLoad,bool testSuccess)
        {
            this.id = id;
            this.health = health;
            this.isAlive = isAlive;
            SceneNameToLoad = sceneNameToLoad;
            this.sceneToLoad = sceneToLoad;
            this.testSuccess = testSuccess;
        }
    }
}