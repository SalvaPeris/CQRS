using MediatR;
using PruebaCQRS.Domain;
using PruebaCQRS.Infrastructure.Persistence;

namespace PruebaCQRS.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public int ProductId { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {

        private readonly MyAppDbContext _context;

        public DeleteProductCommandHandler(MyAppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken token)
        {

            var product = await _context.Products.FindAsync(request.ProductId);

            if (product != null)
                _context.Products.Remove(product); await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}