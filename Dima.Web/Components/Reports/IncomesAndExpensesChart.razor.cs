using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports;

public partial class IncomesAndExpensesComponent : ComponentBase
{

    #region Properties

    public ChartOptions Options { get; set; } = new();
    public List<ChartSeries<double>>? Series { get; set; }
    public List<string> Labels { get; set; } = [];

    #endregion
    
    #region Services

    [Inject] public IReportHandler Handler { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    #endregion
    
    #region Overrides

    protected async override Task OnInitializedAsync()
    {
        var request = new GetIncomesAndExpensesRequest();
        var result = await Handler.GetIncomesAndExpensesReportAsync(request);

        if (!result.IsSuccess
            || result.Data is null)
        {
            Snackbar.Add("Não foi possível obter Entrada/Saída", Severity.Error);
            return;
        }

        var incomes = new List<double>();
        var expenses = new List<double>();

        foreach (var item in result.Data)
        {
            incomes.Add((double) item.Incomes);
            expenses.Add(-(double) item.Expenses);
            Labels.Add(GetMonthName(item.Month));
        }

        Options.ChartPalette = ["76FF01", Colors.Red.Default];

        Series =
        [
            new ChartSeries<double>() {Name = "Receitas", Data = incomes.ToArray()},
            new ChartSeries<double>() {Name = "Despesas", Data = expenses.ToArray()}
        ];
        
        StateHasChanged();
    }

    #endregion

    private static string GetMonthName(int month)
        => new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM");
}