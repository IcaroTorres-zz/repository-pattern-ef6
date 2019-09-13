using System;

namespace Stuart.Domain
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }

        // Audit Fields
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }

        // logic exclusion control flag
        bool Disabled { get; set; }
    }
}