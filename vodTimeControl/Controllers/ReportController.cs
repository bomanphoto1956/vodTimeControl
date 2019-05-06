using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vodTimeControl.Models;

namespace vodTimeControl.Controllers
{
    public class ReportController : Controller
    {


        // GET: Report
        public ActionResult Index()
        {
            CReportPeriod crp = new CReportPeriod();
            CDateParameters cdp = new CDateParameters();
            cdp.dFrom = crp.getFirstReportDate();
            cdp.dTo = crp.getLastReportDate();
            if (cdp.dFrom > cdp.dTo)
                cdp.dFrom = cdp.dTo;
            ViewBag.dFromStr = cdp.dFrom.ToShortDateString();
            return View(cdp);
        }

        [HttpPost]
        public JsonResult createReportPeriod(DateTime dFrom, DateTime dTo)
        {
            string errMess = "";
            int cnt = 0;
            try
            {
                CReportPeriod crp = new CReportPeriod();
                int userId = Convert.ToInt32(Session["userId"]);
                cnt = crp.createReportPeriod(dFrom, dTo, userId);
            }
            catch (Exception ex)
            {
                errMess = ex.Message;
            }

            return Json(new
            {
                errMess,
                cnt
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult reportProperties()
        {
            CReportPeriod crp = new CReportPeriod();
            List<CReportPeriod> periodList = crp.getReportPeriods();
            SelectList sl = new SelectList(periodList, "reportPeriodId", "reportPeriodDescr");
            ViewBag.periodList = sl;
            return View();
        }

        public ActionResult getCustList(int reportPeriodId)
        {
            CReportPeriod crp = new CReportPeriod();
            List<CCustomer> custList = crp.getCustListForReportPeriod(reportPeriodId);
            SelectList sl = new SelectList(custList, "customerID", "custName");
            ViewBag.custList = sl;
            return PartialView("getCustList");

        }

        public ActionResult getProjectList(int reportPeriodId, int customerId)
        {
            CReportPeriod crp = new CReportPeriod();
            List<CProject> projList = crp.getProjListForReportPeriod(reportPeriodId, customerId);
            SelectList sl = new SelectList(projList, "projectID", "projectName");
            ViewBag.projlist = sl;
            return PartialView("getProjectList");

        }
        

        public ActionResult getUserList(int reportPeriodId, int customerId, int projectId)
        {
            CReportPeriod crp = new CReportPeriod();
            List<CUser> userList = crp.getUserListForReportPeriod(reportPeriodId, customerId, projectId);
            SelectList sl = new SelectList(userList, "userId", "userName");
            ViewBag.userlist = sl;
            return PartialView("getUserList");
        }

        public ActionResult getReportPeriodHours(int periodId, int customerId, int projectId, int datePeriod, int showActivity, int userId)
        {
            CReportPeriod crp = new CReportPeriod();
            string error = "";
            List<CReportPeriodHours> repHours = crp.getReportPeriodHours(periodId, customerId, projectId, datePeriod == 1, showActivity == 1, userId, ref error);
            CReportPeriodHours rh = repHours.FirstOrDefault(x => x.projectName == "Summa timmar");
            ViewBag.sumHours = rh.hours.ToString();
            repHours.Remove(rh);
            ViewBag.datePeriod = datePeriod;
            ViewBag.showActivity = showActivity;
            return PartialView("getReportPeriodHours", repHours);
        }


        public JsonResult getReportPeridHoursPDF(int periodId, int customerId, int projectId, int datePeriod, int showActivity, int userId)
        {
            string errStr = "";
            CReportPeriod crp = new CReportPeriod();
            string path = Server.MapPath("~/pdf");
            path += "\\" + periodId.ToString() + ".pdf";
            string relativePath = crp.generatePDFReport(path, periodId, customerId, projectId, datePeriod, showActivity, userId, ref errStr);
            //return File(path, "application/pdf");            
            return Json(new
            {
                relativePath,
                errStr
            }, JsonRequestBehavior.AllowGet);
                        
        }

        public ActionResult reportInterval()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            ViewBag.dFrom = DateTime.Today;
            ViewBag.dTo = DateTime.Today.AddDays(-30);
            List<customer> custList = db.customer.Where( x=> x.active == true).OrderBy(x => x.custName).ToList();
            ViewBag.custList = new SelectList(custList, "customerID", "custName");

            List<userTbl> userList = db.userTbl.OrderBy(x => x.userName).ToList();
            ViewBag.userList = new SelectList(userList, "userId", "userName");

            CReportPeriod crp = new CReportPeriod();
            List<CReportPeriod> periodList = crp.getReportPeriods();
            SelectList sl = new SelectList(periodList, "reportPeriodId", "reportPeriodDescr");
            ViewBag.periodList = sl;
            return View();
        }




    }





}
