
using ApartmentAgencyApp.Exceptions;
using ApartmentAgencyApp.Models;
using System.Collections.Generic;

namespace ApartmentAgencyApp.Services
{
    public class ApartmentAgencyService
    {
        private readonly IDateCalculationService _dateCalculationService;
        private readonly IApartmentService _apartmentService;
        private readonly IReservationService _reservatonService;
        private IDateCalculationService calculationService;
        private IApartmentService apartmentService;
        private IReservationService reservationService;

        public ApartmentAgencyService(IDateCalculationService calculationService, IApartmentService apartmentService, IReservationService reservationService)
        {
            this.calculationService = calculationService;
            this.apartmentService = apartmentService;
            this.reservationService = reservationService;
        }

        public void MakeApartmentReservation(ReservationRequest request)
        {
            RequestDaysInfo daysInfo = _dateCalculationService.GetDaysInfo(request.DateOfArrival, request.DateOfDeparture);
            ApartmentComplex complex;

            if (request.ApartmentType.Equals(ApartmentType.BedOnly) && request.DistanceFromTheBeach < 800) 
            {
                if (request.NumberOfBeds > 2) 
                {
                    complex = ApartmentComplex.ComplexA;
                }
                else
                {
                    complex = ApartmentComplex.ComplexB;
                }
            }
            else if (request.ApartmentType.Equals(ApartmentType.Studio)) 
            {
                if (daysInfo.NumberOfDays > 12 || daysInfo.NumberOfSeasonDays >= 10) 
                {
                    complex = ApartmentComplex.ComplexB;
                }
                else
                {
                    complex = ApartmentComplex.ComplexC;
                }
            }
            else
            {
                complex = ApartmentComplex.ComplexD;
            }

            List<Apartment> availableApartments = _apartmentService.GetAvailableApartments(request);
            if(availableApartments.Count == 0)
            {
                throw new NoAvailableApartmentsException("Cannot make a reservation");
            }
            _reservatonService.MakeReservationInComplex(new Reservation { ApartmentId = availableApartments[0].Id, ReservationRequestId = request.Id, ApartmentComplex = complex });
        }

        public ApartmentRank CalculateApartmentRank(double distanceFromTheBeach, int percentOfPositiveReviews, ApartmentType apartmentType, bool renovatedInTheLastYear)
        {
            if (apartmentType.Equals(ApartmentType.Studio))
            {
                if (percentOfPositiveReviews > 85 && distanceFromTheBeach <= 750.0)
                {
                    if (renovatedInTheLastYear)
                        return ApartmentRank.First;
                    return ApartmentRank.Second;
                }
                return ApartmentRank.Third;
            }
            else if (apartmentType.Equals(ApartmentType.StudioWithTerrace))
            {
                if (percentOfPositiveReviews < 60 || distanceFromTheBeach >= 2000.0)
                    return ApartmentRank.Second;
                return ApartmentRank.First;
            }

            return ApartmentRank.Forth;
        }
    }
}
