using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafEntities.Settings;
using GirafEntities.WeekPlanner;
using Microsoft.AspNetCore.Identity;

namespace GirafEntities.User
{
    /// <summary>
    /// GirafUser defines all relavant data for the user's of Giraf.
    /// </summary>
    [Table("User")]
    public class GirafUser : IdentityUser
    {
        /// <summary>
        /// Whether or not the current user is a DepartmentUser
        /// </summary>
        public bool IsDepartment { get; set; }

        /// <summary>
        /// List of guardians in a relationship with the user. Is empty if the user is a guardian.
        /// </summary>
        public virtual ICollection<GuardianRelation> Guardians { get; set; }

        /// <summary>
        /// List of citizens in a relationship with the user. Is empty if the user is a citizen.
        /// </summary>
        public virtual ICollection<GuardianRelation> Citizens { get; set; }

        /// <summary>
        /// Display name used for showing
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// Icon for user
        /// </summary>
        public virtual byte[]? UserIcon { get; set; }

        /// <summary>
        /// Belonging department key
        /// </summary>
        [ForeignKey("Department")]
        public long? DepartmentKey { get; set; }

        /// <summary>
        /// Belonging Department
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// Collection of week schedules
        /// </summary>
        public virtual ICollection<Week> WeekSchedule { get; set; }

        /// <summary>
        /// Collection of user ressources
        /// </summary>
        public virtual ICollection<UserResource> Resources { get; set; }

        [ForeignKey("Setting")]
        public long? SettingsKey { get; set; }

        /// <summary>
        /// Settings object for user
        /// </summary>
        public virtual Setting Settings { get; set; }

        /// <summary>
        /// Constructor for GirafUser
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="displayName">Display name</param>
        /// <param name="department">Department</param>
        /// <param name="role">Role for user</param>
        /// <param name="userIcon">Profile picture of user</param>
        public GirafUser(string userName, string displayName, Department department, GirafRoles role, byte[] userIcon = null) : base(userName)
        {
            if (role == GirafRoles.Citizen)
            {
                this.Settings = new Setting();
                Settings.WeekDayColors = new List<WeekDayColor>();
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Monday, HexColor = "#08a045", SettingId = Settings.Key});
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Tuesday, HexColor = "#540d6e", SettingId = Settings.Key});
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Wednesday, HexColor = "#f77f00", SettingId = Settings.Key});
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Thursday, HexColor = "#004777", SettingId = Settings.Key});
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Friday, HexColor = "#f9c80e", SettingId = Settings.Key});
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Saturday, HexColor = "#db2b39", SettingId = Settings.Key});
                Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Sunday, HexColor = "#ffffff", SettingId = Settings.Key});
            }

            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            this.Citizens = new List<GuardianRelation>();
            this.Guardians = new List<GuardianRelation>();

            DisplayName = displayName;
            DepartmentKey = department?.Key ?? -1;
            UserIcon = userIcon;
        }

        /// <summary>
        /// "Empty" constructor for JSON Generation
        /// </summary>
        public GirafUser()
        {
            this.Settings = new Setting();
            Settings.WeekDayColors = new List<WeekDayColor>();
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Monday, HexColor = "#08a045", SettingId = Settings.Key});
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Tuesday, HexColor = "#540d6e", SettingId = Settings.Key});
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Wednesday, HexColor = "#f77f00", SettingId = Settings.Key});
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Thursday, HexColor = "#004777", SettingId = Settings.Key});
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Friday, HexColor = "#f9c80e", SettingId = Settings.Key});
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Saturday, HexColor = "#db2b39", SettingId = Settings.Key});
            Settings.WeekDayColors.Add(new WeekDayColor(){Day = Days.Sunday, HexColor = "#ffffff", SettingId = Settings.Key});
            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            this.Citizens = new List<GuardianRelation>();
            this.Guardians = new List<GuardianRelation>();
        }
    }
}