using CrawExpenseReport.Base.Rest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Rest
{
    internal class ICubeService
    {
        private string _url = @"api/v1/icube/";
        public ICubeService()
        {

        }

        public bool CompanyList(string companyType, out string err, out RestResult ret)
        {
            string companyUrl = string.Format("{0}company/{1}", _url.Replace("api", FBaseFunc.Ins.Cfg.API_URL), companyType);
            RestApiService service = new RestApiService(companyUrl, RestApiService.Method_Type.GET, FBaseFunc.Ins.Cfg.Timeout * 1000);
            service.AppendHeaders("auth_token", FBaseFunc.Ins.Token);
            return service.Send(out err, out ret);
        }
        public bool WorkplaceList(string companyCode, out string err, out RestResult ret)
        {
            string workpalceUrl = string.Format("{0}workplace/{1}", _url.Replace("api", FBaseFunc.Ins.Cfg.API_URL), companyCode);
            RestApiService service = new RestApiService(workpalceUrl, RestApiService.Method_Type.GET, FBaseFunc.Ins.Cfg.Timeout * 1000);
            service.AppendHeaders("auth_token", FBaseFunc.Ins.Token);
            return service.Send(out err, out ret);
        }
        public bool AccountList(string companyCode, int debit, out string err, out RestResult ret)
        {
            string accountUrl = string.Format("{0}account/{1}/{2}", _url.Replace("api", FBaseFunc.Ins.Cfg.API_URL), companyCode, debit);
            RestApiService service = new RestApiService(accountUrl, RestApiService.Method_Type.GET, FBaseFunc.Ins.Cfg.Timeout * 1000);
            service.AppendHeaders("auth_token", FBaseFunc.Ins.Token);
            return service.Send(out err, out ret);
        }
        public bool CorrespondentList(string companyCode, string accountType, out string err, out RestResult ret)
        {
            string correspondentUrl = string.Format("{0}correspondent/{1}/{2}", _url.Replace("api", FBaseFunc.Ins.Cfg.API_URL), companyCode, accountType);
            RestApiService service = new RestApiService(correspondentUrl, RestApiService.Method_Type.GET, FBaseFunc.Ins.Cfg.Timeout * 1000);
            service.AppendHeaders("auth_token", FBaseFunc.Ins.Token);
            return service.Send(out err, out ret);
        }
        public bool DepartmentList(string companyCode, string divCode, out string err, out RestResult ret)
        {
            string departmentUrl = string.Format("{0}department/{1}/{2}", _url.Replace("api", FBaseFunc.Ins.Cfg.API_URL), companyCode, divCode);
            RestApiService service = new RestApiService(departmentUrl, RestApiService.Method_Type.GET, FBaseFunc.Ins.Cfg.Timeout * 1000);
            service.AppendHeaders("auth_token", FBaseFunc.Ins.Token);
            return service.Send(out err, out ret);
        }
    }
}
