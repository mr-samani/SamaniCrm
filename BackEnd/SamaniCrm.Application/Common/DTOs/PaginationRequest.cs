using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SamaniCrm.Application.Common.DTOs
{
    public class PaginationRequest
    {


        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
        [DefaultValue("")]
        public string? SortBy { get; set; } = string.Empty;
        [DefaultValue("asc")]
        public string? SortDirection { get; set; } = "asc";
    }


}
