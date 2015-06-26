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

namespace DotNet.CloudFarm.WebSite.Models
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {

        
        
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
            return null;
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