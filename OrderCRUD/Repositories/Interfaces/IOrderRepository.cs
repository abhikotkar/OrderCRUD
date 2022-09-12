using OrderCRUD.Entities;

namespace OrderCRUD.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Orders>> GetOrders();
        public Task<Orders> GetOrderById(int id);
        public Task<double> PlaceOrder(Orders order);
        public Task<int> UpdateOrder(Orders order);

        public Task<int> Delete(int id);
    }
}
