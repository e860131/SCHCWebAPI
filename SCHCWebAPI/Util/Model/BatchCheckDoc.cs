using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
  public class BatchCheckDoc
  {
    public string Cat_ID { get; set; }

    public string Cat_Name { get; set; }

    public string Cat_SimSpec { get; set; }

    public string Cat_Producer { get; set; }

    public string Cat_ProduceNo { get; set; }

    public string BatchNo { get; set; }

    public string CheckNO { get; set; }

    public string imgs { get; set; }
  }

  public class CatDoc
  {
    public string Cat_ID { get; set; }

    public string Cat_Name { get; set; }

    public string Cat_SimSpec { get; set; }

    public string Cat_Producer { get; set; }

    public string Cat_ProduceNo { get; set; }

    public string imgs { get; set; }
  }

  public class SalOrder
  {
    public string Sal_DID { get; set; }
    public string Sal_ID { get; set; }

    public string Sal_OpDate { get; set; }

    public string Cat_ID { get; set; }

    public string Sal_BatchNo { get; set; }

    public string Sal_DetlQty { get; set; }

    public string Sal_DetlAmount { get; set; }

    public string Cat_Name { get; set; }

    public string Cat_SimSpec { get; set; }

    public string Cat_Producer { get; set; }

    public string imgs { get; set; }
  }

  /// <summary>
  /// 图片路径
  /// </summary>
  public class ImageUrl
  {
    /// <summary>
    /// 图片路径
    /// </summary>
    /// <value></value>
    public string src { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    /// <value></value>
    public string name { get; set; }
  }
}
