namespace GirafRepositories.Interfaces
{
    
    public interface IImageRepository : IRepository<Byte[]>
    {
        Task<byte[]> ReadRequestImage(Stream bodyStream);
    }
}
