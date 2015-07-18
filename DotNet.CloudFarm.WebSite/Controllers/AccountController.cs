﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DotNet.CloudFarm.Domain.Contract.User;
using DotNet.CloudFarm.Domain.DTO.User;
using DotNet.CloudFarm.Domain.Impl.User;
using DotNet.CloudFarm.Domain.Model.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using DotNet.CloudFarm.WebSite.Models;
using DotNet.Common.Models;
using DotNet.Identity;
using DotNet.Identity.Database;

namespace DotNet.CloudFarm.WebSite.Controllers
{

    public class AccountController : BaseController
    {
        public AccountController()
            : this(new UserManager<CloudFarmIdentityUser>(new CloudFarmUserStore()))
        {
            UserService=new UserService(new UserDataAccess());
        }

        public AccountController(UserManager<CloudFarmIdentityUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<CloudFarmIdentityUser> UserManager { get; private set; }

        [Ninject.Inject]
        public IUserService UserService { get; set; }

        [HttpPost]
        public async Task<JsonResult> Login(LoginUser loginUser)
        {
            var jsonResult = new JsonResult();
            var result = UserManager.FindByNameAsync(loginUser.Mobile);
            if (result!=null)
            {
                await SignInAsync(result.Result, true);
                jsonResult.Data = new { IsSuccess = true };
            }
            else
            {
                jsonResult.Data = new { IsSuccess = false };
            }

            return jsonResult;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult LogOff()
        {
            var jsonResult = new JsonResult();
            AuthenticationManager.SignOut();
            var result = new Result<object>();
            result.Data = new {LoginUrl="/Account/Login"};
            result.Status = new Status() { Code = "1", Message = "退出登录成功。" };
            jsonResult.Data = result;
            return jsonResult;
        }

        public JsonResult GetMobileCaptcha(string mobile)
        {
            var result=UserService.GetCaptcha(mobile);

            return new JsonResult();
        }

        public ActionResult Login()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(CloudFarmIdentityUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
    }
}