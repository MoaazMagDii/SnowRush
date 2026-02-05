namespace API.Entities;

public class Basket
{
    public int Id { get; set; }
    public required string BasketId { get; set; }
    public List <BasketItem> Items { get; set; } = [];



    public void AddItem(Product product, int quantity)
    {
        var existingItem = FindItem(product.Id);
        if(existingItem != null)
        {
            existingItem.Quantity += quantity; // Update the quantity if the item already exists
        }
        else
        {
            Items.Add(new BasketItem {
                Product = product,
                Quantity = quantity
            });
        }
      
    }

    public void RemoveItem(int productId, int quantity)
    {

        var item = FindItem(productId);
        if (item == null) return;
        item.Quantity -= quantity;

        if (item.Quantity <= 0) Items.Remove(item); // Remove the item if the quantity is zero or less
        
       
    }

    private BasketItem? FindItem(int id)
    {
        return Items.FirstOrDefault(item => item.ProductId == id); 
    }
}
