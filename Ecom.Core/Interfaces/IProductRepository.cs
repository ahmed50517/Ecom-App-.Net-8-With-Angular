﻿using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        //for example, you can add methods specific to Product repository here

        Task<bool> AddProductAsync(AddProductDTO productDTO);
        Task<bool> UpdateProdcutAsync(UpdateProductDTO updateProductDTO);

        Task DeleteProductAsync(Product product);
    }
}
