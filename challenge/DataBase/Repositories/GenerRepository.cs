namespace DataBase.Repositories
{
    public class GenerRepository : BaseRepository<Gener>, IGenerRepository
    {
        public GenerRepository(DisneyContext context) : base(context)
        {
        }

        
    }
}
