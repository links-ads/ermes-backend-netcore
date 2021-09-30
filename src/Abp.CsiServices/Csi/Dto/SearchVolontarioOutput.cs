using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.CsiServices.Csi.Dto
{
    public class SearchVolontarioOutput
    {
        public string ProcessedCode { get; set; }
        public ProcessedCodeType ProcessedCodeTypeEnum
        {
            get {
                return ProcessedCode switch
                {
                    "0001" => ProcessedCodeType.ElaborazioneTerminataCorretamente,
                    "0005" => ProcessedCodeType.CampoObbligatorioNonPresente,
                    "0010" => ProcessedCodeType.ErroreParametriInput,
                    "0022" => ProcessedCodeType.SistemaDiMateriaChiamanteNonEsistente,
                    "0030" => ProcessedCodeType.VolontarioNonEsistente,
                    _ => ProcessedCodeType.SistemaDiMateriaChiamanteNonEsistente,
                };
            }
        }
        public string DescriptionOutcome { get; set; }
        public int VolterId { get; set; }
        
        public enum ProcessedCodeType
        {
            ElaborazioneTerminataCorretamente = 1,
            CampoObbligatorioNonPresente = 5,
            ErroreParametriInput = 10,
            SistemaDiMateriaChiamanteNonEsistente = 22,
            VolontarioNonEsistente = 30
        }
    }
}
