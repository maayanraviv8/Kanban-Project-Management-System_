using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DAO
{
    internal abstract class absDAO<TKey>
    {
        internal abstract TKey PrimaryKey { get; }

        internal abstract string Primarycolumn { get; }
        internal abstract List<string> ColumnNames { get; }
        internal abstract string _tableName { get; }

        internal bool Ispersisted = false;

    }
}
