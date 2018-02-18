

using Iesi.Collections.Generic;
namespace MvcMusicStore.Models
{
    public class Genre : AbstractEntity<int>
    {
        public Genre()
        {
            this.Albums = new HashedSet<Album>();
        }

        public virtual string Name { get; set; }
        
        public virtual string Description { get; set; }
        
        public virtual ISet<Album> Albums { get; set; }//

    }
}
