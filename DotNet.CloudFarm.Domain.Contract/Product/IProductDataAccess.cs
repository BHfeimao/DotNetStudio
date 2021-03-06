﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.CloudFarm.Domain.Model.Product;
using DotNet.Common.Collections;

namespace DotNet.CloudFarm.Domain.Contract.Product
{
    public interface IProductDataAccess
    {
        /// <summary>
        /// 获取产品列表(未删除)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedList<ProductModel> GetProducts(int pageIndex, int pageSize);
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="status">产品状态</param>
        /// <returns></returns>
        PagedList<ProductModel> GetProducts(int pageIndex, int pageSize, int status);


        /// <summary>
        /// 根据商品id获取商品详情
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        ProductModel GetProductById(int productId);

        int InserProduct(ProductModel product);

        void UpdateProduct(ProductModel product);
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        PagedList<ProductModel> GetProducts(int pageIndex, int pageSize, string condition);

        /// <summary>
        /// 更新产品状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        void UpdateStatus(int id, int status);

        /// <summary>
        /// 更新产品虚拟库存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="VirtualSaledCount"></param>
        void UpdateVirtualSaledCount(int id, int VirtualSaledCount);

        /// <summary>
        /// 根据条件获取单个产品
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ProductModel GetProductByCondition(string condition);
    }
}
