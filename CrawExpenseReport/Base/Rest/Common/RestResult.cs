using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Rest.Common
{
    internal class RestResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("msg")]
        public string Msg { get; set; }
        [JsonPropertyName("data")]
        public object Data { get; set; }
        [JsonPropertyName("list")]
        public object List { get; set; }

        public RestResult()
        {
            Success = false;
            Code = -9999;
            Msg = "";
            Data = new();
            List = new();
        }

        public static bool Deserialize(Stream data, out RestResult? ret, out string err)
        {
            RestResult? buff = new();
            try
            {
                buff = Task.Run(async () => await JsonSerializer.DeserializeAsync<RestResult>(data)).Result;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                ret = buff;
                return false;
            }

            ret = buff;
            err = "";
            return true;
        }
        public static bool Deserialize(string data, out RestResult? ret, out string err)
        {
            RestResult? buff = new();
            try
            {
                buff = JsonSerializer.Deserialize<RestResult>(data);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                ret = buff;
                return false;
            }

            ret = buff;
            err = "";
            return true;
        }
        public static bool Deserialize(string fileName, Stream data, out FileStream? ret, out string err)
        {
            string filePath = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            if (File.Exists(filePath))
            {
                int extentionDot = fileName.LastIndexOf(".");
                if (extentionDot == -1)
                {
                    filePath = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                }
                else
                {
                    StringBuilder stb = new();
                    stb.AppendFormat("{0}", fileName[..extentionDot]);
                    stb.AppendFormat("_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));
                    stb.AppendFormat("{0}", fileName[extentionDot..]);
                    filePath = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), stb.ToString());
                }
            }

            ret = null;
            err = "";
            try
            {
                byte[] buff = new byte[10240];
                int pos = 0;
                int count;
                using (ret = new FileStream(filePath, FileMode.Create))
                {
                    do
                    {
                        count = data.Read(buff, pos, buff.Length);
                        ret.Write(buff, 0, count);
                    } while (count > 0);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }

            return true;
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
