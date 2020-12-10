using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tftkeiba.Models.Response
{
    public class MetadataDto
    {
        public string data_version { set; get; }
        public string match_id { set; get; }
        public List<string> participants { set; get; }
    }
}
