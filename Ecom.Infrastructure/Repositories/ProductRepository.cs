using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagmentService imageManagmentService;
        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagmentService imageManagmentService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagmentService = imageManagmentService;
        }

        public async Task<bool> AddProductAsync(AddProductDTO productDTO)
        {
            if (productDTO == null) return false;
            var product =mapper.Map<Product>(productDTO);
           await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var Imagepath = await imageManagmentService.AddImageAsync(productDTO.Photo, productDTO.Name);

            //manual Mapping
            var photo = Imagepath.Select(path => new Photo
            {
                ImageName=path,
                ProductId = product.Id
            }).ToList();
           await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;


        }

        public async Task<bool> UpdateProdcutAsync(UpdateProductDTO updateProductDTO)
        {
            if (updateProductDTO is null)
            {
                return false;
            }
            var FindProduct = await context.Products.Include(x => x.Category)
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x=>x.Id==updateProductDTO.Id);
            if(FindProduct is null)
            {
                return false;
            }
            mapper.Map(updateProductDTO, FindProduct);

            var FindPhoto =await context.Photos.Where(m=>m.ProductId == updateProductDTO.Id).ToListAsync();
            
            foreach(var item in FindPhoto)
            {
                imageManagmentService.DeleteImageAsync(item.ImageName);
            }
            context.Photos.RemoveRange(FindPhoto);

            var ImagePath= await imageManagmentService.AddImageAsync(updateProductDTO.Photo, updateProductDTO.Name);

            var photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = updateProductDTO.Id
            }).ToList();
            await context.Photos.AddRangeAsync(photo); 
            await context.SaveChangesAsync();
            return true;

        }
    }
}
