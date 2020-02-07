using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NPoco;

namespace PagingExample.Models
{
    public static class QualityControl
    {
        private const string SELECT_CONTACTS =
            "SELECT podatek_meritev_id\n" +
            "    , t.naziv_test\n" +
            "    , ka.izhodna_sarza\n" +
            "    , serija_meritve\n" +
            "    , zap_stevilka_meritve\n" +
            "    , m.vrednost_meritve\n" +
            "    , m.datum_meritve\n" +
            "    , p.oznaka\n" +
            "FROM kon.Podatki_meritve m\n" +
            "    INNER JOIN kon.Parametri p ON m.parameter_si = p.parameter_si\n" +
            "    INNER JOIN kon.Kontrole_artiklov_testi kat ON m.kontrola_artikla_test_id = kat.kontrola_artikla_test_id\n" +
            "    INNER JOIN kon.Kontrole_artiklov Ka ON kat.kontrola_artikla_id = Ka.kontrola_artikla_id\n" +
            "    INNER JOIN kon.Testi T ON kat.test_si = T.test_si";
        
        public static Page<Measurement> GetMeasurementsPage(int pageIndex, int pageSize, string criteria,
            IEnumerable<SortOption> sortOptions)
        {
            using var db = new Database("main");
            var sql = new Sql(SELECT_CONTACTS);

            var pocoData = db.PocoDataFactory.ForType(typeof(Measurement));
            
            SetFiltersCriteria(criteria, pocoData, sql);
            SetSorting(sortOptions, pocoData, sql);
            
            if (pageIndex == 0) pageIndex++;
            if (pageSize == 0) pageSize = 20;
            var page = db.Page<Measurement>(pageIndex, pageSize, sql);
            return new Page<Measurement>
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