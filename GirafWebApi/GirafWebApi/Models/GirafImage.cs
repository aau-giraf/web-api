
using GirafWebApi.Models.Persistence;

namespace GirafWebApi.Models{
    public class GirafImage : PersistFileHandle {
        public GirafImage() {}

        public GirafImage(string filePath) : base (filePath) {}
    }
}