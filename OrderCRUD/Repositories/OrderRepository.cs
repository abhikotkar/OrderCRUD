using Dapper;
using OrderCRUD.Context;
using OrderCRUD.Entities;
using OrderCRUD.Repositories.Interfaces;

namespace OrderCRUD.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _context;

        public OrderRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Orders>> GetOrders()
        {
            List<Orders> orders = new List<Orders>();
            var query = "Select * from Orders";
            using (var connection = _context.CreateConnection())
            {
                var ordersraw = await connection.QueryAsync<Orders>(query);
                orders = ordersraw.ToList();
                foreach (var order in orders)
                {
                    var orderdetailsrow = await connection.QueryAsync<OrderDetails>(@"Select * from OrderDetails
                                                                                    where orderId=@id",
                                                                                    new { id = order.orderId });
                    order.OrderDetails = orderdetailsrow.ToList();
                }
                return orders;
            }
        }

        public async Task<Orders> GetOrderById(int id)
        {
            Orders order = null;
            var query = "Select * from Orders where orderId=@id";
            using (var connection = _context.CreateConnection())
            {
                var ordersraw = await connection.QueryAsync<Orders>(query, new { id = id });
                order = ordersraw.FirstOrDefault();
                if (order != null)
                {
                    var orderdetailsrow = await connection.QueryAsync<OrderDetails>(@"Select * from OrderDetails
                                                                                    where orderId=@id",
                                                                                    new { id = id });
                    order.OrderDetails = orderdetailsrow.ToList();
                }
                return order;
            }
        }

        public async Task<double> PlaceOrder(Orders order)
        {
            double result1 = 0;
            int result = 0;
            var query = @"insert into Orders(orderCode,custName,mobileNumber,shippingAddress,billingAddress,totalAmount) VALUES
                          (@orderCode,@custName,@mobileNumber,@shippingAddress,@billingAddress,@totalAmount);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
            List<OrderDetails> odlist = new List<OrderDetails>();
            odlist = order.OrderDetails.ToList();

            using (var connection = _context.CreateConnection())
            {
                result = await connection.ExecuteScalarAsync<int>(query, order);
               // if (result != 0)
               // {
                    result1 = await AddProduct(odlist, result);
                    order.totalAmount = result1;
                var qry1 = "UPDATE Orders SET totalAmount=@totalAmount WHERE orderId=@orderId";

                var result3 = await connection.ExecuteAsync(qry1, new { totalAmount = order.totalAmount, orderId = result });
              

                return result1;
            }

        }

        private async Task<double> AddProduct(List<OrderDetails> orders, int result2)
        {
            int result = 0;
            double grandtotal = 0;
            using (var connection = _context.CreateConnection())
            {
                if (orders.Count > 0)
                {
                    foreach (OrderDetails order in orders)
                    {
                        order.orderId = result2;

                        var query = @"insert into OrderDetails(productId,productName,rate,quentity,
                                    orderId) VALUES(@productId,@productName,@rate,
                                     @quentity,@orderId)";

                        var result1 = await connection.ExecuteAsync(query, order);
                        result = result + result1;
                        var pquery = "select rate from OrderDetails where productId = @pid AND orderId=@orderId";
                        var resprice = await connection.QuerySingleAsync<double>(pquery, new { pid = order.productId, orderId = order.orderId });
                        order.totalAmount = resprice * order.quentity;

                        var qry1 = "UPDATE OrderDetails SET totalAmount=@totalAmount WHERE productId = @pid and orderId=@orderId";

                        var result3 = await connection.ExecuteAsync(qry1, new { totalAmount=order.totalAmount,pid = order.productId,orderId = order.orderId });
                        grandtotal = grandtotal + order.totalAmount;
                    }

                }
                return grandtotal;
            }
        }

        public async Task<int> UpdateOrder(Orders order)
        {
            int result = 0;
            var query = @"UPDATE Orders SET orderCode=@orderCode,custName=@custName,mobileNumber=@mobileNumber,
                         shippingAddress=@shippingAddress,billingAddress=@billingAddress WHERE orderId=@orderId";

            using (var connection = _context.CreateConnection())
            {
                result = await connection.ExecuteAsync(query, order);
                if (result != 0)
                {
                    result = await connection.ExecuteAsync(@"delete from OrderDetails where orderId=@orderId"
                                                           , new { orderId = order.orderId });
                    var result1 = await AddProduct(order.OrderDetails, order.orderId);
                    order.totalAmount = result1;
                    var qry1 = "UPDATE Orders SET totalAmount=@totalAmount WHERE orderId=@orderId";

                    var result3 = await connection.ExecuteAsync(qry1, new { totalAmount = order.totalAmount, orderId = order.orderId });


                }
                return result;
            }
        }

        public async Task<int> Delete(int id)
        {

            var query = @"Delete from OrderDetails where orderId=@Id
                          Delete from Orders where orderId=@Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { id = id });
                return result;
            }
        }
    }
}
