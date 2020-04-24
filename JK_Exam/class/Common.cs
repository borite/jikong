
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

using System.Web.Profile;


namespace ChinaAudio.Class
{
    public class Common
    {
        Code code = new Code();
        Random rd = new Random();
        #region 小方法

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string sha1(string password)
        {
            var buffer = Encoding.UTF8.GetBytes(password);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("X2"));
            }
            string HashPwd = sb.ToString();
            return HashPwd;

        }


        /// <summary>
        /// 读取Session的值
        /// </summary>
        /// <param name="key">Session的键名</param>        
        public string GetSession(string key)
        {
            if (HttpContext.Current.Session[key] == null || HttpContext.Current.Session[key].ToString() == "")
            {
                return null;
            }
            else
            {
                if (key.Length == 0)
                    return string.Empty;

                return HttpContext.Current.Session[key] as string;

            }
            //if (key.Length == 0)
            //    return string.Empty;

            //return HttpContext.Current.Session[key] as string;

        }
        /// <summary>
        /// 读取类型Session的值
        /// </summary>
        /// <param name="key">Session的键名</param>        
        public T GetSessionType<T>(string key)
        {
            if (HttpContext.Current.Session[key] == null || HttpContext.Current.Session[key].ToString() == "")
            {
                return default(T);
            }
            else
            {
                if (key.Length == 0)
                    return default(T);
                return (T)HttpContext.Current.Session[key];

            }

          



        }

        /// <summary>
        /// 写入不同类型的Session 
        /// </summary>
        /// <typeparam name="T">Session键值的类型</typeparam>
        /// <param name="key">Session的键名</param>
        /// <param name="value">Session的键值</param>
        public string WriteSessionType<T>(string key, T value)
        {
            if (key.Length == 0)
                return "0"; //没有成功就用0
            HttpContext.Current.Session[key] = value;
            return "1"; //成功状态是1
        }


        /// <summary>
        /// 生成六位随机验证码
        /// </summary>
        /// <returns></returns>
        public string yzmRandom()
        {
            var randnum = rd.Next(100000, 1000000).ToString(); //六位随机数

            return randnum;
        }

        /// <summary>
        /// 生成guid
        /// </summary>
        /// <returns></returns>
        public string GuidFun()
        {

            var uuidN = Guid.NewGuid().ToString("N"); // e0a953c3ee6040eaa9fae2b667060e09
            return uuidN;
        }




        private static object obj = new object();
        private static int GuidInt { get { return Guid.NewGuid().GetHashCode(); } }
        private static string GuidIntStr { get { return Math.Abs(GuidInt).ToString(); } }

        /// <summary>
                /// 生成相对短一点的订单号
                /// </summary>
                /// <param name="mark">前缀</param>
                /// <param name="timeType">时间精确类型  1 日,2 时,3 分，4 秒(默认) </param>
                /// <param name="id">id 小于或等于0则随机生成id</param>
                /// <returns></returns>
        public string Gener(string mark, int timeType = 4, int id = 0)
        {
            lock (obj)
            {
                var number = mark;
                var ticks = (DateTime.Now.Ticks - GuidInt).ToString();
                int fillCount = 0;//填充位数

                number += GetTimeStr(timeType, out fillCount);
                if (id > 0)
                {
                    number += ticks.Substring(ticks.Length - (fillCount + 3)) + id.ToString().PadLeft(10, '0');
                }
                else
                {
                    number += ticks.Substring(ticks.Length - (fillCount + 3)) + GuidIntStr.PadLeft(10, '0');
                }
                return number;
            }
        }

        /// <summary>
                /// 生成长的订单号
                /// </summary>
                /// <param name="mark">前缀</param>
                /// <param name="timeType">时间精确类型  1 日,2 时,3 分，4 秒(默认)</param>
                /// <param name="id">id 小于或等于0则随机生成id</param>
                /// <returns></returns>
        public string GenerLong(string mark, int timeType = 4, long id = 0)
        {
            lock (obj)
            {
                var number = mark;
                var ticks = (DateTime.Now.Ticks - GuidInt).ToString();
                int fillCount = 0;//填充位数

                number += GetTimeStr(timeType, out fillCount);
                if (id > 0)
                {
                    number += ticks.Substring(ticks.Length - fillCount) + id.ToString().PadLeft(19, '0');
                }
                else
                {
                    number += GuidIntStr.PadLeft(10, '0') + ticks.Substring(ticks.Length - (9 + fillCount));
                }
                return number;
            }
        }

        /// <summary>
                /// 获取时间字符串
                /// </summary>
                /// <param name="timeType">时间精确类型  1 日,2 时,3 分，4 秒(默认)</param>
                /// <param name="fillCount">填充位数</param>
                /// <returns></returns>
        private static string GetTimeStr(int timeType, out int fillCount)
        {
            var time = DateTime.Now;
            if (timeType == 1)
            {
                fillCount = 6;
                return time.ToString("yyyyMMdd");
            }
            else if (timeType == 2)
            {
                fillCount = 4;
                return time.ToString("yyyyMMddHH");
            }
            else if (timeType == 3)
            {
                fillCount = 2;
                return time.ToString("yyyyMMddHHmm");
            }
            else
            {
                fillCount = 0;
                return time.ToString("yyyyMMddHHmmss");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">传入数据的类型</typeparam>
        /// <param name="list">把整理的</param>
        /// <returns></returns>

        public string ToJsonString<T>(List<T> list)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string result = JsonConvert.SerializeObject(list, settings);
            return result;
        }

        /// <summary>
        /// 随机生成验证码
        /// </summary>


        /// <summary>
        /// 一个时间到现在过了多久（一天，一周，一月）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string DateToNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                return dt.ToShortDateString();
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return
                    "1个月前";
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return
                        "2周前";
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return
                            "1周前";
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return
                                string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return
                                    string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return
                                        string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return
                                            string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return
                                            "1秒前";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 日期比较
        /// </summary>
        /// <param name="today">当前日期</param>
        /// <param name="writeDate">输入日期</param>
        /// <param name="n">比较天数</param>
        /// <returns>当前日期大（第一个参数大是false）， 写入天数大是true</returns>
        public bool CompareDate(string today, string writeDate)
        {
            DateTime Today = Convert.ToDateTime(today);
            DateTime WriteDate = Convert.ToDateTime(writeDate);

            if (Today >= WriteDate)
                return false;
            else
                return true;
        }





        public class yzmClass
        {


            public string Message { get; set; }
            public string RequestId { get; set; }
            public string BizId { get; set; }
            /// <summary>
            /// Code OK 表示发送成功 前端判断这里状态即可  一般短信过于频繁会报 如果遇到其他问题详见：https://help.aliyun.com/document_detail/101346.html?spm=a2c4g.11186623.6.621.24ce2246Fo3AOU
            /// </summary>
            public string Code { get; set; }

        }






     



        /// <summary>
        /// 转guid
        /// </summary>
        /// <param name="guidval"></param>
        /// <returns>返回guid格式</returns>
        public dynamic toGuidFun(string guidval)
        {
            Guid gv = new Guid();
            gv = new Guid(guidval);
            return gv;

        }

        #endregion

    }
}