namespace PostServer
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class PostContext : DbContext
    {
        public PostContext()
            : base("name=PostContext")
        {
        }        
        public DbSet<PostInfo> PostInfos { set; get; }
    }
}