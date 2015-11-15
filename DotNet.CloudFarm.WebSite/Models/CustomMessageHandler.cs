﻿using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Helpers;
using log4net;
using DotNet.Common.Utility;
using DotNet.CloudFarm.Domain.Contract.WeiXin;
using DotNet.CloudFarm.Domain.Model.WeiXin;
using DotNet.CloudFarm.Domain.Impl.WeiXin;
using DotNet.CloudFarm.Domain.DTO.WeiXin;
using DotNet.CloudFarm.Domain.Contract.User;
using DotNet.CloudFarm.Domain.Model.User;
using DotNet.CloudFarm.Domain.Impl.User;
using DotNet.CloudFarm.Domain.DTO.User;
using Senparc.Weixin.MP.CommonAPIs;
using System.Web.Configuration;
using DotNet.CloudFarm.Domain.Model;

namespace DotNet.CloudFarm.WebSite.Models
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        /// <summary>
        /// 与微信公众账号后台的AppId设置保持一致，区分大小写。
        /// </summary>
        public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];
        /// <summary>
        /// 与微信公众账号后台的AppSecret设置保持一致，区分大小写。
        /// </summary>
        public static readonly string AppSecret = WebConfigurationManager.AppSettings["WeixinAppSecret"];

        /// <summary>
        /// 微信相关业务
        /// </summary>
        //[Ninject.Inject]
        //public IWeiXinService WeiXinService { get; set; }

        private ILog logger = LogManager.GetLogger("CustomMessageHandler");

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            WeixinContext.ExpireMinutes = 3;
        }
        /// <summary>
        /// 没有处理的消息类型，统一处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            return responseMessage;
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            try
            {
                var keyword = requestMessage.Content;
                var dataAccess = new WeiXinMessageDataAccess();
                var service = new WeiXinService(dataAccess);
                var message = service.AutoReplyMessageGetByKeyword(keyword);

                if (message != null && message.Id > 0)
                {
                    responseMessage.Content = message.ReplyContent;
                    return responseMessage;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
           

        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            if(requestMessage.EventKey=="learnmore")
            {
                var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                var str = "您好，很高兴为您服务。您可以回复数字，查看常见问题的解答，也可以直接发送问题至羊客。\r\n回复【1】了解关于购买和结算；\r\n回复【2】了解关于小羊；\r\n回复【3】了解关于资金安全和收益保障";
                responseMessage.Content = str;
                return responseMessage;
            }
            else if (requestMessage.EventKey == "updating")
            {
                var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                var str = "全新羊客平台升级中，尽请期待";
                responseMessage.Content = str;
                return responseMessage;
            }
            else
            {
                return null;
            }
        }

        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            var welcomeStr = "欢迎来到羊客，快戳链接1元领取一只价值1000元的小羊吧：http://yk.kerchinsheep.com/home/active  10天后点击结算返还1元和5元现金收益，每人限领1只哦！";
            //var welcomeStr = "羊客采取互联网创新思维，让所有牧场主自由领养，全透明地了解养殖标准和流程，获取年化15%的收益或等值的羊肉产品，让你真正实现：一边吃肉，一边赚钱！点击“羊羊得益”马上参与";

            responseMessage.Content = welcomeStr;
            try
            {
                var openId = requestMessage.FromUserName;
                var userDataAccess = new UserDataAccess();
                var userService = new UserService(userDataAccess, null, new CloudFarmDbContext());
                var user = userService.GetUserByWxOpenId(openId);
                if (user == null || user.UserId == 0)
                {
                    var accesstoken = AccessTokenContainer.TryGetToken(AppId, AppSecret);
                    var wxUser = CommonApi.GetUserInfo(accesstoken, openId);
                    if (!string.IsNullOrEmpty(wxUser.headimgurl))
                    {
                        wxUser.headimgurl = wxUser.headimgurl.Substring(0, wxUser.headimgurl.Length - 1) + "96";
                    }
                    var userModel = new UserModel()
                    {
                        CreateTime = DateTime.Now,
                        WxOpenId = openId,
                        WxHeadUrl = wxUser.headimgurl,
                        WxNickName = wxUser.nickname,
                        Status=1
                    };
                    userService.Insert(userModel);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return responseMessage;
        }
        /// <summary>
        /// 在所有消息处理之前执行
        /// </summary>
        public override void OnExecuting()
        {
            //TODO:黑名单相关部分可以在此处理

            //CancelExcute = true; //终止此用户的对话
            ////如果没有下面的代码，用户不会收到任何回复，因为此时ResponseMessage为null
            ////添加一条固定回复
            //var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "Hey！随便测试的！";
            //ResponseMessage = responseMessage;//设置返回对象
            
            
            base.OnExecuting();
        }





    }
}
