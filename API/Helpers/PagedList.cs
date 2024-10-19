using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        //number of current page
        public int CurrentPage {get; set;}

        //number of total page
        public int TotalPages {get; set;}

        //number of page size
        public int PageSize {get; set;}

         //how many item in this query
        public int TotalCount {get; set;}
         
         //ctor to get access to our class
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count/ (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, 
        int pageNumber, int pageSize)
        {
             //pagination query with calculation
            var count = await source.CountAsync();
            var items = source.Skip((pageNumber -1) * pageSize).Take(pageSize);
            var itemCount = await items.ToListAsync();
            return new PagedList<T>(itemCount, count, pageNumber, pageSize);
        }
    }
}