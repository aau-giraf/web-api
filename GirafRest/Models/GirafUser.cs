using GirafRest.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
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
        public virtual byte[] UserIcon { get; set; }

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

        /// <summary>
        /// Settings object for user
        /// </summary>
        public virtual Setting Settings { get; set; }

        private void InitialiseData(GirafRoles role)
        {
            if (role == GirafRoles.Citizen)
            {
                this.Settings = new Setting();
                this.Settings.InitialiseWeekDayColors();
            }
            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            this.Citizens = new List<GuardianRelation>();
            this.Guardians = new List<GuardianRelation>();
        } 
        //
        private void InitialiseData()
        {
            this.Settings = new Setting();
            this.Settings.InitialiseWeekDayColors();
            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            this.Citizens = new List<GuardianRelation>();
            this.Guardians = new List<GuardianRelation>();
        }

        /// <summary>
        /// Iteratr citizens for calling AddCitizen
        /// </summary>
        /// <param name="citizens"></param>
        public void AddCitizens(List<GirafUser> citizens)
        {
            foreach (var citizen in citizens)
            {
                AddCitizen(citizen);
            }
        }

        /// <summary>
        /// Adding citizens 
        /// </summary>
        /// <param name="citizen">Citizen to add</param>
        public void AddCitizen(GirafUser citizen)
        {
            this.Citizens.Add(new GuardianRelation(this, citizen));
        }

        /// <summary>
        /// Action for adding and referencing Guardians
        /// </summary>
        /// <param name="guardians"></param>
        public void AddGuardians(List<GirafUser> guardians)
        {
            foreach (var guardian in guardians)
            {
                AddGuardian(guardian);
            }
        }

        /// <summary>
        /// Add specific Guardian to this
        /// </summary>
        /// <param name="guardian"></param>
        public void AddGuardian(GirafUser guardian)
        {
            this.Guardians.Add(new GuardianRelation(guardian, this));
        }

        /// <summary>
        /// Add specific Guardian to this
        /// </summary>
        /// <param name="trustee"></param>
        public void AddTrustee(GirafUser trustee)
        {
            this.Guardians.Add(new GuardianRelation(trustee, this));
        }

        /// <summary>
        /// Constructor for GirafUser
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="displayName">Display name</param>
        /// <param name="department">Department</param>
        /// <param name="role">Role for user</param>
        public GirafUser(string userName, string displayName, Department department, GirafRoles role) : base(userName)
        {
            InitialiseData(role);
            DisplayName = displayName;
            DepartmentKey = department?.Key ?? -1;
        }

        /// <summary>
        /// "Empty" constructor for JSON Generation
        /// </summary>
        public GirafUser()
        {
            InitialiseData();
        }
    }
}