using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports;

public partial class ExpensesByCategoryChartComponent : ComponentBase
{
    #region Properties

    public List<double> Data { get; set; } = [];
    public List<ChartSeries<double>> Series { get; set; } = [];
    public List<string> Labels { get; set; } = [];

    #endregion
    
    #region Services
    [Inject]
    public IReportHandler Handler { get; set; } = null!;
    
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Overrides

    protected async override Task OnInitializedAsync()
        => await GetExpensesByCategoryAsync();
    

    #endregion
    
    #region PrivateMethods

    private async Task GetExpensesByCategoryAsync()
    {
        var request = new GetExpensesByCategoryRequest();
        var result = await Handler.GetExpensesByCategoryReportAsync(request);

        if (!result.IsSuccess
            || result.Data is null)
        {
            Snackbar.Add("Falha ao obter dados de Gastos!", Severity.Error);
            return;
        }
        
        Labels.Clear();
        Series.Clear();

        var data = new List<double>();

        foreach (var item in result.Data)
        {
            Labels.Add(item.Category);
            data.Add(-(double)item.Expenses);
        }

        Series.Add(new ChartSeries<double>
        {
            Name = "Saídas",
            Data = data.ToArray()
        });
        
    }
    
    #endregion
}