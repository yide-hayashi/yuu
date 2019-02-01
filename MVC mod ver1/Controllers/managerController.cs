using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.Web.Configuration;
using System.IO;
using System.Web.Security;

namespace WebApplication1.Controllers
{
    public class managerController : Controller
    {
        private static List<string> list = new List<string>();
        private static string imgstr = "", modoimg = "" ;
        private static int selectedcount = 0;
        string sqlconStr = WebConfigurationManager.ConnectionStrings[2].ToString();
        // GET: manager
        [HttpGet]
        public ActionResult Index()
        {
            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Response.Cookies["nyaa"].Expires = DateTime.Parse("2000/1/1");
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(managerModel mm)
        {
            if (mm.account_p != "" && mm.account_p!=null &&mm.pw_p!=null && mm.pw_p!="")
            {
                string pw = "";
                MySqlConnection con = new MySqlConnection(sqlconStr);
                MySqlCommand link;
                HttpCookie cookie = new HttpCookie("nyaa");
                Response.Cookies.Clear();
                con.Open();
                link = con.CreateCommand();
                link.CommandText = "SELECT * FROM localsql.account WHERE( account=@acc  )";
                link.Parameters.AddWithValue("acc", mm.account_p.Replace("/[\'\"]+/", mm.account_p));
                MySqlDataReader read = link.ExecuteReader();
                while (read.Read())
                {
                    pw = read["password"].ToString();
                }
                if (pw != mm.pw_p)
                {
                    ViewData["msg"] = "帳號密碼錯誤";
                    return View();
                }
                else
                {
                    cookie.Value = "testlogin_"+mm.account_p;
                    cookie.Name = "nyaa";
                    cookie.Expires = DateTime.Now.AddHours(3);
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("index");
                }
            }
            else
            {
                ViewData["msg"] = "帳號密碼錯誤";
                return View();
            }
        }
        private bool AccountSession()
        {
            string login = "";


            if (Request.Cookies["nyaa"] != null)
            {
                login = Request.Cookies["nyaa"].Value.ToString();
                ViewData["acc"] = login.Substring(10,login.Length-10);
                if (login.Substring(0,9) == "testlogin")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        [HttpGet]
        
        public ActionResult CreateNotice(string tab)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "志工", Value = "志工" });
            items.Add(new SelectListItem() { Text = "活動", Value = "活動" });
            items.Add(new SelectListItem() { Text = "會議", Value = "會議" });
            items.Add(new SelectListItem() { Text = "訓練", Value = "訓練" });
            ViewBag.test = items;
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "", Value = "" });
            types.Add(new SelectListItem() { Text = "一般", Value = "一般" });
            types.Add(new SelectListItem() { Text = "專案", Value = "專案" });


            ViewBag.type = types;
            ViewBag.tab = tab;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateNotice(managerModel mm)
        {
            Boolean f = false; //建立相簿model
            if (ModelState.IsValid && mm.NoticeTitle_p != "")
            {
                try
                {
                    mm.NoticeCreate();
                }
                catch (Exception ex)
                {
                    f = true;
                }
                if (f == false)
                {
                    TempData.Add("error", "0");
                    return RedirectToAction("index");
                }
                else
                {
                    TempData.Add("error", "1");
                    return RedirectToAction("CreateNotice");
                }
            }
            else if (ModelState.IsValid && mm.AlbumTitle_p != "" && mm.AlbumContent_p != "" && list != null &&
                mm.AlbumTitle_p != null && mm.AlbumContent_p != null)
            {
                mm.list_p = list;
                mm.AlbumCreate();
                MoveFiles("\\Files", "\\album\\" + mm.CountLine);
                //檔案移動到  \album\CountLine\imgAddress.jpg
                TempData.Add("error", "0");
                return RedirectToAction("CreateNotice","manager");
            }
            else if (ModelState.IsValid && mm.TeachContent_p != "" && mm.TeachTitle_p != "" &&
                mm.TeachContent_p != null && mm.TeachTitle_p != null)
            {
                mm.TeachCreate();
                TempData.Add("error", "0");
                return RedirectToAction("CreateNotice");
            }
            else
            {
                TempData.Add("error", "2");
                return RedirectToAction("CreateNotice");
            }
        }

