using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace vodTimeControl.Models
{
    public class CTimeTrackClass
    {
        public SelectList getMyCustomers2(int userId, int customerId)
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();

            string sSql = "";
            if (userId == 0)
            {
                sSql = "select c.customerID, c.custName "
                     + " from customer c";
            }
            else
            {
                sSql = "select distinct c.customerID, c.custName "
                     + " from userProject up "
                     + " join project p on up.projectId = p.projectID "
                     + " join customer c on p.customerID = c.customerID "
                     + " where up.userId = @userId "
                     + " and c.active = 1 "
                     + " and p.active = 1 "
                     + " order by c.custName ";
            }
            SqlConnection cn = (SqlConnection)db.Database.Connection;
            SqlCommand cm = new SqlCommand(sSql, cn);
            if (userId != 0)
            {
                SqlParameter pUserId = new SqlParameter("@userId", userId);
                cm.Parameters.Add(pUserId);
            }
            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable dt = new DataTable();

            da.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();
            string s = "";
            foreach (DataRow row in dt.Rows)
            {
                SelectListItem sl = new SelectListItem();
                sl.Text = row["custName"].ToString();
                sl.Value = row["customerID"].ToString();
                sl.Selected = (Convert.ToInt32(row["customerID"]) == customerId);
                if (sl.Selected)
                    s = "selected";
                list.Add(sl);
            }

            return new SelectList(list, "Value", "Text");

        }

        private DataTable getMyCustProjectsDT(int userId, int customerId)
        {
            string sSql = " select distinct p.projectID, p.projectName "
                        + "from userProject up "
                        + " join project p on up.projectId = p.projectID "
                        + " join customer c on p.customerID = c.customerID "
                        + " where up.userId = @userId "
                        + " and p.customerID = @customerId "
                        + " and c.active = 1 "
                        + " and p.active = 1 "
                        + " order by p.projectName ";


            SqlCommand cm = new SqlCommand(sSql, getConn());
            SqlParameter pUserId = new SqlParameter("@userId", userId);
            SqlParameter pCustomerId = new SqlParameter("@customerId", customerId);
            cm.Parameters.Add(pUserId);
            cm.Parameters.Add(pCustomerId);
            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }

        public SelectList getMyCustProjects2(int userId, int customerId)
        {
            DataTable dt = getMyCustProjectsDT(userId, customerId);

            List<project> pList = new List<project>();
            foreach (DataRow dr in dt.Rows)
            {
                project p = new project();
                p.projectID = Convert.ToInt32(dr["projectID"]);
                p.projectName = dr["projectName"].ToString();
                pList.Add(p);
            }
            return new SelectList(pList, "projectID", "projectName");

        }

        public SelectList getMyCustProjects(int userId, int customerId)
        {
            DataTable dt = getMyCustProjectsDT(userId, customerId);

            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row["projectName"].ToString(),
                    Value = row["projectID"].ToString()
                });
            }
            return new SelectList(list, "Value", "Text");
        }


        public List<CTimeTrack> getMyTimeTracks(DataTablesParam param, int userId, DateTime dFrom, DateTime dTo, int customerId, int projectId,
            ref int totalRecords, ref int totalDisplayRecords)
        {

            int pageNo = 1;
            string crit = "";
            if (param.sSearch != null)
                crit = param.sSearch.ToUpper();
            if (param.iDisplayStart >= param.iDisplayLength)
            {
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            }

            string sSortCol = "";

            switch (param.iSortCol_0)
            {
                case 0:
                    sSortCol = "custName";
                    break;
                case 1:
                    sSortCol = "projectName";
                    break;
                case 2:
                    sSortCol = "subProjectName";
                    break;
                case 3:
                    sSortCol = "tDate";
                    break;
                case 4:
                    sSortCol = "hours";
                    break;
                case 5:
                    sSortCol = "shortNote";
                    break;
            }

            string sSql = "select th.timeTrackHeadID, th.customerID, th.tDate, "
                        + " th.userId, tr.timeTrackRowID, tr.subProjectID, "
                        + " tr.[hours], tr.regDate, tr.Note, p.projectName, "
                        + " substring(tr.note, 1, 25) + '...' shortNote, "
                        + " c.custName, "
                        + " sp2.subProjectName "
                        + " from TimeTrackHead th "
                        + " join TimeTrackRow tr on th.timeTrackHeadID = tr.timeTrackHeadID "
                        + " join subProject2 sp2 on tr.subProjectID = sp2.subProjectID "
                        + " join project p on sp2.projectID = p.projectID "
                        + " join customer c on p.customerId = c.customerId "
                        + " where th.userId = @userId "
                        + " and th.tDate >= @dFrom "
                        + " and th.tDate <= @dTo ";
            if (customerId != 0)
                sSql += " and th.customerID = @customerID ";
            if (projectId != 0)
                sSql += " and p.projectID = @projectID ";
            sSql += " order by " + sSortCol + " " + param.sSortDir_0;




            SqlCommand cm = new SqlCommand(sSql, getConn());
            cm.Parameters.AddWithValue("@userId", userId);
            cm.Parameters.AddWithValue("@dFrom", dFrom);
            cm.Parameters.AddWithValue("@dTo", dTo);
            if (customerId != 0)
                cm.Parameters.AddWithValue("@customerID", customerId);
            if (projectId != 0)
                cm.Parameters.AddWithValue("@projectID", projectId);

            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable dt = new DataTable();

            da.Fill(dt);

            totalRecords = dt.Rows.Count;

            DataTable dtResult = new DataTable();
            if (crit != "")
            {
                crit = "%" + crit + "%";
                DataRow[] drs = dt.Select("projectName like '" + crit + "' or custName like '" + crit + "'", sSortCol + " " + param.sSortDir_0);
                if (drs != null)
                    dtResult = drs.CopyToDataTable();
            }
            else
                dtResult = dt.Copy();

            int start = ((pageNo - 1) * param.iDisplayLength) + 1;
            if (start > dtResult.Rows.Count)
                start = dtResult.Rows.Count;
            if (start == 0)
                start++;

            int end = param.iDisplayLength + start - 1;
            if (end > dtResult.Rows.Count)
                end = dtResult.Rows.Count;


            List<CTimeTrack> timeTrackList = new List<CTimeTrack>();
            for (int i = start; i <= end; i++)
            {
                DataRow dr = dtResult.Rows[i - 1];
                CTimeTrack tt = new CTimeTrack();
                tt.timeTrackHeadID = Convert.ToInt32(dr["timeTrackHeadID"]);
                tt.customerID = Convert.ToInt32(dr["customerID"]);
                tt.tDate = Convert.ToDateTime(dr["tDate"]);
                tt.userId = Convert.ToInt32(dr["userId"]);
                tt.timeTrackRowID = Convert.ToInt32(dr["timeTrackRowID"]);
                tt.subProjectID = Convert.ToInt32(dr["subProjectID"]);
                tt.hours = Convert.ToDecimal(dr["hours"]);
                tt.regDate = Convert.ToDateTime(dr["regDate"]);
                tt.note = dr["Note"].ToString();
                tt.projectName = dr["projectName"].ToString();
                tt.shortNote = dr["shortNote"].ToString();
                tt.customerName = dr["custName"].ToString();
                tt.subProjectName = dr["subProjectName"].ToString();
                tt.tDateStr = Convert.ToDateTime(dr["regDate"]).ToShortDateString();
                timeTrackList.Add(tt);
            }
            totalDisplayRecords = dt.Rows.Count;
            return timeTrackList;
        }

        private SqlConnection getConn()
        {
            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            return (SqlConnection)db.Database.Connection;
        }




    }
}