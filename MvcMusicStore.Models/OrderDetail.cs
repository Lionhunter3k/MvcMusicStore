namespace MvcMusicStore.Models
{
    public class OrderDetail : AbstractEntity<int>
    {
        public virtual int Quantity { get; set; }
        
        public virtual decimal UnitPrice { get; set; }

        public virtual Album Album { get; set; }
        
        public virtual Order Order { get; set; }
    }
}
