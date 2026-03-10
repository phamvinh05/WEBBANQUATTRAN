using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuatTran.Application.DTOs;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Interfaces
{
    public interface IShipperService
    {
        Task<IEnumerable<ShipperDto>> GetAllShippersAsync();
        Task<ShipperDto> GetShipperByIdAsync(int shipperId);
        Task<int> AddShipperAsync(ShipperDto shipperDto);
        Task UpdateShipper(ShipperDto shipperDto);
        Task DeleteShipperAsync(int shipperId);
    }
}
