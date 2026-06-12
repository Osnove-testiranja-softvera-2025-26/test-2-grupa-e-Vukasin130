using ApartmentAgencyApp.Exceptions;
using ApartmentAgencyApp.Fakes;
using ApartmentAgencyApp.Models;
using ApartmentAgencyApp.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using NSubstitute;



namespace ApartmentAgencyApp.Test
{

//example of Guid: 00000000-0000-0000-0000-000000000001

    [TestFixture]
    public class ApartmentAgencyServiceTest
    {
        private FakeIApartmentsService _apartmentService;
        private FakeIDataCalculationService _dateCalculationService;
        private FakeIReservationService _reservationService;

        private ApartmentAgencyService _apartmentAgencyService;

        /// <summary>

        /// Test slučajevi se učitavaju iz fajla PictResults.txt.
        /// </summary>
        [TestCaseSource(typeof(PictParser), nameof(PictParser.GetTestCases))]
        public void CalculateApartmentRank_ReturnsExpectedResult(double distanceFromBeach, int percentOfPositiveReviews, ApartmentType apartmentType, bool renovatedInTheLastYear, ApartmentRank expectedRank)

        {
        ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear, expectedRank);



            Assert.That(result, Is.EqualTo(expectedRank));
        }
        [SetUp]
        public void SetUp()
        {
            _apartmentService = new FakeIApartmentsService(
                new List<Apartment>
                {
                    new Apartment
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
                    }
                });

            _dateCalculationService = new FakeIDataCalculationService(new RequestDaysInfo
            {
                NumberOfDays = 10,
                NumberOfSeasonDays = 3
            });

            _reservationService = new FakeIReservationService();

