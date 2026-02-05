using System.ComponentModel.DataAnnotations.Schema;


namespace API.Entities;


[Table("BasketItems")]
public class BasketItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int ProductId { get; set; } //Foreign key property for Product
    public Product Product { get; set; } = null!; //Navigation property between Product and BasketItem

    public int BasketId { get; set; } //Foreign key property for Basket
    public Basket Basket { get; set; } = null!; //Navigation property between Basket and BasketItem
}