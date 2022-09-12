using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderCRUD.Entities
{
    [Table("Orders")]
    public class Orders
    {
        
        
            [Key]
            public int orderId { get; set; }
            public long orderCode { get; set; }
            public string? custName { get; set; }
            public string? mobileNumber { get; set; }
            public string? shippingAddress { get; set; }
            public string? billingAddress { get; set; }
            public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
            public double totalAmount { get; set; }
        }

     /*   public class OrderForPlaceDto
        {
            [Key]
            public long orderCode { get; set; }
            public string? custName { get; set; }
            public string? mobileNumber { get; set; }
            public string? shippingAddress { get; set; }
            public string? billingAddress { get; set; }
            public List<OrderDetails>? OrderDetails { get; set; }
            public double totalAmount { get; set; }
        }*/
       /* public class OrderForUpdateDto : OrderForPlaceDto
        {
            [Key]
            public int orderId { get; set; }
        }*/
    }

