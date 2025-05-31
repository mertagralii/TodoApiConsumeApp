using AutoMapper;
using TodoApiConsumeApp.Data.DTO.Todo;
using TodoApiConsumeApp.Data.DTO.User;
using TodoApiConsumeApp.Data.Entities;

namespace TodoApiConsumeApp;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetTodoDto, Todo>().ReverseMap();
        CreateMap<AddTodoDto, Todo>().ReverseMap();
        CreateMap<UpdateTodoDto,Todo>().ReverseMap();

        CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
        CreateMap<ApplicationUser, LoginDto>().ReverseMap();
    }
}