using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.dbSQL
{
    public partial class FSqlite_Command : FSqlite
    {
        public FSqlite_Command(string dbPath) : base(dbPath)
        {

        }

        public bool CreateDB(string dbPath = "")
        {
            if (dbPath == "")
            {
                dbPath = _dbPath;
            }
            string diPath = dbPath;
            dbPath = CheckPath(dbPath);
            DirectoryInfo di = new DirectoryInfo(dbPath);
            CheckFolder(diPath.Replace(di.Name, ""));

            if (File.Exists(dbPath) == false)
            {
                if (CreateDBFile(dbPath) == false)
                {
                    return false;
                }
            }

            string msg = _selectTables;
            FindData(msg, out DataSet returnData);

            int tableIndex = 0;
            if (returnData.Tables.Count > 0)
            {
                foreach (DataRow dr in returnData.Tables[0].Rows)
                {
                    if (dr["name"].ToString().Equals(FBaseFunc.TABLE_COMPANY_MODEL) == true)
                    {
                        tableIndex |= 0x0001;
                    }
                    else if (dr["name"].ToString().Equals(FBaseFunc.TABLE_WORKPLACE_MODEL) == true)
                    {
                        tableIndex |= 0x0002;
                    }
                    else if (dr["name"].ToString().Equals(FBaseFunc.TABLE_ACCOUNT_MODEL) == true)
                    {
                        tableIndex |= 0x0004;
                    }
                    else if (dr["name"].ToString().Equals(FBaseFunc.TABLE_CORRESPONDENT_MODEL) == true)
                    {
                        tableIndex |= 0x0008;
                    }
                    else if (dr["name"].ToString().Equals(FBaseFunc.TABLE_DEPARTMENT_MODEL) == true)
                    {
                        tableIndex |= 0x0010;
                    }
                    else if (dr["name"].ToString().Equals(FBaseFunc.TABLE_COPY_HEADER_MODEL) == true)
                    {
                        tableIndex |= 0x0020;
                    }
                    else if (dr["name"].ToString().Equals(FBaseFunc.TABLE_COPY_BODY_MODEL) == true)
                    {
                        tableIndex |= 0x0040;
                    }
                }
            }

            if ((tableIndex & 0x0001) == 0)
            {
                CreateTable(FBaseFunc.CompanyModel.CreateTable(), dbPath);
            }
            if ((tableIndex & 0x0002) == 0)
            {
                CreateTable(FBaseFunc.WorkplaceModel.CreateTable(), dbPath);
            }
            if ((tableIndex & 0x0004) == 0)
            {
                CreateTable(FBaseFunc.AccountModel.CreateTable(), dbPath);
            }
            if ((tableIndex & 0x0008) == 0)
            {
                CreateTable(FBaseFunc.CorrespondentModel.CreateTable(), dbPath);
            }
            if ((tableIndex & 0x0010) == 0)
            {
                CreateTable(FBaseFunc.DepartmentModel.CreateTable(), dbPath);
            }
            if ((tableIndex & 0x0020) == 0)
            {
                CreateTable(FBaseFunc.CopyDataTable.CreateTable(), dbPath);
            }
            if ((tableIndex & 0x0040) == 0)
            {
                CreateTable(FBaseFunc.SlipTable.CreateTable(), dbPath);
            }

            return true;
        }

        public bool TruncateTable()
        {
            int err = 0;
            if (!InsertData(FBaseFunc.CompanyModel.GetTruncateTable())) err++;
            if (!InsertData(FBaseFunc.WorkplaceModel.GetTruncateTable())) err++;
            if (!InsertData(FBaseFunc.AccountModel.GetTruncateTable())) err++;
            if (!InsertData(FBaseFunc.CorrespondentModel.GetTruncateTable())) err++;
            if (!InsertData(FBaseFunc.DepartmentModel.GetTruncateTable())) err++;

            if (err != 0)
            {
                return false;
            }

            return true;
        }
        public List<FBaseFunc.CompanyModel> SelectCompanyModel()
        {
            List<FBaseFunc.CompanyModel> buff = new List<FBaseFunc.CompanyModel>();
            if (FindData(FBaseFunc.CompanyModel.GetSelectString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    FBaseFunc.CompanyModel temp = new FBaseFunc.CompanyModel(dr);
                    buff.Add(temp);
                }
            }

            return buff;
        }
        public List<FBaseFunc.WorkplaceModel> SelectWorkplaceModel()
        {
            List<FBaseFunc.WorkplaceModel> buff = new List<FBaseFunc.WorkplaceModel>();
            if (FindData(FBaseFunc.WorkplaceModel.GetSelectString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    FBaseFunc.WorkplaceModel temp = new FBaseFunc.WorkplaceModel(dr);
                    buff.Add(temp);
                }
            }

            return buff;
        }
        public List<FBaseFunc.AccountModel> SelectAccountModel()
        {
            List<FBaseFunc.AccountModel> buff = new List<FBaseFunc.AccountModel>();
            if (FindData(FBaseFunc.AccountModel.GetSelectString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    FBaseFunc.AccountModel temp = new FBaseFunc.AccountModel(dr);
                    buff.Add(temp);
                }
            }

            return buff;
        }
        public List<FBaseFunc.CorrespondentModel> SelectCorrespondentModel()
        {
            List<FBaseFunc.CorrespondentModel> buff = new List<FBaseFunc.CorrespondentModel>();
            if (FindData(FBaseFunc.CorrespondentModel.GetSelectString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    FBaseFunc.CorrespondentModel temp = new FBaseFunc.CorrespondentModel(dr);
                    buff.Add(temp);
                }
            }

            return buff;
        }
        public List<FBaseFunc.DepartmentModel> SelectDepartmentModel()
        {
            List<FBaseFunc.DepartmentModel> buff = new List<FBaseFunc.DepartmentModel>();
            if (FindData(FBaseFunc.DepartmentModel.GetSelectString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    FBaseFunc.DepartmentModel temp = new FBaseFunc.DepartmentModel(dr);
                    buff.Add(temp);
                }
            }

            return buff;
        }
        public bool InsertCompanyModel(List<FBaseFunc.CompanyModel> data)
        {
            return InsertListData(data.Select(x => x.GetInsertString()).ToList());
        }
        public bool InsertWorkplaceModel(List<FBaseFunc.WorkplaceModel> data)
        {
            return InsertListData(data.Select(x => x.GetInsertString()).ToList());
        }
        public bool InsertAccountModel(List<FBaseFunc.AccountModel> data)
        {
            return InsertListData(data.Select(x => x.GetInsertString()).ToList());
        }
        public bool InsertCorrespondentModel(List<FBaseFunc.CorrespondentModel> data)
        {
            return InsertListData(data.Select(x => x.GetInsertString()).ToList());
        }
        public bool InsertDepartmentModel(List<FBaseFunc.DepartmentModel> data)
        {
            return InsertListData(data.Select(x => x.GetInsertString()).ToList());
        }
        
        public List<FBaseFunc.CopyDataTable> SelectCopyCompany()
        {
            List<FBaseFunc.CopyDataTable> buff = new List<FBaseFunc.CopyDataTable>();
            if (FindData(FBaseFunc.CopyDataTable.GetSelectHeaderString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    buff.Add(new FBaseFunc.CopyDataTable(dr));
                }
            }

            return buff;
        }
        public List<FBaseFunc.SlipTable> SelectCopySlipTable()
        {
            List<FBaseFunc.SlipTable> buff = new List<FBaseFunc.SlipTable>();
            if (FindData(FBaseFunc.SlipTable.GetSelectString(), out DataSet ret) == false)
            {
                return buff;
            }

            if (ret.Tables.Count > 0)
            {
                foreach (DataRow dr in ret.Tables[0].Rows)
                {
                    buff.Add(new FBaseFunc.SlipTable(dr));
                }
            }

            return buff;
        }
        public bool InsertCopyCompany(FBaseFunc.CopyDataTable data)
        {
            InsertData(data.GetTruncateHeaderString());
            return InsertData(data.GetInsertHeaderString());
        }
        public bool InsertCopySlipTable(List<FBaseFunc.SlipTable> data)
        {
            InsertData(data[0].GetTruncateString());
            return InsertListData(data.Select(x => x.GetInsertString()).ToList());
        }
    }
}
