using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class Frame {
    public long Key { get; set; }
    protected DateTime lastEdit { get; set; }

    private ICollection<Sequence> partOfSequences { get; set; }
}