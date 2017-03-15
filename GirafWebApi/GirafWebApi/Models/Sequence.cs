using System.Collections.Generic;
using System.Linq;
using GirafWebApi.Models.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi.Models {
    public class Sequence : PictoFrame { 
        private ICollection<Frame> _elements {get; set;}
        public long ThumbnailKey { get; set; }
        [ForeignKey("ThumbnailKey")]
        public Pictogram Thumbnail { get; set; }

        protected Sequence () {}
        public Sequence(string title, AccessLevel accessLevel, Pictogram thumbnail)
        : this(title, accessLevel, thumbnail, new List<Frame>())
        {
            Thumbnail = thumbnail;

        }
        public Sequence(string title, AccessLevel accessLevel, Pictogram thumbnail, ICollection<Frame> elements)
            : base(title, accessLevel)
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

        public void Merge(SequenceDTO sequence, ICollection<Frame> frames) {
            base.Merge(sequence);

            this.ThumbnailKey = sequence.ThumbnailID;
            foreach (var frame in frames) {
                _elements.Add(frame);
            }
        }
    }
}