using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoonAdminMvcCore.Models
{
    public class PaginatedList<T> :List<T>
    {
        public int pageIndex { set; get; }
        public int TotalPages { set; get; }

        public PaginatedList(List<T> items,int count,int _pageindex,int pagesize)
        {
            pageIndex = _pageindex;
            TotalPages = (int)Math.Ceiling(count / (double)pagesize);
            this.AddRange(items);
        }

        public bool previousPage
        {
            get { return pageIndex > 1; }
        }
        public bool NextPage
        {
            get { return pageIndex <TotalPages; }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source,int pageindex,int pagesize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageindex - 1) * pagesize).Take(pagesize).ToListAsync();
            return new PaginatedList<T>(items, count, pageindex, pagesize);
        }
    }
}
