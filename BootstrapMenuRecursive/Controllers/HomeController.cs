using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BootstrapMenuRecursive.Models.MenuModels;

namespace BootstrapMenuRecursive.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["_MenuJson"] = this.GetMenu();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 取得選單
        /// </summary>
        /// <returns></returns>
        public string GetMenu()
        {
            // 樹狀結構物件
            MenuList outModel = new MenuList();

            outModel.Menus = new List<MenuItem>();

            // 取得連線字串
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

            // 當程式碼離開 using 區塊時，會自動關閉連接
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // 資料庫連線
                conn.Open();

                // 取得選單資料
                string sql = "select * from Menu";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                // 執行資料庫查詢動作
                SqlDataAdapter adpt = new SqlDataAdapter();
                adpt.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    // 開始遞迴查詢子節點
                    this.GetMenuRecursive(dt, outModel.Menus, "0"); //0 是根節點
                }
            }

            // 將物件轉為 Json 給前端使用
            string json = JsonConvert.SerializeObject(outModel);
            return json;
        }

        /// <summary>
        /// 取得樹狀結構(遞迴模式)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Menus"></param>
        /// <param name="ParentID"></param>
        public void GetMenuRecursive(DataTable dt, List<MenuItem> Menus, string ParentID)
        {
            // 查詢子節點
            string filter = "ParentID = '" + ParentID + "'";
            DataRow[] drs = dt.Select(filter);
            foreach (DataRow dr in drs)
            {
                // 選單項目
                MenuItem menu = new MenuItem();
                menu.MenuName = dr["MenuName"].ToString();
                menu.Url = dr["Url"].ToString();
                menu.Menus = new List<MenuItem>();

                //加入子節點
                Menus.Add(menu);

                // 遞迴查詢子節點
                string MenuID = dr["MenuID"].ToString();
                this.GetMenuRecursive(dt, menu.Menus, MenuID);
            }
        }
    }
}