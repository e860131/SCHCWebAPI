using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
  public class OrderImp : BaseRepository<SalOrder>
  {
    /// <summary>
    /// 获取订单信息
    /// </summary>
    /// <param name="cst_id"></param>
    /// <param name="CatName"></param>
    /// <param name="batchno"></param>
    /// <param name="Sal_ID"></param>
    /// <param name="begindate"></param>
    /// <param name="endate"></param>
    /// <param name="pageindex"></param>
    /// <returns></returns>
    public async Task<List<SalOrder>> GetOrderInfo(string cst_id, string CatName, string batchno, string Sal_ID, string begindate, string endate, int pageindex)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(@"select Sal_DID, Sal_ID,Sal_OpDate,Cat_ID,Sal_BatchNo,Sal_DetlQty,Cat_Name,
                                                Cat_SimSpec,Cat_Producer, Sal_DetlAmount,imgs=isnull((select count(*) from Sal_OrderDetlAddInfo where Sal_OrderDetlAddInfo.Sal_DID=V_Sal_OrderDetl.Sal_DID),0)
                                         from V_Sal_OrderDetl where Sal_SalStatus='已出库' and Sal_Datamark='正常' and Sal_SalType='销售' and Cst_ID='{0}'
            ", cst_id);
      if (!string.IsNullOrEmpty(CatName))
      {
        stringBuilder.AppendFormat(" and (Cat_ID like '%{0}%' or Cat_Name like '%{0}%' or Cat_ChineseName like '%{0}%' or Cat_SimpleName like '%{0}%' or Cat_Producer like '%{0}%' )", CatName);
      }
      if (!string.IsNullOrEmpty(Sal_ID))
      {
        stringBuilder.AppendFormat(" and Sal_ID like '%{0}%'", Sal_ID);
      }
      if (!string.IsNullOrEmpty(batchno))
      {
        stringBuilder.AppendFormat(" and Sal_BatchNo like '%{0}%'", batchno);
      }
      if (!string.IsNullOrEmpty(begindate))
      {
        stringBuilder.AppendFormat(" and Sal_OpDate>='{0}'", Convert.ToDateTime(begindate).ToShortDateString());
      }
      if (!string.IsNullOrEmpty(endate))
      {
        stringBuilder.AppendFormat(" and Sal_OpDate<'{0}'", Convert.ToDateTime(endate).AddDays(1).ToShortDateString());
      }
      var r = await Task.Run(() => Context.Db.SqlQueryable<SalOrder>(stringBuilder.ToString()).ToPageList(pageindex, 10));

      return r;
    }

    /// <summary>
    /// 获取订单记录数
    /// </summary>
    /// <param name="cst_id"></param>
    /// <param name="CatName"></param>
    /// <param name="batchno"></param>
    /// <param name="Sal_ID"></param>
    /// <param name="begindate"></param>
    /// <param name="endate"></param>
    /// <returns></returns>
    public async Task<int> GetOrderCount(string cst_id, string CatName, string batchno, string Sal_ID, string begindate, string endate)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(@"select Sal_DID,Sal_ID,Sal_OpDate,Cat_ID,Sal_BatchNo,Sal_DetlQty,Cat_Name,
                                                Cat_SimSpec,Cat_Producer, Sal_DetlAmount,imgs=isnull((select count(*) from Sal_OrderDetlAddInfo where Sal_OrderDetlAddInfo.Sal_DID=V_Sal_OrderDetl.Sal_DID),0)
                                         from V_Sal_OrderDetl where Sal_SalStatus='已出库' and Sal_Datamark='正常' and Sal_SalType='销售' and Cst_ID='{0}'
            ", cst_id);
      if (!string.IsNullOrEmpty(CatName))
      {
        stringBuilder.AppendFormat(" and (Cat_ID like '%{0}%' or Cat_Name like '%{0}%' or Cat_ChineseName like '%{0}%' or Cat_SimpleName like '%{0}%' or Cat_Producer like '%{0}%' )", CatName);
      }
      if (!string.IsNullOrEmpty(Sal_ID))
      {
        stringBuilder.AppendFormat(" and Sal_ID like '%{0}%'", Sal_ID);
      }
      if (!string.IsNullOrEmpty(batchno))
      {
        stringBuilder.AppendFormat(" and Sal_BatchNo like '%{0}%'", batchno);
      }
      if (!string.IsNullOrEmpty(begindate))
      {
        stringBuilder.AppendFormat(" and Sal_OpDate>='{0}'", Convert.ToDateTime(begindate).ToShortDateString());
      }
      if (!string.IsNullOrEmpty(endate))
      {
        stringBuilder.AppendFormat(" and Sal_OpDate<'{0}'", Convert.ToDateTime(endate).AddDays(1).ToShortDateString());
      }
      var r = await Task.Run(() => Context.Db.SqlQueryable<SalOrder>(stringBuilder.ToString()).Count());

      return r;
    }
  }
}
