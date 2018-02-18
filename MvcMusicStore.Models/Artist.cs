namespace MvcMusicStore.Models
{
    public class Artist : AbstractEntity<int>//XUnionedWithY - union-subclassing, XJoinedWithY - join-subclassing,XSharedWithY - discriminator-subclassing
    {
        public virtual string Name { get; set; }
    }
}