using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Rest.Common
{
    internal class RestApiService : RestParameter
    {
        public enum Method_Type
        {
            NULL = -1,
            GET = 0,
            POST = 1,
            PUT = 2,
            DELETE = 3,
        }

        int _timeout;
        string _url;
        Method_Type _method;

        public RestApiService()
        {
            _url = "";
            _method = Method_Type.NULL;
            _timeout = 10 * 1000;
        }
        public RestApiService(string url, Method_Type method, int timeout = 10 * 1000)
        {
            _url = url;
            _method = method;
            _timeout = timeout;
        }
        public void SetUrl(string url)
        {
            _url = url;
        }
        public void SetMethod(Method_Type method)
        {
            _method = method;
        }
        public void SetTimeout(int timeout)
        {
            _timeout = timeout;
        }

        public bool Send(out string err, out RestResult? ret)
        {
            bool desRet = false;
            ret = new RestResult();
            if (_url.Length <= 0)
            {
                err = "Url is Empty";
                return desRet;
            }
            else if (!Enum.IsDefined<Method_Type>(_method) || _method == Method_Type.NULL)
            {
                err = "Request Method is NULL";
            }
            else
            {
                err = "";
            }

            try
            {
                StringBuilder stb = new();
                if (GetPaths().Length > 0)
                {
                    stb.AppendFormat("{0}{1}", _url, GetPaths());
                }
                else
                {
                    stb.AppendFormat("{0}", _url);
                }
                if (GetParams().Length > 0)
                {
                    stb.AppendFormat("?{0}", GetParams());
                }

                var httpClient = new HttpClient();
                HttpRequestMessage reqMessage = new();
                reqMessage.RequestUri = new Uri(stb.ToString());
                foreach (var header in GetHeaders2())
                {
                    reqMessage.Headers.Add(header.Key, header.Value?.ToString());
                }

                switch (_method)
                {
                    case Method_Type.NULL: return false;
                    case Method_Type.GET:
                        {
                            reqMessage.Method = HttpMethod.Get;
                        }
                        break;
                    case Method_Type.POST:
                        {
                            reqMessage.Method = HttpMethod.Post;
                        }
                        break;
                    case Method_Type.PUT:
                        {
                            reqMessage.Method = HttpMethod.Put;
                        }
                        break;
                    case Method_Type.DELETE:
                        {
                            reqMessage.Method = HttpMethod.Delete;
                        }
                        break;
                    default:
                        {
                            return false;
                        }
                }

                using var res = httpClient.Send(reqMessage);
                Stream resStream = res.Content.ReadAsStream();
                desRet = RestResult.Deserialize(resStream, out ret, out err);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return desRet;
            }

            return desRet;
        }

        public bool Send(string fileName, out string err, out FileStream? ret)
        {
            bool desRet = false;
            ret = null;
            if (_url.Length <= 0)
            {
                err = "Url is Empty";
                return desRet;
            }
            else if (!Enum.IsDefined<Method_Type>(_method) || _method == Method_Type.NULL)
            {
                err = "Request Method is NULL";
            }
            else
            {
                err = "";
            }

            try
            {
                StringBuilder stb = new();
                if (GetPaths().Length > 0)
                {
                    stb.AppendFormat("{0}{1}", _url, GetPaths());
                }
                else
                {
                    stb.AppendFormat("{0}", _url);
                }
                if (GetParams().Length > 0)
                {
                    stb.AppendFormat("?{0}", GetParams());
                }

                var httpClient = new HttpClient();
                HttpRequestMessage reqMessage = new();
                reqMessage.RequestUri = new Uri(stb.ToString());
                foreach (var header in GetHeaders2())
                {
                    reqMessage.Headers.Add(header.Key, header.Value?.ToString());
                }

                switch (_method)
                {
                    case Method_Type.NULL: return false;
                    case Method_Type.GET:
                        {
                            reqMessage.Method = HttpMethod.Get;
                        }
                        break;
                    case Method_Type.POST:
                        {
                            reqMessage.Method = HttpMethod.Post;
                        }
                        break;
                    case Method_Type.PUT:
                        {
                            reqMessage.Method = HttpMethod.Put;
                        }
                        break;
                    case Method_Type.DELETE:
                        {
                            reqMessage.Method = HttpMethod.Delete;
                        }
                        break;
                    default:
                        {
                            return false;
                        }
                }

                using var res = httpClient.Send(reqMessage);
                Stream resStream = res.Content.ReadAsStream();
                desRet = RestResult.Deserialize(fileName, resStream, out ret, out err);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return desRet;
            }

            return desRet;
        }

        public bool Send(string fileName, out string err, out RestResult? ret)
        {
            bool desRet = false;
            ret = new RestResult();
            if (_url.Length <= 0)
            {
                err = "Url is Empty";
                return desRet;
            }
            else if (!Enum.IsDefined<Method_Type>(_method) || _method == Method_Type.NULL)
            {
                err = "Request Method is NULL";
            }
            else
            {
                err = "";
            }

            try
            {
                StringBuilder stb = new();
                if (GetPaths().Length > 0)
                {
                    stb.AppendFormat("{0}{1}", _url, GetPaths());
                }
                else
                {
                    stb.AppendFormat("{0}", _url);
                }
                if (GetParams().Length > 0)
                {
                    stb.AppendFormat("?{0}", GetParams());
                }

                using HttpClient httpClient = new();
                using MultipartFormDataContent form = new();
                using FileStream file = new(fileName, FileMode.Open);
                using StreamContent streamContent = new(file);
                using ByteArrayContent fileContent = new(Task.Run(async () => await streamContent.ReadAsByteArrayAsync()).Result);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(fileContent, "file", Path.GetFileName(fileName));
                HttpResponseMessage res = Task.Run(async () => await httpClient.PostAsync(stb.ToString(), form)).Result;
                desRet = RestResult.Deserialize(res.Content.ReadAsStream(), out ret, out err);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return desRet;
            }

            return desRet;
        }
    }
}
