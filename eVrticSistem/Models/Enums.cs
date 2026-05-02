namespace EVrtic.Models
{
    public enum Uloga
    {
        RODITELJ,
        ODGAJATELJ,
        ADMINISTRATOR
    }

    public enum StatusNaloga
    {
        AKTIVAN,
        DEAKTIVIRAN
    }

    public enum StatusObroka
    {
        NIJE_POJEDENO,
        DJELIMICNO_POJEDENO,
        POTPUNO_POJEDENO
    }

    public enum TipDogadjaja
    {
        DOLAZAK,
        ODLAZAK
    }

    public enum StatusEvidencije
    {
        EVIDENTIRANO,
        ODBIJENO
    }

    public enum TipSazetka
    {
        SEDMICNI,
        MJESECNI
    }

    public enum KanalSlanja
    {
        APLIKACIJA,
        EMAIL
    }

    public enum StatusObavijesti
    {
        KREIRANA,
        POSLANA,
        NEUSPJELA,
        PROCITANA
    }
}