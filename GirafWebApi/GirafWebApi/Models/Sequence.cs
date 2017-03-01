using System.Collections.Generic;
using System.Linq;

namespace GirafWebApi.Models {
    public class Sequence : PictoFrame { 
        private ICollection<Frame> _elements {get; set;}
        public Pictogram Thumbnail { get; set; }

        protected Sequence () {}
        public Sequence(string title, AccessLevel accessLevel, long deparment_key, string username, Pictogram thumbnail)
        : this(title, accessLevel, deparment_key, username, thumbnail, new List<Frame>())
        {
            Thumbnail = thumbnail;

        }
        public Sequence(string title, AccessLevel accessLevel, long deparment_key, 
            string username, Pictogram thumbnail, ICollection<Frame> elements)
            : base(title, accessLevel, deparment_key, username)
        {
            Thumbnail = thumbnail;
            this._elements = elements;
        }

        public Frame Get(int index) => _elements.ElementAt(index);

        public void Add(Frame frame) => _elements.Add(frame);
        public void AddAll(ICollection<Frame> frames) {
            foreach (var frame in frames) {
                _elements.Add(frame);
            }
        }

        public void Remove(Frame frame) => _elements.Remove(frame);
        public void Clear() => _elements.Clear();

        public System.Collections.Generic.IEnumerator<Frame> GetEnumerator() => _elements.GetEnumerator();

        public void Merge(Sequence sequence) {
            base.Merge(sequence);

            this.Thumbnail = sequence.Thumbnail;
            foreach (var frame in sequence) {
                _elements.Add(frame);
            }
        }
    }
}