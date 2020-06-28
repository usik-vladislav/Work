namespace CountriesLibrary.Repositories
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
