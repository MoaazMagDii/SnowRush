using System.ComponentModel.DataAnnotations.Schema;
namespace API.Entities.OrderAggregate;

[Table("OrderItems")]
public class OrderItem
{
    public int Id { get; set; }
    public required string ProductName { get; set; }
    public int ProductId { get; set; }
    public required string PictureUrl { get; set; }
    public long Price { get; set; }
    public int Quantity { get; set; }
}
