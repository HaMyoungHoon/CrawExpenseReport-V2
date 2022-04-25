using CrawExpenseReport.Base.Rest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Rest
{
    internal class SignInService
    {
        private readonly string _url = @"api/v1/";

        public SignInService()
        {

        }

        public bool SignIn(string id, string pw, out string err, out RestResult? ret)
        {
            string url = $"{ProxyUrl(_url)}signIn";
            RestApiService service = new(url, RestApiService.Method_Type.POST);
            service.AppendParameter("id", id);
            service.AppendParameter("pw", pw);
            return service.Send(out err, out ret);
        }
        public bool GetUser(out string err, out FBaseFunc.PersonModel data)
        {
            string url = $"{ProxyUrl(_url)}user";
            RestApiService service = new(url, RestApiService.Method_Type.GET);
            service.AppendHeaders("snop_token", FBaseFunc.Ins.Token);
            bool ret = service.Send(out err, out RestResult? temp);
            if (!ret || temp == null)
            {
                data = new();
                return ret;
            }
            else
            {
                data = new(temp);
                return ret;
            }
        }
        private static string ProxyUrl(string url)
        {
            return url.Replace("api", FBaseFunc.Ins.Cfg.API_URL);
        }
    }
}
