using System.Collections.Generic;

namespace GirafWebApi
{
    public class Department {
        public long Key { get; set; }

        public string Name { get; private set; }

        public ICollection<GirafUser> members { get; set; }

        protected Department () {}
    }
}