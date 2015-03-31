using System.Collections.Generic;

namespace CherwellSDKUtility.Models
{
    public class ExecuteMethodContainer
    {
        public string SelectedApi { get; set; }

        public string MethodName { get; set; }

        public string CurrentArgument { get; set; }

        public List<ExecuteMethodArgument> MethodArguments { get; set; }
    }
}
