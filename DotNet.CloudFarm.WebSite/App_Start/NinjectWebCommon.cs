using DotNet.CloudFarm.Domain.Contract.Address;
using DotNet.CloudFarm.Domain.Contract.Message;
using DotNet.CloudFarm.Domain.Contract.Order;
using DotNet.CloudFarm.Domain.Contract.Product;
using DotNet.CloudFarm.Domain.Contract.User;
using DotNet.CloudFarm.Domain.Contract.WeiXin;
using DotNet.CloudFarm.Domain.DTO.Address;
using DotNet.CloudFarm.Domain.DTO.WeiXin;
using DotNet.CloudFarm.Domain.DTO.Message;
using DotNet.CloudFarm.Domain.DTO.Order;
using DotNet.CloudFarm.Domain.DTO.Product;
using DotNet.CloudFarm.Domain.DTO.User;
using DotNet.CloudFarm.Domain.Impl.Address;
using DotNet.CloudFarm.Domain.Impl.Message;
using DotNet.CloudFarm.Domain.Impl.Order;
using DotNet.CloudFarm.Domain.Impl.Product;
using DotNet.CloudFarm.Domain.Impl.User;
using DotNet.CloudFarm.Domain.Impl.WeiXin;
using DotNet.CloudFarm.Domain.Model;
using DotNet.CloudFarm.Domain.Model.Order;
using DotNet.CloudFarm.WebSite.Controllers;
using DotNet.Data;
using DotNet.Data.Repositories;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DotNet.CloudFarm.WebSite.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(DotNet.CloudFarm.WebSite.App_Start.NinjectWebCommon), "Stop")]

namespace DotNet.CloudFarm.WebSite.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using DotNet.CloudFarm.Domain.Contract.SMS;
    using DotNet.CloudFarm.Domain.Impl.SMS;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            //ҵ��ӿ�ע���

            //entityframework��ע��
            //kernel.Bind<CloudFarmDbContext>().ToSelf().InRequestScope();

            //kernel.Bind<IDbContext>().To<CloudFarmDbContext>().InRequestScope();
            kernel.Bind<IEntityFrameworkDbContext>().To<CloudFarmDbContext>().InRequestScope();

            //Ԥ����Ʒ
            kernel.Bind<IPreSaleProductAccess>().To<PreSaleProductAccess>().InRequestScope();
            kernel.Bind<IPreSaleProductService>().To<PreSaleProductService>().InRequestScope();

            //Ԥ�۶���
            kernel.Bind<IPreSaleOrderDataAccess>().To<PreSaleOrderDataAccess>().InRequestScope();
            kernel.Bind<IPreSaleOrderService>().To<PreSaleOrderService>().InRequestScope();

            //ʡ����
            kernel.Bind<IAddressDataAccess>().To<AddressDataAccess>().InRequestScope();
            kernel.Bind<IAddressService>().To<AddressService>().InRequestScope();

            //��Ϣ
            kernel.Bind<IMessageDataAccess>().To<MessageDataAccess>();
            kernel.Bind<IMessageService>().To<MessageService>();

            //�û�����
            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IUserDataAccess>().To<UserDataAccess>();

            //��Ʒ����
            kernel.Bind<IProductService>().To<ProductService>();
            kernel.Bind<IProductDataAccess>().To<ProductDataAccess>();

            //��������
            kernel.Bind<IOrderService>().To<OrderService>();
            kernel.Bind<IOrderDataAccess>().To<OrderDataAccess>();

            //΢�ŷ���
            kernel.Bind<IWeiXinMessageDataAccess>().To<WeiXinMessageDataAccess>();
            kernel.Bind<IWeiXinService>().To<WeiXinService>();
            kernel.Bind<IWeixinPayLogDataAccess>().To<WeixinPayLogDataAccess>();

            //���ŷ���
            kernel.Bind<ISMSService>().To<SMSService>();


            
        }        
    }
}
