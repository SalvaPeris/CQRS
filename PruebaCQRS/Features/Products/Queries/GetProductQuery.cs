using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PruebaCQRS.Domain;
using PruebaCQRS.Exceptions;
using PruebaCQRS.Infrastructure.Persistence;
using System.Linq;
using PruebaCQRS.Helpers;

namespace PruebaCQRS.Features.Products.Queries
{
    public class GetProductQuery : IRequest<GetProductQueryResponse>
    {
        public string ProductId { get; set; } = string.Empty;
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery,GetProductQueryResponse>
    {
        private readonly MyAppDbContext _context;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(MyAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetProductQueryResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.ProductId.ToString().FromHashId());

            if(product == null)
            {
                throw new NotFoundException(nameof(Product), request.ProductId);
            }

            return _mapper.Map<GetProductQueryResponse>(product);
        }
    }

    public class GetProductQueryResponse
    {
        public int ProductId { get; set; }
        public string Description { get; set; } = default!;
        public double Price { get; set; }
    }


    public class GetProductQueryProfile : Profile
    {
        public GetProductQueryProfile() => CreateMap<Product, GetProductQueryResponse>();
    }
}
