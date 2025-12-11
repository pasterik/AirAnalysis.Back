using FLS.Rules;
using FLS;
using AirAnalysis.BLL.Services.Fuzzy_LogicService.Interfaces;
using AirAnalysis.BLL.Services.Fuzzy_LogicService.Model;

namespace AirAnalysis.BLL.Services.Fuzzy_LogicService
{
    public class Fuzzy_Logic : IFuzzy_Logic
    {
        public double GetResult(PhenomenFuzzy phenomen, List<FuzzyRule> ruleslist)
        {
            foreach (var property in phenomen.GetType().GetProperties())
            {
                if (property.GetValue(phenomen) is double value && value == 0)
                {
                    property.SetValue(phenomen, 1);
                }
            }
            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            foreach (var rule in ruleslist)
            {
                fuzzyEngine.Rules.Add(rule);
            }

            double result = fuzzyEngine.Defuzzify(phenomen);
            return result;
        }

        public List<FuzzyRule> GetRule() 
        {
            var pm1 = new LinguisticVariable("PM1");
            var low_pm1 = pm1.MembershipFunctions.AddTriangle("Low", 0, 10, 75);
            var medium_pm1 = pm1.MembershipFunctions.AddTriangle("Medium", 10, 75, 250);
            var high_pm1 = pm1.MembershipFunctions.AddTriangle("High", 75, 250, 500);

            var pm2_5 = new LinguisticVariable("PM2_5");
            var low_pm2_5 = pm2_5.MembershipFunctions.AddTriangle("Low", 0, 10, 75);
            var medium_pm2_5 = pm2_5.MembershipFunctions.AddTriangle("Medium", 10, 75, 250);
            var high_pm2_5 = pm2_5.MembershipFunctions.AddTriangle("High", 75, 250, 500);

            var pm10 = new LinguisticVariable("PM10");
            var low_pm10 = pm10.MembershipFunctions.AddTriangle("Low", 0, 30, 80);
            var medium_pm10 = pm10.MembershipFunctions.AddTriangle("Medium", 30, 80, 250);
            var high_pm10 = pm10.MembershipFunctions.AddTriangle("High", 80, 250, 500);

            var co = new LinguisticVariable("CO");
            var low_co = co.MembershipFunctions.AddTriangle("Low", 0, 25, 50);
            var medium_co = co.MembershipFunctions.AddTriangle("Medium", 50, 185, 250);
            var high_co = co.MembershipFunctions.AddTriangle("High", 185, 250, 1000);

            var o3 = new LinguisticVariable("O3");
            var low_o3 = o3.MembershipFunctions.AddTriangle("Low", 0, 25, 60);
            var medium_o3 = o3.MembershipFunctions.AddTriangle("Medium", 60, 185, 250);
            var high_o3 = o3.MembershipFunctions.AddTriangle("High", 185, 250, 500);

            var no2 = new LinguisticVariable("NO2");
            var low_no2 = no2.MembershipFunctions.AddTriangle("Low", 0, 15, 30);
            var medium_no2 = no2.MembershipFunctions.AddTriangle("Medium", 30, 100, 180);
            var high_no2 = no2.MembershipFunctions.AddTriangle("High", 100, 180, 500);

            var so2 = new LinguisticVariable("SO2");
            var low_so2 = so2.MembershipFunctions.AddTriangle("Low", 0, 20, 40);
            var medium_so2 = so2.MembershipFunctions.AddTriangle("Medium", 40, 100, 300);
            var high_so2 = so2.MembershipFunctions.AddTriangle("High", 100, 300, 500);


            var ecol_lev = new LinguisticVariable("EcologicalSafetyLevel");
            var very_good_ecol = ecol_lev.MembershipFunctions.AddTriangle("Very Good", 0, 0, 4);
            var good_ecol = ecol_lev.MembershipFunctions.AddTriangle("Good", 0, 4, 7);
            var normal_ecol = ecol_lev.MembershipFunctions.AddTriangle("Normal", 4, 7, 9);
            var bad_ecol = ecol_lev.MembershipFunctions.AddTriangle("Bad", 7, 9, 10);
            var very_bad_ecol = ecol_lev.MembershipFunctions.AddTriangle("Very Bad", 9, 10, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var ruleslist = new List<FuzzyRule>();

            ruleslist.Add(Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(medium_o3))
                .And(no2.Is(medium_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(high_o3))
                .And(no2.Is(high_no2))
                .And(so2.Is(high_so2))
                ).Then(ecol_lev.Is(very_bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(medium_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(medium_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(good_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(medium_o3))
                .And(no2.Is(medium_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(high_o3))
                .And(no2.Is(high_no2))
                .And(so2.Is(high_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));


            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(high_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(high_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(high_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(high_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2)))
                .Then(ecol_lev.Is(very_good_ecol)));

            ruleslist.Add(Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(high_o3))
                .And(no2.Is(high_no2))
                .And(so2.Is(high_so2)))
                .Then(ecol_lev.Is(very_bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(medium_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(medium_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(high_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(high_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(medium_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(high_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(medium_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(medium_co))
                .And(o3.Is(medium_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(medium_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(normal_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(high_pm1)
                .And(pm2_5.Is(low_pm2_5))
                .And(pm10.Is(medium_pm10))
                .And(co.Is(low_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            ruleslist.Add(
                Rule.If(pm1.Is(low_pm1)
                .And(pm2_5.Is(medium_pm2_5))
                .And(pm10.Is(low_pm10))
                .And(co.Is(high_co))
                .And(o3.Is(low_o3))
                .And(no2.Is(low_no2))
                .And(so2.Is(low_so2))
                ).Then(ecol_lev.Is(bad_ecol)));

            return ruleslist;
        }
    }
}
