namespace AirAnalysis.BLL.Services.Fuzzy_LogicService.Model { 
    public class PhenomenFuzzy
    {
        private double? _PM1;
        private double? _PM2_5;
        private double? _PM10;
        private double? _CO;
        private double? _O3;
        private double? _NO2;
        private double? _SO2;

        public double PM1
        {
            get => _PM1 ?? 1;
            set => _PM1 = value;
        }

        public double PM2_5
        {
            get => _PM2_5 ?? 1;
            set => _PM2_5 = value;
        }

        public double PM10
        {
            get => _PM10 ?? 1;
            set => _PM10 = value;
        }

        public double CO
        {
            get => _CO ?? 1;
            set => _CO = value;
        }

        public double O3
        {
            get => _O3 ?? 1;
            set => _O3 = value;
        }

        public double NO2
        {
            get => _NO2 ?? 1;
            set => _NO2 = value;
        }

        public double SO2
        {
            get => _SO2 ?? 1;
            set => _SO2 = value;
        }
    }
}

