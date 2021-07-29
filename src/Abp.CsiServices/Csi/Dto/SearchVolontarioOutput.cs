using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.CsiServices.Csi.Dto
{
    public class SearchVolontarioOutput
    {
        public int codEsitoElaborazione { get; set; }
        public EsitoElaborazioneType EsiteElaborazione
        {
            get {
                return (EsitoElaborazioneType)codEsitoElaborazione;
            }
        }
        public string descrEsitoElaborazione { get; set; }
        public int IDpersona { get; set; }
        
        public enum EsitoElaborazioneType
        {
            ElaborazioneTerminataCorretamente = 1,
            CampoObbligatorioNonPresente = 5,
            ErroreParametriInput = 10,
            SistemaDiMateriaChiamanteNonEsistente = 22,
            VolontarioNonEsistente = 30
        }
    }
}
