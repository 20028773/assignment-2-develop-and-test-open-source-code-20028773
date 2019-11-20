﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assessment2
{
    /// <summary>
    /// CLASS THAT HANDLES THE RENTAL'S OPERATIONS
    /// </summary>
    public class Rental
    {
        /// <summary>
        /// RENTAL MAIN PROPERTIES
        /// </summary>
        public int Id { get; set; }
        public int vehicleId { get; set; }
        public string customerName { get; set; }
        public type rentType { get; set; }
        public double startOdometer { get; set; }
        public double endOdometer { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string notes { get; set; }
        public double totalPrice { get; set; }
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// RETURN VEHICLE DESCRIPTION
        /// </summary>
        [JsonIgnore]
        public string vehicleDescription
        {
            get
            {
                var sAux = "";

                if (Vehicle.vehicleList != null)
                {
                    sAux = Vehicle.vehicleList.Where(x => x.Id == vehicleId).Select(f => f.vehicleDescription).FirstOrDefault();
                }

                return sAux;
            }
        }

        /// <summary>
        /// RETURN HOW MANY KM THE VEHICLE TRAVELLED
        /// </summary>
        [JsonIgnore]
        public double travelledDistance
        {
            get
            {
                return (endOdometer - startOdometer);
            }
        }
        /// <summary>
        /// MAIN RENTAL LIST - GET IT FROM THE FILE
        /// </summary>
        private static List<Rental> _rentalList { get { return JsonData.Load<Rental>(); } }
        /// <summary>
        /// MAIN RENTAL LIST - PUBLIC
        /// </summary>
        [JsonIgnore]
        public static List<Rental> rentalList { get { return _rentalList; } }
        /// <summary>
        /// RENTAY TYPES
        /// </summary>
        public enum type
        {
            Day,
            KM
        }
        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public Rental() { }
        /// <summary>
        /// CONSTRUCTOR THAT SET THE RENTAL PROPERTIES
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vehicleId"></param>
        /// <param name="customerName"></param>
        /// <param name="rentType"></param>
        /// <param name="startOdometer"></param>
        /// <param name="endOdometer"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="notes"></param>
        /// <param name="totalPrice"></param>
        public Rental(int id, int vehicleId, string customerName, type rentType, double startOdometer, double endOdometer, DateTime startDate, DateTime? endDate, string notes, double totalPrice)
        {
            this.Id = id;
            this.vehicleId = vehicleId;
            this.customerName = customerName;
            this.rentType = rentType;
            this.startOdometer = startOdometer;
            this.endOdometer = endOdometer;
            this.startDate = startDate;
            this.endDate = endDate;
            this.notes = notes;
            this.totalPrice = totalPrice;
            this.ModifiedDate = DateTime.Now;
        }
        /// <summary>
        /// CREATE A NEW RENTAL AND ADD TO THE LIST
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="customerName"></param>
        /// <param name="rentType"></param>
        /// <param name="startOdometer"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="notes"></param>
        public static void AddRental(int vehicleId, string customerName, type rentType, double startOdometer, DateTime startDate, DateTime? endDate, string notes)
        {
            List<Rental> rentalList = _rentalList;
            rentalList.Add(new Rental((rentalList.Count > 0 ? rentalList.Last().Id + 1 : 1), vehicleId, customerName, rentType, startOdometer, 0, startDate, endDate, notes, 0));
            JsonData.Save(rentalList);
        }
        /// <summary>
        /// FINALIZE THE RENTAL
        /// </summary>
        /// <param name="rentalId"></param>
        /// <param name="endOdometer"></param>
        /// <param name="endDate"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static string FinalizeRental(int rentalId, double endOdometer, DateTime endDate, string notes)
        {
            List<Rental> rentalList = _rentalList;

            Rental r = rentalList.Where(x => x.Id == rentalId).FirstOrDefault();

            string sMessage = Vehicle.UpdateOdometer(r.vehicleId, endOdometer);

            if (!string.IsNullOrEmpty(sMessage))
            {
                return sMessage;
            }

            var totalPrice = r.rentType == type.KM
                            ? endOdometer - r.startOdometer
                            : ((endDate - r.startDate).Days > 0 ? (endDate - r.startDate).Days : 1) * 100;

            r.endOdometer = endOdometer;
            r.endDate = endDate;
            r.notes = notes;
            r.totalPrice = totalPrice;
            r.ModifiedDate = DateTime.Now;

            rentalList.ToArray().SetValue(r, 0);

            JsonData.Save(rentalList);

            return "";
        }
        /// <summary>
        /// RETURN THE TOTAL AMOUNT THAT THE VEHICLE MADE
        /// </summary>
        /// <param name="vehicheId"></param>
        /// <returns></returns>
        public static double GetTotalRevenue(int vehicheId)
        {
            return rentalList.Where(x => x.vehicleId == vehicheId).Sum(t => t.totalPrice);
        }
        /// <summary>
        /// RETURN A LIST WITH THE VEHICLES THAT CAN BE RENTED
        /// </summary>
        /// <param name="sFilter"></param>
        /// <returns></returns>
        public static List<Vehicle> GetAvailableVehicles(string sFilter = "")
        {
            List<Vehicle> list = Vehicle.vehicleList.Where(x => x.Status == Vehicle.statusType.Available).ToList();

            if (!string.IsNullOrEmpty(sFilter))
            {
                list = list.Where(x => x.Manufacturer.ToUpper().Contains(sFilter.ToUpper())
                                    || x.Model.ToUpper().Contains(sFilter.ToUpper())
                                    || x.RegistrationNumber.ToUpper().Contains(sFilter.ToUpper())).ToList();
            }

            return list;
        }
        /// <summary>
        /// RETURN A RENTAL LIST ACCORDING TO THE FILTERS
        /// </summary>
        /// <param name="sFilter"></param>
        /// <param name="bFinalized"></param>
        /// <returns></returns>
        public static List<Rental> getRentalList(string sFilter = "", bool bFinalized = false)
        {
            List<Rental> list = rentalList;

            if (!string.IsNullOrEmpty(sFilter))
            {
                list = list.Where(x => x.customerName.ToUpper().Contains(sFilter.ToUpper())
                                    || x.vehicleDescription.ToUpper().Contains(sFilter.ToUpper())).ToList();
            }

            if (!bFinalized)
            {
                list = list.Where(x => x.totalPrice == 0).ToList();
            }

            return list;
        }
    }
}
