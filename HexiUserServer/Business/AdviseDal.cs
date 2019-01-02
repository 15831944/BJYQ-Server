using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using HexiUserServer.Models;

namespace HexiUserServer.Business
{
    public class AdviseDal
    {
        public static StatusReport SetAdvise (string ztName, string name,string phone, string roomNumber,string type,string content, string submitDate)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = " insert into 基础资料_表扬建议管理 (分类,姓名,联系电话,地址,表扬建议,内容,提交时间) " +
                               " select @分类,@姓名,@联系电话,@地址,@表扬建议,@内容,@提交时间 " +
                               " SELECT @@IDENTITY ";
            sr = SQLHelper.Insert("wyt", sqlstring,
                new SqlParameter("@分类", ztName),
                new SqlParameter("@姓名", name),
                new SqlParameter("@联系电话", phone),
                new SqlParameter("@地址", roomNumber),
                new SqlParameter("@表扬建议", type),
                new SqlParameter("@内容", content),
                new SqlParameter("@提交时间", submitDate));
            return sr;
        }

        public static StatusReport GetAdvise (string ztName,string phone)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = " select ID,分类,姓名,联系电话,地址,表扬建议,内容,提交时间,照片1,照片2,照片3 " +
                               " from 基础资料_表扬建议管理 " +
                               " where 分类 = @分类 and 联系电话 = @联系电话" +
                               " order by ID desc ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt",sqlstring,
                new SqlParameter("@分类",ztName),
                new SqlParameter("@联系电话",phone));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }
            List<Advise> adviseList = new List<Advise>();
            foreach(DataRow dr in dt.Rows)
            {
                List<string> imageList = new List<string>();
                Advise advise = new Advise();
                advise.Id = DataTypeHelper.GetIntValue(dr["ID"]);
                advise.classify = DataTypeHelper.GetStringValue(dr["分类"]);
                advise.Name = DataTypeHelper.GetStringValue(dr["姓名"]);
                advise.Phone = DataTypeHelper.GetStringValue(dr["联系电话"]);
                advise.RoomNumber = DataTypeHelper.GetStringValue(dr["地址"]);
                advise.Type = DataTypeHelper.GetStringValue(dr["表扬建议"]); 
                advise.Content = DataTypeHelper.GetStringValue(dr["内容"]);
                advise.SubmitDate = DataTypeHelper.GetDateStringValue(dr["提交时间"]);
                imageList.Add(DataTypeHelper.GetStringValue(dr["照片1"]));
                imageList.Add(DataTypeHelper.GetStringValue(dr["照片2"]));
                imageList.Add(DataTypeHelper.GetStringValue(dr["照片3"]));
                advise.Image = imageList.ToArray();
                adviseList.Add(advise);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = adviseList.ToArray();
            return sr;
        }

        public static StatusReport SetAdviseImage(string ID, string func, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = "照片" + index.ToString();
            string sqlString = " update 基础资料_表扬建议管理 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", ID));
            sr.parameters = index;
            return sr;
        }
    }
}