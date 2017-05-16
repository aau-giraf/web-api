using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Services
{
    
    /// <summary>
    /// A service used by the EmailService. Contains all the necessary information for the EmailService.
    /// </summary>
    public class EmailConfig
    {
        /// <summary>
        /// The name of the sender of the email
        /// </summary>
        public String FromName { get; set; }

        /// <summary>
        /// The address the email should be sent from
        /// </summary>
        public String FromAddress { get; set; }

        public String LocalDomain { get; set; }

        public String MailServerAddress { get; set; }
        public String MailServerPort { get; set; }

        /// <summary>
        /// The Id of the User requesting his password
        /// </summary>
        public String UserId { get; set; }

        /// <summary>
        /// The password of the User in question
        /// </summary>
        public String UserPassword { get; set; }
    }
}
