
namespace RogueLikeMapGenerator.model
{
    public class DebugLogModel
    {
        public string debugLog { get; set; } = "";

        public void AddDebugLog(string msg)
        {
            debugLog += msg + "\n";
        }
    }
}
