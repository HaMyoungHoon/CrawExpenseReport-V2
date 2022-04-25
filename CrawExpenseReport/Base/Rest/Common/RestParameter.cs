using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Rest.Common
{
    internal class RestParameter
    {
        readonly List<string> _path;
        readonly Dictionary<string, object?> _params;
        readonly Dictionary<string, object?> _headers;
        public RestParameter()
        {
            _path = new List<string>();
            _params = new Dictionary<string, object?>();
            _headers = new Dictionary<string, object?>();
        }

        public string GetPaths()
        {
            StringBuilder stb = new();
            foreach (var key in _path)
            {
                stb.AppendFormat("{0}/", key);
            }
            if (stb.Length > 0)
            {
                if (stb[^1] == '/')
                {
                    stb.Remove(stb.Length - 1, 1);
                }
            }
            return stb.ToString();
        }
        public string GetParams()
        {
            StringBuilder stb = new();
            foreach (var key in _params.Keys)
            {
                stb.AppendFormat("{0}={1}&", key, _params[key]);
            }
            if (stb.Length > 0)
            {
                if (stb[^1] == '&')
                {
                    stb.Remove(stb.Length - 1, 1);
                }
            }
            return stb.ToString();
        }
        public List<string> GetHeaders()
        {
            List<string> ret = new();
            foreach (var key in _headers.Keys)
            {
                ret.Add(string.Format("{0}: {1}", key, _headers[key]));
            }
            return ret;
        }
        public Dictionary<string, object?> GetHeaders2()
        {
            return _headers;
        }
        public void AppendPath(string value)
        {
            _path.Add(value);
        }
        public void AppendParameter(string key, object value)
        {
            if (!_params.ContainsKey(key))
            {
                string? valueBuff;
                if (value.GetType().Name.ToLower() == "class")
                {
                    valueBuff = JsonSerializer.Serialize(value);
                }
                else
                {
                    valueBuff = value.ToString();
                }
                _params.Add(key, valueBuff);
            }
        }
        public void AppendHeaders(string key, object value)
        {
            if (!_headers.ContainsKey(key))
            {
                string? valueBuff;
                if (value.GetType().Name.ToLower() == "class")
                {
                    valueBuff = JsonSerializer.Serialize(value);
                }
                else
                {
                    valueBuff = value.ToString();
                }
                _headers.Add(key, valueBuff);
            }
        }
    }
}
