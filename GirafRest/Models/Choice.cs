using GirafRest.Models.DTOs;
using GirafRest.Models.Many_to_Many_Relationships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace GirafRest.Models {
    /// <summary>
    /// A Choice may be present in the Citizen's Weekday's and indicates that the user may choose between a
    /// series of activities.
    /// </summary>
    public class Choice : Resource { 
        /// <summary>
        /// A list of options involved in the choice.
        /// </summary>
        public ICollection<ChoiceResource> Options { get; private set; }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Choice () {}

        /// <summary>
        /// Creates a new Choice with the given list of options.
        /// </summary>
        /// <param name="options">A list of options to add to the choice.</param>
        public Choice(List<PictoFrame> options)
        {
            LastEdit = DateTime.Now;
            Options = new List<ChoiceResource>();
            foreach (PictoFrame option in options)
            {
                Options.Add(new ChoiceResource(this, option));
            }
        }

        /// <summary>
        /// Adds a option to the choice.
        /// </summary>
        /// <param name="option">The pictoframe to be added as an option.</param>
        public void Add (PictoFrame option) => Options.Add(new ChoiceResource(this, option));
        /// <summary>
        /// Adds all the options from the given list.
        /// </summary>
        /// <param name="options">A list of options to add to the choice.</param>
        public void AddAll(ICollection<PictoFrame> options) {
            foreach (var option in options) {
                Options.Add(new ChoiceResource(this, option));
            }
        }

        /// <summary>
        /// Get the index'th option of the Choice.
        /// </summary>
        /// <param name="index">The index to fetch the pictoframe from.</param>
        /// <returns>The PictoFrame of the index'th slot.</returns>
        public PictoFrame Get(int index) => (PictoFrame) Options.ElementAt(index).Resource;

        /// <summary>
        /// Remove the given pictoframe from the list of options.
        /// </summary>
        /// <param name="pictoframe">The pictoframe to remove.</param>
        public void Remove(PictoFrame pictoframe) => Options.Remove(new ChoiceResource(this, pictoframe));

        /// <summary>
        /// Clears all options of the choice.
        /// </summary>
        public void Clear() => Options.Clear();

        /// <summary>
        /// Gets the enumerator of the Choice.
        /// </summary>
        /// <returns>The enumerator.</returns>
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