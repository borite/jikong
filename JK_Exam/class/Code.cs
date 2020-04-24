using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;


namespace ChinaAudio.Class
{

    public class Code
    {
        public string code;
        public dynamic message;
        public dynamic data;
        public dynamic data1;

        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        JsonSerializerSettings settings = new JsonSerializerSettings();

 
        public string Sussess(dynamic message,dynamic data)
        {
            var b = new Code { code = "200", message = message, data = data };


            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string result = JsonConvert.SerializeObject(b, settings);
            return result;
        }


        /// <summary>
        /// 自定义返回
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SussessV( string Code, dynamic message, dynamic data)
        {
            var b = new Code { code = Code, message = message, data = data };


            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string result = JsonConvert.SerializeObject(b, settings);
            return result;
        }



        /// <summary>
        ///  这是第一次登陆201
        /// </summary>
        /// <param name="tokenVal">返回的token</param>
        /// <param name="userInfo">返回的用户信息</param>
        /// <returns></returns>
        public string loginSuccessFirst(dynamic tokenVal ,dynamic userInfo)
        {
            var b = new Code { code = "201", message = tokenVal, data1 = userInfo };


            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string result = JsonConvert.SerializeObject(b, settings);
            return result;
        }



      
        /// <summary>
        /// 登陆成功203
        /// </summary>
        /// <param name="tokenVal"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public string loginSuccess(dynamic tokenVal, dynamic userInfo)
        {
            var b = new Code { code = "203", message = tokenVal, data1 = userInfo };


            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string result = JsonConvert.SerializeObject(b, settings);
            return result;
        }

        /// <summary>
        /// 失败,返回500
        /// </summary>
        /// <param name="message">需要说明情况</param>
        /// <returns></returns>
        public string returnFail(dynamic message)
        {
            var b = new Code { code = "500", message = "操作失败", data1 = message };


            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string result = JsonConvert.SerializeObject(b, settings);
            return result;
        }

        /// <summary>
        ///openID错误
        /// </summary>
        /// <returns></returns>
        public string SystemError(dynamic errormsg)
        {

            var b = new Result { code = "502", message = errormsg, data = "" };


            string result = JsonConvert.SerializeObject(b);
            return result;
        }

        public class Result
        {
            public string code;
            public string message;
            public dynamic data;
        }

    }
}