using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApplication1.Models
{
    // ApplicationUser クラスにプロパティを追加することでユーザーのプロファイル データを追加できます。詳細については、http://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType が CookieAuthenticationOptions.AuthenticationType で定義されているものと一致している必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // ここにカスタム ユーザー クレームを追加します
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    public class GoogleReCaptcha
    {
        public bool Success { get; set; }
        /// <summary>
        /// Google 機器人驗證
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool GetCaptchaResponse(string message)
        {
            try
            {
                /*
                  using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new[]{
                        new KeyValuePair<string, string>("secret", "{YourSecretKey}"),
                        new KeyValuePair<string, string>("response", message),
                    });

                    var result = client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content).Result;
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<GoogleReCaptcha>(resultContent);
                    return data.Success;
                }
                 */
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new[]{
                        new KeyValuePair<string, string>("secret", "{6LfBriQTAAAAANGF9w6CrSl_8yksdNy9dNi7Xp9R}"),
                        new KeyValuePair<string, string>("response'.", message),
                    });

                    var result = client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content).Result;
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<GoogleReCaptcha>(resultContent);
                    return data.Success;
                }
            }
            catch { return false; }
        }
    }
}