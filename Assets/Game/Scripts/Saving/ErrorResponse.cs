using System;

namespace Saving
{
    [Serializable]
    public class ErrorResponse
    {
        public string name;
        public string message;
        public int code;
        public int status;
        public string type;
    }
}