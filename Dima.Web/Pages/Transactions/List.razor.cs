using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions;

public partial class ListTransactionPage : ComponentBase
{
    #region MyRegion

    public bool IsBusy { get; set; } = false;
    public List<Transaction> Transactions { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    public int CurrentYear { get; set; } = DateTime.Now.Year;
    public int CurrentMonth { get; set; } = DateTime.Now.Month;

    public int[] Years { get; set; } =
    [
        DateTime.Now.Year,
        DateTime.Now.AddYears(-1).Year,
        DateTime.Now.AddYears(-2).Year,
        DateTime.Now.AddYears(-3).Year
    ];

    #endregion

    #region Services

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ITransactionHandler TransactionHandler { get; set; } = null!;

    #endregion

    #region Private Methods

    private async Task GetTransactionsAsync()
    {
        IsBusy = true;

        try
        {
            var request = new GetTransactionsByPeriodRequest
            {
                StartDate = DateTime.Now.GetFirstDay(CurrentYear, CurrentMonth),
                EndDate = DateTime.Now.GetLastDay(CurrentYear, CurrentMonth),
                PageNumber = 1,
                PageSize = 10
            };

            var result = await TransactionHandler.GetByPeriodAsync(request);

            if (result.IsSuccess)
                Transactions = result.Data ?? [];

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
    
    private async Task OnDeleteAsync(long id, string title)
    {
        IsBusy = true;

        try
        {
            var result = await TransactionHandler.DeleteAsync(new DeleteTransactionRequest
            {
                Id = id
            });

            if (result.IsSuccess)
            {
                Snackbar.Add($"Lançamento {title} removido!", Severity.Success);
                Transactions.RemoveAll(t => t.Id == id);
            }
            else
                Snackbar.Add(result.Message ?? "Erro ao excluir a transação!", Severity.Error);
            
        }catch(Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
        => await GetTransactionsAsync();

    #endregion

    #region Public Methods

    public Func<Transaction, bool> Filter => transaction =>
    {
        if (string.IsNullOrEmpty(SearchTerm))
            return true;

        return transaction.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
               || transaction.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
    };

    public async void OnDeleteButtonClickedAsync(long id, string title)
    {
        var result = await DialogService.ShowMessageBoxAsync(
            "Atenção",
            $"Ao prosseguir, o lançamento {title} será EXCLUÍDO. Essa ação é irreversível! Deseja continuar?",
            yesText: "Excluir",
            cancelText: "Cancelar");

        if (result is true)
            await OnDeleteAsync(id, title);
        
        StateHasChanged();
    }

    public async Task OnSearchAsync()
    {
        await GetTransactionsAsync();
        StateHasChanged();
    }

    #endregion

}