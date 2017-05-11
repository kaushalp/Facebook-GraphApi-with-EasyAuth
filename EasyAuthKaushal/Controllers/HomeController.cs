using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EasyAuthKaushal.Models;
using Facebook;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Mime;

namespace EasyAuthKaushal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string accessToken = this.Request.Headers["X-MS-TOKEN-FACEBOOK-ACCESS-TOKEN"];
            if(accessToken=="")
            {
                accessToken = "NULL";
            }
            ViewBag.AccessToken = accessToken;
            return View();
            
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Facebook()
        {
            if(User.Identity.IsAuthenticated)
            {
                string accessToken = this.Request.Headers["X-MS-TOKEN-FACEBOOK-ACCESS-TOKEN"];

                // Use Facebooks Graph API Explorer to generate & Test the URL. https://developers.facebook.com/tools/explorer/
                // Here we are retrieving details on the following post: https://www.facebook.com/Microsoft/videos/10154678740513721/?hc_ref=PAGES_TIMELINE 
                string fields = "fields=likes.limit(20){name,username,id},comments.limit(20){comment_count,from,attachment}";
                string formattedJson = CallFacebookGraphApi("10154678740513721", fields, accessToken);
                ViewBag.MsBuild2017 = formattedJson;
                
                // Here we are retrieving the users details
                string myFields = "fields=first_name,last_name,middle_name,birthday,currency,email";
                string myJsonResponse = CallFacebookGraphApi("me", myFields, accessToken);
                ViewBag.Me = myJsonResponse;

                // Here we are retrieving users last 5 posts
                string myRecentPosts = "fields=posts.limit(5){likes{name,id}}";
                string myRecentPostsJsonResponse = CallFacebookGraphApi("me", myRecentPosts, accessToken);
                ViewBag.RecentPosts = myRecentPostsJsonResponse;
                
                // Here we are retrieving users friends who have authenticated against this site.
                string myFriends = "fields=name,birthday,gender&limit=30";
                string myFriendsJsonResponse = CallFacebookGraphApi("me/friends", myFriends, accessToken);
                ViewBag.MyFriends = myFriendsJsonResponse;
                
                // Here we are retrieving the feed of posts (including status updates) and links published by this person, or by others on this person's profile.
                string myFeeds = "fields=story,created_time,name,from&limit=10";
                string myFeedJsonResponse = CallFacebookGraphApi("me/feed", myFeeds, accessToken);
                ViewBag.MyFeeds = myFeedJsonResponse;
            }

            return View();
        }

        private string CallFacebookGraphApi(string id, string fields, string accessToken)
        {
            string fbGraphiApiRequest = "https://graph.facebook.com/" + id + "?" + fields + "&access_token=" + accessToken;
            string response = RequestResponse(fbGraphiApiRequest);
            if (response == "")
            {
                response="<br /><br />Error while accessing the FB Graph API";
            }
            string resJson = response.Replace(@"\", string.Empty);
            var obj = JsonConvert.DeserializeObject(resJson);
            string formattedJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return formattedJson;
        }

        private string RequestResponse(string pUrl)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            Stream responseStream = null;
            StreamReader responseReader = null;
            string responseData = "";
            try
            {
                WebResponse webResponse = webRequest.GetResponse();
                responseStream = webResponse.GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
                
            }
            catch (Exception exc)
            {
                Response.Write("<br /><br />ERROR : " + exc.Message);
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseReader.Close();
                }
            }

            return responseData;
        }


    }
}