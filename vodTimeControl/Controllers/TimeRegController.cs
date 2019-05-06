using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vodTimeControl.Models;

namespace vodTimeControl.Controllers
{
    public class TimeRegController : Controller
    {
        // GET: TimeReg
        public ActionResult Index()
        {
            CDateParameters ct = new CDateParameters();
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            DateTime today = DateTime.Today.AddDays(-1);
            DateTime dtStartDate = today.AddDays(DayOfWeek.Monday - today.DayOfWeek);           
            ct.dFrom = dtStartDate;
            ct.dTo = dtStartDate.AddDays(6);

            int userId = Convert.ToInt32(Session["userId"]);
            CTimeTrack ctc = new CTimeTrack();
            ViewBag.myCustomers = ctc.getMyCustomers(userId);
            return View(ct);
        }


        public JsonResult getTimeReg(DataTablesParam param, DateTime dFrom, DateTime dTo, int customerId, int projectId)
        {
            //Init and calculate variables
            int pageNo = 1;
            string crit = "";
            if (param.sSearch != null)
                crit = param.sSearch.ToUpper();
            if (param.iDisplayStart >= param.iDisplayLength)
            {
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            }

            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            int userId = Convert.ToInt32(Session["userId"]);
            int totalRecords = 0;
            int totalDisplayRecords = 0;
            CTimeTrack ctt = new CTimeTrack();
            List<CTimeTrack> listTT = ctt.getMyTimeTracks(param, userId, dFrom, dTo, customerId, projectId, ref totalRecords, ref totalDisplayRecords);
            return Json(new
            {

                aaData = listTT,
                sEcho = param.sEcho,
                iTotalDisplayRecords = totalDisplayRecords,
                iTotalRecords = totalRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditTimeTrack(int timeTrackRowID, int customerID, int projectID)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            CTimeTrack model = new CTimeTrack();
            int userId = Convert.ToInt32(Session["userId"]);
            if (timeTrackRowID == 0)
            {
                model.timeTrackRowID = 0;
                model.userId = userId;
                model.customerID = customerID;
                model.projectId = projectID;
                model.tDate = System.DateTime.Today;                
                CTimeTrack ct = new CTimeTrack();                
                ViewBag.custList = ct.getMyCustomers(userId);
                ViewBag.projList = ct.getMyCustProjects(userId, customerID);
                CSubproject2 cs2 = new CSubproject2();                
                ViewBag.subProjList = cs2.getSubProjects(projectID);
                return PartialView("AddTimeTrack", model);
            }
            else
            {
                timeTrackRow ttr = db.timeTrackRow.FirstOrDefault(x => x.timeTrackRowID == timeTrackRowID);
                if (ttr != null)
                {
                    model.tDate = ttr.timeTrackHead.tDate;
                    model.subProjectID = ttr.subProjectID;
                    model.hours = ttr.hours;
                    model.note = ttr.note;
                    model.projectName = ttr.subProject2.project.projectName;
                    model.customerName = ttr.timeTrackHead.customer.custName;
                    model.tDateStr = ttr.timeTrackHead.tDate.ToShortDateString();
                    model.timeTrackHeadID = ttr.timeTrackHeadID;
                    model.customerID = ttr.timeTrackHead.customerID;
                    CSubproject2 cs2 = new CSubproject2();
                    ViewBag.subProjList = cs2.getSubProjects(ttr.subProject2.projectID);
                }
            }
            return PartialView("EditTimeTrack", model);

        }

        public ActionResult AddEditTimeTrack2(int timeTrackRowID, int customerID, int projectID)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            
            int userId = Convert.ToInt32(Session["userId"]);

            SelectList custList = null;
            SelectList projList = null;
            SelectList subProjList = null;
            CTimeTrack model = new CTimeTrack();
            CSubproject2 cs2 = new CSubproject2();
            if (timeTrackRowID == 0)
            {
                
                model.timeTrackRowID = timeTrackRowID;
                model.tDate = System.DateTime.Today;
                model.customerID = customerID;
                //model.projectId = projectID;
                CTimeTrack ct = new CTimeTrack();
                custList = ct.getMyCustomers(userId);
                //projList = ctc.getMyCustProjects(userId, customerID);
                //subProjList = cs2.getSubProjects(0);
            }
            else
            {
                
                timeTrackRow ttr = db.timeTrackRow.FirstOrDefault(x => x.timeTrackRowID == timeTrackRowID);
                if (ttr != null)
                {
                    model.timeTrackHeadID = ttr.timeTrackHeadID;
                    model.customerID = ttr.timeTrackHead.customerID;
                    model.tDate = ttr.timeTrackHead.tDate;
                    model.userId = ttr.timeTrackHead.userId;
                    model.timeTrackRowID = ttr.timeTrackRowID;
                    model.subProjectID = ttr.subProjectID;
                    model.hours = ttr.hours;
                    model.regDate = ttr.regDate;
                    model.note = ttr.note;
                    model.projectId = ttr.subProject2.projectID;
                    model.projectName = ttr.subProject2.project.projectName;
                    model.customerName = ttr.timeTrackHead.customer.custName;
                    model.timeTrackHeadID = ttr.timeTrackHeadID;
                }
                // First get the timeTrackHead
               /* custList = ctc.getMyCustomers(0, model.customerID);
                projList = ctc.getMyCustProjects(userId, ttr.timeTrackHead.customerID); */
                

            }
            ViewBag.custList = custList;
            ViewBag.projList = projList;
            subProjList = cs2.getSubProjects(model.projectId);
            ViewBag.subProjList = subProjList;
            return PartialView("AddEditTimeTrack", model);
        }

        public ActionResult GetProjectList(int CustomerId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            CTimeTrack ctc = new CTimeTrack();
            SelectList projList = ctc.getMyCustProjects(userId, CustomerId);
            ViewBag.ProjList = projList;
            return PartialView("GetProjectList");
        }

        public ActionResult GetSubProjectList(int projectId)
        {            
            CSubproject2 cs2 = new CSubproject2();
            SelectList subProjList = cs2.getSubProjects(projectId);
            ViewBag.subProjList = subProjList;
            return PartialView("GetSubProjectList");
        }

        [HttpPost]
        public JsonResult updateTimeTrack3(CTimeTrack model)
        {            
            string message = "";
            // 2018-04-03 KJBO            
            int userId = Convert.ToInt32(Session["userId"]);
            

            CTimeTrack ct = new CTimeTrack();
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            try
            {
                if (model.timeTrackRowID == 0)
                {                    
                    ct.addTimeTrack(model, userId);
/*                    timeTrackHead th = db.timeTrackHead.FirstOrDefault(x => x.customerID == model.customerID
                    && x.userId == userId && x.tDate == model.tDate);
                    if (th == null)
                    {
                        int ttHeadId = db.timeTrackHead.Max(x => x.timeTrackHeadID);
                        ttHeadId++;
                        th = new timeTrackHead();
                        th.timeTrackHeadID = ttHeadId;
                        th.customerID = model.customerID;
                        th.userId = userId;
                        th.tDate = model.tDate;
                        db.timeTrackHead.Add(th);
                        db.SaveChanges();
                    }
                    model.timeTrackHeadID = th.timeTrackHeadID;
                    int ttRowId = db.timeTrackRow.Max(x => x.timeTrackRowID);
                    ttRowId++;
                    timeTrackRow tr = new timeTrackRow();
                    tr.timeTrackHeadID = model.timeTrackHeadID;
                    tr.timeTrackRowID = ttRowId;
                    tr.hours = model.hours;
                    tr.note = model.note;
                    tr.regDate = System.DateTime.Now;
                    tr.subProjectID = model.subProjectID;
                    db.timeTrackRow.Add(tr);
                    db.SaveChanges(); */
                }
                else
                {
                    ct.editTimeTrack(model, userId);
                    /*
                    // First get the timeTrackHead to check if the date is the same as before
                    timeTrackHead th = db.timeTrackHead.FirstOrDefault(x => x.timeTrackHeadID == model.timeTrackHeadID);
                    if (th.tDate != model.tDate)
                    {
                        int customerId = th.customerID;
                        int ttHeadId = db.timeTrackHead.Max(x => x.timeTrackHeadID);
                        ttHeadId++;                                             
                        th = new timeTrackHead();
                        th.timeTrackHeadID = ttHeadId;
                        th.customerID = customerId;
                        th.userId = userId;
                        th.tDate = model.tDate;
                        db.timeTrackHead.Add(th);
                        db.SaveChanges();
                        model.timeTrackHeadID = th.timeTrackHeadID;
                        
                        // Here we have work to do. First check if there are other timeTracks that uses the same timeTrackRow
                        int cntTTh = db.timeTrackRow.Where(x => x.timeTrackHeadID == model.timeTrackHeadID).Count();
                        if (cntTTh == 1)
                        {
                            // In this case, only the current row points to this head and we can easily change the date without any problems
                            th.tDate = model.tDate;
                            db.SaveChanges();
                        }
                        else
                        {                            
                            // Check if there exists one timeTrackHead with the same values and in that case use that
                            th = db.timeTrackHead.FirstOrDefault(x => x.customerID == model.customerID
                                && x.userId == userId && x.tDate == model.tDate);
                            if (th != null)
                                model.timeTrackHeadID = th.timeTrackHeadID;
                            else
                            {
                                int ttHeadId = db.timeTrackHead.Max(x => x.timeTrackHeadID);
                                ttHeadId++;
                                th = new timeTrackHead();
                                th.timeTrackHeadID = ttHeadId;
                                th.customerID = model.customerID;
                                th.userId = userId;
                                th.tDate = model.tDate;
                                db.timeTrackHead.Add(th);
                                db.SaveChanges();
                                model.timeTrackHeadID = th.timeTrackHeadID;
                            }

                        }
                        
                    }
                    timeTrackRow tr = db.timeTrackRow.FirstOrDefault(x => x.timeTrackRowID == model.timeTrackRowID);
                    tr.timeTrackHeadID = model.timeTrackHeadID;
                    tr.subProjectID = model.subProjectID;
                    tr.hours = model.hours;
                    tr.note = model.note;
                    db.SaveChanges();
                    
                }
                
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
                */
                }

            }            
            catch (Exception ex)
            {
                message = ex.Message;
            }            
            
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult updateTimeTrack(CTimeTrack model)
        {
            string message = "";            
            if (Session["userId"] == null)
                message = "Ej inloggad";
            if (message == "")
            {
                int userId = Convert.ToInt32(Session["userId"]);
                CTimeTrack ct = new CTimeTrack();
                pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
                try
                {
                    if (model.timeTrackRowID == 0)
                    {
                        ct.addTimeTrack(model, userId);
                    }
                    else
                    {
                        ct.editTimeTrack(model, userId);
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        public JsonResult deleteTimeTrack(int timeTrackRowId)
        {
            string message = "";
            try
            {
                CTimeTrack ct = new CTimeTrack();
                ct.deleteTimeTrack(timeTrackRowId);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new
            {
                message     
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult sumHours( DateTime dFrom, DateTime dTo,int customerId, int projectId)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            Decimal sumBefore = 0;
            Decimal sumInPeriod = 0;
            Decimal sumAfter = 0;
            CTimeTrack ctt = new CTimeTrack();            
            ctt.sumHours(dFrom, dTo, userId, customerId, projectId, ref sumBefore, ref sumInPeriod, ref sumAfter);
            return Json(new
            {
                sumBefore,
                sumInPeriod,
                sumAfter
            }, JsonRequestBehavior.AllowGet);                
        }


        public JsonResult sumHours2(int projectId, DateTime dFrom, DateTime dTo)
        {
            Decimal sumBefore = 0;
            Decimal sumInPeriod = 0;
            Decimal sumAfter = 0;
           // CTimeTrack ctt = new CTimeTrack();
           // ctt.sumHours(dFrom, dTo, projectId, ref sumBefore, ref sumInPeriod, ref sumAfter);
            return Json(new
            {
                sumBefore,
                sumInPeriod,
                sumAfter
            }, JsonRequestBehavior.AllowGet);
        }




    }

}





