using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;

namespace vodTimeControl.Models
{
    public class CReportPeriod
    {
        public int reportPeriodId { get; set; }
        public System.DateTime fromDate { get; set; }
        public System.DateTime toDate { get; set; }
        public int regUserId { get; set; }
        public System.DateTime regDate { get; set; }

        public string userName { get; set; }
        public string reportPeriodDescr { get; set; }


        BaseFont f_cb = BaseFont.CreateFont("c:\\windows\\fonts\\calibrib.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        BaseFont f_cn = BaseFont.CreateFont("c:\\windows\\fonts\\calibri.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);


        public DateTime getFirstReportDate()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

            DateTime dtFirstDate = db.timeTrackRow.Where(x => x.reportPeriodId == null).Min(x => x.timeTrackHead.tDate);
            return dtFirstDate;
        }

        public DateTime getLastReportDate()
        {
            DateTime dtToday = System.DateTime.Today;
            DateTime dtEndDate = new DateTime(dtToday.Year, dtToday.Month, 1).AddDays(-1);
            return dtEndDate;
        }

        public int createReportPeriod(DateTime dtFrom, DateTime dtTo, int userId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            int cnt = db.timeTrackRow.Where(x => x.timeTrackHead.tDate >= dtFrom && x.timeTrackHead.tDate <= dtTo && x.reportPeriodId == null).Count();
            if (cnt == 0)
                return 0;
            DateTime minDate = db.timeTrackRow.Where(x => x.timeTrackHead.tDate >= dtFrom && x.timeTrackHead.tDate <= dtTo && x.reportPeriodId == null).Min(x => x.timeTrackHead.tDate);
            int reportPeriodId = createReportPeriodRow(minDate, dtTo, userId);

            string sSql = "update tr "
                        + " set reportPeriodId = @reportPeriodId "
                        + " from timeTrackRow tr "
                        + " join timeTrackHead th on tr.timeTrackHeadID = th.timeTrackHeadID "
                        + " where th.tDate between @dFrom and @dTo "
                        + " and tr.reportPeriodId is null ";
            SqlConnection conn = (SqlConnection)db.Database.Connection;
            SqlCommand cm = new SqlCommand(sSql, conn);
            cm.Parameters.AddWithValue("@reportPeriodId", reportPeriodId);
            cm.Parameters.AddWithValue("@dFrom", dtFrom);
            cm.Parameters.AddWithValue("@dTo", dtTo);



            conn.Open();
            cm.ExecuteNonQuery();
            conn.Close();

            return cnt;
        }

        public int createReportPeriodRow(DateTime dFrom, DateTime dTo, int userId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            reportPeriod rp = new reportPeriod();
            rp.fromDate = dFrom;
            rp.toDate = dTo;
            rp.regDate = System.DateTime.Now;
            rp.regUserId = userId;
            db.reportPeriod.Add(rp);
            db.SaveChanges();
            return rp.reportPeriodId;
        }

        public List<CReportPeriod> getReportPeriods()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<CReportPeriod> periodList = db.reportPeriod.Select(x => new CReportPeriod
            {
                reportPeriodId = x.reportPeriodId,
                fromDate = x.fromDate,
                toDate = x.toDate,
                regDate = x.regDate,
                regUserId = x.regUserId,
                userName = x.userTbl.userName
            }).OrderByDescending(x => x.reportPeriodId).ToList();

            foreach (CReportPeriod cr in periodList)
            {
                cr.reportPeriodDescr = cr.fromDate.ToShortDateString() + " - " +
                    cr.toDate.ToShortDateString();
            }

