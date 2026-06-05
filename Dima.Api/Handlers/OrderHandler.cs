using Dima.Api.Data;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class OrderHandler(
    AppDbContext context,
    IStripeHandler stripeHandler) : IOrderHandler
{
    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        Order? order;

        try
        {
            order = await context.Orders
                .Include(o => o.Product)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(o => o.Id == request.Id
                    && o.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>
                    (null, 404, "Pedido não encontrado!");
            
        }
        catch
        {
            return new Response<Order?>
                (null, 500, "Falha ao encontrar o pedido!");
        }

        switch (order.Status)
        {
            case EOrderStatus.Cancelled: 
                return new Response<Order?>
                    (order, 400, "Este pedido já foi cancelado!");
            
            case EOrderStatus.WaitingPayment: 
                break;
            
            case EOrderStatus.Paid: 
                return new Response<Order?>
                    (order, 400, "Este pedido já foi pago e não pode ser cancelado!");
            
            case EOrderStatus.Refunded: 
                return new Response<Order?>
                    (order, 400, "Este pedido já foi estornado e não pode ser cancelado!");
            
            default:
                return new Response<Order?>
                    (order, 400, "Este pedido não pode ser cancelado!");
        }
        
        order.Status = EOrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Não foi possível cancelar o pedido!");
        }
        
        return new Response<Order?>(order, 200, $"Pedido {order.Number} cancelado com sucesso!");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        Product? product;

        try
        {
            product = await context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId
                    && p.IsActive);

            if (product is null)
                return new Response<Order?>
                    (null, 400, "Produto não encontrado!");
            
            //anexar um produto que já foi pego no bd
            context.Attach(product);
        }
        catch
        {
            return new Response<Order?>
                (null, 500, "Não foi possível obter o produto!");
        }
        
        Voucher? voucher = null;

        try
        {

            if (request.VoucherId is not null)
            {
                voucher = await context.Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == request.VoucherId
                                              && v.IsActive);
                
                if(voucher is null)
                    return new Response<Order?>
                        (null, 400, "Voucher inválido ou não encontrado!");

                if (voucher.IsActive == false)
                    return new Response<Order?>
                        (null, 400, "Voucher já utilizado");

                voucher.IsActive = false;
                context.Vouchers.Update(voucher);
            }
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao obter o Voucher!");
        }

        var order = new Order
        {
            UserId = request.UserId,
            Product = product,
            ProductId = request.ProductId,
            Voucher = voucher,
            VoucherId = request.VoucherId,
        };

        try
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>
                (null, 500, "Não foi possível realizar seu pedido!");
        }
        
        return new Response<Order?>(order, 201, $"Pedido {order.Number} realizado com sucesso!");
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        Order? order;

        try
        {
            order = await context.Orders
                .Include(o => o.Product)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(o => o.Number == request.Number
                                          && o.UserId == request.UserId);
            
            if(order is null)
                return new Response<Order?>
                    (null, 404, "Pedido não encontrado!");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao obter o Pedido!");
        }

        switch (order.Status)
        {
            case EOrderStatus.Cancelled:
                return new Response<Order?>
                    (order, 400, "Esse pedido já foi cancelado e não pode ser pago!");
            
            case EOrderStatus.Paid:
                return new Response<Order?>
                    (order, 400, "Esse pedido já foi pago!");
            
            case EOrderStatus.Refunded:
                return new Response<Order?>
                    (order, 400, "Esse pedido já foi estornado e não pode ser pago!");
            
            case EOrderStatus.WaitingPayment:
                break;
            
            default:
                return new Response<Order?>
                    (order, 400, "Não foi possível realizar seu pedido!");
        }
            
        //stripe
        try
        {
            var getTransactionsRequest = new GetTransactionsByOrderNumberRequest
            {
                Number = order.Number
            };
            
            var result = await stripeHandler.GetTransactionsByOrderNumberAsync(getTransactionsRequest);

            if (!result.IsSuccess || result.Data is null)
                return new Response<Order?>(null, 500, "Não foi possível localizar pagamento!");
            
            if(result.Data.Any(x => x.Refunded))
                return new Response<Order?>(null, 400, "Este pedido já teve o pagamento informado!");
            
            if(!result.Data.Any(x => x.Paid))
                return new Response<Order?>(null, 400, "Este pedido não foi pago!");

            request.ExternalReference = result.Data[0].Id;

        }
        catch
        {
            return new Response<Order?>(null, 400, "Não foi possível dar baixa no seu pedido!");

        }
        
        order.Status = EOrderStatus.Paid;
        order.ExternalReference = request.ExternalReference;
        order.UpdatedAt = DateTime.UtcNow;

        try
        { 
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Falha ao pagar o pedido!");
        }
        
        return new Response<Order?>(order, 200, $"Pedido {order.Number} pago com sucesso!");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        Order? order;

        try
        {
            order = await context.Orders
                .Include(o => o.Product)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(o => o.Id == request.Id
                    &&  o.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>
                    (null, 404, "Pedido não encontrado!");
        }
        catch
        {
            return new Response<Order?>
                (null, 500, "Falha ao obter o Pedido!");
        }

        switch (order.Status)
        {
            case EOrderStatus.Cancelled:
                return new Response<Order?>
                    (order, 400, "Esse pedido já foi cancelado e não pode ser estornado!");
            
            case EOrderStatus.Paid:
                break;
            
            case EOrderStatus.Refunded:
                return new Response<Order?>
                    (order, 400, "Esse pedido já foi estornado!");
            
            case EOrderStatus.WaitingPayment:
                return new Response<Order?>
                    (order, 400, "Esse pedido não foi pago e não pode ser estornado!");
            
            default:
                return new Response<Order?>
                    (order, 400, "Não foi possível estornar seu pedido!");

        }

        order.Status = EOrderStatus.Refunded;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>
                (order, 500, "Falha ao estornar o Pedido!");
        }
        
        return new Response<Order?>
            (order, 200, $"Pedido {order.Number} estornado com sucesso!");
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
    {
        try
        {
            var query = context.Orders
                .AsQueryable()
                .Include(o => o.Product)
                .Include(o => o.Voucher)
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(o => o.UpdatedAt);
            
            var orders = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Order>?>
                (orders, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Order>?>
                (null, 500, "Não foi possível obter seus pedidos!");
        }
    }

    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
    {
        try
        {
            var order = await context.Orders
                .AsQueryable()
                .Include(o => o.Product)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(o => o.Number == request.Number
                    && o.UserId == request.UserId);
            
            return order is null
                ? new Response<Order?>(null, 404, "Pedido não encontrado!")
                : new Response<Order?>(order);
        }
        catch
        {
            return new Response<Order?>
                (null, 500, "Não foi possível obter seu pedido!");
        }
    }
}