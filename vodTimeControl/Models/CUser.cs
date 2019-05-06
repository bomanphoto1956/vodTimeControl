using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vodTimeControl.Models
{
    public class CUser
    {
        public int userId { get; set; }
        [Required(ErrorMessage = "Ange användarnamn")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Ange email")]
        public string email { get; set; }
        public System.DateTime regDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        [Required(ErrorMessage = "Välj användarroll")]
        public Nullable<int> userRoleId { get; set; }
        [Required(ErrorMessage = "Ange lösenord")]
        public string password { get; set; }

        public string userRoleName { get; set; }


        public bool userNameAvailable(string userName)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

            int countUsers = db.userTbl.Where(u => u.userName == userName).Count();
            return countUsers == 0;
        }

        public bool emailAvailable( string email)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            int countEmail = db.userTbl.Where(u => u.email == email).Count();
            return countEmail == 0;
        }

    }
}