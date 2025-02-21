namespace Core
{
    public class PauseClass
    {
        private static bool isOpenUI; // TODO static
        private static bool isDialog; // TODO static

        public static bool IsOpenUI // TODO static
        {
            get
            {
                return isOpenUI;
            }
            set
            {
                isOpenUI = value; 
            }
        }
        public static bool IsDialog { get => isDialog; set => isDialog = value; } // TODO static

        public static bool GetPauseState() // TODO static
        {
            return isOpenUI || isDialog;
        }
    }
}
