using LocationVoitures.ApiService.Services;

namespace LocationVoitures.Tests;

public class LocationServiceTests
{
    private LocationService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = new LocationService();
    }

    [Test]
    public void CalculerPrixTotal_ShouldReturn360_For120EurosOver3Days()
    {
        var prix = _service.CalculerPrixTotal(
            120m,
            new DateOnly(2026, 5, 10),
            new DateOnly(2026, 5, 12));

        Assert.That(prix, Is.EqualTo(360m));
    }

    [Test]
    public void CalculerPrixTotal_ShouldChargeOneDay_WhenDateFinEqualsDateDebut()
    {
        var prix = _service.CalculerPrixTotal(
            80m,
            new DateOnly(2026, 5, 10),
            new DateOnly(2026, 5, 10));

        Assert.That(prix, Is.EqualTo(80m));
    }

    [Test]
    public void CalculerPrixTotal_ShouldHandleYearBoundary()
    {
        var prix = _service.CalculerPrixTotal(
            100m,
            new DateOnly(2026, 12, 31),
            new DateOnly(2027, 1, 2));

        Assert.That(prix, Is.EqualTo(300m));
    }

    [Test]
    public void ValiderPeriode_ShouldThrow_WhenDateFinIsBeforeDateDebut()
    {
        var action = () => _service.ValiderPeriode(
            new DateOnly(2026, 5, 12),
            new DateOnly(2026, 5, 10));

        Assert.Throws<ArgumentException>(() => action());
    }
}
