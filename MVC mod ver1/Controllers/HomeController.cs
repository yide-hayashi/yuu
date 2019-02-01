using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Web.Configuration;
using WebApplication1.Models;
using GoogleRecaptcha;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        string sqlconStr = WebConfigurationManager.ConnectionStrings[2].ToString();
        public ActionResult Index()
        {
            string[] imgpath = { "" };
            headdisplay();
            managerModel mm = new managerModel();
            ViewBag.noticeTitle = selectsql("SELECT * FROM localsql.notice order by date desc", new[] { "type", "title" }, 0, 5);
            ViewBag.link = selectsql("SELECT * FROM localsql.link ", new[] { "Img", "linkweb" }, 0, 5);

            int count = 0, rows = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = "SELECT * FROM localsql.album_address";
            MySqlDataReader read = link.ExecuteReader();
            count = 0;
            Array.Resize(ref imgpath, imgpath.Length);
            while (read.Read())
            {
                if (count != int.Parse(read["album_AlbumCount"].ToString()))
                {
                    imgpath[count] = (read["album_AlbumCount"]).ToString() + "/" + read["ImgAddress"].ToString();
                    count++;
                    Array.Resize(ref imgpath, imgpath.Length + 1);
                }
                if (count > 4)
                    break;
            }
            Array.Resize(ref imgpath, imgpath.Length - 1);
            con.Close();
            ViewBag.albumImg = imgpath;

            ViewBag.headTurnImg = selectTurnImg();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.text = selectsql("SELECT * FROM localsql.about ", new[] { "about", "about_img" }, 0, -1);
            headdisplay();
            return View();
        }

        public ActionResult NoticeContent(string name)
        {
            headdisplay();
            ViewBag.text = selectsql("SELECT * FROM localsql.notice WHERE title='" + name + "' ORDER BY date DESC",
                new[] { "title", "type", "content", "date" }, 0, -1);
            return View();
        }

        public ActionResult Address()
        {
            ViewBag.text = selectsql("SELECT * FROM localsql.optionaddress ", new[] { "AddressName", "Img", "address", "phone", "TrafficInfo" }, 0, -1);
            headdisplay();
            return View();
        }

        public ActionResult Service()
        {
            ViewBag.text = selectsql("SELECT * FROM localsql.service ", new[] { "service" }, 0, 1);
            headdisplay();
            return View();
        }

        public ActionResult Normal(int page)
        {
            int RowsCount = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            string[][] imgpath = new string[][]
             {
                new string[] { ""}, new string[] { ""}, new string[] { ""}
             };
            con.Open();
            link = con.CreateCommand();
            link.CommandText = "SELECT count(*) AS count FROM localsql.notice WHERE type='志工' AND type_eventtype='專案' ORDER BY date DESC";
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                RowsCount = int.Parse(read["count"].ToString());
            }
            con.Close();
            ViewBag.displaypage = page;
            ViewBag.pagecount = (RowsCount / 5) + 1;
            imgpath = selectsql("SELECT * FROM localsql.notice WHERE type='志工' AND type_eventtype='專案' ORDER BY date DESC",
                new[] { "title", "content", "date" }, page * 5 - 5, page * 5);
            imgpath[0][1] = DelHtmlTag(imgpath[0][1]);
            ViewBag.text = imgpath;
            headdisplay();
            return View();
        }

        public ActionResult Project(int page)
        {
            int RowsCount = 0;
            string[][] imgpath = new string[][]
             {
                new string[] { ""}, new string[] { ""}, new string[] { ""}
             };
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = "SELECT count(*) AS count FROM localsql.notice WHERE type='活動' AND type_eventtype='專案' ORDER BY date DESC";
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                RowsCount = int.Parse(read["count"].ToString());
            }
            con.Close();
            ViewBag.displaypage = page;
            ViewBag.pagecount = (RowsCount / 5) + 1;


            imgpath = selectsql("SELECT * FROM localsql.notice WHERE type='活動' AND type_eventtype='專案' ORDER BY date DESC",
               new[] { "title", "content", "date" }, page * 5 - 5, page * 5);
            imgpath[0][1] = DelHtmlTag(imgpath[0][1]);
            ViewBag.text = imgpath;
            headdisplay();
            return View();
        }

        public ActionResult Prefecture(int page)
        {
            ViewBag.noticeTitle = selectsql("SELECT * FROM localsql.notice order by date desc", new[] { "type", "title" }, 0, 20);
            int RowsCount = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = "SELECT count(*) AS count FROM localsql.notice  ORDER BY date DESC";
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                link.CommandText = "SELECT count(*) AS count FROM localsql.notice  ORDER BY date DESC";
                RowsCount = int.Parse(read["count"].ToString());
            }
            con.Close();
            ViewBag.displaypage = page;
            ViewBag.pagecount = (RowsCount / 20) + 1;
            headdisplay();
            return View();
        }

        public ActionResult EventAlbum(int page)
        {
            headdisplay();
            List<string[]> list = new List<string[]>();
            string[][] album = selectsql("SELECT * FROM localsql.album",
                new[] { "title", "content", "AlbumCount", "date" }, page * 5 - 5, page * 5);
            for (int i = 0; i < album.Length; i++)
            {
                list.Add(selectImg("SELECT * FROM localsql.album_address WHERE album_AlbumCount='" + album[i][2] + "'", "ImgAddress"));
            }
            ViewBag.Imgs = list;
            ViewBag.albumImg = album;
            return View();
        }

        [HttpGet]
        public ActionResult feedback()
        {
            headdisplay();
            return View();
        }
        [HttpPost]
        public ActionResult feedback(managerModel mm, FormCollection form)
        {

            headdisplay();
            IRecaptcha<RecaptchaV2Result> recaptcha = new RecaptchaV2(new RecaptchaV2Data()
            {
                Secret = "6LfBriQTAAAAANGF9w6CrSl_8yksdNy9dNi7Xp9R"
            });

            // Verify the captcha
            var result = recaptcha.Verify();
            if (mm.FBName_p != "" && mm.FBcontent_p != "" && mm.FBmail_p != null &&
                mm.FBName_p != null && mm.FBcontent_p != null && result.Success == true)
            {
                int count = 0;
                try
                {
                    string sqlstr = @"INSERT INTO localsql.feedback(
                                    idfeedback,Name,content,phone,mail
                                )VALUES(
                                   @idfeedback,@Name,@content,@phone,@mail)",
                                        sqlstrselect = @"SELECT idfeedback FROM localsql.feedback";
                    count = selectCount(count, sqlstrselect);
                    MySqlConnection con = new MySqlConnection(sqlconStr);
                    MySqlCommand link;
                    con.Open();
                    link = con.CreateCommand();
                    link.CommandText = sqlstr;

                    link.Parameters.Clear();
                    link.Parameters.AddWithValue("idfeedback", count + 1);
                    link.Parameters.AddWithValue("Name", mm.FBName_p);
                    link.Parameters.AddWithValue("content", mm.FBcontent_p);
                    link.Parameters.AddWithValue("phone", mm.FBphone_p);
                    link.Parameters.AddWithValue("mail", mm.FBmail_p);
                    link.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex) { }
                TempData["message"] = "送出成功";
                return RedirectToAction("index");
            }
            else
            {
                ViewBag.msg = "0";
                return View();
            }


        }

        public ActionResult Teach(int page)
        {
            ViewBag.teach = selectsql("SELECT * FROM localsql.teach  ORDER BY date DESC", new[] { "title", "date" }, 0, 20);

            int RowsCount = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = "SELECT count(*) AS count FROM localsql.teach ORDER BY date DESC";
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                RowsCount = int.Parse(read["count"].ToString());
            }
            con.Close();
            ViewBag.displaypage = page;
            ViewBag.pagecount = (RowsCount / 20) + 1;
            headdisplay();

            return View();
        }
        public ActionResult TeachContent(string name)
        {
            headdisplay();
            ViewBag.text = selectsql("SELECT * FROM localsql.teach WHERE title='" + name + "'",
                new[] { "title", "content", "date" }, 0, -1);
            return View();
        }

        public ActionResult Link()
        {
            headdisplay();
            ViewBag.link = selectsql("SELECT * From localsql.link", new[] { "Img", "Name", "linkweb" }, 0, -1);
            return View();
        }


        private void headdisplay()
        {
            string sqlstr = @"SELECT * FROM localsql.HeadImg";
            string img = "";
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {
                img = read["HeadImg"].ToString();
            }
            con.Close();
            ViewBag.img = img;
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
            if (count != CountEnd)
            {
                if (imgpath[count][0] == null)
                {
                    Array.Resize(ref imgpath, imgpath.Length - 1);
                }
            }
            con.Close();
            return imgpath;
        }

        private string[] selectImg(string sqlstr, string selectColumn)
        {
            string[] imgpath = new string[]
            {"" };
            int count = 0;
            MySqlConnection con = new MySqlConnection(sqlconStr);
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            count = 0;
            while (read.Read())
            {
                imgpath[count] = read[selectColumn].ToString();
                count++;
                Array.Resize(ref imgpath, imgpath.Length + 1);
            }
            Array.Resize(ref imgpath, imgpath.Length - 1);
            con.Close();
            return imgpath;
        }
        private string DelHtmlTag(string html)
        {
            int i = 0;
            bool flag = false; //判斷旗標
            string context = "";
            for (i = 0; i < html.Length;)
            {
                if (html[i] == '<' && !flag)
                {
                    flag = true;
                }
                else if (html[i] == '>' && flag)
                {
                    html = html.Substring(i + 1, html.Length - (i + 1));
                    i = 0;
                    flag = false;
                }
                if (!flag && html[i] != '<')
                {
                    context += html.Substring(0, 1);
                    html = html.Substring(i + 1, html.Length - (i + 1));
                    i = 0;
                }
                else if (flag)
                    i++;
            }
            return context.Replace("&nbsp;", " ").Replace("\r", "").Replace("\n", "").Replace("\t", "");
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
    }
}