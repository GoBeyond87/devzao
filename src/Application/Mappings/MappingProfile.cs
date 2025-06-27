using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeamento de Produto para ProdutoDto
        CreateMap<Produto, ProdutoDto>();
        
        // Mapeamento de CriarProdutoDto para Produto
        CreateMap<CriarProdutoDto, Produto>();
        
        // Mapeamento de AtualizarProdutoDto para Produto
        // Ignora o Id, pois ele vem da rota
        CreateMap<AtualizarProdutoDto, Produto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
