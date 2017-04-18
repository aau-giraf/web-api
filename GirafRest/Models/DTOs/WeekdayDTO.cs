﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class WeekdayDTO
    {
        public long ThumbnailID { get; set; }
        public bool ElementsSet { get; set; }
        public List<long> ElementIDs { get; set; }
        public Days Day { get; set; }
        public ICollection<FrameDTO> Elements { get; set; }
        public WeekdayDTO(Weekday weekday) {
            try
            {
                this.ThumbnailID = weekday.ThumbnailKey;
            }
            catch (NullReferenceException)
            {
                this.ThumbnailID = 0;
            }
            this.Day = weekday.Day;
            Elements = new List<FrameDTO>();
            ElementIDs = new List<long>();
            if(weekday.Elements != null){
                foreach (var element in weekday.Elements)
                {
                    Elements.Add(new FrameDTO(element.Resource));
                    ElementIDs.Add(element.Resource.Id);
                }
            }
            if(Elements.Count > 0)
                ElementsSet = true;
        }

        public WeekdayDTO() {}
    }
}