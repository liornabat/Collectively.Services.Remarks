﻿using AutoMapper;
using Collectively.Services.Remarks.Domain;
using Collectively.Services.Remarks.Dto;

namespace Collectively.Services.Remarks.Framework
{
    public class AutoMapperConfig
    {
        public static IMapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Remark, RemarkDto>();
                cfg.CreateMap<RemarkState, RemarkStateDto>();
                cfg.CreateMap<RemarkUser, RemarkUserDto>();
                cfg.CreateMap<RemarkCategory, RemarkCategoryDto>();
                cfg.CreateMap<Location, LocationDto>();
                cfg.CreateMap<RemarkPhoto, FileDto>();
                cfg.CreateMap<Category, RemarkCategoryDto>();
                cfg.CreateMap<Tag, TagDto>();
                cfg.CreateMap<Vote, VoteDto>();
            });

            return config.CreateMapper();
        }
    }
}