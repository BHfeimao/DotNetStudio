﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using System.IO;
using System.Web.Configuration;
using Senparc.Weixin.MP.MvcExtension;
using DotNet.CloudFarm.WebSite.Models;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using log4net;
using DotNet.CloudFarm.Domain.Contract.Product;
using DotNet.CloudFarm.Domain.Contract.WeiXin;
using DotNet.CloudFarm.Domain.Contract.Order;
using DotNet.CloudFarm.Domain.Model.WeiXin;
using DotNet.CloudFarm.Domain.Model.WeiXin;
using DotNet.CloudFarm.Domain.Model.Order;

namespace DotNet.CloudFarm.WebSite.Controllers
{
    /// <summary>
    /// 后台
    /// </summary>
    public class HouTaiController : BaseHouTaiController
    {
        [Ninject.Inject]
        public IProductService ProductService { get; set; }
        /// <summary>
        /// 微信相关业务
        /// </summary>
        [Ninject.Inject]
        public IWeiXinService WeiXinService { get; set; }
     
        
        [Ninject.Inject]
        public IOrderService OrderService { get; set; }
        
        public ActionResult Index()
        {
            return View();
        }

        #region 微信菜单
        /// <summary>
        /// 微信底部菜单管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WeixinMenu()
        {
            var accesstoken = AccessTokenContainer.TryGetToken(AppId, AppSecret);
            var menu = CommonApi.GetMenu(accesstoken);
            if(menu!=null)
            {
                ViewBag.MenuList = menu.menu;
            }
            else
            {
                ViewBag.MenuList = null;
            }
            return View();
        }

        #endregion

        #region 微信回复

        /// <summary>
        /// 微信回复内容设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WeixinMessage()
        {
            var accesstoken = AccessTokenContainer.TryGetToken(AppId, AppSecret);
            var autoReplyMessagelist = WeiXinService.AutoReplyMessageGetAll();
            ViewBag.MessageList = autoReplyMessagelist;
            return View();
        }
        /// <summary>
        /// 编辑或添加微信回复内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WeixinMessage(WeixinAutoReplyMessageModel model)
        {
            model.CreateTime = DateTime.Now;
            model.CreatorId = this.Admin.Id;
            if(model.Id==0)
            {
                WeiXinService.AutoReplyMessageInsert(model);
            }
            else
            {
                WeiXinService.AutoReplyMessageUpdate(model);
            }
            var autoReplyMessagelist = WeiXinService.AutoReplyMessageGetAll();
            ViewBag.MessageList = autoReplyMessagelist;
            return View();
        }
        [HttpPost]
        public JsonResult CheckKeyword(string keyword)
        {
            var result = WeiXinService.AutoReplyMessageCheckKeyword(keyword);
            return Json(result);
        }
        [HttpPost]
        public JsonResult DelKeyword(int id)
        {
            WeiXinService.AutoReplyMessageUpdateStatus(id, 0);
            return Json(1);
        }

        #endregion

        #region 产品后台

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Product(int pageIndex=1,int pageSize=10)
        {
            var status = 1;//调取显示的
            var productList =ProductService.GetProducts(pageIndex,pageSize,status);
            ViewBag.ProductList = productList;
            return View();
        }
        /// <summary>
        /// 新增或编辑product页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ProductAddOrEdit(int id=0)
        {
            if(id>0)
            {
                ViewBag.Product = ProductService.GetProductById(id);
            }
            return View();
        }

        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProductAddOrEdit(ProductModel product)
        {
            product.CreateTime = DateTime.Now;
            product.CreatorId = Admin.Id;

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength>0)
            {
                var file = Request.Files[0];
                var urlPath = string.Format("/images/upload/{0}/", DateTime.Now.ToString("yyyyMMdd"));
                var imgFilePath = Server.MapPath(urlPath);
                var imgName = string.Format("{0}.jpg",Guid.NewGuid());
                if (!Directory.Exists(imgFilePath))
                {
                    Directory.CreateDirectory(imgFilePath);
                }
                file.SaveAs(Path.Combine(imgFilePath,imgName));
                product.ImgUrl = string.Format("{0}{1}",urlPath,imgName);
            }
            else
            {
                if(string.IsNullOrEmpty(product.ImgUrl))
                {
                    product.ImgUrl = "/images/no_pic.jpg";
                }
            }
            if(product.Id==0)
            {
                //insert
                ProductService.InsertProduct(product);
            }
            else if(product.Id>0)
            {
                //update
                ProductService.UpdateProduct(product);

            }
            return RedirectToAction("Product");
        }

        /// <summary>
        /// AJAX获取所有product
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetProducts(int pageSize=20,int pageIndex=1)
        {
            var productList = ProductService.GetProducts(pageIndex, pageSize);
            var result = new
            {
                PageIndex = productList.PageIndex,
                PageSize = productList.PageSize,
                Products = productList.ToList(),
                Count = productList.TotalCount,
                PageNo=productList.TotalCount % productList.PageSize != 0 ? (productList.TotalCount / productList.PageSize) + 1 : productList.TotalCount / productList.PageSize
            };
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AJAX删除product
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelProduct(int id, int pageSize = 20, int pageIndex = 1)
        {
            var status = -1;
            ProductService.UpdateStatus(id,status);
            var productList = ProductService.GetProducts(pageIndex, pageSize);
            var result = new
            {
                PageIndex = productList.PageIndex,
                PageSize = productList.PageSize,
                Products = productList.ToList(),
                Count = productList.TotalCount,
                PageNo=productList.TotalCount % productList.PageSize != 0 ? (productList.TotalCount / productList.PageSize) + 1 : productList.TotalCount / productList.PageSize
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region 订单后台
        public ActionResult OrderList()
        {
            
            return View();
        }
        #endregion
    }
}