using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace vodTimeControl.Models
{
    public class CUserProject
    {

        public int userId { get; set; }
        public int projectId { get; set; }
        public string projectName { get; set; }
        public string custName { get; set; }
        public bool active { get; set; }
        


        public List<CUserProject> getUserProject(DataTablesParam param, int userId, 
            int sortOrder, string sortDir, ref int countToDisplay, ref int totalCount)
        {

            int pageNo = 1;
            string crit = "";
            if (param.sSearch != null)
                crit = param.sSearch.ToUpper();
            if (param.iDisplayStart >= param.iDisplayLength)
            {
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
            }


            int iSortCol = param.iSortCol_0 + 1;
            string sSortCol = "";

            switch (iSortCol)
            {
                case 1: sSortCol = "custName";
                    break;
                case 2: sSortCol = "projectName";
                    break;
                case 3: sSortCol = "projectName";
                    break;
            }




            string sSql = "select c.custName, p.projectName,  0 active, p.projectID"
                    + " from project p "
                    + " join customer c on p.customerID = c.customerID"
                    + " where not exists(select 'x' "
                    + "     from userProject up "
                    + "     where up.projectId = p.projectID "
                    + "     and up.userId = @userId) " 
                    + " and p.active = 1 " 
                    + " and c.active = 1  "
                    + " union "
                    + " select c.custName, p.projectName, 1, p.projectID"
                    + " from project p "
                    + " join customer c on p.customerID = c.customerID"
                    + " where exists( select 'x' "
                    + "     from userProject up "
                    + "     where up.projectId = p.projectID "
                    + "     and up.userId = @userId )" 
                    + " and p.active = 1 "
                    + " and c.active = 1  "
                    + " order by " + iSortCol.ToString() + " " + param.sSortDir_0 + " ";

            pdsTidRedLiveEntities db = new pdsTidRedLiveEntities();
            SqlConnection cn = (SqlConnection)db.Database.Connection;
            SqlCommand cm = new SqlCommand(sSql, cn);
            SqlParameter pUserId = cm.Parameters.Add("@userId", SqlDbType.Int);
            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable dt = new DataTable();
            pUserId.Value = userId;
            da.Fill(dt);
            totalCount = dt.Rows.Count;

            DataTable dtResult = new DataTable();
            if (crit != "")
            {
                crit = "%" + crit + "%";
                DataRow[] drs = dt.Select("projectName like '" + crit + "' or custName like '" + crit + "'", sSortCol + " " + param.sSortDir_0);
                if (drs != null && drs.Length > 0)
                    dtResult = drs.CopyToDataTable();
                else
                    dtResult.Rows.Clear();
            }
            else
                dtResult = dt.Copy();


            int start = ((pageNo - 1) * param.iDisplayLength) + 1;
            if (start > dtResult.Rows.Count)
                start = dtResult.Rows.Count;

            int end = param.iDisplayLength + start - 1;
            if (end > dtResult.Rows.Count)
                end = dtResult.Rows.Count;

            List<CUserProject> cuList = new List<CUserProject>();
            if (dtResult.Rows.Count > 0)
            {
                for (int i = start; i <= end; i++)
                {
                    DataRow dr = dtResult.Rows[i - 1];
                    CUserProject cu = new CUserProject();
                    cu.projectId = Convert.ToInt32(dr["projectID"]);
                    cu.projectName = dr["projectName"].ToString();
                    cu.custName = dr["custName"].ToString();
                    cu.active = false;
                    if (Convert.ToInt16(dr["active"]) == 1)
                        cu.active = true;
                    cuList.Add(cu);
                }
            }

            countToDisplay = dtResult.Rows.Count;



            return cuList;

        }
    }
}