using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuatTran.Application.DTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }

        public int UserId { get; set; }

        public int? ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime DateCreated { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }

    }
}
