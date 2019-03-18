//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    ///
    /// <summary>
    /// Constructs a new instance of the business tier.  The format
    /// of the filename should be either |DataDirectory|\filename.mdf,
    /// or a complete Windows pathname.
    /// </summary>
    /// <param name="DatabaseFilename">Name of database file</param>
    /// 
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    ///
    /// <summary>
    ///  Opens and closes a connection to the database, e.g. to
    ///  startup the server and make sure all is well.
    /// </summary>
    /// <returns>true if successful, false if not</returns>
    /// 
    public bool TestConnection()
    {
      return dataTier.OpenCloseConnection();
    }


    ///
    /// <summary>
    /// Returns all the CTA Stations, ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<CTAStation> GetStations()
    {
      List<CTAStation> stations = new List<CTAStation>();

      try
      {

                //
                // TODO!
                //
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = "Select * from Stations Order By Stations.Name";
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    int stationID = Convert.ToInt32(row["StationID"]);  // all rows have same station ID:
                    string stationName = Convert.ToString(row["Name"]);
                    stations.Add(new CTAStation(stationID, stationName));
                }


                return stations;
                //object result = dataTier.ExecuteScalarQuery(sql);

            }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    }

    //********************//**********************//
        
    //*******************//**********************//
    ///
    /// <summary>
    /// Returns the CTA Stops associated with a given station,
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStop objects</returns>
    ///
    public IReadOnlyList<CTAStop> GetStops(int stationID)
    {
      List<CTAStop> stops = new List<CTAStop>();
      try
      {
                //
                // TODO!
                //
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
                SELECT * From Stops
                INNER JOIN Stations ON Stops.StationID = Stations.StationID
                WHERE Stations.StationID = '{0}' 
                ORDER BY Stops.Name ASC;
                ", stationID);

                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    int stopID = Convert.ToInt32(row["StopID"]);
                    string stopName = Convert.ToString(row["Name"]);
                    string direction = Convert.ToString(row["Direction"]);
                    bool ada = Convert.ToBoolean(row["ADA"]);
                    double latitude = Convert.ToDouble(row["Latitude"]);
                    double longitude = Convert.ToDouble(row["Longitude"]);
                    stops.Add(new CTAStop(stopID, stopName, stationID, direction, ada, latitude, longitude));
                }

                return stops;
            }
      catch (Exception ex)
      {
                //foreach(var s in stops)
                //{
                //    Console.Out.WriteLine(s);
                //}
                Console.Out.WriteLine("test 4");
        string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stops;
    }


    ///
    /// <summary>
    /// Returns the top N CTA Stations by ridership, 
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<CTAStation> GetTopStations(int N)
    {
      if (N < 1)
        throw new ArgumentException("GetTopStations: N must be positive");

      List<CTAStation> stations = new List<CTAStation>();

      try
      {

                //
                // TODO!
                //
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"
                SELECT Top 10 Name, Sum(DailyTotal) As TotalRiders
                FROM Riderships
                INNER JOIN Stations ON Riderships.StationID = Stations.StationID 
                GROUP BY Stations.StationID, Name
                ORDER BY TotalRiders DESC;
                ");

                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    int stationID = Convert.ToInt32(row["TotalRiders"]);  // all rows have same station ID:
                    string stationName = Convert.ToString(row["Name"]);
                    stations.Add(new CTAStation(stationID, stationName));
                }

            }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    }


        //
        //Get Number of Stations
        //
        public int getNumberOfStations()
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            //query for Number of Stations
            string sql = string.Format(@"SELECT Count(*) From Stations");

            object result = dataTier.ExecuteScalarQuery(sql);
            int numStations = Convert.ToInt32(result);

            return numStations;
        }

        //
        //Get Total Ridership
        //
        public long GetTotalRidership(int stationID)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            //query for TotalRidership
            string sql = string.Format(@"SELECT SUM(DailyTotal) AS TotalRidership FROM Riderships
                                        INNER JOIN Stations
                                        ON StationS.StationID = Riderships.StationID
                                        WHERE Stations.StationID = '{0}'
                                        ", stationID);

            object result = dataTier.ExecuteScalarQuery(sql);
            long totalRidership = Convert.ToInt64(result);

            return totalRidership;
        }

        //
        //Average Ridership at particular station
        //
        public long getAvgRidership(int stationID)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            //query for AvgRidership
            string sql = string.Format(@"SELECT AVG(DailyTotal) AS AVGRidership FROM Riderships
                                        INNER JOIN Stations
                                        ON StationS.StationID = Riderships.StationID
                                        WHERE Stations.StationID = '{0}'
                                        ", stationID);

            object result = dataTier.ExecuteScalarQuery(sql);
            long avgRidership = Convert.ToInt64(result);

            return avgRidership;

        }

        //
        // '%' Average at particular station
        //
        public long getTotalRidershipAcrossAllStations()
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query for %Ridership
            string sql = string.Format(@"SELECT SUM(Convert(bigInt, DailyTotal)) AS AllRidership FROM Riderships
                                        INNER JOIN Stations
                                        ON StationS.StationID = Riderships.StationID");

            object result = dataTier.ExecuteScalarQuery(sql);
            long allStationTotalRidership = Convert.ToInt64(result);

            return allStationTotalRidership;

        }

        //
        // Ridership for weekdays
        //
        public long getRidershipOnWeekDays(int stationID)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query to get Ridership for weekdays, saturday, sun/holiday
            string sql = string.Format(@"SELECT Sum(DailyTotal) from Riderships
                                          Inner Join Stations
                                          ON stations.StationID = Riderships.StationID
                                          Where Stations.StationID = '{0}' AND 
                                          Riderships.TypeOfDay = 'W'", stationID);

            object result = dataTier.ExecuteScalarQuery(sql);
            long ridership = Convert.ToInt64(result);

            return ridership;

        }

        //
        // Ridership for weekdays
        //
        public long getRidershipOnSatuday(int stationID)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query to get Ridership for weekdays, saturday, sun/holiday
            string sql = string.Format(@"SELECT Sum(DailyTotal) from Riderships
                                          Inner Join Stations
                                          ON stations.StationID = Riderships.StationID
                                          Where Stations.StationID = '{0}' AND 
                                          Riderships.TypeOfDay = 'A'", stationID);

            object result = dataTier.ExecuteScalarQuery(sql);
            long ridership = Convert.ToInt64(result);

            return ridership;

        }

        //
        // Ridership for weekdays
        //
        public long getRidershipOnSundayHoliday(int stationID)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query to get Ridership for weekdays, saturday, sun/holiday
            string sql = string.Format(@"SELECT Sum(DailyTotal) from Riderships
                                          Inner Join Stations
                                          ON stations.StationID = Riderships.StationID
                                          Where Stations.StationID = '{0}' AND 
                                          Riderships.TypeOfDay = 'U'", stationID);

            object result = dataTier.ExecuteScalarQuery(sql);
            long ridership = Convert.ToInt64(result);

            return ridership;

        }

        //
        //Is this stop Handicap Acceisible?
        //
        public Boolean handicapAccessible(string stopName)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query for Handicap Accessible
            string sql = string.Format(@"Select ADA from Stops
                                        Inner Join Stations
                                        ON Stations.StationID = Stops.StationID
                                        WHERE Stops.Name = '{0}'", stopName);

            object result = dataTier.ExecuteScalarQuery(sql);
            bool value = Convert.ToBoolean(result);

            return value;
        }

        //
        //Direction of Travel
        //
        public char getDirection(string stopName)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query for Direction of travel
            string sql = string.Format(@"Select Stops.Direction from Stops
                                          Inner Join Stations
                                          ON Stations.StationID = Stops.StationID
                                          Where Stops.Name = '{0}'", stopName);

            object result = dataTier.ExecuteScalarQuery(sql);
            char direction = Convert.ToChar(result);

            return direction;
        }

        //
        //Location Latitude
        //
        public string getLatitude(string stopName)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query for Location
            string sql = string.Format(@"Select Stops.Latitude from Stops
                                          Inner Join Stations
                                          ON Stations.StationID = Stops.StationID
                                          Where Stops.Name = '{0}'", stopName);

            object result = dataTier.ExecuteScalarQuery(sql);
            string latitude = Convert.ToString(result);
            return latitude;
        }

        //
        //Location Longitude
        //
        public string getLongitude(string stopName)
        {
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query for Location
            string sql = string.Format(@"Select Stops.Longitude from Stops
                                          Inner Join Stations
                                          ON Stations.StationID = Stops.StationID
                                          Where Stops.Name = '{0}'", stopName);

            object result = dataTier.ExecuteScalarQuery(sql);
            string longitude = Convert.ToString(result);
            return longitude;
        }

        //
        //Lines list
        //
        public List<String> getLines(string stopName)
        {
            List<String> lines = new List<string>();
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);

            //query to get lines...
            string sql = string.Format(@"Select Color from Lines
                                        Inner Join StopDetails
                                        On StopDetails.LineID = Lines.LineID
                                        Inner Join Stops
                                        On Stops.StopID = StopDetails.StopID
                                        Where Stops.Name = '{0}'", stopName);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string ld = Convert.ToString(row["Color"]);
                lines.Add(ld);
            }

            return lines;
        }
    }//class
}//namespace
