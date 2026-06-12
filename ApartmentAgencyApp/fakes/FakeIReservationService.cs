using ApartmentAgencyApp.Models;
using ApartmentAgencyApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentAgencyApp.Fakes
{
    public class FakeIReservationService : IReservationService
    {
        public List<Reservation> Reservations { get; } = new List<Reservation>();

        public void MakeReservationInComplex(Reservation reservation)
        {
            Reservations.Add(reservation);
        }
    }
}