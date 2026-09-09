using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iSmileAlignerTS.Models
{
    public enum CaseFileType
    {
        Portrait = 1,
        PortraitSmiling = 2,
        Profil = 3,
        ProfilSmiling = 4,

        ModelBeforeFront = 20,
        ModelAfterFront = 21,
        ModelBeforeRight = 22,
        ModelAfterRight = 23,
        ModelMaxView = 24,
        ModelMandView = 25,
        ModelBeforeOKUK = 26,
        ModelAfterOKUK = 27,

        DataPicture = 30,

        PanoXRay = 40,
        DistXRay = 41,

        AnyPhoto = 50,
        AnyModelOK = 51,
        AnyModelUK = 52,

        SltModel = 60,
        SltModelOK = 61,
        SltModelUK = 62,
        SltModelOKNow = 63,
        SltModelUKNow = 64,
        SltModelOKUK = 65,
        SltModelOKUKNow = 66,

        AnimationGifFront = 70,
        AnimationGifOccl = 71,
        AnimationGifSide = 72,

        PDFFile = 100,
        PDFRechung= 101,
        AnyPDFFile = 102,
        P3szFile = 103,
        AnyFile = 104,

        WarePic = 110,

        ProjectSlt = 120,
        ProjectObj = 121,
    }
}