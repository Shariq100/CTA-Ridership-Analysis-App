//
// N-tier C# and SQL program to analyze CTA Ridership data.
//
// <<SYED SHARIQ UR RAHMAN>>
// U. of Illinois, Chicago
// CS341, Fall2017
// Project #08
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data.SqlClient;

namespace CTA
{

  public partial class Form1 : Form
  {
    private string BuildConnectionString()
    {
      string version = "MSSQLLocalDB";
      string filename = this.txtDatabaseFilename.Text;

      string connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", version, filename);

      return connectionInfo;
    }

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //
      // setup GUI:
      //
      this.lstStations.Items.Add("");
      this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
      this.lstStations.Items.Add("");

      this.lstStations.ClearSelected();

      toolStripStatusLabel1.Text = string.Format("Number of stations:  0");

                string filename = this.txtDatabaseFilename.Text;
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);
                bizTier.TestConnection();

    }


    //
    // File>>Exit:
    //
    private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      this.Close();
    }


    //
    // File>>Load Stations:
    //
    private void toolStripMenuItem2_Click(object sender, EventArgs e)
    {
      //
      // clear the UI of any current results:
      //
      ClearStationUI(true /*clear stations*/);

        string filename = this.txtDatabaseFilename.Text;

        var bizTier = new BusinessTier.Business(filename);

        var stations = bizTier.GetStations();

        foreach (var s in stations)
        {
            this.lstStations.Items.Add(s.Name);
        }
        int numStations = bizTier.getNumberOfStations();
        toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", numStations);
      }


    //
    // User has clicked on a station for more info:
    //
    private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
    {
        // sometimes this event fires, but nothing is selected...
        if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
           return; 
      
        //
        // clear GUI in case this fails:
        //
        ClearStationUI();

        //
        // now display info about selected station:
        //
        string stationName = this.lstStations.Text;
        stationName = stationName.Replace("'", "''");

        string filename = this.txtDatabaseFilename.Text;
        var bizTier = new BusinessTier.Business(filename);
        var stations = bizTier.GetStations();
        int IDStation = stations[this.lstStations.SelectedIndex].ID;

        //
        //Display Station ID
        //
        string strStationID = Convert.ToString(IDStation); 
        this.txtStationID.Text = strStationID;

        //
        //GET TOTAL RIDERSHIP...
        //
        long totalRidership = bizTier.GetTotalRidership(IDStation);
        this.txtTotalRidership.Text = string.Format("{0:#,##0}", totalRidership);

        //
        //GET AVERAGE RIDERSHIP AT THAT PARTICULAR STATION
        //
        long avgridership = bizTier.getAvgRidership(IDStation);
        this.txtAvgDailyRidership.Text = string.Format("{0:#,##0}/day", avgridership);

        //
        //GET %RIDERSHIP
        //
        long totalRidershipAllStations = bizTier.getTotalRidershipAcrossAllStations();
        double perRidership = ((double)(totalRidership) / totalRidershipAllStations) * 100;
        this.txtPercentRidership.Text = string.Format("{0:0.00}%", perRidership);

        //
        //Ridership for WeekDays
        //
        long weekDays = bizTier.getRidershipOnWeekDays(IDStation);
        this.txtWeekdayRidership.Text = string.Format("{0:#,##0}", weekDays);

        //
        //Ridership for Satuday
        //
        long saturday = bizTier.getRidershipOnSatuday(IDStation);
        this.txtSaturdayRidership.Text = string.Format("{0:#,##0}", saturday);

        //
        //Ridership for Sun/Holiday
        //
        long sunHoliday = bizTier.getRidershipOnSundayHoliday(IDStation);
        this.txtSundayHolidayRidership.Text = string.Format("{0:#,##0}", sunHoliday);

        //
        //GET THE STATIONS USING BUSINESS TIER
        //
        var stops = bizTier.GetStops(IDStation);
        foreach (var s in stops)
        {
            this.lstStops.Items.Add(s.Name);
        }

    }

    private void ClearStationUI(bool clearStatations = false)
    {
      ClearStopUI();

      this.txtTotalRidership.Clear();
      this.txtTotalRidership.Refresh();

      this.txtAvgDailyRidership.Clear();
      this.txtAvgDailyRidership.Refresh();

      this.txtPercentRidership.Clear();
      this.txtPercentRidership.Refresh();

      this.txtStationID.Clear();
      this.txtStationID.Refresh();

      this.txtWeekdayRidership.Clear();
      this.txtWeekdayRidership.Refresh();
      this.txtSaturdayRidership.Clear();
      this.txtSaturdayRidership.Refresh();
      this.txtSundayHolidayRidership.Clear();
      this.txtSundayHolidayRidership.Refresh();

      this.lstStops.Items.Clear();
      this.lstStops.Refresh();

      if (clearStatations)
      {
        this.lstStations.Items.Clear();
        this.lstStations.Refresh();
      }
    }


    //
    // user has clicked on a stop for more info:
    //
    private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
    {
      // sometimes this event fires, but nothing is selected...
      if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
        return; 

      //
      // clear GUI in case this fails:
      //
      ClearStopUI();

      //
      // now display info about this stop:
      //
      string stopName = this.lstStops.Text;
      stopName = stopName.Replace("'", "''");

        string filename = this.txtDatabaseFilename.Text;
        var bizTier = new BusinessTier.Business(filename);
        
        //
        //Handicap Accessible
        //
        bool value = bizTier.handicapAccessible(stopName);
        string strValue = Convert.ToString(value);
        string Accessible;
        if (strValue == "True")
           Accessible = "Yes";
        else
           Accessible = "No";
        this.txtAccessible.Text = Accessible;

        //
        //Direction of Travel
        //
        char direction = bizTier.getDirection(stopName);
        this.txtDirection.Text = Convert.ToString(direction);

        //
        //Location
        //
        string latitude = bizTier.getLatitude(stopName);
        string longitude = bizTier.getLongitude(stopName);
        this.txtLocation.Text = "(" + latitude + ", " + longitude + ")";

        //
        //Lines
        //
        List<String> lines = new List<string>();
        lines = bizTier.getLines(stopName);

        foreach(var line in lines)
        {
            this.lstLines.Items.Add(line);
        }

    }

    private void ClearStopUI()
    {
      this.txtAccessible.Clear();
      this.txtAccessible.Refresh();

      this.txtDirection.Clear();
      this.txtDirection.Refresh();

      this.txtLocation.Clear();
      this.txtLocation.Refresh();

      this.lstLines.Items.Clear();
      this.lstLines.Refresh();
    }


    //
    // Top-10 stations in terms of ridership:
    //
    private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //
        // clear the UI of any current results:
        //
        ClearStationUI(true /*clear stations*/);

        //
        // now load top-10 stations:
        //
        //TOP 10 Stations using business tier
        string filename = this.txtDatabaseFilename.Text;

         var bizTier = new BusinessTier.Business(filename);

         var stations = bizTier.GetTopStations(10);
         int numStations = 0;
         foreach (var s in stations)
         {
            this.lstStations.Items.Add(s.Name);
            numStations++;
         }
        toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", numStations);
    }
  }//class
}//namespace
