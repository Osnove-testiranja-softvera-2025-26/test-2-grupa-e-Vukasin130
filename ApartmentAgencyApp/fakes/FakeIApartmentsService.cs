using ApartmentAgencyApp.Models;
using ApartmentAgencyApp.Services;
using System.Collections.Generic;

public class FakeIApartmentsService : IApartmentService
{
    public List<Apartment> _availableApartments;

    public FakeIApartmentsService(List<Apartment> availableApartments)
    {
        _availableApartments = availableApartments;
    }

    public List<Apartment> GetAvailableApartments(ReservationRequest request)
    {
        return _availableApartments;
    }
}
