using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult(IEnumerable<T> Data, int TotalCount, int PageNumber, int PageSize)
        {
            this.Data = Data;
            this.PageNumber = PageNumber;
            this.PageSize = PageSize;
            this.TotalCount = TotalCount;
        }
        public PagedResult()
        {
            
        }
    }

}
