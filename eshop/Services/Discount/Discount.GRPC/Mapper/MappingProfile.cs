using AutoMapper;
using Discount.GRPC.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.GRPC.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CouponModel, Coupon>().ReverseMap();
        }
    }
}
