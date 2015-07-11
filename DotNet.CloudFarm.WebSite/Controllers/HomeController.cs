﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNet.CloudFarm.Domain.Contract;
using DotNet.CloudFarm.Domain.Contract.Message;
using DotNet.CloudFarm.Domain.Contract.Order;
using DotNet.CloudFarm.Domain.Contract.Product;
using DotNet.CloudFarm.Domain.Contract.User;
using DotNet.CloudFarm.Domain.DTO.User;
using DotNet.CloudFarm.Domain.Impl.Order;
using DotNet.CloudFarm.Domain.Model.Base;
using DotNet.CloudFarm.Domain.Model.Order;
using DotNet.CloudFarm.Domain.Model.User;
using DotNet.CloudFarm.Domain.ViewModel;
using DotNet.WebSite.Infrastructure.Config;
using DotNet.WebSite.MVC;
using Microsoft.AspNet.Identity;

namespace DotNet.CloudFarm.WebSite.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserService userService):base(userService)
        {
            
        }

        [Ninject.Inject]
        public IUserService UserService { get; set; }

        [Ninject.Inject]
        public IProductService ProductService { get; set; }

        [Ninject.Inject]
        public IMessageService MessageService { get; set; }

        [Ninject.Inject]
        public IOrderService OrderService { get; set; }

        public ActionResult Default()
        {
            return View();
        }

        public ActionResult Index()
        {
            //数据库
            var user=UserService.GetUserByUserId(1);
            //var userId = UserService.Insert(new UserModel(){Mobile="13716457768",WxSex = 1,CreateTime = DateTime.Now});

            //读取配置文件 配置文件在网站Configs文件夹下的Params.config
            var test=ConfigHelper.ParamsConfig.GetParamValue("test");

            var userid=this.User.Identity.GetUserId();

            var result = UserService.Login(new LoginUser() {Mobile = "13716457768", Captcha = "123456"});
            return View();
        }

        public JsonResult Data()
        {
            var result = new JsonResult();
            var homeViewModel = new HomeViewModel
            {
                Products = ProductService.GetProducts(1, 5, 1), 
                SheepCount = 188
            };
            result.Data = homeViewModel;
            return result;
        }

        public JsonResult GetProductById(int productId)
        {
            var result = new JsonResult();

            if (productId > 0)
            {
                result.Data = ProductService.GetProductById(productId);
            }
            
            return result;
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Product()
        {
            return View();
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Order(int? productId)
        {
            var confirmOrderViewModel = new ConfirmOrderViewModel();
            if (productId.HasValue)
            {
                confirmOrderViewModel.Product = ProductService.GetProductById(productId.Value);
            }
            else
            {
                return RedirectToAction("Default", "Home");
            }
            return View(confirmOrderViewModel);
        }

        public JsonResult SubmitOrder(ConfirmOrderViewModel confirmOrderViewModel)
        {
            var orderModel = new OrderModel
            {
                OrderId = OrderService.GetNewOrderId(),
                UserId = this.UserInfo.UserId,
                ProductId = confirmOrderViewModel.Product.Id,
                Price = confirmOrderViewModel.Product.Price,
                ProductCount = confirmOrderViewModel.Count,
                Status = OrderStatus.Init.GetHashCode(),
                PayType = 0,
                CreateTime = DateTime.Now
            };
            var data=OrderService.SubmitOrder(orderModel);
            var result = new JsonResult();
            result.Data = data;
            return result;
        }

        /// <summary>
        /// 支付页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Pay(long? orderId)
        {
            var orderPayViewModel = new OrderPayViewModel();
            if (orderId.HasValue)
            {
                var orderModel = OrderService.GetOrder(this.UserInfo.UserId, orderId.Value);

                if (orderModel != null)
                {
                    var productModel = ProductService.GetProductById(orderModel.ProductId);

                    if (productModel != null)
                    {
                        orderPayViewModel.Name = productModel.Name;
                        orderPayViewModel.SheepType = productModel.SheepType;
                        orderPayViewModel.OrderId = orderId.Value;
                        orderPayViewModel.Price = productModel.Price;
                        orderPayViewModel.Count = orderModel.ProductCount;
                        orderPayViewModel.TotalPrice = productModel.Price*orderModel.ProductCount;
                        orderPayViewModel.StartTime = productModel.StartTime.ToString("yyyy-MM-dd");
                        orderPayViewModel.EndTime = productModel.EndTime.ToString("yyyy-MM-dd");
                    }
                }
            }

            

            return View(orderPayViewModel);
        }

        public ActionResult PaySuccess()
        {
            return View();
        }

        /// <summary>
        /// 用户中心首页
        /// </summary>
        /// <returns></returns>
        public ActionResult MyCenter()
        {
            var myCenterViewModel = new MyCenterViewModel {User = UserInfo, IsHasNoReadMessage = true};
            return View(myCenterViewModel);
        }

        public ActionResult OrderList(int pageIndex=1,int pageSize=10)
        {
            var result = OrderService.GetOrderList(this.UserInfo.UserId, pageIndex, pageSize);
            return View(result);
        }

        public ActionResult MessageList(int pageIndex=1,int pageSize=10)
        {
            var result = MessageService.GetMessages(this.UserInfo.UserId, pageIndex, pageSize);
            return View(result.Data);
        }

        public ActionResult OrderDetail(long? orderId)
        {
            return View();
        }

        public ActionResult Contract()
        {
            return View();
        }

        /// <summary>
        /// 我的钱包
        /// </summary>
        /// <returns></returns>
        public ActionResult Wallet()
        {
            var walletViewModel = new WalletViewModel();
            return View(walletViewModel);
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferAccount()
        {
            
            return View();
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <returns></returns>
        public ActionResult Redeem()
        {
            return View();
        }
    }
}