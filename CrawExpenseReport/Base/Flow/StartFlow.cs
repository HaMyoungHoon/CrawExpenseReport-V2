using BaseLib_Net6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.Flow
{
    public static class StartFlow
    {
        public static bool Login()
        {
            try
            {
                Thread.Sleep(1000);
                if (!FBaseFunc.Ins.CurrentUrl.ToLower().Contains("app/home") && FBaseFunc.Ins.IsNavigateComp("login"))
                {
                    FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetValue(FBaseFunc.Ins.Cfg.LoginSector.ID, FBaseFunc.Ins.Cfg.LoginInfo.ID, 0, FBaseFunc.ElementType.ID), out bool err);
                    FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementSetValue(FBaseFunc.Ins.Cfg.LoginSector.PW, FBaseFunc.Ins.Cfg.LoginInfo.PW, 0, FBaseFunc.ElementType.ID), out err);
                    FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementClick(FBaseFunc.Ins.Cfg.BtnLogin), out err);                    
                    Thread.Sleep(1000);
                    if (!FBaseFunc.Ins.IsNavigateComp("app/home"))
                    {
                        string loginErr = FBaseFunc.Ins.ExecuteScript(FBaseFunc.ElementGetValue(FBaseFunc.Ins.Cfg.LoginErr, "style", 0, FBaseFunc.ElementType.CLASS), out err);
                        if (loginErr.Contains("display: none;"))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FBaseFunc.Ins.SetResultMethod(ex.Message);
                FBaseFunc.Ins.SetLog(ex.Message);
                return false;
            }

            return true;
        }
    }
}
