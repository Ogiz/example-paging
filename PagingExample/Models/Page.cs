using System.Collections.Generic;

namespace PagingExample.Models
{
    public class Page<T> where T : class
    {
        public int TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}