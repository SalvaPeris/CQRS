using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Common.Domain;
using ApplicationCore.Common.Helpers;
using ApplicationCore.Infrastructure.Persistence;
using AutoMapper.QueryableExtensions;

namespace ApplicationCore.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<List<GetProductsQueryResponse>>
    {
        public int ProductId { get; set; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<GetProductsQueryResponse>>
    {
        private readonly MyAppDbContext _context;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(MyAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GetProductsQueryResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
            await _context.Products.AsNoTracking()
                .ProjectTo<GetProductsQueryResponse>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public class GetProductsQueryResponse
    {
        public string ProductId { get; set; } = string.Empty;
        public string Description { get; set; } = default!;
        public double Price { get; set; }
        public string ListDescription { get; set; } = default!;
    }


    public class GetProductsQueryProfile : Profile
    {
        public GetProductsQueryProfile() => CreateMap<Product, GetProductsQueryResponse>()
                .ForMember(dest =>
                    dest.ListDescription, opt => opt.MapFrom(mf => $"{mf.Description} - {mf.Price}"))
                .ForMember(dest =>
                    dest.ProductId, opt => opt.MapFrom(mf => mf.ProductId.ToHashId()));
    }
}
