using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Operations
{
    /*
        The goal of the table is to log every interaction of this module with services exposed by CSI.
        There are two available services:
            1) SearchVolontario: given a tax code of a first responder, returns an internal volter ID (anonymization of the fiscal code)
            2) InsertInterventiVolontari: communicate to Volter the beginning/end of an intervention made by a first responder
        Some information are duplicated
     */


    [Table("volter_operations")]
    public class Operation : AuditedEntity
    {
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public long PersonId { get; set; }

        [Column("Type")]
        public string TypeString
        {
            get { return Type.ToString(); }
            private set { Type = value.ParseEnum<VolterOperationType>(); }
        }
        [NotMapped]
        public VolterOperationType Type { get; set; }

        /// <summary>
        /// Anonymized Tax code of a first responder
        /// </summary>
        public int PersonLegacyId { get; set; }

        /// <summary>
        /// Internal volter ID for the operation.
        /// Duplication of OperationId inside Intervention class.
        /// </summary>
        
        public int OperationLegacyId { get; set; }


        [Column(TypeName = "jsonb")]
        public string Request { get; set; }

        [Column(TypeName = "jsonb")]
        public VolterResponse Response { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class VolterResponse
    {
        public string DescriptionOutcome { get; set; }
        public int VolterId { get; set; }
        public string ProcessedCode { get; set; }
        public ProcessedCodeType ProcessedCodeTypeEnum
        {
            get
            {
                return ProcessedCode switch
                {
                    "0001" => ProcessedCodeType.ElaborazioneTerminataCorretamente,
                    "0005" => ProcessedCodeType.CampoObbligatorioNonPresente,
                    "0010" => ProcessedCodeType.ErroreParametriInput,
                    "0022" => ProcessedCodeType.SistemaDiMateriaChiamanteNonEsistente,
                    "0030" => ProcessedCodeType.VolontarioNonEsistente,
                    "0031" => ProcessedCodeType.CompitoNonEsistente,
                    "0033" => ProcessedCodeType.InsertOUpdateFallito,
                    "0038" => ProcessedCodeType.EventoNonEsistente,
                    "0039" => ProcessedCodeType.TrovatiPiuInterventiAperti,
                    _ => ProcessedCodeType.SistemaDiMateriaChiamanteNonEsistente,
                };
            }
        }
    }

    public interface IVolterAction
    {
        public string subjectCode { get; set; }
    }

    public class Registration: IVolterAction
    {
        public string subjectCode { get; set; }
        public string fiscalCodeVoluntary { get; set; }
    }

    public class Intervention : IVolterAction
    {
        public string subjectCode { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string volterID { get; set; }
        public string voluntaryActivity { get; set; }
        public DateTime missionDate { get; set; }
        public string status { get; set; }
        public string operationId { get; set; }
    }
}
