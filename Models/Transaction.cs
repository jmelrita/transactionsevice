public class Transaction
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; }
    public long Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Name { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Symbol { get; set; }
    public string OrderSide { get; set; }
    public string OrderStatus { get; set; }
}
