using System.Text.Json.Serialization;

namespace SimpleService;

public class Entity
{
    [JsonPropertyName("id")]
    public Guid Id { get; }
    [JsonPropertyName("operationDate")]
    public DateTime OperationDate { get; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    public Entity()
    {
        Id = Guid.NewGuid();
        OperationDate = DateTime.Now;
    }

    [JsonConstructor]
    public Entity(Guid id, DateTime operationDate, decimal amount)
    {
        Id = id;
        OperationDate = operationDate;
        Amount = amount;
    }
}