            _apartmentAgencyService = new ApartmentAgencyService(
                _dateCalculationService,
                _apartmentService,
                _reservationService);
        }
        /// <summary>
        /// Za BedOnly apartman proverava se slucaj kada je udaljenost manja od 500
        /// Broj Kreveta nam je 3
        /// Ocekuje se ComplexA.
        /// </summary>
        [Test]
        public void MakeApartmentReservation_BedOnlyCloseToBeachWithThreeBeds_ReturnsComplexA()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DistanceFromTheBeach = 400,
                NumberOfBeds = 3,
                ApartmentType = ApartmentType.BedOnly
            };

            ApartmentComplex expectedComplex = ApartmentComplex.ComplexA;
            _apartmentAgencyService.MakeApartmentReservation(request);
            ApartmentComplex result = _reservationService.Reservations.ApartmentComplex;

            Assert.That(result, Is.EqualTo(expectedComplex));
        }
        ///<summary>
        ///Za BedOnlt apartman provera se slucaj kada je udaljenost manja od 500
        ///Ali nam je sad broj kreveta manja od 3 stavicemo 2
        ///Ocekuje se ComplexB.
        ///</summary>
        [Test]
        public void MakeApartmentReservation_BedOnlyCloseToBeachWithThreeBeds_ReturnsComplexA1()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DistanceFromTheBeach = 400,
                NumberOfBeds = 2,
                ApartmentType = ApartmentType.BedOnly
            };
            ApartmentComplex exectedComplex = ApartmentComplex.ComplexB;

            _apartmentAgencyService.MakeApartmentReservation(request);
            ApartmentComplex result = _reservationService.Reservations.ApartmentComplex;

            Assert.That(result, Is.EqualTo(exectedComplex));
        }
        /// <summary>
        /// Studio, broj dana je 12 ili vise
        /// Ocekuje se ComplexB.
        /// </summary>
        [Test]
        public void MakeApartmentReservation_Studio12OrMoreDays_ReturnsComplexB()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DistanceFromTheBeach = 1000,
                NumberOfBeds = 2,
                ApartmentType = ApartmentType.Studio
            };

            RequestDaysInfo requestDaysInfo = new RequestDaysInfo
            {
                NumberOfDays = 12,
                NumberOfSeasonDays = 1
            };

            _dateCalculationService = new FakeIDataCalculationService(requestDaysInfo);
            _apartmentAgencyService = new ApartmentAgencyService(
                _dateCalculationService,
                _apartmentService,
                _reservationService);

            ApartmentComplex expectedComplex = ApartmentComplex.ComplexB;

            _apartmentAgencyService.MakeApartmentReservation(request);
            ApartmentComplex result = _reservationService.Reservations[0].ApartmentComplex;

            Assert.That(result, Is.EqualTo(expectedComplex));
        }
        /// <summary>
        /// Studio, broj dana u lenjoj sezoni je 10 ili vise
        /// Ocekuje se ComplexB.
        /// </summary>
        [Test]
        public void MakeApartmentReservation_Studio12OrMoreDays_ReturnsComplexB2()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DistanceFromTheBeach = 1000,
                NumberOfBeds = 2,
                ApartmentType = ApartmentType.Studio
            };

            RequestDaysInfo requestDaysInfo = new RequestDaysInfo
            {
                NumberOfDays = 2,
                NumberOfSeasonDays = 11
            };

            _dateCalculationService = new FakeIDataCalculationService(requestDaysInfo);
            _apartmentAgencyService = new ApartmentAgencyService(
                _dateCalculationService,
                _apartmentService,
                _reservationService);

            ApartmentComplex expectedComplex = ApartmentComplex.ComplexB;

            _apartmentAgencyService.MakeApartmentReservation(request);
            ApartmentComplex result = _reservationService.Reservations[0].ApartmentComplex;

            Assert.That(result, Is.EqualTo(expectedComplex));
        }

        /// <summary>
        /// Studion, broj dana je manji od 13 i broj dana letnjoj sezoni je manji od 10
        /// Ocekujemo da bude izdat ComplexC
        /// </summary>
        [Test]
        public void MakeApartmentReservation_StudioLessTha13Days_ReturnsComplexC()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DistanceFromTheBeach = 1000,
                NumberOfBeds = 2,
                ApartmentType = ApartmentType.Studio
            };

            RequestDaysInfo requestDaysInfo = new RequestDaysInfo
            {
                NumberOfDays = 12,
                NumberOfSeasonDays = 9
            };

            _dateCalculationService = new FakeIDataCalculationService(requestDaysInfo);
            _apartmentAgencyService = new ApartmentAgencyService(
                _dateCalculationService,
                _apartmentService,
                _reservationService);

            ApartmentComplex expectedComplex = ApartmentComplex.ComplexC;

            _apartmentAgencyService.MakeApartmentReservation(request);
            ApartmentComplex result = _reservationService.Reservations[0].ApartmentComplex;

            Assert.That(result, Is.EqualTo(expectedComplex));
        }

        /// <summary>
        /// Tip apartmana nije BedOnly ni Studio, pa ulazi u ostale slucajeve.
        /// Ocekuje se ComplexD.
        /// </summary>
        [Test]
        public void MakeApartmentReservation_StudioWithTerrace_ReturnsComplexD()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DistanceFromTheBeach = 400,
                NumberOfBeds = 2,
                ApartmentType = ApartmentType.StudioWithTerrace
            };

            ApartmentComplex expectedComplex = ApartmentComplex.ComplexD;

            _apartmentAgencyService.MakeApartmentReservation(request);
            ApartmentComplex result = _reservationService.Reservations[0].ApartmentComplex;

            Assert.That(result, Is.EqualTo(expectedComplex));
        }
        /// <summary>
        /// Studio, procenat pozitivnih recenzija veći od 85, udaljenost od plaze je manja ili jendaka od 750,
        /// apartman je renoviran. Ocekuje se First.
        /// </summary>
        [Test]
        public void CalculateApartmentRank_StudioGoodReviewsCloseToBeachRenovated_ReturnsFirst()
        {
            double distanceFromTheBeach = 500;
            int percentOfPositiveReviews = 86;
            ApartmentType apartmentType = ApartmentType.Studio;
            bool renovatedInTheLastYear = true;

            ApartmentRank expectedRank = ApartmentRank.First;

            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(
                distanceFromTheBeach,
                percentOfPositiveReviews,
                apartmentType,
                renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }
        /// <summary>
        /// Studio, procenat pozitivnih recenzija veći od 85, udaljenost od plaze je manja ili jendaka od 750,
        /// apartman nije renoviran. Ocekuje se Second.
        /// </summary>
        [Test]
        public void CalculateApartmentRank_StudioGoodReviewsCloseToBeachNotRenovated_ReturnsSecond()
        {
            double distanceFromTheBeach = 500;
            int percentOfPositiveReviews = 86;
            ApartmentType apartmentType = ApartmentType.Studio;
            bool renovatedInTheLastYear = false;

            ApartmentRank expectedRank = ApartmentRank.Second;

            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromTheBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }
        /// <summary>
        /// Studio, procenat pozitivnih recenzija manja od 85, udaljenost od plaze je veca  od 750,
        /// apartman nije renoviran. Ocekuje se Third.
        /// </summary>
        [Test]
        public void CalculateApartmentRank_StudioGoodReviewsCloseToBeachNotRenovated_ReturnsThird()
        {
            double distanceFromTheBeach = 800;
            int percentOfPositiveReviews = 80;
            ApartmentType apartmentType = ApartmentType.Studio;
            bool renovatedInTheLastYear = false;

            ApartmentRank expectedRank = ApartmentRank.Third;

            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromTheBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }
        /// <summary>
        /// StudioWithTerrace, procenat pozitivnih recenzija je 60 ili manje.
        /// Ocekuje se Second.
        /// </summary>
        [Test]
        public void CalculateApartmentRank_StudioWithTerraceReviewsLessOrEqual60_ReturnsSecond()
        {
            double distanceFromTheBeach = 1000;
            int percentOfPositiveReviews = 60;
            ApartmentType apartmentType = ApartmentType.StudioWithTerrace;
            bool renovatedInTheLastYear = true;

            ApartmentRank expectedRank = ApartmentRank.Second;

            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(
                distanceFromTheBeach,
                percentOfPositiveReviews,
                apartmentType,
                renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }
        /// <summary>
        /// StudioWithTerrace, udaljenost od plaze je veća od 2000.
        /// Ocekuje se Second.
        /// </summary>
        [Test]
        public void CalculateApartmentRank_StudioWithTerraceFarFromBeach_ReturnsSecond()
        {

            double distanceFromTheBeach = 2001;
            int percentOfPositiveReviews = 90;
            ApartmentType apartmentType = ApartmentType.StudioWithTerrace;
            bool renovatedInTheLastYear = true;

            ApartmentRank expectedRank = ApartmentRank.Second;


            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromTheBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }
        /// <summary>
        /// StudioWithTerrace procenat pozitivnh ocena je veca od 60 i nije udaljen 2000 ili preko .
        /// Ocekuje se First.
        /// </summary>
        [Test]
        public void CalculateApartmentRank_StudioWithTerrace_ReturnsFirst()
        {
            double distanceFromTheBeach = 2001;
            int percentOfPositiveReviews = 90;
            ApartmentType apartmentType = ApartmentType.StudioWithTerrace;
            bool renovatedInTheLastYear = true;

            ApartmentRank expectedRank = ApartmentRank.First;


            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromTheBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }
        ///<summary>
        ///Ukoliko je apartman BedOnly on ulazi u ostale slucajeve
        ///Ocekujemo Forth
        ///</summary>
        [Test]
        public void CalculateApartmentRank_StudioBedOnly_ReturnsForth()
        {

            double distanceFromTheBeach = 2001;
            int percentOfPositiveReviews = 90;
            ApartmentType apartmentType = ApartmentType.BedOnly;
            bool renovatedInTheLastYear = true;

            ApartmentRank expectedRank = ApartmentRank.Forth;


            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromTheBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedRank));
        }


    }
}
