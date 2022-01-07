using AutoMapper;
using Board.Models.Data;
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
            CreateMap<TableModel, TableModel>();
            CreateMap<ColumnModel, ColumnModel>();
            CreateMap<EntryModel, EntryModel>();

            CreateMap<EntryModel, OrderedEntryModel>().ReverseMap();
            CreateMap<ColumnModel, OrderedColumnModel>().ReverseMap();
            CreateMap<TableModel, OrderedTableModel>().ReverseMap();

            CreateMap<Entry, OrderedEntryModel>().ReverseMap();
            CreateMap<Column, OrderedColumnModel>().ReverseMap();
            CreateMap<Table, OrderedTableModel>().ReverseMap();
        }
    }
}
