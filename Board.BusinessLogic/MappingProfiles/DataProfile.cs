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
            CreateMap<Table, TableModel>().ReverseMap()
                .ForMember(e => e.Id, o => o.Ignore());
            CreateMap<Column, ColumnModel>().ReverseMap()
                .ForMember(e => e.Id, o => o.Ignore());
            CreateMap<Entry, EntryModel>().ReverseMap()
                .ForMember(e => e.Id, o => o.Ignore());
            CreateMap<Tag, TagModel>().ReverseMap()
                .ForMember(e => e.Id, o => o.Ignore());
            CreateMap<Comment, CommentModel>().ReverseMap()
                .ForMember(e => e.Id, o => o.Ignore());
            CreateMap<TableModel, TableModel>();
            CreateMap<ColumnModel, ColumnModel>();
            CreateMap<EntryModel, EntryModel>();

            CreateMap<Entry, EntryDisplayModel>();
            CreateMap<Entry, EntryEditModel>();
        }
    }
}
