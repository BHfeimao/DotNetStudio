﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.CloudFarm.Domain.ViewModel
{
    /// <summary>
    /// 订单支付viewmodel
    /// </summary>
    public class OrderPayViewModel
    {
        public long OrderId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 羊类型
        /// </summary>
        public string SheepType { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 支付方式：0-微信支付；1-线下支付
        /// </summary>
        public int PayType { get; set; }

        public string wxEditAddrParam { get; set; }

        public string openid { get; set; }

        /// <summary>
        /// 赎回收益
        /// </summary>
        public decimal Earing { get; set; }

        /// <summary>
        /// 本金
        /// </summary>
        public decimal Principal { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 产品图片URL
        /// </summary>
        public string ImgUrl { get; set; }
    }
}
