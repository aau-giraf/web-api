namespace GirafRest.Data
{
    public interface ID : IDisposable
    {
        void Get();
        void GetAll();
        void Insert();
        void Delete();
        void Update();
        void Save();
    }
}