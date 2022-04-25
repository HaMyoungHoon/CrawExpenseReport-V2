using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrawExpenseReport.Base
{
    public partial class FBaseConfig
    {
        private const string CfgPath = "CrawCfg.xml";
        private const string DataBasePath = "DataBase.db";

        #region Util
        public bool IsDarkTheme { get; set; }
        public Color PrimaryColor { get; private set; }
        public Color SecondaryColor { get; private set; }
        public Color PrimaryForegroundColor { get; private set; }
        public Color SecondaryForegroundColor { get; private set; }
        public string API_URL { get; set; }
        public string WebDriverPath { get; set; }
        public string EdgePath { get; set; }
        public int Timeout { get; set; }
        public int SheetFontSize { get; set; }
        public int DelayTime { get; set; }
        public string Industrial { get; set; }
        public string Petrochem { get; set; }
        public string Company { get; set; }
        public FBaseFunc.LoginInfo OldLoginInfo { get; set; }
        public bool IsStartOnSync { get; set; }
        public string ExpenseReportName { get; set; }
        public int RetryCount { get; set; }
        public bool IsSuccessLogHide { get; set; }
        public bool IsGuardScreenSaver { get; set; }
        #endregion

        #region Login Info
        public FBaseFunc.LoginInfo LoginInfo { get; set; }
        public FBaseFunc.LoginInfo LoginSector { get; set; }
        public string BtnLogin { get; set; }
        public string LoginErr { get; set; }
        #endregion

        public FBaseFunc.CopySectorTable CopySector { get; set; }

        #region Paste
        public string ApprovalUrl { get; set; }
        public string BtnNew { get; set; }
        public string TbSearchNew { get; set; }
        public string BodyNew { get; set; }
        public string BtnConfirm { get; set; }
        public string BtnAddRow { get; set; }
        public string BtnClose { get; set; }
        public FBaseFunc.PasteSectorTable PasteSector { get; set; }
        #endregion

        public List<FBaseFunc.CompanyModel> CompanyModelList;
        public List<FBaseFunc.WorkplaceModel> WorkplaceModelList;
        public List<FBaseFunc.AccountModel> AccountModelList;
        public List<FBaseFunc.CorrespondentModel> CorrespondentModelList;
        public List<FBaseFunc.DepartmentModel> DepartmentModelList;
    }
}
