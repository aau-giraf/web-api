using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models {
    public abstract class Frame {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime lastEdit { get; set; }

        private ICollection<Sequence> partOfSequences { get; set; }

        public Frame()
        {
            partOfSequences = new List<Sequence>();
        }
    }
}