using GirafRest.Models.DTOs;
using GirafRest.Models.Many_to_Many_Relationships;
using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models {
    public class Choice : Frame { 
        public ICollection<ChoiceResource> Options { get; private set; }

        public Choice () {}
        public Choice(List<PictoFrame> options)
        {
            Options = new List<ChoiceResource>();
            foreach (PictoFrame option in options)
            {
                Options.Add(new ChoiceResource(this, option));
            }
        }

        public void Add (PictoFrame option) => Options.Add(new ChoiceResource(this, option));
        public void AddAll(ICollection<PictoFrame> options) {
            foreach (var option in options) {
                Options.Add(new ChoiceResource(this, option));
            }
        }

        public PictoFrame Get(int index) => (PictoFrame) Options.ElementAt(index).Resource;

        public void Remove(PictoFrame pictoframe) => Options.Remove(new ChoiceResource(this, pictoframe));

        public void Clear() => Options.Clear();

        public IEnumerator<PictoFrame> GetEnumerator()
        {
            List<PictoFrame> pictoFrameList = new List<PictoFrame>();
            foreach (var choiceResource in Options)
            {
                pictoFrameList.Add((PictoFrame) choiceResource.Resource);
            }
            return pictoFrameList.GetEnumerator();
        }
    }
}