namespace MicroserviceDB
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public BaseEntity() 
        { 
            Id = Guid.NewGuid();
        }
    }
}
