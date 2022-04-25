using BaseLib_Net6;
using CrawExpenseReport.Base.dbSQL;
using CrawExpenseReport.Base.Rest;
using MaterialDesignExtensions.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base
{
    public partial class FBaseFunc
    {
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)] private static extern ExecutionState SetThreadExecutionState(ExecutionState state);

        private static FBaseFunc? _ins;
        public static FBaseFunc Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new FBaseFunc();
                }

                return _ins;
            }
        }
        public FBaseConfig Cfg;
        public FSqlite_Command Sql;
        private FPrintf _log;
        public CopyDataTable TempCopyTable;
        public List<CopyDataTable> CopyedTable;

        private readonly PaletteHelper _paletteHelper;

        public bool IsSettingOn;
        public bool IsCopyRun;
        public bool IsPasteRun;
        public bool IsTerminateOn;

        public int SelectedList;
        public string CurrentUrl { get; private set; }
        private readonly Queue<string> _ret;
        private readonly Queue<string> _testCmd;

        public delegate void BreakMethod();
        private BreakMethod _breakMethod;

        public delegate void ResultMethod(string data);
        private ResultMethod _resultMethod;
        public delegate void ResultEnd(bool isThreadCall);
        private ResultEnd _copyEnd;
        private ResultEnd _pasteEnd;

        private SignInService _signInService;
        private ICubeService _icubeService;
        public string Token { get; set; }
        public const string PROGRAM_VERSION = "2021-09-01";
        public const string AES_KEY = "1991031065748520";

        public const string TABLE_COMPANY_MODEL = "CompanyModel";
        public const string C_COMPANY_CODE      = "CompanyCode";
        public const string C_COMPANY_NAME      = "CompanyName";

        public const string TABLE_WORKPLACE_MODEL   = "WorkplaceModel";
//        public const string C_COMPANY_CODE          = "CompanyCode";
        public const string C_WORKPLACE_CODE        = "WorkplaceCode";
        public const string C_WORKPLACE_NAME        = "WorkplaceName";

        public const string TABLE_ACCOUNT_MODEL = "AccountModel";
//        public const string C_COMPANY_CODE      = "CompanyCode";
        public const string C_ACCOUNT_DEBIT     = "AccountDebit";
        public const string C_ACCOUNT_CODE      = "AccountCode";
        public const string C_ACCOUNT_NAME      = "AccountName";
        public const string C_ACCOUNT_TYPE      = "AccountType";

        public const string TABLE_CORRESPONDENT_MODEL   = "CorrespondentModel";
//        public const string C_COMPANY_CODE              = "CompanyCode";
        public const string C_CORRESPONDENT_CODE        = "CorrespondentCode";
        public const string C_CORRESPONDENT_NAME        = "CorrespondentName";
        public const string C_CORRESPONDENT_TYPE        = "CorrespondentType";
        public const string C_CORRESPONDENT_NUMBER      = "CorrespondentNumber";

        public const string TABLE_DEPARTMENT_MODEL  = "DepartmentModel";
//        public const string C_COMPANY_CODE          = "CompanyCode";
//        public const string C_WORKPLACE_CODE        = "WorkplaceCode";
        public const string C_DEPARTMENT_CODE       = "DepartmentCode";
        public const string C_DEPARTMENT_NAME       = "DepartmentName";

        public const string TABLE_COPY_HEADER_MODEL = "CopyHeaderModel";
        public const string C_COPY_INDEX            = "CopyIndex";
        public const string C_TITLE                 = "Title";
//        public const string C_COMPANY_CODE          = "CompanyCode";
//        public const string C_COMPANY_NAME          = "CompanyName";
//        public const string C_WORKPLACE_CODE        = "WorkplaceCode";
//        public const string C_WORKPLACE_NAME        = "WorkplaceName";

        public const string TABLE_COPY_BODY_MODEL   = "CopyBodyModel";
//        public const string C_COPY_INDEX            = "CopyIndex";
//        public const string C_COMPANY_CODE          = "CompanyCode";
//        public const string C_WORKPLACE_CODE        = "WorkplaceCode";
        public const string C_GUBUN                 = "Gubun";
//        public const string C_ACCOUNT_DEBIT         = "AccountDebit";
//        public const string C_ACCOUNT_CODE          = "AccountCode";
//        public const string C_ACCOUNT_NAME          = "AccountName";
//        public const string C_ACCOUNT_TYPE          = "AccountType";
        public const string C_TAX_DATE              = "TaxDate";
        public const string C_TYPE                  = "Type";
//        public const string C_CORRESPONDENT_CODE    = "CorrespondentCode";
//        public const string C_CORRESPONDENT_NAME    = "CorrespondentName";
//        public const string C_CORRESPONDENT_TYPE    = "CorrespondentType";
//        public const string C_CORRESPONDENT_NUMBER  = "CorrespondentNumber";
        public const string C_PRICE                 = "Price";
        public const string C_BRIEFS                = "Briefs";
//        public const string C_DEPARTMENT_CODE       = "DepartmentCode";
//        public const string C_DEPARTMENT_NAME       = "DepartmentName";
        public const string C_SUPPLY_PRICE          = "SupplyPrice";

        public const string REG_NUMBER              = @"[+-]?([0-9]+,)+[0-9]+(\.?\d*)";
    }
}
