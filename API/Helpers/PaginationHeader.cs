using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationHeader
    {

        public PaginationHeader(int currentPage, int itemPerPage, int totalItems, int totalPages) 
        {
            CurrentPage = currentPage;
            ItemPerPage = itemPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
   
        }
        
        //number of current page
        public int CurrentPage {get; set;}

        //number of total page
        public int ItemPerPage {get; set;}

        //number of page size
        public int TotalItems {get; set;}

         //how many item in this query
        public int TotalPages {get; set;}
    }
}