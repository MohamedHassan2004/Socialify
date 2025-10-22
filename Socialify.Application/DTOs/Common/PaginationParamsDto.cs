using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Common
{
    public class PaginationParamsDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string CurrentUserId { get; set; }
    }
}
