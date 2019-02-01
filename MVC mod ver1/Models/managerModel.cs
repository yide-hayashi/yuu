using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Web.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WebApplication1.Models
{
    public class managerModel
    {
        #region private
        private string NoticeType="", NoticeType_eventtype = "",
            NoticeTitle = "",NoticeContent = "", noticecol = "";

        private string AlbumTitle = "", AlbumContent = "", AlbumImgadd = "";
        private List<string> list;

        private string TeachTitle = "";
        private string TeachContent = "";

        private string HeadTurnFilestr = "";
        private int HeadTurnFileCount = 0;

        private string OptionAddTrafficInfo = "", OptinoAddPhone = "", OptionAddAddress = "",
            OptionAddAddressName = "", OptionAddImg = "";

        private int OptionAddCount = 0;
        private string account="", pw="";

        private string FBName="", FBcontent="", FBphone="", FBmail="";
         
        private string about="", service="";

        private string OptionLinkImg = "", OptionLinkName="", OptionLinkweb="";

        #region commmon
        private string date = "";

        #endregion



        #endregion

        #region public

        #region Notice
        public string NoticeTitle_p
        {
            get { return this.NoticeTitle; }
            set { this.NoticeTitle = value; }
        }
        public string NoticeType_p
        {
            get { return this.NoticeType; }
            set { this.NoticeType = value; }
        }
        public string NoticeType_eventtype_p
        {
            get { return this.NoticeType_eventtype; }
            set { this.NoticeType_eventtype = value; }
        }
        public string date_p
        {
            get { return this.date; }
            set { this.date = value; }
        }
        public string NoticeContent_p
        {
            get { return this.NoticeContent; }
            set { this.NoticeContent = value; }
        }
        public string noticecol_p
        {
            get { return this.noticecol; }
            set { this.noticecol = value; }
        }
        #endregion

        #region Album
        public string AlbumTitle_p
        {
            get { return this.AlbumTitle; }
            set { this.AlbumTitle = value; }
        }
        public string AlbumContent_p
        {
            get { return this.AlbumContent; }
            set { this.AlbumContent = value; }
        }
        public string AlbumImgadd_p
        {
            get { return this.AlbumImgadd; }
            set { this.AlbumImgadd = value; }
        }
        public List<string> list_p
        {
            get { return list; }
            set { list = value; }
        }
        #endregion

        #region Teach
        public string TeachTitle_p
        {
            get { return TeachTitle ; }
            set { TeachTitle = value; }
        }
        public string TeachContent_p
        {
            get { return TeachContent; }
            set { TeachContent = value; }
        }
        #endregion

        #region HeadTurn
        public string HeadTurnFilestr_p
        {
            set { HeadTurnFilestr = value; }
            get { return HeadTurnFilestr; }
        }
        public int HeadTurnFileCount_p
        {
            set { HeadTurnFileCount = value; }
            get { return HeadTurnFileCount; }
        }
        public int CountLine = 0;
        #endregion

        #region OptionAddresss
        public string OptionAddTrafficInfo_p
        {
            get { return OptionAddTrafficInfo; }
            set { OptionAddTrafficInfo = value; }
        }
        public string OptinoAddPhone_p
        {
            get { return OptinoAddPhone; }
            set { OptinoAddPhone = value; }
        }

        public string OptionAddAddress_p
        {
            get { return OptionAddAddress; }
            set { OptionAddAddress = value; }
        }
         public string OptionAddAddressName_p
        {
            get { return OptionAddAddressName; }
            set { OptionAddAddressName = value; }
        }
        public string OptionAddImg_p 
            {
            get { return OptionAddImg ; }
            set { OptionAddImg  = value; }
        }
        public int OptionAddCount_p
        {
            get { return OptionAddCount; }
            set { OptionAddCount = value; }
        }
        #endregion

        #region about
        public string about_p
        {
            set  { about = value; }
            get { return about; }
        }
        #endregion

        #region service
        public string service_p
        {
            set { service = value; }
            get { return service; }
        }
        #endregion

        #region link
            public string OptionLinkImg_p
        {
            set { OptionLinkImg = value; }
            get { return OptionLinkImg; }
        }
        public string OptionLinkName_p
        {
            set { OptionLinkName = value; }
            get { return OptionLinkName; }
        }
        public string OptionLinkweb_p
        {
            set { OptionLinkweb = value; }
            get { return OptionLinkweb; }
        }
        #endregion
        
        public string FBName_p
        {
            set { FBName = value; }
            get { return FBName; }
        }
        public string FBcontent_p
        {
            set { FBcontent = value; }
            get { return FBcontent; }
        }
        public string FBphone_p
        {
            set { FBphone = value; }
            get { return FBphone; }
        }
        public string FBmail_p
        {
            set { FBmail = value; }
            get { return FBmail; }
        }
        public string account_p
        {
            set { account = value; }
            get { return account; }
        }
        public string pw_p
        {
            set { pw = value; }
            get { return pw; }
        }
        #endregion

        #region function
        public void DeleteFile(string path,string FileName)
        {
            if(File.Exists(path+"\\"+FileName))
            {
                File.Delete(path + "\\" + FileName);
            }
        }
            
        public void NoticeCreate()
        {
            string sqlstr = @"INSERT INTO notice(
                                title,type,type_eventtype,Content,date
                            )VALUES(
                               @title,@type,@type_eventtype,@Content ,@date)";
            MySqlConnection con = new MySqlConnection(WebConfigurationManager.ConnectionStrings[2].ToString());
            MySqlCommand link;
                con.Open();
                link = con.CreateCommand();
                link.CommandText = sqlstr;
                link.Parameters.Clear();
                link.Parameters.AddWithValue("title", NoticeTitle);
                link.Parameters.AddWithValue("type", NoticeType);
                link.Parameters.AddWithValue("type_eventtype", NoticeType_eventtype==null ? "": NoticeType_eventtype);
                link.Parameters.AddWithValue("Content", NoticeContent);
                link.Parameters.AddWithValue("date", DateTime.Now.ToShortDateString() +" "+ DateTime.Now.ToLongTimeString());
            link.Parameters.AddWithValue("noticecol", " ");
            link.ExecuteNonQuery();
            con.Close();
        }
        public void AlbumCreate()
        {
            selectLineCount();
            string sqlstr = @"INSERT INTO album(
                                title,Content,AlbumCount,date
                            )VALUES(
                               @title,@Content,@count,@date)";
            string sqlstr_add = @"INSERT INTO album_address(
                                album_AlbumCount,ImgAddress
                            )VALUES(
                               @album_AlbumCount,@ImgAddress)";
            MySqlConnection con = new MySqlConnection(WebConfigurationManager.ConnectionStrings[2].ToString());
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;

            link.Parameters.AddWithValue("title",AlbumTitle);
            link.Parameters.AddWithValue("Content", AlbumContent);
            link.Parameters.AddWithValue("count", CountLine);
            link.Parameters.AddWithValue("date", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());

            link.ExecuteNonQuery();
   
            foreach (var i in list)
            {
                link.Parameters.Clear();
                link.CommandText = sqlstr_add;
                link.Parameters.AddWithValue("album_AlbumCount", CountLine);
                link.Parameters.AddWithValue("ImgAddress", i);
                link.ExecuteNonQuery();
            }
            con.Close();
        }
        public void TeachCreate()
        {
            string sqlstr = @"INSERT INTO teach(
                                title,Content,date
                            )VALUES(
                               @title,@Content,@date)";
            MySqlConnection con = new MySqlConnection(WebConfigurationManager.ConnectionStrings[2].ToString());
            MySqlCommand link;
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            for (int i = 0; i < 1; i++)/*testm/4!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
            {
                link.Parameters.Clear();

                link.Parameters.AddWithValue("title", TeachTitle);
                link.Parameters.AddWithValue("Content", TeachContent);
                link.Parameters.AddWithValue("date", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());

                link.ExecuteNonQuery();
            }
            con.Close();
        }


        public void DelDir(string dir)
        {
            if(Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        public void movefile(string StartPath, string EndPath,string fileName)
        {
                if (File.Exists(StartPath + "\\" + fileName.ToString()))
                {
                    if (!Directory.Exists(EndPath))
                        Directory.CreateDirectory(EndPath);
                if (File.Exists( EndPath + "\\" + fileName.ToString()))
                {
                    File.Delete(EndPath + "\\" + fileName.ToString());
                }
                File.Move(StartPath + "\\" + fileName.ToString(), EndPath + "\\" + fileName.ToString());
                }
            string[] Tempimgs = Directory.GetFiles(StartPath);
            foreach(string Tempimg in Tempimgs)
            {
                if((DateTime.Now - File.GetCreationTime(Tempimg)).TotalHours>=1)
                {
                    File.Delete(Tempimg);
                }
            }
        }
        public void movefile( string StartPath, string EndPath,List<string> list)
        {
            foreach(var i in list)
            if(File.Exists(StartPath+"\\"+i.ToString()))
            {
                    if (!Directory.Exists(EndPath))
                        Directory.CreateDirectory(EndPath);
                    if (File.Exists(EndPath + "\\" + i.ToString()))
                    {
                        File.Delete(EndPath + "\\" + i.ToString());
                    }
                    File.Move(StartPath+"\\" + i.ToString(), EndPath+"\\" + i.ToString());
            }
        }
        private void selectLineCount()
        {
            string sqlstr = @"SELECT * FROM localsql.album";
            CountLine = 0;
            MySqlConnection con = new MySqlConnection(WebConfigurationManager.ConnectionStrings[2].ToString());
            MySqlCommand link;
            DataTable dt = new DataTable();
            con.Open();
            link = con.CreateCommand();
            link.CommandText = sqlstr;
            MySqlDataReader read = link.ExecuteReader();
            while (read.Read())
            {

                CountLine = int.Parse(read["AlbumCount"].ToString());
            }
                CountLine++;
            con.Close();
        }
        private void SQLlink()
        {
            MySqlConnection con = new MySqlConnection(WebConfigurationManager.ConnectionStrings[2].ToString());
            MySqlCommand link;
            try
            {
                con.Open();
                link = con.CreateCommand();
                //link.CommandText = "";
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 產生縮圖並儲存 寬度維持500pix，高度等比例
        /// </summary>
        /// <param name="srcImagePath">來源圖片的路徑</param>
        /// <param name="saveThumbFilePath">縮圖的儲存檔案路徑</param>
        public void imgfunction(string srcImagePath, string saveThumbFilePath,int maxPx)
        {

            System.Drawing.Image image = System.Drawing.Image.FromFile(srcImagePath);
            //必須使用絕對路徑
            ImageFormat thisFormat = image.RawFormat;
            //取得影像的格式
            int fixWidth = 0;
            int fixHeight = 0;
            //第一種縮圖用
            //int maxPx = Convert.ToInt16(800);  宣告一個最大值
            if (image.Width > maxPx || image.Height > maxPx)
            //如果圖片的寬大於最大值或高大於最大值就往下執行
            {
                if (image.Width >= image.Height)
                //圖片的寬大於圖片的高
                {
                    fixWidth = maxPx;
                    //設定修改後的圖寬
                    fixHeight = Convert.ToInt32((Convert.ToDouble(fixWidth) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                    //設定修改後的圖高
                }
                else
                {
                    fixHeight = maxPx;
                    //設定修改後的圖高
                    fixWidth = Convert.ToInt32((Convert.ToDouble(fixHeight) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                    //設定修改後的圖寬
                }

            }
            else
            //圖片沒有超過設定值，不執行縮圖
            {
                fixHeight = image.Height;
                fixWidth = image.Width;
            }
            Bitmap imageOutput = new Bitmap(image, fixWidth, fixHeight);
            
            //副檔名不應該這樣給，但因為此範例沒有讀取檔案的部份所以demo就直接給啦

            imageOutput.Save(saveThumbFilePath, thisFormat);
            //將修改過的圖存於設定的位子
            imageOutput.Dispose();
            //釋放記憶體
            image.Dispose();
            //釋放掉圖檔 
            File.Delete(srcImagePath);
        }

        /// <summary>
        /// 產生截圖並儲存 寬度維持500pix，高度等比例
        /// </summary>
        /// <param name="srcImagePath">來源圖片的路徑</param>
        /// <param name="saveThumbFilePath">縮圖的儲存檔案路徑</param>
        public void SaveThumbPicWidth(string srcImagePath, string saveThumbFilePath) //截圖
        {
            //讀取原始圖片 
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片 
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(fs);

                //圖片寬高
                int ImgWidth = 500;
                //float ImgX = (float)bitmap.Width / 500;//圖片縮小倍率
                int ImgHeight = 500 ;//Convert.ToInt16(bitmap.Height / ImgX);


                // 產生縮圖 
                using (var bmp = new Bitmap(ImgWidth, ImgHeight))
                {
                    using (var gr = Graphics.FromImage(bmp))
                    {

                        gr.CompositingQuality = CompositingQuality.HighQuality;
                        gr.SmoothingMode = SmoothingMode.HighQuality;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.DrawImage(bitmap, new Rectangle(0, 0, ImgWidth, ImgHeight), 0, 0, ImgWidth, ImgHeight, GraphicsUnit.Pixel);
                        bmp.Save(saveThumbFilePath);
                    }
                }

                fs.Close();

            }
            File.Delete(srcImagePath);
        }
        #endregion
    }
}