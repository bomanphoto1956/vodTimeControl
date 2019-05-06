using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vodTimeControl.Models
{
    public class CSubproject2
    {


        public int subProjectID { get; set; }
        public int projectID { get; set; }
        [Required(ErrorMessage = "Skriv in aktivitet")]
        public string subProjectName { get; set; }


        public string addStdActivity(int projectID)
        {
            string errorStr = "";
            string subProjName = "";
            try
            {
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

                for (int i = 0; i < 5; i++)
                {
                    switch (i)
                    {
                        case 0:
                            subProjName = "Programmering";
                            break;
                        case 1:
                            subProjName = "Test";
                            break;
                        case 2:
                            subProjName = "Buggfix";
                            break;
                        case 3:
                            subProjName = "Möte";
                            break;
                        case 4:
                            subProjName = "Leverans";
                        break;
                    }
                    subProject2 p = new subProject2();
                    p.projectID = projectID;
                    p.subProjectName = subProjName;
                    db.subProject2.Add(p);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                errorStr = "Error while inserting standard activities. Error message : " + ex.Message;
            }

            return errorStr;
        }

        public SelectList getSubProjects( int projectID)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<subProject2> projList = db.subProject2.Where(x => x.projectID == projectID).ToList();
            SelectList sl = new SelectList(projList, "subProjectID", "subProjectName");
            return sl;
        }


    }
}