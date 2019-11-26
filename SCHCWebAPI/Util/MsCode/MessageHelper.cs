using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
    /// <summary>
    /// 消息帮助类
    /// </summary>
    public sealed class MessageHelper
    {
        /// <summary>
        /// 废弃状态
        /// </summary>
        public const int NoEnabled = 0;
        /// <summary>
        /// 正常状态
        /// </summary>
        public const int Enabled = 1;
        /// <summary>
        /// 
        /// </summary>
        public const int NoDeleteMark = 0;
        /// <summary>
        /// 正常状态
        /// </summary>
        public const int DeleteMark = 1;
        /// <summary>
        /// 缓存时间
        /// </summary>
        public const int CacheHour_1 = 1;
        /// <summary>
        /// 缓存key-分页
        /// </summary>
        public const string GetQuery = "GetQuery";
        /// <summary>
        /// 缓存key-总记录
        /// </summary>
        public const string RowTotal = "RowTotal";
        /// <summary>
        /// 每页条目数
        /// </summary>
        public const int PAGESIZE = 10;
        /// <summary>
        /// 默认页码
        /// </summary>

        public const int PAGEINDEX = 0;



        /// <summary>
        /// 需要人员验证
        /// </summary>
        public const int Need_Validate = 1;
        /// <summary>
        /// 不需要人员验证
        /// </summary>
        public const int NoNeed_Validate = 0;
        /// <summary>
        /// 销售订单
        /// </summary>
        public const int SALORDER = 1;
        /// <summary>
        /// 退货
        /// </summary>
        public const int SALRETURN = 2;
        /// <summary>
        /// 已回款
        /// </summary>
        public const int ALREADYPAY = 3;
        /// <summary>
        /// 待回款
        /// </summary>
        public const int NEEDPAY = 5;
        /// <summary>
        /// 开票
        /// </summary>
        public const int SALINVOICE = 4;




        /// <summary>
        /// 获取
        /// </summary>
        public const string Obtain = "获取";

        /// <summary>
        /// 成功
        /// </summary>
        public const string SUCCESS = "成功";
        /// <summary>
        /// 失败
        /// </summary>
        public const string FAILD = "失败";
        /// <summary>
        /// 正常
        /// </summary>
        public const string NORMAL = "正常";
        /// <summary>
        /// 否决
        /// </summary>
        public const string CANCEL = "否决";
        /// <summary>
        /// 待处理
        /// </summary>
        public const string PENDING = "待处理";
        /// <summary>
        /// 挂起
        /// </summary>
        public const string SUSPENDED = "挂起";
        /// <summary>
        /// 认可
        /// </summary>
        public const string APPROVE = "认可";
        /// <summary>
        /// 请求
        /// </summary>
        public const string Request = "请求";
        /// <summary>
        /// 申请
        /// </summary>
        public const string APPLY = "申请";
        /// <summary>
        /// 取消
        /// </summary>
        public const string UNDO = "取消";
        /// <summary>
        /// 废弃
        /// </summary>
        public const string ABANDON = "废弃";
        /// <summary>
        /// 初审通过
        /// </summary>
        public const string FIRST_APPROVE = "初审通过";
        /// <summary>
        /// 终审通过
        /// </summary>
        public const string FINAL_APPROVE = "终审通过";
        /// <summary>
        /// 法务审核通过
        /// </summary>
        public const string LAW_APPROVE = "法务审核通过";
        /// <summary>
        /// 财务审核通过
        /// </summary>
        public const string FIN_APPROVE = "财务审核通过";
        /// <summary>
        /// 大区审核通过
        /// </summary>
        public const string Lar_APPROVE = "大区审核通过";
        /// <summary>
        /// 产品经理审核通过
        /// </summary>
        public const string ProMan_APPROVE = "产品经理审核通过";
        /// <summary>
        /// 产品组长审核通过
        /// </summary>
        public const string ProLen_APPROVE = "产品组长审核通过";
        /// <summary>
        /// 总监审核通过
        /// </summary>
        public const string BUS_APPROVE = "总监审核通过";

        /// <summary>
        /// 已出库
        /// </summary>
        public const string ORDER_OUTED = "已出库";
        /// <summary>
        /// 已收货
        /// </summary>
        public const string ORDER_RECV = "已收货";
        /// <summary>
        /// 已确认
        /// </summary>
        public const string Cfm_Apr = "已确认";
        /// <summary>
        /// 未收货
        /// </summary>
        public const string UnreceivedGoods = "未收货";
        /// <summary>
        /// 未确认
        /// </summary>
        public const string Unconfirmed = "未确认";
        /// <summary>
        /// 新增
        /// </summary>
        public const string ADD = "新增";
        /// <summary>
        /// 修改
        /// </summary>
        public const string UPDATE = "修改";
        /// <summary>
        /// 删除
        /// </summary>
        public const string DELETE = "删除";
        /// <summary>
        /// 已发布
        /// </summary>
        public const string PBU = "已发布";
        /// <summary>
        /// 保证金转款
        /// </summary>
        public const string DopSType = "保证金转款";
        /// <summary>
        /// OA专用
        /// </summary>
        public const string WHS_TYPE_OA = "OA专用";
        /// <summary>
        /// 付款类型
        /// </summary>
        public const string PayType = "支出";
        /// <summary>
        /// 收款类型
        /// </summary>
        public const string RecType = "收入";
        /// <summary>
        /// 默认推广部门编码
        /// </summary>
        public const string DefaultMarketingOrgId = "001008";
        /// <summary>
        /// 默认商务部门编码
        /// </summary>
        public const string DefaultBizOrgId = "001008";
        /// <summary>
        /// 获取人员信用额度信息
        /// </summary>
        public const string GET_SAL_RECEIVABLES = "获取人员信用额度信息";
        /// <summary>
        /// 获取用户分管数据
        /// </summary>
        public const string GET_USER_BRANCHED_PASSAGE = "获取用户分管数据";
        /// <summary>
        /// 获取公告信息
        /// </summary>
        public const string GETBASBULLETINPAGE = "获取公告信息";
        /// <summary>
        /// 公司库存表
        /// </summary>
        public const string GET_SALCST_STOCK_LIST = "获取公司库存表";
        /// <summary>
        /// 获取市场活动汇总
        /// </summary>
        public const string GETCONFERENCECOUNT = "获取市场活动汇总";
        /// <summary>
        /// 获取按初次发生流向日期统计
        /// </summary>
        public const string GETFLWFIRSTHAPPEN = "获取按初次发生流向日期统计";
        /// <summary>
        /// 用户登陆
        /// </summary>
        public const string EMP_LOGIN = "用户登陆";
        /// <summary>
        /// 获取通讯录
        /// </summary>
        public const string GET_LINKMAN_SUCCESS = "获取通讯录";
        /// <summary>
        /// 获取物品领用汇总表
        /// </summary>
        public const string GETGOODSSHEETSUM = "获取物品领用汇总表";
        /// <summary>
        /// 部门信息
        /// </summary>
        public const string GET_Org_SUCCESS = "部门信息";
        /// <summary>
        /// 获取用户物品领用清单数据
        /// </summary>
        public const string GET_GOODS_SHEET_SUM = "获取用户物品领用清单数据";
        /// <summary>
        /// 获取品种资料
        /// </summary>
        public const string GET_CATEGORY_RECORD = "获取品种资料";
        /// <summary>
        /// 获取商业档案
        /// </summary>
        public const string GET_BUSSINESS_RECORD = "获取商业档案";
        /// <summary>
        /// 获取医院档案
        /// </summary>
        public const string GET_HOSPITAL_RECORD = "获取医院档案";
        /// <summary>
        /// 获取代理商档案
        /// </summary>
        public const string GET_AGENT_RECORD = "获取医院档案";
        /// <summary>
        /// 获取人员市场分管
        /// </summary>
        public const string GET_EMP_PSN_LIST = "获取人员市场分管";
        /// <summary>
        /// 获取流向信息
        /// </summary>
        public const string GET_SAL_FLOW_LIST = "获取流向信息";
        /// <summary>
        /// 获取区域年度推广进度
        /// </summary>
        public const string GET_BIZAREA_YEAR_RATE = "获取区域年度推广进度";
        /// <summary>
        /// 获取大区年度推广进度
        /// </summary>
        public const string GET_RM_YEAR_RATE = "获取大区年度推广进度";
        /// <summary>
        /// 获取地区年度推广进度
        /// </summary>
        public const string GET_DM_YEAR_RATE = "获取地区年度推广进度";
        /// <summary>
        /// 获取人员年度推广进度
        /// </summary>
        public const string GET_EMP_YEAR_RATE = "获取人员年度推广进度";
        /// <summary>
        /// 非法员工编码
        /// </summary>
        public const string CHECK_EMP_ISVALIDATED_FAILD = "非法员工编码!";
        /// <summary>
        /// 获取销售协议数据
        /// </summary>
        public const string GET_SALCONTRACT_PAGE = "获取销售协议数据";
        /// <summary>
        /// 员工编码为空
        /// </summary>
        public const string EMPLOYEEID_IS_NULL = "员工编码为空";
        /// <summary>
        /// 员工信息不存在
        /// </summary>
        public const string EMPLOYEE_MSG_NOTEXISTS = "员工信息不存在";
        /// <summary>
        /// 获取App人员菜单权限失败
        /// </summary>
        public const string GET_EMPLIMITS_FAILD = "获取App人员菜单权限失败！";
        /// <summary>
        /// 获取App人员菜单权限成功
        /// </summary>
        public const string GET_EMPLIMITS_SUCCESS = "获取App人员菜单权限成功！";
        /// <summary>
        /// 获取订单信息
        /// </summary>
        public const string GET_SALORDERDER_LIST = "获取订单信息!";
        /// <summary>
        /// 每页条目数不能为0
        /// </summary>
        public const string PAGESIZE_ISZERO = "每页条目数不能为0";
        /// <summary>
        /// 获取物流数据
        /// </summary>
        public const string GET_LOGISTICS = "获取物流信息";
        /// <summary>
        /// 获取库存数据
        /// </summary>
        public const string GET_STOCK_DATA = "获取库存数据";
        /// <summary>
        /// 获取今日订单明细
        /// </summary>
        public const string GET_SALORDER_TODAY = "获取今日订单明细";
        /// <summary>
        /// 获取库存有效期预警数据
        /// </summary>
        public const string GST_WOCKWARNING_MSG = "获取库存数据";
        /// <summary>
        /// 获取客户应收款数据
        /// </summary>
        public const string GET_FINACCOUNTS_PAYABLE = "获取客户应收款数据!";
        /// <summary>
        /// 获取客户应付款数据
        /// </summary>
        public const string GET_FINCSTCATACCOUNTS_PAYABLE = "获取客户应付款数据!";
        /// <summary>
        /// 获取开户汇总信息
        /// </summary>
        public const string GET_CUSTOMER_APPLY = "获取开户汇总信息!";
        /// <summary>
        /// 获取销售员工个人业绩
        /// </summary>
        public const string GET_PERSONAL_PERFORMANCE_SAL = "获取销售员工个人业绩";
        /// <summary>
        /// 商务
        /// </summary>
        public const string BUSSINESS = "商务";
        /// <summary>
        /// 推广
        /// </summary>
        public const string GENERALIZE = "推广";
        /// <summary>
        /// 招商
        /// </summary>
        public const string UNIONGENERALIZE = "招商";
        /// <summary>
        /// 流向单
        /// </summary>
        public const string SalFlowWhere = "流向单";
        /// <summary>
        /// 部门分管
        /// </summary>
        public const string OrgWhere = "部门";
        /// <summary>
        /// 客户+品种
        /// </summary>
        public const string CstAndCatWhere = "客户+品种";

        /// <summary>
        /// 验证用户名密码失败
        /// </summary>
        public const string FailedToVerifyUsernamePassword = "验证用户名密码失败";

        /// <summary>
        /// 用户名不能为空
        /// </summary>
        public const string UserNameCannotBeEmpty = "用户名不能为空!";
        /// <summary>
        /// 密码不能为空
        /// </summary>
        public const string PasswordCannotBeEmpty = "密码不能为空!";

        /// <summary>
        /// 平台不能为空
        /// </summary>
        public const string PlatformCannotBeEmpty = "平台不能为空!";
        /// <summary>
        /// 登陆成功
        /// </summary>
        public const string LandingSuccessfully = "登陆成功";

        /// <summary>
        /// 暂时无返回内容
        /// </summary>
        public const string NoReturnContent = "没有写返回内容";

        /// <summary>
        /// 人员信息
        /// </summary>
        public const string PersonnelInformation = "人员信息";

        /// <summary>
        /// 菜单信息
        /// </summary>
        public const string MenuInformation = "菜单信息";

        /// <summary>
        /// 不能为空
        /// </summary>
        public const string NoEmpty = "不能为空";

        /// <summary>
        /// 操作
        /// </summary>
        public const string Action = "操作";

        /// <summary>
        /// 操作成功
        /// </summary>
        public const string OKMESSAGE = Action + SUCCESS;
        /// <summary>
        /// 操作失败
        /// </summary>
        public const string ERRORMESSAGE = Action + FAILD;
        /// <summary>
        /// 验证失败
        /// </summary>
        public const string ValidationFailed = "验证失败!";
        /// <summary>
        /// 验证码失效
        /// </summary>
        public const string VerificationCodeFailure = "验证码失效";
        /// <summary>
        /// 手机号码无效
        /// </summary>
        public const string InvalidMobilePhoneNumber="手机号码无效";
    }
}
