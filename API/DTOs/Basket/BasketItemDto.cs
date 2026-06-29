namespace API.DTOs;

public class BasketItemDto
{
    public int Quantity { get; set; }
    public required ProductDto Product { get; set; }

}
