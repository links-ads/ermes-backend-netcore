﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Gamification;
using Ermes.Organizations;
using Ermes.Reports;
using Ermes.Teams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Persons
{
    [Table("persons")]
    public class Person : AuditedEntity<long>, IPersonBase, IPassivable
    {
        private const int MaxUsernameLength = 255;
        private const int MaxEmailLength = 255;
        /// <summary>
        /// UserId assign to the user by Fusion Auth app
        /// </summary>
        [Required]
        public Guid FusionAuthUserGuid { get; set; }

        /// <summary>
        /// Reference to organization
        /// </summary>
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public virtual int? OrganizationId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
        public virtual int? TeamId { get; set; }

        [StringLength(MaxUsernameLength)]
        public string Username { get; set; }

        /// <summary>
        /// Token for push notification (i.e. Firebase)
        /// </summary>
        public string RegistrationToken { get; set; }

        /// <summary>
        ///True if it the first time a user logs in --> show tutorial on Chatbot
        /// </summary>
        public bool IsFirstLogin { get; set; } = true;

        public int? LegacyId { get; set; }

        /// <summary>
        /// If true, User has just fill the registration form on FusionAuth and needs to complete registration procedure on Chatbot.
        /// </summary>
        public bool IsNewUser { get; set; }

        /// <summary>
        /// It represents the internal Volter Id for an operation made by a first responder.
        /// This field has a value only for member of organization "Protezione Civile Piemonte" or its children.
        /// </summary>
        public int? CurrentOperationLegacyId { get; set; }

        public virtual ICollection<PersonTip> Tips { get; set; }
        public virtual ICollection<PersonQuiz> Quizzes { get; set; }

        public int Points { get; set; }

        [ForeignKey("LevelId")]
        public virtual Level Level { get; set; }
        public int? LevelId { get; set; }

        [StringLength(MaxEmailLength)]
        public string Email { get; set; }

        public virtual ICollection<ReportValidation> ReportValidations { get; set; }

        /// <summary>
        /// Better IsActive than SoftDeleted, because even if the user cannot access the platform,
        /// we want to show his position and his reports on the frontend
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
