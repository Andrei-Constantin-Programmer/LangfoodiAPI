namespace RecipeSocialMediaAPI.DAL.Repositories
{
    public class Repository : IRepository
    {
        protected string _collectionName = "";
        
        public string CollectionName
        {
            get { return _collectionName; }
        }
    }
}
