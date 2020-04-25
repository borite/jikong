using BigXia_yingxiao.Models;
using ChinaAudio.Class;
using ChinaAudio.DTOModels;
using JK_Exam.Entiy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace JK_Exam.Controllers
{
    /// <summary>
    /// 所有
    /// </summary>
    [RoutePrefix("api/tools")]
    public class UserController : ApiController
    {

        JK_ExamEntities jk = new JK_ExamEntities();

        Common comm = new Common();
        Code code = new Code();
        /// <summary>
        /// 检测openID，是否注册过
        /// </summary>
        /// <param name="obj"> Code参数判断</param>
        /// <returns></returns>
        [HttpPost, Route("LoginAndRrgister")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "返回信息 200登陆成功 201注册成功    传入Json格式  参数是Code")]
        public object LoginAndRrgister([FromBody] JObject obj)
        {
           
            string appid = "wx4a873d58da246519";
            string secret = "65f3c061efbc76fe6a93a9c26c0fc9ef";

            byte[] bs = Encoding.ASCII.GetBytes(obj["Code"].ToString());    //参数转化为ascii码
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + obj["Code"].ToString() + "&grant_type=authorization_code");  //创建请求
            req.Method = "POST";    //确定传值的方式，此处为post方式传值
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }

            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理(第一次 通过code查openid 和 access_token)
                //坑   这里需要把流数据解析出来再放到字符串

              
                string codeVal = new StreamReader(wr.GetResponseStream()).ReadToEnd();//解析出来再放到字符串
                TokenMessage jsonData = JsonConvert.DeserializeObject<TokenMessage>(codeVal); //反序列化成json
                TokenMessage tsave = new TokenMessage();  //实例化一个接收两个参数的类
                tsave.openid = jsonData.openid; //赋值接收到的值
                tsave.access_token = jsonData.access_token; //赋值接收到的值
                if (string.IsNullOrEmpty(tsave.openid) || string.IsNullOrEmpty(tsave.access_token)) //如果请求失败
                {
                    return code.SystemError("微信Code第一步验证出问题了,有可能code过期，需要前端重新访问获取code的地址");


                }
                //再次请求api
                HttpWebRequest req2 = (HttpWebRequest)HttpWebRequest.Create("https://api.weixin.qq.com/sns/userinfo?access_token=" + tsave.access_token + "&openid=" + tsave.openid + "&lang=zh_CN");

                req2.Method = "GET";
                using (WebResponse wr2 = req2.GetResponse())
                {
                    string codeVal2 = new StreamReader(wr2.GetResponseStream()).ReadToEnd();//解析出来再放到字符串
                    if (string.IsNullOrEmpty(codeVal2)) //如果请求失败
                    {
                        return code.SystemError("code第二步骤验证出问题了");
                    }

                    //序列化成json
                    TokenMessage jsonval = JsonConvert.DeserializeObject<TokenMessage>(codeVal2);

                    var cc = jk.User_JK.Where(a => a.OpenID == jsonData.openid).FirstOrDefault();

                    if (cc == null) //如果没有注册
                    {
                        User_JK res = new User_JK();

                        res.OpenID = jsonval.openid;
                        res.WeChatName = jsonval.nickname; //微信名字
                        res.HeadImg = jsonval.headimgurl;
                        res.IsChou = false;
                        jk.User_JK.Add(res);
                        jk.SaveChanges();
                   
                        return code.SussessV("201","新用户注册成功",res);


                    }
                    else //如果注册了
                    {

                        return code.Sussess("登录成功",cc);


                    }





                }



            }
        }



        /// <summary>
        /// 查找个人信息
        /// </summary>
        /// <param name="OpenID"></param>
        /// <returns></returns>
        [HttpGet ,Route("UserInfo")]

        public IHttpActionResult UserInfo( string OpenID)
        {

            var cc = jk.User_JK.Where(a => a.OpenID == OpenID).FirstOrDefault();
            if (cc==null)
            {
                return Content(HttpStatusCode.OK, code.SussessV("404", "信息找不到", ""));
            }
            else
            {
                return Content(HttpStatusCode.OK, code.SussessV("200", "返回信息成功", cc));

            }
         

        }




  /// <summary>
  /// 进行一次抽奖
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
        [HttpPost,Route("Chou")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "返回信息 200抽奖成功 202重复抽奖 400发生并发，前端按没有抽中处理即可")]

        public IHttpActionResult Chou(ChouDTO obj)
        {
            var CC = jk.User_JK.Where(a => a.OpenID == obj.OpenID).FirstOrDefault();
            if (CC.IsChou==false) //如果没有抽过奖
            {
                try
                {
                    Random r1 = new Random();//定义随机器

                    double[] moneyPool = { 1.00,1.66,1.88,2.00,2.66,2.88,3.00 };
                    int jiangIndex = r1.Next(1, moneyPool.Length); //产生1..100之间的随机数

                    var jiang = moneyPool[jiangIndex];

                    //if (jiang != 0) //不是0后台标记为抽过奖（中奖），用户状态修改，中奖记录加数据
                    //{
                        CC.IsChou = true;
                        //抽奖表增加记录
                        Prize_JK res = new Prize_JK();
                        res.ChouTime = DateTime.Now;
                        res.UserID = CC.ID;
                        res.OpenID = CC.OpenID;
                        res.Money = Convert.ToDecimal(jiang);
                        res.WeChatName = CC.WeChatName;
                        jk.Prize_JK.Add(res);
                        jk.SaveChanges();
                        return Content(HttpStatusCode.OK, code.SussessV("200", "抽奖成功", jiang));
                   // }
                    //else //抽奖没有中奖
                    //{
                    //    return Content(HttpStatusCode.OK, code.SussessV("200", "抽奖成功，未中奖", jiang));
                    //}
                }
                catch (Exception)
                {

                    return Content(HttpStatusCode.OK, code.SussessV("400", "发生并发，请重新尝试", "按未抽奖处理"));
                }
               
            }
            else //抽过奖了
            {
                return Content(HttpStatusCode.OK, code.SussessV("202", "不能重复抽奖，已经抽过奖了", "")); 
            }

        }

        

        /// <summary>
        /// 获取中奖信息
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>


        [HttpGet,Route("GetUserPrize")]
        public IHttpActionResult GetUserPrize(string OpenId) 
        {

            var cc = jk.Prize_JK.Where(a => a.OpenID == OpenId).FirstOrDefault();
            return Content(HttpStatusCode.OK, code.SussessV("200", "返回信息成功", cc));
        }


        /// <summary>
        /// 随机抽取题
        /// </summary>
        /// <param name="ExamNum">抽取题数</param>
        /// <returns></returns>
        [HttpGet, Route("GetExam")]

        public IHttpActionResult GetExam(int ExamNum) 
        {

            var data = jk.Exam_JK.OrderBy(a => Guid.NewGuid()).Take(ExamNum);
            return Content(HttpStatusCode.OK, code.SussessV("200", "返回题目成功", data));









        }





    }
}
