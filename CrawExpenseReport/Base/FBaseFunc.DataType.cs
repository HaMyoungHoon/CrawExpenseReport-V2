using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static CrawExpenseReport.Base.FBaseFunc;

namespace CrawExpenseReport.Base
{
    public partial class FBaseFunc
    {
        public enum ElementType
        {
            XPATH,
            ID,
            CLASS,
        }
        [FlagsAttribute]
        public enum ExecutionState : uint
        {
            /// <summary> 
            /// ES_SYSTEM_REQUIRED 
            /// </summary> 
            ES_SYSTEM_REQUIRED = 0x00000001,
            /// <summary> 
            /// ES_DISPLAY_REQUIRED 
            /// </summary> 
            ES_DISPLAY_REQUIRED = 0x00000002,
            /// <summary> 
            /// ES_AWAYMODE_REQUIRED 
            /// </summary> 
            ES_AWAYMODE_REQUIRED = 0x00000040, 
            /// <summary> 
            /// ES_CONTINUOUS 
            /// </summary> 
            ES_CONTINUOUS = 0x80000000,

            ES_ALL = ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED | ES_AWAYMODE_REQUIRED | ES_CONTINUOUS
        }
        public class LoginInfo
        {
            public string ID;
            public string PW;
            public LoginInfo()
            {
                ID = "";
                PW = "";
            }
            public bool IsEmpty()
            {
                if (ID.Length <= 0 || PW.Length <= 0)
                {
                    return true;
                }

                return false;
            }
        }

        public class CopyDataTable
        {
            public int CopyIndex { get; set; }
            public string Title { get; set; }
            public CompanyModel CompanyValue { get; set; }
            public WorkplaceModel WorkplaceValue { get; set; }
            public List<SlipTable> SlipValues { get; set; }
            public CopyDataTable()
            {
                CopyIndex = -1;
                Title = "";
                CompanyValue = new CompanyModel();
                WorkplaceValue = new WorkplaceModel();
                SlipValues = new List<SlipTable>();
            }
            public CopyDataTable(CopyDataTable data)
            {
                CopyIndex = data.CopyIndex;
                Title = data.Title;
                CompanyValue = new CompanyModel()
                {
                    CompanyCode = data.CompanyValue.CompanyCode,
                    CompanyName = data.CompanyValue.CompanyName,
                };
                WorkplaceValue = new WorkplaceModel()
                {
                    WorkplaceCode = data.WorkplaceValue.WorkplaceCode,
                    WorkplaceName = data.WorkplaceValue.WorkplaceName,
                };
                SlipValues = new List<SlipTable>();
                for (int i = 0; i < data.SlipValues.Count; i++)
                {
                    SlipTable buff = new SlipTable()
                    {
                        Gubun = data.SlipValues[i].Gubun,
                        Account = data.SlipValues[i].Account,
                        TaxDate = data.SlipValues[i].TaxDate,
                        Type = data.SlipValues[i].Type,
                        Correspondent = data.SlipValues[i].Correspondent,
                        Price = data.SlipValues[i].Price,
                        Briefs = data.SlipValues[i].Briefs,
                        Department = data.SlipValues[i].Department,
                        SupplyPrice = data.SlipValues[i].SupplyPrice,
                    };
                    SlipValues.Add(buff);
                }
            }
            public CopyDataTable(DataRow dr)
            {
                if (int.TryParse(dr[0].ToString(), out int ret))
                {
                    CopyIndex = ret;
                }
                else
                {
                    CopyIndex = -1;
                }

                Title = dr[1].ToString();
                CompanyValue = new CompanyModel();
                CompanyValue.CompanyCode = dr[2].ToString();
                CompanyValue.CompanyName = dr[3].ToString();
                WorkplaceValue = new WorkplaceModel();
                WorkplaceValue.WorkplaceCode = dr[4].ToString();
                WorkplaceValue.WorkplaceName = dr[5].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_COPY_HEADER_MODEL);
                stb.AppendFormat("{0} INT, ", C_COPY_INDEX);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_TITLE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_COMPANY_NAME);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_WORKPLACE_CODE);
                stb.AppendFormat("{0} NVARCHAR(100))", C_WORKPLACE_NAME);

