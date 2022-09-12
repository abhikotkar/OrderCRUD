using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderCRUD.Entities
{
    [Table("OrderDetails")]
    public class OrderDetails
    {
        [Key]
        public int detailsId { get; set; }
        public long productId { get; set; }
        public string? productName { get; set; }
        public double rate { get; set; }
        public int quentity { get; set; }
        public double totalAmount { get; set; }
        public int orderId { get; set; }
    }
}
