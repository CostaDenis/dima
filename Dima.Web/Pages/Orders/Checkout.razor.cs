using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class CheckoutPage : ComponentBase
{

    #region Parameters

    [Parameter] public string ProductSlug { get; set; } = string.Empty;
    [SupplyParameterFromQuery(Name = "voucher")] public string? VoucherNumber { get; set; }

    #endregion

    #region Properties
    
    //Recurso do Mud para mascaras
    public PatternMask Mask = new("####-####")
    {
        //Permite caracteres de 0 a 9, e fA até F
        MaskChars = [new MaskChar('#', @"[0-9a-fA-F]")],
        Placeholder = '_',
        CleanDelimiters = true, //tira os caracteres n permitidos pelo regex
        Transformation = AllUperCase
    };
    public bool IsBusy { get; set; }
    protected bool IsValid { get; set; }
    public CreateOrderRequest InputModel { get; set; } = new();
    protected Product? Product { get; set; }
    protected Voucher? Voucher { get; set; }
    protected decimal Total { get; set; }

    #endregion

    #region Services

    [Inject] public IProductHandler ProductHandler { get; set; } = null!;
    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject] public IVoucherHandler VoucherHandler { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await ProductHandler.GetBySlugAsync(
                new GetProductBySlugRequest
                {
                    Slug = ProductSlug
                });

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível obter o produto", Severity.Error);
                IsValid = false;
                return;
            }
            
            Product = result.Data;
            
        }
        catch
        {
            Snackbar.Add("Não foi possível obter o produto", Severity.Error);
            IsValid = false;
        }

        if (Product == null)
        {
            Snackbar.Add("Não foi possível obter o produto", Severity.Error);
            IsValid = false;
        }

        if (!string.IsNullOrEmpty(VoucherNumber))
        {
            try
            {
                var result = await VoucherHandler.GetByNumberAsync(
                    new GetVoucherByNumberRequest
                    {
                        Number = VoucherNumber.Replace("-", "")
                    });

                if (!result.IsSuccess)
                {
                    VoucherNumber = string.Empty;
                    Snackbar.Add("Não foi possível obter o voucher!", Severity.Error);
                }

                if (result.Data == null)
                {
                    VoucherNumber = string.Empty;
                    Snackbar.Add("Não foi possível obter o voucher!", Severity.Error);
                }
                
                Voucher = result.Data;
            }
            catch
            {
                VoucherNumber = string.Empty;
                Snackbar.Add("Não foi possível obter o voucher!", Severity.Error);
            }
        }
        
        IsValid = true;
        Total = Product!.Price - (Voucher?.Amount ?? 0);
    }

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            var request = new CreateOrderRequest
            {
                ProductId = Product!.Id,
                VoucherId = Voucher?.Id ?? null,
            };
            
            var result = await OrderHandler.CreateAsync(request);

            if (result.IsSuccess)
                NavigationManager.NavigateTo($"/pedidos/{result.Data!.Number}");
            else
                Snackbar.Add(result.Message ?? "Erro inesperado", Severity.Error);
            
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);

        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
    
    #region PrivateMethods

    private static char AllUperCase(char c)
        => c.ToString().ToUpperInvariant()[0];

    #endregion
}