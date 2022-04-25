using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Base.dbSQL
{
    public partial class FSqlite_Command
    {
        private const string _selectTables = "SELECT name FROM SQLITE_MASTER WHERE TYPE='table'";
    }
}
