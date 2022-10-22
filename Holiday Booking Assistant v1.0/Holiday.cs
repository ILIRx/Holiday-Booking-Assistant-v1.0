using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NEA_Project
{
    internal class Holiday
    {
        protected double tropicalLevel, urbanLevel, adventureLevel, nightlifeLevel, temperatureLevel, pricePP;
        protected double sumOfAttributes;
        protected string HolidayName, ClimateCode, departureDate, returnDate, transportationType;
        protected int HID;
        protected static Random ran = new Random();
        public Holiday()
        {
            string[] NamesAndCodes = randomName();
            HolidayName = NamesAndCodes[0];
            ClimateCode = NamesAndCodes[1];

            string[] departureAndReturnDates = departureAndReturnDate();
            departureDate = departureAndReturnDates[0];
            returnDate = departureAndReturnDates[1];

            transportationType = transportType();

            generateAttributes(ClimateCode);

            sumOfAttributes = tropicalLevel + urbanLevel + adventureLevel + nightlifeLevel + temperatureLevel;
            if (sumOfAttributes > 10 && sumOfAttributes < 26)
            {
                pricePP = ran.Next(200, 501);
            }
            else if (sumOfAttributes > 25 && sumOfAttributes < 41)
            {
                pricePP = ran.Next(500, 701);
            }
            else if (sumOfAttributes > 40 && sumOfAttributes < 51)
            {
                pricePP = ran.Next(700, 2001);
            }
            else pricePP = ran.Next(50, 201);
        }
        public Holiday(int HID, double tropicalLevel, double urbanLevel, double adventureLevel, double nightlifeLevel, double temperatureLevel, double pricePP, double sumOfAttributes, string holidayName, string departureDate, string returnDate, string transportationType)
        {
            this.HID = HID;
            this.tropicalLevel = tropicalLevel;
            this.urbanLevel = urbanLevel;
            this.adventureLevel = adventureLevel;
            this.nightlifeLevel = nightlifeLevel;
            this.temperatureLevel = temperatureLevel;
            this.pricePP = pricePP;
            this.sumOfAttributes = sumOfAttributes;
            HolidayName = holidayName;
            this.departureDate = departureDate;
            this.returnDate = returnDate;
            this.transportationType = transportationType;
        }
        public int getHID()
        {
            return HID;
        }
        private string[] randomName()
        {

            string fileName = "HolidayNames.txt";
            using (StreamReader sr = new StreamReader(fileName))
            {
                string[] line = new string[2];
                int i = 1;
                int lineNum = ran.Next(1, 164);
                while (sr.EndOfStream == false && i != lineNum)
                {
                    line = sr.ReadLine().Split(' ');
                    i++;
                }
                return line;
            }
        }
        private string transportType()
        {
            string[] transportationTypes = { "Coach", "Plane", "Ferry", "Cruise Ship" };

            return transportationTypes[ran.Next(0, transportationTypes.Length)];
        }
        private string[] departureAndReturnDate()
        {
            string[] departureAndReturnDates = new string[2];
            int day = ran.Next(1, 29);
            int month = ran.Next(1, 12);
            int year = ran.Next(2023, 2025);

            string departureDate = $"{day.ToString()}/{month.ToString()}/{year.ToString()}";

            string returnDate = $"{ran.Next(day + 1, 29).ToString()}/{ran.Next(month, month + 1)}/{year.ToString()}";

            departureAndReturnDates[0] = departureDate;
            departureAndReturnDates[1] = returnDate;

            return departureAndReturnDates;

        }
        private void generateAttributes(string ClimateCode)
        {
            //DSB = Continental Dry Summer Warm Summer
            //CSB = Temperate Dry Summer Warm Summer
            //BWH = Arid Desert Hot
            //AW = Tropical Savanna, Dry winter
            //CFB = Temperate No dry season Warm Summer
            //AF = Tropical Rainforest
            //CFA = Temperate No Dry Season Hot Summer
            //DFB = Continental No Dry season Warm Summer
            //BSK = Arid Steppe Cold
            //AM = Tropical Monsoon
            //CWB = Temperate Dry Winter Warm Summer
            //BS = Arid Steppe
            //BSH = Arid Steppe Hot
            //DFC = Continental No Dry season Cold Summer
            //CWA = Temperate Dry Winter Hot Summer
            //ET = Polar Tundra
            //CFC = Temperate No Dry Season Cold Summer
            //CSA = Temperate Dry Summer Hot Summer
            //EF = Polar Eternal frost (ice cap)
            //BWK = Arid Desert Cold
            //DWB = Continental Dry Winter Warm Summer
            //AS = Tropical Savanna (Dry summer)
            //DSC = Continental Dry Summer Cold Summer


            //I used https://en.wikipedia.org/wiki/K%C3%B6ppen_climate_classification#Group_D:_Continental/microthermal_climates
            //To help me determine the meanings of the Climate Zone Codes and to give me more knowledge to make
            //an educated guess on what sort of attributes each zone would have

            double[] attributes = new double[5];

            switch (ClimateCode)
            {
                case "DSB":
                    tropicalLevel = ran.Next(1, 4);
                    urbanLevel = ran.Next(4, 9);
                    adventureLevel = ran.Next(3, 6);
                    nightlifeLevel = ran.Next(6, 9);
                    temperatureLevel = ran.Next(3, 7);
                    break;
                case "CSB":
                    tropicalLevel = ran.Next(6, 10);
                    urbanLevel = ran.Next(5, 9);
                    adventureLevel = ran.Next(6, 11);
                    nightlifeLevel = ran.Next(8, 11);
                    temperatureLevel = ran.Next(5, 10);
                    break;
                case "BWH":
                    tropicalLevel = ran.Next(6, 10);
                    urbanLevel = ran.Next(6, 11);
                    adventureLevel = ran.Next(3, 10);
                    nightlifeLevel = ran.Next(3, 8);
                    temperatureLevel = ran.Next(7, 11);
                    break;
                case "AW":
                    tropicalLevel = ran.Next(8, 11);
                    urbanLevel = ran.Next(6, 10);
                    adventureLevel = ran.Next(8, 11);
                    nightlifeLevel = ran.Next(9, 11);
                    temperatureLevel = ran.Next(6, 10);
                    break;
                case "CFB":
                    tropicalLevel = ran.Next(3, 8);
                    urbanLevel = ran.Next(7, 11);
                    adventureLevel = ran.Next(3, 7);
                    nightlifeLevel = ran.Next(6, 11);
                    temperatureLevel = ran.Next(3, 7);
                    break;
                case "AF":
                    tropicalLevel = ran.Next(7, 11);
                    urbanLevel = ran.Next(6, 10);
                    adventureLevel = ran.Next(8, 11);
                    nightlifeLevel = ran.Next(9, 11);
                    temperatureLevel = ran.Next(3, 7);
                    break;
                case "CFA":
                    tropicalLevel = ran.Next(5, 9);
                    urbanLevel = ran.Next(7, 11);
                    adventureLevel = ran.Next(7, 10);
                    nightlifeLevel = ran.Next(9, 11);
                    temperatureLevel = ran.Next(6, 10);
                    break;
                case "DFB":
                    tropicalLevel = ran.Next(1, 3);
                    urbanLevel = ran.Next(8, 11);
                    adventureLevel = ran.Next(5, 9);
                    nightlifeLevel = ran.Next(5, 10);
                    temperatureLevel = ran.Next(3, 7);
                    break;
                case "BSK":
                    tropicalLevel = ran.Next(4, 10);
                    urbanLevel = ran.Next(5, 9);
                    adventureLevel = ran.Next(7, 11);
                    nightlifeLevel = ran.Next(3, 9);
                    temperatureLevel = ran.Next(2, 11);
                    break;
                case "AM":
                    tropicalLevel = ran.Next(5, 9);
                    urbanLevel = ran.Next(8, 11);
                    adventureLevel = ran.Next(4, 8);
                    nightlifeLevel = ran.Next(8, 11);
                    temperatureLevel = ran.Next(7, 10);
                    break;
                case "CWB":
                    tropicalLevel = ran.Next(6, 9);
                    urbanLevel = ran.Next(5, 10);
                    adventureLevel = ran.Next(3, 7);
                    nightlifeLevel = ran.Next(4, 9);
                    temperatureLevel = ran.Next(3, 9);
                    break;
                case "BS":
                    tropicalLevel = ran.Next(4, 10);
                    urbanLevel = ran.Next(5, 9);
                    adventureLevel = ran.Next(7, 11);
                    nightlifeLevel = ran.Next(3, 9);
                    temperatureLevel = ran.Next(1, 11);
                    break;
                case "BSH":
                    tropicalLevel = ran.Next(2, 6);
                    urbanLevel = ran.Next(5, 10);
                    adventureLevel = ran.Next(3, 9);
                    nightlifeLevel = ran.Next(4, 9);
                    temperatureLevel = ran.Next(3, 7);
                    break;
                case "DFC":
                    tropicalLevel = ran.Next(1, 3);
                    urbanLevel = ran.Next(7, 11);
                    adventureLevel = ran.Next(3, 6);
                    nightlifeLevel = ran.Next(3, 8);
                    temperatureLevel = ran.Next(1, 4);
                    break;
                case "CWA":
                    tropicalLevel = ran.Next(1, 6);
                    urbanLevel = ran.Next(9, 11);
                    adventureLevel = ran.Next(3, 6);
                    nightlifeLevel = ran.Next(9, 11);
                    temperatureLevel = ran.Next(4, 9);
                    break;
                case "ET":
                    tropicalLevel = 1;
                    urbanLevel = ran.Next(1, 3);
                    adventureLevel = ran.Next(6, 11);
                    nightlifeLevel = ran.Next(1, 4);
                    temperatureLevel = ran.Next(1, 2);
                    break;
                case "CFC":
                    tropicalLevel = ran.Next(1, 4);
                    urbanLevel = ran.Next(2, 5);
                    adventureLevel = ran.Next(4, 10);
                    nightlifeLevel = ran.Next(1, 3);
                    temperatureLevel = ran.Next(1, 4);
                    break;
                case "CSA":
                    tropicalLevel = ran.Next(3, 10);
                    urbanLevel = ran.Next(6, 10);
                    adventureLevel = ran.Next(5, 10);
                    nightlifeLevel = ran.Next(7, 11);
                    temperatureLevel = ran.Next(4, 9);
                    break;
                case "EF":
                    tropicalLevel = 1;
                    urbanLevel = 1;
                    adventureLevel = ran.Next(5, 11);
                    nightlifeLevel = 1;
                    temperatureLevel = 1;
                    break;
                case "BWK":
                    tropicalLevel = ran.Next(2, 4);
                    urbanLevel = ran.Next(3, 9);
                    adventureLevel = ran.Next(5, 10);
                    nightlifeLevel = ran.Next(3, 8);
                    temperatureLevel = ran.Next(3, 7);
                    break;
                case "DWB":
                    tropicalLevel = ran.Next(1, 3);
                    urbanLevel = ran.Next(1, 11);
                    adventureLevel = ran.Next(1, 3);
                    nightlifeLevel = ran.Next(1, 8);
                    temperatureLevel = ran.Next(1, 3);
                    break;
                case "AS":
                    tropicalLevel = ran.Next(9, 11);
                    urbanLevel = ran.Next(7, 10);
                    adventureLevel = ran.Next(7, 11);
                    nightlifeLevel = ran.Next(9, 11);
                    temperatureLevel = ran.Next(8, 11);
                    break;
                case "DSC":
                    tropicalLevel = ran.Next(1, 2);
                    urbanLevel = ran.Next(5, 9);
                    adventureLevel = ran.Next(4, 6);
                    nightlifeLevel = ran.Next(3, 7);
                    temperatureLevel = ran.Next(1, 3);
                    break;
                default:
                    break;
            }
        }
        public double getSumOfAttributes()
        {
            return sumOfAttributes;
        }
        public double getTropical()
        {
            return tropicalLevel;
        }
        public double getUrban()
        {
            return urbanLevel;
        }
        public double getAdventure()
        {
            return adventureLevel;
        }
        public double getNightlife()
        {
            return nightlifeLevel;
        }
        public double getTemperature()
        {
            return temperatureLevel;
        }
        public double getPricePP()
        {
            return pricePP;
        }
        public string getName()
        {
            return HolidayName;
        }
        public string getTransportType()
        {
            return transportationType;
        }
        public string getDepartureDate()
        {
            return departureDate;
        }
        public string getReturnDate()
        {
            return returnDate;
        }
    }
}
