using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
  public class ImageImp : BaseRepository<ImageUrl>
  {
    /// <summary>
    /// 获取品种,批号对应的药检单路径明细
    /// </summary>
    /// <param name="fileurl"></param>
    /// <param name="CatBatchStr">110@77^88@99</param>
    /// <returns></returns>
    public async Task<List<ImageUrl>> GetCatBatchDetl(string fileurl, string CatBatchStr)
    {
      string[] str = CatBatchStr.Split('^');
      StringBuilder stringBuilder = new StringBuilder();
      int i = 0;
      foreach (string str1 in str)
      {
        i++;
        string[] str2 = str1.Split('@');
        stringBuilder.AppendFormat("select src='{2}'+REPLACE(ImgPaths,'\','/'),name=CheckNO+'-'+Convert(nvarchar(20),CCD_ID)  from  WH_CatBatchCheckDoc a where Cat_ID='{0}' and BatchNo='{1}' and imgpaths is not null", str2[0], str2[1], fileurl);
        if (i < str.Count())
          stringBuilder.Append(" union ");
      }
      return await Task.Run(() => Context.Db.SqlQueryable<ImageUrl>(stringBuilder.ToString()).ToList());
    }

    /// <summary>
    /// 获取品种对应的材料路径明细
    /// </summary>
    /// <param name="fileurl"></param>
    /// <param name="Cat_IDStr"></param>
    /// <returns></returns>
    public async Task<List<ImageUrl>> GetCatDocDetl(string fileurl, string Cat_IDStr)
    {

      string[] CatID = Cat_IDStr.Split('@');
      string salstr = "";
      foreach (var s in CatID)
      {
        if (!string.IsNullOrEmpty(salstr))
        {
          salstr = salstr + ",'" + s + "'";
        }
        else
        {
          salstr = "'" + s + "'";
        }
      }
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(@"select src='{1}'+REPLACE(CtDoc_PicLuJin,'\','/'),name=Cat_ID+'-'+Convert(nvarchar(20),CtDoc_ID)
                                           from V_Bas_CategoryDoc a where Cat_ID in ('{0}') 
                           and(CtDoc_ValidDate is null or CtDoc_ValidDate > CONVERT(date, GETDATE())) and CtDoc_PicLuJin is not null and CtDoc_PicLuJin <> ''", salstr, fileurl);

      var dt = await Task.Run(() => Context.Db.SqlQueryable<ImageUrl>(stringBuilder.ToString()).ToList());
      return dt;
    }

    /// <summary>
    /// 获取对应销售明细单号的材料路径明细
    /// </summary>
    /// <param name="fileurl"></param>
    /// <param name="Sal_DIDStr">明细单号字符串,如:1832@24323</param>
    /// <returns></returns>
    public async Task<List<ImageUrl>> GetInvOrderDocDetl(string fileurl, string Sal_DIDStr)
    {
      //获取采购单
      string[] Sal_DID = Sal_DIDStr.Split('@');
      if (Sal_DID.Count() > 0)
      {
        string salstr = "";
        foreach (var s in Sal_DID)
        {
          if (!string.IsNullOrEmpty(salstr))
          {
            salstr = salstr + ',' + s;
          }
          else
          {
            salstr = s;
          }
        }
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat(@"select name=Item,src='{1}'+REPLACE(FileUrl,'\','/') from Sal_OrderDetlAddInfo where Sal_DID in ({0})", salstr, fileurl);
        var dt = await Task.Run(() => Context.Db.SqlQueryable<ImageUrl>(stringBuilder.ToString()).ToList());
        return dt;
      }
      else
        return null;
    }
  }
}