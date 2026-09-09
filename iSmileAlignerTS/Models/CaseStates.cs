using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmileAlignerTS.Models
{
    public enum CaseStates
    {
        Planung = 1,
        PlanungBezahlt,
        FallPlanung1,
        FallPlanung2,
        FallPlanung3,
        Herstellung,
        HerstellungBezahlt,
        Versand,
        FallAbgeschlossen
    }

    public enum CaseStripping
    {
        Ja = 1,
        Nein,
        WennNotwendig
    }
}