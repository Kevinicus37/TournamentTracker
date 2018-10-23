using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PrizeModel
    {
        /// <summary>
        /// The unique identifier for the Prize
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the place position this prize is awarded to.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// A title for the place position.
        /// Example: Place 1 - Champion,
        /// Place 2 - Runner-Up
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// Represents the amount awarded for this prize
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// Represents the percentage of the total tournament awards that
        /// go to this particular prize
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }

        public PrizeModel(string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;

        }
    }

    
}
