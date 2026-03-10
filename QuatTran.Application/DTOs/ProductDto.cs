using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuatTran.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm bắt buộc")]
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Chọn danh mục")]
        public int? CategoryId { get; set; }
        public List<ProductImageDto> AdditionalImages { get; set; } = new List<ProductImageDto>();
    }

}
