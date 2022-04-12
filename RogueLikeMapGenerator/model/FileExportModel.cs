using System.IO;
using System.Threading.Tasks;

namespace RogueLikeMapGenerator.model
{
    class FileExportModel
    {
        /// <summary>
        /// RogueLikeMap.txt 라는 파일로 맵을 추출한다.
        /// </summary>
        public static async Task WriteFile(string str)
        {
            try
            {
                await File.WriteAllTextAsync("RogueLikeMap.txt", str);
            }
            catch
            {
                throw;
            }
        }
    }
}
