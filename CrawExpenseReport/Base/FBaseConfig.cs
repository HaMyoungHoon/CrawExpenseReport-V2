using BaseLib_Net6;
using CrawExpenseReport.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrawExpenseReport.Base
{
    public partial class FBaseConfig
    {
        public FBaseConfig()
        {
            IsDarkTheme = false;
            IsStartOnSync = false;
            WebDriverPath = "";
            EdgePath = "";
            Timeout = 10;
            Industrial = @"URL 제거";
            Petrochem = @"URL 제거";
            Company = "";
            ExpenseReportName = "";
            RetryCount = 0;
            IsSuccessLogHide = false;

            OldLoginInfo = new FBaseFunc.LoginInfo();
            LoginInfo = new FBaseFunc.LoginInfo();
            LoginSector = new FBaseFunc.LoginInfo();
            BtnLogin = "";
            LoginErr = "";

            CopySector = new FBaseFunc.CopySectorTable();

            API_URL = "";
            ApprovalUrl = "";
            BtnNew = "";
            TbSearchNew = "";
            BodyNew = "";
            BtnConfirm = "";
            BtnAddRow = "";
            BtnClose = "";
            PasteSector = new FBaseFunc.PasteSectorTable();

            CompanyModelList = new List<FBaseFunc.CompanyModel>();
            WorkplaceModelList = new List<FBaseFunc.WorkplaceModel>();
            AccountModelList = new List<FBaseFunc.AccountModel>();
            CorrespondentModelList = new List<FBaseFunc.CorrespondentModel>();
            DepartmentModelList = new List<FBaseFunc.DepartmentModel>();
        }

        private bool CheckFolder(string path)
        {
            string temp = path;
            DirectoryInfo filePath = new DirectoryInfo(path);
            DirectoryInfo di = new DirectoryInfo(temp.Replace(filePath.Name, ""));

            if (di.Exists == false)
            {
                di.Create();
                return false;
            }

            return true;
        }
        private bool CheckFile(string path)
        {
            if (File.Exists(path) == false)
            {
                using (File.Create(path))
                {

                }

                return false;
            }

            return true;
        }

        private void CreateSettingFile()
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            CheckFolder(filePath);
            if (CheckFile(filePath) == true)
            {
                return;
            }
            File.WriteAllText(filePath, "<?xml version='1.0' encoding='UTF-8'?>\n<CRAW>\n</CRAW>");
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            CreateUtilConfig(xmlData);
            CreateLoginConfig(xmlData);
            CreateCopySectorConfig(xmlData);
            CreatePasteSectorConfig(xmlData);

        }
        public void LoadSettingFile()
        {
            CreateSettingFile();
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            SetUtilConfig(xmlData);
            SetLoginInfoConfig(xmlData);
            SetCopySectorConfig(xmlData);
            SetPasteSectorConfig(xmlData);
        }

        public string GetDBPath()
        {
            return string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", DataBasePath);
        }

        private void CreateUtilConfig(FFileParser xmlData)
        {
            xmlData.SetString("CRAW,UTIL,IS_DARK_THEME", "false");
            xmlData.SetString("CRAW,UTIL,PRIMARY_COLOR", "#FF4527A0");
            xmlData.SetString("CRAW,UTIL,SECONDARY_COLOR", "#FF795548");
            xmlData.SetString("CRAW,UTIL,PRIMARY_FOREGROUND_COLOR", "#FFE3F2FD");
            xmlData.SetString("CRAW,UTIL,SECONDARY_FOREGROUND_COLOR", "#FFF1F8E9");
            xmlData.SetString("CRAW,UTIL,API_URL", FAES128.Encrypt(@"URL 제거", FBaseFunc.AES_KEY));
            xmlData.SetString("CRAW,UTIL,WEB_DRIVER", string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), @"edgedriver_win64\"));
            xmlData.SetString("CRAW,UTIL,EDGE_PATH", @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe");
            xmlData.SetString("CRAW,UTIL,TIMEOUT", "3");
            xmlData.SetString("CRAW,UTIL,SHEET_FONT_SIZE", "11");
            xmlData.SetString("CRAW,UTIL,DELAY_TIME", "100");
            xmlData.SetString("CRAW,UTIL,DEF_IND", @"URL 제거");
            xmlData.SetString("CRAW,UTIL,DEF_PET", @"URL 제거");
            xmlData.SetString("CRAW,UTIL,COMPANY", "산업");
            xmlData.SetString("CRAW,UTIL,LOGIN,OLD_ID", FAES128.Encrypt("", FBaseFunc.AES_KEY));
            xmlData.SetString("CRAW,UTIL,LOGIN,OLD_PW", FAES128.Encrypt("", FBaseFunc.AES_KEY));
            xmlData.SetString("CRAW,UTIL,IS_START_ON_SYNC", "false");
            xmlData.SetString("CRAW,UTIL,IS_HOT_RELOAD", "false");
            xmlData.SetString("CRAW,UTIL,EXPENSE_REPORT_NAME", "지출/수입품의서");
            xmlData.SetString("CRAW,UTIL,RETRY_COUNT", "3");
            xmlData.SetString("CRAW,UTIL,IS_NEW_FIRST", "false");
            xmlData.SetString("CRAW,UTIL,IS_DIRECT_INPUT", "false");
            xmlData.SetString("CRAW,UTIL,IS_SUCCESS_LOG_HIDE", "false");
            xmlData.SetString("CRAW,UTIL,IS_SPEED_UP", "false");
            xmlData.SetString("CRAW,UTIL,IS_GUARD_SCREEN_SAVER", "true");
        }
        private void CreateLoginConfig(FFileParser xmlData)
        {
            xmlData.SetString("CRAW,LOGIN,ByID,ID", "username");
            xmlData.SetString("CRAW,LOGIN,ByID,PW", "password");
            xmlData.SetString("CRAW,LOGIN,ByID,BTN_LOGIN", "login_submit");
            xmlData.SetString("CRAW,LOGIN,ByCLASSNAME,LOGIN_ERR", "long_msg");

            xmlData.SetString("CRAW,LOGIN,VALUE,ID", FAES128.Encrypt("", FBaseFunc.AES_KEY));
            xmlData.SetString("CRAW,LOGIN,VALUE,PW", FAES128.Encrypt("", FBaseFunc.AES_KEY));
        }
        private void CreateCopySectorConfig(FFileParser xmlData)
        {
            xmlData.SetString("CRAW,COPY,UTIL,TYPE", "완료");
            xmlData.SetString("CRAW,COPY,UTIL,URL", "");

            xmlData.SetString("CRAW,COPY,ByID,TITLE", "subject");
            xmlData.SetString("CRAW,COPY,COMPANY,ByCLASSNAME,COMPANY_CODE", "coCd");
            xmlData.SetString("CRAW,COPY,COMPANY,ByCLASSNAME,COMPANY_NAME", "coCd");
            xmlData.SetString("CRAW,COPY,COMPANY,ByCLASSNAME,WORKPLACE_CODE", "divName");
            xmlData.SetString("CRAW,COPY,COMPANY,ByCLASSNAME,WORKPLACE_NAME", "divName");

            xmlData.SetString("CRAW,COPY,SLIP,ByID,BODY", "slipBplTable");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,GUBUN", "gubun");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,ACCOUNT", "debitRmk");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,TAX_DATE", "taxDate");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,TYPE", "taxInvestigationDivision");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,CORRESPONDENT", "dztrade");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,PRICE", "valueOfSupply");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,BRIEFS", "summary");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,DEPARTMENT", "useDeptName");
            xmlData.SetString("CRAW,COPY,SLIP,ByCLASSNAME,SUPPLY_PRICE", "supplyAmt");
        }
        private void CreatePasteSectorConfig(FFileParser xmlData)
        {
            xmlData.SetString("CRAW,PASTE,UTIL,URL", "app/approval");
            xmlData.SetString("CRAW,PASTE,UTIL,ByXPATH,NEW", "/html/body/div/div[2]/div[1]/section[2]/a");
            xmlData.SetString("CRAW,PASTE,UTIL,ByXPATH,SERACH", "/html/body/div[3]/div/div[2]/div[1]/form/section/div[1]/input[1]");
            xmlData.SetString("CRAW,PASTE,UTIL,ByXPATH,BODY", "/html/body/div[3]/div/div[2]/div[1]/div/div[2]/div/ul/li/a");
            xmlData.SetString("CRAW,PASTE,UTIL,ByXPATH,CONFIRM", "/html/body/div[3]/footer/a[1]");
            xmlData.SetString("CRAW,PASTE,UTIL,ByID,ADD_ROW", "plusRow");
            xmlData.SetString("CRAW,PASTE,UTIL,ByID,CLOSE", "go_popup_close_icon");

            xmlData.SetString("CRAW,PASTE,COMPANY,ByXPATH,COMPANY", "/html/body/div[4]/div/div/table/tbody");
            xmlData.SetString("CRAW,PASTE,COMPANY,ByXPATH,WORKPLACE", "/html/body/div[4]/div/span/div/div/table/tbody");

            xmlData.SetString("CRAW,PASTE,SLIP,ByID,ACCOUNT_SEARCH_TYPE", "searchtype");
            xmlData.SetString("CRAW,PASTE,SLIP,ByID,ACCOUNT_SEARCH_TEXT", "keyword");
            xmlData.SetString("CRAW,PASTE,SLIP,ByID,ACCOUNT_SEARCH_BTN", "searchBtn");
            xmlData.SetString("CRAW,PASTE,SLIP,ByXPATH,ACCOUNT_SEARCH_BODY", "/html/body/div[4]/div/div/div/table/tbody");

            xmlData.SetString("CRAW,PASTE,SLIP,ByID,CORRESPONDENT_SEARCH_TYPE", "searchtype2");
            xmlData.SetString("CRAW,PASTE,SLIP,ByID,CORRESPONDENT_SEARCH_TEXT", "partnerKeyword");
            xmlData.SetString("CRAW,PASTE,SLIP,ByID,CORRESPONDENT_SEARCH_BTN", "partnerSearchBtn");
            xmlData.SetString("CRAW,PASTE,SLIP,ByXPATH,CORRESPONDENT_SEARCH_BODY", "/html/body/div[4]/div/span/div/div[2]/table/tbody");

            xmlData.SetString("CRAW,PASTE,SLIP,ByID,DEPARTMENT_SEARCH_TYPE", "searchtype2");
            xmlData.SetString("CRAW,PASTE,SLIP,ByID,DEPARTMENT_SEARCH_TEXT", "partnerKeyword");
            xmlData.SetString("CRAW,PASTE,SLIP,ByID,DEPARTMENT_SEARCH_BTN", "partnerSearchBtn");
            xmlData.SetString("CRAW,PASTE,SLIP,ByXPATH,DEPARTMENT_SEARCH_BODY", "/html/body/div[4]/div/span/div/div/table/tbody");
        }

        private void SetUtilConfig(FFileParser xmlData)
        {
            IsDarkTheme                 = xmlData.GetString("CRAW,UTIL,IS_DARK_THEME", "true").ToLower() == "true";
            PrimaryColor                = (Color)ColorConverter.ConvertFromString(xmlData.GetString("CRAW,UTIL,PRIMARY_COLOR", "#FF4527A0"));
            SecondaryColor              = (Color)ColorConverter.ConvertFromString(xmlData.GetString("CRAW,UTIL,SECONDARY_COLOR", "#FF795548"));
            PrimaryForegroundColor      = (Color)ColorConverter.ConvertFromString(xmlData.GetString("CRAW,UTIL,PRIMARY_FOREGROUND_COLOR", "#FFE3F2FD"));
            SecondaryForegroundColor    = (Color)ColorConverter.ConvertFromString(xmlData.GetString("CRAW,UTIL,SECONDARY_FOREGROUND_COLOR", "#FFF1F8E9"));
            API_URL         = FAES128.Decrypt(xmlData.GetString("CRAW,UTIL,API_URL", ""), FBaseFunc.AES_KEY);
            WebDriverPath   = xmlData.GetString("CRAW,UTIL,WEB_DRIVER", string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), @"edgedriver_win64\"));
            EdgePath        = xmlData.GetString("CRAW,UTIL,EDGE_PATH", @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe");
            Timeout         = xmlData.GetInt("CRAW,UTIL,TIMEOUT", 10);
            SheetFontSize   = xmlData.GetInt("CRAW,UTIL,SHEET_FONT_SIZE", 11);
            DelayTime       = xmlData.GetInt("CRAW,UTIL,DELAY_TIME", 100);
            Company         = xmlData.GetString("CRAW,UTIL,COMPANY", "산업");
            OldLoginInfo.ID = FAES128.Decrypt(xmlData.GetString("CRAW,UTIL,LOGIN,OLD_ID", ""), FBaseFunc.AES_KEY);
            OldLoginInfo.PW = FAES128.Decrypt(xmlData.GetString("CRAW,UTIL,LOGIN,OLD_PW", ""), FBaseFunc.AES_KEY);
            IsStartOnSync   = xmlData.GetString("CRAW,UTIL,IS_START_ON_SYNC", "false").ToLower() == "true";
            ExpenseReportName   = xmlData.GetString("CRAW,UTIL,EXPENSE_REPORT_NAME", "");
            RetryCount          = xmlData.GetInt("CRAW,UTIL,RETRY_COUNT", 2);
            IsSuccessLogHide    = xmlData.GetString("CRAW,UTIL,IS_SUCCESS_LOG_HIDE", "false").ToLower() == "true";
            IsGuardScreenSaver  = xmlData.GetString("CRAW,UTIL,IS_GUARD_SCREEN_SAVER", "true").ToLower() == "true";
            if (Timeout < 1)
            {
                SetTimeout(1);
            }
            if (DelayTime < 50)
            {
                SetDelayTime(50);
            }
            if (SheetFontSize < 1)
            {
                SetSheetFontSize(1);
            }
            if (RetryCount < 1)
            {
                SetRetryCount(1);
            }
        }
        private void SetLoginInfoConfig(FFileParser xmlData)
        {
            LoginSector.ID = xmlData.GetString("CRAW,LOGIN,ByID,ID", "username");
            LoginSector.PW = xmlData.GetString("CRAW,LOGIN,ByID,PW", "papasswordsswd");
            BtnLogin = xmlData.GetString("CRAW,LOGIN,ByID,BTN_LOGIN", "login_submit");
            LoginErr = xmlData.GetString("CRAW,LOGIN,ByCLASSNAME,LOGIN_ERR", "long_msg");
            LoginInfo.ID = FAES128.Decrypt(xmlData.GetString("CRAW,LOGIN,VALUE,ID", ""), FBaseFunc.AES_KEY);
            LoginInfo.PW = FAES128.Decrypt(xmlData.GetString("CRAW,LOGIN,VALUE,PW", ""), FBaseFunc.AES_KEY);
        }
        private void SetCopySectorConfig(FFileParser xmlData)
        {
            CopySector.Type = xmlData.GetString("CRAW,COPY,UTIL,TYPE", "");
            CopySector.Url = xmlData.GetString("CRAW,COPY,UTIL,URL", "");

            CopySector.Title = xmlData.GetString("CRAW,COPY,ByID,TITLE", "");
            CopySector.CompanySector.CompanyCode = xmlData.GetString("CRAW,COPY,COMPANY,ByCLASSNAME,COMPANY_CODE", "");
            CopySector.CompanySector.CompanyName = xmlData.GetString("CRAW,COPY,COMPANY,ByCLASSNAME,COMPANY_NAME", "");
            CopySector.WorkplaceSector.WorkplaceCode = xmlData.GetString("CRAW,COPY,COMPANY,ByCLASSNAME,WORKPLACE_CODE", "");
            CopySector.WorkplaceSector.WorkplaceName = xmlData.GetString("CRAW,COPY,COMPANY,ByCLASSNAME,WORKPLACE_NAME", "");

            CopySector.Body = xmlData.GetString("CRAW,COPY,SLIP,ByID,BODY", "");
            CopySector.SlipSector.Gubun = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,GUBUN", "");
            CopySector.SlipSector.Account.AccountName = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,ACCOUNT", "");
            CopySector.SlipSector.TaxDate = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,TAX_DATE", "");
            CopySector.SlipSector.Type = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,TYPE", "");
            CopySector.SlipSector.Correspondent.CorrespondentName = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,CORRESPONDENT", "");
            CopySector.SlipSector.Price = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,PRICE", "");
            CopySector.SlipSector.Briefs = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,BRIEFS", "");
            CopySector.SlipSector.Department.DepartmentName = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,DEPARTMENT", "");
            CopySector.SlipSector.SupplyPrice = xmlData.GetString("CRAW,COPY,SLIP,ByCLASSNAME,SUPPLY_PRICE", "");
        }
        private void SetPasteSectorConfig(FFileParser xmlData)
        {
            ApprovalUrl = xmlData.GetString("CRAW,PASTE,UTIL,URL", "app/approval");
            BtnNew = xmlData.GetString("CRAW,PASTE,UTIL,ByXPATH,NEW", "/html/body/div/div[2]/div[1]/section[2]/a");
            TbSearchNew = xmlData.GetString("CRAW,PASTE,UTIL,ByXPATH,SERACH", "/html/body/div[3]/div/div[2]/div[1]/form/section/div[1]/input[1]");
            BodyNew = xmlData.GetString("CRAW,PASTE,UTIL,ByXPATH,BODY", "/html/body/div[3]/div/div[2]/div[1]/div/div[2]/div/ul/li/a");
            BtnConfirm = xmlData.GetString("CRAW,PASTE,UTIL,ByXPATH,CONFIRM", "/html/body/div[3]/footer/a[1]/span[2]");
            BtnAddRow = xmlData.GetString("CRAW,PASTE,UTIL,ByID,ADD_ROW", "plusRow");
            BtnClose = xmlData.GetString("CRAW,PASTE,UTIL,ByID,CLOSE", "go_popup_close_icon");

            PasteSector.Company = xmlData.GetString("CRAW,PASTE,COMPANY,ByXPATH,COMPANY", "");
            PasteSector.Workplace = xmlData.GetString("CRAW,PASTE,COMPANY,ByXPATH,WORKPLACE", "");

            PasteSector.AccountSearchType = xmlData.GetString("CRAW,PASTE,SLIP,ByID,ACCOUNT_SEARCH_TYPE", "");
            PasteSector.AccountSearchText = xmlData.GetString("CRAW,PASTE,SLIP,ByID,ACCOUNT_SEARCH_TEXT", "");
            PasteSector.AccountSearchBtn = xmlData.GetString("CRAW,PASTE,SLIP,ByID,ACCOUNT_SEARCH_BTN", "");
            PasteSector.AccountSearchBody = xmlData.GetString("CRAW,PASTE,SLIP,ByXPATH,ACCOUNT_SEARCH_BODY", "");

            PasteSector.CorrespondentSearchType = xmlData.GetString("CRAW,PASTE,SLIP,ByID,CORRESPONDENT_SEARCH_TYPE", "");
            PasteSector.CorrespondentSearchText = xmlData.GetString("CRAW,PASTE,SLIP,ByID,CORRESPONDENT_SEARCH_TEXT", "");
            PasteSector.CorrespondentSearchBtn = xmlData.GetString("CRAW,PASTE,SLIP,ByID,CORRESPONDENT_SEARCH_BTN", "");
            PasteSector.CorrespondentSearchBody = xmlData.GetString("CRAW,PASTE,SLIP,ByXPATH,CORRESPONDENT_SEARCH_BODY", "");
            
            PasteSector.DepartmentSearchType = xmlData.GetString("CRAW,PASTE,SLIP,ByID,DEPARTMENT_SEARCH_TYPE", "");
            PasteSector.DepartmentSearchText = xmlData.GetString("CRAW,PASTE,SLIP,ByID,DEPARTMENT_SEARCH_TEXT", "");
            PasteSector.DepartmentSearchBtn = xmlData.GetString("CRAW,PASTE,SLIP,ByID,DEPARTMENT_SEARCH_BTN", "");
            PasteSector.DepartmentSearchBody = xmlData.GetString("CRAW,PASTE,SLIP,ByXPATH,DEPARTMENT_SEARCH_BODY", "");
        }

        public void SetPrimaryColor(Color color)
        {
            PrimaryColor = color;
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            CheckFolder(filePath);

            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,PRIMARY_COLOR", PrimaryColor.ToString());
        }
        public void SetSecondaryColor(Color color)
        {
            SecondaryColor = color;
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            CheckFolder(filePath);

            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,SECONDARY_COLOR", SecondaryColor.ToString());
        }
        public void SetPrimaryForegroundColor(Color color)
        {
            PrimaryForegroundColor = color;
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            CheckFolder(filePath);

            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,PRIMARY_FOREGROUND_COLOR", PrimaryForegroundColor.ToString());
        }
        public void SetSecondaryForegroundColor(Color color)
        {
            SecondaryForegroundColor = color;
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            CheckFolder(filePath);

            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,SECONDARY_FOREGROUND_COLOR", SecondaryForegroundColor.ToString());
        }
        public void SetDarkTheme(bool data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,IS_DARK_THEME", data.ToString());
            IsDarkTheme = data;
        }
        public void SetStartOnSync(bool data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,IS_START_ON_SYNC", data.ToString());
            IsStartOnSync = data;
        }
        public void SetTimeout(int data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetInt("CRAW,UTIL,TIMEOUT", data);
            Timeout = data;
        }
        public void SetSheetFontSize(int data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetInt("CRAW,UTIL,SHEET_FONT_SIZE", data);
            SheetFontSize = data;
        }
        public void SetDelayTime(int data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetInt("CRAW,UTIL,DELAY_TIME", data);
            DelayTime = data;
        }   
        public void SetDefIndustiral(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,DEF_IND", data);
            Industrial = data;
        }
        public void SetDefPetrochem(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,DEF_PET", data);
            Petrochem = data;
        }
        public void SetCompany(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,COMPANY", data);
            Company = data;
        }
        public void SetExpenseReportName(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,EXPENSE_REPORT_NAME", data);
            ExpenseReportName = data;
        }
        public void SetRetryCount(int data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetInt("CRAW,UTIL,RETRY_COUNT", data);
            RetryCount = data;
        }
        public void SetIsSuccessLogHide(bool data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,IS_SUCCESS_LOG_HIDE", data.ToString());
            IsSuccessLogHide = data;
        }
        public void SetIsGuardScreenSaver(bool data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,IS_GUARD_SCREEN_SAVER", data.ToString());
            IsGuardScreenSaver = data;
        }

        public void SetOldID(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,LOGIN,OLD_ID", FAES128.Encrypt(data, FBaseFunc.AES_KEY));
            OldLoginInfo.ID = data;
        }
        public void SetOldPW(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,UTIL,LOGIN,OLD_PW", FAES128.Encrypt(data, FBaseFunc.AES_KEY));
            OldLoginInfo.PW = data;
        }
        public void SetID(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,LOGIN,VALUE,ID", FAES128.Encrypt(data, FBaseFunc.AES_KEY));
            LoginInfo.ID = data;
        }
        public void SetPW(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,LOGIN,VALUE,PW", FAES128.Encrypt(data, FBaseFunc.AES_KEY));
            LoginInfo.PW = data;
        }

        public void SetCopyType(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,COPY,UTIL,TYPE", data);
            CopySector.Type = data;
        }
        public void SetCopyUrl(string data)
        {
            string filePath = string.Format(@"{0}\{1}\{2}", Directory.GetCurrentDirectory(), "Setting", CfgPath);
            FFileParser xmlData = new FFileParser(filePath, FFileParser.FILE_TYPE.XML);
            xmlData.SetString("CRAW,COPY,UTIL,URL", data);
            CopySector.Url = data;
        }

        public void ClearModelList()
        {
            CompanyModelList.Clear();
            WorkplaceModelList.Clear();
            AccountModelList.Clear();
            CorrespondentModelList.Clear();
            DepartmentModelList.Clear();
        }
        public List<FBaseFunc.CompanyModel> CallCompanyList()
        {
            return CompanyModelList.ToList();
        }
        public List<FBaseFunc.WorkplaceModel> CallWorkplaceList(string companyName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<FBaseFunc.WorkplaceModel>();
            }

            return WorkplaceModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode).ToList();
        }
        public List<FBaseFunc.AccountModel> CallAccountList(string gubun, string companyName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<FBaseFunc.AccountModel>();
            }

            string debit = "";
            switch (gubun)
            {
                case "차변": debit = "1"; break;
                case "대변": debit = "2"; break;
                default: return new List<FBaseFunc.AccountModel>();
            }

            return AccountModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && (x.GetAccountCode() < 40000 || (x.GetAccountCode() >= 40000 && x.AccountDebit == debit))).ToList();
        }
        public List<string> CallTypeList()
        {
            List<string> data = new List<string>();
            data.Add("세금계산서(과세)");
            data.Add("세금계산서(불공)");
            data.Add("세금계산서(영세)");
            data.Add("세금계산서(수입)");
            data.Add("계산서(면세)");
            data.Add("법인카드");
            data.Add("현금영수증");
            data.Add("개인카드");
            data.Add("기타");
            return data;
        }
        public List<FBaseFunc.CorrespondentModel> CallCorrespondentList(string companyName, string accountName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<FBaseFunc.CorrespondentModel>();
            }

            var accountData = AccountModelList.Find(x => x.AccountName == accountName);
            if (accountData == null)
            {
                return new List<FBaseFunc.CorrespondentModel>();
            }

            switch (accountData.AccountType)
            {
                case "A1":  return CorrespondentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode).ToList();
                case "A2":  return CorrespondentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && x.GetCorrespondentCode() >= 90000).ToList();
                default:    return CorrespondentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && x.GetCorrespondentCode() > 100000).ToList();
            }
        }
        public List<FBaseFunc.DepartmentModel> CallDepartmentList(string companyName, string workplaceName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<FBaseFunc.DepartmentModel>();
            }
            var workplaceData = WorkplaceModelList.Find(x => x.CompanyCode == companyData.CompanyCode && x.WorkplaceName == workplaceName);
            if (workplaceData == null)
            {
                return new List<FBaseFunc.DepartmentModel>();
            }

            return DepartmentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && x.WorkplaceCode == workplaceData.WorkplaceCode).ToList();
        }
        public List<string> CallCompanyNameList()
        {
            return CompanyModelList.Select(x => x.GetCompanyName()).ToList();
        }
        public List<string> CallWorkplaceNameList(string company)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == company);
            if (companyData == null)
            {
                return new List<string>();
            }

            return WorkplaceModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode).Select(x => x.GetWorkplaceName()).ToList();
        }
        public List<string> CallAccountNameList(string gubun, string companyName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<string>();
            }

            string debit = "";
            switch (gubun)
            {
                case "차변":  debit = "1";  break;
                case "대변":  debit = "2";  break;
                default:      return new List<string>();
            }

            return AccountModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && (x.GetAccountCode() < 40000 || (x.GetAccountCode() >= 40000 && x.AccountDebit == debit))).Select(x => x.GetAccountName()).ToList();
        }
        public List<string> CallTypeNameList()
        {
            List<string> data = new List<string>();
            data.Add("세금계산서(과세)");
            data.Add("세금계산서(불공)");
            data.Add("세금계산서(영세)");
            data.Add("세금계산서(수입)");
            data.Add("계산서(면세)");
            data.Add("법인카드");
            data.Add("현금영수증");
            data.Add("개인카드");
            data.Add("기타");
            return data;
        }
        public List<string> CallCorrespondentNameList(string companyName, string accountName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<string>();
            }

            var accountData = AccountModelList.Find(x => x.AccountName == accountName);
            if (accountData == null)
            {
                return new List<string>();
            }

            switch (accountData.AccountType)
            {
                case "A1":  return CorrespondentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode).Select(x => x.GetCorrespondentName()).ToList();
                case "A2":  return CorrespondentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && x.GetCorrespondentCode() >= 90000).Select(x => x.GetCorrespondentName()).ToList();
                default:    return CorrespondentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && x.GetCorrespondentCode() > 100000).Select(x => x.GetCorrespondentName()).ToList();
            }
        }
        public List<string> CallDepartmentNameList(string companyName, string workplaceName)
        {
            var companyData = CompanyModelList.Find(x => x.CompanyName == companyName);
            if (companyData == null)
            {
                return new List<string>();
            }
            var workplaceData = WorkplaceModelList.Find(x => x.CompanyCode == companyData.CompanyCode && x.WorkplaceName == workplaceName);
            if (workplaceData == null)
            {
                return new List<string>();
            }

            return DepartmentModelList.FindAll(x => x.CompanyCode == companyData.CompanyCode && x.WorkplaceCode == workplaceData.WorkplaceCode).Select(x => x.GetDepartmentName()).ToList();
        }
    }
}