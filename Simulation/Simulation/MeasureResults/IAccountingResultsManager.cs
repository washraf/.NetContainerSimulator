using Simulation.Measure;

namespace Simulation.AccountingResults
{
    public interface IAccountingResultsManager
    {
        void WriteDataToDisk(MeasureValueHolder measureValueHolder, int trialNo);
        MeasureValueHolder ReadDataFromDisk(string mainFile);
    }
}
