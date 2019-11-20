﻿using System.Linq;
using System.Windows;

namespace Assessment2
{
    /// <summary>
    /// INTERACTION LOGIC FOR FORM_VEHICLEHISTORY.XAML
    /// </summary>
    public partial class Form_VehicleHistory : Window
    {
        /// <summary>
        /// CONSTRUCTOR - POPULATE THE VEHICLE INFORMATION AND THE LIST VIEW WITH ALL PREVIOUS ENTRIES
        /// </summary>
        /// <param name="v"></param>
        public Form_VehicleHistory(Vehicle v)
        {
            InitializeComponent();

            txtManufacturer.Text = v.Manufacturer;
            txtModel.Text = v.Model;
            txtRegistration.Text = v.RegistrationNumber;
            txtYear.Text = v.MakeYear.ToString();
            txtOdometer.Text = v.OdometerReading.ToString();
            txtTank.Text = v.TankCapacity.ToString();

            lvRentHistory.ItemsSource = Rental.rentalList.Where(x => x.vehicleId == v.Id && x.totalPrice > 0);
            lvRentHistory.Items.Refresh();

            lvServiceHistory.ItemsSource = Service.serviceList.Where(x => x.vehicleId == v.Id);
            lvServiceHistory.Items.Refresh();

            lvFuelHistory.ItemsSource = FuelPurchase.fuelList.Where(x => x.vehicleId == v.Id);
            lvFuelHistory.Items.Refresh();
        }
    }
}
