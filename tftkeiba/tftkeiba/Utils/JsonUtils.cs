using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Utils
{
    public class JsonUtils
    {
        /// <summary>
        /// json形式のファイルをクラスに格納して返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadJson<T>(string path)
            where T : class, new()
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                T res = JsonConvert.DeserializeObject<T>(json);
                return res;
            }
        }
    }
}
