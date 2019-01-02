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
    public class DeliveryDal
    {
        public static StatusReport GetDelivery(string ztcode, string phone)
        {
            StatusReport sr = new StatusReport();
            string sqlString =  " SELECT ID, 分类, 收件人地址, 收件人, 联系电话, 物品描述, 送件人, 送件时间, 领取人, 领取人电话,领取时间, 登记人, 登记时间, 状态 " +
                                " FROM dbo.基础资料_代收管理 " +
                                " WHERE   (分类 = @分类) AND (联系电话 = @联系电话) " +
                                " ORDER BY ID DESC";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@分类", ztcode),
                new SqlParameter("@联系电话", phone));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "无数据";
            }
            else
            {
                List<Delivery> deliveryList = new List<Delivery>();
                foreach(DataRow dr in dt.Rows)
                {
                    Delivery delivery = new Delivery()
                    {
                        Classify = DataTypeHelper.GetStringValue(dr["分类"]),
                        Address = DataTypeHelper.GetStringValue(dr["收件人地址"]),
                        GoodsInfo = DataTypeHelper.GetStringValue(dr["物品描述"]),
                        Addressee = DataTypeHelper.GetStringValue(dr["收件人"]),
                        AddresseePhone = DataTypeHelper.GetStringValue(dr["联系电话"]),
                        Deliver = DataTypeHelper.GetStringValue(dr["送件人"]),
                        DeliveryTime = DataTypeHelper.GetDateStringValue(dr["送件时间"]),
                        Receiver = DataTypeHelper.GetStringValue(dr["领取人"]),
                        ReceiverPhone = DataTypeHelper.GetStringValue(dr["领取人电话"]),
                        ReceiveTime = DataTypeHelper.GetDateStringValue(dr["领取时间"]),
                        Registrant = DataTypeHelper.GetStringValue(dr["登记人"]),
                        RegisterTime = DataTypeHelper.GetDateStringValue(dr["登记时间"]),
                        Status = DataTypeHelper.GetStringValue(dr["状态"]),
                    };
                    deliveryList.Add(delivery);
                }
                sr.status = "Success";
                sr.result = "成功";
                sr.data = deliveryList.ToArray();
            }
            return sr;
        }
    }
}