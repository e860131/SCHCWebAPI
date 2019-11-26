using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
  public class CatDocImp : BaseRepository<CatDoc>
  {
    /// <summary>
    /// 获取品种材料记录
    /// </summary>
    /// <param name="cst_id"></param>
    /// <param name="CatName"></param>
    /// <param name="pageindex"></param>
    /// <returns></returns>
    public async Task<List<CatDoc>> GetCatDoc(string cst_id, string CatName, int pageindex)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(@"select Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,imgs=count(*) 
                                         from V_Bas_CategoryDoc a where (CtDoc_ValidDate is null or CtDoc_ValidDate>CONVERT(date,GETDATE()))
                                          and exists(select * from Sal_OrderDetl l,Sal_Order m where l.Sal_ID=m.Sal_ID and Cat_ID=a.Cat_ID and Cst_ID='{0}')
                                          and CtDoc_PicLuJin<>''
            ", cst_id);
      if (!string.IsNullOrEmpty(CatName))
      {
        stringBuilder.AppendFormat(" and (Cat_ID like '%{0}%' or Cat_Name like '%{0}%' or Cat_ChineseName like '%{0}%' or Cat_SimpleName like '%{0}%' or Cat_Producer like '%{0}%' )", CatName);
      }
      stringBuilder.AppendFormat("group by Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo");

      var r = await Task.Run(() => Context.Db.SqlQueryable<CatDoc>(stringBuilder.ToString()).ToPageList(pageindex, 10));

      return r;
    }

    /// <summary>
    /// 获取品种材料记录数
    /// </summary>
    /// <param name="cst_id"></param>
    /// <param name="CatName"></param>
    /// <returns></returns>
    public async Task<int> GetCatDocCount(string cst_id, string CatName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(@"select Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo,imgs=count(*) 
                                         from V_Bas_CategoryDoc a where (CtDoc_ValidDate is null or CtDoc_ValidDate>CONVERT(date,GETDATE()))
                                          and exists(select * from Sal_OrderDetl l,Sal_Order m where l.Sal_ID=m.Sal_ID and Cat_ID=a.Cat_ID and Cst_ID='{0}')
                                          and CtDoc_PicLuJin<>''
            ", cst_id);
      if (!string.IsNullOrEmpty(CatName))
      {
        stringBuilder.AppendFormat(" and (Cat_ID like '%{0}%' or Cat_Name like '%{0}%' or Cat_ChineseName like '%{0}%' or Cat_SimpleName like '%{0}%' or Cat_Producer like '%{0}%' )", CatName);
      }
      stringBuilder.AppendFormat("group by Cat_ID,Cat_Name,Cat_SimSpec,Cat_Producer,Cat_ProduceNo");

      var r = await Task.Run(() => Context.Db.SqlQueryable<CatDoc>(stringBuilder.ToString()).Count());

      return r;
    }
  }
}