                return stb.ToString();
            }
            public static string GetSelectHeaderString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_COPY_HEADER_MODEL);
                stb.AppendFormat("ORDER BY {0}, {1}, {2} ASC", C_COPY_INDEX, C_COMPANY_CODE, C_WORKPLACE_CODE);
                
                return stb.ToString();
            }
            public string GetInsertHeaderString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_COPY_HEADER_MODEL);
                stb.AppendFormat("({0}, '{1}', '{2}', '{3}', '{4}', '{5}')", 
                    CopyIndex, Title, CompanyValue.CompanyCode, CompanyValue.CompanyName, WorkplaceValue.WorkplaceCode, WorkplaceValue.WorkplaceName);

                return stb.ToString();
            }
            public static string GetTruncateHeaderString(int index)
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("DELETE FROM {0} WHERE {1} = {2}", TABLE_COPY_HEADER_MODEL, C_COPY_INDEX, index);
                return stb.ToString();
            }
            public string GetTruncateHeaderString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("DELETE FROM {0} WHERE {1} = {2}", TABLE_COPY_HEADER_MODEL, C_COPY_INDEX, CopyIndex);
                return stb.ToString();
            }

            public void SetHeader(CopyDataTable data)
            {
                CopyIndex = data.CopyIndex;
                Title = data.Title;
                CompanyValue = data.CompanyValue;
                WorkplaceValue = data.WorkplaceValue;
            }
            public void SetBody(List<SlipTable> data)
            {
                SlipValues = data;
            }
            public void SetBody(SlipTable data)
            {
                SlipValues.Add(data);
            }
            public bool IsEmpty()
            {
                if (Title.Length <= 0 || CompanyValue.IsEmpty() || SlipValues.Count == 0)
                {
                    return true;
                }

                return false;
            }
            public void Clear()
            {
                CopyIndex = -1;
                Title = "";
                CompanyValue.Clear();
                WorkplaceValue.Clear();
                SlipValues.Clear();
            }
            public CopyDataTable Clone()
            {
                return new CopyDataTable(this);
            }
        }

        public class CopySectorTable
        {
            public string Type;
            public string Url;
            public string Title;
            public CompanyModel CompanySector;
            public WorkplaceModel WorkplaceSector;
            public string Body;
            public SlipTable SlipSector;

            public CopySectorTable()
            {
                Type = "";
                Url = "";
                Title = "";
                CompanySector = new CompanyModel();
                WorkplaceSector = new WorkplaceModel();
                Body = "";
                SlipSector = new SlipTable();
            }

            public bool IsEmpty()
            {
                if (Type.Length <= 0 || Url.Length <= 0 || Body.Length <= 0 || Title.Length <= 0)
                {
                    return true;
                }

                return false;
            }
        }

        public class SlipTable
        {
            public int CopyIndex                    { get; set; }
            public string Gubun                     { get; set; }
            public AccountModel Account             { get; set; }
            public string TaxDate                   { get; set; }
            public string Type                      { get; set; }
            public CorrespondentModel Correspondent { get; set; }
            public string Price                     { get; set; }
            public string Briefs                    { get; set; }
            public DepartmentModel Department       { get; set; }
            public string SupplyPrice               { get; set; }

            public SlipTable()
            {
                CopyIndex = -1;
                Gubun = "";
                Account = new AccountModel();
                TaxDate = DateTime.Now.ToString("yyyy-MM-dd(ddd)");
                Type = "";
                Correspondent = new CorrespondentModel();
                Price = "";
                Briefs = "";
                Department = new DepartmentModel();
                SupplyPrice = "";
            }
            public SlipTable(string[] data)
            {
                Account = new AccountModel();
                Correspondent = new CorrespondentModel();
                Department = new DepartmentModel();
                if (data.Count() >= 10)
                {
                    CopyIndex       = -1;
                    Gubun           = data[3];
                    Account.SetData(data[4]);
                    int taxDateIndex = data[5].IndexOf("(");
                    if (taxDateIndex == -1)
                    {
                        if (DateTime.TryParse(data[5], out DateTime ret))
                        {
                            TaxDate = ret.ToString("yyyy-MM-dd(ddd)");
                        }
                        else
                        {
                            TaxDate = DateTime.Now.ToString("yyyy-MM-dd(ddd)");
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(data[5].Substring(0, taxDateIndex), out DateTime ret))
                        {
                            TaxDate = ret.ToString("yyyy-MM-dd(ddd)");
                        }
                        else
                        {
                            TaxDate = DateTime.Now.ToString("yyyy-MM-dd(ddd)");
                        }
                    }
                    
                    Type            = data[6];
                    Correspondent.SetData(data[7]);
                    Price           = data[8];
                    Briefs          = data[9];
                    Department.SetData(data[10]);
                }
                if (data.Count() == 12)
                {
                    SupplyPrice = data[11];
                }
            }
            public SlipTable(DataRow dr)
            {
                Account = new AccountModel();
                Correspondent = new CorrespondentModel();
                Department = new DepartmentModel();
                if (int.TryParse(dr[0].ToString(), out int ret))
                {
                    CopyIndex = ret;
                }
                else
                {
                    CopyIndex = -1;
                }
                Account.CompanyCode         = dr[1].ToString();
                Correspondent.CompanyCode   = dr[1].ToString();
                Department.CompanyCode      = dr[1].ToString();
                Department.WorkplaceCode    = dr[2].ToString();

                Gubun = dr[3].ToString();
                Account.AccountDebit = dr[4].ToString();
                Account.AccountCode = dr[5].ToString();
                Account.AccountName = dr[6].ToString();
                Account.AccountType = dr[7].ToString();
                TaxDate = dr[8].ToString();
                Type = dr[9].ToString();
                Correspondent.CorrespondentCode = dr[10].ToString();
                Correspondent.CorrespondentName = dr[11].ToString();
                Correspondent.CorrespondentType = dr[12].ToString();
                Correspondent.CorrespondentNumber = dr[13].ToString();
                Price = dr[14].ToString();
                Briefs = dr[15].ToString();
                Department.DepartmentCode = dr[16].ToString();
                Department.DepartmentName = dr[17].ToString();
                SupplyPrice = dr[18].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_COPY_BODY_MODEL);
                stb.AppendFormat("{0} INT, ", C_COPY_INDEX);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_WORKPLACE_CODE);
                stb.AppendFormat("{0} NVARCHAR(5), ", C_GUBUN);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_ACCOUNT_DEBIT);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_ACCOUNT_CODE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_ACCOUNT_NAME);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_ACCOUNT_TYPE);
                stb.AppendFormat("{0} NVARCHAR(20), ", C_TAX_DATE);
                stb.AppendFormat("{0} NVARCHAR(20), ", C_TYPE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_CORRESPONDENT_CODE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_CORRESPONDENT_NAME);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_CORRESPONDENT_TYPE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_CORRESPONDENT_NUMBER);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_PRICE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_BRIEFS);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_DEPARTMENT_CODE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_DEPARTMENT_NAME);
                stb.AppendFormat("{0} NVARCHAR(100))", C_SUPPLY_PRICE);

                return stb.ToString();
            }
            public static string GetSelectString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_COPY_BODY_MODEL);
                stb.AppendFormat("ORDER BY {0}, {1}, {2} ASC", C_COPY_INDEX, C_COMPANY_CODE, C_WORKPLACE_CODE);
                return stb.ToString();
            }
            public string GetInsertString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_COPY_BODY_MODEL);
                stb.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}', ",    CopyIndex, Account.CompanyCode, Department.WorkplaceCode, Gubun, Account.AccountDebit);
                stb.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",     Account.AccountCode, Account.AccountName, Account.AccountType, TaxDate, Type);
                stb.AppendFormat("'{0}', '{1}', '{2}', '{3}', '{4}', ",     Correspondent.CorrespondentCode, Correspondent.CorrespondentName, Correspondent.CorrespondentType, Correspondent.CorrespondentNumber, Price);
                stb.AppendFormat("'{0}', '{1}', '{2}', '{3}')",      Briefs, Department.DepartmentCode, Department.DepartmentName, SupplyPrice);
                return stb.ToString();
            }
            public static string GetTruncateString(int index)
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("DELETE FROM {0} WHERE {1} = {2}", TABLE_COPY_BODY_MODEL, C_COPY_INDEX, index);
                return stb.ToString();
            }
            public string GetTruncateString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("DELETE FROM {0} WHERE {1} = {2}", TABLE_COPY_BODY_MODEL, C_COPY_INDEX, CopyIndex);
                return stb.ToString();
            }

            public bool IsEmptyGubun()
            {
                if (Gubun.Length <= 0)
                {
                    return true;
                }

                return false;
            }
            public bool IsEmptyAccount()
            {
                return Account.IsEmpty();
            }
            public bool IsEmptyTaxDate()
            {
                if (TaxDate.Length <= 0)
                {
                    return true;
                }

                return false;
            }
            public bool IsEmptyType()
            {
                if (Type.Length <= 0)
                {
                    return true;
                }

                return false;
            }
            public bool IsEmptyCorrespondent()
            {
                return Correspondent.IsEmpty();
            }
            public bool IsEmptyPrice()
            {
                if (Price.Length <= 0)
                {
                    return true;
                }

                return false;
            }
            public bool IsEmptyBriefs()
            {
                if (Briefs.Length <= 0)
                {
                    return true;
                }

                return false;
            }
            public bool IsEmptyDepartment()
            {
                return Department.IsEmpty();
            }
            public bool IsEmptySupplyPrice()
            {
                if (SupplyPrice.Length <= 0)
                {
                    return true;
                }

                return false;
            }

            public string GetTaxTime()
            {
                if (TaxDate == null || TaxDate.Length <= 0)
                {
                    return DateTime.Now.ToString("yyyy-MM-dd");
                }
                int index = TaxDate.IndexOf("(");
                if (index == -1)
                {
                    return DateTime.Now.ToString("yyyy-MM-dd");
                }

                return TaxDate.Substring(0, index);

            }
            public DateTime GetTaxTimeDateType()
            {
                if (TaxDate == null || TaxDate.Length <= 0)
                {
                    return DateTime.Now;
                }
                int index = TaxDate.IndexOf("(");
                if (index == -1)
                {
                    return DateTime.Now;
                }
                if (DateTime.TryParse(TaxDate.Substring(0, index), out DateTime ret))
                {
                    return ret;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            public void SetTaxTime(string data)
            {
                int index = data.IndexOf("(");
                if (index == -1)
                {
                    if (DateTime.TryParse(data, out DateTime ret))
                    {
                        TaxDate = ret.ToString("yyyy-MM-dd(ddd)");
                    }
                    else
                    {
                        TaxDate = DateTime.Now.ToString("yyyy-MM-dd(ddd)");
                    }
                }
                else
                {
                    if (DateTime.TryParse(data.Substring(0, index), out DateTime ret))
                    {
                        TaxDate = ret.ToString("yyyy-MM-dd(ddd)");
                    }
                    else
                    {
                        TaxDate = DateTime.Now.ToString("yyyy-MM-dd(ddd)");
                    }
                }
            }
            public void SetTaxTime(DateTime data)
            {
                TaxDate = data.ToString("yyyy-MM-dd(ddd)");
            }
            public string GetString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("구분 : {0}\t계정 : {1}\t증빙일자 : {2}\t증빙유형 : {3}\t거래처 : {4}\t금액 : {5}\n", 
                    Gubun, Account.GetAccountName(), TaxDate, Type, Correspondent.GetCorrespondentName(), Price);
                stb.AppendFormat("적요 : {0}\t사용부서 : {1}\t공급가액 : {2}", 
                    Briefs, Department.GetDepartmentName(), SupplyPrice);

                return stb.ToString();
            }
            public void NullValueDelete()
            {
                if (Gubun == null)
                {
                    Gubun = "";
                }
                if (Account == null)
                {
                    Account = new AccountModel();
                }
                if (Type == null)
                {
                    Type = "";
                }
                if (Correspondent == null)
                {
                    Correspondent = new CorrespondentModel();
                }
                if (Price == null)
                {
                    Price = "";
                }
                if (Briefs == null)
                {
                    Briefs = "";
                }
                if (Department == null)
                {
                    Department = new DepartmentModel();
                }
                if (SupplyPrice == null)
                {
                    SupplyPrice = "";
                }
            }
        }

        public class PasteSectorTable
        {
            public string Company;
            public string Workplace;
            public string AccountSearchType;
            public string AccountSearchText;
            public string AccountSearchBtn;
            public string AccountSearchBody;
            public string CorrespondentSearchType;
            public string CorrespondentSearchText;
            public string CorrespondentSearchBtn;
            public string CorrespondentSearchBody;
            public string DepartmentSearchType;
            public string DepartmentSearchText;
            public string DepartmentSearchBtn;
            public string DepartmentSearchBody;

            public PasteSectorTable()
            {
                Company = "";
                Workplace = "";
                AccountSearchType = "";
                AccountSearchText = "";
                AccountSearchBtn = "";
                AccountSearchBody = "";
                CorrespondentSearchType = "";
                CorrespondentSearchText = "";
                CorrespondentSearchBtn = "";
                CorrespondentSearchBody = "";
                DepartmentSearchType = "";
                DepartmentSearchText = "";
                DepartmentSearchBtn = "";
                DepartmentSearchBody = "";
            }
        }

        public class CSVSectionTable
        {
            public string Title { get; set; }
            public string Company { get; set; }
            public string Workplace { get; set; }
            public string Gubun { get; set; }
            public string Account { get; set; }
            public string TaxDate { get; set; }
            public string Type { get; set; }
            public string Correspondent { get; set; }
            public string Price { get; set; }
            public string Briefs { get; set; }
            public string Department { get; set; }
            public string SupplyPrice { get; set; }

            public CSVSectionTable()
            {
                Title           = "";
                Company         = "";
                Workplace       = "";
                Gubun           = "";
                Account         = "";
                Type            = "";
                Correspondent   = "";
                Price           = "";
                Briefs          = "";
                Department      = "";
                SupplyPrice     = "";
            }
            public CSVSectionTable(string[] data)
            {
                if (data.Count() == 12)
                {
                    Title           = data[0];
                    Company         = data[1];
                    Workplace       = data[2];
                    Gubun           = data[3];
                    Account         = data[4];
                    TaxDate         = data[5];
                    Type            = data[6];
                    Correspondent   = data[7];
                    Price           = data[8];
                    Briefs          = data[9];
                    Department      = data[10];
                    SupplyPrice     = data[11];
                }

                if (Title != "제목" || Company != "회사" || Workplace != "사업장" ||
                    Gubun != "구분" || Account != "계정" || TaxDate != "증빙일자" || Type != "증빙유형" || Correspondent != "거래처" || Price != "금액" ||
                    Briefs != "적요" || Department != "부서" || SupplyPrice != "공급가액")
                {
                    Clear();
                }
            }

            public bool IsEmpty()
            {
                return Title.Length <= 0 || Company.Length <= 0 || Workplace.Length <= 0 ||
                    Gubun.Length <= 0 || Account.Length <= 0 || Type.Length <= 0 || Correspondent.Length <= 0 || Price.Length <= 0 ||
                    Briefs.Length <= 0 || Department.Length <= 0 || SupplyPrice.Length <= 0;
            }

            public void Clear()
            {
                Title           = "";
                Company         = "";
                Workplace       = "";
                Gubun           = "";
                Account         = "";
                TaxDate         = "";
                Type            = "";
                Correspondent   = "";
                Price           = "";
                Briefs          = "";
                Department      = "";
                SupplyPrice     = "";
            }
        }

        public class CompanyModel : ICloneable
        {
            [JsonPropertyName("companyCode")]
            public string CompanyCode { get; set; }
            [JsonPropertyName("companyName")]
            public string CompanyName { get; set; }
            public CompanyModel()
            {
                CompanyCode = "";
                CompanyName = "";
            }
            public CompanyModel(CompanyModel data)
            {
                CompanyCode = data.CompanyCode;
                CompanyName = data.CompanyName;
            }
            public CompanyModel(object data)
            {
                CompanyModel buff = JsonSerializer.Deserialize<CompanyModel>(data.ToString());
                CompanyCode = buff.CompanyCode;
                CompanyName = buff.CompanyName;
            }
            public CompanyModel(DataRow dr)
            {
                CompanyCode = dr[0].ToString();
                CompanyName = dr[1].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_COMPANY_MODEL);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(100))", C_COMPANY_NAME);
                return stb.ToString();
            }
            public static string GetTruncateTable()
            {
                return string.Format("DELETE FROM {0}", TABLE_COMPANY_MODEL);
            }
            public static string GetSelectString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_COMPANY_MODEL);
                stb.AppendFormat("ORDER BY {0} ASC", C_COMPANY_CODE);
                return stb.ToString();
            }
            public string GetInsertString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_COMPANY_MODEL);
                stb.AppendFormat("('{0}', '{1}')", CompanyCode, CompanyName);
                return stb.ToString();
            }

            public string GetCompanyName()
            {
                return string.Format("{0}/{1}", CompanyCode, CompanyName);
            }
            public bool IsEmpty()
            {
                return CompanyCode.Length <= 0 || CompanyName.Length <= 0;
            }
            public void Clear()
            {
                CompanyCode = "";
                CompanyName = "";
            }
            public bool SetCSV(string[] data)
            {
                if (data.Count() >= 3)
                {
                    try
                    {
                        int slashIndex = data[1].IndexOf("/");
                        if (slashIndex == -1)
                        {
                            slashIndex = data[1].Length - 1;
                        }
                        CompanyCode = data[1].Substring(0, slashIndex);
                        CompanyName = data[1].Substring(slashIndex + 1);
                    }
                    catch
                    {
                        CompanyCode = "";
                        CompanyName = "";
                        return false;
                    }
                    return true;
                }

                return false;
            }

            public object Clone()
            {
                return new CompanyModel(this);
            }
        }
        public class WorkplaceModel : ICloneable
        {
            [JsonPropertyName("companyCode")]
            public string CompanyCode { get; set; }
            [JsonPropertyName("workplaceCode")]
            public string WorkplaceCode { get; set; }
            [JsonPropertyName("workplaceName")]
            public string WorkplaceName { get; set; }
            public WorkplaceModel()
            {
                CompanyCode     = "";
                WorkplaceCode   = "";
                WorkplaceName   = "";
            }
            public WorkplaceModel(WorkplaceModel data)
            {
                CompanyCode     = data.CompanyCode;
                WorkplaceCode   = data.WorkplaceCode;
                WorkplaceName   = data.WorkplaceName;
            }
            public WorkplaceModel(object data)
            {
                WorkplaceModel buff = JsonSerializer.Deserialize<WorkplaceModel>(data.ToString());
                CompanyCode     = buff.CompanyCode;
                WorkplaceCode   = buff.WorkplaceCode;
                WorkplaceName   = buff.WorkplaceName;
            }
            public WorkplaceModel(DataRow dr)
            {
                CompanyCode     = dr[0].ToString();
                WorkplaceCode   = dr[1].ToString();
                WorkplaceName   = dr[2].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_WORKPLACE_MODEL);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_WORKPLACE_CODE);
                stb.AppendFormat("{0} NVARCHAR(100))", C_WORKPLACE_NAME);
                return stb.ToString();
            }
            public static string GetTruncateTable()
            {
                return string.Format("DELETE FROM {0}", TABLE_WORKPLACE_MODEL);
            }
            public static string GetSelectString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_WORKPLACE_MODEL);
                stb.AppendFormat("ORDER BY {0}, {1} ASC", C_COMPANY_CODE, C_WORKPLACE_CODE);
                return stb.ToString();
            }
            public string GetInsertString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_WORKPLACE_MODEL);
                stb.AppendFormat("('{0}', '{1}', '{2}')", CompanyCode, WorkplaceCode, WorkplaceName);
                return stb.ToString();
            }

            public string GetWorkplaceName()
            {
                return string.Format("{0}/{1}", WorkplaceCode, WorkplaceName);
            }
            public bool IsEmpty()
            {
                return CompanyCode.Length <= 0 || WorkplaceCode.Length <= 0 || WorkplaceName.Length <= 0;
            }
            public void Clear()
            {
                CompanyCode = "";
                WorkplaceCode = "";
                WorkplaceName = "";
            }
            public bool SetCSV(string companyCode, string[] data)
            {
                if (data.Count() >= 3)
                {
                    try
                    {
                        int slashIndex = data[2].IndexOf("/");
                        if (slashIndex == -1)
                        {
                            slashIndex = data[2].Length - 1;
                        }
                        WorkplaceCode = data[2].Substring(0, slashIndex);
                        WorkplaceName = data[2].Substring(slashIndex + 1);
                    }
                    catch
                    {
                        WorkplaceCode = "";
                        WorkplaceName = "";
                        return false;
                    }
                    CompanyCode = companyCode;
                    return true;
                }

                return false;
            }

            public object Clone()
            {
                return new WorkplaceModel(this);
            }
        }
        public class AccountModel : ICloneable
        {
            [JsonPropertyName("companyCode")]
            public string CompanyCode { get; set; }
            [JsonPropertyName("accountDebit")]
            public string AccountDebit { get; set; }
            [JsonPropertyName("accountCode")]
            public string AccountCode { get; set; }
            [JsonPropertyName("accountName")]
            public string AccountName { get; set; }
            [JsonPropertyName("accountType")]
            public string AccountType { get; set; }
            public AccountModel()
            {
                CompanyCode     = "";
                AccountDebit    = "";
                AccountCode     = "";
                AccountName     = "";
                AccountType     = "";
            }
            public AccountModel(AccountModel data)
            {
                CompanyCode     = data.CompanyCode;
                AccountDebit    = data.AccountDebit;
                AccountCode     = data.AccountCode;
                AccountName     = data.AccountName;
                AccountType     = data.AccountType;
            }
            public AccountModel(object data)
            {
                AccountModel buff = JsonSerializer.Deserialize<AccountModel>(data.ToString());
                CompanyCode     = buff.CompanyCode;
                AccountDebit    = buff.AccountDebit;
                AccountCode     = buff.AccountCode;
                AccountName     = buff.AccountName;
                AccountType     = buff.AccountType;
            }
            public AccountModel(DataRow dr)
            {
                CompanyCode     = dr[0].ToString();
                AccountDebit    = dr[1].ToString();
                AccountCode     = dr[2].ToString();
                AccountName     = dr[3].ToString();
                AccountType     = dr[4].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_ACCOUNT_MODEL);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_ACCOUNT_DEBIT);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_ACCOUNT_CODE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_ACCOUNT_NAME);
                stb.AppendFormat("{0} NVARCHAR(10))", C_ACCOUNT_TYPE);
                return stb.ToString();
            }
            public static string GetTruncateTable()
            {
                return string.Format("DELETE FROM {0}", TABLE_ACCOUNT_MODEL);
            }
            public static string GetSelectString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_ACCOUNT_MODEL);
                stb.AppendFormat("ORDER BY {0}, {1} ASC", C_COMPANY_CODE, C_ACCOUNT_CODE);
                return stb.ToString();
            }
            public string GetInsertString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_ACCOUNT_MODEL);
                stb.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}')", CompanyCode, AccountDebit, AccountCode, AccountName, AccountType);
                return stb.ToString();
            }

            public string GetAccountName()
            {
                return string.Format("{0}/{1}", AccountCode, AccountName);
            }
            public int GetAccountCode()
            {
                int.TryParse(AccountCode, out int ret);
                return ret;
            }
            public bool IsEmpty()
            {
                return CompanyCode.Length <= 0 || AccountCode.Length <= 0 || AccountName.Length <= 0;
            }
            public void Clear()
            {
                CompanyCode = "";
                AccountDebit = "";
                AccountCode = "";
                AccountName = "";
            }
            public void SetData(string data)
            {
                int index = data.IndexOf("/");
                if (index == -1)
                {
                    if (int.TryParse(data, out int ret))
                    {
                        AccountCode = ret.ToString();
                    }
                    else
                    {
                        AccountName = data;
                    }
                }
                else
                {
                    AccountCode = data.Substring(0, index);
                    AccountName = data.Substring(index + 1);
                }
            }

            public object Clone()
            {
                return new AccountModel(this);
            }
        }
        public class CorrespondentModel : ICloneable
        {
            [JsonPropertyName("companyCode")]
            public string CompanyCode { get; set; }
            [JsonPropertyName("correspondentCode")]
            public string CorrespondentCode { get; set; }
            [JsonPropertyName("correspondentName")]
            public string CorrespondentName { get; set; }
            [JsonPropertyName("correspondentType")]
            public string CorrespondentType { get; set; }
            [JsonPropertyName("correspondentNumber")]
            public string CorrespondentNumber { get; set; }
            public CorrespondentModel()
            {
                CompanyCode         = "";
                CorrespondentCode   = "";
                CorrespondentName   = "";
                CorrespondentType   = "";
                CorrespondentNumber = "";
            }
            public CorrespondentModel(CorrespondentModel data)
            {
                CompanyCode         = data.CompanyCode;
                CorrespondentCode   = data.CorrespondentCode;
                CorrespondentName   = data.CorrespondentName;
                CorrespondentType   = data.CorrespondentType;
                CorrespondentNumber = data.CorrespondentNumber;
            }
            public CorrespondentModel(object data)
            {
                CorrespondentModel buff = JsonSerializer.Deserialize<CorrespondentModel>(data.ToString());
                CompanyCode         = buff.CompanyCode;
                CorrespondentCode   = buff.CorrespondentCode;
                CorrespondentName   = buff.CorrespondentName;
                CorrespondentType   = buff.CorrespondentType;
                CorrespondentNumber = buff.CorrespondentNumber;
            }
            public CorrespondentModel(DataRow dr)
            {
                CompanyCode         = dr[0].ToString();
                CorrespondentCode   = dr[1].ToString();
                CorrespondentName   = dr[2].ToString();
                CorrespondentType   = dr[3].ToString();
                CorrespondentNumber = dr[4].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_CORRESPONDENT_MODEL);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_CORRESPONDENT_CODE);
                stb.AppendFormat("{0} NVARCHAR(100), ", C_CORRESPONDENT_NAME);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_CORRESPONDENT_TYPE);
                stb.AppendFormat("{0} NVARCHAR(100))", C_CORRESPONDENT_NUMBER);
                return stb.ToString();
            }
            public static string GetTruncateTable()
            {
                return string.Format("DELETE FROM {0}", TABLE_CORRESPONDENT_MODEL);
            }
            public static string GetSelectString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_CORRESPONDENT_MODEL);
                stb.AppendFormat("ORDER BY {0}, {1} ASC", C_COMPANY_CODE, C_CORRESPONDENT_CODE);
                return stb.ToString();
            }
            public string GetInsertString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_CORRESPONDENT_MODEL);
                stb.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}')", CompanyCode, CorrespondentCode, CorrespondentName, CorrespondentType, CorrespondentNumber);
                return stb.ToString();
            }

            public int GetCorrespondentCode()
            {
                int.TryParse(CorrespondentCode, out int ret);
                return ret;
            }
            public string GetCorrespondentName()
            {
                return string.Format("{0}/{1}", CorrespondentCode, CorrespondentName);
            }
            public int GetCorrespondentType()
            {
                int.TryParse(CorrespondentType, out int ret);
                return ret;
            }
            public bool IsEmpty()
            {
                return CompanyCode.Length <= 0 || CorrespondentCode.Length <= 0 || CorrespondentName.Length <= 0;
            }
            public void Clear()
            {
                CompanyCode         = "";
                CorrespondentCode   = "";
                CorrespondentName   = "";
                CorrespondentNumber = "";
            }
            public void SetData(string data)
            {
                int index = data.IndexOf("/");
                if (index == -1)
                {
                    if (int.TryParse(data, out int ret))
                    {
                        CorrespondentCode = ret.ToString();
                    }
                    else
                    {
                        CorrespondentName = data;
                    }
                }
                else
                {
                    CorrespondentCode = data.Substring(0, index);
                    CorrespondentName = data.Substring(index + 1);
                }
            }

            public object Clone()
            {
                return new CorrespondentModel(this);
            }
        }
        public class DepartmentModel : ICloneable
        {
            [JsonPropertyName("companyCode")]
            public string CompanyCode { get; set; }
            [JsonPropertyName("workplaceCode")]
            public string WorkplaceCode { get; set; }
            [JsonPropertyName("departmentCode")]
            public string DepartmentCode { get; set; }
            [JsonPropertyName("departmentName")]
            public string DepartmentName { get; set; }
            public DepartmentModel()
            {
                CompanyCode     = "";
                WorkplaceCode   = "";
                DepartmentCode  = "";
                DepartmentName  = "";
            }
            public DepartmentModel(DepartmentModel data)
            {
                CompanyCode     = data.CompanyCode;
                WorkplaceCode   = data.WorkplaceCode;
                DepartmentCode  = data.DepartmentCode;
                DepartmentName  = data.DepartmentName;
            }
            public DepartmentModel(object data)
            {
                DepartmentModel buff = JsonSerializer.Deserialize<DepartmentModel>(data.ToString());
                CompanyCode     = buff.CompanyCode;
                WorkplaceCode   = buff.WorkplaceCode;
                DepartmentCode  = buff.DepartmentCode;
                DepartmentName  = buff.DepartmentName;
            }
            public DepartmentModel(DataRow dr)
            {
                CompanyCode     = dr[0].ToString();
                WorkplaceCode   = dr[1].ToString();
                DepartmentCode  = dr[2].ToString();
                DepartmentName  = dr[3].ToString();
            }

            public static string CreateTable()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("CREATE TABLE {0} (", TABLE_DEPARTMENT_MODEL);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_COMPANY_CODE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_WORKPLACE_CODE);
                stb.AppendFormat("{0} NVARCHAR(10), ", C_DEPARTMENT_CODE);
                stb.AppendFormat("{0} NVARCHAR(100))", C_DEPARTMENT_NAME);
                return stb.ToString();
            }
            public static string GetTruncateTable()
            {
                return string.Format("DELETE FROM {0}", TABLE_DEPARTMENT_MODEL);
            }
            public static string GetSelectString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT * FROM {0} ", TABLE_DEPARTMENT_MODEL);
                stb.AppendFormat("ORDER BY {0}, {1} ASC", C_COMPANY_CODE, C_DEPARTMENT_CODE);
                return stb.ToString();
            }
            public string GetInsertString()
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO {0} VALUES ", TABLE_DEPARTMENT_MODEL);
                stb.AppendFormat("('{0}', '{1}', '{2}', '{3}')", CompanyCode, WorkplaceCode, DepartmentCode, DepartmentName);
                return stb.ToString();
            }

            public string GetDepartmentName()
            {
                return string.Format("{0}/{1}", DepartmentCode, DepartmentName);
            }
            public bool IsEmpty()
            {
                return CompanyCode.Length <= 0 || WorkplaceCode.Length <= 0 || DepartmentCode.Length <= 0 || DepartmentName.Length <= 0;
            }
            public void Clear()
            {
                CompanyCode     = "";
                WorkplaceCode   = "";
                DepartmentCode  = "";
                DepartmentName  = "";
            }
            public void SetData(string data)
            {
                int index = data.IndexOf("/");
                if (index == -1)
                {
                    if (int.TryParse(data, out int ret))
                    {
                        DepartmentCode = ret.ToString();
                    }
                    else
                    {
                        DepartmentName = data;
                    }
                }
                else
                {
                    DepartmentCode = data.Substring(0, index);
                    DepartmentName = data.Substring(index + 1);
                }
            }

            public object Clone()
            {
                return new DepartmentModel(this);
            }
        }
        internal class PersonModel : ICloneable
        {
            [JsonPropertyName("seq")]
            public int Seq { get; set; }
            [JsonPropertyName("id")]
            public string ID { get; set; }
            [JsonPropertyName("pw")]
            public string PW { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("entry_date")]
            public string EntryDate { get; set; }
            [JsonPropertyName("resign_date")]
            public string? ResignDate { get; set; }
            [JsonPropertyName("roles")]
            public List<string> Roles { get; set; }

            public PersonModel()
            {
                Seq         = -1;
                ID          = "";
                PW          = "";
                Name        = "";
                EntryDate   = "";
                ResignDate  = "";
                Roles       = new();
            }
            public PersonModel(PersonModel data)
            {
                Seq         = data.Seq;
                ID          = data.ID;
                PW          = data.PW;
                Name        = data.Name;
                EntryDate   = data.EntryDate;
                ResignDate  = data.ResignDate;
                Roles       = data.Roles;
            }
            public PersonModel(object data)
            {
                if (data == null)
                {
                    Seq         = -1;
                    ID          = "";
                    PW          = "";
                    Name        = "";
                    EntryDate   = "";
                    ResignDate  = "";
                    Roles       = new();
                    return;
                }

                try
                {
                    var temp = data.ToString();
                    if (temp != null)
                    {
                        PersonModel? buff = JsonSerializer.Deserialize<PersonModel>(temp);
                        if (buff != null)
                        {
                            Seq = buff.Seq;
                            ID = buff.ID;
                            PW = buff.PW;
                            Name = buff.Name;
                            EntryDate = buff.EntryDate;
                            ResignDate = buff.ResignDate;
                            Roles = buff.Roles;
                            return;
                        }
                    }
                }
                catch
                {
                }

                Seq         = -1;
                ID          = "";
                PW          = "";
                Name        = "";
                EntryDate   = "";
                ResignDate  = "";
                Roles       = new();
            }

            public bool IsEmpty()
            {
                return Seq == -1 || ID == "" || PW == "" || Name == "" || EntryDate == "" || ResignDate == "" || Roles.Count == 0;
            }
            public void Clear()
            {
                Seq         = -1;
                ID          = "";
                PW          = "";
                Name        = "";
                EntryDate   = "";
                ResignDate  = "";
                Roles       = new();
            }
            public object Clone()
            {
                return new PersonModel(this);
            }
        }
    }
}
