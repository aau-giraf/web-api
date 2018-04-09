using System;
namespace GirafRest
{
    public class WeekNameDTO
    {
        /// <summary>
        /// A Name describing the week.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The id of the week.
        /// </summary>

        public long Id { get; set; }

        public WeekNameDTO(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
