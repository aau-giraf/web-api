using System.Collections.Generic;

public class Department {
    public long Key { get; set; }

    public string Name { get; private set; }

    public ICollection<User> members { get; set; }

    protected Department () {}
}