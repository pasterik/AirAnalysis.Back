using FLS.Rules;
using AirAnalysis.BLL.Services.Fuzzy_LogicService.Model;

namespace AirAnalysis.BLL.Services.Fuzzy_LogicService.Interfaces
{
    public interface IFuzzy_Logic
    {
        List<FuzzyRule> GetRule();
        double GetResult(PhenomenFuzzy phenomen, List<FuzzyRule> ruleslist);

    }
}
