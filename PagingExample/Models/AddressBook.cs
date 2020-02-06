using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NPoco;

namespace PagingExample.Models
{
    public static class AddressBook
    {
        private const string SELECT_CONTACTS =
        "SELECT sfppos_index\n" +
        "    \t, sfppos_ime\n" +
        "    \t, sfppos_priimek\n" +
        "    \t, sfppa_kopis\n" +
        "FROM sif_poslposebe\n" +
        "    \tLEFT JOIN sif_poslpar ON sif_poslposebe.sfppos_poslpar = sif_poslpar.sfppa_sifra";
        
        public static Page<Contact> GetContactPage(int pageIndex, int pageSize, string criteria,
            IEnumerable<SortOption> sortOptions)
        {
            using var db = new Database("main");
            var sql = new Sql(SELECT_CONTACTS);

            var pocoData = db.PocoDataFactory.ForType(typeof(Contact));
            
            SetFiltersCriteria(criteria, pocoData, sql);
            SetSorting(sortOptions, pocoData, sql);
            
            if (pageIndex == 0) pageIndex++;
            if (pageSize == 0) pageSize = 20;
            var page = db.Page<Contact>(pageIndex, pageSize, sql);
            return new Page<Contact>
            {
                Items = page.Items,
                TotalItems = (int) page.TotalItems
            };
        }
        
        private static void SetSorting(IEnumerable<SortOption> sortOptions, PocoData pocoData, Sql sql)
        {
            if (sortOptions?.Any() == true)
            {
                var sortedFields = sortOptions.OrderBy(x => x.SortIndex).ToArray()
                    .Select(x => $"{GetColumnName(pocoData, x.Field)} {GetSortOrder(x.SortOrder)}");
                sql.Append($"ORDER BY {string.Join(",", sortedFields)}");
            }
        }

        private static void SetFiltersCriteria(string criteria, PocoData pocoData, Sql sql)
        {
            if (string.IsNullOrEmpty(criteria) || criteria.Equals("null")) return;
            foreach (var c in pocoData.AllColumns)
                criteria = Regex.Replace(criteria, c.MemberInfoKey, c.ColumnName, RegexOptions.IgnoreCase);
            sql.Where(criteria);
        }
        
        private static string GetColumnName(PocoData pocoData, string fieldName)
        {
            return pocoData.AllColumns.Find(x => x.MemberInfoKey.Equals(fieldName)).ColumnName;
        }
        
        private static string GetSortOrder(string sortOrder)
        {
            return sortOrder.StartsWith("A") ? "ASC" : "DESC";
        }
    }
}