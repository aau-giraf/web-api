namespace GirafRest.Models
{
    public interface IManyToMany<TKey, T>
    {
        long Key {get; set;}
        long ResourceKey { get; set; }
        Frame Resource { get; set; }

        TKey OtherKey { get; set; }
        T Other { get; set; }
    }
}