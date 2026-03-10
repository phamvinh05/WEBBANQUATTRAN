using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuatTran.Infrastructure;

namespace QuatTran.Application.DTOs
{
    public class ShipperDto
    {
        public int ShipperId { get; set; }

        [Required(ErrorMessage = "Tên công ty bắt buộc nhập")]
        public string? CompanyName { get; set; }

        public string? Phone { get; set; }
    }
}
