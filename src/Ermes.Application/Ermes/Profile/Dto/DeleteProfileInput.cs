using System;

namespace Ermes.Profile.Dto
{
    public class DeleteProfileInput
    {
        public Guid Id { get; set; }
        public bool HardDelete { get; set; }
    }
}
