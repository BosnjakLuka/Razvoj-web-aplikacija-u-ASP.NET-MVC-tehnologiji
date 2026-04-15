using planinarenje.Entiteti;

namespace planinarenje.Repositories;

public interface ILab1DataStore
{
    Lab1Podaci Data { get; }
}

public sealed class Lab1DataStore : ILab1DataStore
{
    public Lab1Podaci Data { get; } = Lab1PodaciFactory.Kreiraj();
}

public interface IPodrucjeMockRepository
{
    IReadOnlyList<Podrucje> GetAll();
    Podrucje? GetById(int id);
}

public interface IKorisnikMockRepository
{
    IReadOnlyList<Korisnik> GetAll();
    Korisnik? GetById(int id);
}

public interface IKnjizicaMockRepository
{
    IReadOnlyList<Knjizica> GetAll();
    Knjizica? GetById(int id);
}

public interface IMedaljaMockRepository
{
    IReadOnlyList<Medalja> GetAll();
    Medalja? GetById(int id);
}

public interface IPlaninarskaUdrugaMockRepository
{
    IReadOnlyList<PlaninarskaUdruga> GetAll();
    PlaninarskaUdruga? GetById(int id);
}

public interface IPlaninarskiObjektMockRepository
{
    IReadOnlyList<PlaninarskiObjekt> GetAll();
    PlaninarskiObjekt? GetById(int id);
}

public interface IKontrolnaTockaMockRepository
{
    IReadOnlyList<KontrolnaTocka> GetAll();
    KontrolnaTocka? GetById(int id);
}

public interface IRutaMockRepository
{
    IReadOnlyList<Ruta> GetAll();
    Ruta? GetById(int id);
}

public interface IPosjetMockRepository
{
    IReadOnlyList<Posjet> GetAll();
    Posjet? GetById(int id);
}

public interface IFotografijaMockRepository
{
    IReadOnlyList<Fotografija> GetAll();
    Fotografija? GetById(int id);
}

public interface IKorisnikMedaljaMockRepository
{
    IReadOnlyList<KorisnikMedalja> GetAll();
    KorisnikMedalja? GetById(int id);
}

public sealed class PodrucjeMockRepository(ILab1DataStore store) : IPodrucjeMockRepository
{
    public IReadOnlyList<Podrucje> GetAll() => store.Data.Podrucja;
    public Podrucje? GetById(int id) => store.Data.Podrucja.FirstOrDefault(x => x.IdPodrucje == id);
}

public sealed class KorisnikMockRepository(ILab1DataStore store) : IKorisnikMockRepository
{
    public IReadOnlyList<Korisnik> GetAll() => store.Data.Korisnici;
    public Korisnik? GetById(int id) => store.Data.Korisnici.FirstOrDefault(x => x.IdKorisnik == id);
}

public sealed class KnjizicaMockRepository(ILab1DataStore store) : IKnjizicaMockRepository
{
    public IReadOnlyList<Knjizica> GetAll() => store.Data.Knjizice;
    public Knjizica? GetById(int id) => store.Data.Knjizice.FirstOrDefault(x => x.IdKnjizica == id);
}

public sealed class MedaljaMockRepository(ILab1DataStore store) : IMedaljaMockRepository
{
    public IReadOnlyList<Medalja> GetAll() => store.Data.Medalje;
    public Medalja? GetById(int id) => store.Data.Medalje.FirstOrDefault(x => x.IdMedalja == id);
}

public sealed class PlaninarskaUdrugaMockRepository(ILab1DataStore store) : IPlaninarskaUdrugaMockRepository
{
    public IReadOnlyList<PlaninarskaUdruga> GetAll() => store.Data.PlaninarskeUdruge;
    public PlaninarskaUdruga? GetById(int id) => store.Data.PlaninarskeUdruge.FirstOrDefault(x => x.IdPlaninarskaUdruga == id);
}

public sealed class PlaninarskiObjektMockRepository(ILab1DataStore store) : IPlaninarskiObjektMockRepository
{
    public IReadOnlyList<PlaninarskiObjekt> GetAll() => store.Data.PlaninarskiObjekti;
    public PlaninarskiObjekt? GetById(int id) => store.Data.PlaninarskiObjekti.FirstOrDefault(x => x.IdPlaninarskiObjekt == id);
}

public sealed class KontrolnaTockaMockRepository(ILab1DataStore store) : IKontrolnaTockaMockRepository
{
    public IReadOnlyList<KontrolnaTocka> GetAll() => store.Data.KontrolneTocke;
    public KontrolnaTocka? GetById(int id) => store.Data.KontrolneTocke.FirstOrDefault(x => x.IdKontrolnaTocka == id);
}

public sealed class RutaMockRepository(ILab1DataStore store) : IRutaMockRepository
{
    public IReadOnlyList<Ruta> GetAll() => store.Data.Rute;
    public Ruta? GetById(int id) => store.Data.Rute.FirstOrDefault(x => x.IdRuta == id);
}

public sealed class PosjetMockRepository(ILab1DataStore store) : IPosjetMockRepository
{
    public IReadOnlyList<Posjet> GetAll() => store.Data.Posjeti;
    public Posjet? GetById(int id) => store.Data.Posjeti.FirstOrDefault(x => x.IdPosjet == id);
}

public sealed class FotografijaMockRepository(ILab1DataStore store) : IFotografijaMockRepository
{
    public IReadOnlyList<Fotografija> GetAll() => store.Data.Fotografije;
    public Fotografija? GetById(int id) => store.Data.Fotografije.FirstOrDefault(x => x.IdFotografija == id);
}

public sealed class KorisnikMedaljaMockRepository(ILab1DataStore store) : IKorisnikMedaljaMockRepository
{
    public IReadOnlyList<KorisnikMedalja> GetAll() => store.Data.KorisnikMedalje;
    public KorisnikMedalja? GetById(int id) => store.Data.KorisnikMedalje.FirstOrDefault(x => x.IdKorisnikMedalja == id);
}
