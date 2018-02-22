using Simulation.Measure;

namespace Simulation.AccountingResults
{
    public interface IAccountingResultsManager
    {
        void WriteDataToDisk(MeasureValueHolder measureValueHolder);
        MeasureValueHolder ReadDataFromDisk(string mainFile);
    }
}
