using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Analyzers.Bookings.Models {
    public class Extra {
        public Extra() {
            SelectedNights = new List<Night>();
        }
        public string Code;
        public string Name;
        
        public string FixedPrice;
        public string FixedPricePerNight;
        
        public string PricePerUnit;
        public string PricePerUnitPerNight;
        
        public string AdultPrice;
        public string PerAdultPrice;
        public string PerAdultPricePerNight;
        
        public string ChildPrice;
        public string PerChildPrice;
        public string PerChildPricePerNight;

        public string IsPerNight;
        public string IsNightsSelectable;
        public string IsAllowOccupancySelect;
        
        public List<Night> SelectedNights;
    }

    public class Night {
        public string SelectedNight;
    }
}
