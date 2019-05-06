using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vodTimeControl.Models;

namespace vodTimeControl.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["userId"] == null)
                return View("Login");
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoginUser(CUser model)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            
            userTbl user = db.userTbl.SingleOrDefault(x => x.userName == model.userName && x.password == model.password);
            string result = "Felaktigt användarnamn eller lösenord";
            if (user != null)
            {

                Session["userId"] = user.userId;
                Session["userName"] = user.userName;
                if (user.userRole.roleLevel == 10)
                    result = "Admin";
                else
                    result = "User";
                Session["roleLevel"] = user.userRole.roleLevel.ToString();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Registration()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<userRole> userRoleList = db.userRole.ToList();
            ViewBag.UserRole = new SelectList(userRoleList, "userRoleId", "userRoleName");
            return View();
        }

        [HttpPost]

        public JsonResult RegisterUser(CUser model)
        {
            CUser cUser = new CUser();
            if (!cUser.userNameAvailable(model.userName))
                return Json("Användarnamn upptaget. Välj ett annat namn.", JsonRequestBehavior.AllowGet);
            if (!cUser.emailAvailable(model.email))
                return Json("Denna emailadress finns redan i systemet.", JsonRequestBehavior.AllowGet);

            string resultText = "Success";
            try
            {
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                userTbl u = new userTbl();
                u.userName = model.userName;
                u.email = model.email;
                u.userRoleId = model.userRoleId;
                u.password = model.password;

                u.regDate = System.DateTime.Now;

                db.userTbl.Add(u);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                resultText = "Error while inserting user. Error message : " + ex.Message;
            }
            return Json(resultText, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Logout()
        {

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");

        }

        public ActionResult registerCustomer()
        {
            /*           pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                       List<CCustomer> custList = db.customer.Select( x => new CCustomer {
                           customerID = x.customerID,
                           custName = x.custName,
                           custTypeID = x.custTypeID,
                           address1 = x.address1,
                           address2 = x.address2,
                           zipCode = x.zipCode,
                           city = x.city,
                           email = x.email,
                           custTypeName = x.custType.custType1,
                           toBeInvoiced = x.custType.toBeInvoiced

                       }).ToList(); */
            return View();
        }

        public JsonResult getAllCustomers(DataTablesParam param)
        {
            int pageNo = 1;
            string crit = "";
            if (param.sSearch != null)
                crit = param.sSearch.ToUpper();
            if (param.iDisplayStart >= param.iDisplayLength)
            {
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            }

            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            int totalCount = db.customer.Count();
            List<CCustomer> custList = db.customer.Select(x => new CCustomer
            {
                customerID = x.customerID,
                custName = x.custName,
                custTypeID = x.custTypeID,
                address1 = x.address1,
                address2 = x.address2,
                zipCode = x.zipCode,
                city = x.city,
                email = x.email,                
                custTypeName = x.custType.custType1,
                toBeInvoiced = x.custType.toBeInvoiced,
                active = x.active
            }).ToList();

            if (crit != null && crit != "")
                custList = custList.Where(x => x.custName.ToUpper().Contains(crit)).ToList();



            if (param.iSortCol_0 == 0 && param.sSortDir_0 == "asc")
                custList = custList.OrderBy(o => o.custName).ToList();
            if (param.iSortCol_0 == 0 && param.sSortDir_0 == "desc")
                custList = custList.OrderByDescending(o => o.custName).ToList();
            if (param.iSortCol_0 == 1 && param.sSortDir_0 == "asc")
                custList = custList.OrderBy(o => o.custTypeName).ToList();
            if (param.iSortCol_0 == 1 && param.sSortDir_0 == "desc")
                custList = custList.OrderByDescending(o => o.custTypeName).ToList();
            if (param.iSortCol_0 == 2 && param.sSortDir_0 == "asc")
                custList = custList.OrderBy(o => o.address1).ToList();
            if (param.iSortCol_0 == 2 && param.sSortDir_0 == "desc")
                custList = custList.OrderByDescending(o => o.address1).ToList();
            if (param.iSortCol_0 == 3 && param.sSortDir_0 == "asc")
                custList = custList.OrderBy(o => o.city).ToList();
            if (param.iSortCol_0 == 3 && param.sSortDir_0 == "desc")
                custList = custList.OrderByDescending(o => o.city).ToList();
            if (param.iSortCol_0 == 4 && param.sSortDir_0 == "asc")
                custList = custList.OrderBy(o => o.email).ToList();
            if (param.iSortCol_0 == 4 && param.sSortDir_0 == "desc")
                custList = custList.OrderByDescending(o => o.email).ToList();

            int countToDisplay = custList.Count();
            custList = custList.Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();


            return Json(new
            {
                aaData = custList,
                sEcho = param.sEcho,
                iTotalDisplayRecords = countToDisplay,
                iTotalRecords = totalCount,
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddEditCust(int CustomerID)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<custType> list = db.custType.ToList();
            ViewBag.UserRoleList = new SelectList(list, "custTypeID", "custType1");

            CCustomer model = new CCustomer();

            if (CustomerID > 0)
            {
                customer c = db.customer.SingleOrDefault(x => x.customerID == CustomerID);
                model.customerID = c.customerID;
                model.custTypeID = c.custTypeID;
                model.custName = c.custName;
                model.address1 = c.address1;
                model.address2 = c.address2;
                model.zipCode = c.zipCode;
                model.city = c.city;
                model.email = c.email;
                model.active = c.active;
            }
            else
                model.active = true;
            return PartialView("AddEditCust", model);
        }

        public ActionResult AddEditProj(int ProjectID, int CustomerID)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

            CProject model = new CProject();
            if (ProjectID > 0)
            {
                project p = db.project.FirstOrDefault(x => x.projectID == ProjectID);
                model.projectID = p.projectID;
                model.projectName = p.projectName;
                model.customerID = CustomerID;
                model.active = p.active;
            }
            else
                model.active = true;

            return PartialView("AddEditProj", model);
        }

        public ActionResult AddEditSubProj(int subProjectID, int ProjectID)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

            CSubproject2 model = new CSubproject2();
            if (subProjectID > 0)
            {
                subProject2 p = db.subProject2.FirstOrDefault(x => x.subProjectID == subProjectID);
                model.subProjectID = p.subProjectID;
                model.subProjectName = p.subProjectName;
                model.projectID = p.projectID;
            }

            return PartialView("AddEditSubProj", model);
        }




        [HttpPost]
        public JsonResult updateCustomer(CCustomer model)
        {
            string message = "";
            try
            {
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                customer c = null;
                if (model.customerID == 0)
                {
                    c = new customer();
                }
                else
                {
                    c = db.customer.SingleOrDefault(x => x.customerID == model.customerID);
                }
                c.custName = model.custName;
                c.custTypeID = model.custTypeID;
                c.email = model.email;
                c.address1 = model.address1;
                c.address2 = model.address2;
                c.zipCode = model.zipCode;
                c.city = model.city;
                c.active = model.active;
                if (model.customerID == 0)
                    db.customer.Add(c);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult updateSubProject(CSubproject2 model)
        {
            string message = "";
            try
            {
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                subProject2 p = null;
                if (model.subProjectID == 0)
                {
                    p = new subProject2();
                    p.projectID = model.projectID;
                }
                else
                    p = db.subProject2.SingleOrDefault(x => x.subProjectID == model.subProjectID);
                p.subProjectName = model.subProjectName;
                if (model.subProjectID == 0)
                    db.subProject2.Add(p);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult updateMyProjects(int projectId, bool selected)
        {
            string message = "";
            int userId = Convert.ToInt32(Session["userId"]);
            try
            {
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                if (selected)
                {
                    if (db.userProject.Where(x => x.userId == userId && x.projectId == projectId).Count() == 0)
                    {
                        userProject up = new userProject();
                        up.userId = userId;
                        up.projectId = projectId;
                        db.userProject.Add(up);
                        db.SaveChanges();
                    }

                }
                else
                {
                    
                    userProject up = db.userProject.FirstOrDefault(x => x.userId == userId && x.projectId == projectId);
                    if (up != null)
                    {
                        db.userProject.Remove(up);
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }            
            return Json(message, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult updateProject(CProject model)
        {
            string message = "";
            bool bNew = false;
            int projID = 0;
            try
            {
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                project p = null;
                if (model.projectID == 0)
                {
                    p = new project();
                    p.customerID = model.customerID;
                    bNew = true;
                }
                else
                    p = db.project.SingleOrDefault(x => x.projectID == model.projectID);
                p.projectName = model.projectName;
                p.active = model.active;
                if (model.projectID == 0)
                    db.project.Add(p);
                db.SaveChanges();
                projID = p.projectID;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (message == "" && bNew)
            {
                CSubproject2 cs2 = new CSubproject2();
                message = cs2.addStdActivity(projID);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        public ActionResult registerProject()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<customer> customerList = db.customer.Where(x => x.active == true).OrderBy(x => x.custName).ToList();
            ViewBag.Customer = new SelectList(customerList, "customerID", "custName");
            return View();
        }

        public JsonResult getProjects(DataTablesParam param, int customerID)
        {
            int pageNo = 1;
            string crit = "";
            if (param.sSearch != null)
                crit = param.sSearch.ToUpper();
            if (param.iDisplayStart >= param.iDisplayLength)
            {
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            }

            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            int totalCount = db.project.Where(x => x.customerID == customerID).Count();
            //int totalCount = db.project.Count();
            List<CProject> projList = db.project.Select(x => new CProject
            {
                projectID = x.projectID,
                customerID = x.customerID,
                projectName = x.projectName,
                active = x.active
            }).Where(x => x.customerID == customerID).ToList();


            if (crit != null && crit != "")
                projList = projList.Where(x => x.projectName.ToUpper().Contains(crit)).ToList();

            if (param.iSortCol_0 == 0 && param.sSortDir_0 == "asc")
                projList = projList.OrderBy(o => o.projectName).ToList();
            if (param.iSortCol_0 == 0 && param.sSortDir_0 == "desc")
                projList = projList.OrderByDescending(o => o.projectName).ToList();
            if (param.iSortCol_0 == 1 && param.sSortDir_0 == "asc")
                projList = projList.OrderBy(o => o.active).ToList();
            if (param.iSortCol_0 == 1 && param.sSortDir_0 == "desc")
                projList = projList.OrderByDescending(o => o.active).ToList();

            int countToDisplay = projList.Count();

            projList = projList.Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();

            return Json(new
            {
                aaData = projList,
                sEcho = param.sEcho,
                iTotalDisplayRecords = countToDisplay,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult getUserProject(DataTablesParam param)
        {


            CUserProject cu = new CUserProject();
            int countToDisplay = 0;
            int totalCount = 0;


            List<CUserProject> list = cu.getUserProject(param, Convert.ToInt32(Session["userId"]), 1, "asc", ref countToDisplay, ref totalCount);


            return Json(new
            {
                aaData = list,
                sEcho = param.sEcho,
                iTotalDisplayRecords = countToDisplay,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult getSubProjects(DataTablesParam param, int ProjectId)
        {

            int pageNo = 1;
            string crit = "";
            if (param.sSearch != null)
                crit = param.sSearch.ToUpper();
            if (param.iDisplayStart >= param.iDisplayLength)
            {
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            }

            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            int totalCount = db.subProject2.Where(x => x.projectID == ProjectId).Count();
            List<CSubproject2> projList = db.subProject2.Select(x => new CSubproject2
            {
                subProjectID = x.subProjectID,
                projectID = x.projectID,
                subProjectName = x.subProjectName,
            }).Where(x => x.projectID == ProjectId).ToList();


            if (crit != null && crit != "")
                projList = projList.Where(x => x.subProjectName.ToUpper().Contains(crit)).ToList();

            if (param.iSortCol_0 == 0 && param.sSortDir_0 == "asc")
                projList = projList.OrderBy(o => o.subProjectName).ToList();
            if (param.iSortCol_0 == 0 && param.sSortDir_0 == "desc")
                projList = projList.OrderByDescending(o => o.subProjectName).ToList();

            int countToDisplay = projList.Count();

            projList = projList.Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();


            return Json(new
            {
                aaData = projList,
                sEcho = param.sEcho,
                iTotalDisplayRecords = countToDisplay,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult registerActivity()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<customer> customerList = db.customer.OrderBy(x => x.custName).ToList();
            ViewBag.Customer = new SelectList(customerList, "customerID", "custName");
            return View();
        }

        public ActionResult timeReg()
        {
            return View();
        }

        public ActionResult myProjects()
        {
            return View();
        }


        public ActionResult GetProjectList(int CustomerId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<project> projList = db.project.Where(x => x.customerID == CustomerId).ToList();
            ViewBag.ProjList = new SelectList(projList, "projectID", "projectName");

            return PartialView("GetProjectList");

        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
