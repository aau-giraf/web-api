using GirafRest.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models {
    public class Choice : Frame { 
        public ICollection<PictoFrame> Options { get; private set; }

        protected Choice () {}
        public Choice(List<PictoFrame> options)
        {
            this.Options = options;
        }

        public void Add (PictoFrame option) => Options.Add(option);
        public void AddAll(ICollection<PictoFrame> options) {
            foreach (var pf in options) {
                options.Add(pf);
            }
        }

        public PictoFrame Get(int index) => Options.ElementAt(index);

        public void Remove(PictoFrame pictoframe) => Options.Remove(pictoframe);

        public void Clear() => Options.Clear();

        public IEnumerator<PictoFrame> GetEnumerator() => Options.GetEnumerator();

        public virtual void Merge(ChoiceDTO other)
        {
            this.Options = other.Options;
        }
    }
}