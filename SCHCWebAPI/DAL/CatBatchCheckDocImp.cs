using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Data;

namespace SCHCWebAPI
{
  /// <summary>
  /// 药检单
  /// </summary>
  public class CatBatchCheckDocImp : BaseRepository<BatchCheckDoc>
  {
    /// <summary>
    /// 获取药检单记录
    /// </summary>
    /// <param name="cst_id"></param>
    /// <param name="CatName"></param>
    /// <param name="opdatebegin"></param>
    /// <param name="batchno"></param>
    /// <param name="pageindex"></param>
    /// <returns></returns>
    public async Task<List<BatchCheckDoc>> GetCatBatchCheckDoc(string cst_id, string CatName, string opdatebegin, string batchno, int pageindex)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(opdatebegin))
      {
        stringBuilder.AppendFormat(@"select ROW_NUMBER() OVER (ORDER BY Cat_ID )AS Row,
            Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,BatchNo,CheckNO,imgs=count(imgpaths) 
            from V_WH_CatBatchCheckDoc a where 1=1 and 
            exists(select * from Sal_OrderDetl l,Sal_Order m where l.Sal_ID=m.Sal_ID and l.Cat_ID=a.Cat_ID and Sal_BatchNo=a.BatchNo and sal_salstatus='已出库' 
                and m.Cst_ID='{0}'
                and m.Sal_Opdate>='{1}'
            )", cst_id, Convert.ToDateTime(opdatebegin).ToShortDateString());
      }
      else
      {
        stringBuilder.AppendFormat(@"select ROW_NUMBER() OVER (ORDER BY Cat_ID )AS Row,
            Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,BatchNo,CheckNO,imgs=count(imgpaths) 
            from V_WH_CatBatchCheckDoc a where 1=1 and 
            exists(select * from Sal_OrderDetl l,Sal_Order m where l.Sal_ID=m.Sal_ID and l.Cat_ID=a.Cat_ID and Sal_BatchNo=a.BatchNo and sal_salstatus='已出库' 
                and m.Cst_ID='{0}'
            )", cst_id);
      }
      if (!string.IsNullOrEmpty(CatName))
      {
        stringBuilder.AppendFormat(" and (Cat_ID like '%{0}%' or Cat_Name like '%{0}%' or Cat_ChineseName like '%{0}%' or Cat_SimpleName like '%{0}%' or Cat_Producer like '%{0}%' )", CatName);
      }
      if (!string.IsNullOrEmpty(batchno))
      {
        stringBuilder.AppendFormat(" and BatchNo like '%{0}%'", batchno);
      }
      stringBuilder.AppendFormat("group by Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,BatchNo,CheckNO");

      var r = await Task.Run(() => Context.Db.SqlQueryable<BatchCheckDoc>(stringBuilder.ToString()).ToPageList(pageindex, 10));

      return r;
    }
    /// <summary>
    /// 获取药检条目数
    /// </summary>
    /// <param name="cst_id"></param>
    /// <param name="CatName"></param>
    /// <param name="opdatebegin"></param>
    /// <param name="batchno"></param>
    /// <returns></returns>
    public async Task<int> GetCatBatchCount(string cst_id, string CatName, string opdatebegin, string batchno)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(opdatebegin))
      {
        stringBuilder.AppendFormat(@"select 
            Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,BatchNo,CheckNO,imgs=count(imgpaths) 
            from V_WH_CatBatchCheckDoc a where 1=1 and 
            exists(select * from Sal_OrderDetl l,Sal_Order m where l.Sal_ID=m.Sal_ID and l.Cat_ID=a.Cat_ID and Sal_BatchNo=a.BatchNo and sal_salstatus='已出库' 
                and m.Cst_ID='{0}'
                and m.Sal_Opdate>='{1}'
            )", cst_id, Convert.ToDateTime(opdatebegin).ToShortDateString());
      }
      else
      {
        stringBuilder.AppendFormat(@"select 
            Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,BatchNo,CheckNO,imgs=count(imgpaths) 
            from V_WH_CatBatchCheckDoc a where 1=1 and 
            exists(select * from Sal_OrderDetl l,Sal_Order m where l.Sal_ID=m.Sal_ID and l.Cat_ID=a.Cat_ID and Sal_BatchNo=a.BatchNo and sal_salstatus='已出库' 
                and m.Cst_ID='{0}'
            )", cst_id);
      }
      if (!string.IsNullOrEmpty(CatName))
      {
        stringBuilder.AppendFormat(" and (Cat_ID like '%{0}%' or Cat_Name like '%{0}%' or Cat_ChineseName like '%{0}%' or Cat_SimpleName like '%{0}%' or Cat_Producer like '%{0}%' )", CatName);
      }
      if (!string.IsNullOrEmpty(batchno))
      {
        stringBuilder.AppendFormat(" and BatchNo like '%{0}%'", batchno);
      }
      stringBuilder.AppendFormat("group by Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,BatchNo,CheckNO");

      var r = await Task.Run(() => Context.Db.SqlQueryable<BatchCheckDoc>(stringBuilder.ToString()).Count());

      return r;
    }

  }
}
