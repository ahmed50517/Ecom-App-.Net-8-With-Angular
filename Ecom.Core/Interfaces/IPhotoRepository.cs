﻿using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IPhotoRepository:IGenericRepository<Photo>
    {
        //for example, you can add methods specific to Photo repository here
    }
}