            return periodList;
        }

        public List<CCustomer> getCustListForReportPeriod(int reportPeriodId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

            List<CCustomer> custList = db.timeTrackRow.Where(x => x.reportPeriodId == reportPeriodId)
                .Select(x => new CCustomer
                {
                    customerID = x.timeTrackHead.customerID,
                    custName = x.timeTrackHead.customer.custName
                }).Distinct().ToList();
            custList.OrderBy(x => x.custName);
            return custList;
        }

        public List<CProject> getProjListForReportPeriod(int reportPeriodId, int customerId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            List<CProject> projList = db.timeTrackRow.Where(x => x.reportPeriodId == reportPeriodId &&
                x.timeTrackHead.customerID == customerId).Select(x => new CProject
                {
                    projectID = x.subProject2.projectID,
                    projectName = x.subProject2.project.projectName
                }).Distinct().ToList();

            projList.OrderBy(x => x.projectName);
            return projList;
        }


        public List<CUser> getUserListForReportPeriod(int reportPeriodId, int customerId, int projectId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            string sSql = "select distinct u.userId, u.userName"
                        + " from userTbl u "
                        + " join timeTrackHead tth on u.userId = tth.userId "
                        + " join timeTrackRow ttr on tth.timeTrackHeadID = ttr.timeTrackHeadID "
                        + " join subProject2 sp2 on ttr.subProjectID = sp2.subProjectID "
                        + " where ttr.reportPeriodId = @reportPeriodId "
                        + " and tth.customerId = @customerId ";
            if (projectId != 0)
                sSql += " and sp2.projectId = @projectId ";
            sSql += " order by u.userName";

            SqlConnection cn = (SqlConnection)db.Database.Connection;
            SqlCommand cm = new SqlCommand(sSql, cn);
            cm.Parameters.AddWithValue("@reportPeriodId", reportPeriodId);
            cm.Parameters.AddWithValue("@customerId", customerId);
            if (projectId != 0)
                cm.Parameters.AddWithValue("@projectId", projectId);
            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<CUser> userList = new List<CUser>();
            foreach (DataRow dr in dt.Rows)
            {
                CUser u = new CUser();
                u.userId = Convert.ToInt32(dr["userId"]);
                u.userName = dr["userName"].ToString();
                userList.Add(u);
            }
            return userList;
        }

        public DataSet getReportPeriodHoursDT(int reportPeriodId, int customerId, int projectId, bool onDateLevel, bool showSubProject, int userId, ref string error)
        {
            string sSql = " select c.custName "
            + " , p.projectName ";
            if (showSubProject)
                sSql += " , sp2.subProjectName ";
            if (onDateLevel)
            {
                sSql += " , th.tDate "
                + " , tr.timeTrackRowId ";
            }
            sSql += " , sum(tr.hours) hours "
            + " from TimeTrackRow tr "
            + " join TimeTrackHead th on tr.timeTrackHeadID = th.timeTrackHeadID "
            + " join reportPeriod rp on rp.reportPeriodId = tr.reportPeriodId "
            + " join subProject2 sp2 on tr.subProjectID = sp2.subProjectID "
            + " join project p on sp2.projectID = p.projectID "
            + " join customer c on th.customerID = c.customerID "
            + " where rp.reportPeriodId = @reportPeriodId "
            + " and c.customerID = @customerID ";
            if (projectId != 0)
                sSql += " and sp2.projectID = @projectID ";
            if (userId != 0)
                sSql += " and th.userId = @userId ";
            sSql += " group by p.projectName ";
            if (showSubProject)
            {
                sSql += " , sp2.subProjectName ";

            }
            if (onDateLevel)
            {
                sSql += " , th.tDate "
                    + " , tr.timeTrackRowId ";
            }
            sSql += " , c.custName ";
            if (onDateLevel)
                sSql += " order by th.tDate, p.projectName";
            else
                sSql += " order by p.projectName";


            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            SqlConnection cn = (SqlConnection)db.Database.Connection;
            SqlCommand cm = new SqlCommand(sSql, cn);
            cm.Parameters.AddWithValue("@reportPeriodId", reportPeriodId);
            cm.Parameters.AddWithValue("@customerID", customerId);
            if (projectId != 0)
                cm.Parameters.AddWithValue("@projectID", projectId);
            if (userId != 0)
                cm.Parameters.AddWithValue("@userId", userId);
            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable dt = new DataTable("timeTrackRows");
            DataTable dtHead = new DataTable("head");
            DataSet ds = new DataSet();
            List<CReportPeriodHours> periodList = new List<CReportPeriodHours>();


            string sSqlNote = " select note from timeTrackRow "
                        + " where timeTrackRowId = @timeTrackRowId ";
            SqlCommand cmNote = new SqlCommand(sSqlNote, cn);
            cmNote.Parameters.Add("@timeTrackRowId", SqlDbType.Int);

            Decimal sumHours = 0;
            try
            {
                cn.Open();
                da.Fill(dt);
                DataColumn dcNote = new DataColumn("note", System.Type.GetType("System.String"));
                dt.Columns.Add(dcNote);
                foreach (DataRow dr2 in dt.Rows)
                {
                    dr2["note"] = "";
                    if (onDateLevel)
                        dr2["note"] = getNote(cmNote, Convert.ToInt32(dr2["timeTrackRowId"]));
                    sumHours += Convert.ToDecimal(dr2["hours"]);
                }
                cn.Close();
                DataRow dr = dt.NewRow();
                dr["custName"] = "";
                if (showSubProject)
                    dr["subProjectName"] = "";
                if (onDateLevel)
                {
                    dr["tDate"] = System.DateTime.Today;
                    dr["note"] = "";
                }
                dr["projectName"] = "Summa timmar";
                dr["hours"] = sumHours;
                dt.Rows.Add(dr);
                int antal = dt.Rows.Count;


                reportPeriod rp = db.reportPeriod.Where(x => x.reportPeriodId == reportPeriodId).FirstOrDefault();
                string period = rp.fromDate.ToShortDateString() + " - " + rp.toDate.ToShortDateString();
                vodTimeControl.Models.customer c = db.customer.Where(x => x.customerID == customerId).FirstOrDefault();
                string customer = c.custName;
                dtHead.Columns.Add(new DataColumn("period"));
                dtHead.Columns.Add(new DataColumn("customer"));
                dtHead.Columns.Add(new DataColumn("onDateLevel", System.Type.GetType("System.Boolean")));
                dtHead.Columns.Add(new DataColumn("showSubProject", System.Type.GetType("System.Boolean")));
                dtHead.Columns.Add(new DataColumn("userName"));
                DataRow dtHeadRow = dtHead.NewRow();
                dtHeadRow["period"] = period;
                dtHeadRow["customer"] = customer;
                dtHeadRow["onDateLevel"] = onDateLevel;
                dtHeadRow["showSubProject"] = showSubProject;
                dtHeadRow["userName"] = "";
                if (userId != 0)
                {
                    userTbl ut = db.userTbl.FirstOrDefault(x => x.userId == userId);
                    dtHeadRow["userName"] = ut.userName;
                }
                dtHead.Rows.Add(dtHeadRow);

            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            ds.Tables.Add(dt);
            ds.Tables.Add(dtHead);
            return ds;


        }


        public List<CReportPeriodHours> getReportPeriodHours(int reportPeriodId, int customerId, int projectId, bool onDateLevel, bool showSubProject, int userId, ref string error)
        {
            DataTable dt = getReportPeriodHoursDT(reportPeriodId, customerId, projectId, onDateLevel, showSubProject, userId, ref error).Tables["timeTrackRows"];
            List<CReportPeriodHours> periodList = new List<CReportPeriodHours>();

            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    CReportPeriodHours ph = new CReportPeriodHours();
                    ph.customerName = dr["custName"].ToString();
                    ph.hours = Convert.ToDecimal(dr["hours"]);
                    ph.projectName = dr["projectName"].ToString();
                    ph.subProjectName = "";
                    if (showSubProject)
                        ph.subProjectName = dr["subProjectName"].ToString();
                    ph.tDate = "";
                    if (onDateLevel && ph.customerName != "")
                        ph.tDate = Convert.ToDateTime(dr["tDate"]).ToShortDateString();
                    ph.note = "";
                    if (showSubProject)
                        ph.note = dr["note"].ToString();
                    periodList.Add(ph);

                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return periodList;
        }


        private string getNote(SqlCommand cm, int timeTrackRowId)
        {
            cm.Parameters[0].Value = timeTrackRowId;

            string note = cm.ExecuteScalar().ToString();

            if (note.Length > 25)
                note = note.Substring(0, 24) + "...";
            return note;

        }




        // PDF Creation

        public int createHeader(PdfContentByte cb, DataRow drHead, int datePeriod, int showActivity)
        {
            writeText(cb, "Tidrapport", 50, 810, f_cb, 14);
            writeText(cb, "Avser perioden : ", 50, 790, f_cb, 10);
            writeText(cb, drHead["period"].ToString(), 120, 790, f_cn, 10);
            writeText(cb, "Kund : ", 50, 775, f_cb, 10);
            writeText(cb, drHead["customer"].ToString(), 120, 775, f_cn, 10);
            writeText(cb, "Utskriftsdatum", 350, 810, f_cb, 10);
            writeText(cb, System.DateTime.Today.ToShortDateString(), 420, 810, f_cn, 10);
            if (drHead["userName"].ToString() != "")
            {
                writeText(cb, "Arb utfört av", 350, 790, f_cb, 10);
                writeText(cb, drHead["userName"].ToString(), 420, 790, f_cn, 10);
            }
            writeText(cb, "Projekt", 50, 744, f_cb, 10);
            if (showActivity == 1)
                writeText(cb, "Aktivitet", 180, 744, f_cb, 10);
            if (datePeriod == 1)
                writeText(cb, "Datum", 300, 744, f_cb, 10);
            writeTextRight(cb, "Timmar", 400, 744, f_cb, 10);
            if (datePeriod == 1)
                writeText(cb, "Notering", 430, 744, f_cb, 10);
            return 742;
        }

        private void writeText(PdfContentByte cb, string Text, int X, int Y, BaseFont font, int Size)
        {
            cb.SetFontAndSize(font, Size);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Text, X, Y, 0);
        }

        private void writeTextRight(PdfContentByte cb, string Text, int X, int Y, BaseFont font, int Size)
        {
            cb.SetFontAndSize(font, Size);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, Text, X, Y, 0);
        }


        public string generatePDFReport(string path, int periodId, int customerId, int projectId, int datePeriod, int showActivity, int userId, ref string errStr)
        {

            errStr = "";
            DataSet ds = getReportPeriodHoursDT(periodId, customerId, projectId, datePeriod == 1, showActivity == 1, userId, ref errStr);
            if (errStr != "")
                return "-1";
            try
            {
                DataRow drHead = ds.Tables["head"].Rows[0];
                DataTable dtRows = ds.Tables["timeTrackRows"];
                using (System.IO.FileStream fs = new FileStream(path, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 25, 25, 30, 1);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    // Add meta information to the document
                    document.AddAuthor("vodTimeControl");
                    document.AddCreator("vodTimeControl");
                    document.AddKeywords("PDF timecontrol");
                    document.AddSubject("TimeReport generation");
                    document.AddTitle("TimeReport for period " + periodId.ToString());
                    document.Open();

                    // Makes it possible to add text to a specific place in the document using 
                    // a X & Y placement syntax.
                    PdfContentByte cb = writer.DirectContent;
                    //cb.AddTemplate(PdfFooter(cb, drPayee), 30, 1);
                    cb.BeginText();
                    int y = createHeader(cb, drHead, datePeriod, showActivity);
                    foreach (DataRow dr in dtRows.Rows)
                    {
                        if (y < 60)
                        {
                            cb.EndText();
                            document.NewPage();
                            cb.BeginText();
                            y = createHeader(cb, drHead, datePeriod, showActivity);
                        }
                        if (dr["projectName"].ToString() != "Summa timmar")
                        {
                            y -= 12;
                            writeText(cb, dr["projectName"].ToString(), 50, y, f_cn, 10);
                            if (showActivity == 1)
                                writeText(cb, dr["subProjectName"].ToString(), 180, y, f_cn, 10);
                            if (datePeriod == 1)
                                writeText(cb, Convert.ToDateTime(dr["tDate"]).ToShortDateString(), 300, y, f_cn, 10);
                            writeTextRight(cb, Convert.ToDecimal(dr["hours"]).ToString(), 400, y, f_cn, 10);
                            if (datePeriod == 1)
                                writeText(cb, dr["note"].ToString(), 430, y, f_cn, 10);
                        }
                    }
                    DataRow[] drs = dtRows.Select("projectName = 'Summa timmar'");
                    if (drs != null && drs.Length == 1)
                    {
                        y -= 20;
                        writeText(cb, "Summa timmar : ", 300, y, f_cb, 10);
                        writeTextRight(cb, Convert.ToDecimal(drs[0]["hours"]).ToString(), 400, y, f_cb, 10);
                    }
                    cb.EndText();
                    document.Close();
                    writer.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                errStr = ex.Message;
                return "-1";
            }
        
            return "../pdf/" + periodId.ToString() + ".pdf";

        }






}
}