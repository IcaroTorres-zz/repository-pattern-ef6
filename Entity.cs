using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuart.Repository
{
    public abstract class Entity<TKey>
    {
        [Key, Index(IsUnique = true), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

        #region Audit Fields
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; internal set; } = DateTime.UtcNow;
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime ModifiedDate { get; internal set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        #endregion

        // logic control flag
        public bool Disabled { get; set; }
    }
}