using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class GirafUserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public byte[] UserIcon { get; set; }

        public long DepartmentKey { get; set; }

        public ICollection<long> WeekScheduleIds { get; set; }
        public virtual ICollection<long> Resources { get; set; }

        public bool UseGrayscale { get; set; }
        public bool DisplayLauncherAnimations { get; set; }
        public ICollection<ApplicationOption> AvailableApplications { get; set; }
        public int ApplicationCount;

        public GirafUserDTO()
        {
            WeekScheduleIds = new List<long>();
            Resources = new List<long>();
            AvailableApplications = new List<ApplicationOption>();
        }

        public GirafUserDTO(GirafUser user) 
        {
            Id = user.Id;
            Username = user.UserName;
            DisplayName = user.DisplayName;
            UserIcon = user.UserIcon;
            DepartmentKey = user.DepartmentKey;
            WeekScheduleIds = user.WeekSchedule.Select(w => w.Id).ToList();
            Resources = new List<long>();
            foreach (var res in user.Resources)
            {
                Resources.Add(res.Key);
            }
            UseGrayscale = user.UseGrayscale;
            DisplayLauncherAnimations = user.DisplayLauncherAnimations;
            AvailableApplications = user.AvailableApplications;
            ApplicationCount = user.AvailableApplications.Count;
        }
    }
}
