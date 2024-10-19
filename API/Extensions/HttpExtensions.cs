using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, 
        int currentPage, int itemPerPage, int totalItems, int totalPages)
        {
              var paginationHeader = new PaginationHeader
              (
                 currentPage,
                  itemPerPage,
                  totalItems,
                 totalPages
              );

             var options = new JsonSerializerOptions 
             {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
             };
                //custom header 
              response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));

              //And becuase we are adding a custom header "Pagination" we need to bring cors header below 
              //to make the "Pagination" custom header available

              /*Cross-Origin Resource Sharing (CORS) is an HTTP-header based 
              mechanism that allows a server to indicate any origins (domain, scheme, 
              or port) other than its own from which a browser should permit loading resources.
              */
              response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); //key, name of the header we are exposing
        }
    }
}