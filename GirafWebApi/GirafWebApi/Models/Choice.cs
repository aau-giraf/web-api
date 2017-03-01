using System.Collections.Generic;
using System.Linq;

namespace GirafWebApi.Models {
    public class Choice : Frame { 
        ICollection<PictoFrame> Options { get; set; }

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

        public System.Collections.Generic.IEnumerator<PictoFrame> GetEnumerator() => Options.GetEnumerator();
    }
}