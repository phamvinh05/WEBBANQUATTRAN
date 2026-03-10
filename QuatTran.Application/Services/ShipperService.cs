using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuatTran.Application.DTOs;
using QuatTran.Domain.Interfaces;
using QuatTran.Application.Interfaces;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Services
{
    public class ShipperService : IShipperService
    {
        public readonly IRepository<Shipper> _repository;
        public ShipperService(IRepository<Shipper> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<ShipperDto>> GetAllShippersAsync()
        {
            var shippers = await _repository.GetAllAsync();
            return shippers.Select(s => new ShipperDto
            {
                ShipperId = s.ShipperId,
                CompanyName = s.CompanyName,
                Phone = s.Phone
            }).ToList();
        }
        public async Task<ShipperDto?> GetShipperByIdAsync(int shipperId)
        {
            var shipper = await _repository.GetByIdAsync(shipperId);
            if (shipper == null) return null;

            return new ShipperDto
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                Phone = shipper.Phone
            };
        }

        public async Task<int> AddShipperAsync(ShipperDto shipperDto)
        {
            var shipper = new Shipper
            {
                CompanyName = shipperDto.CompanyName,
                Phone = shipperDto.Phone
            };
            await _repository.AddAsync(shipper);
            await _repository.SaveChangesAsync();
            return shipper.ShipperId;
        }
        public async Task UpdateShipper(ShipperDto shipperDto)
        {
            var shipper = await _repository.GetByIdAsync(shipperDto.ShipperId);
            if (shipper != null)
            {
                shipper.CompanyName = shipperDto.CompanyName;
                shipper.Phone = shipperDto.Phone;
                await _repository.UpdateAsync(shipper);
                await _repository.SaveChangesAsync();
            }
        }
        public async Task DeleteShipperAsync(int shipperId)
        {
            var shipper = await _repository.GetByIdAsync(shipperId);
            if (shipper != null)
            {
                await _repository.DeleteAsync(shipper);
                await _repository.SaveChangesAsync();
            }

        }
    }
}
