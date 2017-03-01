using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi.Models.Persistence
{
    public class PersistFileHandle
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        public string FilePath { get; private set; }

        public PersistFileHandle(string filepath)
        {
            this.FilePath = filepath;
        }
        public PersistFileHandle() { }

    }
}