using FluentValidation;
using MediatR;
using ApplicationCore.Common.Domain;
using ApplicationCore.Infrastructure.Persistence;

namespace ApplicationCore.Features.Products.Commands
{
    public class CreateProductCommand : IRequest
    {
        public string Description { get; set; } = default!;
        public double Price { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {

        private readonly MyAppDbContext _context;

        public CreateProductCommandHandler(MyAppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken token)
        {
            Product newProduct = new Product
            {
                Price = request.Price,
                Description = request.Description,
            };

            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Price).NotNull().GreaterThan(0);
        }
    }
}