        [HttpGet]
        public ActionResult NoticeDel(string na)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM localsql.notice
                            WHERE  title=@title";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("title", na);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("LookNoticeTable");
        }

        [HttpGet]
        public ActionResult ChangeNotice(string na)
        {
            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            managerModel mm = new managerModel();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "志工", Value = "志工" });
            items.Add(new SelectListItem() { Text = "活動", Value = "活動" });
            items.Add(new SelectListItem() { Text = "會議", Value = "會議" });
            items.Add(new SelectListItem() { Text = "訓練", Value = "訓練" });
            ViewBag.test = items;
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "", Value = "" });
            types.Add(new SelectListItem() { Text = "一般", Value = "一般" });
            types.Add(new SelectListItem() { Text = "專案", Value = "專案" });
            ViewBag.type = types;
            string[][]data=selectsql("SELECT * FROM localsql.notice WHERE( title='"+na+"' ) ", new[] { "title","type","type_eventtype","Content" }, 0, -1);
            mm.NoticeTitle_p = data[0][0];
            mm.NoticeType_p = data[0][1];
            mm.NoticeType_eventtype_p = data[0][2];
            mm.NoticeContent_p = data[0][3];
            return View(mm);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangeNotice(managerModel mm)
        {
           string sqlstr = @"UPDATE localsql.notice SET type=@type,type_eventtype=@tet, Content=@content
                                WHERE title=@title";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            link = con.CreateCommand();
            con.Open();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("title", mm.NoticeTitle_p);
            link.Parameters.AddWithValue("content", mm.NoticeContent_p);
            link.Parameters.AddWithValue("type", mm.NoticeType_p);
            link.Parameters.AddWithValue("tet", mm.NoticeType_eventtype_p);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("LookNoticeTable");
        }
        [HttpGet]
        public ActionResult LookNoticeTable()
        {
            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadNoticeList());
        }

        [HttpGet]
        public ActionResult LookFeedbackTable()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadFBList());
        }

        [HttpGet]
        public ActionResult LookAlbumTable()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadAlbumList());
        }
        [HttpGet]
        public ActionResult upalbum()
        {
            list.Clear();
            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }

            return View();
        }
        [HttpPost]
        public ActionResult upalbum(IEnumerable<HttpPostedFileBase> files)
        {
            bool error = false;
            int count = 0;
            string message = "";
            managerModel mm = new managerModel();
            
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        //驗證檔案類型
                        if (file.ContentType.Substring(0, 5) == "image")
                        {

                            if (!Directory.Exists(Server.MapPath("~/Files/")))
                                Directory.CreateDirectory(Server.MapPath("~/Files/"));
                            string path = Path.Combine(Server.MapPath("~/Files/"), fileName);

                            list.Add(fileName);
                            count++;
                            file.SaveAs(path+"0");
                            mm.imgfunction(path+"0", path,800);
                        }
                        else
                        {
                            error = true;
                            message = "0x001"; //タイプが違いあり
                        }
                    }
                }
                if (error == false)
                {
                    message = "0";
                    ViewData["img"] = list;
                }
                
            }
            return View("upalbum");
        }
        [HttpGet]
        public ActionResult AlbumChange(int x)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            managerModel mm = new managerModel();
            string sqlstr = @"SELECT * FROM localsql.album WHERE(album.AlbumCount=" + x.ToString() + ")";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                mm.AlbumTitle_p = read["title"].ToString();
                mm.AlbumContent_p = read["Content"].ToString();
                mm.date_p = read["date"].ToString();
                mm.OptionAddCount_p = int.Parse(read["albumCount"].ToString());
            }
            ViewBag.album = x;
            ViewBag.imgArray = selectsql("SELECT * FROM localsql.album_address WHERE(" + x.ToString() + "=album_address.album_AlbumCount)", new[] { "ImgAddress" }, 0, -1);
            con.Close();
            return View(mm);
        }
        [HttpPost]
        public ActionResult AlbumChange(managerModel mm)
        {
            string sqlstr = @"INSERT INTO localsql.album_address(
                                ImgAddress,album_AlbumCount)VALUES(@ImgAddress," + mm.OptionAddCount_p + ")";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            link = con.CreateCommand();
            con.Open();
            int i = 0;
            if (list.Count != 0)
            {
                MoveFiles("\\Files", "\\album\\" + mm.OptionAddCount_p);
                while (i < list.Count)
                {
                        link.CommandText = sqlstr;
                        link.Parameters.Clear();
                        link.Parameters.AddWithValue("ImgAddress", list[i].ToString());
                        link.ExecuteNonQuery();
                  
                    i++;
                }
            }
            sqlstr = @"UPDATE localsql.album SET title=@title, content=@content
                                WHERE AlbumCount=@AlbumCount";
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("title", mm.AlbumTitle_p);
            link.Parameters.AddWithValue("content", mm.AlbumContent_p);
            link.Parameters.AddWithValue("AlbumCount", mm.OptionAddCount_p);
            link.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("LookAlbumTable");
        }
        [HttpGet]
        public ActionResult AlbumDel(int x, string na)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM localsql.album_address
                            WHERE Imgaddress=@Imgaddress AND album_AlbumCount=@album_AlbumCount";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("album_AlbumCount", x);
            link.Parameters.AddWithValue("Imgaddress", na);
            link.ExecuteNonQuery();
            con.Close();
            if (Directory.Exists(Server.MapPath("/album/" + x.ToString() + "/")))
            {
                managerModel mm = new managerModel();
                mm.DeleteFile(Server.MapPath("/album/" + x.ToString() + "/"), na);
            }
            return RedirectToAction("AlbumChange", new { x = x });
        }

        [HttpGet]
        public ActionResult AlbumDelall(int x)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM  localsql.album_address
                            WHERE album_AlbumCount=@AlbumCount";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("AlbumCount", x);
            link.ExecuteNonQuery();

            sqlstr = @"DELETE FROM  localsql.album
                            WHERE AlbumCount=@AlbumCount";
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("AlbumCount", x);
            link.ExecuteNonQuery();
            con.Close();
            if (Directory.Exists(Server.MapPath("/album/" + x.ToString() + "/")))
            {
                managerModel mm = new managerModel();
                mm.DelDir(Server.MapPath("/album/" + x.ToString() + "/"));
            }
            return RedirectToAction("LookAlbumTable");
        }
         
        
       [HttpGet]
        public ActionResult FeedbackDelall(int x)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM  localsql.feedback
                            WHERE idfeedback=@idFB";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("idFB", x);
            link.ExecuteNonQuery();

            
            con.Close();
            return RedirectToAction("LookFeedbackTable");
        }

        [HttpGet]
        public ActionResult LookTeachTable()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadTeachList());
        }
        [HttpGet]
        public ActionResult TeachChange(string name)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            managerModel mm = new managerModel();
            string sqlstr = @"SELECT * FROM localsql.teach WHERE(title= '" + name.ToString() + "')";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                mm.TeachTitle_p = read["title"].ToString();
                mm.TeachContent_p = read["Content"].ToString();
            }
            con.Close();
            return View(mm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TeachChange(managerModel mm)
        {
            string sqlstr = "";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            link = con.CreateCommand();
            con.Open();
            int i = 0;
            sqlstr = @"UPDATE localsql.teach SET content=@content
                                WHERE title= '" + mm.TeachTitle_p.ToString() + "'";
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("content", mm.TeachContent_p);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("LookTeachTable");
        }
        [HttpGet]
        public ActionResult TeachDel(string name)
        {
            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM localsql.teach
                            WHERE title=@title";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("title", name);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("LookTeachTable");
        }


        [HttpGet]
        public ActionResult HeadImgOption()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"SELECT * FROM localsql.HeadImg";
            string imgpath = "";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                imgpath = read["HeadImg"].ToString();
            }
            con.Close();
            ViewBag.imgpath = imgpath;
            return View();
        }
        [HttpGet]
        public ActionResult HeadImgOptionChange()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult HeadImgOptionChange(IEnumerable<HttpPostedFileBase> files) //要事先新增一個count=0的欄位
        {
            bool error = false;
            int count = 0;
            string message = "";
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        //驗證檔案類型
                        if (file.ContentType.Substring(0, 5) == "image")
                        {

                            if (!Directory.Exists(Server.MapPath("~/img/head")))
                                Directory.CreateDirectory(Server.MapPath("~/img/head"));
                            string path = Path.Combine(Server.MapPath("~/img/head"), fileName);
                            string sqlstr = @"UPDATE headimg SET HeadImg=@HeadImg 
                                WHERE count=@count";
                            MySqlConnection con = new MySqlConnection(sqlconStr);
                            MySqlCommand link;
                            con.Open();
                            link = con.CreateCommand();
                            link.CommandText = sqlstr;
                            link.Parameters.Clear();
                            link.Parameters.AddWithValue("HeadImg", fileName);
                            link.Parameters.AddWithValue("count", 0);
                            link.ExecuteNonQuery();
                            con.Close();
                            file.SaveAs(path);
                        }
                        else
                        {
                            error = true;
                            message = "0x001"; //タイプが違いあり
                        }
                    }
                }
                if (error == false)
                {
                    message = "0";
                    ViewData["img"] = list;

                }
            }
            return RedirectToAction("HeadImgOption");
        }

        [HttpGet]
        public ActionResult HeadTurnImg()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            ViewBag.headTurnImg = selectTurnImg();
            return View();
        }
        [HttpGet]
        public ActionResult HeadTurnAdd()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult HeadTurnAdd(IEnumerable<HttpPostedFileBase> files)
        {
            bool error = false;
            int count = 0;
            string message = "";
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        //驗證檔案類型
                        if (file.ContentType.Substring(0, 5) == "image")
                        {
                            managerModel mm = new managerModel();
                            if (!Directory.Exists(Server.MapPath("~/img/turn")))
                                Directory.CreateDirectory(Server.MapPath("~/img/turn"));
                            string path = Path.Combine(Server.MapPath("~/img/turn"), fileName),
                                sqlstrselect = @"SELECT count FROM localsql.optionheadturnimg",
                            sqlstr = @"INSERT INTO optionheadturnimg(
                                headPageTurnImg,count
                            )VALUES(
                               @headPageTurnImg,@count)";
                            count = selectCount(count, sqlstrselect);
                            MySqlConnection con = new MySqlConnection(sqlconStr);
                            MySqlCommand link;
                            con.Open();
                            link = con.CreateCommand();
                            link.CommandText = sqlstr;

                            link.Parameters.Clear();
                            link.Parameters.AddWithValue("headPageTurnImg", fileName);
                            link.Parameters.AddWithValue("count", count + 1);
                            link.ExecuteNonQuery();
                            con.Close();
                            file.SaveAs(path + "0");
                            mm.imgfunction(path + "0", path, 800);
                            //圖片等比縮小 以高維600的比例
                        }
                        else
                        {
                            error = true;
                            message = "0x001"; //タイプが違いあり
                        }
                    }
                }
                if (error == false)
                {
                    message = "0";

                }
            }
            return RedirectToAction("HeadTurnImg");
        }
        [HttpGet]
        public ActionResult HeadTurnDel()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadHeadTurnImgList());
        }
        [HttpGet]
        public ActionResult HeadTurnDelFx(int del, string dx)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM optionheadturnimg
                            WHERE count=@count";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("count", del);
            link.ExecuteNonQuery();
            con.Close();
            if (Directory.Exists(Server.MapPath("/img/turn")))
            {
                managerModel mm = new managerModel();
                mm.DeleteFile(Server.MapPath("/img/turn"), dx);
            }
            return RedirectToAction("HeadTurnDel");
        }

        [HttpGet]
        public ActionResult OptionAddress()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadOptionAddressList());
        }
        [HttpGet]
        public ActionResult OptionAddressCreate()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OptionAddressCreate(managerModel mm)
        {
            if (mm.OptionAddAddressName_p != null && mm.OptionAddAddressName_p != "" &&
                mm.OptionAddAddress_p != null && mm.OptionAddAddress_p != "" &&
                mm.OptinoAddPhone_p != null && mm.OptinoAddPhone_p != ""
                )
            {

                if (mm.OptionAddTrafficInfo_p == null)
                    mm.OptionAddTrafficInfo_p = "";
                int count = 0;
                count = selectCount(count, "SELECT count FROM localsql.optionaddress");
                string sqlstr = @"INSERT INTO localsql.optionaddress(
                                TrafficInfo,phone,address,AddressName,Img,count
                            )VALUES(
                               @TrafficInfo,@phone,@address,@AddressName,@Img,@count)";
                if (imgstr != "")
                {
                    mm.OptionAddImg_p = imgstr;
                    mm.movefile(Server.MapPath("/Files/"), Server.MapPath("/img/"), imgstr);
                    imgstr = "";
                }
                MySqlConnection con = new MySqlConnection(sqlconStr);
                MySqlCommand link;
                con.Open();
                link = con.CreateCommand();
                link.CommandText = sqlstr;
                link.Parameters.Clear();

                link.Parameters.AddWithValue("TrafficInfo", mm.OptionAddTrafficInfo_p.ToString());
                link.Parameters.AddWithValue("phone", mm.OptinoAddPhone_p.ToString());
                link.Parameters.AddWithValue("address", mm.OptionAddAddress_p.ToString());
                link.Parameters.AddWithValue("AddressName", mm.OptionAddAddressName_p.ToString());
                link.Parameters.AddWithValue("Img", mm.OptionAddImg_p.ToString());
                link.Parameters.AddWithValue("count", count + 1);
                link.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("OptionAddress");
            }
            else
            {
                ViewBag.msg = "0";
                return View();
            }
        }
        [HttpGet]
        public ActionResult OptionAddressMapUp()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult OptionAddressMapUp(IEnumerable<HttpPostedFileBase> files)
        {
            bool error = false;
            string fileName = "";
            int count = 0;
            string message = "";
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(file.FileName);
                        //驗證檔案類型
                        if (file.ContentType.Substring(0, 5) == "image")
                        {

                            if (!Directory.Exists(Server.MapPath("~/Files/")))
                                Directory.CreateDirectory(Server.MapPath("~/Files"));
                            string path = Path.Combine(Server.MapPath("~/Files"), fileName);
                            file.SaveAs(path);
                            imgstr = fileName;
                        }
                        else
                        {
                            error = true;
                            message = "0x001"; //タイプが違いあり
                        }
                    }
                }
                if (error == false)
                {
                    message = "0";
                    TempData["fs"] = fileName;
                }
            }
            return RedirectToAction("OptionAddressMapUp");
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult OptionAddressChange(int x, string na)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"SELECT * FROM localsql.optionaddress WHERE( count=" + x.ToString() + ")";
            managerModel mm = new managerModel();
            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {

                mm.OptionAddAddressName_p = read["AddressName"].ToString();
                mm.OptionAddAddress_p = read["address"].ToString();
                mm.OptinoAddPhone_p = read["phone"].ToString();
                mm.OptionAddTrafficInfo_p = read["TrafficInfo"].ToString();
            }
            mm.OptionAddCount_p = x;
            modoimg = na;
            TempData["filesmodo"] = na;
            con.Close();
            return View(mm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OptionAddressChange(managerModel mm)
        {
            int count = 0;
            count = mm.OptionAddCount_p;
            if (imgstr != "")
            {
                mm.OptionAddImg_p = imgstr;
                mm.movefile(Server.MapPath("/Files/"), Server.MapPath("/img/"), imgstr);
                imgstr = "";
            }
            else
            {
                mm.OptionAddImg_p = modoimg;
                modoimg = "";
            }
            string sqlstr = @"UPDATE localsql.optionaddress SET AddressName=@AddressName, address=@address,
                        phone=@phone,TrafficInfo=@TrafficInfo,Img=@Img
                                WHERE count=@count";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("AddressName", mm.OptionAddAddressName_p);
            link.Parameters.AddWithValue("address", mm.OptionAddAddress_p);
            link.Parameters.AddWithValue("phone", mm.OptinoAddPhone_p);
            link.Parameters.AddWithValue("TrafficInfo", mm.OptionAddTrafficInfo_p);
            link.Parameters.AddWithValue("count", count);
            link.Parameters.AddWithValue("Img", mm.OptionAddImg_p);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("OptionAddress");
        }
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult OptionAddressDel(int x, string na)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM optionaddress
                            WHERE count=@count";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("count", x);
            link.ExecuteNonQuery();
            con.Close();
            if (Directory.Exists(Server.MapPath("/img")))
            {
                managerModel mm = new managerModel();
                mm.DeleteFile(Server.MapPath("/img"), na);
            }
            return RedirectToAction("OptionAddress");
        }

        [HttpGet]
        public ActionResult OptionAbout()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"SELECT * FROM localsql.about WHERE aboutCount=0";
            managerModel mm = new managerModel();
            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {

                ViewBag.about = read["about"].ToString();
                ViewBag.about_img = read["about_img"].ToString();
            }
            con.Close();
            return View();
        }
        [HttpGet]
        public ActionResult OptionAboutChange()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"SELECT about,about_img FROM localsql.about WHERE( aboutCount=0)";
            managerModel mm = new managerModel();
            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {

                mm.about_p = read["about"].ToString();
                modoimg = read["about_img"].ToString();
            }
            con.Close();
            return View(mm);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OptionAboutChange(managerModel mm)
        {
            if (imgstr != "")
            {
                mm.OptionAddImg_p = imgstr;
                mm.movefile(Server.MapPath("/Files/"), Server.MapPath("/img/"), imgstr);
                imgstr = "";
                modoimg = "";
            }
            else
            {
                mm.OptionAddImg_p = modoimg;
                modoimg = "";
            }
            string sqlstr = @"UPDATE localsql.about SET about=@about,about_img=@about_img
                                WHERE aboutCount=0";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("about", mm.about_p);
            link.Parameters.AddWithValue("about_img", mm.OptionAddImg_p);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("OptionAbout");
        }

        [HttpGet]
        public ActionResult OptionServiec()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"SELECT service FROM localsql.service WHERE count=0";

            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {
                ViewBag.service = read["service"].ToString();
            }
            con.Close();
            return View();
        }
        [HttpGet]
        public ActionResult OptionServiecChange()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"SELECT service FROM localsql.service WHERE count=0";
            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            managerModel mm = new managerModel();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {

                mm.service_p = read["service"].ToString();
            }
            con.Close();
            return View(mm);
        }
        [HttpPost]
        public ActionResult OptionServiecChange(managerModel mm)
        {
            string sqlstr = @"UPDATE localsql.service SET service=@service
                                WHERE count=0";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("service", mm.service_p);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("OptionServiec");
        }

        [HttpGet]
        public ActionResult OptionLink()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View(ReadOptionLinkList());
        }
        [HttpGet]
        public ActionResult OptionLinkAdd()
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OptionLinkAdd(managerModel mm)
        {
            if (mm.OptionLinkName_p != "" && mm.OptionLinkName_p != null)
            {
                int count = 0;
                count = selectCount(count, "SELECT count FROM localsql.link");
                string sqlstr = @"INSERT INTO localsql.link(
                                Name,linkweb,Img,count
                            )VALUES(
                               @Name,@linkweb,@Img,@count)";
                if (imgstr != "")
                {
                    mm.OptionLinkImg_p = imgstr;
                    mm.movefile(Server.MapPath("/Files/"), Server.MapPath("/img/link/"), imgstr);
                    imgstr = "";
                }
                MySqlConnection con = new MySqlConnection(sqlconStr);
                MySqlCommand link;
                con.Open();
                link = con.CreateCommand();
                link.CommandText = sqlstr;
                link.Parameters.Clear();

                link.Parameters.AddWithValue("Name", mm.OptionLinkName_p.ToString());
                link.Parameters.AddWithValue("linkweb", mm.OptionLinkweb_p.ToString());
                link.Parameters.AddWithValue("Img", mm.OptionLinkImg_p.ToString());
                link.Parameters.AddWithValue("count", count + 1);
                link.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("OptionLink");
            }
            else
            {
                ViewBag.msg = "0";
                return RedirectToAction("OptionLink");
            }
        }
        [HttpGet]
        public ActionResult OptionLinkChange(int x, string na)
        {

            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            selectedcount = x;
            modoimg = na;
            string sqlstr = @"SELECT Img,Name,linkweb FROM localsql.link WHERE( count=" + selectedcount + ")";
            managerModel mm = new managerModel();
            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {

                mm.OptionLinkName_p = read["Name"].ToString();
                mm.OptionLinkweb_p = read["linkweb"].ToString();
                modoimg = read["Img"].ToString();
            }
            TempData["linkmodo"] = modoimg;
            con.Close();
            return View(mm);
        }
        [HttpPost]
        public ActionResult OptionLinkChange(managerModel mm)
        {
            string sqlstr = @"UPDATE localsql.link SET Img=@Img,Name=@Name,linkweb=@lw
                                WHERE count=@count";
            if (imgstr == "")
                mm.OptionLinkImg_p = modoimg;
            else
            {
                mm.OptionLinkImg_p = imgstr;
                mm.movefile(Server.MapPath("/Files/"), Server.MapPath("/img/link/"), imgstr);
            }
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("Img", mm.OptionLinkImg_p);
            link.Parameters.AddWithValue("Name", mm.OptionLinkName_p);
            link.Parameters.AddWithValue("lw", mm.OptionLinkweb_p);
            link.Parameters.AddWithValue("count", selectedcount);
            link.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("OptionLink");
        }
        [HttpGet]
        public ActionResult OptionLinkDel(int x, string na)
        {
            if (!AccountSession())
            {
                return RedirectToAction("Login");
            }
            string sqlstr = @"DELETE FROM link
                            WHERE count=@count";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            link.Parameters.Clear();
            link.Parameters.AddWithValue("count", x);
            link.ExecuteNonQuery();
            con.Close();
            if (Directory.Exists(Server.MapPath("/img/link")))
            {
                managerModel mm = new managerModel();
                mm.DeleteFile(Server.MapPath("/img/link"), na);
            }
            return RedirectToAction("OptionLink");
        }


        private void MoveFiles(string StartPath, string EndPath) //ファイルが多い時 listで伝送する
        {
            if (Directory.Exists(Server.MapPath(StartPath)))
            {
                managerModel mm = new managerModel();
                mm.movefile(Server.MapPath(StartPath), Server.MapPath(EndPath), list);
            }
        }
        private void MoveFiles(string StartPath, string EndPath, string FileName)
        {
            if (Directory.Exists(Server.MapPath(StartPath)))
            {
                managerModel mm = new managerModel();
                mm.movefile(Server.MapPath(StartPath), Server.MapPath(EndPath), FileName);
            }
        }

        public List<managerModel> ReadNoticeList()
        {
            string sqlstr = @"SELECT * FROM localsql.notice  ORDER BY date DESC";

            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {
                    NoticeType_p = read["type"].ToString(),
                    NoticeContent_p = read["Content"].ToString(),
                    date_p = read["date"].ToString(),
                    noticecol_p = read["noticecol"].ToString(),
                    NoticeType_eventtype_p = read["type_eventtype"].ToString(),
                    NoticeTitle_p = read["title"].ToString()
                });
            }
            con.Close();
            return List;
        }
        public List<managerModel> ReadAlbumList()
        {
            string sqlstr = @"SELECT * FROM localsql.album,localsql.album_address WHERE(album.AlbumCount=album_address.album_AlbumCount)  ORDER BY date DESC";
            int count = 0;//count ->同じテーマだがインメージ違う数,temp=>loop向かいArrayの数
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {
                    AlbumTitle_p = read["title"].ToString(),
                    AlbumContent_p = read["Content"].ToString(),
                    date_p = read["date"].ToString(),
                    AlbumImgadd_p = "/album/" + read["AlbumCount"].ToString() + "/" + read["ImgAddress"].ToString(),
                    OptionAddCount_p = int.Parse(read["albumCount"].ToString())
                });
            }
            for (int i = 0; i < List.Count;) //Webgrid只抓string當欄位 相同的就合併 用'!'區隔
            {
                if (i > 0)
                {
                    if (List[i].AlbumTitle_p == List[i - 1].AlbumTitle_p &&
                        List[i].AlbumContent_p == List[i - 1].AlbumContent_p)
                    {
                        List[i - 1].AlbumImgadd_p += "!" + List[i].AlbumImgadd_p;
                        List.RemoveAt(i);
                    }
                    else
                    {
                        count = 1;
                        i++;
                    }
                }
                else
                {
                    count = 1;
                    i++;
                }
            }
            con.Close();
            return List;
        }
        public List<managerModel> ReadTeachList()
        {
            string sqlstr = @"SELECT * FROM localsql.Teach  ORDER BY date DESC";

            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {

                    TeachContent_p = read["Content"].ToString(),
                    date_p = read["date"].ToString(),
                    TeachTitle_p = read["title"].ToString()
                });
            }
            con.Close();
            return List;
        }
        public List<managerModel> ReadHeadTurnImgList()
        {
            string sqlstr = @"SELECT headPageTurnImg,count FROM localsql.optionheadturnimg";

            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {
                    HeadTurnFileCount_p = int.Parse(read["count"].ToString()),
                    HeadTurnFilestr_p = read["headPageTurnImg"].ToString()
                });
            }
            con.Close();
            return List;
        }
        public List<managerModel> ReadOptionAddressList()
        {
            string sqlstr = @"SELECT * FROM localsql.optionaddress";

            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {
                    OptionAddTrafficInfo_p = read["TrafficInfo"].ToString(),
                    OptinoAddPhone_p = read["phone"].ToString(),
                    OptionAddAddress_p = read["address"].ToString(),
                    OptionAddAddressName_p = read["AddressName"].ToString(),
                    OptionAddImg_p = read["Img"].ToString(),
                    OptionAddCount_p = int.Parse(read["count"].ToString())
                });
            }
            con.Close();
            return List;
        }
        public List<managerModel> ReadOptionLinkList()
        {
            string sqlstr = @"SELECT * FROM localsql.link";

            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {
                    OptionLinkName_p = read["Name"].ToString(),
                    OptionLinkImg_p = read["Img"].ToString(),
                    OptionLinkweb_p = read["linkweb"].ToString(),
                    OptionAddCount_p = int.Parse(read["count"].ToString())
                });
            }
            con.Close();
            return List;
        }
        public List<managerModel> ReadFBList()
        {
            string sqlstr = @"SELECT * FROM localsql.feedback";

            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            List<managerModel> List = new List<managerModel>();
            while (read.Read())
            {
                List.Add(new managerModel
                {
                    FBName_p = read["Name"].ToString(),
                    FBphone_p = read["phone"].ToString(),
                    FBcontent_p = read["content"].ToString(),
                    OptionAddCount_p = int.Parse(read["idfeedback"].ToString()),
                    FBmail_p = read["mail"].ToString()
                });
            }
            con.Close();
            return List;
        }


       
        private int selectCount(int count, string sql)
        {
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sql;
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                count = int.Parse(read[0].ToString());
            }
            con.Clone();
            return count;
        }
        private string[] selectTurnImg()
        {
            string sqlstr = @"SELECT * FROM localsql.optionheadturnimg";
            string[] imgpath = new string[] { "" };
            int i = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            i = 0;
            while (read.Read())
            {
                imgpath[i] = read["headPageTurnImg"].ToString();
                i++;
                Array.Resize(ref imgpath, imgpath.Length + 1);
            }
            con.Close();
            return imgpath;
        }
        private string[][] selectsql(string sqlstr, string[] selectColumn, int CountStart, int CountEnd) //Count=-1 全拉
        {
            string[][] imgpath = new string[][]
            {
                new string[] { ""}
            };
            int count = 0, rows = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            count = 0;
            Array.Resize(ref imgpath[0], selectColumn.Length);
            while (read.Read())
            {
                if (count >= CountStart)
                {
                    for (int row = 0; row < selectColumn.Length; row++)
                    {
                        imgpath[rows][row] = read[selectColumn[row]].ToString();
                    }
                    rows++;
                }
                count++;
                if (count == CountEnd)
                    break;
                if (count >= CountStart && rows > 0)
                {
                    Array.Resize(ref imgpath, imgpath.Length + 1);
                    Array.Resize(ref imgpath[rows], selectColumn.Length);
                }
            }
            if (CountEnd == -1 || (imgpath.Length < CountEnd - CountStart))
            {
                Array.Resize(ref imgpath, imgpath.Length - 1);
            }
            con.Close();
            return imgpath;
        }
    }
 }