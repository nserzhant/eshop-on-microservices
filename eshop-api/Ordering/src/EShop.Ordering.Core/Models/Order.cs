using EShop.Ordering.Core.Exceptions;

namespace EShop.Ordering.Core.Models;

public enum OrderStatus { Created, Shipped, Delivered, Returned }

public class Order : BaseEntity
{
    private List<OrderItem> _orderItems;

    public OrderStatus OrderStatus { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerEmail { get; private set; }
    public string ShippingAddress { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    public Order(DateTime orderDate, Guid customerId, string customerEmail, string shippingAddress, List<OrderItem> orderItems)
    {
        if (orderItems.Count == 0)
        {
            throw new NoOrderItemsExceptions();
        }

        _orderItems = orderItems;

        OrderStatus = OrderStatus.Created;
        OrderDate = orderDate;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        ShippingAddress = shippingAddress;
    }

    public void Ship()
    {
        if (OrderStatus == OrderStatus.Created)
        {
            OrderStatus = OrderStatus.Shipped;
        }
        else
        {
            throw new InvalidOrderStatusException();
        }
    }

    public void Deliver()
    {
        if (OrderStatus == OrderStatus.Shipped)
        {
            OrderStatus = OrderStatus.Delivered;
        }
        else
        {
            throw new InvalidOrderStatusException();
        }
    }

    public void Return()
    {
        if (OrderStatus == OrderStatus.Delivered)
        {
            OrderStatus = OrderStatus.Returned;
        }
        else
        {
            throw new InvalidOrderStatusException();
        }
    }
}
