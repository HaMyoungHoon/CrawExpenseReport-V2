using BaseLib_Net6;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace CrawExpenseReport.Base.dbSQL
{
    public partial class FSqlite
    {
        private FPrintf _log;
        protected string _dbPath;
        public FSqlite(string dbPath)
        {
            _dbPath = dbPath;
            _log = new FPrintf(@"Log\Tongsin", "sqlite");
        }
        protected static void CheckFolder(string folerPath)
        {
            DirectoryInfo di = new DirectoryInfo(folerPath);

            if (di.Exists == false)
            {
                di.Create();
            }
        }
        protected bool CreateDBFile(string dbPath = "")
        {
            dbPath = CheckPath(dbPath);

            try
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            catch (Exception ex)
            {
                _log.PRINT_F(ex.ToString());
                return false;
            }
            return true;
        }
        protected bool CreateTable(string cmd, string dbPath = "")
        {
            dbPath = CheckPath(dbPath);
            var sqlCon = new SQLiteConnection(string.Format("Data Source={0}", dbPath));
            try
            {
                sqlCon.Open();
                var sqlCmd = new SQLiteCommand(cmd, sqlCon);
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.PRINT_F(ex.ToString());
                return false;
            }

            sqlCon.Close();

            return true;

        }


        public bool InsertData(string cmd, string dbPath = "")
        {
            dbPath = CheckPath(dbPath);
            if (cmd.Length == 0)
            {
                return false;
            }

            var sqlCon = new SQLiteConnection(string.Format("Data Source={0}", dbPath));
            try
            {
                sqlCon.Open();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                int index = error.IndexOf("\n");
                if (index == -1)
                {
                    index = error.Length;
                }
                _log.PRINT_F(error.Substring(0, index));
                return false;
            }
            var sqlCmd = new SQLiteCommand(sqlCon);

            try
            {
                using (var transaction = sqlCon.BeginTransaction())
                {
                    if (cmd.Contains("SELECT") == false)
                    {
                        _log.PRINT_F(cmd);
                    }
                    sqlCmd.CommandText = cmd;
                    sqlCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                int index = error.IndexOf("\n");
                if (index == -1)
                {
                    index = error.Length;
                }
                _log.PRINT_F(error.Substring(0, index));
                sqlCon.Close();
                return false;
            }
            sqlCon.Close();
            return true;
        }
        public bool InsertListData(List<string> cmd, string dbPath = "")
        {
            dbPath = CheckPath(dbPath);
            if (cmd.Count == 0)
            {
                return false;
            }

            var sqlCon = new SQLiteConnection(string.Format("Data Source={0}", dbPath));
            try
            {
                sqlCon.Open();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                int index = error.IndexOf("\n");
                if (index == -1)
                {
                    index = error.Length;
                }
                _log.PRINT_F(error.Substring(0, index));
                return false;
            }
            var sqlCmd = new SQLiteCommand(sqlCon);

            try
            {
                using (var transaction = sqlCon.BeginTransaction())
                {
                    foreach (string item in cmd)
                    {
                        sqlCmd.CommandText = item;
                        sqlCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                int index = error.IndexOf("\n");
                if (index == -1)
                {
                    index = error.Length;
                }
                _log.PRINT_F(error.Substring(0, index));
                sqlCon.Close();
                return false;
            }
            sqlCon.Close();
            return true;
        }
        public bool FindData(string cmd, out DataSet returnData, string dbPath = "")
        {
            returnData = new DataSet();
            if (cmd.Length == 0)
            {
                return false;
            }
            dbPath = CheckPath(dbPath);
            var sqlCon = new SQLiteConnection(string.Format("Data Source={0}", dbPath));
            try
            {
                sqlCon.Open();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                int index = error.IndexOf("\n");
                if (index == -1)
                {
                    index = error.Length;
                }
                _log.PRINT_F(error.Substring(0, index));
                return false;
            }

            var adpt = new SQLiteDataAdapter(cmd, sqlCon);
            if (cmd.Contains("SELECT") == false)
            {
                _log.PRINT_F(cmd);
            }

            try
            {
                adpt.Fill(returnData);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                int index = error.IndexOf("\n");
                if (index == -1)
                {
                    index = error.Length;
                }
                _log.PRINT_F(error.Substring(0, index));
                sqlCon.Close();
                return false;
            }
            sqlCon.Close();
            return true;
        }

        protected string CheckPath(string dbPath)
        {
            if (dbPath == null)
            {
                dbPath = _dbPath;
            }
            if (dbPath.Length <= 0)
            {
                dbPath = _dbPath;
            }

            return dbPath;
        }
    }
}
