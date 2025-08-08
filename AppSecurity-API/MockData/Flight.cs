namespace AppSecurity_API.MockData
{
    public class Flight
    {
        public string FlightNumber { get; set; }
        public string Airline { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public bool IsDirect { get; set; }
    }

    public static class FlightList
    {
        public static List<Flight> GetMockFlights()
        {
            return new List<Flight>
        {
            new Flight
            {
                FlightNumber = "AC101",
                Airline = "Air Canada",
                DepartureCity = "Vancouver",
                ArrivalCity = "Toronto",
                DepartureTime = new DateTime(2025, 9, 15, 8, 0, 0),
                ArrivalTime = new DateTime(2025, 9, 15, 15, 30, 0),
                Price = 450.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "AC102",
                Airline = "Air Canada",
                DepartureCity = "Vancouver",
                ArrivalCity = "Calgary",
                DepartureTime = new DateTime(2025, 9, 16, 10, 30, 0),
                ArrivalTime = new DateTime(2025, 9, 16, 12, 45, 0),
                Price = 320.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "AC103",
                Airline = "Air Canada",
                DepartureCity = "Toronto",
                ArrivalCity = "Montreal",
                DepartureTime = new DateTime(2025, 9, 17, 14, 0, 0),
                ArrivalTime = new DateTime(2025, 9, 17, 15, 20, 0),
                Price = 210.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "WS202",
                Airline = "WestJet",
                DepartureCity = "Calgary",
                ArrivalCity = "Montreal",
                DepartureTime = new DateTime(2025, 9, 16, 9, 45, 0),
                ArrivalTime = new DateTime(2025, 9, 16, 17, 0, 0),
                Price = 390.00m,
                IsDirect = false
            },
            new Flight
            {
                FlightNumber = "WS203",
                Airline = "WestJet",
                DepartureCity = "Edmonton",
                ArrivalCity = "Toronto",
                DepartureTime = new DateTime(2025, 9, 19, 11, 10, 0),
                ArrivalTime = new DateTime(2025, 9, 19, 17, 0, 0),
                Price = 430.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "DL303",
                Airline = "Delta",
                DepartureCity = "Seattle",
                ArrivalCity = "New York",
                DepartureTime = new DateTime(2025, 9, 17, 6, 30, 0),
                ArrivalTime = new DateTime(2025, 9, 17, 14, 15, 0),
                Price = 520.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "DL304",
                Airline = "Delta",
                DepartureCity = "Atlanta",
                ArrivalCity = "Los Angeles",
                DepartureTime = new DateTime(2025, 9, 20, 9, 30, 0),
                ArrivalTime = new DateTime(2025, 9, 20, 12, 50, 0),
                Price = 480.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "UA404",
                Airline = "United Airlines",
                DepartureCity = "Los Angeles",
                ArrivalCity = "Chicago",
                DepartureTime = new DateTime(2025, 9, 18, 10, 15, 0),
                ArrivalTime = new DateTime(2025, 9, 18, 16, 50, 0),
                Price = 470.00m,
                IsDirect = false
            },
            new Flight
            {
                FlightNumber = "UA405",
                Airline = "United Airlines",
                DepartureCity = "Chicago",
                ArrivalCity = "Miami",
                DepartureTime = new DateTime(2025, 9, 21, 13, 0, 0),
                ArrivalTime = new DateTime(2025, 9, 21, 17, 20, 0),
                Price = 400.00m,
                IsDirect = true
            },
            new Flight
            {
                FlightNumber = "SW500",
                Airline = "Southwest",
                DepartureCity = "San Diego",
                ArrivalCity = "Las Vegas",
                DepartureTime = new DateTime(2025, 9, 15, 7, 15, 0),
                ArrivalTime = new DateTime(2025, 9, 15, 8, 45, 0),
                Price = 150.00m,
                IsDirect = true
            },
        };
        }
    }
}
