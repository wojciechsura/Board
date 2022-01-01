using AutoMapper;
using Board.BusinessLogic.Models.Data;
using Board.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.MappingProfiles
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<Table, TableModel>().ReverseMap();
            CreateMap<Column, ColumnModel>().ReverseMap();
            CreateMap<Entry, EntryModel>().ReverseMap();
        }
    }
}
